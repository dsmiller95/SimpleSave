using Dman.SimpleJson;
using UnityEngine;

public class WackySaveDataDemo : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Save"))
        {
            SimpleSave.Save();
        }
        if (GUILayout.Button("Load"))
        {
            SimpleSave.Refresh();
        }
        if (GUILayout.Button("Open Save Directory"))
        {
            Application.OpenURL(SimpleSave.FullSaveFolderPath);
        }
            
        GUILayout.Label("EntryOne");
        var entryOne = SimpleSave.GetString("EntryOne");
        var newEntryOne = GUILayout.TextField(entryOne);
        if (newEntryOne != entryOne)
        {
            SimpleSave.SetString("EntryOne", newEntryOne);
        }
            
        GUILayout.Label("EntryTwo");
        var entryTwo = SimpleSave.GetFloat("EntryTwo");
        var newEntryTwo = GUILayout.HorizontalSlider(entryTwo, 0, 100);
        if (!Mathf.Approximately(newEntryTwo, entryTwo))
        {
            SimpleSave.SetFloat("EntryTwo", newEntryTwo);
        }
            
        GUILayout.Label("EntryThree");
        var entryThree = SimpleSave.GetInt("EntryThree");
        var newEntryThree = (int)GUILayout.HorizontalSlider(entryThree, 0, 10);
        if (newEntryThree != entryThree)
        {
            SimpleSave.SetInt("EntryThree", newEntryThree);
        }
    }
}