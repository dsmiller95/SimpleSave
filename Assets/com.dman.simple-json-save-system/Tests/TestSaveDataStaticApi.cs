using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Dman.SimpleJson.Tests
{
    public class TestSaveDataStaticApi
    {
        [OneTimeSetUp]
        public void Init()
        {
            var settings = JsonSaveSystemSettings.Create("TestFolder", "rootTest");
            JsonSaveSystemSettings.ForceOverrideSettingsObject(settings, suppressWarningDangerously: true);
        }

        [SetUp]
        public void Setup()
        {
            SimpleSave.ChangeSaveFileToDefault();
        }
        
        [TearDown]
        public void CleanUp()
        {
            SimpleSave.FileSystem.DeleteAll();
            SimpleSave.DeleteAll();
        }
        
        [Test]
        public void WhenSetsString_CanGetString()
        {
            SimpleSave.SetString("testKey", "testValue");
            
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("testValue", result);
        }
        
        [Test]
        public void WhenSetsInt_CanGetInt()
        {
            SimpleSave.SetInt("testKey", 5);
            
            var result = SimpleSave.GetInt("testKey");
            
            Assert.AreEqual(5, result);
        }
        
        [Test]
        public void WhenSetsFloat_CanGetFloat()
        {
            SimpleSave.SetFloat("testKey", 4.2f);
            
            var result = SimpleSave.GetFloat("testKey");
            
            Assert.AreEqual(4.2f, result);
        }
        
        [Test]
        public void WhenSetsBool_CanGetBool()
        {
            SimpleSave.SetBool("testKey1", true);
            SimpleSave.SetBool("testKey2", false);
            
            Assert.AreEqual(SimpleSave.GetBool("testKey1", defaultValue: false), true);
            Assert.AreEqual(SimpleSave.GetBool("testKey2", defaultValue: true), false);
        }
        
        [Test]
        public void WhenSetsBool_CanGetString()
        {
            SimpleSave.SetBool("testKey", true);
            
            Assert.AreEqual("True", SimpleSave.GetString("testKey"));
        }

        [Test]
        public void WhenSetsString_HasKeySwitchesToTrue()
        {
            Assert.IsFalse(SimpleSave.HasKey("testKey"));
            
            SimpleSave.SetString("testKey", "testValue");
            Assert.IsTrue(SimpleSave.HasKey("testKey"));
            
            SimpleSave.DeleteKey("testKey");
            Assert.IsFalse(SimpleSave.HasKey("testKey"));
        }
        
        [Test]
        public void WhenSetsVector2_CanGetVector2()
        {
            SimpleSave.Set("testKey", new Vector2(1, 2));
            
            var result = SimpleSave.Get<Vector2>("testKey");
            
            Assert.AreEqual(new Vector2(1, 2), result);
        }
        
        [Test]
        public void WhenSetsVector2_CannotGetString()
        {
            SimpleSave.Set("testKey", new Vector2(1, 2));
            
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("", result);
            LogAssert.Expect(LogType.Error, new Regex("Failed to load data of type System.String for key testKey. Raw json"));
        }

        private enum CustomEnumA
        {
            None = 0,
            Value1,
            Value4,
        }

        private enum CustomEnumB
        {
            None = 0,
            Value2,
            Value3,
        }

        [Test]
        public void WhenSetsEnum_CanGetEnum_OrString()
        {
            SimpleSave.SetEnum("testKey", CustomEnumA.Value4);
            
            var result = SimpleSave.GetEnum<CustomEnumA>("testKey");
            var resultStr = SimpleSave.GetString("testKey");
            
            Assert.AreEqual(CustomEnumA.Value4, result);
            Assert.AreEqual("Value4", resultStr);
        }
        
        
        [Test]
        public void WhenSetsEnum_CannotGetDifferentEnum()
        {
            SimpleSave.SetEnum("testKey", CustomEnumA.Value4);
            
            var result = SimpleSave.GetEnum("testKey", defaultValue: CustomEnumB.None);
            
            Assert.AreEqual(CustomEnumB.None, result);
            
            LogAssert.Expect(LogType.Error, new Regex("Failed to load data of type [^ ]+CustomEnumB for key testKey. Raw json: Value4"));
        }
        
        
        [Test]
        public void WhenSetsInt_CanGetString()
        {
            SimpleSave.SetInt("testKey", 4);
            
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("4", result);
        }
        
        [Test]
        public void WhenSetsString_CanGetInt()
        {
            SimpleSave.SetString("testKey", "4");
            
            var result = SimpleSave.GetInt("testKey", 88);
            
            Assert.AreEqual(4, result);
        }
        
        
        [Test]
        public void WhenSetsString_ThenDeletesKey_CannotGetString()
        {
            SimpleSave.SetString("testKey", "testValue");
            
            SimpleSave.DeleteKey("testKey");
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("", result);
        }
        
        [Test]
        public void WhenSetsString_ThenDeletesAll_CannotGetString()
        {
            SimpleSave.SetString("testKey1", "testValue");
            SimpleSave.SetString("testKey2", "testValue");
            
            SimpleSave.DeleteAll();
            
            Assert.AreEqual("", SimpleSave.GetString("testKey1"));
            Assert.AreEqual("", SimpleSave.GetString("testKey2"));
        }
        
        [Test]
        public void WhenSetsString_ThenForceQuits_CannotGetString()
        {
            SimpleSave.SetString("testKey", "testValue");
            
            SimpleSave.EmulateForcedQuit();
            
            Assert.AreEqual("", SimpleSave.GetString("testKey"));
        }
        
        [Test]
        public void WhenSetsString_ThenManagedQuit_CanGetString()
        {
            SimpleSave.SetString("testKey", "testValue");
            
            SimpleSave.EmulateManagedApplicationQuit();
            
            Assert.AreEqual("testValue", SimpleSave.GetString("testKey"));
        }
        
        [Test]
        public void WhenSetsString_ThenSaves_ThenClearsMemory_CanGetString()
        {
            SimpleSave.SetString("testKey", "testValue");
            
            SimpleSave.Save();
            SimpleSave.EmulateForcedQuit();
            
            Assert.AreEqual("testValue", SimpleSave.GetString("testKey"));
        }
        
        [Test]
        public void WhenSetsString_ThenSaves_ThenSetsString_ThenClearsMemory_CanGetFirstString()
        {
            SimpleSave.SetString("testKey", "testValue1");
            
            SimpleSave.Save();
            SimpleSave.SetString("testKey", "testValue2");
            SimpleSave.EmulateForcedQuit();
            
            Assert.AreEqual("testValue1", SimpleSave.GetString("testKey"));
        }
        
        [Test]
        public void WhenSetsString_ThenChangesSaveFile_CannotGetString()
        {
            SimpleSave.SetString("testKey123", "testValue");

            SimpleSave.ChangeSaveFile("newTestFile");
            
            Assert.AreEqual("", SimpleSave.GetString("testKey123"));
        }
        
        [Test]
        public void WhenSetsString_ThenChangesSaveFile_AndSetsString_CanGetStringsOnBoth()
        {
            SimpleSave.SetString("testKey", "testValue1");
            var previousSaveFile = SimpleSave.SaveFileName;
            
            SimpleSave.ChangeSaveFile("newTestFile");
            SimpleSave.SetString("testKey", "testValue2");
            
            SimpleSave.ChangeSaveFile(previousSaveFile);
            Assert.AreEqual("testValue1", SimpleSave.GetString("testKey"));
            
            SimpleSave.ChangeSaveFile("newTestFile");
            Assert.AreEqual("testValue2", SimpleSave.GetString("testKey"));
        }

        /// <summary>
        /// Write directly to the current save file, bypassing the save system
        /// </summary>
        private void SideWriteToFile(string fileContents)
        {
            var fullPath = SimpleSave.FullSaveFolderPath;
            var externalPersistence = FileSystemPersistence.CreateAtAbsoluteFolderPath(fullPath);
            using var writer = externalPersistence.WriteTo(SimpleSave.SaveFileName);
            writer.Write(fileContents);
            writer.Close();
        }
        
        [Test]
        public void WhenSetsString_ThenExternalWriteToFile_ThenRefresh_GetsStringWrittenToFile()
        {
            SimpleSave.SetString("testKey", "testValue1295");

            SideWriteToFile(@"{""testKey"":""testValueWrittenToFile""}");
            
            SimpleSave.Refresh();
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("testValueWrittenToFile", result);
        }
        
        [Test]
        public void WhenTryLoadFromMalformedFile_DoesNotLoad_LogsError()
        {
            SimpleSave.SetString("testKey", "testValue1295");

            SideWriteToFile(@"{{testKey"":""testValueWrittenToFile""}");
            
            SimpleSave.Refresh();
            var result = SimpleSave.GetString("testKey");
            
            Assert.AreEqual("testValue1295", result);

            LogAssert.Expect(LogType.Error, new Regex($@"Failed to load data for {SimpleSave.SaveFileName}\.json, malformed Json"));
            LogAssert.Expect(LogType.Exception, new Regex(@"JsonReaderException: Invalid property identifier character: \{"));
        }
    }
}