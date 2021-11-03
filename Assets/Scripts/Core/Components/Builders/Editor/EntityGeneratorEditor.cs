using UnityEngine;
using UnityEditor;

namespace Guinea.Core.Components
{
    [CustomEditor(typeof(EntityGenerator))]
    [CanEditMultipleObjects]
    public class EntityGeneratorEditor : Editor
    {
        GUIStyle label_style = new GUIStyle();

        void Awake()
        {
            label_style.richText = true;
            label_style.alignment = TextAnchor.MiddleCenter;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EntityGenerator entityGenerator = (EntityGenerator)target;
            GUILayout.Label("<size=16><color=green>Entity Generator</color></size>", style: label_style);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Serialize"))
            {
                entityGenerator.Serialize();
            }
            if (GUILayout.Button("Deserialize"))
            {
                entityGenerator.Deserialize();
            }
            if (GUILayout.Button("Generate Entity"))
            {
                entityGenerator.Generate();
            }
            GUILayout.EndHorizontal();
        }
    }
}