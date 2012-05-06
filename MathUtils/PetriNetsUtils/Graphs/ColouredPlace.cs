using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class ColouredPlace : Place
    {
        public ColorSet ColorSet { get { return this.collection[this.ColorSetName]; } }

        public string ColorSetName { get; set; }

        public List<Token> InitTokens { get; private set; }
        public MultiSet<Function> Tokens { get; set; }
        private string _initFunction;
        public string InitFunction { get { return this._initFunction; } set { this._initFunction = value; this.ResetMarking(); } }

        private ColorSetCollection collection { get; set; }

        public ColouredPlace(PetriNetGraph graph, string name, ColorSet color)
            : this(graph, name, color.Name) { }

        public ColouredPlace(PetriNetGraph graph, string name, string colorName = "")
            : base(graph, name)
        {
            this.ColorSetName = colorName;
            this.collection = (this.Graph as ColouredPetriGraph).Colors;
            this.InitTokens = new List<Token>();
            this.Tokens = new MultiSet<Function>();
        }

        public bool IsLegalColor()
        {
            return this.collection.ContainsColorSet(this.ColorSetName);
        }

        public bool IsLegalInitTokens()
        {
            return this.InitTokens.TrueForAll(t => this.ColorSet.IsLegal(t.Function));
        }

        public override void CopyTo(MathUtils.Graphs.IVertex vertex)
        {
            base.CopyTo(vertex);

            var place = vertex as ColouredPlace;
            place.ColorSetName = this.ColorSetName;
            place.InitFunction = this.InitFunction;
            place.collection = this.collection;
        }

        public override object Clone()
        {
            ColouredPlace place = new ColouredPlace(this.Graph as ColouredPetriGraph, this.Name);

            this.CopyTo(place);
            
            return place;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetMarking()
        {
            this.Tokens = new MultiSet<Function>();
            try
            {
                var f = new Function();
                f.ParseFromString(this.InitFunction);
                if (f.Variables.Length == 0)
                {
                    this.Tokens = new MultiSet<Function>(f);
                }
            }
            catch
            {
            }
        }
    }
}
