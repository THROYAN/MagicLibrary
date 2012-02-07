using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MagicLibrary.MathUtils
{
    [Serializable]
    public class Matrix
    {
        private Matrix _catchedReverse { get; set; }
        private bool _matrixChanged = true;
        /// <summary>
        /// Двумерный массив - элементы матрицы
        /// </summary>
        private IMatrixElement[,] _matrix;
        /// <summary>
        /// Элемент матрицы
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="col">Столбец</param>
        /// <returns></returns>
        public IMatrixElement this[int row, int col = 0]
        {
            get
            {
                return _matrix[row, col];
            }
            set
            {
                _matrix[row, col] = value;
                this._matrixChanged = true;
            }
        }
        /// <summary>
        /// Инициализация матрицы с помощью двумерно массива вещественных чисел
        /// </summary>
        /// <param name="matrix">Массив элементов матрицы</param>
        public Matrix(IMatrixElement[,] matrix)
        {
            _matrix = matrix.Clone() as IMatrixElement[,];
            this._matrixChanged = true;
        }

        public Matrix(IMatrixElement[] vector)
        {
            _matrix = new IMatrixElement[vector.Length, 1];
            for (int i = 0; i < vector.Length; i++)
                _matrix[i, 0] = vector[i].Clone();
            this._matrixChanged = true;
        }

        public Matrix(double[,] matrix)
        {
            _matrix = new DoubleMatrixElement[matrix.GetLength(0),matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(1); i++)
                for (int j = 0; j < matrix.GetLength(0); j++)
                    _matrix[j,i] = new DoubleMatrixElement(matrix[j, i]);
            this._matrixChanged = true;
        }

        public Matrix(double[] vector)
        {
            _matrix = new DoubleMatrixElement[vector.Length, 1];
            for (int i = 0; i < vector.Length; i++)
                _matrix[i, 0] = new DoubleMatrixElement(vector[i]);
            this._matrixChanged = true;
        }

        /// <summary>
        /// Создание нулевой матрицы заданных размеров
        /// </summary>
        /// <param name="rows">Количество строк</param>
        /// <param name="cols">Количество столбцов</param>
        public Matrix(int rows, int cols = 1)
        {
            _matrix = new IMatrixElement[rows, cols];
            this._matrixChanged = true;
            //for (int i = 0; i < cols; i++)
            //    for (int j = 0; j < rows; j++)
            //        _matrix[j, i] = element;
        }
        public Matrix(int rows, int cols,IMatrixElement element)
        {
            _matrix = new IMatrixElement[rows, cols];
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < rows; j++)
                    _matrix[j, i] = element;
            this._matrixChanged = true;
        }
        /// <summary>
        /// Создание матрицы, представляющей 2D вектор с дополнительным элементом - единицей,
        /// необходимой для применения к вектору простых преобразований
        /// </summary>
        /// <param name="p">Координаты вектора</param>
        public Matrix(PointF p)
        {
            _matrix = new DoubleMatrixElement[3, 1]{
                                            {new DoubleMatrixElement(p.X)},
                                            {new DoubleMatrixElement(p.Y)},
                                            {new DoubleMatrixElement(1)}
                                        };
            this._matrixChanged = true;
        }
        /// <summary>
        /// Копирование матрицы
        /// </summary>
        /// <param name="m">Матрица</param>
        public Matrix(Matrix m)
        {
            _matrix = new IMatrixElement[m.Rows, m.Cols];
            for (int i = 0; i < m.Cols; i++)
            {
                for (int j = 0; j < m.Rows; j++)
                {
                    _matrix[j, i] = m[j, i].Clone();
                }
            }
            this._matrixChanged = true;
        }
        /// <summary>
        /// Количесто строк
        /// </summary>
        public int Rows
        {
            get
            {
                return _matrix.GetLength(0);
            }
        }
        /// <summary>
        /// Количество столбцов
        /// </summary>
        public int Cols
        {
            get
            {
                return _matrix.GetLength(1);
            }
        }

        /// <summary>
        /// Elements of the matrix col
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMatrixElement[] Col(int index)
        {
            List<IMatrixElement> l = new List<IMatrixElement>();
            for (int i = 0; i < this.Rows; i++)
            {
                l.Add(this[i, index]);
            }
            return l.ToArray();
        }

        /// <summary>
        /// Elements of the matrix row
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMatrixElement[] Row(int index)
        {
            List<IMatrixElement> l = new List<IMatrixElement>();
            for (int i = 0; i < this.Cols; i++)
            {
                l.Add(this[index, i]);
            }
            return l.ToArray();
        }

        /// <summary>
        /// Перемножение матриц.
        /// Необходимое условие - количество строк первой матрицы должно быть равно количеству стобцов второй.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix res = new Matrix(m1.Rows, m2.Cols, m1[0,0].Zero());
            if (m1.Cols != m2.Rows)
            //    return m1;
                throw new InvalidMatrixSizeException("Для перемножения матриц, количество столбцов первой матрицы должно быть равно количеству строк второй!");
            for (int i1 = 0; i1 < m1.Rows; i1++)
            {
                for (int j2 = 0; j2 < m2.Cols; j2++)
                {
                    for (int j1 = 0; j1 < m1.Cols; j1++)
                    {
                        res[i1, j2] = res[i1, j2].Add(m1[i1, j1].Mul(m2[j1, j2]));
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Сложение матриц одинаковых размеров.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows && m1.Cols != m2.Cols)
                //return m1;
                throw new InvalidMatrixSizeException("Для сложения матриц их размеры должны быть одинаковы!");
            Matrix res = new Matrix(m1);
            for (int i = 0; i < m1.Cols; i++)
            {
                for (int j = 0; j < m1.Rows; j++)
                {
                    res[j, i] = res[j, i].Add(m2[j, i]);
                }
            }
            return res;
        }
        /// <summary>
        /// Вычитание матриц одинаковых размеров.
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
                //return m1;
                throw new InvalidMatrixSizeException("Для вычитания матриц их размеры должны быть одинаковы!");
            Matrix res = new Matrix(m1.Rows, m1.Cols);
            for (int i = 0; i < m1.Cols; i++)
            {
                for (int j = 0; j < m1.Rows; j++)
                {
                    res[j, i] = m1[j, i].Sub(m2[j, i]);
                }
            }
            return res;
        }
        /// <summary>
        /// Умножение всех элементов матрицы на число
        /// </summary>
        /// <param name="d">Вещественное число</param>
        /// <param name="m">Матрица</param>
        /// <returns></returns>
        public static Matrix operator *(IMatrixElement e, Matrix m)
        {
            Matrix res = new Matrix(m);
            for (int i = 0; i < m.Cols; i++)
            {
                for (int j = 0; j < m.Rows; j++)
                {
                    res[j, i] = e.Mul(res[j, i]);
                }
            }
            return res;
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();

            int[] headersLengthes = new int[this.Cols];

            for (int col = 0; col < this.Cols; col++)
            {
                headersLengthes[col] = this.Col(col).Max(e => e.ToString().Length);
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    res.AppendFormat("| {0," + headersLengthes[j] + "} ",_matrix[i, j].ToString());
                }
                res.AppendLine("|");
            }
            return res.ToString();
        }

        /// <summary>
        /// Возвращает строковое представление матрицы с указанным разделителем элементов.
        /// Между строками ставится символ новой строки.
        /// </summary>
        /// <param name="elementsSeparator">Разделитель</param>
        /// <returns></returns>
        public string ToString(string elementsSeparator)
        {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    res.AppendFormat("{0}{1}",_matrix[i, j].ToString(),elementsSeparator);
                }
                res.AppendLine();
            }
            return res.ToString();
        }
        /// <summary>
        /// Возвращает строковое представление матрицы с указанным разделителем элементов и указанным началом и концом каждой строки.
        /// Символ новой строки по-умолчанию не ставится.
        /// </summary>
        /// <param name="startLine">Начало каждой строки</param>
        /// <param name="elementsSeparator">Разделитель элементов</param>
        /// <param name="endLine">Конец каждой строки</param>
        /// <returns></returns>
        public string ToString(string startLine, string elementsSeparator, string endLine)
        {
            StringBuilder res = new StringBuilder();

            int[] headersLengthes = new int[this.Cols];

            for (int col = 0; col < this.Cols; col++)
            {
                headersLengthes[col] = this.Col(col).Max(e => e.ToString().Length);
            }

            for (int i = 0; i < Rows; i++)
            {
                res.AppendFormat("{0}", startLine);
                for (int j = 0; j < Cols; j++)
                {
                    res.AppendFormat("{0}{1," + headersLengthes[j] + "}", j == 0 ? "" : elementsSeparator, _matrix[i, j].ToString());
                }
                res.AppendFormat("{0}", endLine);
            }
            return res.ToString();
        }
        /// <summary>
        /// Преобразование матрицы размеров 2х1 или 3х1 в точку, представляющей координаты вектора.
        /// </summary>
        /// <returns>Координаты вектора</returns>
        public PointF ToPointF()
        {
            if (Cols != 1 || (Rows != 2 && Rows != 3))
                throw new InvalidMatrixSizeException(String.Format("Невозможно преобразовать матрицу {0}x{1} в вектор!", Rows, Cols));
            return new PointF((float)(_matrix[0, 0] as DoubleMatrixElement).D, (float)(_matrix[1, 0] as DoubleMatrixElement).D);
        }
        /// <summary>
        /// Умножение матрицы на вектор.
        /// </summary>
        /// <param name="m">Матрица 3х3</param>
        /// <param name="P">Координаты вектора</param>
        /// <returns>Координаты вектора - результата перемножения.</returns>
        public static PointF operator *(Matrix m, PointF P)
        {
            if(m.Cols != 3 || m.Rows != 3)
                throw new InvalidMatrixSizeException(String.Format("Невозможно матрицу {0}x{1} умножить на вектор - необходима матрица 3х3!", m.Rows, m.Cols));
            Matrix mP = new Matrix(P);
            Matrix res = m * mP;
            return res.ToPointF();
        }
        ///// <summary>
        ///// Нельзя типа
        ///// </summary>
        ///// <param name="P"></param>
        ///// <param name="m"></param>
        ///// <returns></returns>
        //public static PointF operator *(PointF P, Matrix m)
        //{
        //    return m * P;
        //}
        /// <summary>
        /// Умножение матрицы на пачку векторов.
        /// </summary>
        /// <param name="m">Матрица 3х3</param>
        /// <param name="P">Массив координат векторов.</param>
        /// <returns></returns>
        public static PointF[] operator *(Matrix m,PointF[] P)
        {
            PointF[] P1 = new PointF[P.Length];
            for (int i = 0; i < P.Length; i++)
                P1[i] = m * P[i];
            return P1;
        }

        /// <summary>
        /// Умножение матрицы на прямоугольник
        /// </summary>
        /// <param name="m"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static PointF[] operator *(Matrix m, RectangleF r)
        {
            PointF[] ps = new PointF[5];

            ps[0] = m * (new PointF(r.Left, r.Top));
            ps[1] = m * (new PointF(r.Right, r.Top));
            ps[2] = m * (new PointF(r.Right, r.Bottom));
            ps[3] = m * (new PointF(r.Left, r.Bottom));
            ps[4] = m * (new PointF(r.Left, r.Top));

            return ps;
        }
        ///// <summary>
        ///// Тоже нельзя
        ///// </summary>
        ///// <param name="P"></param>
        ///// <param name="m"></param>
        ///// <returns></returns>
        //public static PointF[] operator *(PointF[] P, Matrix m)
        //{
        //    return m * P;
        //}
        /// <summary>
        /// Вычеркивание столбца матрицы. Если столбца с таким номером нет, то ничего не вычеркивается.
        /// </summary>
        /// <param name="col">Номер столбца</param>
        /// <returns>Матрица, полученная вычеркиванием столбца, количество её столбцов меньше на 1, чем исходной матрицы.</returns>
        public Matrix StrikeOutCol(int col)
        {
            if(col >= Cols)
                return this;
            Matrix res = new Matrix(Rows,Cols - 1);
            int i1 = 0;
            for (int i = 0; i < Cols; i++)
            {
                if(i != col)
                {
                    for (int j = 0; j < Rows; j++)
                    {
                        res[j, i1] = this[j, i].Clone();
                    }
                    i1++;
                }
            }
            return res;
        }
        /// <summary>
        /// Вычеркивание строки матрицы. Если строки с таким номером нет, то ничего не вычеркивается.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <returns>Матрица, полученная вычеркиванием строки, количество её строки меньше на 1, чем исходной матрицы.</returns>
        public Matrix StrikeOutRow(int row)
        {
            if (row >= Rows)
                return this;
            Matrix res = new Matrix(Rows - 1, Cols);
            for (int i = 0; i < Cols; i++)
            {
                int j1 = 0;
                for (int j = 0; j < Rows; j++)
                {
                    if(j !=row)
                        res[j1++, i] = this[j, i].Clone();
                }
            }
            return res;
        }
        /// <summary>
        /// Вычеркивание строки и стобца с заданными индексами (начиная с 0).
        /// </summary>
        /// <param name="row">Индекс строки.</param>
        /// <param name="col">Индекс столбца.</param>
        /// <returns></returns>
        public Matrix StrikeOut(int row, int col)
        {
            Matrix res = new Matrix(Rows - 1, Cols - 1);
            int i1 = 0;
            for (int i = 0; i < Cols; i++)
            {
                if (i  != col)
                {
                    int j1 = 0;
                    for (int j = 0; j < Rows; j++)
                    {
                        if (j != row)
                            res[j1++, i1] = this[j, i].Clone();
                    }
                    i1++;
                }
            }
            return res;
        }

        /// <summary>
        /// Вычеркивание строк и стобцов с заданными индексами (начиная с 0).
        /// </summary>
        /// <param name="rows">Индексы строк.</param>
        /// <param name="cols">Индексы столбцов.</param>
        /// <returns></returns>
        public Matrix StrikeOut(int[] rows, int[] cols)
        {
            Matrix res = new Matrix(Rows - rows.Length, Cols - cols.Length);
            int i1 = 0;
            for (int i = 0; i < Cols; i++)
            {
                if (!cols.Contains(i))
                {
                    int j1 = 0;
                    for (int j = 0; j < Rows; j++)
                    {
                        if (!rows.Contains(j))
                            res[j1++, i1] = this[j, i].Clone();
                    }
                    i1++;
                }
            }
            return res;
        }

        /// <summary>
        /// Вычеркивание некоторого количества строк и стобцов, начиная от заданных.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <param name="col">Номер столбца.</param>
        /// <param name="rowsCount">Количество строк.</param>
        /// <param name="colsCount">Количество столбцов.</param>
        /// <returns></returns>
        public Matrix StrikeOut(int row, int col, int rowsCount = 1, int colsCount = 1)
        {
            Matrix res = new Matrix(Rows - rowsCount, Cols - colsCount);
            int i1 = 0;
            for (int i = 0; i < Cols; i++)
            {
                if (!(i >= col && i < col + colsCount))
                {
                    int j1 = 0;
                    for (int j = 0; j < Rows; j++)
                    {
                        if (!(j >= row && j < row + rowsCount))
                            res[j1++, i1] = this[j, i].Clone();
                    }
                    i1++;
                }
            }
            return res;
        }

        /// <summary>
        /// Транспонирование матрицы.
        /// </summary>
        /// <returns></returns>
        public Matrix Transposition()
        {
            Matrix m = new Matrix(Cols, Rows);
            for (int i = 0; i < Cols; i++)
                for (int j = 0; j < Rows; j++)
                    m[i, j] = this[j, i].Clone();
            return m;
        }

        public static Matrix IdentityMatrix(int size)
        {
            Matrix m = new Matrix(size, size, new DoubleMatrixElement(0));
            for (int i = 0; i < size; i++)
                m[i, i] = new DoubleMatrixElement(1);
            return m;
        }
        /// <summary>
        /// Единичная матрица заданного размера.
        /// </summary>
        /// <param name="size">Размерность матрицы.</param>
        /// <returns></returns>
        public static Matrix IdentityMatrix(int size, IMatrixElement elementForGetType)
        {
            Matrix m = new Matrix(size, size,elementForGetType.Zero());
            for (int i = 0; i < size; i++)
                m[i, i] = elementForGetType.One();
            return m;
        }
        /// <summary>
        /// Определитель матрицы.
        /// Матрица должна быть квадратной, т.е. размера nxn.
        /// </summary>
        /// <returns></returns>
        public IMatrixElement Determinant()
        {
            if (Cols != Rows)
                throw new InvalidMatrixSizeException("Вычислить определитель можно только квадратой матрицы!");
            if (Cols == 1 && Rows == 1)
                return this[0, 0];
            IMatrixElement det = this[0,0].Zero();
            for (int i = 0; i < Cols; i++)
            {
                IMatrixElement one = this[0, i].One();
                det = det.Add((i%2==0?one:one.Inverse()).Mul(this[0, i]).Mul(StrikeOut(0, i).Determinant()));
            }
            return det;
        }

        public Matrix Reverse()
        {
            if (this._matrixChanged)
            {
                if (this.Cols == 1 && this.Rows == 1)
                {
                    return new Matrix(new IMatrixElement[,] { { this[0,0].Pow(-1) } });
                }
                this._catchedReverse = 
                // 1/det(A)
                    this.Determinant().Pow(-1) *
                    // * C*[T]
                        (this.AdjugateMatrix().Transposition());
            }
            return this._catchedReverse;
        }

        public Matrix AdjugateMatrix()
        {
            Matrix t = this.Transposition();

            Matrix m = new Matrix(t);
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Cols; j++)
                {
                    m[j, i] = t.Cofactor(i, j);
                }
            }
            return m;
        }

        public IMatrixElement Cofactor(int row, int col)
        {               // -1 ^ (i + j)
            return (this[row, col].One().Inverse().Pow(row + col))
                        // *    Mij
                        .Mul(this.Minor(row, col));
        }

        public IMatrixElement Minor(int row, int col)
        {
            return this.StrikeOut(row, col).Determinant();
        }

        public IMatrixElement Minor(int row, int col, int rowsCount = 1, int colsCount = 1)
        {
            return this.StrikeOut(row, col, rowsCount, colsCount).Determinant();
        }

        public IMatrixElement Minor(int[] rows, int[] cols)
        {
            return this.StrikeOut(rows, cols).Determinant();
        }

        public System.Drawing.Drawing2D.Matrix ToSystemMatrix()
        {
            return new System.Drawing.Drawing2D.Matrix(
                    (float)this[0, 0].ToDouble(),
                    (float)this[0, 1].ToDouble(),
                    (float)this[1, 0].ToDouble(),
                    (float)this[1, 1].ToDouble(),
                    (float)this[0, 2].ToDouble(),
                    (float)this[1, 2].ToDouble()
                );
        }

