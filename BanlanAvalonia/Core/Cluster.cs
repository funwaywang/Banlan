using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Banlan.Core
{
    /// <summary>
    /// reference to https://github.com/dcollien/Dreamcoat
    /// </summary>
    class Cluster
    {
        public int size { get; private set; }
        private readonly int[] dimensions;
        private readonly float[] sum;
        private readonly List<int> Positions = new List<int>();

        public Cluster(int numDimensions)
        {
            size = 0;
            dimensions = Enumerable.Range(0, numDimensions).Select(it => it).ToArray();
            sum = new float[dimensions.Length];
        }

        public int Position => Positions.Where(p => p > -1).Select(p => (int?)p).LastOrDefault() ?? -1;

        public Cluster AddPoint(DataPoint point)
        {
            if (point != null)
            {
                size += point.Weight;
                for (int i = 0; i < dimensions.Length; i++)
                {
                    sum[i] += point.Data[i] * point.Weight;
                }
                point.Cluster = this;
                Positions.Add(point.Position);
            }
            return this;
        }

        public float[] RemovePoint(DataPoint point)
        {
            var changed = new float[dimensions.Length];
            if (point != null)
            {
                point.Cluster = null;
                size -= point.Weight;

                for (int i = 0; i < dimensions.Length; i++)
                {
                    sum[i] -= point.Data[i] * point.Weight;
                    changed[i] = sum[i];
                }

                var index = Positions.IndexOf(point.Position);
                if (index > -1)
                {
                    Positions.RemoveAt(index);
                }
            }

            return changed;
        }

        public float[] GetMean()
        {
            var mean = new float[dimensions.Length];
            if (size > 0)
            {
                for (int i = 0; i < dimensions.Length; i++)
                {
                    mean[i] = sum[i] / size;
                }
            }
            return mean;
        }

        public float GetDistanceTo(DataPoint point)
        {
            var centroid = GetMean();
            var squaredDist = 0f;
            for (int i = 0; i < dimensions.Length; i++)
            {
                var diff = centroid[i] - point.Data[i];
                squaredDist += diff * diff;
            }

            return squaredDist;
        }
    }
}
