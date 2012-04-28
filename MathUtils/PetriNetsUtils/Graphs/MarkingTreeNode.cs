using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.Graphs.Trees;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    public class MarkingTreeNode : TreeGraphNode
    {
        public uint[] Marking
        {
            get;
            set;
        }

        public static uint w = UInt32.MaxValue;

        public MarkingNodeStatus Status { get; set; }

        public MarkingTreeNode(TreeGraph tree, string markingName, MarkingTreeNode parent, uint[] marking)
            : base(tree, markingName, parent)
        {
            this.Marking = marking;
            this.Status = MarkingNodeStatus.Boundary;
        }

        public string MarkingToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            for (int i = 0; i < this.Marking.Length; i++)
            {
                string marking = this.Marking[i] == MarkingTreeNode.w ? "w" : this.Marking[i].ToString();
                sb.Append(i < this.Marking.Length - 1 ? marking + "," : marking);
            }
            sb.Append("]");

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendFormat("[{1}]{0}[",
                                this.Value.ToString(),
                                this.Parent == null ?
                                    "start" :
                                    ((this.Tree as MarkingTree)[
                                        (this.Parent as MarkingTreeNode).Value,
                                        this.Value
                                    ] as NamedArc).Name
                            );

            for (int i = 0; i < this.Marking.Length; i++)
            {
                string marking = this.Marking[i] == w ? "w" : this.Marking[i].ToString();
                sb.Append(i < this.Marking.Length - 1 ? marking + "," : marking);
            }
            sb.AppendFormat("]_{0}",this.Status.ToString());
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Compares node marking and this marking
        /// </summary>
        /// <param name="node"></param>
        /// <returns>-1 - if this is not bigger as node, 0 - if this marking is equals as node marking and 1 if this marking is bigger</returns>
        public int Compare(MarkingTreeNode node)
        {
            if (this.Marking.Length != node.Marking.Length)
                throw new Exception("Ivalid parameter");
            bool f = false;
            for (int i = 0; i < node.Marking.Length; i++)
            {
                // 0 < 1 or 1 < w
                if ((node.Marking[i] > this.Marking[i] || node.Marking[i] == MarkingTreeNode.w) && this.Marking[i] != MarkingTreeNode.w)
                    return -1;
                // 1 > 0 or w > 1
                // w == w
                if (node.Marking[i] < this.Marking[i] || (this.Marking[i] == MarkingTreeNode.w && node.Marking[i] != MarkingTreeNode.w))
                    f = true;
            }
            return f ? 1 : 0;
        }

        public void RecalculateMarking()
        {
            uint[] newM = new uint[this.Marking.Length];
            uint[] oldMarking = (this.Parent as MarkingTreeNode).Marking;

            (this.Parent as MarkingTreeNode).Status = MarkingNodeStatus.Inside;

            for (int i = 0; i < newM.Length; i++)
            {
                // а) если М(х)i=w, то М(z)i=w.
                if (oldMarking[i] == MarkingTreeNode.w)
                {
                    newM[i] = MarkingTreeNode.w;
                }
                else
                {
                    // б) если на пути от корневой вершины к х существует вершина у с
                    // М(у)<б(M(x), tj) и М(у)i<б(M(x), tj)i, то М(z)i=w, б - функция следующего состояния (срабатывания перехода).
                    // newState - это и есть результат б(M(x), tj), M(x) - newState.Parent.Marking

                    bool f = false;

                    foreach (MarkingTreeNode node in this.GetPathToMe())
                    {
                        if (this.Compare(node) == 1 && node.Marking[i] < this.Marking[i])
                        {
                            newM[i] = MarkingTreeNode.w;
                            f = true;
                            break;
                        }
                    }

                    // в) в противном случае М(z)i=б(M(x), tj)i,
                    if (!f)
                    {
                        newM[i] = this.Marking[i];
                    }
                }
            }

            this.Marking = newM;
        }

        public void Process(PetriNet petriNet)
        {
            if (this.Status != MarkingNodeStatus.Boundary)
                return;

            // Marking is dublicate
            if (this.Graph.GetVertex(v =>
                    this.Compare(v as MarkingTreeNode) == 0 &&
                    this.Value != v.Value &&
                    (v as MarkingTreeNode).Status != MarkingNodeStatus.Boundary) != null
            )
            {
                this.Status = MarkingNodeStatus.Dublicate;
                return;
            }

            // Marking is terminal
            Transition[] availableTransitions = petriNet.GetAvailableTransitions(this.Marking);
            if (availableTransitions.Length == 0)
            {
                this.Status = MarkingNodeStatus.Terminal;
                return;
            }

            foreach (Transition transition in availableTransitions)
            {
                (this.Tree as MarkingTree).AddMarking(petriNet.GetStateAfterExecute(this.Marking, transition), this.Value.ToString(), transition.Value.ToString());
            }
        }

        public bool HasW()
        {
            return this.Marking.ToList().Any(i => i == MarkingTreeNode.w);
        }

        public int GetMarksSum()
        {
            if(this.HasW())
                return -1;
            return this.Marking.Sum(e => (int)e);
        }

        public int GetMarksMaxValue()
        {
            if (this.HasW())
                return -1;
            return (int)this.Marking.Max();
        }

         
        /// <summary>
        /// Проверка на покрываемость какой-то вершины в дереве данной вершиной.
        /// Возвращает null, если эта вершина не покрывает никакой из достижимых.
        /// </summary>
        /// <returns></returns>
        public MarkingTreeNode Covers()
        {
            return this.Covers(this.Tree.Root as MarkingTreeNode);
        }
        private MarkingTreeNode Covers(MarkingTreeNode node)
        {
            if (this.Compare(node) == 1)
                return node;
            MarkingTreeNode res = null;
            node.Children.ForEach(delegate(ITreeNode child)
            {
                res = this.Covers(child as MarkingTreeNode);
                if (res != null)
                    return;
            });
            return res;
        }


    }

    public enum MarkingNodeStatus { Boundary, Terminal, Dublicate, Inside }
}
