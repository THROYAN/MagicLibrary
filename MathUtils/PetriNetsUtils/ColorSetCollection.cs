using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.PetriNetsUtils
{
    public class ColorSetCollection
    {
        private List<ColorSet> colorSets { get; set; }

        public ColorSetCollection()
        {
            this.colorSets = new List<ColorSet>();
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
    }
}
