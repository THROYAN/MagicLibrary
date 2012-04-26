using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;

namespace MagicLibrary.MathUtils
{
    public class Function : FunctionElement
    {
        private List<VariablesMulriplication> variables;

        public Function(double constant = 0, string varName = "", double degree = 1)
        {
            variables = new List<VariablesMulriplication>();
            this.Functions = new List<MathFunctions.IMathFunction>();
            this.Degree = degree;
            this.AddVariablesMul(new VariablesMulriplication(new Variable(varName, degree), constant));
        }

        public Function(VariablesMulriplication v, double degree = 1)
        {
            variables = new List<VariablesMulriplication>();
            this.Functions = new List<MathFunctions.IMathFunction>();
            this.Degree = degree;
            this.AddVariablesMul(v);
        }

        public Function(VariablesMulriplication[] variables, double degree = 1)
        {
            this.variables = new List<VariablesMulriplication>();
            this.Functions = new List<MathFunctions.IMathFunction>();
            this.Degree = degree;
            foreach (var v in variables)
            {
                this.AddVariablesMul(v);
            }
        }

        public Function(FunctionElement el)
        {
            this.variables = new List<VariablesMulriplication>();
            this.Functions = new List<MathFunctions.IMathFunction>();
            if (el is Variable)
            {
                this.AddVariablesMul(new VariablesMulriplication(el));
            }
            else
            {
                this.variables = new List<VariablesMulriplication>((el as Function).variables);
                el.Functions.ForEach(f => this.Functions.Add(f.Clone() as IMathFunction));
            }
        }

        public void AddVariablesMul(VariablesMulriplication variablesMulriplication)
        {
            var temp = variablesMulriplication.ToString();
            if (temp == "0" || temp == "")
                return;
            var v = this.variables.Find(vs => vs.Equals(variablesMulriplication));
            if (v != null)
            {
                v.Constant += variablesMulriplication.Constant;
            }
            else
            {
                if (variablesMulriplication.IsFunction())
                {
                    //this.variables.Add(variablesMulriplication);
                    //(this + variablesMulriplication.ToFunction()).CopyTo(this);
                    var func = variablesMulriplication.ToFunction();
                    if (func.Functions.Exists(f => !(f is Power)) || func.Degree != 1)
                    {
                        this.variables.Add(variablesMulriplication);
                    }
                    else
                    {
                        func.variables.ForEach(vs => this.AddVariablesMul(vs));
                    }
                }
                else
                {
                    this.variables.Add(variablesMulriplication);
                }
            }
        }

        private static Function _add(Function f1, Function f2)
        {
            Function f = new Function();

            foreach (VariablesMulriplication vars in f1.variables)
            {
                if (vars.Constant != 0)
                {
                    f.AddVariablesMul(vars.Clone() as VariablesMulriplication);
                }
            }

            foreach (VariablesMulriplication vars in f2.variables)
            {
                if (vars.Constant != 0)
                {
                    f.AddVariablesMul(vars.Clone() as VariablesMulriplication);
                }
            }

            return f;
        }

        private static Function _mul(Function f1, Function f2)
        {
            Function f = new Function();

            foreach (VariablesMulriplication vars1 in f1.variables)
            {
                foreach (VariablesMulriplication vars2 in f2.variables)
                {
                    f += vars1 * vars2;
                }
            }

            return f;
        }

