using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	public class MyAnimator : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float teste;
		
		[SerializeField] private AnimationGraph animationGraph;

		private PlayableGraph playableGraph;
		private List<AnimationScriptPlayable> animationScriptPlayables = new List<AnimationScriptPlayable>();
		private List<AnimationMixerPlayable> animationMixerPlayables = new List<AnimationMixerPlayable>();
		private List<AnimationClipPlayable> animationClipPlayables = new List<AnimationClipPlayable>();
		
		private void OnEnable()
		{
			var animator = GetComponent<UnityEngine.Animator>();
			
			BuildPlayableGraph(animator);
			
			playableGraph.Play();
		}

		private void BuildPlayableGraph(Animator animator)
		{
			animationGraph = Instantiate(animationGraph);
			animationGraph.animator = animator;
			playableGraph = animationGraph.CreatePlayableGraph(animator);
		}
		

		private void Update()
		{
			animationGraph.teste = teste;
			animationGraph.Update();
		}

		private void UpdateParameters()
		{
		}
		
		private void UpdateJobData()
		{
		}

		private void OnDisable()
		{
			playableGraph.Destroy();
		}
	}
}