using System;

namespace NelderMeadMethod
{
    public class Simplex
    {
        public double[][] Points { get; private set; }
        public double[] Values { get; private set; }

        public int Dimension => Points[0].Length;
        public int Size => Points.Length;

        public Simplex(double[][] points, double[] values)
        {
            Points = points;
            Values = values;
        }

        public static Simplex CreateInitial(double[] start, double delta = 1.0)
        {
            int n = start.Length;
            var points = new double[n + 1][];
            for (int i = 0; i <= n; i++)
            {
                points[i] = new double[n];
                Array.Copy(start, points[i], n);
                if (i > 0)
                    points[i][i - 1] += delta;
            }
            return new Simplex(points, new double[n + 1]);
        }

        public void EvaluateAll(Func<double[], double> f)
        {
            for (int i = 0; i < Points.Length; i++)
                Values[i] = f(Points[i]);
        }

        public double[] GetCentroid(int excludeIndex)
        {
            int n = Dimension;
            var centroid = new double[n];
            for (int i = 0; i < Size; i++)
            {
                if (i == excludeIndex) continue;
                for (int j = 0; j < n; j++)
                    centroid[j] += Points[i][j];
            }
            for (int j = 0; j < n; j++)
                centroid[j] /= n;
            return centroid;
        }

        public (int iLow, int iHigh, int iSecondHigh) GetIndicesByValues()
        {
            int iLow = 0, iHigh = 0, iSecondHigh = 0;
            for (int i = 1; i < Size; i++)
            {
                if (Values[i] < Values[iLow]) iLow = i;
                if (Values[i] > Values[iHigh]) iHigh = i;
            }
            iSecondHigh = iLow;
            for (int i = 0; i < Size; i++)
            {
                if (i == iHigh) continue;
                if (Values[i] > Values[iSecondHigh]) iSecondHigh = i;
            }
            return (iLow, iHigh, iSecondHigh);
        }

        public void ReplacePoint(int idx, double[] newPoint, double newValue)
        {
            Points[idx] = newPoint;
            Values[idx] = newValue;
        }

        public void Shrink(int iLow, Func<double[], double> f, double sigma = 0.5)
        {
            for (int i = 0; i < Size; i++)
            {
                if (i == iLow) continue;
                for (int j = 0; j < Dimension; j++)
                    Points[i][j] = Points[iLow][j] + sigma * (Points[i][j] - Points[iLow][j]);
                Values[i] = f(Points[i]);
            }
        }

        public double MaxDistance()
        {
            double max = 0;
            for (int i = 0; i < Size; i++)
                for (int k = i + 1; k < Size; k++)
                    max = Math.Max(max, Distance(Points[i], Points[k]));
            return max;
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

        public (double[] bestPoint, double bestValue) GetBest()
        {
            int bestIdx = Array.IndexOf(Values, MinValue(Values));
            return (Points[bestIdx], Values[bestIdx]);
        }

        private static double MinValue(double[] arr)
        {
            double m = arr[0];
            for (int i = 1; i < arr.Length; i++)
                if (arr[i] < m) m = arr[i];
            return m;
        }
    }
}