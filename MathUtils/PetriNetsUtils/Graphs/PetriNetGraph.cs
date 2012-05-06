using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class PetriNetGraph : BiGraph
    {
        private int pCounter { get; set; }
        private int tCounter { get; set; }

        public int PlacesCount { get { return GetPlaces().Count; } }
        public int TransitionsCount { get { return GetTransitions().Count; } }
        
        public Place GetPlace(string name)
        {
            return GetVertexFromFirstPart(name) as Place;
        }
        public ColouredTransition GetTransition(string name)
        {
            return GetVertexFromSecondPart(name) as ColouredTransition;
        }

        public List<Place> GetPlaces()
        {
            return GetVertices().OfType<Place>().ToList();
        }
        public List<ColouredTransition> GetTransitions()
        {
            List<ColouredTransition> l = new List<ColouredTransition>();
            this.GetVertices(v => v is ColouredTransition).ForEach(v => l.Add(v as ColouredTransition));
            return l.ToList();
        }

        public PetriNetGraph() : base()
        {
            PetriNetGraph.SetDefaultEventHandlers(this);
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new Transition(this, vertexValue.ToString()) as IVertex
                        : new Place(this, vertexValue.ToString()) as IVertex;
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
