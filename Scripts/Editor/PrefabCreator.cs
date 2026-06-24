using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to batch-create enemy prefabs.
///
/// HOW TO USE:
///   1. Place this script in any Editor/ folder in your project.
///   2. Open via Tools > Enemy Prefab Batch Creator.
///   3. Fill in all four fields and click "Generate Enemy Prefabs".
///
/// WHAT IT DOES:
///   For each direct child of [Visual Source Parent]:
///     - Instantiates a copy of [Base Enemy Prefab]
///     - Parents the visual copy under the "Offset" child
///     - Finds the Enemy component on the root and assigns the matching
///       EnemyData ScriptableObject (matched by name) from [EnemyData Folder]
///     - Saves the result as [VisualName].prefab in [Output Folder]
/// </summary>
public class EnemyPrefabBatchCreator : EditorWindow {
    // ?? Inspector fields ??????????????????????????????????????????????????????

    /// <summary>Parent GameObject whose direct children are the visual meshes.</summary>
    private GameObject visualSourceParent;

    /// <summary>Premade base prefab with an Enemy component on the root and an "Offset" child.</summary>
    private GameObject baseEnemyPrefab;

    /// <summary>Project-relative folder containing EnemyData ScriptableObject assets.</summary>
    private string enemyDataFolder = "Assets/Data/Enemies";

    /// <summary>Project-relative folder where generated prefabs will be saved.</summary>
    private string outputFolder = "Assets/Prefabs/Enemies";

    // ?? Window ????????????????????????????????????????????????????????????????

    [MenuItem("Tools/Enemy Prefab Batch Creator")]
    public static void OpenWindow() =>
        GetWindow<EnemyPrefabBatchCreator>("Enemy Prefab Batch Creator");

    private void OnGUI() {
        GUILayout.Label("Enemy Prefab Batch Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        visualSourceParent = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Visual Source Parent",
                "Parent whose children are the visual GameObjects to process."),
            visualSourceParent, typeof(GameObject), true);

        baseEnemyPrefab = (GameObject)EditorGUILayout.ObjectField(
            new GUIContent("Base Enemy Prefab",
                "Prefab with an Enemy component on its root and an 'Offset' child."),
            baseEnemyPrefab, typeof(GameObject), false);

        enemyDataFolder = EditorGUILayout.TextField(
            new GUIContent("EnemyData Folder",
                "Folder containing EnemyData ScriptableObjects, matched by name to each visual."),
            enemyDataFolder);

        outputFolder = EditorGUILayout.TextField(
            new GUIContent("Output Folder",
                "Folder where generated prefabs will be saved."),
            outputFolder);

        EditorGUILayout.Space();

        bool ready = visualSourceParent != null
                  && baseEnemyPrefab != null
                  && !string.IsNullOrEmpty(enemyDataFolder)
                  && !string.IsNullOrEmpty(outputFolder);

        EditorGUI.BeginDisabledGroup(!ready);
        if (GUILayout.Button("Generate Enemy Prefabs", GUILayout.Height(32)))
            GeneratePrefabs();
        EditorGUI.EndDisabledGroup();

