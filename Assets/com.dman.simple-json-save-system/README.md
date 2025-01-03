# Player prefs inspired save system

Save data is organized into contexts, which are keyed by strings. This allows for saving and
loading into save slots, or saving and loading a "root" context.

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
private struct SomeData
{
    public int someInt;
    public string someString;
}

private void IncrementSomeDataInt(string atKey)
{
    var existing = SimpleSave.Get<SomeData>(atKey, defaultValue: new SomeData());
    existing.someInt++;
    SimpleSave.Set(atKey, existing);
}

private void Run()
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
    
    // to apply changes to the filesystem immediately, call Save.
    // Otherwise, changes will be saved on application exit.
    SimpleSave.Save();
}
```

By default, json will be saved to `{Application.persistentDataPath}/SaveContexts/Root.json` in the following format.
To configure the save location, create a save settings asset under the menu option SaveSystem/JsonSaveSystemSettings.

After invoking Run this is the contents of the save file:

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
