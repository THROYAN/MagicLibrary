using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs.Trees
{
    public class TreeGraph : DirectedGraph, ITree
    {
        public ITreeNode Root { get; protected set; }

        private ITreeNode _addTo { get; set; }

        public TreeGraph(object rootValue = null)
            : base()
        {
            TreeGraph.SetDefaultEventHandlers( this );
            if(rootValue != null)
                this.AddTreeNode( rootValue );
        }

        public static void SetDefaultEventHandlers( TreeGraph tree )
        {
            DirectedGraph.SetDefaultEventHandlers( tree );

            tree.OnAddVertex += new EventHandler<VerticesModifiedEventArgs>(tree.tree_OnAddVertex);
            tree.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(tree.tree_OnAddEdge);
            tree.OnRemoveVertex += new EventHandler<VerticesModifiedEventArgs>(tree.tree_OnRemoveVertex);
            tree.OnRemoveEdge += new EventHandler<EdgesModifiedEventArgs>(tree.tree_OnRemoveEdge);
            tree.OnVertexAdded += new EventHandler<VerticesModifiedEventArgs>(tree.tree_OnVertexAdded);
        }

        void tree_OnVertexAdded(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if (this._addTo == null)
                {
                    this.Root = e.Vertex as ITreeNode;
                }
                else
                {
                    this._addTo.AddChild(e.Vertex as ITreeNode);
                    //Add the edge between new vertex and its parent
                    this.AddArc((this._addTo as TreeGraphNode).Value, e.Vertex.Value);
                }
            }
            this._addTo = this.Root;
        }

        void tree_OnRemoveEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                this.RemoveVertex((e.Edge as Arc).Head.Value);
            }
        }

        void tree_OnRemoveVertex(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                TreeGraphNode tNode = e.Vertex as TreeGraphNode;

                foreach (TreeGraphNode child in tNode.Children)
                {
                    this.RemoveVertex(child.Value);
                }

                if( !tNode.IsRoot() )
                    tNode.Parent.Children.Remove(tNode);
            }
        }

        void tree_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                TreeGraphNode head = (e.Edge as Arc).Head as TreeGraphNode;
                TreeGraphNode tail = (e.Edge as Arc).Tail as TreeGraphNode;

                if (head.IsRoot() || ! (head.Parent.Equals(tail) && tail.IsChild(head)) )
                {
                    e.Status = ModificationStatus.Error;
                }
            }
        }

        void tree_OnAddVertex(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                e.Vertex = new TreeGraphNode(e.Vertex.Graph as TreeGraph, e.Vertex.Value, this._addTo as TreeGraphNode);
            }
        }

        public void AddTreeNode(object nodeValue, object parentNodeValue = null)
        {
            if (parentNodeValue != null)
                this._addTo = this[parentNodeValue] as ITreeNode;
            this.AddVertex(nodeValue);
        }

        public override string ToString()
        {
            return this.Root.ToStringWithChildren();
        }

        public void CalculateAllRelations()
        {
            foreach (TreeGraphNode node in this.GetVertices())
            {
                node.Parent = node.GetParent();
                node.Children = node.GetChildren().ToList();
            }
        }

        public override object Clone()
        {
            TreeGraph t = new TreeGraph();

            this.CopyTo(t);

            return t;
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            TreeGraph.SetDefaultEventHandlers(graph as TreeGraph);
        }

        public virtual void AddTree(ITree tree, object parentNodeValue = null)
        {
            //if (parentNodeValue != null)
            //    this._addTo = this[parentNodeValue] as TreeGraphNode;
            //this._addTo.AddTree(tree);
            this.AddTreeNode((tree.Root as TreeGraphNode).Value, parentNodeValue);

            this.GraphMerge(tree as TreeGraph);
        }

        public override void GraphMerge(IGraph graph)
        {
            graph.GetVertices().ForEach(v => 
                this.AddTreeNode(
                    v.Value,
                    (v as TreeGraphNode).Parent != null ? ((v as TreeGraphNode).Parent as TreeGraphNode).Value : null
                )
            );
        }

        public virtual ITreeNode[] Path(ITreeNode node)
        {
            List<ITreeNode> path = new List<ITreeNode>();
            ITreeNode currentNode = node.Parent;
            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            return path.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator() as IEnumerator;
        }

        public TreeEnum GetEnumerator()
        {
            return new TreeEnum(this);
        }
    }

    public class TreeEnum : IEnumerator
    {
        public TraversalMethods TraversalMethod { get; set; }

        private int position = -1;

        private List<ITreeNode> passed;

        private TreeGraph treeGraph;

        public TreeEnum(TreeGraph treeGraph)
        {
            this.treeGraph = treeGraph;
            passed = new List<ITreeNode>();
            this.TraversalMethod = TraversalMethods.PreOrderWalk;

            this.Current = this.treeGraph.Root;
            passed.Add(this.Current);
        }
        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public ITreeNode Current { get; set; }

        public bool MoveNext()
        {
            switch (this.TraversalMethod)
            {
                case TraversalMethods.PreOrderWalk:
                    this.Current = FindNext(this.Current);

                    if (this.Current == null)
                    {
                        return false;
                    }
                    else
                    {
                        this.passed.Add(this.Current);
                        return true;
                    }
                default:
                    this.position++;
                    if (this.position < this.treeGraph.Order)
                    {
                        this.Current = this.treeGraph.GetVertices()[this.position] as ITreeNode;
                        return true;
                    }
                    return false;
            }
        }

        public ITreeNode FindNext(ITreeNode node)
        {
            if (node == null)
                return null;
            var temp = node.Children.Find(child => !this.passed.Contains(child));

            if (temp != null)
            {
                return temp;
            }
            else
            {
                return this.FindNext(node.Parent);
            }
        }

        public void Reset()
        {
            position = -1;
        }
    }

    public enum TraversalMethods { PreOrderWalk, PostOrderWalk, WalkByTheWidth, ByIndexes }
}
