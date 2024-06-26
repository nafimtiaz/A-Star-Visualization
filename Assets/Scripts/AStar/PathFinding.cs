using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class PathFinding : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Transform start;
        [SerializeField] private Transform target;

        // this list stores the calculated path
        private List<Node> _calculatedPath;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grid.PopulateGrid();
                FindPath(start.position, target.position);
            }
        }

        // This function calculates the path using A* algorithm
        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = grid.GetNodeByPosition(startPos);
            Node targetNode = grid.GetNodeByPosition(targetPos);

            // Open set means these nodes are still modifiable and
            // not explored yet, will be marked with yellow color in game view
            List<Node> openSet = new List<Node>();
            
            // closedSet means these nodes are no longer modifiable, we have explored it completely
            // that means, we can't change the G and H costs and parent node,
            // will be marked with pinkish red color in game view
            HashSet<Node> closedSet = new HashSet<Node>();

            // Add start node to open set, this is our entry node
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                
                for (int i = 0; i < openSet.Count; i++)
                {
                    // Choose the open node with lowest F cost
                    // or by the lowest H cost if F costs are equal
                    // F cost = G cost + H cost
                    if (openSet[i].FCost < currentNode.FCost ||
                        (openSet[i].FCost == currentNode.FCost &&
                         openSet[i].HCost < currentNode.HCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                // If target node is found, calculate the path
                if (currentNode == targetNode)
                {
                    _calculatedPath = GetRetracedPath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in currentNode.Neighbours)
                {
                    // No need to update the costs of the neighbouring explored nodes and obstacle nodes
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    // Check if the new G cost is less than the current G cost of the neighbouring
                    // node, if yes we update the costs. Also, if the neighbouring node
                    // is not in open set, we add them to open set, so that on next iteration
                    // we can choose the next lowest H cost node from these neighbours
                    int newMovementCostToNeighbour =
                        currentNode.GCost + GetDistanceBetweenNodes(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistanceBetweenNodes(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
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
    }
}