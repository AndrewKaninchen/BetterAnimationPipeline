using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BetterAnimationPipeline
{
	public class AnimationGraphSearchWindowProvider : ScriptableObject, ISearchWindowProvider
	{
		private EditorWindow editorWindow;
		private AnimationGraph graph;
		private AnimationGraphView graphView;
		private Dictionary<SearchTreeEntry, Type> nodeDictionary;
		private List<SearchTreeEntry> searchTree;
		private bool isInitialized = false;

		public void Initialize(AnimationGraph graph, AnimationGraphView graphView, EditorWindow editorWindow)
		{
			this.editorWindow = editorWindow;
			this.graph = graph;
			this.graphView = graphView;
			PopulateNodeDictionary();
			PrecomputeSearchTree();
			isInitialized = true;
		}
		
		private void PopulateNodeDictionary ()
		{
			nodeDictionary = new Dictionary<SearchTreeEntry, Type>();
			foreach (var type in typeof(Node).Assembly.GetTypes())
			{
				if (type.IsSubclassOf(typeof(Node)))
				{
					var categoryAttribute = type.GetCustomAttribute<AnimationNodeCategoryAttribute>();
					if (categoryAttribute != null)
					{
						Debug.Log($"Found {categoryAttribute.name}");
						nodeDictionary.Add(
							new SearchTreeEntry(new GUIContent(categoryAttribute.name))
								{level = categoryAttribute.level},
							type);
					}
				}
			}
		}

		public void PrecomputeSearchTree()
		{
			searchTree = new List<SearchTreeEntry>(){new SearchTreeGroupEntry(new GUIContent("Create Node"))};
			searchTree.AddRange(nodeDictionary.Keys.ToList());
		}
		
		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			if(!isInitialized) Initialize(graph, graphView, editorWindow);
			return searchTree;
		}

		public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
		{
			var windowRoot = editorWindow.rootVisualElement;
			var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - editorWindow.position.position);
			var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

			//var node = (Node) Activator.CreateInstance(nodeDictionary[entry], BindingFlags.Default, new {});
			
			var node = graph.AddNodeOfType(nodeDictionary[entry]);
			EditorUtility.SetDirty(graph);
			graphView.AddNode(node,
				graphMousePosition,
				new Vector2(context.requestedWidth, context.requestedHeight));
			return true;
		}
	}
}