        if (!ready)
            EditorGUILayout.HelpBox("Fill in all four fields before generating.", MessageType.Info);
    }

    // ?? Generation ????????????????????????????????????????????????????????????

    private void GeneratePrefabs() {
        EnsureFolderExists(outputFolder);

        int childCount = visualSourceParent.transform.childCount;
        if (childCount == 0) {
            EditorUtility.DisplayDialog("No children found",
                "The Visual Source Parent has no child GameObjects.", "OK");
            return;
        }

        // Snapshot children before any re-parenting touches the hierarchy
        Transform[] visuals = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            visuals[i] = visualSourceParent.transform.GetChild(i);

        int success = 0, skipped = 0;

        foreach (Transform visual in visuals) {
            if (ProcessVisual(visual.gameObject)) success++;
            else skipped++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string msg = $"Generated: {success} prefab(s).";
        if (skipped > 0) msg += $"\nSkipped:   {skipped} (check Console for details).";
        EditorUtility.DisplayDialog("Done", msg + $"\n\n? {outputFolder}", "OK");
        Debug.Log($"[EnemyPrefabBatchCreator] {msg}");
    }

    /// <summary>Returns true on success.</summary>
    private bool ProcessVisual(GameObject visual) {
        string visualName = visual.name;
        string prefabPath = $"{outputFolder}/{visualName}.prefab";

        // ?? 1. Instantiate base prefab ????????????????????????????????????????
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(baseEnemyPrefab);
        instance.name = visualName;

        // ?? 2. Find Enemy component on root ???????????????????????????????????
        Enemy enemy = instance.GetComponent<Enemy>();
        if (enemy == null) {
            Debug.LogError($"[EnemyPrefabBatchCreator] Base prefab root has no Enemy component. " +
                           $"Skipping: {visualName}");
            DestroyImmediate(instance);
            return false;
        }

        // ?? 3. Find matching EnemyData ScriptableObject by name ???????????????
        EnemyData data = LoadEnemyDataByName(visualName);
        if (data == null) {
            Debug.LogWarning($"[EnemyPrefabBatchCreator] No EnemyData named '{visualName}' found " +
                             $"in '{enemyDataFolder}'. enemyData will be null for: {visualName}");
            // We continue — you may prefer to return false here if a missing SO is fatal.
        }

        // Assign via SerializedObject so the change is properly recorded
        SerializedObject serializedEnemy = new SerializedObject(enemy);
        SerializedProperty dataProp = serializedEnemy.FindProperty("enemyData");

        if (dataProp == null) {
            Debug.LogError($"[EnemyPrefabBatchCreator] Could not find serialized field 'enemyData' " +
                           $"on Enemy component. Check the field name matches exactly. Skipping: {visualName}");
            DestroyImmediate(instance);
            return false;
        }

        dataProp.objectReferenceValue = data;
        serializedEnemy.ApplyModifiedPropertiesWithoutUndo();

        // ?? 4. Find Offset child ??????????????????????????????????????????????
        Transform offset = FindChildNamed(instance.transform, "Visual");
        if (offset == null) {
            Debug.LogError($"[EnemyPrefabBatchCreator] Base prefab has no 'Offset' child. " +
                           $"Skipping: {visualName}");
            DestroyImmediate(instance);
            return false;
        }

        // ?? 5. Duplicate visual ? parent under Offset ?????????????????????????
        GameObject visualCopy = Instantiate(visual, offset);
        visualCopy.name = visualName;
        visualCopy.transform.localPosition = Vector3.zero;
        visualCopy.transform.localRotation = Quaternion.identity;
        visualCopy.transform.localScale = Vector3.one;

        // ?? 6. Save prefab ????????????????????????????????????????????????????
        bool saved;
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath, out saved);

        if (saved) Debug.Log($"[EnemyPrefabBatchCreator] Saved ? {prefabPath}" +
                             (data != null ? $"  (EnemyData: {data.name})" : "  (EnemyData: MISSING)"));
        else Debug.LogError($"[EnemyPrefabBatchCreator] PrefabUtility failed to save: {prefabPath}");

        // ?? 7. Clean up scene instance ????????????????????????????????????????
        DestroyImmediate(instance);
        return saved;
    }

    // ?? Helpers ???????????????????????????????????????????????????????????????

    /// <summary>
    /// Loads the first EnemyData asset whose filename matches <paramref name="name"/>
    /// inside <see cref="enemyDataFolder"/> (non-recursive).
    /// </summary>
    private EnemyData LoadEnemyDataByName(string name) {
        // FindAssets searches by filename (without extension) across the given folder.
        string[] guids = AssetDatabase.FindAssets($"{name} t:EnemyData", new[] { enemyDataFolder });
        foreach (string guid in guids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EnemyData so = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
            // Exact name match — FindAssets does a "contains" search, so we verify.
            if (so != null && so.name == name)
                return so;
        }
        return null;
    }

    /// <summary>
    /// Returns the first Transform named <paramref name="childName"/> in the hierarchy
    /// (direct child first, then deep search).
    /// </summary>
    private static Transform FindChildNamed(Transform root, string childName) {
        Transform direct = root.Find(childName);
        if (direct != null) return direct;

        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            if (t != root && t.name == childName)
                return t;

        return null;
    }

    /// <summary>Creates every missing folder in a project-relative path.</summary>
    private static void EnsureFolderExists(string folderPath) {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string[] parts = folderPath.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++) {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
