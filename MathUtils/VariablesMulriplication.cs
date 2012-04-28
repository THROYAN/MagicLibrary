using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public class VariablesMulriplication : ICloneable
    {
        /// <summary>
        /// Value of constant before the multiplication.
        /// Like two in 2*x*y
        /// </summary>
        public double Constant { get; set; }
        private List<FunctionElement> variables;

        public VariablesMulriplication(FunctionElement[] variables, double constant = 1)
        {
            this.variables = new List<FunctionElement>();
            this.Constant = constant;
            foreach (var v in variables)
            {
                this.AddVariable(v);
            }
        }
        public VariablesMulriplication(FunctionElement variable, double constant = 1) : this(new FunctionElement[] { variable }, constant) { }
        public VariablesMulriplication(double constant = 1) : this(new FunctionElement[] { new Variable("") }, constant) { }
        public VariablesMulriplication(string name, double degree = 1, double constant = 1) : this(new FunctionElement[] { new Variable(name, degree) }, constant) { }

        public VariablesMulriplication AddVariable(FunctionElement variable)
        {
            if (this.HasVariable(variable.Name) && !variable.Functions.Exists(f => !(f is MagicLibrary.MathUtils.MathFunctions.PowerFunction)))
            {
                this.GetVariableByName(variable.Name).Degree += variable.Degree;
            }
            else
            {
                if (variable.IsConstant())
                {
                    if (this.variables.Count == 0)
                    {
                        this.variables.Add(new Variable(""));
                    }
                    this.Constant *= variable.ToDouble();
                }
                else
                {
                    if (variable is Function && variable.IsVariableMultiplication())
                    {
                        var temp = variable.ToVariableMultiplication();
                        temp.variables.ForEach(v => this.AddVariable(v));
                        this.Constant *= temp.Constant;
                    }
                    else
                    {
                        this.variables.Add(variable);
                    }
                }
            }
            return this;
        }

        private static Function _add(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            List<FunctionElement> vars = new List<FunctionElement>();
            vars1.variables.ForEach(v => vars.Add(v.Clone() as FunctionElement));

            if (vars1.Equals(vars2))
            {
                return new Function(new VariablesMulriplication(vars.ToArray(), vars1.Constant + vars2.Constant));
            }

            List<FunctionElement> vars22 = new List<FunctionElement>();
            vars2.variables.ForEach(v => vars22.Add(v.Clone() as FunctionElement));
            return new Function(new VariablesMulriplication[]{
                new VariablesMulriplication(vars.ToArray(), vars1.Constant),
                new VariablesMulriplication(vars22.ToArray(), vars2.Constant)
            });
        }

        private static Function _mul(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            List<FunctionElement> vars = new List<FunctionElement>();
            vars1.variables.ForEach(v => vars.Add(v.Clone() as FunctionElement));
            var d = 1.0;

            vars2.variables.ForEach(delegate(FunctionElement v)
            {
                var temp = vars.Find(v1 => v1.Name.Equals(v.Name));
                if (temp != null)
                {
                    temp.Degree += v.Degree;
                }
                else
                {
                    if (v.IsConstant())
                    {
                        d *= v.ToDouble();
                    }
                    else
                    {
                        vars.Add(v.Clone() as FunctionElement);
                    }
                }
            });
            return new Function(new VariablesMulriplication(vars.ToArray(), d * vars1.Constant * vars2.Constant));
        }

        private static Function _mul(VariablesMulriplication vars, double d)
        {
            VariablesMulriplication vars2 = vars.Clone() as VariablesMulriplication;
            vars2.Constant *= d;
            return new Function(vars2);
        }

        #region operator +

        public static Function operator +(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            return VariablesMulriplication._add(vars1, vars2);
        }

        public static Function operator +(VariablesMulriplication vars, FunctionElement var)
        {
            return vars + var.ToVariableMultiplication();
        }

        public static Function operator +(FunctionElement var, VariablesMulriplication vars)
        {
            return vars + var;
        }

        public static Function operator +(VariablesMulriplication vars, double d)
        {
            return vars + new VariablesMulriplication(d);
        }

        public static Function operator +(double d, VariablesMulriplication vars)
        {
            return vars + d;
        }

        #endregion

        #region operator -

        public static Function operator -(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            return vars1 + vars2 * -1;
        }

        public static Function operator -(VariablesMulriplication vars, FunctionElement var)
        {
            return vars - var.ToVariableMultiplication();
        }

        public static Function operator -(FunctionElement var, VariablesMulriplication vars)
        {
            return var.ToVariableMultiplication() - vars;
        }

        public static Function operator -(VariablesMulriplication vars, double d)
        {
            return vars + -d;
        }

        public static Function operator -(double d, VariablesMulriplication vars)
        {
            return new VariablesMulriplication(d) - vars;
        }

        #endregion

        #region operator *
        public static Function operator *(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            return VariablesMulriplication._mul(vars1, vars2);
        }

        public static Function operator *(VariablesMulriplication vars, FunctionElement var)
        {
            return vars * var.ToVariableMultiplication();
        }

        public static Function operator *(FunctionElement var, VariablesMulriplication vars)
        {
            return vars * var;
        }

        public static Function operator *(VariablesMulriplication vars, double d)
        {
            return VariablesMulriplication._mul(vars, d);
        }

        public static Function operator *(double d, VariablesMulriplication vars)
        {
            return vars * d;
        }
        #endregion

        #region operator /
        public static Function operator /(VariablesMulriplication vars1, VariablesMulriplication vars2)
        {
            return vars1 * vars2.Pow(-1);
        }

        public static Function operator /(VariablesMulriplication vars, FunctionElement var)
        {
            return vars / var.ToVariableMultiplication();
        }

        public static Function operator /(FunctionElement var, VariablesMulriplication vars)
        {
            return var.ToVariableMultiplication() / vars;
        }

        public static Function operator /(VariablesMulriplication vars, double d)
        {
            return vars * Math.Pow(d, -1);
        }

        public static Function operator /(double d, VariablesMulriplication vars)
        {
            return new VariablesMulriplication(d) / vars;
        }

        #endregion

        public object Clone()
        {
            List<FunctionElement> vars = new List<FunctionElement>();
            var d = 1.0;
            bool isVariables = false;
            foreach (var item in this.variables)
            {
                if (item.IsConstant())
                {
                    d *= item.ToDouble();
                }
                else
                {
                    isVariables = true;
                    vars.Add(item.Clone() as FunctionElement);
                }
            }
            //this.variables.ForEach(v => vars.Add(v.Clone() as FunctionElement));
            if (!isVariables)
                vars.Add(new Variable(""));
            return new VariablesMulriplication(vars.ToArray(), this.Constant * d);
        }

        private string getVarString(FunctionElement e)
        {
            var degree = Math.Abs(e.Degree);
            if (degree == 1)
            {
                return e.Name;
            }
            /*if (degree == 0.5)
            {
                return String.Format("sqrt({0})", e.Name);
            }*/
            if (e is Variable)
            {
                return String.Format("{0}^{1}", e.Name, degree);
            }
            else
            {
                if ((e as Function).IsNeedBracketsForPower())
                {
                    return String.Format("({0})^{1}", e.Name, degree);
                }
                else
                {
                    return String.Format("{0}^{1}", e.Name, degree);
                }
            }
        }

        public override string ToString()
        {
            if (this.Constant == 0)
                return "0";

            StringBuilder sb = new StringBuilder();
            
            int upCount = 0, downCount = 0;
            StringBuilder upSB = new StringBuilder();
            StringBuilder downSB = new StringBuilder();

            if (this.Constant < 0)
            {
                sb.Append(" - ");
            }

            var constant = Math.Abs(this.Constant);
            if (constant != 1)
            {
                if (Math.Round(constant) != constant && Math.Round(1.0 / constant) == 1.0 / constant)
                {
                    downSB.AppendFormat("{0}*", Math.Round(1.0 / constant));
                    downCount++;
                }
                else
                {
                    upSB.AppendFormat("{0}*", constant);
                    upCount++;
                }
            }

            foreach (var v in this.variables)
            {
                if (v.Name != "" && !v.ToString().Equals("1"))
                {
                    if (v.Degree < 0)
                    {
                        if (v is Variable)
                        {
                            downSB.AppendFormat("{0}*", this.getVarString(v));
                            downCount++;
                        }
                        else
                        {
                            if ((v as Function).IsNeedBrackets())
                            {
                                downSB.AppendFormat("({0})*", this.getVarString(v));
                            }
                            else
                            {
                                downSB.AppendFormat("{0}*", this.getVarString(v));
                            }
                            downCount++;
                        }
                    }
                    if (v.Degree > 0)
                    {
                        if (v is Variable)
                        {
                            upSB.AppendFormat("{0}*", this.getVarString(v));
                            upCount++;
                        }
                        else
                        {
                            if ((v as Function).IsNeedBrackets())
                            {
                                upSB.AppendFormat("({0})*", this.getVarString(v));
                            }
                            else
                            {
                                upSB.AppendFormat("{0}*", this.getVarString(v));
                            }
                            upCount++;
                        }
                    }
                }
            }

            if (upSB.Length == 0)
            {
                upSB = new StringBuilder("1");
            }
            else
            {
                upSB.Remove(upSB.Length - 1, 1);
            }

            if (downSB.Length != 0)
            {
                downSB.Remove(downSB.Length - 1, 1);
                if (upCount > 1)
                {
                    sb.AppendFormat("({0})", upSB);
                }
                else
                {
                    sb.Append(upSB);
                }
                sb.Append(" / ");
                if (downCount > 1)
                {
                    sb.AppendFormat("({0})", downSB);
                }
                else
                {
                    sb.Append(downSB);
                }
                //sb.AppendFormat("({0}) / ({1})", upSB, downSB);
            }
            else
            {
                sb.Append(upSB);
            }

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is VariablesMulriplication)
            {
                var v = obj as VariablesMulriplication;

                if (this.VarsCount != v.VarsCount)
                    return false;

                for (int i = 0; i < v.variables.Count; i++)
                {
                    /*if (!v.variables[i].IsConstant())
                    {
                        */
                        if (!v.variables[i].Equals(this.GetVariableByName(v.variables[i].Name)))
                        {
                            return false;
                        }
                    /*}
                    else
                    {
                        //if (v.Constant != 0 && this.variables.Exists(var => !var.IsConstant()))
                        //    return false;
                    }*/
                }

                for (int i = 0; i < this.variables.Count; i++)
                {
                    //if (!this.variables[i].IsConstant())
                    //{

                        if (!this.variables[i].Equals(v.GetVariableByName(this.variables[i].Name)))
                        {
                            return false;
                        }
                    //}
                    //else
                    //{
                    //    //if (!v.variables.Exists(var => !var.IsVariable()))
                    //    //    return false;
                    //}
                }

                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Return variable object by specify identifier - name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Variable instanse</returns>
        public FunctionElement GetVariableByName(string name)
        {
            return this.variables.Find(v => v.HasVariable(name));
        }

        /// <summary>
        /// Check existing of variable with specify name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasVariable(string name)
        {
            return this.Variables.Contains(name);
            //return this.variables.Exists(v => v.Name.Equals(name) && v.IsVariable());
        }

        /// <summary>
        /// Check existing of variable with specify name and degree
        /// </summary>
        /// <param name="name"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public bool HasVariable(string name, double degree)
        {
            return this.variables.Exists(v => v.Name.Equals(name) && v.Degree == degree);
        }

        /// <summary>
        /// Set variable value by name.
        /// If variable with specify name is not in variables list, it's returned copy of this object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Function SetVariableValue(string name, double value)
        {
            return this.SetVariableValue(name, new Variable("", value));
        }

        public Function SetVariableValue(string name, FunctionElement value)
        {
            if (this.variables.Count == 0)
                return new Function();
            Function f = new Function(1);
            this.variables.ForEach(delegate(FunctionElement v)
            {
                var temp = v.Clone() as FunctionElement;
                if (!temp.IsConstant() && temp.HasVariable(name))
                {
                    f *= temp.SetVariableValue(name, value);
                }
                else
                {
                    f *= temp;
                }
            });
            return f * this.Constant;
        }

        /// <summary>
        /// Return variable by adding order
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public FunctionElement this[int i]
        {
            get
            {
                return this.variables[i];
            }
        }

        /// <summary>
        /// Count of all variables (with empty)
        /// </summary>
        public int Count
        {
            get
            {
                return this.variables.Count(v => v.ToString() != "1" && v.ToString() != "");
            }
        }

        /// <summary>
        /// Count of available variables.
        /// Value of name isn't like "" and degree not equals 0.
        /// </summary>
        public int VarsCount
        {
            get
            {
                if(this.Constant == 0)
                    return 0;
                return this.Variables.Length;
            }
        }

        public VariablesMulriplication Derivative(string name)
        {
            if (this.HasVariable(name))
            {
                VariablesMulriplication vs = new VariablesMulriplication(this.Constant);
                VariablesMulriplication upWithVar = null;
                VariablesMulriplication downWithVar = null;

                this.variables.ForEach(delegate(FunctionElement v)
                {
                    var temp = v.Clone() as FunctionElement;
                    if (temp.HasVariable(name))
                    {
                        if (temp.Degree < 0)
                        {
                            if (downWithVar != null)
                            {
                                downWithVar.AddVariable(temp);
                            }
                            else
                            {
                                downWithVar = new VariablesMulriplication(temp);
                            }
                        }
                        else
                        {
                            if (upWithVar != null)
                            {
                                upWithVar.AddVariable(temp);
                            }
                            else
                            {
                                upWithVar = new VariablesMulriplication(temp);
                            }
                        }
                    }
                    else
                    {
                        vs.AddVariable(temp);
                    }
                });

                if (upWithVar != null)
                {
                    if (downWithVar != null && upWithVar.Count == 1 && downWithVar.Count == 1)
                    {
                        vs.AddVariable((upWithVar.Derivative(name) * downWithVar - downWithVar.Derivative(name) * upWithVar) / downWithVar.Pow(2));
                    }
                    else
                    {
                        if (downWithVar != null)
                        {
                            downWithVar.variables.ForEach(v => upWithVar.variables.Add(v));
                        }
                        Function upFunc = new Function();
                        foreach (var v1 in upWithVar.variables)
                        {
                            VariablesMulriplication temp = v1.Derivative(name);
                            foreach (var v2 in upWithVar.variables)
                            {
                                if (v1 != v2) // Да, именно по ссылкам
                                {
                                    temp.AddVariable(v2);
                                }
                            }
                            upFunc += temp;
                        }
                        vs.AddVariable(upFunc);
                    }
                }
                return vs;
            }
            return new VariablesMulriplication(0);
        }

        public string[] Variables
        {
            get
            {
                if (this.Constant == 0)
                    return new string[0];
                List<string> vars = new List<string>();
                this.variables.ForEach(delegate(FunctionElement v)
                {
                    if (v is Variable)
                    {
                        if (!v.IsConstant() && !vars.Contains(v.Name))
                            vars.Add(v.Name);
                    }
                    else
                    {
                        var vars2 = (v as Function).Variables;
                        foreach (var v2 in vars2)
                        {
                            if (!vars.Contains(v2))
                                vars.Add(v2);
                        }
                    }
                });
                return vars.ToArray();
            }
        }

        public VariablesMulriplication Pow(double power)
        {
            List<FunctionElement> vs = new List<FunctionElement>();
            this.variables.ForEach(v => vs.Add(v.Pow(power)));
#warning переделать!!
            return new VariablesMulriplication(vs.ToArray(), Math.Pow(this.Constant, power));
        }

        public bool IsFunction()
        {
            if (this.Constant != 1)
                return false;
            return (
                    this.variables.Exists(v => v.Name.Equals("")) && this.variables.Count == 2 && 
                        ((this.variables[0] is Function /*&& this.variables[0].Degree == 1*/) || 
                        (this.variables[1] is Function /*&& this.variables[1].Degree == 1*/))
                ) || (this.variables.Count == 1 && this.variables[0] is Function/* && this.variables[0].Degree == 1*/);
        }

        public Function ToFunction()
        {
            if (this.IsFunction())
            {
                Function f;
                if (this.variables[0].Name == "")
                    f = new Function(this.variables[1]);
                else
                    f = new Function(this.variables[0]);
                return f.ForceMul(new Function(this.Constant));
            }
            throw new Exception();
        }

        private string getVarMLString(FunctionElement e, bool needBrackets = true)
        {
            MagicLibrary.MathUtils.MathFunctions.PowerFunction p = new MathFunctions.PowerFunction(e.Degree);
            var format = p.FormatPower();
            var name = e.ToMathMLShort();

            if (e is Function && (e as Function).IsNeedBracketsForPower() && (
                    p.IsNeedBrackets() ||
                    needBrackets
                ))
            {
                name = String.Format("<mfenced><mrow><mi>{0}</mi></mrow></mfenced>", name);
            }
            else
            {
                name = String.Format("<mi>{0}</mi>", name);
            }
            return String.Format(format, name);
        }

        public string ToMathML()
        {
            string mul = "<mo>*</mo>";
            if (this.Constant == 0)
                return "<mi>0</mi>";

            StringBuilder sb = new StringBuilder();
            
            List<FunctionElement> upVars = new List<FunctionElement>(),
                downVars = new List<FunctionElement>();

            if (this.Constant < 0)
            {
                sb.Append("<mo>-</mo>");
            }

            var constant = Math.Abs(this.Constant);
            if (constant != 1)
            {
                if (Math.Round(constant) != constant && Math.Round(1.0 / constant) == 1.0 / constant)
                {
                    downVars.Add(new Function(Math.Round(1.0 / constant)));
                }
                else
                {
                    upVars.Add(new Function(constant));
                }
            }

            if (this.Count == 1 && this.Constant == 1)
            {
                var v = this.variables.Find(v1 => v1.Name != "");
                if (v.Degree < 0)
                {
                    downVars.Add(v);
                }
                else
                {
                    upVars.Add(v);
                }
            }
            else
            {
                foreach (var v in this.variables)
                {
                    if (v.Name != "" && !v.ToString().Equals("1"))
                    {
                        if (v.Degree < 0)
                        {
                            downVars.Add(v);
                        }
                        else
                        {
                            upVars.Add(v);
                        }
                    }
                }
            }

            StringBuilder upSB = new StringBuilder(), downSB = new StringBuilder();
            if (upVars.Count == 0)
            {
                upVars.Add(new Function(1));
            }

            if (upVars.Count == 1 && (this.Constant != -1 || downVars.Count != 0))
            {
                upSB.Append(this.getVarMLString(upVars[0], false));
            }
            else
            {
                for (int i = 0; i < upVars.Count - 1; i++)
                {
                    upSB.AppendFormat("<mrow>{0}</mrow>{1}", this.getVarMLString(upVars[i]), mul);
                }
                upSB.AppendFormat("<mrow>{0}</mrow>", this.getVarMLString(upVars.Last()));
            }

            if (downVars.Count != 0)
            {
                if (downVars.Count == 1)
                {
                    downSB.Append(this.getVarMLString(downVars[0], false));
                }
                else
                {
                    for (int i = 0; i < downVars.Count - 1; i++)
                    {
                        downSB.AppendFormat("<mrow>{0}</mrow>{1}", this.getVarMLString(downVars[i]), mul);
                    }
                    downSB.AppendFormat("<mrow>{0}</mrow>", this.getVarMLString(downVars.Last()));
                }

                sb.AppendFormat("<mrow><mfrac><mrow>{0}</mrow><mrow>{1}</mrow></mfrac></mrow>", upSB, downSB);
            }
            else
            {
                sb.AppendFormat("<mrow>{0}</mrow>", upSB);
            }

            return sb.ToString();
        }
    }
}
