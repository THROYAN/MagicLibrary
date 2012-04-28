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
        public ColorSet ColorSet { get; set; }

        public MarkedPlace(IGraph graph, string name, uint tokensCount)
            : base(graph, name)
        {
            this.TokensCount = tokensCount;
        }

        /// <summary>
        /// Количество фишек
        /// </summary>
        public uint TokensCount { get; set; }
    }
}
