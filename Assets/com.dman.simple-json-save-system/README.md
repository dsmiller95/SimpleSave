# Player prefs inspired save system

Save data is organized into contexts, which are keyed by strings. This allows for saving and
loading into save slots, or saving and loading a "root" context.

## Simple usage example

This script will maintain a list of strings that have been seen, and save them to the save system.
The "seen" strings can be queried from outside, of managed with logic contained to this class.

```csharp
public class SomethingThatSavesData : MonoBehaviour
{
    private bool HasSeen(string thing)
    {
        var key = _saveKeyRoot + "_" + "set";
        var seenSet = SimpleSave.Get<List<string>>(key);
        if(seenSet != null)
        {
            return seenSet.Contains(thing);
        }

        return false;
    }

    private void SetSeen(string thing, bool seen = true)
    {
        var key = _saveKeyRoot + "_" + "set";
        var seenSet = SimpleSave.Get<List<string>>(key) ?? new List<string>();
        var containsThing = seenSet.Contains(thing);
        if (seen == containsThing) return;

        if ( seen) seenSet.Add(thing);
        if (!seen) seenSet.Remove(thing);
        
        SimpleSave.Set(key, seenSet);
    }
}
```

The json will be saved to `{Application.persistentDataPath}/SaveContexts/Root.json` in the following format.
The default location can be configured in the JsonSaveSystemSettings asset, created under SaveSystem/JsonSaveSystemSettings.
```json
{
  "SomethingThatSavesData_SeenStrings_set": [
    "thing1",
    "thing_Two",
    "another thingamajig"
  ]
}
```

# Examples

Taken from this test: [TestSaveDataExamples.cs](../../Tests/JsonSaveSystem/TestSaveDataExamples.cs#L92)

Given this object graph:

```csharp
new MovementStrategy
{
    MovementAffectors = new List<IAffectMovement>
    {
        new AddInputToVelocity
        {
            InputToVelocityMultiplier = 5
        },
        new DampenVelocity
        {
            DampingFactor = 1.1f
        },
        new AddVelocityToPosition()
    },
    Input = new MovementInputParams
    {
        axisInput = new Vector2(1, 0),
        deltaTime = 0.1f
    },
    State = new MovementState
    {
        CurrentPosition = new Vector2(2, 3.2f),
        CurrentVelocity = new Vector2(4, 5),
    },
};

// forces unity-style serialization: 
// public fields or fields marked w/ [SerializeField] are included
[Serializable]
public struct MovementInputParams
{
    [SerializeField] private Vector2 axisInput;
    public float deltaTime;
}
// Newtonsoft.Json serialization, only public settable properties
public class MovementState
{
    public Vector2 CurrentPosition { get; set; }
    public Vector2 CurrentVelocity { get; set; }
}

public interface IAffectMovement {
    public void AffectMovement(MovementState state, MovementInputParams input);
}
public class AddInputToVelocity : IAffectMovement {
    public float InputToVelocityMultiplier { get; set; }
    public void AffectMovement(){/*...*/}
}
public class DampenVelocity : IAffectMovement {
    public float DampingFactor { get; set; }
    public void AffectMovement(){/*...*/}
}
public class AddVelocityToPosition : IAffectMovement {
    public void AffectMovement(){/*...*/}
}

public class MovementStrategy {
    // polymorphic list will include specific type-identifying information in json
    public List<IAffectMovement> MovementAffectors { get; set; }
    public MovementInputParams Input { get; set; }
    public MovementState State { get; set; }
}
```

Serializes to this format:

```json
{
  "movementAffectors": [
    {
      "$type": "{Namespace}.AddInputToVelocity, {Assembly}",
      "inputToVelocityMultiplier": 5.0
    },
    {
      "$type": "{Namespace}.DampenVelocity, {Assembly}",
      "dampingFactor": 1.1
    },
    {
      "$type": "{Namespace}.AddVelocityToPosition, {Assembly}"
    }
  ],
  "input": {
    "axisInput": {"x":1.0,"y":0.0},
    "deltaTime": 0.1
  },
  "state": {
    // Unity primitives like Vector2 are delegated to Unity's JsonUtility
    "currentPosition": {"x":2.0,"y":3.200000047683716},
    "currentVelocity": {"x":4.0,"y":5.0}
  }
}
```
