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
	public class TestAnimator : MonoBehaviour
	{
		[SerializeField] private AnimationGraph controller;
		private PlayableGraph graph;
		private List<AnimationScriptPlayable> animationScriptPlayables = new List<AnimationScriptPlayable>();
		
		private void Start()
		{
			var animator = GetComponent<UnityEngine.Animator>();

			graph = PlayableGraph.Create("TransformStreamHandleExample");
			var output = AnimationPlayableOutput.Create(graph, "output", animator);
			var mixer = AnimationMixerPlayable.Create(graph, data.Count);
			
			for (var i = 0; i < data.Count; i++)
			{
				var animationJob = new TransformStreamHandleJob
				{
					handle = animator.BindStreamTransform(gameObject.transform)
				};
				var playable = AnimationScriptPlayable.Create(graph, animationJob);
				animationScriptPlayables.Add(playable);

				mixer.ConnectInput(i, animationScriptPlayables[i], 0);
				mixer.SetInputWeight(i, 1f/data.Count);
			}

			output.SetSourcePlayable(mixer);
			graph.Play();
		}

		[SerializeField] private List<Data> data = new List<Data>();

		[Serializable]
		public struct Data
		{
			public Vector3 position;
			public Vector3 rotation;
			public Vector3 scale;
		}
		private void Update()
		{
			for (var i = 0; i < animationScriptPlayables.Count; i++)
			{
				var animationScriptPlayable = animationScriptPlayables[i];
				var animationJob = animationScriptPlayable.GetJobData<TransformStreamHandleJob>();
				animationJob.position = data[i].position;
				animationJob.rotation = data[i].rotation;
				animationJob.scale = data[i].scale;
				animationScriptPlayable.SetJobData(animationJob);
			}
		}

		private void OnDestroy()
		{
			graph.Destroy();
		}
	}
	public struct TransformStreamHandleJob : IAnimationJob
	{
		public TransformStreamHandle handle;
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public void ProcessRootMotion(AnimationStream stream)
		{
			handle.SetLocalPosition(stream, position);
			handle.SetLocalRotation(stream, Quaternion.Euler(rotation));
			handle.SetLocalScale(stream, scale);
		}

		public void ProcessAnimation(AnimationStream stream)
		{
		}
	}
}