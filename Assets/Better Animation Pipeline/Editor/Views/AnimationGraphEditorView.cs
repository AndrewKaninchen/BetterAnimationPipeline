using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BetterAnimationPipeline
{
	public class AnimationGraphEditorView : VisualElement
	{
		private AnimationGraph graph;
		private AnimationGraphView graphView;
		private EdgeConnectorListener connectorListener;
		private AnimationGraphSearchWindowProvider animationGraphSearchWindowProvider;
		private EditorWindow editorWindow;

		public AnimationGraphEditorView(EditorWindow editorWindow, AnimationGraph graph = null)
		{
			this.editorWindow = editorWindow;
			if (graph == null) graph = ScriptableObject.CreateInstance<AnimationGraph>();
			this.graph = graph;
			
			animationGraphSearchWindowProvider = ScriptableObject.CreateInstance<AnimationGraphSearchWindowProvider>();
			connectorListener = new EdgeConnectorListener();
			graphView = new AnimationGraphView(graph, connectorListener, animationGraphSearchWindowProvider) { name = "GraphView", viewDataKey = "AnimationGraphView" };
			
			connectorListener.Initialize(graphView, animationGraphSearchWindowProvider);
			animationGraphSearchWindowProvider.Initialize(graph, graphView, editorWindow);
			
			var content = new VisualElement { name = "content" };
			{
				var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Better Animation Pipeline/Editor/Styles/GraphEditorView.uss");
				styleSheets.Add(styleSheet);
				content.Add(graphView);
			}
			
			Add(content);
		}
	}
}