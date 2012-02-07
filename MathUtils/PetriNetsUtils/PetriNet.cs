using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.PetriNetsUtils.Graphs;
using MagicLibrary.MathUtils.Graphs.Trees;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    [Serializable]
    public class PetriNet
    {
        private Random r;
        public MarkedPetriGraph Graph { get; set; }

        public PetriNet(MarkedPetriGraph graph)
        {
            this.Graph = graph;
            r = new Random();
        }

        public void ResetMarking()
        {
            this.SetMarking(this.StartMarking);
        }

        public int[] StartMarking { get; private set; }

        public int[] GetState()
        {
            var places = this.Graph.GetPlaces();

            int[] state = new int[places.Count];
            int i = 0;
            places.ForEach(p => state[i++] = (p as MarkedPlace).TokenCount);

            return state;
        }

        public void SaveMarkingAsStartMarking()
        {
            this.StartMarking = this.GetState();
        }

        public void SetMarking(int[] marking)
        {
            if (marking.Length != this.Graph.PlacesCount)
                throw new Exception("Marking array length must equals a places count of Petri Net");

            var places = this.Graph.GetPlaces();

            int i = 0;
            // If marking[i] == w then set it very big integer
            places.ForEach(p =>
                (p as MarkedPlace).TokenCount = 
                    marking[i++] != MarkingTreeNode.w ? marking[i - 1] : Int32.MaxValue
            );
        }

        public MarkingTree GetReachabilityTree()
        {
            MarkingTree tree = new MarkingTree(this.GetState());
            // Set the root as a boundary node
            (tree.Root as MarkingTreeNode).Status = MarkingNodeStatus.Boundary;

            tree.ProcessBoundaries(this);

            return tree;
        }

        //public MarkingTree GetReachabilityTree(int deep = 3, MarkingTreeNode currentNode = null)
        //{
        //    //this.ResetMarking();
        //    MarkingTree tree;
        //    if (currentNode == null)
        //    {
        //        tree = new MarkingTree(this.GetState());
        //        currentNode = tree.Root as MarkingTreeNode;
        //    }
        //    else
        //    {
        //        tree = currentNode.Tree as MarkingTree;
        //    }

        //    // Marking is dublicate
        //    if (tree.GetVertex(v => 
        //            currentNode.Compare(v as MarkingTreeNode) == 0 &&
        //            currentNode.Value != v.Value &&
        //            (v as MarkingTreeNode).Status != MarkingNodeStatus.Boundary) != null
        //    )
        //    {
        //        currentNode.Status = MarkingNodeStatus.Dublicate;
        //    }

        //    // Marking is terminal
        //    if (this.GetAvailableTransitions().Length == 0)
        //    {
        //        currentNode.Status = MarkingNodeStatus.Terminal;
        //    }

        //    if (currentNode.Status == MarkingNodeStatus.Boundary)
        //    {
        //        foreach (Transition transition in this.GetAvailableTransitions())
        //        {
        //            this.ExecuteTransition(transition);

        //            tree.AddMarking(this.GetState(), currentNode.Value.ToString());

        //            //t.AddTree(this.GetReachabilityTree(deep, currentNode.Level + 1), currentNode.Value);
        //            //t.AddMarking(this.GetState(), parent.Value.ToString());//, transition.Value.ToString());

        //            this.SetMarking(currentNode.Marking);
        //        }

        //        foreach (MarkingTreeNode node in currentNode.Children)
        //        {
        //            this.GetReachabilityTree(3, node);
        //        }
        //    }

        //    return tree;
        //}

        //public MarkedPlace[] GetEnters(Transition t)
        //{
        //    List<MarkedPlace> l = new List<MarkedPlace>();
        //    graph.GetEdges(e => (e as Arc).Head.Value.Equals(t.Value)).ForEach(edge => l.Add((edge as Arc).Tail as MarkedPlace));
        //    return l.ToArray();
        //}
        //public MarkedPlace[] GetExits(Transition t)
        //{
        //    List<MarkedPlace> l = new List<MarkedPlace>();
        //    graph.GetEdges(e => (e as Arc).Tail.Value.Equals(t.Value)).ForEach(edge => l.Add((edge as Arc).Head as MarkedPlace));
        //    return l.ToArray();
        //}
        //public bool IsAvailableTransition(Transition t)
        //{
        //    bool f = true;// GetEnters(t).Length > 0;
        //    GetEnters(t).ToList().ForEach(delegate(MarkedPlace p)
        //    {
        //        if (p.TokenCount < (graph[p.Value, t.Value] as WeightedArc).Weight)
        //            f = false;
        //    });
        //    return f;
        //}

        /// <summary>
        /// Get array of transitions which are available at this marking
        /// If marking is null it use current net state
        /// </summary>
        /// <param name="marking">Marking in which we want to know available transitions (marking of the net will not change)</param>
        /// <returns></returns>
        public Transition[] GetAvailableTransitions(int[] marking = null)
        {
            if (marking != null)
            {
                this.SaveMarkingAsStartMarking();
                this.SetMarking(marking);
            }
            List<Transition> l = new List<Transition>();
            Graph.GetVertices().ForEach(delegate(IVertex v)
            {
                if (v is Transition)
                {
                    if ((v as Transition).IsAvailable())
                    {
                        l.Add(v as Transition);
                    }
                }
            });

            if (marking != null)
            {
                this.ResetMarking();
            }

            return l.ToArray();
        }

        /// <summary>
        /// Function of execute transition in special marking
        /// State of the net won't change
        /// </summary>
        /// <param name="marking"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public int[] GetStateAfterExecute(int[] marking, Transition t)
        {
            int[] newMarking = null;

            this.SaveMarkingAsStartMarking();
            this.SetMarking(marking);

            if (t.IsAvailable())
            {
                // execute transition
                t.Execute();
                // save marking
                newMarking = this.GetState();
            }
            // reset marking to start marking
            this.ResetMarking();

            // if t is unavailable return null
            return newMarking;
        }

        public void ExecuteTransition(Transition t)
        {
            t.Execute();
        }

        public void ExecuteRandomTransition()
        {
            Transition[] ts = GetAvailableTransitions();
            if (ts.Length > 0)
            {
                ExecuteTransition(ts[r.Next(ts.Length)]);
            }
        }
    }
}