        public Function ForceMul(FunctionElement e)
        {
            if (e.IsConstant() && e.ToDouble() == 1)
            {
                return this.Clone() as Function;
            }
            Function f = new Function();
            this.variables.ForEach(vs => f.variables.Add((vs * (e.Pow(1.0 / this.Degree))).variables[0]));
            f.OverrideFunctions(this);
            return f;
        }
/*
        #region operator +
        //public static Function operator +(Function f1, Function f2)
        //{
        //    return Function._add(f1, f2);
        //}

        //public static Function operator +(Function f, VariablesMulriplication vs)
        //{
        //    return Function._add(f, new Function(vs));
        //}

        //public static Function operator +(VariablesMulriplication vs, Function f)
        //{
        //    return f + vs;
        //}

        //public static Function operator +(Function f, FunctionElement v)
        //{
        //    return f + new Function(v);
        //}

        //public static Function operator +(FunctionElement v, Function f)
        //{
        //    return f + v;
        //}

        //public static Function operator +(Function f, double d)
        //{
        //    return f + new Function(d);
        //}

        //public static Function operator +(double d, Function f)
        //{
        //    return f + d;
        //}

        #endregion

        #region operator -
        //public static Function operator -(Function f1, Function f2)
        //{
        //    return f1 + f2 * -1;
        //}

        //public static Function operator -(Function f, VariablesMulriplication vs)
        //{
        //    return f + vs * -1;
        //}

        //public static Function operator -(VariablesMulriplication vs, Function f)
        //{
        //    return new Function(vs) - f;
        //}

        //public static Function operator -(Function f, FunctionElement v)
        //{
        //    return f - new Function(v);
        //}

        //public static Function operator -(FunctionElement v, Function f)
        //{
        //    return new Function(v) - f;
        //}

        //public static Function operator -(Function f, double d)
        //{
        //    return f + new Function(-d);
        //}

        //public static Function operator -(double d, Function f)
        //{
        //    return new Function(d) - f;
        //}
        #endregion

        #region operator *
        //public static Function operator *(Function f1, Function f2)
        //{
        //    return Function._mul(f1, f2);
        //}

        //public static Function operator *(Function f, VariablesMulriplication vs)
        //{
        //    return f * new Function(vs);
        //}

        //public static Function operator *(VariablesMulriplication vs, Function f)
        //{
        //    return f * vs;
        //}

        //public static Function operator *(Function f, FunctionElement v)
        //{
        //    return f * new Function(v);
        //    if ((v is Variable || v.IsConstant()) && f.Degree == 1)
        //    {
        //        Function newF = new Function();
        //        foreach (VariablesMulriplication vars in f.variables)
        //        {
        //            newF += vars * v;
        //        }
        //        return newF;
        //    }
        //    else
        //    {
        //        return new Function(new VariablesMulriplication(new FunctionElement[] { f, v }));
        //    }
        //}

        //public static Function operator *(FunctionElement v, Function f)
        //{
        //    return f * v;
        //}

        //public static Function operator *(Function f, double d)
        //{
        //    if (f.Degree != 1)
        //    {
        //        Function F = new Function();
        //        F.AddVariablesMul(new VariablesMulriplication(d));
        //        F.AddVariablesMul(new VariablesMulriplication(f));
        //        return F;
        //    }
        //    Function newF = new Function();
        //    foreach (VariablesMulriplication vars in f.variables)
        //    {
        //        newF += vars * d;
        //    }
        //    return newF;
        //}

        //public static Function operator *(double d, Function f)
        //{
        //    return f * d;
        //}
        #endregion

        #region operator /
//        public static Function operator /(Function f1, Function f2)
//        {
//#warning Сделать деление функций
//            if (f2.IsConstant())
//            {
//                return f1 / f2.ToDouble();
//            }

//            if (f2.VarsCount > 1 || (f2.VarsCount == 0 && f2.ToDouble() == 0))
//            {
//                Function f = f1.Clone() as Function;
//                Function f22 = f2.Clone() as Function;
//                f22.Degree *= -1;
//                f.variables.Add(new VariablesMulriplication(f2));
//                return f;
//                throw new Exception("Я еще не умею так делить.");
//            }
//            Function res = new Function();
//            VariablesMulriplication d = f2.variables.Find(v => v.VarsCount != 0);

//            foreach (var v in f1.variables)
//            {
//                res += v / d;
//            }
//            return res;
//        }

//        public static Function operator /(Function f, VariablesMulriplication vs)
//        {
//            Function newF = new Function();
//            foreach (var v in f.variables)
//            {
//                newF += v / vs;
//            }
//            return newF;
//        }

//        public static Function operator /(VariablesMulriplication vs, Function f)
//        {
//            return new Function(vs) / f;
//        }

//        public static Function operator /(Function f, FunctionElement v)
//        {
//            Function newF = new Function();
//            foreach (var vs in f.variables)
//            {
//                newF += vs / v;
//            }
//            return newF;
//        }

//        public static Function operator /(FunctionElement v, Function f)
//        {
//            return new Function(new VariablesMulriplication(v)) / f;
//        }

//        public static Function operator /(Function f, double d)
//        {
//            Function newF = new Function();
//            foreach (var vs in f.variables)
//            {
//                newF += vs / d;
//            }
//            return newF;
//        }

//        public static Function operator /(double d, Function f)
//        {
//            return new Function(d) / f;
//        }

        #endregion
        */
        public override object Clone()
        {
            List<VariablesMulriplication> vars = new List<VariablesMulriplication>();
            foreach (var vs in this.variables)
            {
                if (vs.Constant != 0)
                {
                    vars.Add(vs.Clone() as VariablesMulriplication);
                }
            }
            //this.variables.ForEach(v => vars.Add(v.Clone() as VariablesMulriplication));
            var temp = new Function(vars.ToArray(), this.Degree);
            temp.OverrideFunctions(this);
            return temp;// new Function(vars.ToArray(), this.Degree);
        }

