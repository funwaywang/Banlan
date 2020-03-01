using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Banlan.Core
{
    /// <summary>
    /// reference to https://github.com/dcollien/Dreamcoat
    /// </summary>
    class Octree
    {
        public const int branching = 8;
        public const int dimensions = 3;

        private OctreeNode prevNode;
        private byte[] prevVect;

        public Octree(int maxBits = 8)
        {
            MaxBits = maxBits;
            LeafCount = 0;
            NumVectors = 0;
            ReducibleNodes = new OctreeNode[MaxBits + 1];
            LevelMasks = Enumerable.Range(0, maxBits).Reverse().Select(it => (int)Math.Pow(2, it)).ToArray();
            Root = new OctreeNode(this, 0);
        }

        public int MaxBits { get; private set; }

        public int LeafCount { get; set; }

        public int NumVectors { get; private set; }

        public OctreeNode Root { get; private set; }

        public OctreeNode[] ReducibleNodes { get; private set; }

        public int[] LevelMasks { get; private set; }

        private bool IsVectorEqual(byte[] v1, byte[] v2)
        {
            if (v1 == null || v2 == null)
            {
                return false;
            }

            for (int i = 0; i < Octree.dimensions; i++)
            {
                if (v1[i] != v2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int InsertVector(byte[] newVect, int position)
        {
            if ((prevNode != null) && (IsVectorEqual(newVect, prevVect)))
            {
                prevNode.InsertVector(newVect, this, 0, position);
            }
            else
            {
                prevVect = newVect;
                prevNode = Root.InsertVector(newVect, this, 0, position);
            }
            return NumVectors++;
        }

        private OctreeNode Reduce()
        {
            var levelIndex = MaxBits - 1;
            while (levelIndex > 0 && (ReducibleNodes[levelIndex] == null))
            {
                levelIndex--;
            }

            var node = ReducibleNodes[levelIndex];
            ReducibleNodes[levelIndex] = node.NextReducible;
            LeafCount -= node.Reduce();

            return prevNode = null;
        }

        public List<Palette> ReduceToSize(int itemCount)
        {
            while (LeafCount > itemCount)
            {
                Reduce();
            }

            return Root.GetData();
        }
    }
}
