using System;
using System.Globalization;

namespace NelderMeadMethod
{
    public static class ConsoleUI
    {
        public static int ReadInt(string prompt)
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

        public static double ReadDouble(string prompt)
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

        public static void Run()
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

            Func<double[], double> func = FunctionBuilder.Build(userExpression, n);
            var optimizer = new NelderMeadOptimizer();

            try
            {
                var (xMin, fMin) = optimizer.Minimize(func, start);

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
    }
}