        public Function Inverse()
        {
            return this * -1;
        }

        public override FunctionElement Pow(double power)
        {
            /*if (this.VariablesMulsWithVariablesCount == 1 && this.VariablesMulsWithoutVariablesCount == 0)
            {
                var vs = this.variables.Find(v => v.VarsCount != 0);
                return new Function(vs.Pow(power));
            }*/
            if (this.IsConstant())
            {
                return new Function(Math.Pow(this.ToDouble(), power));
            }
            Function F = this.Clone() as Function;
            F.Degree *= power;
            return F;
        }

        public FunctionElement Sqrt()
        {
            return this.Pow(0.5);
        }

        public override double ToDouble()
        {
            var v = this.variables.Find(vs => vs.VarsCount == 0 && vs.Constant != 0);
            return v == null ? 0 : this.Calculate(v.Constant);
        }

        public FunctionElement SetVariablesValues(Dictionary<string, double> dict)
        {
            Function e = this.Clone() as Function;

            foreach (var item in dict)
            {
                e = e.SetVariableValue(item.Key, item.Value) as Function;
            }

            if (e.IsConstant())
            {
                return new Function(e.Calculate(e.ToDouble()));
            }

            return e;
        }

        public Equation EquationSolutionByVariable(string name, Function equationAnswer = null)
        {
            if (equationAnswer == null)
                equationAnswer = new Function(0);
            Equation equation = new Equation(
                this - equationAnswer,
                new Function(0)
            );

            Function curX, nextX = new Function(1);
            double eps = 0.0001;
            Function derivative = new Function(equation.LeftPart.Derivative());
            do
            {
                curX = nextX;
                var up = equation.LeftPart.SetVariableValue(name, curX);
                var down = derivative.SetVariableValue(name, curX);
                nextX = curX - up / down;
            }
            while (Math.Abs((curX - nextX).ToDouble()) > eps);
            return new Equation(new Function(1, name), nextX);

            double degree = equation.LeftPart.Degree * equation.LeftPart.variables.Max(delegate(VariablesMulriplication vs)
            {
                if (vs.HasVariable(name))
                {
                    return vs.GetVariableByName(name).Degree;
                }
                return 0;
            });

            switch (degree.ToString())
            {
                case "1":
                    equation.LeftPart.variables.ForEach(delegate(VariablesMulriplication vs)
                    {
                        if (vs.HasVariable(name))
                        {
                            if (vs.Constant < 0)
                            {
                                equation.InverseSign();
                            }
                            var newV = vs.GetVariableByName(name);
                            //leftPart.AddVariablesMul(newV);
                            equation.LeftPart *= newV / vs;
                            equation.RightPart *= newV / vs;
                        }
                        else
                        {
                            //rightPart = rightPart.Mul(new EquationMatrixElement(new VariablesMulriplication(1) / vs)) as EquationMatrixElement;
                            equation.RightPart -= vs;
                        }
                    });
                    equation.LeftPart = new Function(1, name);
                    break;
                // Квадратное уравнение
                case "2":
                    // Всё умножения, содержащие нужную переменную в 1 степени
                    var v = equation.LeftPart.GetElementsWithVariable(name, 1);
                    // И во 2
                    var v2 = equation.LeftPart.GetElementsWithVariable(name, 2);
                    // Если есть только 2 степень
                    if (v.VarsCount == 0)
                    {
                        // берём коэффициенты при 2 степени, т.е. делим всё, что есть со 2 степенью на эту переменную во второй степени
                        var elements = v2 / new Variable(name, 2);

                        // правая часть - это всё без переменной во 2 степени с минусом и делённое на коэффициент при переменной во 2 степеи
                        var right = equation.LeftPart.GetElementsWithoutVariable(name, 2) / elements * -1;

                        // берём корень
                        equation.RightPart = new Function(right.Sqrt());
                        equation.LeftPart = new Function(1, name);
                        // всё
                        return equation;
                    }
                    else
                    {
                        // коэффциент при второй степени
                        Function a = v2 / new Variable(name, 2);
                        // при первой
                        Function b = v / new Variable(name, 1);
                        // и всё, что осталось
                        Function c = equation.LeftPart - (v + v2);

                        // доделываем всё это дело до квадрата суммы

                        // Находим какое должно быть c, чтобы это было квадрат суммы
                        Function neededC = new Function((b / (a.Sqrt() * 2)).Pow(2));

                        // находим остаток, например нужно c = 5, а есть c = -4, делаем 5 + (-4 - 5) = -4
                        var rightC = c - neededC;
                        if (rightC.VarsCount > 0 || rightC.ToDouble() <= 0)
                        {
                            // Переносим вправо и берём корень
                            equation.RightPart = rightC.Inverse().Sqrt() as Function;

                            // узнаём ли у нас квадрат суммы или разности

                            // Слева остаётся квадрат суммы, типа взяли корень, осталось sqrt(a) +(-) sqrt(neededC) = sqrt(-rightC)
                            // Делаем так, чтобы осталось только наша переменная
                            equation.RightPart -= b / (a.Sqrt() * 2);
                            equation.RightPart /= a.Sqrt();


                            equation.LeftPart = new Function(1, name);
                            return equation;
                        }

                        // Если это не сумма квадратов, решаем квадратное уравнение
                        Function d = b.Pow(2) - a * c * 4;
                        if (d.VarsCount > 0 || d.ToDouble() >= 0)
                        {
                            Function x1 = b.Inverse() + d.Sqrt() / (a * 2);
                            Function x2 = b.Inverse() - d.Sqrt() / (a * 2);
                            equation.RightPart = x1;
                            equation.LeftPart = new Function(1, name);
                            return equation;
                        }
                        else
                        {
                            throw new Exception("Нет решений. Я еще не знаю комплексных чисел. =)");
                        }
                    }
                default:
                    break;
            }

            return equation;
        }

