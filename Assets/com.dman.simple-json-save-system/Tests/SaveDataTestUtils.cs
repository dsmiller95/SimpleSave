﻿using Dman.Utilities;
using NUnit.Framework;

namespace Dman.SimpleJson.Tests
{
    public class SaveDataTestUtils
    {
        public static readonly string Namespace = "Dman.SimpleJson.Tests";
        public static readonly string Assembly = "com.dman.simple-json-save-system.tests";
        
        public static string GetSerializedToAndAssertRoundTrip(params (string key, object data)[] datas)
        {
            string serializedString = SerializeToString(assertInternalRoundTrip: true, datas: datas);
            
            AssertExternalRoundTrip(datas, serializedString);

            return serializedString;
        }

        public static void AssertDeserializeWithoutError(
            string serializedString,
            params (string key, object data)[] datas)
        {
            var context = "ContextName1";
            using var stringStore = StringStorePersistText.WithFiles((context, serializedString));
            var saveDataContextProvider = SaveDataContextProvider.CreateAndPersistTo(stringStore);
            saveDataContextProvider.LoadContext(context);
            var saveDataContext = saveDataContextProvider.GetContext(context);
            foreach (var (key, data) in datas)
            {
                Assert.IsTrue(saveDataContext.TryLoad(key, out var actualData, data.GetType()));
            }
        }
        
        private static void AssertExternalRoundTrip(
            (string key, object data)[] datas,
            string serializedString)
        {
            var context = "ContextName2";
            using var stringStore = StringStorePersistText.WithFiles((context, serializedString));
            var saveDataContextProvider = SaveDataContextProvider.CreateAndPersistTo(stringStore);
            saveDataContextProvider.LoadContext(context);
            var saveDataContext = saveDataContextProvider.GetContext(context);
            foreach (var (key, data) in datas)
            {
                Assert.IsTrue(saveDataContext.TryLoad(key, out var actualData, data.GetType()));
                Assert.AreEqual(data, actualData);
            }
        }

        public static bool TryLoad<T>(string serializedString, string key, out T data)
        {
            using var stringStore = StringStorePersistText.WithFiles(("tmp", serializedString));
            var saveDataContextProvider = SaveDataContextProvider.CreateAndPersistTo(stringStore);
            saveDataContextProvider.LoadContext("tmp");
            var saveDataContext = saveDataContextProvider.GetContext("tmp");
            return saveDataContext.TryLoad(key, out data);
        }

        public static string SerializeToString(
            bool assertInternalRoundTrip = true,
            params (string key, object data)[] datas)
        {
            var context = "ContextName3";
            using var stringStore = new StringStorePersistText();
            var saveDataContextProvider = SaveDataContextProvider.CreateAndPersistTo(stringStore);
            var saveDataContext = saveDataContextProvider.GetContext(context);
            foreach (var (key, data) in datas)
            {
                saveDataContext.Save(key, data);
            }
            saveDataContextProvider.PersistContext(context);

            if (assertInternalRoundTrip)
            {
                // assert round-trip without re-load
                foreach (var (key, data) in datas)
                {
                    Assert.IsTrue(saveDataContext.TryLoad(key, out var actualData, data.GetType()));
                    Assert.AreEqual(data, actualData);
                }
            }
                
            return stringStore.ReadFrom(context)!.ReadToEnd();
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