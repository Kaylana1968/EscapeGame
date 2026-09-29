using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

/// <summary>
/// Transforme les "Switch" du pack LowPolyDungeon en leviers VR (XRLever),
/// en gardant le modèle en bois d'origine.
/// Usage : sélectionner les Switch dans la Hierarchy, puis Tools > Leviers > Configurer les leviers sélectionnés.
/// </summary>
public static class WoodenLeverSetup
{
    const string HandleName = "Switch_Lever";
    const float MinAngle = -40f; // position "repos" (haut)
    const float MaxAngle = 40f;  // position "abaissé"

    [MenuItem("Tools/Leviers/Configurer les leviers sélectionnés")]
    static void SetupSelected()
    {
        int done = 0;

        foreach (var root in Selection.gameObjects)
        {
            Transform handle = root.transform.Find(HandleName);
            if (handle == null)
            {
                Debug.LogWarning($"[WoodenLeverSetup] '{root.name}' n'a pas d'enfant '{HandleName}', ignoré.");
                continue;
            }

            // Le manche doit pouvoir bouger : on retire le flag Static (sinon le static batching le fige)
            Undo.RecordObject(handle.gameObject, "Leviers : retirer Static");
            GameObjectUtility.SetStaticEditorFlags(handle.gameObject, 0);

            // Collider sur le manche pour que la main puisse l'attraper
            if (handle.GetComponent<Collider>() == null)
            {
                var box = Undo.AddComponent<BoxCollider>(handle.gameObject);
                var meshFilter = handle.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    box.center = meshFilter.sharedMesh.bounds.center;
                    box.size = meshFilter.sharedMesh.bounds.size;
                }
            }

            // Composant XRLever sur la racine
            var lever = root.GetComponent<XRLever>();
            if (lever == null)
                lever = Undo.AddComponent<XRLever>(root);

            var so = new SerializedObject(lever);
            so.FindProperty("m_Handle").objectReferenceValue = handle;
            so.FindProperty("m_LockToValue").boolValue = true;
            so.FindProperty("m_MinAngle").floatValue = MinAngle;
            so.FindProperty("m_MaxAngle").floatValue = MaxAngle;
            so.FindProperty("m_Value").boolValue = false;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(root);
            done++;
        }

        Debug.Log($"[WoodenLeverSetup] {done} levier(s) configuré(s).");
    }

    [MenuItem("Tools/Leviers/Configurer les leviers sélectionnés", true)]
    static bool ValidateSetupSelected() => Selection.gameObjects.Length > 0;
}