        public Function GetElementsWithVariable(string name)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (vs.HasVariable(name))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }
        public Function GetElementsWithVariable(string name, double degree)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (vs.HasVariable(name, degree))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        public Function GetElementsWithoutVariable(string name)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (!vs.HasVariable(name))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        public Function GetElementsWithoutVariable(string name, double degree)
        {
            Function f = new Function();
            foreach (var vs in this.variables)
            {
                if (!vs.HasVariable(name, degree))
                {
                    f.AddVariablesMul(vs);
                }
            }
            return f;
        }

        /// <summary>
        /// Set variables in equation.
        /// If use vector, variables are set by adding order.
        /// Using valuesVector, when first col is variables name and second is values, solve uncertainly.
        /// If variables repeated, it sets only first value.
        /// </summary>
        /// <param name="valuesVector">Vector or valuesVector.</param>
        /// <returns></returns>
        public FunctionElement SetVariablesValues(Matrix valuesVector)
        {
            if (valuesVector.Cols == 1)
            {
                //if (valuesVector.Rows > this.VarsCount)
                //{
                //    throw new InvalidMatrixSizeException("Too much values");
                //}
                string[] vars = this.Variables;
                Dictionary<string, IMatrixElement> dict = new Dictionary<string, IMatrixElement>();
                for (int i = 0; i < valuesVector.Rows && i < vars.Length; i++)
                {
                    //this.SetVariableValue(vars[i], valuesVector[i, 0].ToDouble());
                    dict.Add(vars[i], valuesVector[i, 0]);
                }
                return this.SetVariablesValues(dict);
            }
            if (valuesVector.Cols == 2)
            {
                Dictionary<string, IMatrixElement> dict = new Dictionary<string, IMatrixElement>();
                for (int i = 0; i < valuesVector.Rows; i++)
                {
                    //this.SetVariableValue(valuesVector[i, 0].ToString(), valuesVector[i, 1].ToDouble());
                    dict.Add(valuesVector[i, 0].ToString(), valuesVector[i, 1]);
                }
                return this.SetVariablesValues(dict);
            }
            throw new InvalidMatrixSizeException("Vector must have only one col, or I can use 2 col matrix, when first col is variables names");
        }

        public FunctionElement SetVariablesValuesWithFixedOrder(Matrix valuesVector, Matrix variablesOrder)
        {
            if (valuesVector.Rows != variablesOrder.Rows)
                throw new Exception("Вектора значений и переменных должны иметь одинаковое количество строк");

            if (variablesOrder.Cols != 1 || variablesOrder.Cols != 1)
                throw new Exception("Вектор должен содержать только 1 столбик");

            Matrix newValuesVector = new Matrix(valuesVector.Rows, 2, new FunctionMatrixElement());
            for (int i = 0; i < valuesVector.Rows; i++)
            {
                // Первый столбик будет содержать имена переменных
                newValuesVector[i, 0] = variablesOrder[i, 0];
                // Второй столбик - значения
                newValuesVector[i, 1] = valuesVector[i, 0];
            }

            return this.SetVariablesValues(newValuesVector);
        }

