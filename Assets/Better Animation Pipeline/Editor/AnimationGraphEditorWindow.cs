using System;
using BetterAnimationPipeline;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace BetterAnimationPipeline
{
    public class AnimationGraphEditorWindow : EditorWindow
    {
        private AnimationGraph graph;
        
        [UnityEditor.Callbacks.OnOpenAsset(-1222)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var graph = EditorUtility.InstanceIDToObject(instanceID) as AnimationGraph;
            if (graph != null) {
                var window = GetWindow();
                window.graph = graph;
                window.OnEnable();
                return true;
            }

            Debug.Log(EditorUtility.InstanceIDToObject(instanceID).GetType());
            return false;
        }
        
        
        
        [MenuItem("Window/UIElements/MyAnimator")]
        public static AnimationGraphEditorWindow GetWindow()
        {
            var window = GetWindow<AnimationGraphEditorWindow>();
            window.titleContent = new GUIContent("MyAnimator");
            return window;
        }

        public void OnEnable()
        {
            var root = rootVisualElement;
            root.Clear();
            if (graph != null)
            {
                var graphEditorView = new AnimationGraphEditorView(this, graph);
                root.Add(graphEditorView);
            }
        }
    }
}