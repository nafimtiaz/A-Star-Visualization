using AStar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Color32 openColor;
    [SerializeField] private Color32 closedColor;
    [SerializeField] private Color32 pathColor;
    [SerializeField] private TextMeshProUGUI GCost;
    [SerializeField] private TextMeshProUGUI HCost;
    [SerializeField] private TextMeshProUGUI FCost;
    [SerializeField] private GameObject[] dirImages;
    [SerializeField] private GameObject glowEffect;

    public void Populate(Node node)
    {
        GCost.text = node.GCost.ToString();
        HCost.text = node.HCost.ToString();
        FCost.text = node.FCost.ToString();
        
        glowEffect.SetActive(node.NodeStatus == NodeStatus.Path);

        switch (node.NodeStatus)
        {
            case NodeStatus.Open:
                background.color = openColor;
                break;
            case NodeStatus.Closed:
                background.color = closedColor;
                break;
            case NodeStatus.Path:
                background.color = pathColor;
                break;
        }
        
        SetParentDirection(node);
    }

    // This function enables the sprite that
    // points to the parent node
    private void SetParentDirection(Node node)
    {
        foreach (var image in dirImages)
        {
            image.SetActive(false);
        }
        
        if (node.Parent == null)
        {
            return;
        }

        Vector3 dir = (node.Parent.WorldPos - node.WorldPos).normalized;
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        dirImages[Constants.NODE_DIR_ANGLES.IndexOf((int)yaw)].SetActive(true);
        gameObject.name = $"NodeInfo_{node.GridX}_{node.GridZ}_{yaw}";
    }
}
