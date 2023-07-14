using System;
using UnityEngine;

namespace BetterAnimationPipeline
{
	[Serializable]
	public class Edge
	{
		public Edge(NodeSlot to, NodeSlot from)
		{
			this.to = to;
			this.from = from;
		}

		public NodeSlot to;
		public NodeSlot from;
	}
}