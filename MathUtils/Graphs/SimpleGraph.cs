using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    /// <summary>
    /// Простой граф - неориентированный граф,
    /// не содержащий петель и между любыми разными вершинами
    /// может быть только 1 ребро
    /// </summary>
    public class SimpleGraph : UndirectedGraph
    {
        public SimpleGraph()
        {
            SetDefaultEventHandlers( this );
        }

        public static void SetDefaultEventHandlers( SimpleGraph graph )
        {
            graph.ClearEventHandlers();

            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.SimpleGraph_OnAddEdge);
        }

        void SimpleGraph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                object u = e.Edge.Vertices[0].Value, v = e.Edge.Vertices[1].Value;
                if (this[u, v] != null || u == v)
                    e.Status = ModificationStatus.Error;
            }
        }
        //new public void AddEdge(object u, object v)
        //{
        //    if (this[u, v] == null && u != v)
        //    {
        //        base.AddEdge(u, v);
        //    }
        //}

        public override object Clone()
        {
            SimpleGraph s = new SimpleGraph();

            base.CopyTo(s);

            return s;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            SimpleGraph.SetDefaultEventHandlers( graph as SimpleGraph );
        }
    }
}
