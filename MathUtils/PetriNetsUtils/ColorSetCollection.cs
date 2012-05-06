using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MagicLibrary.Exceptions;
using MagicLibrary.MathUtils.MathFunctions;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    [Serializable]
    public class ColorSetCollection : IEnumerable<ColorSet>
    {
        private List<ColorSet> colorSets { get; set; }
        public List<ColorVariable> ColorVariables { get; set; }
        public List<string> FunctionsDescription { get; set; }

        public ColorSetCollection()
        {
            this.colorSets = new List<ColorSet>();
            this.ColorVariables = new List<ColorVariable>();
            this.FunctionsDescription = new List<string>();
        }

        public ColorSetCollection(string colorsDescription)
            : this()
        {
            this.LoadColorsFromString(colorsDescription);
        }

        /// <summary>
        /// Создаёт цвета используя строковое представление
        /// </summary>
        /// <param name="colorsDescription"></param>
        public void LoadColorsFromString(string colorsDescription)
        {
            this.colorSets = new List<ColorSet>();
            //var cs = colorsDescription.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cs = Regex.Split(colorsDescription, "(\n|\t|\r)");
            this.LoadColorsFromString(cs);
        }

        /// <summary>
        /// Создаёт набор цветов используя массив строковых представлений цветов
        /// </summary>
        /// <param name="colorsDescription"></param>
        public void LoadColorsFromString(string[] colorSets)
        {
            this.colorSets = new List<ColorSet>();
            //var cs = colorsDescription.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var c in colorSets)
            {
                try
                {
                    if (!Regex.IsMatch(c, "^\\s*$"))
                        this.AddColorSet(new ColorSet(c, this));
                }
                catch (InvalidColorSetAttributesException e)
                {
                    throw e;
                }
            }
        }

        public void AddColorSet(ColorSet cs)
        {
            if (!this.colorSets.Contains(cs) && !this.ContainsColorSet(cs.Name))
            {
                this.colorSets.Add(cs);
                switch (cs.Type)
                {
                    case ColorSetType.Enum:
                        //foreach (var item in cs.ParsedAttributes)
                        //{
                        //    this.AddVariable(item.Value, cs.Name);
                        //}

                        break;
                    case ColorSetType.Bool:
                        this.AddVariable(cs.ParsedAttributes[ColorSet.TrueStringAttribute], cs.Name);
                        this.AddVariable(cs.ParsedAttributes[ColorSet.FalseStringAttribute], cs.Name);
                        this.AddVariable(ColorSet.TrueDefault, cs.Name);
                        this.AddVariable(ColorSet.FalseDefault, cs.Name);
                        break;
                    case ColorSetType.Unit:
                        this.AddVariable(cs.ParsedAttributes[ColorSet.NameAttribute], cs.Name);
                        break;
                    case ColorSetType.Index:
                        this.AddVariable(cs.Name, cs.Name);
                        break;
                }
            }
        }

        public bool ContainsColorSet(ColorSet cs)
        {
            return this.colorSets.Contains(cs);
        }

        public bool ContainsColorSet(string name)
        {
            return this.colorSets.Exists(c => c.Name.Equals(name));
        }

        public ColorSet this[string name]
        {
            get
            {
                return this.GetColorSet(name);
            }
        }

        public ColorSet GetColorSet(string name)
        {
            return this.colorSets.Find(c => c.Name.Equals(name));
        }

        public IEnumerator<ColorSet> GetEnumerator()
        {
            return this.colorSets.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return this.ToString("[", ", ", "]");
        }

        public string ToString(string leftString, string elementsSeparator, string rightString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var color in this.colorSets)
            {
                sb.AppendFormat("{0}{1}", color.ToString(), elementsSeparator);
            }

            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - elementsSeparator.Length, elementsSeparator.Length);
            }

            return String.Format("{1}{0}{2}", sb.ToString(), leftString, rightString);
        }

        public bool HasVariable(string name)
        {
            return this.ColorVariables.Exists(cv => cv.Name.Equals(name));
        }

        public bool HasVariable(string name, ColorSet colorSet)
        {
            return this.ColorVariables.Exists(cv => cv.Name.Equals(name) && cv.ColorSet == colorSet);
        }

        public void AddVariable(string name, string colorName)
        {
            if (!this.HasVariable(name) && !String.IsNullOrEmpty(name) && !name.Equals(ColorSet.EmptyAttribute))
            {
                this.ColorVariables.Add(new ColorVariable(name, this[colorName]));
            }
        }

        public ColorVariable GetVariable(string name)
        {
            return this.ColorVariables.Find(cv => cv.Name.Equals(name));
        }

        public void ClearColors()
        {
            this.colorSets.Clear();
            this.ClearVariables();
        }

        public void ClearVariables()
        {
            this.ColorVariables.Clear();
        }

        public void ClearFunctions()
        {
            this.FunctionsDescription.Clear();
            Function.ResetMathFunctions();
        }

        public void AddFunction(string function)
        {
            Function.RegisterMathFunction(MathFunction.MultiLineFunction(function));
            this.FunctionsDescription.Add(function);
        }

        public void RegisterAllFunctions()
        {
            Function.ResetMathFunctions();
            foreach (var f in this.FunctionsDescription)
            {
                var mf = MathFunction.MultiLineFunction(f);
                Function.RegisterMathFunction(mf);
            }
        }
    }
}