        public Function SetVariablesValues(Dictionary<string, IMatrixElement> dict)
        {
            Function e = this.Clone() as Function;

            foreach (var item in dict)
            {
                e = e.SetVariableValue(item.Key, item.Value);
            }

            if (e.IsConstant())
            {
                return new Function(e.Calculate(e.ToDouble()));
            }
            return e;
        }

        private Function SetVariableValue(string name, IMatrixElement value)
        {
            if (value is FunctionMatrixElement)
            {
                return new Function(this.SetVariableValue(name, (value as FunctionMatrixElement).Function));
            }
            if (value is DoubleMatrixElement)
            {
                return new Function(this.SetVariableValue(name, (value as DoubleMatrixElement).D));
            }
            throw new Exception("Unsupported matrix element type");
        }

        /// <summary>
        /// Set value instead of the variable with the specify name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override FunctionElement SetVariableValue(string name, double value)
        {
            return this.SetVariableValue(name, new Function(value));
        }

        public override FunctionElement SetVariableValue(string name, FunctionElement e)
        {
            Function newF = new Function();
            this.variables.ForEach(v => newF += v.SetVariableValue(name, e));
            //newF.Degree = this.Degree;
            //newF.Functions = new List<MathFunctions.IMathFunction>(this.Functions);
            newF.OverrideFunctions(this);
            if (newF.IsConstant())
                return new Function(newF.ToDouble());//Math.Pow(newF.ToDouble(), this.Degree));
            return newF;
        }

        public override VariablesMulriplication Derivative(string name)
        {
            VariablesMulriplication newVS = new VariablesMulriplication();
            Function e = new Function();
            
            this.variables.ForEach(vs => e.AddVariablesMul(vs.Derivative(name)));
            newVS.AddVariable(e);

            if (this.Degree != 1)
            {
                Function predF = this.Clone() as Function;
                predF.Degree--;
                newVS.AddVariable(predF);
                newVS.Constant *= this.Degree;
            }

            return newVS;
        }

        public Matrix Gradient()
        {
            string[] vars = this.Variables;

            Matrix m = new Matrix(vars.Length, 1);

            for (int i = 0; i < vars.Length; i++)
            {
                m[i, 0] = new FunctionMatrixElement(new Function(this.Derivative(vars[i])));
            }

            return m;
        }

        public override string ToString()
        {
            if (this.Degree == 0)
                return "1";
            if (this.Degree == 1)
                return this.Name;
            /*if (this.Degree == 0.5)
            {
                return String.Format("sqrt({0})", this.Name);
            }*/
            if (this.Degree < 0)
            {
                if (this.IsNeedBracketsForPower())
                {
                    return String.Format("({0})^({1})", this.Name, this.Degree);
                }
                return String.Format("{0}^({1})", this.Name, this.Degree);
            }

            if (this.IsNeedBracketsForPower())
            {
                return String.Format("({0})^{1}", this.Name, this.Degree);
            }
            return String.Format("{0}^{1}", this.Name, this.Degree);
        }

        public override string ToMathML()
        {
            string s = this.ToMathMLShort();

            Power p = new Power(this.Degree);

            return String.Format("<math xmlns='http://www.w3.org/1998/Math/MathML'><mrow>{0}</mrow></math>", p.FormatStringML(s));
        }

