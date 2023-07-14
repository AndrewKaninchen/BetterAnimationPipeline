using UnityEngine;

using UnityEngine.Playables;

using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]

public class MixAnimationSample : MonoBehaviour

{

	public AnimationClip clip0;

	public AnimationClip clip1;

	[Range(0f, 1f)]
	public float weight;

	private PlayableGraph playableGraph;

	private AnimationMixerPlayable mixerPlayable;

	AnimationClipPlayable clipPlayable0;
	AnimationClipPlayable clipPlayable1;
		
		
	private void OnEnable()

	{
		// Creates the graph, the mixer and binds them to the Animator.

		playableGraph = PlayableGraph.Create();
		mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);

		var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());
		playableOutput.SetSourcePlayable(mixerPlayable);

		// Creates AnimationClipPlayable and connects them to the mixer.
		
		clipPlayable0 = AnimationClipPlayable.Create(playableGraph, clip0);
		clipPlayable1 = AnimationClipPlayable.Create(playableGraph, clip1);

		playableGraph.Connect(clipPlayable0, 0, mixerPlayable, 0);
		playableGraph.Connect(clipPlayable1, 0, mixerPlayable, 1);

		// Plays the Graph.

		playableGraph.Play();

	}

	private void Update()

	{
		clipPlayable0.SetTime(.1f);
		mixerPlayable.SetInputWeight(0, 1.0f-weight);

		mixerPlayable.SetInputWeight(1, weight);
	}

	private void OnDisable()

	{

		// Destroys all Playables and Outputs created by the graph.

		playableGraph.Destroy();

	}

}
