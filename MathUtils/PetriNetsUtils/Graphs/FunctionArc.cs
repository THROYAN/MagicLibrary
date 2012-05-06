using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class FunctionArc : NamedArc
    {
        protected Function _func;
        public Function ParsedFunction { get { return this._func; } }

        public FunctionArc(IGraph graph, object tailValue, object headValue, string func)
            : base(graph, tailValue, headValue, func) { }

        protected override void _setName(string name)
        {
            base._setName(name);
            Function.TryParse(name, out this._func);
        }

        public override void CopyTo(IEdge edge)
        {
            base.CopyTo(edge);
            (edge as FunctionArc)._func = this._func;
        }

        public override object Clone()
        {
            var f = new FunctionArc(this.Graph, this.Tail.Value, this.Head.Value, this._name);
            this.CopyTo(f);
            return f;
        }
    }
}
