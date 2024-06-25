using UnityEngine;

namespace AStar
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private Vector2 gridSize;
        [SerializeField] private float nodeRadius;

        private int GridX => (int)gridSize.x;
        private int GridZ => (int)gridSize.y;
    
        private Node[,] _grid;

        private void Start()
        {
            CreateGrid();
        
            // Adding obstacles manually for the time being
            _grid[2, 4].IsWalkable = false;
            _grid[3, 4].IsWalkable = false;
            _grid[4, 4].IsWalkable = false;
            _grid[5, 4].IsWalkable = false;
            _grid[6, 4].IsWalkable = false;
            _grid[6, 5].IsWalkable = false;
            _grid[6, 6].IsWalkable = false;
        }

        private void CreateGrid()
        {
            _grid = new Node[GridX , GridZ];

            for (int x = 0; x < GridX; x++)
            {
                for (int z = 0; z < GridZ; z++)
                {
                    _grid[x, z] = new Node(true, new Vector3(x, 0f, z));
                }
            }
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
