using System;
using System.Collections.Generic;
using System.Linq;
using AStar;
using UnityEngine;
using Grid = AStar.Grid;

public class AppManager : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform target;
    
    [SerializeField] private PathFinding pathFinder;
    [SerializeField] private Grid grid;
    
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform obstaclePoolParent;
    
    [SerializeField] private GameObject nodeInfoPrefab;
    [SerializeField] private Transform nodeInfoGridParent;
    [SerializeField] private Transform nodeInfoPoolParent;

    private List<GameObject> _obstaclePool;
    private Dictionary<Node, GameObject> _currentObstaclesDict;
    private Node StartNode => grid.GetNodeByPosition(start.position);
    private Node TargetNode => grid.GetNodeByPosition(target.position);
    

    void Start()
    {
        grid.PopulateGrid();
        CreateObstaclePool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pathFinder.FindPath(grid, start.position, target.position);
        }
    }

    #region Obstacles

    private void CreateObstaclePool()
    {
        _obstaclePool = new List<GameObject>();
        
        for (int i = 0; i < grid.GridX * grid.GridZ; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, obstaclePoolParent, true) as GameObject;
            obstacle.SetActive(false);
            _obstaclePool.Add(obstacle);
        }
    }

    public void AddObstacle(Vector3 pos)
    {
        if (_currentObstaclesDict == null)
        {
            _currentObstaclesDict = new Dictionary<Node, GameObject>();
        }

        Node n = grid.GetNodeByPosition(pos);

        if (n == StartNode || n == TargetNode)
        {
            return;
        }
        
        if (_currentObstaclesDict.Keys.Contains(n))
        {
            return;
        }
        
        foreach (var obstacle in _obstaclePool)
        {
            if (!obstacle.activeInHierarchy)
            {
                _currentObstaclesDict.Add(n, obstacle);
                obstacle.SetActive(true);
                obstacle.transform.position = pos;
                break;
            }
        }
        
        grid.ToggleNodeAsObstacle(pos, true);
    }

    public void RemoveObstacle(Vector3 pos)
    {
        Node n = grid.GetNodeByPosition(pos);
        
        if (_currentObstaclesDict == null || !_currentObstaclesDict.Keys.Contains(n))
        {
            return;
        }
        
        grid.ToggleNodeAsObstacle(pos, false);
        _currentObstaclesDict[n].SetActive(false);
        _currentObstaclesDict.Remove(n);
    }

    #endregion
}
