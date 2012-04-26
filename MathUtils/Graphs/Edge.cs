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

        private object u { get { return this.Vertices[0].Value; } set { this.Vertices[0].Value = value; } }
        private object v { get { return this.Vertices[1].Value; } set { this.Vertices[1].Value = value; } }

        public IVertex[] Vertices
        {
            get;// { return new IVertex[] { this.Graph[u], this.Graph[v] }; }
            set;
            //{
            //    this.u = value[0].Value;
            //    this.v = value[1].Value;
            //}
        }

        public Edge(IGraph graph, object u, object v)
        {
            this.Graph = graph;
            //this.u = u;
            //this.v = v;
            this.Vertices = new IVertex[2];
            this.Vertices[0] = this.Graph[u];
            this.Vertices[1] = this.Graph[v];
        }

        public virtual void CopyTo(IEdge edge)
        {
            edge.Graph = this.Graph;
            var e = edge as Edge;
            e.Vertices[0] = this.Vertices[0];
            e.Vertices[1] = this.Vertices[1];
        }

        public virtual object Clone()
        {
            Edge e = new Edge(this.Graph, this.u, this.v);
            this.CopyTo(e);
            return e;
        }

        public override string ToString()
        {
            return String.Format("[{0}-{1}]", this.Vertices[0].Value, this.Vertices[1].Value);
        }
    }
}
