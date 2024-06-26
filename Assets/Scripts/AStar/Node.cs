using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Node
    {
        public bool IsWalkable;   // We set IsWalkable = false for obstacle nodes
        public Vector3 WorldPos;  // THis Vector3 stores the real world Position of the node
        
        public int GridX;   // Index of the node across world X axis
        public int GridZ;   // Index of the node across world Z axis
        
        // we will continuously set the lowest possible distance between a node
        // and start node to GCost, until the node is done exploring
        public int GCost;
        
        // we will continuously set the lowest possible distance between a node
        // and target node to HCost, until the node is done exploring
        public int HCost;
        
        // List of neighbouring nodes that surrounds a node, doesn't include obstacle nodes
        public List<Node> Neighbours;
        public Node Parent;
        public NodeStatus NodeStatus;

        public int FCost => GCost + HCost;

        public Node(bool isWalkable, Vector3 worldPos, int gridX, int gridZ)
        {
            IsWalkable = isWalkable;
            WorldPos = worldPos;
            GridX = gridX;
            GridZ = gridZ;
        }
    }
}
