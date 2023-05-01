using System;
using UnityEngine;

namespace Safa_Packs.Swipe_System
{
    public class SwipeHorizontalMover
    {
        private readonly Transform _center;
        private readonly float _moveDistance;
        private readonly float _horizontalMoveSpeed;
        private int _currentLane;

        public SwipeHorizontalMover(Transform center, float moveDistance, float horizontalMoveSpeed)
        {
            _center = center;
            _moveDistance = moveDistance;
            _horizontalMoveSpeed = horizontalMoveSpeed;
        }

        public Vector3 GetMovementVector(float moveSpeed)
        {
            Vector3 moveVector;
            
            moveVector.x = GetSnapLaneValue();
            moveVector.y = 0f;
            moveVector.z = moveSpeed;

            return moveVector;
        }

        public void CheckSwipe()
        {
            if (SwipeManager.Instance.SwipeRight)
            {
                ChangeCurrentLane(1);
            }
            
            if (SwipeManager.Instance.SwipeLeft)
            {
                ChangeCurrentLane(-1);
            }
        }

        private void ChangeCurrentLane(int direction)
        {
            _currentLane = Mathf.Clamp(_currentLane + direction, -1, 1);
        }

        private float GetSnapLaneValue()
        {
            float snapValue;
            
            if (Math.Abs(_center.position.x - (_currentLane * _moveDistance)) > .01f)
            {
                
                float deltaPos = (_currentLane * _moveDistance) - _center.position.x;
                snapValue = (deltaPos > 0) ? 1 : -1;
                snapValue *= _horizontalMoveSpeed;
                
                float actualDistance = snapValue * Time.deltaTime;
                
                if (Mathf.Abs(actualDistance) > Mathf.Abs(deltaPos))
                {
                    snapValue = deltaPos * (1f / Time.deltaTime);
                }

            }
            else
            {
                snapValue = 0f;
            }
            
            return snapValue;
        }
    }
}