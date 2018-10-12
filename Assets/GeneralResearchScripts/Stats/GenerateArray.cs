using System.Collections.Generic;
namespace Research
{
    public class GenerateArray
    {
        public static List<int> Range(int n)
        {
            List<int> vals = new List<int>();
            for (int i = 0; i < n; i++)
            {
                vals.Add(i);
            }
            return vals;
        }

        public static List<int> RandPerm(int n)
        {
            int ind;
            System.Random rand = new System.Random();

            List<int> vals = Range(n);
            List<int> shuffle = new List<int>();
            while (vals.Count > 0)
            {
                ind = rand.Next(vals.Count);
                shuffle.Add(vals[ind]);
                vals.RemoveAt(ind);
            }
            return shuffle;
        }
    }
}
