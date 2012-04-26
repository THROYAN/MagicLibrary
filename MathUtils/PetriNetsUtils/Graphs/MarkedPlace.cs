using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class MarkedPlace : Place
    {
        public IColorSet ColorSet { get; set; }

        public List<Token> Tokens { get; set; }
        public MarkedPlace(IGraph graph, string name, uint tokensCount)
            : base(graph, name)
        {
            this.Tokens = new List<Token>();
            //this.Tokens.Add(new Token(new UnitColorSet("token"), "()", tokensCount));
        }

        /// <summary>
        /// Количество фишек
        /// </summary>
        public uint TokensCount { get { return (uint)this.Tokens.Count; } }

        /// <summary>
        /// Удаление фишек из позиции
        /// </summary>
        /// <param name="tokens"></param>
        public void RemoveTokens(Token[] tokens)
        {
            foreach (var token in tokens)
            {
                this.Tokens.Remove(token);
            }
        }

        public void AddTokens(Token[] tokens)
        {
            foreach (var token in tokens)
            {
                this.Tokens.Add(token);
            }
        }
    }
}
