using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AStar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Grid = AStar.Grid;

public class AppManager : MonoBehaviour
{
    [Header("A-Star Related")]
    [SerializeField] private Transform start;
    [SerializeField] private Transform target;
    
    [SerializeField] private PathFinding pathFinder;
    [SerializeField] private Grid grid;
    
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform obstaclePoolParent;
    
    [SerializeField] private GameObject nodeInfoPrefab;
    [SerializeField] private Transform nodeInfoGridParent;
    [SerializeField] private Transform nodeInfoPoolParent;

    [Header("UI")] 
    [SerializeField] private CursorControl cursorControl;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextStepButton;
    [SerializeField] private Button completeSearchButton;
    [SerializeField] private Button clearEverythingButton;
    [SerializeField] private TMP_Dropdown placementSelection;

    private List<GameObject> _obstaclePool;
    private Dictionary<Node, GameObject> _currentObstaclesDict;
    
    private List<GameObject> _nodeInfoPool;
    private Dictionary<Node, GameObject> _currentNodeInfoParentsDict;


    void Start()
    {
        grid.PopulateGrid();
        CreateObstaclePool();
        CreateNodeInfoViewsAndParents();
        AssignButtonCallbacks();
        cursorControl.canPlace = true;
        pathFinder.PrepareForNewPathFinding(grid, start.position, target.position);
    }

    #region UI/Callbacks

    private void AssignButtonCallbacks()
    {
        nextStepButton.onClick.AddListener(ShowNextSearchStep);
        completeSearchButton.onClick.AddListener(CompleteFullSearch);
        clearEverythingButton.onClick.AddListener(ClearEverything);
        playButton.onClick.AddListener(PlayPathFinding);
        pauseButton.onClick.AddListener(PausePathFinding);
    }
    
    private void PlayPathFinding()
    {
        if (pathFinder.IsSearchComplete)
        {
            return;
        }
        
        StartCoroutine(PlayNextSearch());
        UpdateUIOnPlayBtnPressed();
    }
    
    private void PausePathFinding()
    {
        StopAllCoroutines();
        UpdateUIOnActivityDone();
    }
    
    IEnumerator PlayNextSearch()
    {
        cursorControl.canPlace = false;
        placementSelection.interactable = false;

        while (!pathFinder.IsSearchComplete)
        {
            pathFinder.FindPath();

            if (pathFinder.IsSearchComplete)
            {
                UpdateUIOnActivityDone();
            }
            
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void ShowNextSearchStep()
    {
        cursorControl.canPlace = false;
        placementSelection.interactable = false;
        pathFinder.FindPath();
        UpdateUIOnActivityDone();
    }

    private void CompleteFullSearch()
    {
        cursorControl.canPlace = false;
        placementSelection.interactable = false;
        
        while (!pathFinder.IsSearchComplete)
        {
           pathFinder.FindPath();
        }
        
        UpdateUIOnActivityDone();
    }

    private void ClearEverything()
    {
        ClearObstacles();
        ClearNodeInfoViews();
        grid.ResetAllNodes();
        cursorControl.canPlace = true;
        pathFinder.PrepareForNewPathFinding(grid, start.position, target.position);
        UpdateUIOnEverythingCleared();
    }

    #region Toggling Elements

    private void UpdateUIOnPlayBtnPressed()
    {
        placementSelection.interactable = false;
        nextStepButton.interactable = false;
        completeSearchButton.interactable = false;
        clearEverythingButton.interactable = false;
    }
    
    private void UpdateUIOnActivityDone()
    {
        bool isComplete = pathFinder.IsSearchComplete;
        placementSelection.interactable = false;
        nextStepButton.interactable = !isComplete;
        completeSearchButton.interactable = !isComplete;
        clearEverythingButton.interactable = true;
    }
    
    private void UpdateUIOnEverythingCleared()
    {
        placementSelection.interactable = true;
        nextStepButton.interactable = true;
        completeSearchButton.interactable = true;
        clearEverythingButton.interactable = true;
    }

    #endregion

    #endregion

    #region Start/Target Nodes

    private Node StartNode => grid.GetNodeByPosition(start.position);
    private Node TargetNode => grid.GetNodeByPosition(target.position);

    public void PlaceStartNode(Vector3 pos)
    {
        Node n = grid.GetNodeByPosition(pos);

        if (!n.IsWalkable || n == TargetNode)
        {
            return;
        }
        
        start.position = n.WorldPos;
        pathFinder.PrepareForNewPathFinding(grid, start.position, target.position);
    }
    
    public void PlaceTargetNode(Vector3 pos)
    {
        Node n = grid.GetNodeByPosition(pos);
        
        if (!n.IsWalkable || n == StartNode)
        {
            return;
        }
        
        target.position = n.WorldPos;
        pathFinder.PrepareForNewPathFinding(grid, start.position, target.position);
    }

    #endregion

    #region Obstacles

    private void ClearObstacles()
    {
        if (_currentObstaclesDict != null && _currentObstaclesDict.Count > 0)
        {
            foreach (var obstacle in _currentObstaclesDict.Values)
            {
                obstacle.SetActive(false);
            }
            
            _currentObstaclesDict.Clear();
            _currentObstaclesDict.TrimExcess();
        }
    }

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

    private void ClearNodeInfoViews()
    {
        foreach (var n in _nodeInfoPool)
        {
            n.transform.SetParent(nodeInfoPoolParent);
            n.SetActive(false);
        }
    }

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
