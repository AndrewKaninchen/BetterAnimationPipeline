using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	public class AnimationJobNode<TJob, TJobData>
		where TJob : struct, IAnimationJob
	{
		private TJob job;
		private TJobData jobData;
		
		private AnimationScriptPlayable playable;

		public void Initialize(PlayableGraph graph)
		{
			job = new TJob();
			playable = AnimationScriptPlayable.Create(graph, job);
		}

		public void Update()
		{
		}
	}
}
