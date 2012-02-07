using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public interface IMatrixElement
    {
        IMatrixElement One();

        IMatrixElement Mul(IMatrixElement e);

        IMatrixElement Add(IMatrixElement e);

        IMatrixElement Sub(IMatrixElement e);

        IMatrixElement Div(IMatrixElement e);

        IMatrixElement Clone();

        IMatrixElement Inverse();

        IMatrixElement Zero();

        IMatrixElement Pow(double power);

        IMatrixElement Sqrt();

        double ToDouble();

        string ToMathML();
    }
}
