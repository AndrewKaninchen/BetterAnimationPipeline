using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	[Serializable, AnimationNodeCategory("Output", 1)]
	public class MasterNode : Node
	{
		[InputSlot(name = "Pose")]
		private Pose pose;
		private AnimationPlayableOutput playableOutput;
		
		public override void PreparePlayable(AnimationGraph animationGraph, PlayableGraph playableGraph)
		{
			playableOutput = AnimationPlayableOutput.Create(playableGraph, "sdasd", animationGraph.animator);
		}

		public override void Setup(AnimationGraph animationGraph, PlayableGraph playableGraph)
		{
			Debug.Log(pose);
			switch (pose.Playable)
			{
				case AnimationScriptPlayable s:
					playableOutput.SetSourcePlayable(s);
					break;
				case AnimationMixerPlayable m:
					playableOutput.SetSourcePlayable(m);
					break;
				case AnimationClipPlayable c:
					playableOutput.SetSourcePlayable(c);
					break;
			}
		}

		public MasterNode(int id) : base(id)
		{
		}
	}
}