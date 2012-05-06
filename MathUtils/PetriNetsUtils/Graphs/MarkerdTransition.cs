using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class MarkedTransition : Transition
    {
        public MarkedTransition(IGraph graph, string name) : base(graph, name) { }

        public MarkedPlace[] GetEnters()
        {
            List<MarkedPlace> l = new List<MarkedPlace>();
            Graph.GetEdges(e => (e as Arc).Head.Value.Equals(this.Value)).ForEach(edge => l.Add((edge as Arc).Tail as MarkedPlace));
            return l.ToArray();
        }

        public MarkedPlace[] GetExits()
        {
            List<MarkedPlace> l = new List<MarkedPlace>();
            Graph.GetEdges(e => (e as Arc).Tail.Value.Equals(this.Value)).ForEach(edge => l.Add((edge as Arc).Head as MarkedPlace));
            return l.ToArray();
        }

        public virtual bool IsAvailable()
        {
            bool f = true;// GetEnters(t).Length > 0;

            var enters = this.GetEnters();


            foreach (var p in enters)
            {
                if (p.TokensCount < (this.Graph[p.Value, this.Value] as WeightedArc).Weight)
                    f = false;
            }
            return f;
        }

        public virtual void Execute()
        {
            if (!IsAvailable())
                return;

            //foreach (var item in this.GetRemovingTokens())
            //{
            //    item.Key.RemoveTokens(item.Value);
            //}
            //foreach (var item in this.GetAddingTokens())
            //{
            //    item.Key.AddTokens(item.Value);
            //}

            GetEnters().ToList().ForEach(p =>
                p.TokensCount -= (uint)(Graph[p.Value, this.Value] as WeightedArc).Weight
            );
            GetExits().ToList().ForEach(p =>
                p.TokensCount += (uint)(Graph[this.Value, p.Value] as WeightedArc).Weight
            );
        }

        public virtual Dictionary<MarkedPlace, Token[]> GetRemovingTokens()
        {
            Dictionary<MarkedPlace, Token[]> tokens = new Dictionary<MarkedPlace, Token[]>();
            foreach (var p in this.GetEnters())
            {
                int count = (int)(Graph[p.Value, this.Value] as WeightedArc).Weight;
                //tokens[p] = p.Tokens.GetRange(0, count - 1).ToArray();
            }
            return tokens;
        }

        public virtual Dictionary<MarkedPlace, Token[]> GetAddingTokens()
        {
            Dictionary<MarkedPlace, Token[]> tokens = new Dictionary<MarkedPlace, Token[]>();

            foreach (var p in this.GetExits())
            {
                uint count = (uint)(Graph[this.Value, p.Value] as WeightedArc).Weight;

                //tokens[p] = new Token[] { new Token(new UnitColorSet("token"), "()", count) };
            }

            return tokens;
        }

        public override object Clone()
        {
            MarkedTransition t = new MarkedTransition(this.Graph, this.Name);

            this.CopyTo(t);

            return t;

        }
    }
}
