using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.OptimisationMethods
{
    public static class SecondOrderMethods
    {
        static public Tuple<Matrix, string> NuthonMethod(Function f, Matrix x0, double e1, double e2, int M)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";
            StringBuilder sb = new StringBuilder("Метод Ньютона\n");
            int k = 0;
            double t = 0;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);
            int n = vars.Length;

            for (int i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            Matrix gradF = f.Gradient();
            Matrix H = f.GesseMatrix();
            sb.AppendFormat("Шаг 1. Задаём: x0 = {0}, e1 = {1}, e2 = {2}, M = {3}\n", x0.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine), e1, e2, M);
            sb.AppendFormat("       Градиент функции: ▼f = {0}\n", gradF.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("       Матрица Гессе: H = {0}\n", H.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("Шаг 2. Положим: k = {0}\n", k);

            List<Matrix> gradFs = new List<Matrix>();
            List<Matrix> x = new List<Matrix>();
            List<Matrix> d = new List<Matrix>();

            x.Add(x0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------{0} иттерация-------------------\n", k);
                gradFs.Add(gradF.SetVariablesValuesWithFixedOrder(x[k], varsNames));
                sb.AppendFormat("Шаг 3. Градиент: ▼f(x[{0}]) = {1}\n", k, gradFs[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                double normF = gradFs[k].Norm();
                sb.AppendFormat("Шаг 4. 1-ый критерий окончания: || ▼f(x[{0}]) || = {1} {2} e1 = {3}\n", k, normF, normF <= e1 ? " <= " : " > ", e1);

                if (normF <= e1)
                {
                    answer = x[k];
                    sb.AppendFormat("1-ый критерий выполнен, следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                sb.AppendFormat("Шаг 5. Проверяем k >= M: {0} {1} {2}\n", k, k < M ? "<" : ">=", M);
                if (k >= M)
                {
                    answer = x[k];
                    sb.AppendFormat("Следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }

                Matrix Hk = H.SetVariablesValuesWithFixedOrder(x[k], varsNames);
                sb.AppendFormat("Шаг 6. H(x[{0}]):\n{1}\n", k, Hk.ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                sb.AppendFormat("Шаг 7. Обратная матрица матрицы H(x[{0}]):\n{1}\n", k, Hk.Reverse().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                bool flag = Hk.Reverse().Silvestr();
                sb.AppendFormat("Шаг 8. Проверяем условие H^-1[{0}] > 0: {1}", k, flag);

                if (flag)
                {
                    sb.AppendFormat(" => переходим к 9 шагу.\n");
                    d.Add(new FunctionMatrixElement(-1) * Hk.Reverse() * gradFs[k]);
                    sb.AppendFormat("Шаг 9. Находим d[{0}]:\n{1}\n", k, d[k].ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    t = 1;
                }
                else
                {
                    d.Add(new FunctionMatrixElement(-1) * gradFs[k]);
                    sb.AppendFormat(" => переходим к 10 шагу, положим d[{0}] = -▼f(x[{0}]):{1}\n", k, d[k].ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    Matrix tf = x[k] - new FunctionMatrixElement(1, "t#") * gradFs[k];
                    Function fi = new Function(f.SetVariablesValues(tf));

                    sb.AppendFormat("x{0} - t{0}*▼f(x{0}) = {1}\n", k, tf.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                    sb.AppendFormat("f(x{0} - t{0}*▼f(x{0})) = {1}\n", k, fi);

                    Equation et = new Function(fi.Derivative("t#")).EquationSolutionByVariable("t#", new Function(0));

                    sb.AppendLine(et.ToString());

                    t = et.RightPart.ToDouble();
                }
                
                x.Add(x[k] + new FunctionMatrixElement(t) * d[k]);
                sb.AppendFormat("Шаг 10. x{0} = {1}\n", k + 1, x[k + 1].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                if (k == 0)
                {
                    k++;
                    sb.AppendFormat("Шаг 11. Т.к. это первая иттерация, то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    continue;
                }

                double normX1 = (x[k + 1] - x[k]).Norm(), normX2 = (x[k] - x[k - 1]).Norm();
                double normF1 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[k + 1], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[k], varsNames)).ToDouble()),
                        normF2 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[k], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[k - 1], varsNames)).ToDouble());

                sb.AppendFormat("Шаг 11. ||x{0} - x{1}|| = {2} {3} {4}\n       ||f(x{0}) - f(x{1})|| = {5} {6} {4}\n",
                    k, k + 1, normX1, normX1 < e2 ? "<" : ">", e2, normF1, normF1 < e2 ? "<" : ">");
                sb.AppendFormat("       ||x{0} - x{1}|| = {2} {3} {4}\n       ||f(x{0}) - f(x{1})|| = {5} {6} {4}\n",
                    k - 1, k, normX2, normX2 < e2 ? "<" : ">", e2, normF2, normF2 < e2 ? "<" : ">");

                if (normX1 < e2 && normX2 < e2 && normF1 < e2 && normF2 < e2)
                {
                    answer = x[k + 1];
                    sb.AppendFormat("Следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                else
                {
                    k++;
                    sb.AppendFormat("Т.к. хотя бы одно условие не выполнено (не <), то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    continue;
                }
            }

            return new Tuple<Matrix, string>(answer, sb.ToString());
        }
    }
}
