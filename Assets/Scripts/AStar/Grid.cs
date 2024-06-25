using System;
using AStar;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector2 gridSize;
    [SerializeField] private float nodeRadius;
    
    private Node[,] _grid;

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