        public override string ToMathMLShort()
        {
            StringBuilder sb = new StringBuilder();

            if (this.IsConstant())
            {
                return String.Format("<math><mrow><mi>{0}</mi></mrow></math>", this.ToDouble());
            }

            bool first = true;
            foreach (var vs in this.variables)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (vs.Constant > 0)
                    {
                        sb.AppendFormat("<mo>+</mo>");
                    }
                }
                sb.AppendFormat("<mrow>{0}</mrow>",vs.ToMathML());
            }
            
            return String.Format("<math><mrow>{0}</mrow></math>", this.ShowMLFunctions(sb.Length == 0 ? "0" : sb.ToString()));
            //return String.Format("<mrow>{0}</mrow>", this.ShowMLFunctions(sb.Length == 0 ? "0" : sb.ToString()));
        }

        public int VarsCount
        {
            get
            {
                return this.Variables.Length;
            }
        }

        public int VariablesMulsWithVariablesCount
        {
            get
            {
                return this.variables.Count(vs => vs.VarsCount != 0);
            }
        }

        public int VariablesMulsWithoutVariablesCount
        {
            get
            {
                return this.variables.Count(vs => vs.Constant != 0 && vs.VarsCount == 0);
            }
        }

        public string[] Variables
        {
            get
            {
                List<string> vars = new List<string>();
                this.variables.ForEach(delegate(VariablesMulriplication vs)
                {
                    if (vs.Constant != 0)
                    {
                        foreach (string var in vs.Variables)
                        {
                            if (!vars.Contains(var))
                                vars.Add(var);
                        }
                    }
                });
                return vars.ToArray();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Function)
            {
                Function f = obj as Function;
                foreach (var vs in this.variables)
                {
                    f -= vs;
                }
                return f.VarsCount == 0 && f.ToDouble() == 0;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Matrix GesseMatrix()
        {
            string[] vars = this.Variables;
            Matrix m = new Matrix(vars.Length, vars.Length);
            for (int i = 0; i < vars.Length; i++)
            {
                for (int j = 0; j < vars.Length; j++)
                {
                    m[i, j] = new FunctionMatrixElement(new Function(this.Derivative(vars[i]).Derivative(vars[j])));
                }
            }
            return m;
        }

        public override string Name
        {
            get
            {
                if (this.variables.Count == 0)
                    return "0";
                if (this.VarsCount == 0)
                    return this.ToDouble().ToString();

                StringBuilder sb = new StringBuilder();

                bool first = true;

                //this.variables.Sort(new Comparison<VariablesMulriplication>((v1, v2) => v1.

                foreach (var vars in this.variables)
                {
                    double constant = vars.Constant;
                    if (constant != 0 || this.variables.Count == 1)
                    {
                        if (!first && constant > 0)
                            sb.Append(" + ");
                        //if (constant != 1 || vars.VarsCount == 0)
                        //    sb.AppendFormat("{0}{1}", constant < 0 ? " - " : "", Math.Abs(constant));
                        if (constant != 0)
                            sb.Append(vars.ToString());
                        first = false;
                    }
                }

                return this.ShowFunctions(sb.ToString());
            }
        }

        public override bool IsConstant()
        {
            return this.Variables.Length == 0;
        }

        public override VariablesMulriplication Derivative()
        {
            if (Variables.Length == 0)
            {
                return new VariablesMulriplication(0);
            }
            var variable = this.Variables[0];
            return this.Derivative(variable);
        }

        public override bool HasVariable(string name)
        {
            return this.Variables.Contains(name);
        }

        public void CopyTo(Function func)
        {
            func.variables = new List<VariablesMulriplication>();
            this.variables.ForEach(vs => func.variables.Add(vs));
            func.Degree = this.Degree;
        }

        public Function OpenAllBrackets()
        {
            Function f = new Function();

#warning Сделать что-то такое

            return f;
        }

        public bool IsNeedBrackets()
        {
            if (Math.Abs(this.Degree) != 1)
                return false;
            return this.IsNeedBracketsForPower();
        }

        public bool IsNeedBracketsForPower()
        {
            int count = 0;
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].Constant != 0)
                    count++;

                if (count > 1)
                    return true;

                if (this.variables[i].Count > 1)
                    return true;
            }

            if (this. IsVariableMultiplication())
            {
                var temp = this.ToVariableMultiplication();
                if (temp.IsFunction())
                {
                    return temp.ToFunction().IsNeedBracketsForPower();
                }
            }
            return false;
        }

        public override bool IsVariableMultiplication()
        {
            if (this.Degree != 1)
                return false;
            int count = 0;
            foreach (var vs in this.variables)
            {
                if (vs.Constant != 0 && vs.ToString() != "0")
                {
                    count++;
                }
                if (count > 1)
                    return false;
            }
            return true;
        }

        public override VariablesMulriplication ToVariableMultiplication()
        {
            if (this.IsVariableMultiplication())
            {
                var v = this.variables.Find(vs => vs.Constant != 0 && vs.ToString() != "0");
                if (v != null)
                {
                    return v;
                }
                else
                {
                    return new VariablesMulriplication(0);
                }
            }
            return new VariablesMulriplication(this);
        }
    }
}
