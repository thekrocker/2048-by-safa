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

        public Tween ScaleUp()
        {
            var initialScale = transform.localScale;
            transform.localScale = Vector3.zero;
            return transform.DOScale(initialScale, .3f);
        }

        public override void SetCoords(int x, int y)
        {
            base.SetCoords(x, y);
            gameObject.name = $"Piece ({x},{y})";
        }


        public Tween Move(Vector2 position, float travelTime = .1f)
        {
            return transform.DOMove(position, travelTime);
        }

        public bool AvailableForMatch(int value)
        {
            return Value == value;
        }
    }
}