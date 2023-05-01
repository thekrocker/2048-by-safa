using UnityEngine;

namespace _Project.Scripts
{
    public class TileNode : GameElement
    {
        public override void Initialize(int x, int y)
        {
            base.Initialize(x, y);
            gameObject.name = $"Tile ({x},{y})";
        }
    }
}