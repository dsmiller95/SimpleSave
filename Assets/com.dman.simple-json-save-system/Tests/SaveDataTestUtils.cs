using Dman.Utilities;
using NUnit.Framework;

namespace Dman.SimpleJson.Tests
{
    public class SaveDataTestUtils
    {
        public static readonly string Namespace = "Dman.SimpleJson.Tests";
        public static readonly string Assembly = "com.dman.simple-json-save-system.tests";
        
        public static string GetSerializedToAndAssertRoundTrip(TokenMode tokenMode, params (string key, object data)[] datas)
        {
            string serializedString = SerializeToString(assertInternalRoundTrip: true, datas: datas, tokenMode: tokenMode);
            
            AssertExternalRoundTrip(datas, serializedString, tokenMode: tokenMode);

            return serializedString;
        }

        public static void AssertDeserializeWithoutError(
            string serializedString,
            TokenMode tokenMode,
            params (string key, object data)[] datas)
        {
            using var stringStore = StringStorePersistText.WithFiles(("tmp", serializedString));
            var persistor = PersistSaves.Create(stringStore);
            var loadedFile = persistor.LoadSave("tmp");
            if (loadedFile == null)
            {
                Assert.Fail("Failed to load file");
                return;
            }
            
            foreach (var (key, data) in datas)
            {
                Assert.IsTrue(loadedFile.TryGet(key, out var actualData, data.GetType(), tokenMode));
            }
        }
        
        private static void AssertExternalRoundTrip(
            (string key, object data)[] datas,
            string serializedString,
            TokenMode tokenMode)
        {
            using var stringStore = StringStorePersistText.WithFiles(("tmp", serializedString));
            var persistor = PersistSaves.Create(stringStore);
            var loadedFile = persistor.LoadSave("tmp");
            if (loadedFile == null)
            {
                Assert.Fail("Failed to load file");
                return;
            }
            
            foreach (var (key, data) in datas)
            {
                Assert.IsTrue(loadedFile.TryGet(key, out var actualData, data.GetType(), tokenMode));
                Assert.AreEqual(data, actualData);
            }
        }

        public static bool TryLoad<T>(string serializedString, string key, out T data, TokenMode tokenMode)
        {
            using var stringStore = StringStorePersistText.WithFiles(("tmp", serializedString));
            var persistor = PersistSaves.Create(stringStore);
            var loadedFile = persistor.LoadSave("tmp");
            if (loadedFile == null)
            {
                data = default;
                return false;
            }
            return loadedFile.TryGet(key, out data, tokenMode);
        }

        public static string SerializeToString(
            TokenMode tokenMode,
            bool assertInternalRoundTrip = true,
            params (string key, object data)[] datas)
        {
            using var stringStore = new StringStorePersistText();
            var persistor = PersistSaves.Create(stringStore);

            var saveData = persistor.CreateEmptySave();
            foreach (var (key, data) in datas)
            {
                saveData.Set(key, data, tokenMode);
            }
            
            persistor.PersistSave(saveData, "tmp");

            if (assertInternalRoundTrip)
            {
                // assert round-trip without re-load
                foreach (var (key, data) in datas)
                {
                    Assert.IsTrue(saveData.TryGet(key, out var actualData, data.GetType(), tokenMode));
                    Assert.AreEqual(data, actualData);
                }
            }
                
            return stringStore.ReadFrom("tmp")!.ReadToEnd();
        }

        public static void AssertMultilineStringEqual(string expected, string actual)
        {
            expected = expected.Trim();
            actual = actual.Trim();
            if (expected == actual) return;
            Assert.Fail(StringDiffUtils.StringEqualErrorMessage(expected, actual));
        }
    }
}