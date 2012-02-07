using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class PetriNetGraph : WeightedBiGraph
    {
        private int pCounter { get; set; }
        private int tCounter { get; set; }

        public int PlacesCount { get { return GetPlaces().Count; } }
        public int TransitionsCount { get { return GetTransitions().Count; } }
        
        public Place GetPlace(string name)
        {
            return GetVertexFromFirstPart(name) as Place;
        }
        public Transition GetTransition(string name)
        {
            return GetVertexFromSecondPart(name) as Transition;
        }

        public List<Place> GetPlaces()
        {
            return GetVertices().OfType<Place>().ToList();
        }
        public List<Transition> GetTransitions()
        {
            return GetVertices().OfType<Transition>().ToList();
        }

        public PetriNetGraph() : base()
        {
            PetriNetGraph.SetDefaultEventHandlers(this);
        }

        public static void SetDefaultEventHandlers(PetriNetGraph graph)
        {
            WeightedBiGraph.SetDefaultEventHandlers(graph);

            graph.OnAddVertex += new EventHandler<VerticesModifiedEventArgs>(graph.PetriNetGraph_OnAddVertex);
        }

        void PetriNetGraph_OnAddVertex(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                if (addToSecond)
                {
                    e.Vertex = new Transition(sender as IGraph, e.VertexValue as string);
                }
                else
                {
                    e.Vertex = new Place(sender as IGraph, e.VertexValue as string);
                }
            }
        }

        public void AddPlace(string name = null)
        {
            if (name == null)
                name = "P" + pCounter++;
            AddVertexToFirstPart(name);
        }

        public void AddTransition(string name = null)
        {
            if (name == null)
                name = "T" + tCounter++;
            AddVertexToSecondPart(name);
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            var pGraph = graph as PetriNetGraph;
            pGraph.pCounter = this.pCounter;
            pGraph.tCounter = this.tCounter;

            PetriNetGraph.SetDefaultEventHandlers(graph as PetriNetGraph);
        }

        public override object Clone()
        {
            PetriNetGraph pGraph = new PetriNetGraph();

            this.CopyTo(pGraph);

            return pGraph;
        }
    }
}
