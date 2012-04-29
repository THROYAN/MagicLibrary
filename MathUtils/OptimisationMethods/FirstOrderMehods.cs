using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils;
using MagicLibrary.MathUtils.Functions;

namespace MagicLibrary.MathUtils.OptimisationMethods
{
    public static class FirstOrderMehods
    {
        static public Tuple<Matrix, string> GradientDescent(Function f, Matrix x0, double e1, double e2, int M, double t0)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";
            StringBuilder sb = new StringBuilder("Метод градиентного спуска\n");
            int k = 0;
            double t = t0;

            sb.AppendFormat("Функция f = {0}\n", f);

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);

            for (int i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }


            Matrix gradF = f.Gradient();
            sb.AppendFormat("Шаг 1. Задаём: x0 = {0}, e1 = {1}, e2 = {2}, M = {3}\n", x0.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine), e1, e2, M);
            sb.AppendFormat("       Градиент функции: ▼f = {0}\n", gradF.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("Шаг 2. Положим: k = {0}\n", k);

            List<Matrix> gradFs = new List<Matrix>();
            List<Matrix> x = new List<Matrix>();

            x.Add(x0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------{0} иттерация-------------------\n", k);
                gradFs.Add(gradF.SetVariablesValuesWithFixedOrder(x[k], varsNames));
                sb.AppendFormat("Шаг 3. Градиент: ▼f(x{0}) = {1}\n", k, gradFs[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                double normF = gradFs[k].Norm();
                sb.AppendFormat("Шаг 4. 1-ый критерий окончания: || ▼f(x{0}) || = {1} {2} e1 = {3}\n", k, normF, normF < e1 ? " < " : " >= ", e1);

                if (normF < e1)
                {
                    answer = x[k];
                    sb.AppendFormat("1-ый критерий выполнел, следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                sb.AppendFormat("Шаг 5. Проверяем k >= M: {0} {1} {2}\n", k, k < M ? "<" : ">=", M);
                if (k >= M)
                {
                    answer = x[k];
                    sb.AppendFormat("Следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                sb.AppendFormat("Шаг 6. Задаем шаг t{0} = {1}\n", k, t);

                double dF = 0;
                bool flag = false;

                do
                {
                    flag = false;
                    x.Add(x[k] - new FunctionMatrixElement(t) * gradFs[k]);
                    sb.AppendFormat("Шаг 7. x{0} = {1}\n", k + 1, x[k + 1].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    Function f1 = new Function(f.SetVariablesValuesWithFixedOrder(x[k + 1], varsNames)), f2 = new Function(f.SetVariablesValuesWithFixedOrder(x[k], varsNames));
                    dF = f1.ToDouble() - f2.ToDouble();
                    sb.AppendFormat("Шаг 8. f(x{0}) - f(x{1}) = {3} - {4} = {2}", k + 1, k, dF, f1, f2);
                    if (dF >= 0 && t >= Double.Epsilon)
                    {
                        t /= 2;
                        sb.AppendFormat(" >= 0 =>\n\t положим t{0} = {1} и возвращаемся на 7 шаг\n", k, t);
                        x.RemoveAt(k + 1);
                        flag = true;
                    }
                    else
                    {
                        sb.AppendFormat(" < 0 => переходим к 9 шагу.\n");
                    }
                }
                while (flag);
                if (k == 0)
                {
                    k++;
                    sb.AppendFormat("Шаг 9. Т.к. это первая иттерация, то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    continue;
                }

                double normX1 = (x[k + 1] - x[k]).Norm(), normX2 = (x[k] - x[k - 1]).Norm();
                double normF1 = Math.Abs((f.SetVariablesValuesWithFixedOrder(x[k + 1], varsNames) - f.SetVariablesValuesWithFixedOrder(x[k], varsNames)).ToDouble()),
                        normF2 = Math.Abs((f.SetVariablesValuesWithFixedOrder(x[k], varsNames) - f.SetVariablesValuesWithFixedOrder(x[k - 1], varsNames)).ToDouble());

                sb.AppendFormat("Шаг 9. ||x{0} - x{1}|| = {2} {3} {4}\n       ||f(x{0}) - f(x{1})|| = {5} {6} {4}\n",
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
                    sb.AppendFormat("Т.к. хотя бы одно условие не выполнено (не <), то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    k++;
                    continue;
                }
            }

            return new Tuple<Matrix, string>(answer, sb.ToString());
        }

        static public Tuple<Matrix, string> SteepesGradientDescent(Function f, Matrix x0, double e1, double e2, int M)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";
            StringBuilder sb = new StringBuilder("Метод наискорейшего градиентного спуска\n");

            sb.AppendFormat("Функция f = {0}\n", f);

            int k = 0;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);

            for (int i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            Matrix gradF = f.Gradient();
            sb.AppendFormat("Шаг 1. Задаём: x0 = {0}, e1 = {1}, e2 = {2}, M = {3}\n", x0.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine), e1, e2, M);
            sb.AppendFormat("       Градиент функции: ▼f = {0}\n", gradF.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("Шаг 2. Положим: k = {0}\n", k);

            List<Matrix> gradFs = new List<Matrix>();
            List<Matrix> x = new List<Matrix>();

            x.Add(x0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------{0} иттерация-------------------\n", k);
                gradFs.Add(gradF.SetVariablesValuesWithFixedOrder(x[k], varsNames));
                sb.AppendFormat("Шаг 3. Градиент: ▼f(x{0}) = {1}\n", k, gradFs[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                double normF = gradFs[k].Norm();
                sb.AppendFormat("Шаг 4. 1-ый критерий окончания: || ▼f(x{0}) || = {1} {2} e1 = {3}\n", k, normF, normF < e1 ? " < " : " >= ", e1);

                if (normF < e1)
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

                Matrix tf = x[k] - new FunctionMatrixElement(1, "t#") * gradFs[k];
                Function fi = new Function(f.SetVariablesValuesWithFixedOrder(tf, varsNames));

                sb.AppendFormat("x{0} - t{0}*▼f(x{0}) = {1}\n", k, tf.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                sb.AppendFormat("f(x{0} - t{0}*▼f(x{0})) = {1}\n", k, fi);

                Equation et = new Function(fi.Derivative("t#")).EquationSolutionByVariable("t#", new Function(0));

                sb.AppendLine(et.ToString());

                double t = et.RightPart.ToDouble();

                sb.AppendFormat("Шаг 6. Задаем шаг t{0} = {1}\n", k, t);

                x.Add(x[k] - new FunctionMatrixElement(t) * gradFs[k]);
                sb.AppendFormat("Шаг 7. x{0} = {1}\n", k + 1, x[k + 1].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                if (k == 0)
                {
                    k++;
                    sb.AppendFormat("Шаг 8. Т.к. это первая иттерация, то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    continue;
                }

                double normX1 = (x[k + 1] - x[k]).Norm(), normX2 = (x[k] - x[k - 1]).Norm();
                double normF1 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[k + 1], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[k], varsNames)).ToDouble()),
                        normF2 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[k], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[k - 1], varsNames)).ToDouble());

                sb.AppendFormat("Шаг 8. ||x{0} - x{1}|| = {2} {3} {4}\n       ||f(x{0}) - f(x{1})|| = {5} {6} {4}\n",
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

        static public Tuple<Matrix, string> CoordinatewiseGradientDescent(Function f, Matrix x00, double e1, double e2, int M, double t0)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";
            StringBuilder sb = new StringBuilder("Метод покоординатного спуска\n");
            int k = 0;
            int j = 0;

            double[] t = new double[2] { t0, t0 };
            bool skip = false;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);
            int n = vars.Length;

            for (int i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            Matrix e = Matrix.IdentityMatrix(n, new FunctionMatrixElement());
            if (M % n != 0)
                throw new Exception("M должно быть кратно n - количеству переменных фукции");

            Matrix gradF = f.Gradient();
            sb.AppendFormat("Шаг 1. Задаём: x00 = {0}, e1 = {1}, e2 = {2}, M = {3}, n = {4}\n", x00.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine), e1, e2, M, n);
            sb.AppendFormat("       Градиент функции: ▼f = {0}\n", gradF.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("Шаг 2. Положим: j = {0}\n", j);

            List<Matrix[]> gradFs = new List<Matrix[]>();
            List<Matrix[]> x = new List<Matrix[]>();

            Matrix[] x0 = new Matrix[n + 1];
            x0[0] = x00;
            x.Add(x0);
            gradFs.Add(new Matrix[n + 1]);

            Matrix answer = x00;
            while (true)
            {
                while (true)
                {
                    sb.AppendFormat("---------------------------{0} иттерация-------------------\n", j);
                    if (!skip)
                    {
                        sb.AppendFormat("Шаг 3. Проверяем условие j >= M: j = {0} {1} {2}", j, j >= M ? ">=" : "<", M);

                        if (j >= M)
                        {
                            answer = x[j][k];
                            sb.AppendFormat("\nСледовательно расчёт окончен.\n\tОтвет: x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                            break;
                            // Это выход только из вложенного вайла
                            // Там внизу еще выход из внешнего
                        }
                        else
                        {
                            sb.AppendFormat(" => переходим к 4 шагу.\n");
                        }
                        k = 0;
                        sb.AppendFormat("Шаг 4. Зададим k = {0}\n", k);
                    }
                    skip = false;

                    sb.AppendFormat("Шаг 5. Проверяем условие k <= n - 1: k = {0} {1} {2}", k, k <= n - 1 ? "<=" : ">", n - 1);
                    if (k <= n - 1)
                    {
                        sb.AppendFormat(" => переходим к 6 шагу.\n");
                        break;
                    }
                    else
                    {
                        x.Add(new Matrix[n + 1]);
                        gradFs.Add(new Matrix[n + 1]);
                        x[j + 1][0] = x[j][n];
                        sb.AppendFormat("\nСледовательно положим j = j + 1 = {0} и x[{0}][0] = x[{1}][{2}] = {3}\n\tи перейдём на 3 шаг.\n", j + 1, j, n, x[j][n].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                        j++;
                        k = 0;
                    }
                }
                // Выход из внешнего вайла
                if (j >= M)
                {
                    break;
                }

                gradFs[j][k] = gradF.SetVariablesValuesWithFixedOrder(x[j][k], varsNames);
                sb.AppendFormat("Шаг 6. Градиент: ▼f(x[{0}][{1}]) = {2}\n", j, k, gradFs[j][k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                double normF = gradFs[j][k].Norm();
                sb.AppendFormat("Шаг 7. Проверяем критерий окончания: || ▼f(x[{0}][{1}]) || = {2} {3} e1 = {4}", j, k, normF, normF < e1 ? " < " : " >= ", e1);

                if (normF < e1)
                {
                    answer = x[j][k];
                    sb.AppendFormat("\nКритерий выполнен, следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                else
                {
                    sb.AppendFormat(" => переходим к следующему шагу.\n");
                }

                sb.AppendFormat("Шаг 8. Задаем шаг t{0} = {1}\n", k, t[k]);

                double dF = 0;
                bool flag = false;

                do
                {
                    flag = false;
                    x[j][k + 1] = x[j][k] - new FunctionMatrixElement(new Function(f.Derivative(vars[k])).SetVariablesValuesWithFixedOrder(x[j][k], varsNames) * new Function(t[k])) * new Matrix(e.Col(k));

                    sb.AppendFormat("Шаг 9. x[{0}][{1}] = {2}\n", j, k + 1, x[j][k + 1].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                    Function f1 = new Function(f.SetVariablesValuesWithFixedOrder(x[j][k + 1], varsNames)), f2 = new Function(f.SetVariablesValuesWithFixedOrder(x[j][k], varsNames));
                    dF = f1.ToDouble() - f2.ToDouble();

                    sb.AppendFormat("Шаг 10. f(x[{0}][{1}]) - f(x[{0}][{2}]) = {4} - {5} = {3}", j, k + 1, k, dF, f1, f2);
                    if (dF >= 0 && t[k] >= Double.Epsilon)
                    {
                        t[k] /= 2;
                        sb.AppendFormat(" >= 0 =>\n\t положим t{0} = {1} и возвращаемся на 9 шаг\n", k, t[k]);
                        flag = true;
                    }
                    else
                    {
                        sb.AppendFormat(" < 0 => переходим к 11 шагу.\n");
                    }
                }
                while (flag);

                if (j == 0)
                {
                    k++;
                    skip = true;
                    sb.AppendFormat("Шаг 11. Т.к. это первая иттерация, то увеличиваем k и возвращаемся к 5 шагу.\n\tk = {0}\n", k);
                    continue;
                }

                double normX1 = (x[j][k + 1] - x[j][k]).Norm(), normX2 = (x[j - 1][k + 1] - x[j - 1][k]).Norm();
                double normF1 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[j][k + 1], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[j][k], varsNames)).ToDouble()),
                        normF2 = Math.Abs((new Function(f.SetVariablesValuesWithFixedOrder(x[j - 1][k + 1], varsNames)) - f.SetVariablesValuesWithFixedOrder(x[j - 1][k], varsNames)).ToDouble());

                sb.AppendFormat("Шаг 11. ||x[{0}][{1}] - x[{0}][{2}]|| = {3} {4} {5}\n       ||f(x[{0}][{1}]) - f(x[{0}][{2}])|| = {6} {7} {5}\n",
                    j, k + 1, k, normX1, normX1 < e2 ? "<" : ">", e2, normF1, normF1 < e2 ? "<" : ">");
                sb.AppendFormat("        ||x[{0}][{1}] - x[{0}][{2}]|| = {3} {4} {5}\n       ||f(x[{0}][{1}]) - f(x[{0}][{2}])|| = {6} {7} {5}\n",
                    j - 1, k + 1, k, normX2, normX2 < e2 ? "<" : ">", e2, normF2, normF2 < e2 ? "<" : ">");

                if (normX1 < e2 && normX2 < e2 && normF1 < e2 && normF2 < e2)
                {
                    answer = x[j][k + 1];
                    sb.AppendFormat("Следовательно x* = {0}", answer.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    break;
                }
                else
                {
                    k++;
                    skip = true;
                    sb.AppendFormat("Т.к. хотя бы одно условие не выполнено (не <), то увеличиваем k и возвращаемся к 5 шагу.\n\tk = {0}\n", k);
                    continue;
                }
            }

            return new Tuple<Matrix, string>(answer, sb.ToString());
        }

        static public Tuple<Matrix, string> FletcherReevesMethod(Function f, Matrix x0, double e1, double e2, int M)
        {
            string matrixStartLine = "[", matrixElementsSeparator = ", ", matrixEndLine = "]";
            StringBuilder sb = new StringBuilder("Метод Флетчера-Ривза\n");
            int k = 0;

            string[] vars = f.Variables;
            Matrix varsNames = new Matrix(vars.Length);
            int n = vars.Length;

            for (int i = 0; i < vars.Length; i++)
            {
                varsNames[i] = new FunctionMatrixElement(1, vars[i]);
            }

            Matrix gradF = f.Gradient();
            sb.AppendFormat("Шаг 1. Задаём: x0 = {0}, e1 = {1}, e2 = {2}, M = {3}\n", x0.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine), e1, e2, M);
            sb.AppendFormat("       Градиент функции: ▼f = {0}\n", gradF.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
            sb.AppendFormat("Шаг 2. Положим: k = {0}\n", k);

            List<Matrix> gradFs = new List<Matrix>();
            List<Matrix> x = new List<Matrix>();
            List<Matrix> d = new List<Matrix>();
            double beta = 0;

            x.Add(x0);
            Matrix answer = x0;
            while (true)
            {
                sb.AppendFormat("---------------------------{0} иттерация-------------------\n", k);
                gradFs.Add(gradF.SetVariablesValuesWithFixedOrder(x[k], varsNames));
                sb.AppendFormat("Шаг 3. Градиент: ▼f(x{0}) = {1}\n", k, gradFs[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                double normF = gradFs[k].Norm();
                sb.AppendFormat("Шаг 4. 1-ый критерий окончания: || ▼f(x{0}) || = {1} {2} e1 = {3}\n", k, normF, normF < e1 ? " < " : " >= ", e1);

                if (normF < e1)
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
                else
                {
                    if (k == 0)
                    {
                        d.Add(new FunctionMatrixElement(-1) * gradFs[0]);
                        sb.AppendFormat("Т.к. k = 0, переходим к 6 шагу, а потом сразу к 9.\nШаг 6. d[0] = -▼f(x[0]) = {0}\n", d[0].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    }
                    else
                    {
                        //beta = Math.Pow(gradFs[k].Norm(), 2) / Math.Pow(gradFs[k - 1].Norm(), 2);
                        //beta = k % n == 0 ? 0 : Matrix.CosBetweenVectors(gradFs[k], gradFs[k] - gradFs[k - 1]).ToDouble()
                        //                            / Math.Pow(gradFs[k - 1].Norm(), 2);
                        beta = k % n == 0 ? 0 : Matrix.VectorsMultiplication(gradFs[k], gradFs[k] - gradFs[k - 1]).ToDouble()
                                                    / Math.Pow(gradFs[k - 1].Norm(), 2);
                        sb.AppendFormat("Т.к. k >= 1, переходим на 7 шаг.\nШаг 7. Beta[{0}] = {1}\n", k - 1, beta);
                        d.Add(new FunctionMatrixElement(-1) * gradFs[k] + new FunctionMatrixElement(beta) * d[k - 1]);
                        sb.AppendFormat("Шаг 8. d[{0}] = {1}\n", k, d[k].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));
                    }
                }

                sb.AppendFormat("Шаг 9. Находим t[{0}]:\n", k);

                Matrix tf = x[k] + new FunctionMatrixElement(1, "t#") * d[k];
                Function fi = new Function(f.SetVariablesValuesWithFixedOrder(tf, varsNames));

                sb.AppendFormat("        x[{0}] + t[{0}]*d[{0}] = {1}\n", k, tf.Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                sb.AppendFormat("        f(x[{0}] + t[{0}]*d[{0}] = {1}\n", k, fi);

                Equation et = new Function(fi.Derivative("t#")).EquationSolutionByVariable("t#", new Function(0));

                sb.AppendLine(et.ToString());

                double t = et.RightPart.ToDouble();

                sb.AppendFormat("       t[{0}] = {1}\n", k, t);

                x.Add(x[k] + new FunctionMatrixElement(t) * d[k]);
                sb.AppendFormat("Шаг 10. x{0} = {1}\n", k + 1, x[k + 1].Transposition().ToString(matrixStartLine, matrixElementsSeparator, matrixEndLine));

                if (k == 0)
                {
                    k++;
                    sb.AppendFormat("Шаг 11. Т.к. это первая иттерация, то увеличиваем k и возвращаемся к 3 шагу.\n\tk = {0}\n", k);
                    continue;
                }

                double normX1 = (x[k + 1] - x[k]).Norm(), normX2 = (x[k] - x[k - 1]).Norm();
                double normF1 = Math.Abs((f.SetVariablesValuesWithFixedOrder(x[k + 1], varsNames) - f.SetVariablesValuesWithFixedOrder(x[k], varsNames)).ToDouble()),
                        normF2 = Math.Abs((f.SetVariablesValuesWithFixedOrder(x[k], varsNames) - f.SetVariablesValuesWithFixedOrder(x[k - 1], varsNames)).ToDouble());

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
