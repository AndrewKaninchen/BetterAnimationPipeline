using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	[Serializable, AnimationNodeCategory("Blend", 1)]
	public class BlendAnimationNode : Node <AnimationMixerPlayable>
	{
		[InputSlot] private Pose A;
		[InputSlot] private Pose B;
		[InputSlot] private float Alpha;

		private AnimationMixerPlayable playable;
		protected override IPlayable Playable => playable;
		public override void PreparePlayable(AnimationGraph animationGraph, PlayableGraph playableGraph)
		{
			playable = AnimationMixerPlayable.Create(playableGraph, 2);
		}

		public override void Setup(AnimationGraph animationGraph, PlayableGraph playableGraph)
		{
			void Connect(IPlayable p, int sourceOutputPort, int inputPort)
			{
				switch (p)
				{
					case AnimationScriptPlayable s:
						playableGraph.Connect(s, sourceOutputPort, playable, inputPort);
						break;
					case AnimationMixerPlayable m:
						playableGraph.Connect(m, sourceOutputPort, playable, inputPort);
						break;
					case AnimationClipPlayable c:
						playableGraph.Connect(c, sourceOutputPort, playable, inputPort);
						break;
				}
			}
			
			Connect(A.Playable, A.OutputPort, 0);
			Connect(B.Playable, B.OutputPort, 1);
		}

		public override void Update(AnimationGraph graph)
		{
			playable.SetTime(Time.time);
			playable.SetInputWeight(0, 1f-Alpha);
			playable.SetInputWeight(1, Alpha);
		}

		public BlendAnimationNode(int id) : base(id)
		{
		}
	}
}