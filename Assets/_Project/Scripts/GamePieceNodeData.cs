using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class NodeData
{
    public Color backgroundColor;
    public Color textColor;
    public int value;
}

[CreateAssetMenu(menuName = "Data/Game Piece Data")]
public class GamePieceNodeData : ScriptableObject
{
    [SerializeField ] private NodeData[] nodeData;

    public NodeData GetNodeDataByValue(int value)
    {
        return nodeData.First(x => x.value == value);
    }
}
