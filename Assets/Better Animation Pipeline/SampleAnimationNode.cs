using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	[Serializable, AnimationNodeCategory("Sample Animation Clip", 1)]
	public class SampleAnimationNode : Node <AnimationClipPlayable>
	{
		[EmbeddedSlot, SerializeField]
		private AnimationClip clip;
		
		private AnimationClipPlayable playable;
		protected override IPlayable Playable => playable;
		public override void PreparePlayable(AnimationGraph animationGraph, PlayableGraph playableGraph)
		{
			playable = AnimationClipPlayable.Create(playableGraph, clip);
		}

		public override void Setup(AnimationGraph graph, PlayableGraph playableGraph)
		{
		}

		public override void Update(AnimationGraph graph)
		{
			base.Update(graph);
			playable.SetTime(Time.time);
		}

		public SampleAnimationNode(int id) : base(id)
		{
		}
	}
}