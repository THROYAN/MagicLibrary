using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class ColorSetCollection : IEnumerable<ColorSet>
    {
        private List<ColorSet> colorSets { get; set; }

        public ColorSetCollection()
        {
            this.colorSets = new List<ColorSet>();
        }

        public ColorSetCollection(string colorsDescription)
        {
            this.colorSets = new List<ColorSet>();

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
            foreach (var c in cs)
            {
                if(!Regex.IsMatch(c, "^\\s*$"))
                    new ColorSet(c, this);
            }
        }

        public void AddColorSet(ColorSet cs)
        {
            if (!this.colorSets.Contains(cs) && !this.Contains(cs.Name))
            {
                this.colorSets.Add(cs);
            }
        }

        public bool Contains(ColorSet cs)
        {
            return this.colorSets.Contains(cs);
        }

        public bool Contains(string name)
        {
            return this.colorSets.Exists(c => c.Name.Equals(name));
        }

        public ColorSet this[string name]
        {
            get
            {
                return this.colorSets.Find(c => c.Name.Equals(name));
            }
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
    }
}
