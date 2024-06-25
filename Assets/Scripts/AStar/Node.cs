using UnityEngine;

namespace AStar
{
    public class Node
    {
        private bool _isWalkable;
        private Vector3 _worldPosition;

        public Node(bool isWalkable, Vector3 worldPosition)
        {
            _isWalkable = isWalkable;
            _worldPosition = worldPosition;
        }
    }
}
