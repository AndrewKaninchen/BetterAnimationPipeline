using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;


namespace BetterAnimationPipeline
{
//	public abstract class AnimatorParameter
//	{
//		public object value;
//		public string name;
//	}

	public abstract class SlotAttribute : Attribute
	{
		public string name;
	}
	
	public class InputSlotAttribute : SlotAttribute {}
	
	public class OutputSlotAttribute : SlotAttribute {}
	
	public class EmbeddedSlotAttribute : SlotAttribute {}

	public class Pose
	{
		public AnimationStream stream;
		public IPlayable Playable { get; set; }
		public int OutputPort { get; set; }

		public Pose(AnimationStream? stream, IPlayable playable, int outputPort)
		{
			if (stream != null) this.stream = stream.Value; //TODO: de fato conectar esta bosta de alguma forma? Talvez não seja necessário no fim das contas eu acho, porque vai ter uma classe de Node específica pra AnimationJob
			Playable = playable;
			OutputPort = outputPort;
		}
	}

	[CreateAssetMenu(fileName = "AG_New")]
	public class AnimationGraph : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector]
		private SerializationData serializationData;

		[SerializeField] public float teste;
		
		//public List<AnimatorParameter> parameters;
		[NonSerialized, OdinSerialize] public Dictionary<string, object> parameters = new Dictionary<string, object>(){{"Test", 0f}};

		[NonSerialized] public MasterNode masterNode;
		[NonSerialized, OdinSerialize] public List<Node> nodes;
		[NonSerialized, OdinSerialize] public List<Edge> edges;
		[NonSerialized] public PlayableGraph playableGraph;
		[SerializeField] private List<UnityEngine.Object> unityReferences;

		public Animator animator;
		
		public AnimationGraph()
		{
			masterNode = new MasterNode(0){};
			
			nodes = new List<Node>() {masterNode};
			edges = new List<Edge>();
		}
		
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			using (var cachedContext = Cache<DeserializationContext>.Claim())
			{
				cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
				serializationData.ReferencedUnityObjects = unityReferences;
				UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData, cachedContext.Value);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			using (var cachedContext = Cache<SerializationContext>.Claim())
			{
				cachedContext.Value.Config.SerializationPolicy = SerializationPolicies.Everything;
				UnitySerializationUtility.SerializeUnityObject(this, ref serializationData, true, cachedContext.Value);
				unityReferences = serializationData.ReferencedUnityObjects;
			}
		}
		
		public PlayableGraph CreatePlayableGraph(Animator animator)
		{
			playableGraph = PlayableGraph.Create();
			this.animator = animator;
			
			foreach (var node in nodes)
			{
				//node.OnEnable();
				node.PreparePlayable(this, playableGraph);
				node.PrepareOutput();
			}
			
			foreach (var edge in edges)
				edge.to.SetValue(this, edge.from.GetValue(this));
		
			foreach (var node in nodes)
				node.Setup(this, playableGraph);

			return playableGraph;
		}

		public void Update()
		{
			foreach (var node in nodes)
				node.Update(this);
			
			foreach (var edge in edges)
				edge.to.SetValue(this, edge.from.GetValue(this));
		}
		public Node AddNodeOfType(Type T)
		{
			var node = (Node) Activator.CreateInstance(T, nodes.Count);
			nodes.Add(node);
			return node;
		}

		public void AddEdge(NodeSlot outputSlot, NodeSlot inputSlot)
		{
			edges.Add(new Edge(inputSlot, outputSlot));
		}
	}
}
