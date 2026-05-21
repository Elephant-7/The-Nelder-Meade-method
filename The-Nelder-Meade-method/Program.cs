using NTDLS.ExpressionParser;
using System;
using System.Globalization;
using System.Linq.Expressions;

class NelderMead
{
    private const double Alpha = 1.0;
    private const double Gamma = 2.0;
    private const double Rho = 0.5;
    private const double Sigma = 0.5;

    public static (double[] xMin, double fMin) Minimize(
        Func<double[], double> f,
        double[] start,
        double tol = 1e-6,
        int maxIter = 1000)
    {
        int n = start.Length;
        double[][] simplex = new double[n + 1][];

        double delta = 1.0;
        for (int i = 0; i <= n; i++)
        {
            simplex[i] = new double[n];
            Array.Copy(start, simplex[i], n);
            if (i > 0)
                simplex[i][i - 1] += delta;
        }

        double[] fvals = new double[n + 1];
        for (int i = 0; i <= n; i++)
            fvals[i] = f(simplex[i]);

        for (int iter = 0; iter < maxIter; iter++)
        {
            int iLo = 0, iHi = 0, iSecondHi = 0;

            for (int i = 1; i <= n; i++)
            {
                if (fvals[i] < fvals[iLo]) iLo = i;
                if (fvals[i] > fvals[iHi]) iHi = i;
            }

            iSecondHi = iLo;
            for (int i = 0; i <= n; i++)
            {
                if (i == iHi) continue;
                if (fvals[i] > fvals[iSecondHi]) iSecondHi = i;
            }

            double[] centroid = new double[n];
            for (int i = 0; i <= n; i++)
            {
                if (i == iHi) continue;
                for (int j = 0; j < n; j++)
                    centroid[j] += simplex[i][j];
            }
            for (int j = 0; j < n; j++)
                centroid[j] /= n;

            double[] xr = new double[n];
            for (int j = 0; j < n; j++)
                xr[j] = centroid[j] + Alpha * (centroid[j] - simplex[iHi][j]);

            double fr = f(xr);

            if (fr < fvals[iLo])
            {
                double[] xe = new double[n];
                for (int j = 0; j < n; j++)
                    xe[j] = centroid[j] + Gamma * (xr[j] - centroid[j]);

                double fe = f(xe);
                if (fe < fr)
                    Replace(simplex, fvals, iHi, xe, fe);
                else
                    Replace(simplex, fvals, iHi, xr, fr);
            }
            else if (fr < fvals[iSecondHi])
            {
                Replace(simplex, fvals, iHi, xr, fr);
            }
            else
            {
                double[] xc = new double[n];
                if (fr < fvals[iHi])
                {
                    for (int j = 0; j < n; j++)
                        xc[j] = centroid[j] + Rho * (xr[j] - centroid[j]);
                }
                else
                {
                    for (int j = 0; j < n; j++)
                        xc[j] = centroid[j] + Rho * (simplex[iHi][j] - centroid[j]);
                }

                double fc = f(xc);
                if (fc < fvals[iHi])
                    Replace(simplex, fvals, iHi, xc, fc);
                else
                    Shrink(simplex, fvals, f, iLo);
            }

            double maxDist = 0;
            for (int i = 0; i <= n; i++)
            {
                for (int k = i + 1; k <= n; k++)
                    maxDist = Math.Max(maxDist, Distance(simplex[i], simplex[k]));
            }

            if (maxDist < tol)
                break;
        }

        int best = Array.IndexOf(fvals, MinValue(fvals));
        return (simplex[best], fvals[best]);
    }

    private static void Replace(double[][] simplex, double[] fvals, int idx, double[] x, double fx)
    {
        simplex[idx] = x;
        fvals[idx] = fx;
    }

    private static void Shrink(double[][] simplex, double[] fvals, Func<double[], double> f, int iLo)
    {
        for (int i = 0; i < simplex.Length; i++)
        {
            if (i == iLo) continue;

            for (int j = 0; j < simplex[i].Length; j++)
                simplex[i][j] = simplex[iLo][j] + Sigma * (simplex[i][j] - simplex[iLo][j]);

            fvals[i] = f(simplex[i]);
        }
    }

    private static double Distance(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            double d = a[i] - b[i];
            sum += d * d;
        }
        return Math.Sqrt(sum);
    }

    private static double MinValue(double[] arr)
    {
        double m = arr[0];
        for (int i = 1; i < arr.Length; i++)
            if (arr[i] < m) m = arr[i];
        return m;
    }

    static void Main()
    {
        Console.WriteLine("Метод Нелдера-Мида");
        int n = ReadInt("Введите размерность: ");

        double[] start = new double[n];
        Console.WriteLine("Введите начальную точку:");
        for (int i = 0; i < n; i++)
            start[i] = ReadDouble($"x{i} = ");

        Console.WriteLine();
        Console.WriteLine("Введите функцию через x0, x1, ..., например:");
        Console.WriteLine("(x0-2)^2 + (x1-1)^2");
        Console.WriteLine("x0*x0 + x1*x1");
        Console.Write("f(x) = ");
        string userExpression = Console.ReadLine();

        Func<double[], double> func = BuildFunction(userExpression, n);

        try
        {
            var (xMin, fMin) = Minimize(func, start);

            Console.WriteLine();
            Console.WriteLine("=== РЕЗУЛЬТАТ ===");
            Console.WriteLine("Минимум найден в точке:");
            for (int i = 0; i < xMin.Length; i++)
                Console.WriteLine($"x{i} = {xMin[i]:F6}");
            Console.WriteLine($"f(x*) = {fMin:F6}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка в функции: " + ex.Message);
        }
    }

    static Func<double[], double> BuildFunction(string expressionText, int n)
    {
        return x =>
        {
            var expression = new Expression(expressionText);

            for (int i = 0; i < n; i++)
                expression.SetParameter($"x{i}", x[i]);

            var result = expression.Evaluate();
            return Convert.ToDouble(result, CultureInfo.InvariantCulture);
        };
    }

    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string s = Console.ReadLine();
            if (int.TryParse(s, out int value))
                return value;

            Console.WriteLine("Введите целое число.");
        }
    }

    static double ReadDouble(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string s = Console.ReadLine();

            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
                double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
                return value;

            Console.WriteLine("Введите число.");
        }
    }
}