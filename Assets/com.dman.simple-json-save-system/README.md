# Player prefs inspired json save system

A way to save data to a json file in a way which resembles the Unity PlayerPrefs API. Also supports unity json serializable types.

## Installation

Install the `com.dman.simple-json-save-system` package [from git url](https://docs.unity3d.com/6000.0/Documentation/Manual/upm-git.html):
```
https://github.com/dsmiller95/SimpleSave.git?path=Assets/com.dman.simple-json-save-system
```

Optionally, check out the provided Samples.

## Usage example

```csharp
void ReplaceFlagIf(string key, string newFlag, string existingEqualTo)
{
    var existing = SimpleSave.GetString(key);
    if (existing != existingEqualTo) return;
    
    SimpleSave.SetString(key, newFlag);
}

void Increment(string key)
{
    var existing = SimpleSave.GetInt(key, defaultValue: 0);
    SimpleSave.SetInt(key, existing);
}

[Serializable]
struct SomeData
{
    public int someInt;
    public string someString;
}

void IncrementSomeDataInt(string atKey)
{
    var existing = SimpleSave.Get<SomeData>(atKey, defaultValue: new SomeData());
    existing.someInt++;
    SimpleSave.Set(atKey, existing);
}

void Run()
{
    // refresh is used to account for changes made outside of the current application
    SimpleSave.Refresh();
    
    SimpleSave.DeleteAll();
    SimpleSave.SetString("flagKey", "RedFlag");
    ReplaceFlagIf("flagKey", "BlueFlag", "RedFlag");
    ReplaceFlagIf("flagKey", "Galf", "MissingFlag");
    
    Increment("intKey");
    Increment("intKey");
    Increment("intKey");

    var initialSomeData = new SomeData();
    initialSomeData.someString = "Hello from save";
    initialSomeData.someInt = 0;

    SimpleSave.Set("dataKey", initialSomeData);
    IncrementSomeDataInt("dataKey");
    IncrementSomeDataInt("dataKey");
    
    // To apply changes to the filesystem immediately, call Save.
    // Otherwise, changes will be saved on application exit.
    SimpleSave.Save();
}
```

By default, json will be saved to `{Application.persistentDataPath}/SaveContexts/Root.json` in the following format.
To configure the save location, create a save settings asset under the menu option SaveSystem/JsonSaveSystemSettings.

After invoking Run these are the contents of the save file:

```json
{
  "flagKey": "BlueFlag",
  "intKey": 3,
  "dataKey": {
    "someInt": 2,
    "someString": "Hello from save"
  }
}
```
