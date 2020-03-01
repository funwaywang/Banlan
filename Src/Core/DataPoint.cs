using System;
using System.Collections.Generic;
using System.Text;

namespace Banlan.Core
{
    class DataPoint
    {
        public readonly float[] Data;
        public readonly int Weight;
        public readonly int Position;
        public Cluster Cluster { get; set; }

        public DataPoint(float[] data, int weight, Cluster cluster, int position)
        {
            Data = data;
            Weight = weight;
            Cluster = cluster;
            Position = position;
        }
    }
}
