using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.OptimisationMethods
{
    public class FunctionsVariations
    {
        static public Tuple<Function, string> FunctionVariation(Function I, double level = 1, double from = 0, double to = 1, string by = "x")
        {
            StringBuilder sb = new StringBuilder(
            @"<!DOCTYPE html>
            <?xml-stylesheet type='text/xsl' href='mathml.xsl'?>
            <html
             xmlns='http://www.w3.org/1999/xhtml'
             xmlns:math='http://www.w3.org/1998/Math/MathML'
             >
            <head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /></head>
            <body>
                <b>Вариация функции:</b></br>"
            );

            sb.AppendFormat("<math><mrow><mi>I</mi><mo>=</mo><mrow><msubsup><mo>&int;</mo><mn>{1}</mn><mn>{2}</mn></mrow><mrow><mfenced>{0}</mfenced></mrow><mo>d</mo><mi>{3}</mi></mrow></math></br>", I.ToMathML(), from, to, by);
            string[] vars = I.Variables;
            Dictionary<string, IMatrixElement> varsValues = new Dictionary<string, IMatrixElement>();

            Variable Alpha = new Variable("&alpha;");
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i] != by)
                {
                    varsValues[vars[i]] = new FunctionMatrixElement(new Function(new Variable(vars[i]) + Alpha * new Variable("d" + vars[i])));
                }
            }

            Function I2 = new Function(I.SetVariablesValues(varsValues));
            sb.AppendFormat(@"
                <math><mrow>
                    <mi>I</mi><mfenced><mrow><mi>y</mi><mo>+</mo><mi>{4}</mi><mo>d</mo><mi>y</mi></mrow></mfenced><mo>=</mo>
                    <mrow>
                        <mrow><msubsup><mo>&int;</mo><mn>{1}</mn><mn>{2}</mn></msubsup></mrow>
                        <mrow>{0}</mrow><mrow><mo>d</mo><mi>{3}</mi></mrow></mrow>
                    </mrow>
                </mrow></math></br>", I2.ToMathML(), from, to, by, Alpha.Name);

            var curI = I2;
            for (int i = 0; i < level; i++)
            {
                sb.AppendFormat("</br></br>{0} вариация:</br>", i + 1);
                var dI2 = new Function(curI.Derivative(Alpha.Name));
                
                sb.AppendFormat("<math><mrow><msup><mo>d</mo><mn>{4}</mn></msup></mrow><mi>I</mi><mo>=</mo><msubsup><mo>&int;</mo><mn>{1}</mn><mn>{2}</mn></msubsup><mfenced><mrow>{0}</mrow></mfenced><mo>d</mo><mi>{3}</mi><math></br></br>", dI2.ToMathML(), from, to, by, i + 1);
                sb.AppendFormat("<math><mrow><msup><mo>d</mo><mn>{4}</mn></msup></mrow><mi>I</mi><mo>&InvisibleTimes;</mo><mrow><mfenced><mrow>{5}</mrow><mo>=</mo><mi>0</mi></mfenced></mrow><mo>=</mo><msubsup><mo>&int;</mo><mn>{1}</mn><mn>{2}</mn></msubsup><mfenced><mrow>{0}<mrow></mfenced><mo>d</mo><mi>{3}</mi><math>", dI2.SetVariableValue(Alpha.Name, 0).ToMathML(), from, to, by, i + 1, Alpha.Name);
                curI = dI2;
            }
            sb.Append("</body></html>");

            return new Tuple<Function, string>(new Function(curI.SetVariableValue(Alpha.Name, 0)), sb.ToString());
        }

        static public Tuple<Function, string> FunctionExtremum(Function F, string by = "x")
        {
            StringBuilder sb = new StringBuilder(
                @"<!DOCTYPE html>
                <?xml-stylesheet type='text/xsl' href='mathml.xsl'?>
                <html
                 xmlns='http://www.w3.org/1999/xhtml'
                 xmlns:math='http://www.w3.org/1998/Math/MathML'
                 >
                <head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /></head>
                <body>
                    <b>Экстремум функции:</b></br>"
            );
            
            string[] vars = F.Variables;
            Dictionary<string, IMatrixElement> varsValues = new Dictionary<string, IMatrixElement>();

            sb.AppendFormat("<b>F</b>({0}", by);
            for (int i = 0; i < vars.Length; i++)
            {
                if (vars[i] != by)
                {
                    //varsValues[vars[i]] = new FunctionMatrixElement(new Function(new Variable(vars[i]) + Alpha * new Variable("d" + vars[i])));

                    sb.AppendFormat(", {0}", vars[i]);
                }
            }
            sb.AppendFormat(") = {0}</br></br>", F.ToMathML());

            Function answer = F.Derivative("y") - F.Derivative("x").Derivative("y'") - new Variable("y'") * F.Derivative("y").Derivative("y'") - new Variable("y''") * F.Derivative("y'").Derivative("y'");
            sb.AppendFormat("<b>Ответ</b>:<br>{0} = 0", answer.ToMathML());
            if (answer.HasVariable("y") && answer.HasVariable("y'") && answer.HasVariable("y''"))
            {
                sb.Append("</br>Yes");
            }
            sb.Append("</body></html>");

            return new Tuple<Function, string>(answer, sb.ToString());
        }
    }
}
