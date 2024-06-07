using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Banlan.Core
{
    /// <summary>
    /// reference to https://github.com/dcollien/Dreamcoat
    /// </summary>
    class KMeans
    {
        private readonly int numClusters;
        private readonly Cluster[] clusters;
        private static readonly Random random = new Random();

        public KMeans(int numClusters, int numDimensions)
        {
            this.numClusters = numClusters;
            clusters = Enumerable.Range(0, numClusters).Select(it => new Cluster(numDimensions)).ToArray();
        }

        public DataPoint[]? DataPoints { get; private set; }

        public void SetPoints(List<Palette> palettes)
        {
            DataPoints = (from pt in palettes
                          select new DataPoint(pt.Mean, pt.Count, null, pt.Position)).ToArray();
        }

        private Cluster[] InitClusters()
        {
            if (DataPoints == null)
            {
                throw new NullReferenceException(nameof(DataPoints));
            }

            var firstCenterIndex = random.Next(DataPoints.Length);
            clusters[0].AddPoint(DataPoints[firstCenterIndex]);

            foreach (var cluster in clusters.Skip(1))
            {
                var maxDist = 0f;
                DataPoint? bestCenter = null;
                foreach (var point in DataPoints)
                {
                    if (point.Cluster == null)
                    {
                        var minDist = NearestClusterDistance(point);
                        if (bestCenter == null || minDist > maxDist)
                        {
                            maxDist = minDist;
                            bestCenter = point;
                        }
                    }
                }

                if (bestCenter != null)
                {
                    cluster.AddPoint(bestCenter);
                }
            }
            return clusters.Skip(1).ToArray();
        }

        private float NearestClusterDistance(DataPoint point)
        {
            var minDist = float.MaxValue;
            foreach (var cluster in this.clusters)
            {
                if (cluster != null && cluster.size > 0)
                {
                    minDist = Math.Min(minDist, cluster.GetDistanceTo(point));
                }
            }

            return minDist;
        }

        public Cluster[] PerformCluster()
        {
            InitClusters();
            while (ClusterStep() != 0)
            {
            }
            return clusters;
        }

        private Cluster? NearestClusterTo(DataPoint point)
        {
            Cluster? nearestCluster = null;
            var nearestDistance = float.MaxValue;
            foreach (var cluster in clusters)
            {
                if (cluster != null && cluster.size > 0)
                {
                    if (nearestCluster == null)
                    {
                        nearestCluster = cluster;
                        nearestDistance = nearestCluster.GetDistanceTo(point);
                    }
                    else
                    {
                        var distance = cluster.GetDistanceTo(point);
                        if (cluster.GetDistanceTo(point) < nearestDistance)
                        {
                            nearestCluster = cluster;
                            nearestDistance = distance;
                        }
                    }
                }
            }

            return nearestCluster;
        }

        private bool ReassignPoint(DataPoint point, Cluster cluster)
        {
            var currentCluster = point.Cluster;
            var wasAbleToAssign = false;
            if (currentCluster == null)
            {
                cluster.AddPoint(point);
                wasAbleToAssign = true;
            }
            else if (currentCluster.size > 1 && cluster != currentCluster)
            {
                currentCluster.RemovePoint(point);
                cluster.AddPoint(point);
                wasAbleToAssign = true;
            }
            return wasAbleToAssign;
        }

        private int ClusterStep()
        {
            var numMoves = 0;
            if(DataPoints != null)
            {
                foreach (var point in DataPoints)
                {
                    var nearestCluster = NearestClusterTo(point);
                    if (nearestCluster != null && ReassignPoint(point, nearestCluster))
                    {
                        numMoves++;
                    }
                }
            }

            return numMoves;
        }
    }
}
