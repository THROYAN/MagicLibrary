using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class ColouredArc : FunctionArc
    {
        public new ColouredPetriGraph Graph { get { return base.Graph as ColouredPetriGraph; } set { base.Graph = value; } }
        public MultiSet<Function> Tokens { get; set; }
        public ColorSet Color
        {
            get
            {
                var tail = this.Tail as ColouredPlace;
                var head = this.Head as ColouredPlace;
                if (tail != null)
                {
                    return tail.ColorSet;
                }
                return head.ColorSet;
            }
        }

        public ColouredArc(ColouredPetriGraph graph, string tail, string head, string function)
            : base(graph, tail, head, function)
        { }

        protected override void _setName(string name)
        {
            base._setName(name);
            if (String.IsNullOrEmpty(name))
            {
                return;
            }

            var cs = this.Color;
            if (cs == null)
            {
                this._func = null;
                this.Tokens = null;
                return;
            }

            if (this.ParsedFunction != null)
            {
                this.Tokens = new MultiSet<Function>(this.ParsedFunction);
                foreach (var token in this.Tokens)
                {
                    Token t = new Token(token.Key, cs);
                    if (!t.IsLegal())
                    {
                        this._func = null;
                        this.Tokens = null;
                        break;
                    }
                }
            }
        }

        public override void CopyTo(MathUtils.Graphs.IEdge edge)
        {
            base.CopyTo(edge);
            if (this.Tokens != null)
            {
                (edge as ColouredArc).Tokens = new MultiSet<Function>(this.Tokens);
            }
        }

        public override object Clone()
        {
            var ca = new ColouredArc(this.Graph, this.Tail.Value.ToString(), this.Head.Value.ToString(), this.Name);
            this.CopyTo(ca);
            return ca;
        }
    }
}
