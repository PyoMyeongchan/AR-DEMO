using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TouchUI : MonoBehaviour
{
    [SerializeField] private Image touchObject;
    [SerializeField] private Camera arCamera;

    [SerializeField] private InputActionReference tapPosition;
    [SerializeField] private InputActionReference tapPress;

    private bool isPressed;
    private Vector2 cachedTouch;

    private void OnEnable()
    {
        tapPosition.action.performed += OnTouchPosition;
        tapPress.action.performed += OnTouchPress;

        tapPosition.action.Enable();
        tapPress.action.Enable();
    }

    private void OnDisable()
    {
        tapPosition.action.performed -= OnTouchPosition;
        tapPress.action.performed -= OnTouchPress;

        tapPosition.action.Disable();
        tapPress.action.Disable();
    }

    private void OnTouchPosition(InputAction.CallbackContext context)
    {
        cachedTouch = context.ReadValue<Vector2>();
    }

    private void OnTouchPress(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            CheckTouch();
        }
    }

    private void CheckTouch()
    {
        Ray ray = arCamera.ScreenPointToRay(cachedTouch);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 5f);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Raycast");
            if (hit.collider != null && hit.collider.CompareTag("ARObject"))
            {
                string objName = hit.collider.name.Replace("(Clone)", "").Trim();                
                string resourcePath = $"Marker/{objName}";
                Sprite loadedSprite = Resources.Load<Sprite>(resourcePath);
  
                if (loadedSprite != null)
                {
                    touchObject.sprite = loadedSprite;
                    touchObject.enabled = true;
                }
                else
                {
                    Debug.LogWarning($"[로드 실패] Resources.Load<Sprite>('{resourcePath}') 실패 - 경로 혹은 타입 확인 필요");
                    
                    touchObject.enabled = false;
                }
            }
            else
            {
                touchObject.enabled = false;
            }
        }
        else
        {
            touchObject.enabled = false;
        }
    }
}
