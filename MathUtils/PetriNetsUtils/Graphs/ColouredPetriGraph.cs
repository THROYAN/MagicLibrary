using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class ColouredPetriGraph : PetriNetGraph
    {
        private string _currentArcFunc { get; set; }
        private string _currentColorName { get; set; }
        private string _currentInitFunc { get; set; }

        public ColorSetCollection Colors { get; set; }

        public ColouredPetriGraph(ColorSetCollection colors = null)
            : base()
        {
            if (colors == null)
            {
                this.Colors = new ColorSetCollection();
            }
            else
            {
                this.Colors = colors;
            }

            this._currentArcFunc = "";
            this._currentColorName = "";
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return this.addToSecond
                        ? new ColouredTransition(this, vertexValue.ToString()) as IVertex
                        : new ColouredPlace(this, vertexValue.ToString(), this._currentColorName) { InitFunction=this._currentInitFunc } as IVertex;
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new ColouredArc(this, u.ToString(), v.ToString(), this._currentArcFunc);
        }

        public void AddArc(object u, object v, string arcFunc)
        {
            this._currentArcFunc = arcFunc;
            this.AddArc(u, v);
            this._currentArcFunc = "";
        }

        public void AddPlace(string name, string color, string initFunc = "")
        {
            this._currentInitFunc = initFunc;
            this._currentColorName = color;
            this.AddPlace(name);
            this._currentInitFunc = "";
            this._currentColorName = "";
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            var pGraph = graph as ColouredPetriGraph;
            pGraph.Colors = this.Colors;
        }

        public override object Clone()
        {
            ColouredPetriGraph g = new ColouredPetriGraph();

            this.CopyTo(g);

            return g;
        }

        /// <summary>
        /// Сбрасывает маркировка на начальную.
        /// У каждой позиции - это функция инициализации.
        /// </summary>
        public void ResetMarking()
        {
            foreach (ColouredPlace place in this.GetVertices())
            {
                place.ResetMarking();
            }
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
                        var a = this[VerticesValues[i], VerticesValues[j]] as FunctionArc;
                        matrix[i, j] = a == null ?
                                            new FunctionMatrixElement(new Function()) :
                                            new FunctionMatrixElement(a.ParsedFunction == null ? new Function("ErrorFunc") : a.ParsedFunction);
                    }
                }
                return matrix;
            }
        }
    }
}
