using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class Place : Vertex
    {
        /*
         *   а зачем я выше писал две звездочки?
         *   Это LEGO!
         *   Что это блеать за комменты?
         */
        public Place(IGraph graph, string name) : base(graph,name) { }

        //public override void Draw(Graphics g, Pen p)
        //{
        //    base.Draw(g, p);
        //}
    }
}
