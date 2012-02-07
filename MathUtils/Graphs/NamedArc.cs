using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    public class NamedArc : Arc
    {
        public string Name { get; set; }

        public NamedArc(IGraph graph, object tail, object head, string name)
            : base(graph, tail, head)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
