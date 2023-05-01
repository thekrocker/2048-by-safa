using UnityEngine;

namespace _Project.Scripts
{
    public abstract class GameElement : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Vector2 Position => transform.position;

        public virtual void Initialize(int x, int y)
        {
            SetCoords(x, y);
        }

        public virtual void SetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}