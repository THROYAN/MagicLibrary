using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.SemanticNetworkUtils.Graphs
{
    [Serializable]
    public class ConceptVertex : Vertex
    {
        public SemanticGraph SemanticGraph { get { return this.Graph as SemanticGraph; } }

        public string Name { get { return this.Value.ToString(); } set { this.Value = value; } }

        private int id;

        public int Id
        {
            get { return id; }
            set { if (value == id) return; if (this.Graph.GetVertex(v => (v as ConceptVertex).id == value) == null) id = value; }
        }
        

        public ConceptVertex(SemanticGraph graph, int id, string name)
            : base(graph, name)
        {
            this.Id = id;
        }
    }
}
