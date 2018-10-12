using System.Collections.Generic;
namespace Research
{
    public class Statistics
    {
        public static float Mean(List<float> X)
        {
            return Sum(X) / (float)X.Count;
        }

        public static float Sum(List<float> X)
        {
            float sum = 0f;
            for (int i = 0; i < X.Count; i++)
            {
                sum += X[i];
            }
            return (sum);
        }

        public static float Median(List<float> X)
        {
            X.Sort();
            int midPt = X.Count / 2;
            if (X.Count % 2 == 0)
            {
                return 0.5f * (X[midPt - 1] + X[midPt]);
            } else
            {
                return X[midPt];
            }
        }
    }
}