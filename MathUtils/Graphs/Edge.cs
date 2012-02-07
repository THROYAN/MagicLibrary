using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class Edge : IEdge
    {
        public IGraph Graph { get; set; }

        public IVertex[] Vertices { get; set; }

        public Edge(IGraph graph, object u, object v)
        {
            this.Graph = graph;
            this.Vertices = new IVertex[2];
            this.Vertices[0] = Graph[u];
            this.Vertices[1] = Graph[v];
        }

        public void CopyTo(out IEdge edge)
        {
            edge = this.Clone() as Edge;
        }

        public virtual void CopyTo(IEdge edge)
        {
            edge.Graph = this.Graph;
            edge.Vertices = this.Vertices;
        }

        public virtual object Clone()
        {
            return new Edge(this.Graph, this.Vertices[0], this.Vertices[1]);
        }
    }
}
