using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shortcuts : MonoBehaviour {
    
    [MenuItem("Tools/Toggle Inspector Lock %l")] // Ctrl + L
    public static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
    
    
    // [MenuItem ("Tools/Select First Agent %1")]
    // static void SelectSceneObject ()
    // {
        // var obj = FindObjectOfType<Character>();
        // var obj = AssetDatabase.LoadAssetAtPath<GameConfig> ("Assets/Data/GameConfig.asset");
        // Selection.activeObject = obj;
    // }
    
    // [MenuItem ("Tools/Select First Bot %2")]
    // static void SelectSceneObject2 ()
    // {
        // var obj = FindObjectOfType<Bot>();
        // var obj = AssetDatabase.LoadAssetAtPath<GameConfig> ("Assets/Data/GameConfig.asset");
        // Selection.activeObject = obj;
    // }
}