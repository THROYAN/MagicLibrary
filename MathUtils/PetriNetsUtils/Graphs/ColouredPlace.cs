﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    public class ColouredPlace : Place
    {
        private ColorSet colorSet { get { return this.collection[this.ColorSetName]; } }

        public string ColorSetName { get; set; }

        public List<Token> InitTokens { get; private set; }
        public List<Token> Tokens { get; set; }
        public string InitFunction { get; set; }

        private ColorSetCollection collection { get; set; }

        public ColouredPlace(PetriNetGraph graph, string name, ColorSet color)
            : this(graph, name, color.Name) { }

        public ColouredPlace(PetriNetGraph graph, string name, string colorName = "")
            : base(graph, name)
        {
            this.ColorSetName = colorName;
            this.collection = (this.Graph as ColouredPetriGraph).Colors;
            this.InitTokens = new List<Token>();
            this.Tokens = new List<Token>();
        }

        public bool IsLegalColor()
        {
            return this.collection.Contains(this.ColorSetName);
        }

        public bool IsLegalInitTokens()
        {
            return this.InitTokens.TrueForAll(t => this.colorSet.IsLegal(t.Function));
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
            this.Tokens = new List<Token>();

            
        }
    }
}
