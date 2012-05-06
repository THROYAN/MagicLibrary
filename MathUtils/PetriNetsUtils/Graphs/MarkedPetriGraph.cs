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
        private uint currentTokenCount;
        private double _currentWeight;

        public MarkedPetriGraph()
            : base()
        {
            currentTokenCount = 0;
            this._currentWeight = 1;

            MarkedPetriGraph.SetDefaultEventHandlers(this);
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new MarkedTransition(this, vertexValue.ToString()) as IVertex
                        : new MarkedPlace(this, vertexValue.ToString(), this.currentTokenCount) as IVertex;
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new WeightedArc(this, u, v, this._currentWeight);
        }

        public void AddPlace(string name, uint tokenCount)
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

        public void AddArc(string tail, string head, double weight)
        {
            this._currentWeight = weight;
            this.AddArc(tail, head);
            this._currentWeight = 1;
        }
    }
}
