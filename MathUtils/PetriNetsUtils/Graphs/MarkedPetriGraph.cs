using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class MarkedPetriGraph : PetriNetGraph
    {
        private int currentTokenCount;
        public MarkedPetriGraph()
            : base()
        {
            currentTokenCount = 0;

            MarkedPetriGraph.SetDefaultEventHandlers(this);
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new Transition(this, vertexValue.ToString()) as IVertex
                        : new MarkedPlace(this, vertexValue.ToString()) { TokenCount = this.currentTokenCount } as IVertex;
        }

        public void AddPlace(string name, int tokenCount)
        {
            currentTokenCount = tokenCount;
            AddPlace(name);
            currentTokenCount = 0;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            MarkedPetriGraph.SetDefaultEventHandlers(graph as MarkedPetriGraph);
        }

        public override object Clone()
        {
            MarkedPetriGraph graph = new MarkedPetriGraph();

            this.CopyTo(graph);

            return graph;
        }
    }
}
