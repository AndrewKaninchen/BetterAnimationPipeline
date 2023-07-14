using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace BetterAnimationPipeline
{
	public class NodeSlot
	{
		public enum SlotDirection { Input, Output, Embedded }
		
		public int nodeID;
		public int slotID;
		public SlotDirection direction;
		public string displayName = "Pose";
		public string fieldName;
		public Type slotType;
		
		[NonSerialized] private FieldInfo fieldInfo;
		[NonSerialized] private Node node;
		
		public void GetFieldInfo(AnimationGraph graph)
		{
			node = graph.nodes[nodeID];
			fieldInfo = node.GetType().GetField(fieldName,
				BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
				BindingFlags.GetProperty);
		}
		
		public void SetValue<T>(AnimationGraph graph, T value)
		{
			if (fieldInfo == null) GetFieldInfo(graph);
			if (fieldInfo != null) fieldInfo.SetValue(node, value);
		}
		
		public object GetValue(AnimationGraph graph)
		{
			if (fieldInfo == null) GetFieldInfo(graph);
			return fieldInfo?.GetValue(node);
		}
	}

	[Serializable]
	public abstract class Node<T> : Node where T : IPlayable
	{
		public override void PrepareOutput()
		{
			outputPose = new Pose(null, Playable, 0);
		}

		public override void Update(AnimationGraph graph)
		{
		}

		protected Node(int id) : base(id)
		{
		}
	}
	
	[Serializable]
	public class Node// : ISerializationCallbackReceiver
	{
		[OutputSlot(name = "Pose")]
		public Pose outputPose;
		public Vector2 position;
		public string typeName;
		
//		[NonSerialized] 
		public List<NodeSlot> slots;
		
		protected virtual IPlayable Playable { get; }
		[SerializeField] private int id;
		
		protected Node(int id)
		{
			this.id = id;
			slots = new List<NodeSlot>();
			typeName = GetType().AssemblyQualifiedName;
			GetSlots(false);
		}
		
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			slots = new List<NodeSlot>();
			GetSlots(true);
		}
		private void GetSlots(bool isDeserializing)
		{
			var fields = Type.GetType(typeName)?.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();

			if (fields == null)
				return;

			foreach (var field in fields)
			{
//				Debug.Log($"({GetType().Name}) Field: {field.Name}");
				var input = field.GetCustomAttribute<InputSlotAttribute>();
				var output = field.GetCustomAttribute<OutputSlotAttribute>();
				var embedded = field.GetCustomAttribute<EmbeddedSlotAttribute>();

				if (input == null && output == null && embedded == null)
					continue;

				var direction = input != null ? NodeSlot.SlotDirection.Input :
					output != null ? NodeSlot.SlotDirection.Output : NodeSlot.SlotDirection.Embedded;

				var slotID = slots.Count;
				
				var slot = new NodeSlot
				{
					nodeID = id,
					slotID = slotID,
					direction = direction,
					displayName = input != null
						? input.name ?? field.Name
						: output?.name ?? field.Name,
					fieldName = field.Name,
					slotType = field.FieldType
				};
				
				slots.Add(slot);
			}
		}
		
		public virtual void Update(AnimationGraph graph) {}
		public virtual void PrepareOutput(){}
		public virtual void PreparePlayable(AnimationGraph animationGraph, PlayableGraph playableGraph) {}
		public virtual void Setup(AnimationGraph graph, PlayableGraph playableGraph){}
	}
	
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class AnimationNodeCategoryAttribute : Attribute
	{
		public string name;
		public int level;
		public AnimationNodeCategoryAttribute(string name, int level)
		{
			this.name = name;
			this.level = level;
		}
	}
}