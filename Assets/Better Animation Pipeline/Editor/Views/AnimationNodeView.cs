using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EditorGUILayout = UnityEditor.EditorGUILayout;
using Object = UnityEngine.Object;

namespace BetterAnimationPipeline
{
	public class AnimationNodeView : UnityEditor.Experimental.GraphView.Node
	{
		public Node node;
		public Dictionary<NodeSlot, AnimationPort> slotToPort = new Dictionary<NodeSlot, AnimationPort>();

		public AnimationClip clip = null;

		
		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			node.position = newPos.position;
		}

		public AnimationNodeView(Node node, AnimationGraph graph, IEdgeConnectorListener connectorListener) : base()
		{
			this.node = node;
			var nodeType = Type.GetType(node.typeName);
			Debug.Log(nodeType);
			var categoryAttribute = nodeType?.GetCustomAttribute<AnimationNodeCategoryAttribute>();
			
			if (categoryAttribute == null)
				return;
			
			this.title = categoryAttribute.name;

			foreach (var slot in node.slots)
			{
				if (slot.direction != NodeSlot.SlotDirection.Embedded)
				{
					var port = AnimationPort.Create(slot, connectorListener, slot.slotType) as AnimationPort;
					slotToPort.Add(slot, port);

					if (slot.direction == NodeSlot.SlotDirection.Input)
						inputContainer.Add(port);
					else
						outputContainer.Add(port);
				}
				else
				{
					var field = new ObjectField
					{
						objectType = slot.slotType,
						allowSceneObjects = false,
						value = (Object) slot.GetValue(graph)
					};
					
					field.RegisterValueChangedCallback((x) =>
					{
						slot.SetValue(graph, x.newValue);
						EditorUtility.SetDirty(graph);
					});
					mainContainer.Add(field);
				}
			}
			
			if(inputContainer.childCount == 0)
				inputContainer.style.flexGrow = 0f;
			if(outputContainer.childCount == 0)
				outputContainer.style.flexGrow = 0f;
			
			BringToFront();
		}
	}
	public class AnimationPort : Port
	{
		public NodeSlot slot { get; private set; }

		private AnimationPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
			: base(portOrientation, portDirection, portCapacity, type)
		{
			//styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderPort"));
		}


		public static Port Create(NodeSlot slot, IEdgeConnectorListener connectorListener, Type type)
		{
			var port = new AnimationPort(Orientation.Horizontal, slot.direction == NodeSlot.SlotDirection.Input ? Direction.Input : Direction.Output,
				slot.direction == NodeSlot.SlotDirection.Input ? Capacity.Single : Capacity.Multi, type)
			{
				m_EdgeConnector = new EdgeConnector<UnityEditor.Experimental.GraphView.Edge>(connectorListener),
			};
			port.AddManipulator(port.m_EdgeConnector);
			port.slot = slot;
			port.portName = slot.displayName;
			return port;
		}
	}
}
