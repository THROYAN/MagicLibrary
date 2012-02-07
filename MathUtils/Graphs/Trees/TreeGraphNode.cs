using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs.Trees
{
    public class TreeGraphNode : Vertex, ITreeNode
    {
        public ITree Tree { get { return this.Graph as TreeGraph; } }

        public ITreeNode Parent
        {
            //get
            //{
            //    for (int i = 0; i < Graph.Order; i++)
            //    {
            //        if (Graph[Graph.VerticesValues[i], this] != null)
            //            return Graph.GetVertices()[i] as TreeGraphNode;
            //    }
            //    return null;
            //}
            //set
            //{
            //    value.AddChild(this);
            //}
            get;
            set;
        }

        public List<ITreeNode> Children
        {
            //get
            //{
            //    List<TreeGraphNode> children = new List<TreeGraphNode>();
            //    for (int i = 0; i < Graph.Order; i++)
            //    {
            //        if (Graph[this, Graph.VerticesValues[i]] != null)
            //            children.Add(Graph.GetVertices()[i] as TreeGraphNode);
            //    }
            //    return children.ToArray();
            //}
            get;
            set;
        }

        public TreeGraphNode(TreeGraph tree, object value, TreeGraphNode parent)
            : base(tree, value)
        {
            this.Parent = parent;
            this.Children = new List<ITreeNode>();
            //parent.AddChild( this );
        }

        public ITreeNode GetParent()
        {
            var vertices = Graph.GetVertices();
            for (int i = 0; i < Graph.Order; i++)
            {
                if ((this.Tree as TreeGraph)[vertices[i].Value, this.Value] != null)
                    return vertices[i] as ITreeNode;
            }
            return null;
        }

        /// <summary>
        /// Return children of the node by the edges
        /// </summary>
        /// <returns></returns>
        public ITreeNode[] GetChildren()
        {
            var vertices = (this.Tree as TreeGraph).GetVertices();
            List<ITreeNode> children = new List<ITreeNode>();

            for (int i = 0; i < this.Graph.Order; i++)
            {
                if ((this.Tree as TreeGraph)[this.Value, vertices[i].Value] != null)
                    children.Add(vertices[i] as TreeGraphNode);
            }

            return children.ToArray();
        }

        /// <summary>
        /// Add child and set parent of node
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(ITreeNode node)
        {
            if(node.Equals(this) || this.IsChild(node))
                return;
            node.Parent = this;
            this.Children.Add(node);
            //this.tree.AddArc(this.Value, node.Value);
        }

        /// <summary>
        /// Return true if node is a child of this vertex
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsChild(ITreeNode node)
        {
            return this.Children.Contains(node);
            //return this.Graph[this.Value, node.Value] != null;
        }

        public override void CopyTo(IVertex vertex)
        {
            base.CopyTo(vertex);

            TreeGraphNode tNode = vertex as TreeGraphNode;
            tNode.Parent = this.Parent;
            tNode.Children = this.Children.ToList();
        }

        public override object Clone()
        {
            TreeGraphNode tNode = new TreeGraphNode( this.Tree as TreeGraph, this.Value, this.Parent as TreeGraphNode );
            
            base.CopyTo( tNode );

            return tNode;
        }

        /// <summary>
        /// If the node is the root of tree return true.
        /// </summary>
        /// <returns></returns>
        public bool IsRoot()
        {
            return this.Parent == null;
        }

        public bool IsLeaf()
        {
            return this.Children.Count == 0;
        }

        public int Level
        {
            get
            {
                if (this.IsRoot())
                    return 0;
                return this.Parent.Level + 1;
            }
        }

        public new int Degree
        {
            get
            {
                return this.Children.Count;
            }
        }

        public virtual string ToStringWithChildren()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Level; i++)
            {
                sb.Append("\t");
            }
            sb.AppendLine(this.ToString());
            //foreach (TreeGraphNode child in this.Children)
            //{
            //    sb.Append(child.ToStringWithChildren());
            //}
            this.Children.ForEach(child => sb.Append(child.ToStringWithChildren()));
            if (this.IsLeaf())
                sb.AppendLine();

            return sb.ToString();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public void AddTree(ITree tree)
        {
            this.AddChild(tree.Root);
        }

        public ITreeNode[] GetPathToMe()
        {
            return (this.Tree as TreeGraph).Path(this);
        }

        public int GetLeavesCount()
        {
            int count = 0;

            this.Children.ForEach(child => count += child.IsLeaf() ? 1 : (child as TreeGraphNode).GetLeavesCount());

            return count;
        }
    }
}
