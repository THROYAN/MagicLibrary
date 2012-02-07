using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class Transition : Vertex
    {
        public Transition(IGraph graph,string name) : base(graph,name) { }

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
            GetEnters().ToList().ForEach(delegate(MarkedPlace p)
            {
                if (p.TokenCount < (this.Graph[p.Value, this.Value] as WeightedArc).Weight)
                    f = false;
            });
            return f;
        }

        public virtual void Execute()
        {
            if (!IsAvailable())
                return;
            GetEnters().ToList().ForEach(p => 
                p.TokenCount -= (int)(Graph[p.Value, this.Value] as WeightedArc).Weight
            );
            GetExits().ToList().ForEach(p => 
                p.TokenCount += (int)(Graph[this.Value, p.Value] as WeightedArc).Weight
            );
        }
    }
}
