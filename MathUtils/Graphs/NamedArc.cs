using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class NamedArc : Arc
    {
        protected string _name;
        public string Name { get { return this._name; } set { this._name = value; this._setName(value); } }

        public NamedArc(IGraph graph, object tail, object head, string name)
            : base(graph, tail, head)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }

        protected virtual void _setName(string name)
        {
            
        }

        public override void CopyTo(IEdge edge)
        {
            base.CopyTo(edge);
            (edge as NamedArc)._name = this._name;
        }

        public override object Clone()
        {
            var n = new NamedArc(this.Graph, this.Tail.Value, this.Head.Value, this._name);
            this.CopyTo(n);
            return n;
        }
    }
}
