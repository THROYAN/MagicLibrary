using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils
{
    public class FunctionMatrixElement : IMatrixElement
    {
        public Function Function { get; set; }

        public FunctionMatrixElement(double constant = 0, string varName = "", double degree = 1)
        {
            this.Function = new Function(constant, varName, degree);
        }

        public FunctionMatrixElement(Function f)
        {
            this.Function = f;
        }

        public FunctionMatrixElement(FunctionElement fe)
        {
            this.Function = new Function(fe);
        }

        public IMatrixElement One()
        {
            return new FunctionMatrixElement(1);
        }

        public IMatrixElement Mul(IMatrixElement e)
        {
            FunctionMatrixElement eq = e as FunctionMatrixElement;

            return new FunctionMatrixElement(eq.Function * this.Function);
        }

        public IMatrixElement Add(IMatrixElement e)
        {
            return new FunctionMatrixElement(this.Function + (e as FunctionMatrixElement).Function);
        }

        public IMatrixElement Sub(IMatrixElement e)
        {
            return new FunctionMatrixElement(this.Function - (e as FunctionMatrixElement).Function);
        }

        public IMatrixElement Clone()
        {
            return new FunctionMatrixElement(this.Function.Clone() as Function);
        }

        public IMatrixElement Inverse()
        {
            return new FunctionMatrixElement(this.Function.Inverse());
        }

        public IMatrixElement Zero()
        {
            return new FunctionMatrixElement(0);
        }

        public IMatrixElement Pow(double power)
        {
            return new FunctionMatrixElement(this.Function.Pow(power));
        }

        public IMatrixElement Sqrt()
        {
            return this.Pow(0.5);
        }

        public IMatrixElement Div(IMatrixElement e)
        {
            return new FunctionMatrixElement(this.Function / (e as FunctionMatrixElement).Function);
        }

        public double ToDouble()
        {
            return this.Function.ToDouble();
        }

        public override string ToString()
        {
            return this.Function.ToString();
        }


        public string ToMathML()
        {
            return String.Format("<mrow>{0}</mrow>", this.Function.ToMathML());
        }
    }
}
