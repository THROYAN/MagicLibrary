using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.Graphs.Trees;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    public class MarkingTree : TreeGraph
    {
        private uint[] _currentMarking;
        private string _currentTransitionName;
        private int _count;

        public MarkingTree(uint[] startMarking, string rootName = null)
            : base()
        {
            this._count = 0;
            this._currentTransitionName = "";

            MarkingTree.SetDefaultEventHandlers(this);

            this.AddMarking(startMarking, null, rootName);
        }

        public static void SetDefaultEventHandlers(MarkingTree tree)
        {
            TreeGraph.SetDefaultEventHandlers(tree);

            tree.OnVertexAdded += new EventHandler<VerticesModifiedEventArgs>(tree.tree_OnVertexAdded);
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new NamedArc(this, u, v, this._currentTransitionName);
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return new MarkingTreeNode(this, vertexValue.ToString(), this._addTo as MarkingTreeNode, this._currentMarking);
        }

        void tree_OnVertexAdded(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if(!(e.Vertex as MarkingTreeNode).IsRoot())
                    (e.Vertex as MarkingTreeNode).RecalculateMarking();
            }
        }

        public void AddMarking(uint[] marking, string parentMarkingName, string transitionName = "", string nodeName = null)
        {
            if (nodeName == null)
                nodeName = String.Format("S{0}", ++this._count);
            this._currentMarking = marking;
            this._currentTransitionName = transitionName;

            this.AddTreeNode(nodeName, parentMarkingName);

            this._currentMarking = new uint[] { 0 };

            //return this[nodeName] as MarkingTreeNode;
        }

        public override void AddTree(ITree tree, object parentNodeValue = null)
        {
            this.AddMarking((tree.Root as MarkingTreeNode).Marking, parentNodeValue != null ? parentNodeValue.ToString() : null, (tree.Root as TreeGraphNode).Value.ToString());

            this.GraphMerge(tree as TreeGraph);
        }

        public override void GraphMerge(IGraph graph)
        {
            graph.GetVertices().ForEach(v =>
                this.AddMarking(
                    (v as MarkingTreeNode).Marking,
                    (v as TreeGraphNode).Parent != null ? ((v as TreeGraphNode).Parent as TreeGraphNode).Value.ToString() : null,
                    v.Value.ToString()
                )
            );
        }

        public MarkingTreeNode[] GetBoundaries()
        {
            return this.GetVertices(v => (v as MarkingTreeNode).Status == MarkingNodeStatus.Boundary).OfType<MarkingTreeNode>().ToArray();
        }

        /// <summary>
        /// Обрабатывает все граничные вершины дерева
        /// </summary>
        /// <param name="petriNet">Сеть, по которой строится дерево</param>
        public void ProcessBoundaries(PetriNet petriNet)
        {
            MarkingTreeNode[] boundaries = this.GetBoundaries();

            if (boundaries.Length == 0)
                return;

            foreach (MarkingTreeNode node in boundaries)
            {
                node.Process(petriNet);
            }
            this.ProcessBoundaries(petriNet);
        }

        /// <summary>
        /// Решение задачи ограниченности сети. Если ни в одной его вершине нет символа w, то сеть ограничена.
        /// </summary>
        /// <returns></returns>
        public bool IsLimited()
        {
            return !this.HasW(this.Root as MarkingTreeNode);
        }

        public int GetLimit()
        {
            if (!this.IsLimited())
                return -1;
            uint max = 0;
            for (int i = 0; i < (this.Root as MarkingTreeNode).Marking.Length; i++)
            {
                uint l = this.GetPlaceLimit(i);
                if (max < l)
                    max = l;
            }
            return (int)max;
        }

        public uint GetPlaceLimit(int index)
        {
            if (index >= (this.Root as MarkingTreeNode).Marking.Length)
                throw new IndexOutOfRangeException();
            return this.GetPlaceLimit(index, this.Root as MarkingTreeNode, (this.Root as MarkingTreeNode).Marking[index]);
        }
        private uint GetPlaceLimit(int index, MarkingTreeNode node, uint max)
        {
            uint _max;
            if (node.Marking[index] > max)
                _max = node.Marking[index];
            else
                _max = max;

            if (node.Children.Count == 0)
                return _max;

            uint maxChilds = node.Children.Max(child => this.GetPlaceLimit(index, child as MarkingTreeNode, _max));
            return Math.Max(_max, maxChilds);
        }

        /// <summary>
        /// Проверка на наличие сивола w в маркировке и всех её детях.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasW(MarkingTreeNode node)
        {
            if (node.HasW())
                return true;
            return node.Children.Any(child => this.HasW(child as MarkingTreeNode));
        }

        /// <summary>
        /// Если количество фишек, перемещающихся по сети не изменяется, то сеть называется сохраняющей.
        /// </summary>
        /// <returns></returns>
        public bool IsPreserving()
        {
            return EqualsMarks(this.Root as MarkingTreeNode, (this.Root as MarkingTreeNode).GetMarksSum());
        }

        /// <summary>
        /// Если сеть сохраняющая и 1-ограничена, она безопасна.
        /// </summary>
        /// <returns></returns>
        public bool IsSafe()
        {
            return this.IsPreserving() && this.GetLimit() == 1;
        }

        private bool EqualsMarks(MarkingTreeNode node, int sum)
        {
            if (node.HasW() || node.GetMarksSum() != sum)
                return false;
            return node.Children.All(child => this.EqualsMarks(child as MarkingTreeNode, sum));
        }

        public bool IsNotActive()
        {
            foreach (MarkingTreeNode m in this)
            {
                if (m.Status == MarkingNodeStatus.Terminal)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка на достижимости маркировки. Если маркировка достижима, то возвращается 1, если нет, то -1.
        /// В случае, если достижимость не возможно определить с помощью дерева достижимости возвращается 0.
        /// </summary>
        /// <param name="marking"></param>
        /// <returns></returns>
        public int IsAchievable(uint[] marking)
        {
            bool f = false;
            var m = new MarkingTreeNode(null, null, null, marking);
            foreach (MarkingTreeNode node in this)
            {
                if(node.Compare(m) == 0)
                {
                    return 1;
                }
                if (node.Compare(m) == 1)
                {
                    f = true;
                }
            }
            return f ? 0 : -1;
        }

        public string GetAnalizeReport()
        {
            StringBuilder sb = new StringBuilder();
            int limit = this.GetLimit();
            if (limit != -1)
            {
                sb.AppendFormat("Сеть {0}-ограничена;", limit);
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("Сеть неограничена;");
            }
            if (this.IsPreserving())
            {
                sb.Append("Сеть сохраняющая");
                if (this.IsSafe())
                {
                    sb.AppendLine(" и безопасная");
                }
                else
                {
                    sb.AppendLine(", но не безопасная");
                }
            }
            else
            {
                sb.AppendLine("Сеть несохраняющая;");
            }
            if (this.IsNotActive())
            {
                sb.AppendLine("Сеть неактивная;");
            }
            else
            {
                sb.AppendLine("Об активности сети по дереву достижимости судить ничего сказать нельзя;");
            }
            return sb.ToString();
        }
    }
}
