using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class WeightedArc : Arc
    {
        public double Weight { get; set; }

        public WeightedArc(IGraph graph, object tail, object head) : base(graph, tail, head) { Weight = 1; }
        public WeightedArc(IGraph graph, object tail, object head, double weight) : base(graph, tail, head) { Weight = weight; }

        public override void CopyTo(IEdge edge)
        {
            base.CopyTo(edge);

            (edge as WeightedArc).Weight = this.Weight;
        }

        public override object Clone()
        {
            return new WeightedArc(this.Graph, this.Tail.Value, this.Head.Value, this.Weight);
        }
    }
}
