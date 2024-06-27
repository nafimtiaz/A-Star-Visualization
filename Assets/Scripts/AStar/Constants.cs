using System.Collections.Generic;

namespace AStar
{
    public class Constants
    {
        public static readonly int NODE_DIAGONAL_DIST = 14;
        public static readonly int NODE_DIRECT_DIST = 10;
        public static readonly List<int> NODE_DIR_ANGLES = new() { 0, 45, 90, 135, 180, -135, -90, -45};
    }
}