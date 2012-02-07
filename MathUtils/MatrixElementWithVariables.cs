using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    [Serializable]
    public class MatrixElementWithVariables : IMatrixElement
    {
        private Dictionary<string, double> element;

        public MatrixElementWithVariables()
        {
            element = new Dictionary<string, double>();
        }

        public MatrixElementWithVariables(Dictionary<string, double> element)
        {
            this.element = new Dictionary<string, double>(element);
        }

        public MatrixElementWithVariables(string variable, double constant)
        {
            element = new Dictionary<string, double>();
            element[variable] = constant;
        }

        public MatrixElementWithVariables(double c)
        {
            element = new Dictionary<string, double>();
            element[""] = c;
        }

        public MatrixElementWithVariables(string var)
        {
            element = new Dictionary<string, double>();
            element[var] = 1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var keyValue in this.element)
            {
                double constant = keyValue.Value;
                if (constant != 0 || element.Keys.Count == 1)
                {
                    if (!first && constant > 0)
                        sb.Append(" + ");
                    if (constant != 1 || keyValue.Key == "")
                        sb.AppendFormat("{0}{1}", constant < 0 ? " - " : "", Math.Abs(constant));
                    if (constant != 0)
                        sb.Append(keyValue.Key);
                    first = false;
                }
            }
            return sb.ToString();
        }

        public IMatrixElement Mul(IMatrixElement e)
        {
            Dictionary<string, double> element2 = (e as MatrixElementWithVariables).element, res = new Dictionary<string, double>();
            foreach (var keyValue1 in this.element)
            {
                foreach (var keyValue2 in element2)
                {
                    string var;

                    if (keyValue1.Key == "" && keyValue2.Key == "")
                        var = "";
                    else if (keyValue1.Key == "")
                        var = keyValue2.Key;
                    else if (keyValue2.Key == "")
                        var = keyValue1.Key;
                    else if (keyValue1.Key == keyValue2.Key)
                        var = keyValue1.Key + "^2";
                    else
                        var = keyValue1.Key + "+" + keyValue2.Key;

                    //= var1;
                    //if (var1 != var2)
                    //    var += " * " + var2;
                    //else if (var1 != "")
                    //    var += "^2";
                    if (res.ContainsKey(var))
                        res[var] += keyValue1.Value * keyValue2.Value;
                    else
                        res.Add(var, keyValue1.Value * keyValue2.Value);
                }
            }
            return new MatrixElementWithVariables(res);
        }

        public IMatrixElement Add(IMatrixElement e)
        {
            Dictionary<string, double> element2 = (e as MatrixElementWithVariables).element, res = new Dictionary<string, double>(element);
            foreach (string var1 in element2.Keys)
            {
                if (res.ContainsKey(var1))
                    res[var1] += element2[var1];
                else
                    res.Add(var1, element2[var1]);
            }

            return new MatrixElementWithVariables(res);
        }

        public IMatrixElement Zero()
        {
            return new MatrixElementWithVariables();
        }

        public IMatrixElement Clone()
        {
            return new MatrixElementWithVariables(element);
        }

        public IMatrixElement Inverse()
        {
            return Mul(new MatrixElementWithVariables(-1));
        }

        public IMatrixElement One()
        {
            return new MatrixElementWithVariables(1);
        }

        public IMatrixElement Sub(IMatrixElement e)
        {
            Dictionary<string, double> element2 = (e as MatrixElementWithVariables).element, res = new Dictionary<string, double>(element);
            foreach (string var1 in element2.Keys)
            {
                if (res.ContainsKey(var1))
                    res[var1] -= element2[var1];
                else
                    res.Add(var1, -element2[var1]);
            }

            return new MatrixElementWithVariables(res);
        }

        public static MatrixElementWithVariables operator *(double d, MatrixElementWithVariables e)
        {
            return (e.Mul(new MatrixElementWithVariables(d))) as MatrixElementWithVariables;
        }

        public static MatrixElementWithVariables operator *(string var, MatrixElementWithVariables e)
        {
            return (e.Mul(new MatrixElementWithVariables(var))) as MatrixElementWithVariables;
        }

        public MatrixElementWithVariables SetVariable(string var, double value)
        {
            Dictionary<string, double> res = new Dictionary<string, double>();

            foreach (string var1 in this.element.Keys)
            {
                if (var1 != var)
                {
                    if (res.ContainsKey(var1))
                    {
                        res[var1] += this.element[var1];
                    }
                    else
                    {
                        res.Add(var1, this.element[var1]);
                    }
                }
                else
                {
                    if (res.ContainsKey(""))
                    {
                        res[""] += this.element[var] * value;
                    }
                    else
                    {
                        res.Add("", this.element[var] * value);
                    }
                }
            }
            return new MatrixElementWithVariables(res);
        }

        public double GetConstant(string var = "")
        {
            return this.element[var];
        }


#warning доделать сортировку по степени в элементе матрицы.
        public MatrixElementWithVariables SortByPower()
        {
            MatrixElementWithVariables e = new MatrixElementWithVariables();

            element.Keys.ToList().ForEach(delegate(string var)
            {
                int powerIndex = var.IndexOf('^');
                if (powerIndex != -1)
                {
                    string var1 = var.Substring(0, powerIndex);
                    string power = var.Substring(powerIndex);

                    //element.Keys.ToList().FindAll(key => key.Length >= powerIndex && key.Substring(0, powerIndex) == var1).Max(
                    //    delegate(string key)
                    //    {
                    //        Convert.ToDouble(key.Substring(powerIndex));
                    //    }
                    //);
                }
            });

            return e;
        }


        public IMatrixElement Pow(double power)
        {
            Dictionary<string, double> res = new Dictionary<string, double>();
            this.element.Keys.ToList().ForEach(delegate(string var)
            {
                res.Add(var + "^" + power, Math.Pow(res[var], power));
            });
            return new MatrixElementWithVariables(res);
        }


        public double ToDouble()
        {
            return this.GetConstant();
        }

#warning доделать!!!
        //public double ToDouble(double[] variablesValues)
        //{

        //}


        public IMatrixElement Div(IMatrixElement e)
        {
            throw new NotImplementedException();
        }


        public IMatrixElement Sqrt()
        {
            throw new NotImplementedException();
        }


        public string ToMathML()
        {
            throw new NotImplementedException();
        }
    }
}
