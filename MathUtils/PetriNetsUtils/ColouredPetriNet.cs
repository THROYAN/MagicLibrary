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
    public class ColouredPetriNet
    {
        private Random r;
        public ColouredPetriGraph Graph { get; set; }

        public ColouredPetriNet(ColouredPetriGraph graph)
        {
            this.Graph = graph;
            this.Graph.Colors.RegisterAllFunctions();
            r = new Random();
        }

        public void ResetMarking()
        {
            this.SetMarking(this.StartMarking);
        }

        public uint[] StartMarking { get; private set; }

        public uint[] GetState()
        {
            var places = this.Graph.GetPlaces();

            uint[] state = new uint[places.Count];
            int i = 0;
            places.ForEach(p => state[i++] = (p as MarkedPlace).TokensCount);

            return state;
        }

        public void SaveMarkingAsStartMarking()
        {
            this.StartMarking = this.GetState();
        }

        public void SetMarking(uint[] marking)
        {
            if (marking.Length != this.Graph.PlacesCount)
                throw new Exception("Marking array length must equals a places count of Petri Net");

            var places = this.Graph.GetPlaces();

            int i = 0;
            // If marking[i] == w then set it very big integer
            places.ForEach(p =>
                (p as MarkedPlace).TokensCount =
                        marking[i++] != MarkingTreeNode.w ?
                            marking[i - 1]
                            : UInt32.MaxValue
            );
        }

        /// <summary>
        /// Get array of transitions which are available
        /// </summary>
        /// <returns></returns>
        public ColouredTransition[] GetAvailableTransitions()
        {
            List<ColouredTransition> l = new List<ColouredTransition>();

            foreach (var t in this.Graph.GetTransitions())
            {
                if (t.IsAvailable())
                {
                    l.Add(t);
                }
            }

            return l.ToArray();
        }

        /// <summary>
        /// Function of execute the transition in a special marking
        /// State of the net won't change
        /// </summary>
        /// <param name="marking"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public uint[] GetStateAfterExecute(uint[] marking, ColouredTransition t)
        {
            uint[] newMarking = null;

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

        public void ExecuteTransition(ColouredTransition t)
        {
            t.Execute();
        }

        public void ExecuteRandomTransition()
        {
            ColouredTransition[] ts = GetAvailableTransitions();
            if (ts.Length > 0)
            {
                ExecuteTransition(ts[r.Next(ts.Length)]);
            }
        }
    }
}
