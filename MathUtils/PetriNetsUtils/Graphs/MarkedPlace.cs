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
        public MarkedPlace(IGraph graph, string name)
            : base(graph, name)
        {
            tokenCount = 0;
        }

        /// <summary>
        /// Количество фишек
        /// </summary>
        public int TokenCount { get { return tokenCount; } set { if (value >= 0) tokenCount = value; } }

        private int tokenCount;
    }
}
