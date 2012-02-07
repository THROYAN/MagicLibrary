using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    [Serializable]
    /// <summary>
    /// Неориентированный граф
    /// </summary>
    public class UndirectedGraph : IGraph
    {
        /// <summary>
        /// Список вершин
        /// </summary>
        private List<Vertex> vertices { get; set; }

        /// <summary>
        /// Список рёбер
        /// </summary>
        private List<Edge> edges { get; set; }

        public UndirectedGraph()
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();
        }

        public int Order
        {
            get { return vertices.Count; }
        }

        public int Size
        {
            get { return edges.Count; }
        }

        public object[] VerticesValues
        {
            get
            {
                object[] values = new object[vertices.Count];
                int i = 0;
                vertices.ForEach(v => values[i++] = v.Value);
                return values;
            }
        }

        public IVertex this[object vertexValue]
        {
            get
            {
                return vertices.Find(v => v.Value.Equals(vertexValue));
            }
        }

        public IEdge this[object uValue, object vValue]
        {
            get
            {
                return edges.Find(e =>
                    (e.Vertices[0].Value.Equals(uValue) && e.Vertices[1].Value.Equals(vValue)) ||
                    (e.Vertices[1].Value.Equals(uValue) && e.Vertices[0].Value.Equals(vValue)));
            }
        }

        public void AddVertex(object vertexValue)
        {
            ModificationStatus status = !vertices.Exists(v => v.Value.Equals(vertexValue)) ? ModificationStatus.Successful : ModificationStatus.AlreadyExist;

            VerticesModifiedEventArgs e = new VerticesModifiedEventArgs(status, new Vertex(this,vertexValue),vertexValue);
            if (OnAddVertex != null)
                OnAddVertex(this, e);

            if (e.Status == ModificationStatus.Successful)
                vertices.Add(e.Vertex as Vertex);

            if (OnVertexAdded != null)
                OnVertexAdded(this, e);
        }

        public void RemoveVertex(object vertexValue)
        {
            ModificationStatus status = vertices.Exists(v => v.Value.Equals(vertexValue)) ? ModificationStatus.Successful : ModificationStatus.NotExist;

            VerticesModifiedEventArgs e = new VerticesModifiedEventArgs(status, this[vertexValue],vertexValue);
            if (OnRemoveVertex != null)
                OnRemoveVertex(this, e);

            if (e.Status == ModificationStatus.Successful)
            {
                RemoveEdges(e.Vertex.Value);
                vertices.Remove(e.Vertex as Vertex);
            }

            if (OnVertexRemoved != null)
                OnVertexRemoved(this, e);
        }

        public void AddEdge(object u, object v)
        {
            ModificationStatus status = ModificationStatus.Successful;

            if (!(vertices.Exists(vert => vert.Value.Equals(v)) && vertices.Exists(vert => vert.Value.Equals(u))))
                status = ModificationStatus.InvalidParameters;

            Edge edge = new Edge(this, u, v);
            EdgesModifiedEventArgs e = new EdgesModifiedEventArgs(status, edge, u, v);

            if (OnAddEdge != null)
                OnAddEdge(this, e);

            if (e.Status == ModificationStatus.Successful)
                edges.Add(e.Edge as Edge);

            if (OnEdgeAdded != null)
                OnEdgeAdded(this, e);
        }

        public void RemoveEdge(object u, object v)
        {
            Edge edge = edges.Find(e =>
                                (e.Vertices[0].Value.Equals(u) && e.Vertices[1].Value.Equals(v)) ||
                                (e.Vertices[1].Value.Equals(u) && e.Vertices[0].Value.Equals(v)));

            ModificationStatus status = edge != null ? ModificationStatus.Successful : ModificationStatus.NotExist;
            EdgesModifiedEventArgs eArgs = new EdgesModifiedEventArgs(status, edge, u, v);

            if (OnRemoveEdge!= null)
                OnRemoveEdge(this, eArgs);

            if (eArgs.Status == ModificationStatus.Successful)
                edges.Remove(eArgs.Edge as Edge);

            if (OnEdgeRemoved != null)
                OnEdgeRemoved(this, eArgs);
        }

        public void RemoveEdges(object vertexValue)
        {
            edges.FindAll(e => e.Vertices[0].Value == vertexValue || e.Vertices[1].Value == vertexValue)
                .ForEach(e => RemoveEdge(e.Vertices[0].Value, e.Vertices[1].Value));
        }
        
        public IVertex GetVertex(Predicate<IVertex> match)
        {
            return vertices.Find(match);
        }

        public IEdge GetEdge(Predicate<IEdge> match)
        {
            return edges.Find(match);
        }


        public List<IVertex> GetVertices(Predicate<IVertex> match)
        {
            return vertices.FindAll(match).ToList<IVertex>();
        }

        public List<IEdge> GetEdges(Predicate<IEdge> match)
        {
            return edges.FindAll(match).ToList<IEdge>();
        }


        public List<IVertex> GetVertices()
        {
            return vertices.ToList<IVertex>();
        }

        public List<IEdge> GetEdges()
        {
            return edges.ToList<IEdge>();
        }


        public virtual void CopyTo(out IGraph graph)
        {
            graph = new UndirectedGraph();
            UndirectedGraph g = graph as UndirectedGraph;
            g.vertices = new List<Vertex>(this.vertices);
            g.edges = new List<Edge>(this.edges);
        }


        public virtual Matrix IncidentsMatrix
        {
            get
            {
                Matrix matrix = new Matrix(vertices.Count, vertices.Count);
                for (int i = 0; i < vertices.Count; i++)
                {
                    for (int j = 0; j < vertices.Count; j++)
                    {
                        matrix[i, j] = this[vertices[i].Value, vertices[j].Value] != null ? new DoubleMatrixElement(1) : new DoubleMatrixElement(0);
                    }
                }
                return matrix;
            }
        }

        public virtual object[] IncidentsMatrixTopHeaders
        {
            get
            {
                return this.VerticesValues;
            }
        }

        public virtual object[] IncidentsMatrixLeftHeaders
        {
            get
            {
                return this.VerticesValues;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int leftLenght = this.IncidentsMatrixLeftHeaders.Max(l => l.ToString().Length);
            int topLenght = this.IncidentsMatrixTopHeaders.Max(t => t.ToString().Length);
            int j = 0;
            Matrix m = this.IncidentsMatrix;
            for (int i = 0; i < m.Rows; i++)
            {
                for (j = 0; j < m.Cols; j++)
                {
                    if (m[i, j].ToString().Length > topLenght)
                        topLenght = m[i, j].ToString().Length;
                }
            }

            sb.AppendFormat("|{0," + leftLenght + "}|", "\\");
            foreach (string header in this.IncidentsMatrixTopHeaders)
            {
                sb.AppendFormat("{0," + topLenght + "}|", header);
            }
            sb.AppendLine();

            j = 0;
            foreach (string header in this.IncidentsMatrixLeftHeaders)
            {
                sb.AppendFormat("|{0," + leftLenght + "}|", header);

                for (int i = 0; i < m.Cols; i++)
                {
                    sb.AppendFormat("{0," + topLenght + "}|", m[j, i]);
                }
                sb.AppendLine();
                j++;
            }

            return sb.ToString();
        }

        public virtual object Clone()
        {
            return new UndirectedGraph()
            {
                edges = this.edges.ToList(),
                vertices = this.vertices.ToList()
                //OnAddEdge = this.OnAddEdge.Clone() as EventHandler,
                //OnAddVertex = this.OnAddVertex.Clone() as EventHandler,
                //OnRemoveEdge = this.OnRemoveEdge.Clone() as EventHandler,
                //OnRemoveVertex = this.OnRemoveVertex.Clone() as EventHandler
            };
        }

        public virtual void CopyTo(IGraph graph)
        {

            (graph as UndirectedGraph).edges = new List<Edge>(this.edges);
            (graph as UndirectedGraph).vertices = new List<Vertex>(this.vertices);

            graph.GetVertices().ForEach(v => v.Graph = graph);
            graph.GetEdges().ForEach(e => e.Graph = graph);
            //graph.GraphMerge(this);
            
        }

        public void ClearEventHandlers()
        {
            this.OnAddEdge = null;
            this.OnAddVertex = null;
            this.OnRemoveEdge = null;
            this.OnRemoveVertex = null;
            this.OnVertexAdded = null;
            this.OnVertexRemoved = null;
            this.OnEdgeAdded = null;
            this.OnEdgeRemoved = null;
        }

        public event EventHandler<VerticesModifiedEventArgs> OnAddVertex;

        public event EventHandler<VerticesModifiedEventArgs> OnRemoveVertex;

        public event EventHandler<VerticesModifiedEventArgs> OnVertexAdded;

        public event EventHandler<VerticesModifiedEventArgs> OnVertexRemoved;

        public event EventHandler<EdgesModifiedEventArgs> OnAddEdge;

        public event EventHandler<EdgesModifiedEventArgs> OnRemoveEdge;

        public event EventHandler<EdgesModifiedEventArgs> OnEdgeAdded;

        public event EventHandler<EdgesModifiedEventArgs> OnEdgeRemoved;

        public IGraph GetMergedCopy(IGraph graph)
        {
            IGraph newGraph = this.Clone() as IGraph;

            newGraph.GraphMerge(graph);

            return newGraph;
        }

        public virtual void GraphMerge(IGraph graph)
        {
            graph.GetVertices().ForEach(v => this.AddVertex(v.Value));
            graph.GetEdges().ForEach(e => this.AddEdge(e.Vertices[0].Value, e.Vertices[1].Value));
        }
    }
}
