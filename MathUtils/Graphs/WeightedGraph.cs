using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    public class WeightedGraph : DirectedGraph
    {
        private double currentWeigth;

        public WeightedGraph()
        {
            SetDefaultEventHandlers( this );
            currentWeigth = 1;
        }

        public static void SetDefaultEventHandlers( WeightedGraph graph )
        {
            graph.ClearEventHandlers();
            DirectedGraph.SetDefaultEventHandlers(graph);
            
            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.WeightedGraph_OnAddEdge);
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new WeightedArc(this, u, v, this.currentWeigth);
        }

        void WeightedGraph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.AlreadyExist)
            {
                (e.Edge as WeightedArc).Weight += currentWeigth;
            }
        }

        public void AddArc(object tail, object head, double weight)
        {
            currentWeigth = weight;
            this.AddEdge(tail, head);
            currentWeigth = 1;
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
                            new DoubleMatrixElement(0) : new DoubleMatrixElement((this[VerticesValues[i], VerticesValues[j]] as WeightedArc).Weight);
                    }
                }
                return matrix;
            }
        }

        public override object Clone()
        {
            WeightedGraph w = new WeightedGraph();

            base.CopyTo(w);

            return w;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);
            WeightedGraph.SetDefaultEventHandlers( graph as WeightedGraph );
        }
    }
}
