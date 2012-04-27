using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    public class ColouredPetriGraph : PetriNetGraph
    {
        private ColorSetCollection colors { get; set; }

        public ColouredPetriGraph(ColorSetCollection colors = null)
            : base()
        {
            if (colors == null)
            {
                this.colors = new ColorSetCollection();
            }
            else
            {
                this.colors = colors;
            }
        }

        public override MathUtils.Graphs.IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new Transition(this, vertexValue.ToString()) as IVertex
                        : new ColouredPlace(this, vertexValue.ToString(), this.colors) as IVertex;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            var pGraph = graph as ColouredPetriGraph;
            pGraph.colors = this.colors;
        }

        public override object Clone()
        {
            ColouredPetriGraph g = new ColouredPetriGraph();

            this.CopyTo(g);

            return g;
        }
    }
}
