using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class DirectedGraph : UndirectedGraph
    {
        public DirectedGraph()
            : base()
        {
            SetDefaultEventHandlers( this );
        }

        public static void SetDefaultEventHandlers( DirectedGraph graph )
        {
            graph.ClearEventHandlers();

            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.DirectedGraph_OnAddEdge);
            graph.OnRemoveEdge += new EventHandler<EdgesModifiedEventArgs>(graph.DirectedGraph_OnRemoveEdge);
        }

        void DirectedGraph_OnRemoveEdge(object sender, EdgesModifiedEventArgs e)
        {
            DirectedGraph graph = sender as DirectedGraph;
            Arc arc = e.Edge as Arc;

            if (e.Status == ModificationStatus.Successful)
            {
                arc = graph.GetEdge(edge => (edge as Arc).Head.Value == e.v && (edge as Arc).Tail.Value == e.u) as Arc;
                if (arc == null)
                    e.Status = ModificationStatus.NotExist;
            }
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new Arc(this, u, v);
        }

        void DirectedGraph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if (this[e.u, e.v] != null)
                {
                    e.Status = ModificationStatus.AlreadyExist;
                    e.Edge = this[e.u, e.v];
                }
            }
        }

        public new Arc this[object tail, object head]
        {
            get
            {
                return GetEdge(edge => (edge as Arc).Head.Value.Equals(head) && (edge as Arc).Tail.Value.Equals(tail)) as Arc;
            }
        }

        //private new void AddEdge(object u, object v)
        //{
        //    base.AddEdge(u, v);
        //}

        public void AddArc(object tail, object head)
        {
            AddEdge(tail, head);
        }

        public void RemoveArc(object tail, object head)
        {
            RemoveEdge(tail, head);
        }

        /// <summary>
        /// Удаление всех рёбер, входящих и выходящих из вершины.
        /// </summary>
        /// <param name="vertexValue"></param>
        public void RemoveAllArcs(object vertexValue)
        {
            RemoveEdges(vertexValue);
        }

        /// <summary>
        /// Удаление всех дуг, для которых вершина является хвостом
        /// </summary>
        /// <param name="vertexValue">"Содержимое" вершин"</param>
        public void RemoveOutArcs(object vertexValue)
        {
            GetEdges(a => (a as Arc).Tail.Value == vertexValue)
                .ForEach(arc => RemoveArc((arc as Arc).Tail.Value, (arc as Arc).Head.Value));
        }

        /// <summary>
        /// Удаление всех дуг, для которых вершина является головой
        /// </summary>
        /// <param name="vertexValue"></param>
        public void RemoveInArcs(object vertexValue)
        {
            GetEdges(a => (a as Arc).Head.Value == vertexValue)
                .ForEach(arc => RemoveArc((arc as Arc).Tail.Value, (arc as Arc).Head.Value));
        }

        public override Matrix IncidentsMatrix
        {
            get
            {
                Matrix matrix = new Matrix(VerticesValues.Length, VerticesValues.Length);
                for (int i = 0; i < VerticesValues.Length; i++)
                {
                    for (int j = 0; j < VerticesValues.Length; j++)
                    {
                        matrix[i, j] = this[VerticesValues[i], VerticesValues[j]] == null ?
                                            //this[VerticesValues[j], VerticesValues[i]] == null ?
                                                new DoubleMatrixElement(0) :
                                                //new DoubleMatrixElement(-1) :
                                            new DoubleMatrixElement(1);
                    }
                }
                return matrix;
            }
        }

        public override object Clone()
        {
            DirectedGraph d = new DirectedGraph();

            this.CopyTo(d);

            return d;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);
            DirectedGraph.SetDefaultEventHandlers( graph as DirectedGraph );
        }

        public override IEdge[] FindPath(object from, object to)
        {
            return this.findPath(from, to, null);
        }

        private IEdge[] findPath(object from, object to, IEdge[] path)
        {
            if (from.Equals(to))
            {
                return path;
            }
            foreach (var e in this.GetOutArcs(from))
            {
                if (path == null || !path.Contains(e))
                {
                    List<IEdge> newPath = new List<IEdge>();
                    if (path != null)
                        newPath = new List<IEdge>(path);
                    newPath.Add(e);
                    var res = this.findPath(e.Vertices[1].Value, to, newPath.ToArray());
                    if (res != null)
                        return res;
                }
            }
            return null;
        }

        public List<IEdge> GetOutArcs(object v)
        {
            return this.GetEdges(e => (e as Arc).Tail.Value.Equals(v));
        }

        public List<IEdge> GetInArcs(object v)
        {
            return this.GetEdges(e => (e as Arc).Head.Value.Equals(v));
        }
    }
}
