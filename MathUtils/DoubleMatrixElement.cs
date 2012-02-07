using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    [Serializable]
    public class DoubleMatrixElement : IMatrixElement
    {
        public double D { get; set; }

        public DoubleMatrixElement()
        {
            D = 0;
        }

        public DoubleMatrixElement(double d)
        {
            D = d;
        }

        public static DoubleMatrixElement operator *(DoubleMatrixElement e1, DoubleMatrixElement e2)
        {
            return new DoubleMatrixElement(e1.D * e2.D);
        }

        public static DoubleMatrixElement operator +(DoubleMatrixElement e1, DoubleMatrixElement e2)
        {
            return new DoubleMatrixElement(e1.D + e2.D);
        }

        public IMatrixElement One()
        {
            return new DoubleMatrixElement(1);
        }

        public IMatrixElement Mul(IMatrixElement e)
        {
            DoubleMatrixElement e1 = e as DoubleMatrixElement;
            return new DoubleMatrixElement(D * e1.D);
        }

        public IMatrixElement Add(IMatrixElement e)
        {
            DoubleMatrixElement e1 = e as DoubleMatrixElement;
            return new DoubleMatrixElement(D + e1.D);
        }

        public IMatrixElement Sub(IMatrixElement e)
        {
            return new DoubleMatrixElement(D - (e as DoubleMatrixElement).D);
        }

        public override string ToString()
        {
            return D.ToString();
        }

        public IMatrixElement Clone()
        {
            return new DoubleMatrixElement(D);
        }

        public IMatrixElement Inverse()
        {
            return new DoubleMatrixElement(-D);
        }

        public IMatrixElement Zero()
        {
            return new DoubleMatrixElement();
        }


        public IMatrixElement Pow(double power)
        {
            return new DoubleMatrixElement(Math.Pow(this.D, power));
        }

        public double ToDouble()
        {
            return this.D;
        }

        public IMatrixElement Div(IMatrixElement e)
        {
            return new DoubleMatrixElement(this.D / (e as DoubleMatrixElement).D);
        }


        public IMatrixElement Sqrt()
        {
            return this.Pow(0.5);
        }


        public string ToMathML()
        {
            return String.Format("<mi>{0}</mi>", this.D);
        }
    }
}
