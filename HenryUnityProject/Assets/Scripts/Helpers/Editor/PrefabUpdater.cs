using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PrefabUpdater {

    public static Dictionary<string, string> guidToFileIDMap = new Dictionary<string, string> {
        {"fileID: 11500000, guid: 68ee60b5bcd2668489cac0473619dffe", "fileID: -1811796171, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 7d97a6e4a1ffe974ba28701e8f7bd348", "fileID: 1143142494, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 261c50d8a80898e4ebf2e8e415ea6766", "fileID: -226763392, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 1f614a7e494406347a5ab8313eadbc6b", "fileID: 1480773730, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: a63dc2c00bc45234bab375db0c723d96", "fileID: 1783796254, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 50213d3126f9b2a45ae3b0a558547f80", "fileID: -1294804675, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: db3f2aec5abca794c9f7f7d05059f80e", "fileID: -202159943, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 751bd37d2e55f5f448dbb0d97812d9ba", "fileID: -517518216, guid: 951ce57ad999ac1f040a4dceb5f8b763" },
        {"fileID: 11500000, guid: 98763764711d9a645b02986adc14cee6", "fileID: -1792105662, guid: 951ce57ad999ac1f040a4dceb5f8b763" }
    };

    private static string m_ProjectPath;

    [MenuItem("Tools/Update Existing Prefabs To RoR2 Scripts")]
    private static void UpdateExistingPrefabsToRoR2Scripts() {

        if (!EditorUtility.DisplayDialog("Upgrade Existing Prefabs", "This will update your prefabs to reference scripts in the ror2 assembly proper.\n\n" +
            "It is Heavily recommended to make a backup of your project before continuing!", "I've made a backup, go ahead!", "Cancel")) {
            return;
        }

        if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath("951ce57ad999ac1f040a4dceb5f8b763"))) {
            Debug.LogError("Tried to convert prefabs without RoR2 in your project. Use thunderkit to import the RoR2 assembly first");
            return;
        }

        m_ProjectPath = Path.GetFullPath("Assets/..");

        ScanAndUpdateFiles();

        //AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out string guid, out long _);
        //Debug.LogWarning(guid);
        //TryUpgradePrefab(guid);
        //AssetDatabase.Refresh();
        //Selection.objects = new UnityEngine.Object[0];
    }

    private static void ScanAndUpdateFiles() {

        string[] prefabGuids = AssetDatabase.FindAssets("t:prefab");

        for (int i = 0; i < prefabGuids.Length; i++) {

            string guid = prefabGuids[i];
            TryUpgradePrefab(guid);
        }

        AssetDatabase.Refresh();
        Selection.objects = new UnityEngine.Object[0];
    }

    private static void TryUpgradePrefab(string guid) {

        string assetDataFile;
        string assetDataPath = m_ProjectPath + "/" + AssetDatabase.GUIDToAssetPath(guid);

        try {
            assetDataFile = File.ReadAllText(assetDataPath);
        }
        catch {
            Debug.LogError($"could not file from path {assetDataPath}");
            return;
        }

        bool changed = false;
        foreach (KeyValuePair<string, string> kvp in guidToFileIDMap) {

            if (assetDataFile.Contains(kvp.Key)) {
                assetDataFile = assetDataFile.Replace(kvp.Key, kvp.Value);
                changed = true;
            }
        }
        if (changed) {
            Debug.Log($"Saving updates to {assetDataPath}");
            File.WriteAllText(assetDataPath, assetDataFile);
        }
    }
}