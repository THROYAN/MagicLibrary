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

        public static void SetDefaultEventHandlers(MarkedPetriGraph graph)
        {
            PetriNetGraph.SetDefaultEventHandlers(graph);

            graph.OnAddVertex += new EventHandler<VerticesModifiedEventArgs>(graph.MarkedPetriGraph_OnAddVertex);
        }

        void MarkedPetriGraph_OnAddVertex(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if (e.Vertex is Place)
                {
                    e.Vertex = new MarkedPlace(this, e.Vertex.Value as string) { TokenCount = currentTokenCount };
                }
            }
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
