using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    /// <summary>
    /// Двудольный граф.
    /// Граф имеет 2 подмножества вершин. Вершины одного подмножества могут соединяться только с вершинами другого подможества.
    /// По-умолчанию вершины добавляются в первое подмножество.
    /// </summary>
    public class BiGraph : DirectedGraph
    {
        protected List<IVertex> firstPart;
        protected List<IVertex> secondPart;

        public int FirstPartOrder
        {
            get
            {
                return this.firstPart.Count;
            }
        }

        public int SecondPartOrder
        {
            get
            {
                return this.secondPart.Count;
            }
        }

        protected bool addToSecond;

        public BiGraph() : base()
        {
            firstPart = new List<IVertex>();
            secondPart = new List<IVertex>();
            addToSecond = false;

            SetDefaultEventHandlers(this);
        }

        public static void  SetDefaultEventHandlers( BiGraph graph )
        {
            DirectedGraph.SetDefaultEventHandlers( graph );

            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.BiGraph_OnAddEdge);
            graph.OnVertexAdded += new EventHandler<VerticesModifiedEventArgs>(graph.graph_OnVertexAdded);
            graph.OnVertexRemoved += new EventHandler<VerticesModifiedEventArgs>(graph.graph_OnVertexRemoved);
        }

        void graph_OnVertexRemoved(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                firstPart.Remove(e.Vertex);
                secondPart.Remove(e.Vertex);
            }
        }

        void graph_OnVertexAdded(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if (addToSecond)
                    secondPart.Add(e.Vertex);
                else
                    firstPart.Add(e.Vertex);
            }
        }

        void BiGraph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                Arc arc = e.Edge as Arc;
                if ((firstPart.Exists(v => v.Value.Equals(arc.Head.Value)) && firstPart.Exists(v => v.Value.Equals(arc.Tail.Value)))
                        || (secondPart.Exists(v => v.Value.Equals(arc.Head.Value)) && secondPart.Exists(v => v.Value.Equals(arc.Tail.Value))))
                {
                    e.Status = ModificationStatus.Error;
                }
            }
        }

        public void AddVertexToFirstPart(object vertexValue)
        {
            addToSecond = false;
            AddVertex(vertexValue);
        }

        public void AddVertexToSecondPart(object vertexValue)
        {
            addToSecond = true;
            AddVertex(vertexValue);
            addToSecond = false;
        }

        public IVertex GetVertexFromFirstPart(object vertexValue)
        {
            return firstPart.Find(v => v.Value == vertexValue);
        }

        public IVertex GetVertexFromSecondPart(object vertexValue)
        {
            return secondPart.Find(v => v.Value == vertexValue);
        }

        public List<IVertex> GetVerticesFromFirstPart(Predicate<IVertex> match)
        {
            return firstPart.FindAll(match);
        }

        public List<IVertex> GetVerticesFromFirstPart()
        {
            return firstPart.ToList();
        }

        public List<IVertex> GetVerticesFromSecondPart(Predicate<IVertex> match)
        {
            return secondPart.FindAll(match);
        }

        public List<IVertex> GetVerticesFromSecondPart()
        {
            return secondPart.ToList();
        }

        public object[] FirstPartVerticesValues
        {
            get
            {
                return this.firstPart.Select(v => v.Value).ToArray();
            }
        }

        public object[] SecondPartVerticesValues
        {
            get
            {
                return this.secondPart.Select(v => v.Value).ToArray();
            }
        }



        //public override Matrix IncidentsMatrix
        //{
        //    get
        //    {
        //        Matrix matrix = new Matrix(this.firstPart.Count, this.secondPart.Count);
        //        for (int i = 0; i < this.firstPart.Count; i++)
        //        {
        //            for (int j = 0; j < this.secondPart.Count; j++)
        //            {
        //                WeightedArc arc = this[this.firstPart[i].Value, this.secondPart[j].Value] as WeightedArc;
        //                matrix[j, i] = arc == null ?
        //                    new DoubleMatrixElement(0) : new DoubleMatrixElement(arc.Weight);
        //            }
        //        }
        //        return matrix;
        //    }
        //}

        //public override object[] IncidentsMatrixTopHeaders
        //{
        //    get
        //    {
        //        return this.firstPart.Select( v => v.Value).ToArray();
        //    }
        //}

        //public override object[] IncidentsMatrixLeftHeaders
        //{
        //    get
        //    {
        //        return this.secondPart.Select( v => v.Value ).ToArray();
        //    }
        //}

        public override object Clone()
        {
            BiGraph b = new BiGraph();

            this.CopyTo(b);

            return b;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            BiGraph b = graph as BiGraph;

            b.firstPart = this.firstPart.ToList();
            b.secondPart = this.secondPart.ToList();
            b.addToSecond = false;

            BiGraph.SetDefaultEventHandlers(b);
        }
    }

    [Serializable]
    public class WeightedBiGraph : BiGraph
    {
        private double currentWeigth;

        public WeightedBiGraph() : base()
        {
            currentWeigth = 1;
            SetDefaultEventHandlers(this);
        }

        public static void SetDefaultEventHandlers( WeightedBiGraph graph )
        {
            BiGraph.SetDefaultEventHandlers(graph);

            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.WeightedBiGraph_OnAddEdge);
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new WeightedArc(this, u, v);
        }

        void WeightedBiGraph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.AlreadyExist)
            {
                (e.Edge as WeightedArc).Weight += currentWeigth;
            }
        }

        public void AddArc(object tail, object head, double weight)
        {
            currentWeigth = weight;
            AddArc(tail, head);
            currentWeigth = 1;
        }

        public override object Clone()
        {
            WeightedBiGraph wb = new WeightedBiGraph();

            this.CopyTo( wb );

            return wb;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo( graph );

            WeightedBiGraph wb = graph as WeightedBiGraph;

            wb.currentWeigth = 1;

            WeightedBiGraph.SetDefaultEventHandlers( graph as WeightedBiGraph );
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
    }
}
