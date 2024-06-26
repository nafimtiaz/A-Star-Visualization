using System.Collections.Generic;
using UnityEngine;

namespace AStar
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector2 gridSize;
        [SerializeField] private float nodeRadius;

        public int GridX => (int)gridSize.x;
        public int GridZ => (int)gridSize.y;
    
        private Node[,] _grid;

        public void PopulateGrid()
        {
            CreateGrid();
            CacheNodeNeighbours();
        }

        public void ToggleNodeAsObstacle(Vector3 pos, bool isObstacle)
        {
            Node n = GetNodeByPosition(pos);
            n.IsWalkable = !isObstacle;
        }
        
                
        private void CreateGrid()
        {
            _grid = new Node[GridX , GridZ];

            for (int x = 0; x < GridX; x++)
            {
                for (int z = 0; z < GridZ; z++)
                {
                    _grid[x, z] = new Node(true, new Vector3(x, 0f, z), x, z);
                }
            }
        }

        // Caching neighbouring nodes in advance
        private void CacheNodeNeighbours()
        {
            for (int x = 0; x < GridX; x++)
            {
                for (int z = 0; z < GridZ; z++)
                {
                    _grid[x, z].Neighbours = GetNeighbourNodes(_grid[x, z]);
                }
            }
        }
        
        // We are using 1x1 area as a node and the first node starts at world origin
        // so, it's safe to return a node by converting the floor values of X and
        // Z positions and use them as node indices to get a node by position
        public Node GetNodeByPosition(Vector3 pos)
        {
            int xIndex = Mathf.FloorToInt(pos.x);
            int zIndex = Mathf.FloorToInt(pos.z);

            return _grid[xIndex, zIndex];
        }

        public Node GetNodeByIndex(int gridX, int gridZ)
        {
            return _grid[gridX, gridZ];
        }

        // Get neighbouring nodes of a node, checking 8 possible locations
        // considering grid size limits (excluding the original node itself)
        // Nodes on the border will have < 8 neighbouring nodes
        private List<Node> GetNeighbourNodes(Node node)
        {
            List<Node> neighbours = new List<Node>();
            
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int neighbourX = node.GridX + x;
                    int neighbourZ = node.GridZ + z;

                    if (neighbourX >= 0 &&
                        neighbourX < gridSize.x &&
                        neighbourZ >= 0 &&
                        neighbourZ < gridSize.y &&
                        !(x == 0 && z == 0))
                    {
                        neighbours.Add(_grid[neighbourX,neighbourZ]);
                    }
                }
            }

            return neighbours;
        }

        private void OnDrawGizmos()
        {
            float gizmoHeight = 1f;
            Vector3 gizmoPos = transform.position;
            gizmoPos.y += (gizmoHeight * 0.5f);
            gizmoPos.x += gridSize.x * 0.5f;
            gizmoPos.z += gridSize.y * 0.5f;
        
            Gizmos.DrawWireCube(gizmoPos, new Vector3(gridSize.x, gizmoHeight, gridSize.y));
        }
    }
}
