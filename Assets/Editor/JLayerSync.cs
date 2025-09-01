using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class JLayerSync : MonoBehaviour
{
    [MenuItem("Tools/Sync Prefab Layer To Scene Instances")]
    static void SyncPrefabLayerAndTag()
    {
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        int layerChangeCount = 0;
        int tagChangeCount = 0;

        foreach(GameObject go in allObjects)
        {
            if(go.name == "SM_platform_japan_01")
            {
                int a = 0;
            }

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);

            if(prefab != null)
            {
                GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
                GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefabRoot);

                if(prefabSource != null)
                {
                    if (go.layer != prefabSource.layer)
                    {
                        go.layer = prefabSource.layer;
                        ++layerChangeCount;
                    }

                    if (go.tag != prefabSource.tag)
                    {
                        go.tag = prefabSource.tag;
                        ++tagChangeCount;
                    }
                }
            }
        }

        Debug.Log($"[PrefabSync] Updated {layerChangeCount} objects to match prefab Tag.");
        Debug.Log($"[PrefabSync] Updated {tagChangeCount} objects to match prefab Layers.");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
