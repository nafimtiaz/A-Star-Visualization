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
    
    private List<GameObject> _nodeInfoPool;
    private Dictionary<Node, GameObject> _currentNodeInfoParentsDict;
    private Node StartNode => grid.GetNodeByPosition(start.position);
    private Node TargetNode => grid.GetNodeByPosition(target.position);
    

    void Start()
    {
        grid.PopulateGrid();
        CreateObstaclePool();
        CreateNodeInfoViewsAndParents();
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
            GameObject obstacle = Instantiate(obstaclePrefab, obstaclePoolParent, true);
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
                obstacle.name = $"Obstacle_{n.GridX}_{n.GridZ}";
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

    #region Node Info View

    private void CreateNodeInfoViewsAndParents()
    {
        _currentNodeInfoParentsDict = new Dictionary<Node, GameObject>();
        _nodeInfoPool = new List<GameObject>();
        
        for (int x = 0; x < grid.GridX; x++)
        {
            for (int z = 0; z < grid.GridZ; z++)
            {
                GameObject nodeInfoParent = new GameObject($"NodeInfoParent_{x}_{z}")
                {
                    transform =
                    {
                        parent = nodeInfoGridParent,
                        localEulerAngles = Vector3.zero,
                        localScale = Vector3.one
                    }
                };
                
                nodeInfoParent.AddComponent<Canvas>();
                _currentNodeInfoParentsDict.Add(grid.GetNodeByIndex(x, z), nodeInfoParent);

                GameObject nodeInfoViewObject = Instantiate(nodeInfoPrefab, nodeInfoPoolParent);
                _nodeInfoPool.Add(nodeInfoViewObject);
                nodeInfoViewObject.SetActive(false);
            }
        }
    }

    // Enable/Change the node info view whenever we change the status
    public void UpdateNodeInfoViewStatus(Node n, NodeStatus status)
    {
        n.NodeStatus = status;
        Transform nodeInfoViewParent = _currentNodeInfoParentsDict[n].transform;
        NodeInfoView nodeInfoView = null;

        if (nodeInfoViewParent.childCount <= 0)
        {
            foreach (var infoView in _nodeInfoPool)
            {
                if (!infoView.activeInHierarchy)
                {
                    nodeInfoView = infoView.GetComponent<NodeInfoView>();
                    break;
                }
            }
        }
        else
        {
            nodeInfoView = nodeInfoViewParent.GetChild(0).GetComponent<NodeInfoView>();
        }

        if (nodeInfoView != null)
        {
            nodeInfoView.gameObject.SetActive(true);
            RectTransform rect = nodeInfoView.GetComponent<RectTransform>();
            rect.SetParent(nodeInfoViewParent);
                
            rect.offsetMin = new Vector2(0f, 0f);
            rect.offsetMax = new Vector2(0f, 0f);

            nodeInfoView.GetComponent<NodeInfoView>().Populate(n);
        }
        else
        {
            Debug.LogWarning("NodeInfoView not available in pool");
        }
    }

    #endregion
}
