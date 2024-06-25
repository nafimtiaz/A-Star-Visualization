using UnityEngine;

namespace AStar
{
    public class Node
    {
        private bool _isWalkable;
        private Vector3 _worldPos;

        public bool IsWalkable
        {
            get => _isWalkable;
            set => _isWalkable = value;
        }
        public Vector3 GetWorldPosition => _worldPos;

        public Node(bool isWalkable, Vector3 worldPos)
        {
            _isWalkable = isWalkable;
            _worldPos = worldPos;
        }
    }
}
