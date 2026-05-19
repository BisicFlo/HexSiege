using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class RenameTiles : EditorWindow {
    [MenuItem("Tools/Rename Tiles Sequentially")]
    public static void ShowWindow() {
        GetWindow<RenameTiles>("Rename Tiles");
    }

    private string tilesFolder = "Assets/Florian/Prefabs/GrassTilesV2"; // C:\Wkspaces\TowerDefense\Assets\Florian\Prefabs\GrassTilesV2
    private void OnGUI() {
        GUILayout.Label("Rename All Tiles Sequentially", EditorStyles.boldLabel);
        GUILayout.Space(10);

        tilesFolder = EditorGUILayout.TextField("Tiles Folder Path", tilesFolder);

        GUILayout.Space(10);
        if (GUILayout.Button("Rename to Tile 1, Tile 2, Tile 3...", GUILayout.Height(40))) {
            RenameAllTiles();
        }

        GUILayout.Space(5);
        EditorGUILayout.HelpBox("This will rename every prefab in the folder to Tile 1, Tile 2, etc.\n" +
                              "All references will be automatically updated.", MessageType.Info);
    }

    private void RenameAllTiles() {
        if (!Directory.Exists(tilesFolder)) {
            EditorUtility.DisplayDialog("Error", "Folder not found:\n" + tilesFolder, "OK");
            return;
        }

        // Get all prefabs in the folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { tilesFolder });
        var prefabPaths = guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToList();

        if (prefabPaths.Count == 0) {
            EditorUtility.DisplayDialog("Info", "No prefabs found in the folder.", "OK");
            return;
        }

        // Sort by current name (alphabetically)
        prefabPaths.Sort();

        int successCount = 0;

        for (int i = 0; i < prefabPaths.Count; i++) {
            string oldPath = prefabPaths[i];
            string newName = $"Tile {i + 1}";
            string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName + ".prefab");

            string result = AssetDatabase.RenameAsset(oldPath, newName);

            if (string.IsNullOrEmpty(result)) {
                successCount++;
                Debug.Log($"Renamed: {Path.GetFileNameWithoutExtension(oldPath)} ? {newName}");
            }
            else {
                Debug.LogError($"Failed to rename {oldPath}: {result}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success!",
            $"Renamed {successCount} prefabs to Tile 1, Tile 2, ...",
            "OK");
    }
}