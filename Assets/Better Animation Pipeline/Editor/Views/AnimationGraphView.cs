using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BetterAnimationPipeline
{
	public class AnimationGraphView : GraphView
	{
		public AnimationGraph graph;
		public IEdgeConnectorListener edgeConnectorListener;
		private AnimationGraphSearchWindowProvider animationGraphSearchWindowProvider;
		private Dictionary<NodeSlot, Port> slotToPort = new Dictionary<NodeSlot, Port>();
		
		
		public AnimationGraphView(AnimationGraph graph, IEdgeConnectorListener connectorListener,
			AnimationGraphSearchWindowProvider animationGraphSearchWindowProvider)
		{
			edgeConnectorListener = connectorListener;
			this.animationGraphSearchWindowProvider = animationGraphSearchWindowProvider;
			
			this.graph = graph;
			var gridBackground = new GridBackground();
			Add(gridBackground);
			gridBackground.SendToBack();
			
			SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new ClickSelector());
//                RegisterCallback<KeyDownEvent>(OnSpaceDown);
//                groupTitleChanged = OnGroupTitleChanged;
//                elementsAddedToGroup = OnElementsAddedToGroup;
//                elementsRemovedFromGroup = OnElementsRemovedFromGroup;

			
			nodeCreationRequest = c =>
			{
				SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), animationGraphSearchWindowProvider);
			};

			if (graph != null)
			{
				foreach (var node in graph.nodes)
					AddNode(node, node.position, Vector2.one);

				foreach (var edge in graph.edges)
					AddEdge(edge.from, edge.to);
			}
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList().Where 
				(nap =>
					nap.direction != startPort.direction 
					&& nap.node != startPort.node 
//					&& (nodeAdapter.GetAdapter(nap.source, startPort.source) != null)
				)
				.ToList();
		}
	
		public void AddNode(Node node, Vector2 position, Vector2 size)
		{
			var nodeView = new AnimationNodeView(node, graph, edgeConnectorListener);
			nodeView.slotToPort.ToList().ForEach(x => slotToPort.Add(x.Key, x.Value));
			AddElement(nodeView);
			nodeView.SetPosition(new Rect(position, size));
		}

		public void AddEdge(NodeSlot from, NodeSlot to)
		{
//			foreach (var port in slotToPort)
//			{
//				Debug.Log($"{port.Key.displayName} -{port.Key == from || port.Key == to}- {from.displayName} ");
//			}
			var edge = new UnityEditor.Experimental.GraphView.Edge {output = slotToPort[from], input = slotToPort[to]};
			edge.input?.ConnectTo(edge.output);
			AddElement(edge);
		}
	}
	public class EdgeConnectorListener : IEdgeConnectorListener
	{
		private AnimationGraph graph;
		private AnimationGraphView graphView;
		private AnimationGraphSearchWindowProvider animationGraphSearchWindowProvider;

		public void Initialize(GraphView graphView,
			AnimationGraphSearchWindowProvider animationGraphSearchWindowProvider)
		{
			this.graphView = graphView as AnimationGraphView;
			graph = this.graphView?.graph;
			this.animationGraphSearchWindowProvider = animationGraphSearchWindowProvider;
		}

		public void OnDropOutsidePort(UnityEditor.Experimental.GraphView.Edge edge, Vector2 position)
		{
			SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), animationGraphSearchWindowProvider);
		}

		public void OnDrop(GraphView graphView, UnityEditor.Experimental.GraphView.Edge edge)
		{
			edge.input.ConnectTo(edge.output);
			graphView.AddElement(edge);
			
			var inputNodeView = edge.input.node as AnimationNodeView;
			var outputNodeView = edge.output.node as AnimationNodeView;
			var inputPort = edge.input as AnimationPort;
			var outputPort = edge.output as AnimationPort;
			
			var inputSlot = (inputNodeView?.node.slots)?.First(x => inputPort?.slot == x);
			var outputSlot = (outputNodeView?.node.slots)?.First(x => outputPort?.slot == x);
			
			graph.AddEdge(outputSlot, inputSlot);
			EditorUtility.SetDirty(graph);
		}
	}
}