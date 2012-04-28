using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    public class ColouredPetriGraph : PetriNetGraph
    {
        public ColorSetCollection Colors { get; private set; }

        public ColouredPetriGraph(ColorSetCollection colors = null)
            : base()
        {
            if (colors == null)
            {
                this.Colors = new ColorSetCollection();
            }
            else
            {
                this.Colors = colors;
            }
        }

        public override MathUtils.Graphs.IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new Transition(this, vertexValue.ToString()) as IVertex
                        : new ColouredPlace(this, vertexValue.ToString()) as IVertex;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            var pGraph = graph as ColouredPetriGraph;
            pGraph.Colors = this.Colors;
        }

        public override object Clone()
        {
            ColouredPetriGraph g = new ColouredPetriGraph();

            this.CopyTo(g);

            return g;
        }

        /// <summary>
        /// Сбрасывает маркировка на начальную.
        /// У каждой позиции - это функция инициализации.
        /// </summary>
        public void ResetMarking()
        {
            foreach (ColouredPlace place in this.GetVertices())
            {
                place.ResetMarking();
            }
        }
    }
}
