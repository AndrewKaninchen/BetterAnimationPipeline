using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace BetterAnimationPipeline
{
	public class AnimationNode<TJob, TData, TBinder>
		where TJob : struct, IAnimationJob
		where TData : struct, IAnimationJobData
		where TBinder : AnimationJobBinder<TJob, TData>, new()
	{
	}
	
	public interface IAnimationJobBinder
	{
		IAnimationJob Create(TestAnimator testAnimator, IAnimationJobData data);
		void Destroy(IAnimationJob job);
		void Update(IAnimationJob job, IAnimationJobData data);
		AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job);
	}

	public abstract class AnimationJobBinder<TJob, TData> : IAnimationJobBinder
		where TJob : struct, IAnimationJob
		where TData : struct, IAnimationJobData
	{
		public abstract TJob Create(TestAnimator testAnimator, ref TData data);

		public abstract void Destroy(TJob job);

		public virtual void Update(TJob job, ref TData data) {}

		IAnimationJob IAnimationJobBinder.Create(TestAnimator testAnimator, IAnimationJobData data)
		{
			Debug.Assert(data is TData);
			var tData = (TData)data;
			return Create(testAnimator, ref tData);
		}

		void IAnimationJobBinder.Destroy(IAnimationJob job)
		{
			Debug.Assert(job is TJob);
			Destroy((TJob)job);
		}

		void IAnimationJobBinder.Update(IAnimationJob job, IAnimationJobData data)
		{
			Debug.Assert(data is TData && job is TJob);
			var tData = (TData)data;
			Update((TJob)job, ref tData);
		}

		AnimationScriptPlayable IAnimationJobBinder.CreatePlayable(PlayableGraph graph, IAnimationJob job)
		{
			Debug.Assert(job is TJob);
			return AnimationScriptPlayable.Create(graph, (TJob)job);
		}
	}
}