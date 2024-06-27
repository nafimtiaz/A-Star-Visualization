using System;
using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class PathFinding : MonoBehaviour
    {
        [SerializeField] private AppManager appManager;
        
        // this list stores the calculated path
        private List<Node> _calculatedPath;
        private Node _startNode;
        private Node _targetNode;
        private List<Node> _openSet;
        private HashSet<Node> _closedSet;
        private bool _isPathFindingEnabled;
        private bool _isSearchComplete;

        public bool IsSearchComplete => _isSearchComplete;

        public void PrepareForNewPathFinding(Grid grid, Vector3 startPos, Vector3 targetPos)
        {
            _isPathFindingEnabled = true;
            _isSearchComplete = false;
            _startNode = grid.GetNodeByPosition(startPos);
            _targetNode = grid.GetNodeByPosition(targetPos);

            // Open set means these nodes are still modifiable and
            // not explored yet, will be marked with yellow color in game view
            _openSet = new List<Node>();

            // closedSet means these nodes are no longer modifiable, we have explored it completely
            // that means, we can't change the G and H costs and parent node,
            // will be marked with pinkish red color in game view
            _closedSet = new HashSet<Node>();

            // Add start node to open set, this is our entry node
            _openSet.Add(_startNode);
        }

        // This function calculates the path using A* algorithm
        public void FindPath()
        {
            if (!_isPathFindingEnabled || _isSearchComplete)
            {
                return;
            }
            
            Node currentNode = _openSet[0];

            for (int i = 0; i < _openSet.Count; i++)
            {
                // Choose the open node with lowest F cost
                // or by the lowest H cost if F costs are equal
                // F cost = G cost + H cost
                if (_openSet[i].FCost < currentNode.FCost ||
                    (_openSet[i].FCost == currentNode.FCost &&
                     _openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = _openSet[i];
                }
            }

            _openSet.Remove(currentNode);
            _closedSet.Add(currentNode); 
            appManager.UpdateNodeInfoViewStatus(currentNode, NodeStatus.Closed);

            // If target node is found, calculate the path
            if (currentNode == _targetNode)
            {
                _calculatedPath = GetRetracedPath(_startNode, _targetNode);
                _isPathFindingEnabled = false;
                _isSearchComplete = true;
            }

            foreach (Node neighbour in currentNode.Neighbours)
            {
                // No need to update the costs of the neighbouring explored nodes and obstacle nodes
                if (!neighbour.IsWalkable || _closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Check if the new G cost is less than the current G cost of the neighbouring
                // node, if yes we update the costs. Also, if the neighbouring node
                // is not in open set, we add them to open set, so that on next iteration
                // we can choose the next lowest H cost node from these neighbours
                int newMovementCostToNeighbour =
                    currentNode.GCost + GetDistanceBetweenNodes(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !_openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistanceBetweenNodes(neighbour, _targetNode);
                    neighbour.Parent = currentNode;

                    if (!_openSet.Contains(neighbour))
                    {
                        _openSet.Add(neighbour);
                        appManager.UpdateNodeInfoViewStatus(neighbour, NodeStatus.Open);
                    }
                }
                
                if (_openSet.Count == 0)
                {
                    _isPathFindingEnabled = false;
                    _isSearchComplete = true;
                }
            }
        }

        // Roll back from target node to start node
        // by using parents reference, this will give us the path
        private List<Node> GetRetracedPath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode.Parent;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                appManager.UpdateNodeInfoViewStatus(currentNode, NodeStatus.Path);
                currentNode = currentNode.Parent;
            }
            
            path.Reverse();
            return path;
        }

        // This method returns smallest possible distance between two nodes
        // if x = distance across X axis and z = distance across Z axis
        // if x < y, we choose x for diagonal distance, and (y - x) for direct distance, (14x + 10(y-x))
        // as unit diagonal distance (14) is greater than unit direct distance (10)
        // So, by multiplying x with unit diagonal distance ensures smallest distance connecting two nodes
        private int GetDistanceBetweenNodes(Node a, Node b)
        {
            int distX = Mathf.Abs(a.GridX - b.GridX);
            int distZ = Mathf.Abs(a.GridZ - b.GridZ);

            if (distX < distZ)
            {
                return Constants.NODE_DIAGONAL_DIST * distX + (distZ - distX) * Constants.NODE_DIRECT_DIST;
            }
            
            return Constants.NODE_DIAGONAL_DIST * distZ + (distX - distZ) * Constants.NODE_DIRECT_DIST;
        }

        /*
        private void OnDrawGizmos()
        {
            if (_calculatedPath != null && _calculatedPath.Count > 0)
            {
                Gizmos.color = Color.green;
                foreach (var node in _calculatedPath)
                {
                    Vector3 origin = new Vector3(node.GridX + 0.5f, 0.25f, node.GridZ + 0.5f); 
                    Gizmos.DrawCube(origin, new Vector3(1f, 0.5f, 1f));
                }
            }
        }
        */
    }
}