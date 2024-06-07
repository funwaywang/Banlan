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
    class OctreeNode
    {
        private readonly float[] mean;
        private readonly OctreeNode?[] children;
        private int count = 0;
        private bool isLeaf;

        public OctreeNode(Octree octree, int level)
        {
            isLeaf = level == octree.MaxBits;
            mean = new float[Octree.dimensions];

            if (isLeaf)
            {
                octree.LeafCount++;
                NextReducible = null;
                children = [];
            }
            else
            {
                NextReducible = octree.ReducibleNodes[level];
                octree.ReducibleNodes[level] = this;
                children = new OctreeNode[Octree.branching];
            }
        }

        public int Position { get; private set; }

        public OctreeNode? NextReducible { get; private set; }

        public OctreeNode InsertVector(byte[] v, Octree octree, int level, int position = -1)
        {
            if (isLeaf)
            {
                count++;
                for (int i = 0; i < Octree.dimensions; i++)
                {
                    if (count == 1)
                    {
                        mean[i] = v[i];
                    }
                    else
                    {
                        mean[i] = (mean[i] * (count - 1) + v[i]) / count;
                    }
                }
                return this;
            }
            else
            {
                var index = GetIndex(v, level, octree);
                var child = children[index];
                if (child == null)
                {
                    child = new OctreeNode(octree, level + 1)
                    {
                        Position = position
                    };
                    children[index] = child;
                }
                return child.InsertVector(v, octree, level + 1, position);
            }
        }

        private int GetIndex(byte[] v, int level, Octree octree)
        {
            var shift = octree.MaxBits - 1 - level;
            var index = 0;
            for (int i = 0; i < Octree.dimensions; i++)
            {
                var reverseIndex = Octree.dimensions - 1 - i;
                index |= (v[i] & octree.LevelMasks[level]) >> (shift - reverseIndex);
            }
            return index;
        }

        public int Reduce()
        {
            if (isLeaf)
            {
                return 0;
            }

            var numChildren = 0;
            for (var childIndex = 0; childIndex < children.Length; childIndex++)
            {
                var child = children[childIndex];
                if (child != null)
                {
                    var newCount = count + child.count;
                    for (int i = 0; i < Octree.dimensions; i++)
                    {
                        var nodeSum = mean[i] * count;
                        var childSum = child.mean[i] * child.count;
                        mean[i] = (nodeSum + childSum) / newCount;
                    }

                    count = newCount;
                    numChildren++;
                    children[childIndex] = null;
                }
            }

            isLeaf = true;
            return numChildren - 1;
        }

        public List<Palette> GetData(List<Palette>? data = null, int index = 0)
        {
            if (data == null)
            {
                data = GetData(new List<Palette>(), 0);
            }
            else if (isLeaf)
            {
                data.Add(new Palette(mean, count, Position));
            }
            else
            {
                foreach (var child in children)
                {
                    if (child != null)
                    {
                        data = child.GetData(data, index);
                    }
                }
            }
            return data;
        }
    }
}
