using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts
{
    public class GamePieceNode : GameElement
    {
        [SerializeField] private SpriteRenderer sRenderer;
        [SerializeField] private TMP_Text valueText;
        
        public int Value { get; private set; }

        public TileNode TileNode { get; private set; }
        
        public GamePieceNode MatchableNode { get; set; }
        
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);
            gameObject.name = $"Piece ({x},{y})";
        }

        public void SetGamePieceNode(NodeData data)
        {
            sRenderer.color = data.backgroundColor;
            valueText.color = data.textColor;
            Value = data.value;
            valueText.text = Value.ToString();
        }

        public void SetTileNode(TileNode node)
        {
            // Clear our tile's assigned piece..
            // Then set our next tile node.
            if (TileNode != null) TileNode.PieceNode = null;
            TileNode = node;
            TileNode.PieceNode = this;
        }

        public Tween Move(Vector2 position, float travelTime = .1f)
        {
            return transform.DOMove(position, travelTime);
        }

        public bool AvailableForMatch(int value)
        {
            return Value == value && MatchableNode != null;
        }

        public void Match(GamePieceNode nodeToMatch)
        {
            MatchableNode = nodeToMatch;
            TileNode.PieceNode = null;
        }
    }
}