using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-305)]
public class SwipeManager : MonoBehaviour
{
    public static SwipeManager Instance;

    [SerializeField] private float swipeThreshold = 50f;
    
    public Vector2 MousePosition => _swipeControlAction.SwipeControls.Position.ReadValue<Vector2>();
    
    private SwipeControlAction _swipeControlAction;

    private Vector2 _dragStartPosition;
    
    public bool SwipeLeft { get; private set; }
    public bool SwipeRight { get; private set; }
    public bool SwipeUp { get; private set; }
    public bool SwipeDown { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;

        _swipeControlAction = new SwipeControlAction();
    }
    
    private void OnEnable()
    {
        _swipeControlAction.SwipeControls.OnStartSwipe.performed += OnSwipeStart;
        _swipeControlAction.SwipeControls.OnEndSwipe.performed += OnSwipeEnd;
    }

    public void EnableSwipe(bool enable)
    {
        if (enable)
        {
            _swipeControlAction.Enable();
        }
        else
        {
            _swipeControlAction.Disable();
        }
    }


    private void OnDisable()
    {
        _swipeControlAction.SwipeControls.OnStartSwipe.performed -= OnSwipeStart;
        _swipeControlAction.SwipeControls.OnEndSwipe.performed -= OnSwipeEnd;
    }

    private void OnSwipeStart(InputAction.CallbackContext obj)
    {
        _dragStartPosition = MousePosition;
    }
    
    private void OnSwipeEnd(InputAction.CallbackContext obj)
    {
        Vector2 delta = MousePosition - _dragStartPosition;
        float sqrDistance = delta.magnitude;

        if (sqrDistance > swipeThreshold)
        {
            float x = Mathf.Abs(delta.x);
            float y = Mathf.Abs(delta.y);

            // We are swiping horizontally
            if (x > y)
            {
                if (delta.x > 0f)
                {
                    SwipeRight = true;
                }
                else
                {
                    SwipeLeft = true;
                }
            }
            else
            {
                if (delta.y > 0f)
                {
                    SwipeUp = true;
                }
                else
                {
                    SwipeDown = true;
                }
            }
        }

        _dragStartPosition = Vector2.zero;
    }

    private void LateUpdate()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        SwipeRight = SwipeLeft = SwipeUp = SwipeDown = false;
    }
}
