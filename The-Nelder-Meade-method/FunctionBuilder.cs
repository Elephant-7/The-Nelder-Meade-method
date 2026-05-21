using System;
using System.Globalization;
using NTDLS.ExpressionParser;

namespace NelderMeadMethod
{
    public static class FunctionBuilder
    {
        public static Func<double[], double> Build(string expressionText, int n)
        {
            return x =>
            {
                var expression = new Expression(expressionText);
                for (int i = 0; i < n; i++)
                    expression.SetParameter($"x{i}", x[i]);
                object result = expression.Evaluate();
                return Convert.ToDouble(result, CultureInfo.InvariantCulture);
            };
        }
    }
}