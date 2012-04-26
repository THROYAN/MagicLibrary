using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class Arc : Edge
    {
        /// <summary>
        /// "Голова" дуги - вершина, в которую входит дуга
        /// </summary>
        public IVertex Head
        {
            get
            {
                return this.Vertices[1];
            }
            set
            {
                this.Vertices[1] = value;
            }
        }

        /// <summary>
        /// "Хвост" дуги - вершина, из которой исходит дуга
        /// </summary>
        public IVertex Tail
        {
            get
            {
                return this.Vertices[0];
            }
            set
            {
                this.Vertices[0] = value;
            }
        }

        public Arc(IGraph graph, object tail, object head) : base(graph, tail, head) { }

        public override string ToString()
        {
            return String.Format("[{0}->{1}]", this.Tail.Value, this.Head.Value);
        }
    }
}
