using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class MarkedPlace : Place
    {
        public MarkedPlace(IGraph graph, string name, uint tokensCount)
            : base(graph, name)
        {
            this.TokensCount = tokensCount;
        }

        /// <summary>
        /// Количество фишек
        /// </summary>
        public uint TokensCount { get; set; }

        public override void CopyTo(IVertex vertex)
        {
            base.CopyTo(vertex);
            (vertex as MarkedPlace).TokensCount = this.TokensCount;
        }

        public override object Clone()
        {
            MarkedPlace m = new MarkedPlace(this.Graph, this.Name, this.TokensCount);
            this.CopyTo(m);
            return m;
        }
    }
}
