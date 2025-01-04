using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static Dman.SimpleJson.Tests.SaveDataTestUtils;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Dman.SimpleJson.Tests
{
    [Serializable]
    public class SerializableAnimal
    {

        [SerializeField] private string name;
        [SerializeField] private int age;

        // intentionally obfuscated constructor, ensures private fields cannot be populated from json via this constructor
        public SerializableAnimal(int choice)
        {
            switch (choice)
            {
                case 0:
                    name = "Borg";
                    age = 3000;
                    break;
                case 1:
                    name = "Fido";
                    age = 3;
                    break;
                case 2:
                    name = "Mr. green";
                    age = 6;
                    break;
                case 3:
                    name = "Eternal Crab";
                    age = 3;
                    break;
            }
        }

        public override string ToString()
        {
            return $"animal: {name}, {age}yrs";
        }

        protected bool Equals(SerializableAnimal other)
        {
            return name == other.name && age == other.age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableAnimal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((name != null ? name.GetHashCode() : 0) * 397) ^ age;
            }
        }
    }

    [Serializable]
    public class SerializableDog : SerializableAnimal
    {
        [SerializeField] private string taggedName;

        // intentionally obfuscated constructor, ensures private fields cannot be populated from json via this constructor
        public SerializableDog(int choice, string taggedName) : base(choice)
        {
            this.taggedName = taggedName;
        }
        
        public override string ToString()
        {
            return $"dog: {taggedName}, {base.ToString()}";
        }
        protected bool Equals(SerializableDog other)
        {
            return base.Equals(other) && taggedName == other.taggedName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableDog)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (taggedName != null ? taggedName.GetHashCode() : 0);
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
    [Serializable]
    public class SerializableCat : SerializableAnimal
    {
        [SerializeField] private Personality personality;

        // intentionally obfuscated constructor, ensures private fields cannot be populated from json via this constructor
        public SerializableCat(int choice, Personality personality) : base(choice)
        {
            this.personality = personality;
        }
        public override string ToString()
        {
            return $"cat: {personality}, {base.ToString()}";
        }
        
        protected bool Equals(SerializableCat other)
        {
            return base.Equals(other) && personality == other.personality;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableCat)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int)personality;
            }
        }
    }
    
    [Serializable]
    public class PartiallySerializableAnimal
    {
        [SerializeField] private string name;
        private int age;

        // intentionally obfuscated constructor, ensures private fields cannot be populated from json via this constructor
        public PartiallySerializableAnimal(int choice)
        {
            switch (choice)
            {
                case 0:
                    name = "Borg";
                    age = 3000;
                    break;
                case 1:
                    name = "Fido";
                    age = 3;
                    break;
                case 2:
                    name = "Mr. green";
                    age = 6;
                    break;
                case 3:
                    name = "Eternal Crab";
                    age = 3;
                    break;
                case 4:
                    name = "Eternal Crab";
                    age = 0;
                    break;
            }
        }
        
        protected bool Equals(PartiallySerializableAnimal other)
        {
            return name == other.name && age == other.age;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PartiallySerializableAnimal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((name != null ? name.GetHashCode() : 0) * 397) ^ age;
            }
        }

    }

    
    public class TestSaveDataWithSerializableAttributes
    {
        [Test]
        public void WhenSavedSerializableType_SavesPrivateSerializableFields()
        {
            // arrange
            var savedData = new SerializableDog(1, "Fido the Third");
            var expectedSavedString = @"
{
  ""dogg"": {
    ""name"": ""Fido"",
    ""age"": 3,
    ""taggedName"": ""Fido the Third""
  }
}
".Trim();
            // act
            var savedString = GetSerializedToAndAssertRoundTrip(TokenMode.SerializableObject, ("dogg", savedData));
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
        
        [Test]
        public void WhenSavedPartiallySerializableType_SavesPrivateSerializableFields()
        {
            // arrange
            var savedData = new PartiallySerializableAnimal(3);
            var expectedSavedString = @"
{
  ""creb"": {
    ""name"": ""Eternal Crab""
  }
}
".Trim();
            // act
            var savedString = SerializeToString(TokenMode.SerializableObject, assertInternalRoundTrip: false, ("creb", savedData));
            var loaded = TryLoad(savedString, "creb", out PartiallySerializableAnimal loadedData, TokenMode.SerializableObject);
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
            Assert.IsTrue(loaded);
            var expectedLoadedData = new PartiallySerializableAnimal(4);
            Assert.AreEqual(expectedLoadedData, loadedData);
        }
        
        [Test]
        public void WhenLoadsTypeDifferentSerializableType_ThanSaved_UnityAllowsAndDefaults()
        {
            // arrange
            var savedData = new SerializableDog(1, "Fido the Third");
            
            // act
            using var stringStore = new StringStorePersistText();

            var file = SaveData.Empty();
            file.Set("dogg", savedData, TokenMode.SerializableObject);
            
            
            stringStore.PersistSaveTo("test", file);
            file = stringStore.LoadSaveFrom("test");
            Assert.NotNull(file);
            
            var didLoad = file.TryGet("dogg", out SerializableCat cat, TokenMode.SerializableObject);
            
            // assert
            Assert.IsTrue(didLoad, "The load should succeed, because Unity");
            var expectedCat = new SerializableCat(1, Personality.Friendly);
            Assert.AreEqual(expectedCat, cat);
        }

        
        [Test]
        public void WhenSavedDifferentDerivedSerializableTypeDirectly_SavesNoMetadataInBoth()
        {
            // arrange
            var savedDog = new SerializableDog(1, "Fido the Third");
            var savedCat = new SerializableCat(2, Personality.Indifferent);
            var savedAnimal = new SerializableAnimal(0);
            var expectedSavedString = @"
{
  ""dogg"": {
    ""name"": ""Fido"",
    ""age"": 3,
    ""taggedName"": ""Fido the Third""
  },
  ""kitty"": {
    ""name"": ""Mr. green"",
    ""age"": 6,
    ""personality"": 2
  },
  ""???"": {
    ""name"": ""Borg"",
    ""age"": 3000
  }
}
".Trim();
            // act
            var savedString = GetSerializedToAndAssertRoundTrip(TokenMode.SerializableObject, 
                ("dogg", savedDog),
                ("kitty", savedCat),
                ("???", savedAnimal)
                );
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
        
        
        [Test]
        public void WhenSavedListOfDifferentDerivedSerializableTypeDirectly_CannotSerialize()
        {
            // arrange
            var savedData = new List<SerializableAnimal>
            {
                new SerializableDog(1, "Fido the Third"),
                new SerializableCat(2, Personality.Indifferent),
                new SerializableAnimal(0)
            };
            var expectedSavedString = @$"
{{
  ""zoo"": {{}}
}}
".Trim();
            // act
            var savedString = SerializeToString(TokenMode.SerializableObject, assertInternalRoundTrip: false, 
                ("zoo", savedData)
            );
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
        
        public class SerializableMonoBehavior : MonoBehaviour
        {
            public string data;
        }
        [Test]
        public void WhenSavedMonobehavior_DoesNotThrowException_SavesData()
        {
            // arrange
            var gameObject = new GameObject();
            var savedData = gameObject.AddComponent<SerializableMonoBehavior>();
            savedData.data = "hello";
            savedData.name = "can't save me";
            
            var expectedSavedString = @"
{
  ""mono"": {
    ""data"": ""hello""
  }
}
".Trim();
            
            // act
            var savedString = SerializeToString(TokenMode.SerializableObject, assertInternalRoundTrip: false, ("mono", savedData));
            
            // assert
            AssertMultilineStringEqual(expectedSavedString, savedString);
        }
    }
}
