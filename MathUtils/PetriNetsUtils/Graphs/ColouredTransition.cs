using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.PetriNetsUtils.Graphs
{
    [Serializable]
    public class ColouredTransition : Transition
    {
        private ColouredPetriGraph _graph { get { return this.Graph as ColouredPetriGraph; } }

        private Dictionary<string, FunctionElement> _assVariables;
        private Dictionary<ColouredPlace, MultiSet<Function>> _addingTokens;
        private Dictionary<ColouredPlace, MultiSet<Function>> _removingTokens;

        public ColouredTransition(ColouredPetriGraph graph, string name) : base(graph, name) { }

        public ColouredPlace[] GetEnters()
        {
            List<ColouredPlace> l = new List<ColouredPlace>();
            Graph.GetEdges(e => (e as Arc).Head.Value.Equals(this.Value)).ForEach(edge => l.Add((edge as Arc).Tail as ColouredPlace));
            return l.ToArray();
        }

        public ColouredPlace[] GetExits()
        {
            List<ColouredPlace> l = new List<ColouredPlace>();
            Graph.GetEdges(e => (e as Arc).Tail.Value.Equals(this.Value)).ForEach(edge => l.Add((edge as Arc).Head as ColouredPlace));
            return l.ToArray();
        }

        public Dictionary<string, FunctionElement> AssignVariables()
        {
            Dictionary<string, FunctionElement> d = new Dictionary<string, FunctionElement>();

            Dictionary<ColouredPlace, Function> arcs = new Dictionary<ColouredPlace, Function>();
            
            foreach (var p in this.GetEnters())
            {
                arcs.Add(p, (this.Graph[p.Value, this.Value] as ColouredArc).ParsedFunction);
            }

            var flag1 = true; // индикатор того, что мы что-то нашли
            bool flag2 = false; // индикатор ошибки
            while (flag1)
            {
                flag1 = false;
                flag2 = false;

                // Проходимся по всем выражениям на дугах
                foreach (var arc in arcs)
                {
                    if (arc.Key.Tokens.Count == 0)
                    {
                        return null;
                    }

                    var ms = arc.Value.ToLeaf() as MultiSet<Function>;
                    if (ms != null) // если выражение на дуге - это мультимножество, то проходимся еще и по нему
                    {
                        MultiSet<Function> newMS = new MultiSet<Function>();
                        foreach (var set in ms)
                        {
                            foreach (var token in arc.Key.Tokens) // разбираем фишки в позиции, вдруг что подойдёт
                            {
                                if (token.Value != set.Value)
                                {
                                    continue;
                                }
                                var newValue = set.Key.SetVariablesValues(d) as Function;
                                newMS[newValue] = ms[set.Key];
                                if (!newValue.IsConstant())
                                {
                                    flag2 = true;
                                    var d1 = newValue.GetVariablesByConstant(token.Key);
                                    if (d1 == null) // ошибочка какая-то
                                    {
                                        return null;
                                    }
                                    if (d1.Count > 0) // если что-то нашли
                                    {
                                        foreach (var item in d1)
                                        {
                                            if (item.Value.IsConstant()) // и это еще и константа
                                            {
                                                flag1 = true;
                                                d[item.Key] = item.Value;
                                            }
                                        }
                                    }
                                }
                            } // конец цикла по фишкам
                        } // конец цикла по мультимножеству
                    }
                    else
                    {
                        foreach (var token in arc.Key.Tokens) // разбираем фишки в позиции, вдруг что подойдёт
                        {
                            var newValue = arc.Value.SetVariablesValues(d) as Function;
                            if (!newValue.IsConstant())
                            {
                                flag2 = true;
                                var d1 = newValue.GetVariablesByConstant(token.Key);
                                if (d1 == null) // ошибочка какая-то
                                {
                                    continue;
                                    //return null;
                                }
                                if (d1.Count > 0) // если что-то нашли
                                {
                                    foreach (var item in d1)
                                    {
                                        if (item.Value.IsConstant()) // и это еще и константа
                                        {
                                            flag1 = true;
                                            d[item.Key] = item.Value;
                                        }
                                    }
                                }
                            }
                        } // конец цикла по фишкам
                    }
                }
            }

            return flag2 ? null : d;
        }

        public virtual bool IsAvailable()
        {
            var enters = this.GetEnters();

            foreach (var p in enters)
            {
                if ((this.Graph[p.Value, this.Value] as ColouredArc).ParsedFunction == null)
                {
                    return false;
                }
            }

            foreach (var p in this.GetExits())
            {
                if ((this.Graph[this.Value, p.Value] as ColouredArc).ParsedFunction == null)
                {
                    return false;
                }
            }

            try
            {
                this._assVariables = this.AssignVariables();
                this._addingTokens = this.GetAddingTokens();
                this._removingTokens = this.GetRemovingTokens();
            }
            catch // I like it :)
            {
                return false;
            }

            return true;
        }

        public virtual void Execute()
        {
            if (!IsAvailable())
                return;

            foreach (var p in this.GetEnters())
            {
                p.Tokens = new MultiSet<Function>(p.Tokens.ApplyFunction("multiset sub", this._removingTokens[p]));
            }

            foreach (var p in this.GetExits())
            {
                p.Tokens = new MultiSet<Function>(p.Tokens.ApplyFunction("multiset sum", this._addingTokens[p]));
            }
        }

        public virtual Dictionary<ColouredPlace, MultiSet<Function>> GetRemovingTokens()
        {
            Dictionary<ColouredPlace, MultiSet<Function>> tokens = new Dictionary<ColouredPlace, MultiSet<Function>>();
            foreach (var p in this.GetEnters())
            {
                var temp = (Graph[p.Value, this.Value] as ColouredArc).ParsedFunction.Clone() as Function;
                tokens[p] = new MultiSet<Function>(temp.SetVariablesValues(this._assVariables));
            }
            return tokens;
        }

        public virtual Dictionary<ColouredPlace, MultiSet<Function>> GetAddingTokens()
        {
            Dictionary<ColouredPlace, MultiSet<Function>> tokens = new Dictionary<ColouredPlace, MultiSet<Function>>();

            foreach (var p in this.GetExits())
            {
                var temp = (Graph[this.Value, p.Value] as ColouredArc).ParsedFunction.Clone() as Function;
                temp = temp.SetVariablesValues(this._assVariables);
                foreach (var var in temp.Variables)
                {
                    temp = temp.SetVariableValue(var, this._graph.Colors.GetVariable(var).ColorSet.GetRandomValue()) as Function;
                }
                tokens[p] = new MultiSet<Function>(temp);
            }

            return tokens;
        }

        public override void CopyTo(IVertex vertex)
        {
            base.CopyTo(vertex);
        }

        public override object Clone()
        {
            ColouredTransition t = new ColouredTransition(this.Graph as ColouredPetriGraph, this.Name);

            this.CopyTo(t);

            return t;

        }
    }
}
