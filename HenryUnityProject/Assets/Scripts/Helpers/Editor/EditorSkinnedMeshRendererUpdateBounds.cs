using UnityEditor;
using UnityEngine;

public class EditorSkinnedMeshRendererUpdateBounds {

    [CanEditMultipleObjects]
    [MenuItem("CONTEXT/SkinnedMeshRenderer/RecalculateBounds")]
    public static void recalculateBounds() {

        foreach (Transform item in Selection.transforms){
            SkinnedMeshRenderer skinnedMeshRenderer = item.GetComponent<SkinnedMeshRenderer>();
            if (!skinnedMeshRenderer)
                return;//continue;
            Undo.RecordObject(skinnedMeshRenderer, "recalculating bounds");
            skinnedMeshRenderer.updateWhenOffscreen = true;
            Bounds bounds = new Bounds() {
                center = skinnedMeshRenderer.localBounds.center,
                extents = skinnedMeshRenderer.localBounds.extents
            };
            skinnedMeshRenderer.updateWhenOffscreen = false;
            skinnedMeshRenderer.localBounds = bounds;
        }
    }
}
