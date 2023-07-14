namespace BetterAnimationPipeline
{
	[AnimationNodeCategory("Parameter", 1)]
	public class ParameterNode : Node
	{
		[OutputSlot(name = "Value")]
		private float value;

//		[EmbeddedSlot(name = "Parameter")]
		private string parameterName;

		public ParameterNode(int id) : base(id)
		{
			parameterName = "Test";
		}

		public override void Update(AnimationGraph graph)
		{
			value = graph.teste;
			//value = (float) graph.parameters[parameterName];
		}
	}
}