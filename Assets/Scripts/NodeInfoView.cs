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

    public void Populate(Node node)
    {
        GCost.text = node.GCost.ToString();
        HCost.text = node.HCost.ToString();
        FCost.text = node.FCost.ToString();

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
    }
}
