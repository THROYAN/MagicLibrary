using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class Vertex : IVertex
    {
        public IGraph Graph { get; set; }

        private object value;
        public object Value { get { return value; } set { if (this.value.Equals(value)) return; if (Graph.GetVertices(v => v.Value.Equals(value)).Count == 0) this.value = value; } }

        public Vertex(IGraph graph, object value)
        {
            this.value = value;
            this.Graph = graph;
        }

        public int Degree
        {
            get
            {
                int degree = 0;
                for (int i = 0; i < Graph.Order; i++)
                {
                    if (Graph[this,Graph.VerticesValues[i]] != null)
                        degree++;
                    if (Graph[Graph.VerticesValues[i], this] != null)
                        degree++;
                }
                return degree;
            }
        }

        public virtual void CopyTo(out IVertex vertex)
        {
            vertex = new Vertex(this.Graph, this.Value);
            vertex.Graph = this.Graph;
            (vertex as Vertex).value = this.value;
        }

        public virtual void CopyTo(IVertex vertex)
        {
            vertex.Graph = this.Graph;
            (vertex as Vertex).value = this.value;
        }

        public virtual object Clone()
        {
            return new Vertex(this.Graph, this.Value);
        }
    }
}
