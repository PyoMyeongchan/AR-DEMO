using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImageTracker : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabsToSpawn = new List<GameObject>();

    private ARTrackedImageManager _trackedImageManager;
    private Dictionary<string, GameObject> _objects;

    private void Start()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        if (_trackedImageManager == null)
        {
            return;
        }

        _trackedImageManager.trackedImagesChanged += OnImageTrackerChanged;
        _objects = new Dictionary<string, GameObject>();
        SetupSceneElements();
    }

    private void OnDestroy()
    {
        if (_trackedImageManager != null)
            _trackedImageManager.trackedImagesChanged -= OnImageTrackerChanged;
    }

    private void SetupSceneElements()
    {
        foreach (var prefab in prefabsToSpawn)
        {
            var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.name = prefab.name;
            obj.SetActive(false);
            _objects.Add(obj.name, obj);
        }
    }

    private void OnImageTrackerChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            DisableTrackedImage(trackedImage);
        }
    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null)
            return;

        string imageName = trackedImage.referenceImage.name;

        if (string.IsNullOrEmpty(imageName))
        {
            return;
        }

        if (!_objects.ContainsKey(imageName))
        {
            return;
        }

        GameObject obj = _objects[imageName];

        if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
            obj.transform.position = trackedImage.transform.position;
            obj.transform.rotation = trackedImage.transform.rotation;
        }
    }

    private void DisableTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null)
            return;

        string imageName = trackedImage.referenceImage.name;

        if (!string.IsNullOrEmpty(imageName) && _objects.ContainsKey(imageName))
        {
            _objects[imageName].SetActive(false);
        }
    }
}