#warning Тут что-то не так, подозрительно как-то!
        public double Norm()
        {
            if (!this.IsVector())
            {
                throw new Exception("Vector must have only one col.");
            }
            double d = 0;
            for (int i = 0; i < this.Rows; i++)
            {
                d += Math.Pow(this[i].ToDouble(), 2);
            }
            return Math.Sqrt(d);
        }

        public bool IsVector()
        {
            return this.Cols == 1;// && (this.Rows == 2 || this.Rows == 3);
        }

        static public IMatrixElement CosBetweenVectors(Matrix v1, Matrix v2)
        {
            if (!v1.IsVector() || !v2.IsVector() || v1.Rows != v2.Rows)
            {
                throw new Exception("Неверный формат векторов");
            }
            //cos = (x1*x2 + y1*y2) / sqrt ((x1*x1 + y1*y1) * (x2*x2 + y2*y2))

            IMatrixElement numerator = v1[0].Mul(v2[0]),
                denomenator1 = v1[0].Pow(2), demomenator2 = v2[0].Pow(2);
            for (int i = 1; i < v1.Rows; i++)
            {
                numerator = numerator.Add(v1[i].Mul(v2[i]));
                denomenator1 = denomenator1.Add(v1[i].Pow(2));
                demomenator2 = demomenator2.Add(v2[i].Pow(2));
            }

            return numerator.Div( denomenator1.Mul(demomenator2).Sqrt() );

            //if (v1.Rows == 2)
            //{
            //    return (( v1[0].Mul(v2[0]) ).Add( v1[1].Mul(v2[1]) )).Div(
            //            (( v1[0].Pow(2).Add(v1[1].Pow(2)) ).Mul( ( v2[0].Pow(2).Add(v2[1].Pow(2)) ) )).Pow(0.5)
            //        );
            //}
            ////cos = (x1*x2 + y1*y2 + z1*z2) / sqrt ((x1^2 + y1^2 + z1^2) * (x2^2 + y2^2 + z2^2))
            //return (( v1[0].Mul(v2[0]) ).Add( v1[1].Mul(v2[1]) ).Add( v1[2].Mul(v2[2]) )).Div(
            //            ((v1[0].Pow(2).Add(v1[1].Pow(2).Add(v1[2].Pow(2)))).Mul((v2[0].Pow(2).Add(v2[1].Pow(2).Add(v2[2].Pow(2)))))).Pow(0.5)
            //        );
        }

        static public double AngleBetweenVectors(Matrix v1, Matrix v2)
        {
            return Math.Acos(Matrix.CosBetweenVectors(v1, v2).ToDouble());
        }

        public IMatrixElement VectorLength()
        {
            if (!this.IsVector())
            {
                throw new Exception("Неверный формат вектора");
            }
            // sqrt(x^2 + y^2 + ...)

            IMatrixElement e = this[0].Pow(2);
            for (int i = 1; i < this.Rows; i++)
            {
                e = e.Add(this[i].Pow(2));
            }
            return e.Sqrt();

            //if (this.Rows == 2)
            //{
            //    return (this[0].Pow(2).Add(this[1].Pow(2))).Pow(0.5);
            //}

            //return (this[0].Pow(2).Add(this[1].Pow(2)).Add(this[2].Pow(2))).Pow(0.5);
        }

        public static IMatrixElement VectorsMultiplication(Matrix v1, Matrix v2)
        {
            if (!v1.IsVector() || !v2.IsVector() || v1.Rows != v2.Rows)
            {
                throw new Exception("Неверный формат векторов");
            }
#warning Исправить!
            return v1.VectorLength().Mul(v2.VectorLength()).Mul(Matrix.CosBetweenVectors(v1, v2));
        }

        public Matrix SetVariablesValues(Matrix valuesVector)
        {
            if (!(this[0, 0] is FunctionMatrixElement))
                return new Matrix(this);
            Matrix m = new Matrix(this.Rows, this.Cols, new FunctionMatrixElement());

            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    m[i, j] = new FunctionMatrixElement((this[i, j] as FunctionMatrixElement).Function.SetVariablesValues(valuesVector));
                }
            }

            return m;
        }

        /// <summary>
        /// Каждый элемент матрицы - вектора значений подставляется в соответсвующую строку исходной матрицы.
        /// Не знаю зачем этот метод.
        /// </summary>
        /// <param name="valuesVector"></param>
        /// <returns></returns>
        public Matrix SetVariablesValuessByOne(Matrix valuesVector)
        {
            if (!(this[0, 0] is FunctionMatrixElement))
                return new Matrix(this);
            Matrix m = new Matrix(this.Rows, this.Cols, new FunctionMatrixElement());

            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    var temp = valuesVector.Row(i);
                    Matrix tempM = new Matrix(1, temp.Length);
                    int k = 0;
                    foreach (var item in temp)
	                {
		                tempM[0, k] = temp[k++];
	                }
                    m[i, j] = new FunctionMatrixElement((this[i, j] as FunctionMatrixElement).Function.SetVariablesValues(tempM));
                }
            }

            return m;
        }

        /// <summary>
        /// Устанавливаются значений переменых в таком порядке, в котором они заданы во втором параметре.
        /// Размеры матриц должны совпадать!
        /// Удобная функция для подставления значений в градиент, где некоторые переменных могут пропасть
        /// после взятия производной.
        /// </summary>
        /// <param name="valuesVector">Матрица значений для подставновке</param>
        /// <param name="variablesOrder">Матрица переменных, которые должны содержать только имя переменной</param>
        /// <returns></returns>
        public Matrix SetVariablesValuesWithFixedOrder(Matrix valuesVector, Matrix variablesOrder)
        {
            if (valuesVector.Rows != variablesOrder.Rows)
                throw new Exception("Вектора значений и переменных должны иметь одинаковое количество строк");

            if (!(this[0, 0] is FunctionMatrixElement))
                return new Matrix(this);

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

        public bool Silvestr()
        {
            if (this.Cols != this.Rows)
                throw new Exception("Matrix must have same cols and rows count");
            for (int i = 0; i < this.Rows; i++)
            {
                if (this.Minor(i + 1, i + 1, Rows - i - 1, Rows - i - 1).ToDouble() <= 0)
                    return false;
            }
            return true;
        }

        public string ToMathML()
        {
            StringBuilder sb = new StringBuilder("<math><mfenced open=\"(\" close=\")\"><mtable>");

            //sb.Append(this.Transposition().ToString("<mo>[<mi>","</mi><mo>,</mo><mi>","</mi><mo>]</mo>"));

            for (int i = 0; i < Rows; i++)
            {
                sb.Append("<mtr>");
                for (int j = 0; j < Cols; j++)
                {
                    sb.AppendFormat("<mtr><mn>{0}</mn></mtr>", _matrix[i, j].ToMathML());
                }
                sb.AppendFormat("</mtr>");
            }

            sb.Append("</mtable></mfenced></math>");
            return sb.ToString();
        }
    }
}
