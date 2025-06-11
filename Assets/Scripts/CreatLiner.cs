using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CreatLiner : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private GameObject linePrefab;
    private LineRenderer line;
    private Vector3 newPos;

    public List<Vector3> linePositions = new List<Vector3>();
    private List<GameObject> drawnLines = new List<GameObject>();

    [Header("Input Actions")]
    [SerializeField] InputActionReference _touchPosition; // Vector2
    [SerializeField] InputActionReference _touchPress;    // Button

    private bool isDrawing = false;
    private Vector2 _cachedTouchPosition;
    private Vector2 _cachedBeginDragPosition;

    private void OnEnable()
    {
        _touchPosition.action.performed += OnTouchPositionPerformed;
        _touchPosition.action.Enable();
        _touchPress.action.performed += OnTouchPressPerformed;
        _touchPress.action.Enable();
    }

    private void OnDisable()
    {
        _touchPosition.action.performed -= OnTouchPositionPerformed;
        _touchPosition.action.Disable();
        _touchPress.action.performed -= OnTouchPressPerformed;
        _touchPress.action.Disable();
    }

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (isDrawing)
        {
            Vector3 screenPosition = new Vector3(_cachedTouchPosition.x, _cachedTouchPosition.y, _cam.nearClipPlane + 1f);
            newPos = _cam.ScreenToWorldPoint(screenPosition);

            linePositions.Add(newPos);

            line.positionCount++;
            line.SetPosition(line.positionCount - 1, newPos);
        }
    }

    void OnTouchPositionPerformed(InputAction.CallbackContext context)
    {
        _cachedTouchPosition = context.ReadValue<Vector2>();
    }

    void OnTouchPressPerformed(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (!isDrawing)
            {
                isDrawing = true;
                _cachedBeginDragPosition = _cachedTouchPosition;
                StartLine();
            }
        }
        else
        {
            if (isDrawing)
            {
                isDrawing = false;
            }
        }
    }

    private void StartLine()
    {
        Vector3 screenPosition = new Vector3(_cachedTouchPosition.x, _cachedTouchPosition.y, _cam.nearClipPlane + 1f);
        newPos = _cam.ScreenToWorldPoint(screenPosition);

        linePositions.Clear();
        linePositions.Add(newPos);

        GameObject obj = Instantiate(linePrefab);
        line = obj.GetComponent<LineRenderer>();
        line.positionCount = 1;
        line.SetPosition(0, newPos);

        drawnLines.Add(obj);
    }

    public void EraseLines()
    {
        foreach (var obj in drawnLines)
        {
            Destroy(obj);
        }

        drawnLines.Clear();
        linePositions.Clear();
    }
}
