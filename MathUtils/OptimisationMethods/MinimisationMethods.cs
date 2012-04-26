using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.MathFunctions;

namespace MagicLibrary.MathUtils.OptimisationMethods
{
    public class MinimisationMethods
    {
        static public Tuple<Matrix, string> PenaltyMethod(Function f, Equation[] q, Matrix x0, double e, double r0, double C)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";

            List<Function> qm = new List<Function>(), qp = new List<Function>();
            StringBuilder sb = new StringBuilder("<h1>Метод штрафных функций</h1></br>");

            foreach (var qi in q)
            {
                if (qi.Sign == "<=")
                {
                    qp.Add(qi.AllToLeft());
                }
                if (qi.Sign == "=")
                {
                    qm.Add(qi.AllToLeft());
                }
            }

            sb.AppendFormat("<b>Функция</b> f = {0}</br><b>Ограничения:</b></br>", f.ToMathML());
            int i = 0;
            qm.ForEach(qi => sb.AppendFormat("&nbsp&nbsp&nbsp&nbspq({0}) = {1} = 0</br>", i++, qi.ToMathML()));
            i = 0;
            qp.ForEach(qi => sb.AppendFormat("&nbsp&nbsp&nbsp&nbspq({0}) = {1} <= 0</br>", i++, qi.ToMathML()));

            int k = 0;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);

            for (i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            sb.AppendFormat("Шаг 1. Задаём: <math><mrow><msub><mi>x</mi><mn>0</mn></msub></mrow><mo>=</mo><mrow>{0}</mrow><mo></math>,штраф <math><msub><mi>r</mi><mn>0</mn></msub><mo>=</mo></math>{1}, C = {2}, e = {3}, k = {4}</br>", x0.ToMathML(), r0, C, e, k);
            
            List<Matrix> x = new List<Matrix>();
            List<double> r = new List<double>();
            Function Sm = new Function();
            Function Sp = new Function();
            qm.ForEach(qi => Sm += qi.Pow(2));
            qp.ForEach(qi => Sp += qi.AddFunction(new FunctionSlice()).Pow(2));
            Function P = new Variable("rk") / 2.0 * (Sm + Sp);

            x.Add(x0);
            r.Add(r0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------{0} иттерация-------------------\n", k);
                sb.AppendFormat("{0}\n", P);
                Function F = f + P.SetVariableValue("rk", r[k]);

                sb.AppendFormat("Шаг 2. Вспомогательная функция = {0}\n", F);

                Matrix minX = SecondOrderMethods.NuthonMethod(F, x[k], e, e, 5).Item1;

                sb.AppendFormat("Шаг 3. Минимум функции F:\n\tx[0] = x[{0}] = {1}\n\tX* = {2}\n",
                                        k,
                                        x[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine),
                                        minX.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine)
                );

                sb.AppendFormat("Шаг 4. Условие окончания: P(x*(r[{0}],r[{0}]) <=? e = {1}\n", k, e);
                double p = new Function(P.SetVariableValue("rk", r[k])).SetVariablesValuesWithFixedOrder(minX, varsNames).ToDouble();

                sb.AppendFormat("       {0} {1} {2}\n", p, p <= e ? "<=" : ">", e);
                if (p <= e)
                {
                    answer = minX;
                    sb.AppendFormat("Следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                else
                {
                    r.Add(C * r[k]);
                    x.Add(minX);
                    k++;
                }
            }

            return new Tuple<Matrix, string>(answer, sb.ToString());
        }

        static public Tuple<Matrix, string> BarrierMethod(Function f, Equation[] q, Matrix x0, double e, double r0, double C)
        {
            List<Function> qm = new List<Function>(), qp = new List<Function>();
            StringBuilder sb = new StringBuilder("<h1>Метод барьерных функций</h1></br>");

            foreach (var qi in q)
            {
                if (qi.Sign == "<=")
                {
                    qp.Add(qi.AllToLeft());
                }
                if (qi.Sign == "=")
                {
                    qm.Add(qi.AllToLeft());
                }
            }

            sb.AppendFormat("<b>Функция</b> f = {0}</br><b>Ограничения:</b></br>", f.ToMathML());
            int i = 0;
            qm.ForEach(qi => sb.AppendFormat("&nbsp&nbsp&nbsp&nbspq({0}) = {1} = 0</br>", i++, qi.ToMathML()));
            i = 0;
            qp.ForEach(qi => sb.AppendFormat("&nbsp&nbsp&nbsp&nbspq({0}) = {1} <= 0</br>", i++, qi.ToMathML()));

            int k = 0;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);

            for (i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            sb.AppendFormat("Шаг 1. Задаём: <math><mrow><msub><mi>x</mi><mn>0</mn></msub></mrow><mo>=</mo><mrow>{0}</mrow><mo></math>,штраф <math><msub><mi>r</mi><mn>0</mn></msub><mo>=</mo></math>{1}, C = {2}, e = {3}, k = {4}</br>", x0.ToMathML(), r0, C, e, k);

            List<Matrix> x = new List<Matrix>();
            List<double> r = new List<double>();
            Function Sq = new Function();
            foreach(var qi in q) {
                Sq += qi.AllToLeft().Pow(-1);
            }
                
            Function P = new Variable("rk") * Sq;

            x.Add(x0);
            r.Add(r0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------<b>{0} иттерация</b>-------------------</br>", k);
               
                Function F = f - P.SetVariableValue("rk", r[k]);
                
                sb.AppendFormat("Шаг 2. Вспомогательная функция = {0}</br>", F.ToMathML());
                //Function der = F.Derivative();

                //Matrix minX = SecondOrderMethods.NuthonMethod(F, x[k], e, e, 5).Item1;
                Matrix minX = MinimisationMethods.PenaltyMethod(F, q.ToArray(), x[k], e, r0, C).Item1;

                sb.AppendFormat("Шаг 3. Минимум функции F:</br>&nbsp&nbsp&nbsp&nbsp&nbsp<math><msub><mi>x</mi><mn>0</mn></msub><mo>=</mo><msub><mi>x</mi><mn>{0}</mn></msub><mo>=</mo></math>{1}</br>&nbsp&nbsp&nbsp&nbspX* = {2}</br>",
                                        k,
                                        x[k].ToMathML(),
                                        minX.ToMathML()
                );

                sb.AppendFormat("Шаг 4. Условие окончания: <math><mi>P</mi><mfenced><msup><mi>x</mi><mn>*</mn></msup><mfenced><msub><mi>r</mi><mn>{0}</mn></msub></mfenced><mo>,</mo><msub><mi>r</mi><mn>{0}</mn></msub></mfenced><mo><=?</mo><mi>e</mi><mo>=</mo><mi>{1}</mi></math></br>", k, e);
                double p = Math.Abs(new Function(P.SetVariableValue("rk", r[k])).SetVariablesValuesWithFixedOrder(minX, varsNames).ToDouble());

                sb.AppendFormat("&nbsp&nbsp&nbsp&nbsp{0} {1} {2}</br>", p, p <= e ? "<=" : ">", e);
                if (p <= e)
                {
                    answer = minX;
                    sb.AppendFormat("Следовательно x* = {0}", answer.ToMathML());
                    break;
                }
                else
                {
                    r.Add(r[k] / C);
                    x.Add(minX);
                    k++;
                }
            }

            return new Tuple<Matrix, string>(answer, sb.ToString());
        }
    }
}
