using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Dman.SimpleJson.Tests.SaveDataTestUtils;

namespace Dman.SimpleJson.Tests
{
    public class Animal : IEquatable<Animal>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public bool Equals(Animal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Age == other.Age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Animal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
            }
        }
    }

    public class Dog : Animal, IEquatable<Dog>
    {
        public string TaggedName { get; set; }

        public bool Equals(Dog other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && TaggedName == other.TaggedName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Dog)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (TaggedName != null ? TaggedName.GetHashCode() : 0);
            }
        }

    }

    [TypeConverter(typeof(EnumConverter))]
    public enum Personality
    {
        Friendly,
        Aggressive,
        Indifferent
    }
    public class Cat : Animal, IEquatable<Cat>
    {
        public Personality Personality { get; set; }
        
        public bool Equals(Cat other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Personality == other.Personality;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Cat)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int)Personality;
            }
        }

    }
    
    public class Zoo : IEquatable<Zoo>
    {
        public string Name { get; set; }
        public List<Animal> Animals { get; set; }
        
        public bool Equals(Zoo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Animals.SequenceEqual(other.Animals);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Zoo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Animals != null ? Animals.GetHashCode() : 0);
            }
        }
    }
    
    public class TestSaveDataRoundTrip
    {
        [Test]
        public void WhenSavedDerivedTypeDirectly_SavesNoMetadata()
        {
            // arrange
            var savedData = new Dog
            {
                Name = "Fido",
                Age = 3,
                TaggedName = "Fido the Third"
            };
            var expectedSavedString = @"
{
  ""dogg"": {
    ""taggedName"": ""Fido the Third"",
    ""name"": ""Fido"",
    ""age"": 3
  }
}
".Trim();
            // act
            var savedString = GetSerializedToAndAssertRoundTrip(TokenMode.Newtonsoft, ("dogg", savedData));
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
        
        [Test]
        public void WhenLoadsTypeDifferentThanSaved_LogsError()
        {
            // arrange
            var savedData = new Dog
            {
                Name = "Fido",
                Age = 3,
                TaggedName = "Fido the Third"
            };
            
            // act
            using var stringStore = new StringStorePersistText();
            var persistor = PersistSaves.Create(stringStore);

            var file = persistor.CreateEmptySave();
            file.Set("dogg", savedData, TokenMode.Newtonsoft);
            
            persistor.PersistSave(file, "test");
            file = persistor.LoadSave("test");
            Assert.NotNull(file);
            
            var didLoad = file.TryGet("dogg", out Cat _, TokenMode.Newtonsoft);
            
            // assert
            Assert.IsFalse(didLoad, "The load should fail");
            LogAssert.Expect(LogType.Error, new Regex(@$"Failed to load data of type {Namespace}\.Cat for key dogg\. Raw json"));
        }

        
        [Test]
        public void WhenSavedDifferentDerivedTypeDirectly_SavesNoMetadataInBoth()
        {
            // arrange
            var savedDog = new Dog
            {
                Name = "Fido",
                Age = 3,
                TaggedName = "Fido the Third"
            };
            var savedCat = new Cat
            {
                Name = "Mr. green",
                Age = 6,
                Personality = Personality.Indifferent
            };
            var savedAnimal = new Animal
            {
                Name = "Borg",
                Age = 3000
            };
            var expectedSavedString = @"
{
  ""dogg"": {
    ""taggedName"": ""Fido the Third"",
    ""name"": ""Fido"",
    ""age"": 3
  },
  ""kitty"": {
    ""personality"": ""Indifferent"",
    ""name"": ""Mr. green"",
    ""age"": 6
  },
  ""???"": {
    ""name"": ""Borg"",
    ""age"": 3000
  }
}
".Trim();
            // act
            var savedString = GetSerializedToAndAssertRoundTrip(TokenMode.Newtonsoft, 
                ("dogg", savedDog),
                ("kitty", savedCat),
                ("???", savedAnimal)
                );
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
        
        
        [Test]
        public void WhenSavedListOfDifferentDerivedTypeDirectly_SavesAllMetadataNeeded()
        {
            // arrange
            var savedData = new Zoo
            {
                Name = "Chaos zoo",
                Animals = new List<Animal>
                {
                    new Dog
                    {
                        Name = "Fido",
                        Age = 3,
                        TaggedName = "Fido the Third"
                    },
                    new Cat
                    {
                        Name = "Mr. green",
                        Age = 6,
                        Personality = Personality.Indifferent
                    },
                    new Animal
                    {
                        Name = "Borg",
                        Age = 3000
                    }
                }
            };
            var expectedSavedString = @$"
{{
  ""zoo"": {{
    ""name"": ""Chaos zoo"",
    ""animals"": [
      {{
        ""$type"": ""{Namespace}.Dog, {Assembly}"",
        ""taggedName"": ""Fido the Third"",
        ""name"": ""Fido"",
        ""age"": 3
      }},
      {{
        ""$type"": ""{Namespace}.Cat, {Assembly}"",
        ""personality"": ""Indifferent"",
        ""name"": ""Mr. green"",
        ""age"": 6
      }},
      {{
        ""name"": ""Borg"",
        ""age"": 3000
      }}
    ]
  }}
}}
".Trim();
            // act
            var savedString = GetSerializedToAndAssertRoundTrip(TokenMode.Newtonsoft, 
                ("zoo", savedData)
            );
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
    }
}
