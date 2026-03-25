using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject prefab;
    public LayerMask layerMask;
    private ARRaycastManager _raycastManager;
    private ARPlaneManager _planeManager;

    private List<ARRaycastHit> _hitList;
    private GameObject selectedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hitList = new List<ARRaycastHit>();    
        _raycastManager = FindObjectOfType<ARRaycastManager>();
        _planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void TrySelect(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if(Physics.Raycast(ray, out RaycastHit hit, 500, layerMask))
        {
            selectedObject = hit.transform.gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();// Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Destroy Item
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                DestroyItem(mousePos);
            }

            if (Input.touchCount != 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == UnityEngine.TouchPhase.Began)
                {
                    DestroyItem(touch.position);
                }
            }
        }
    }

    void SpawnItem(Vector2 screenPos)
    {
        if (_raycastManager.Raycast(screenPos, _hitList, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitpose = _hitList[0].pose;
            Instantiate(prefab, hitpose.position, hitpose.rotation);
            Debug.Log("Spawned item");
        }
    }

    void DestroyItem(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 500, layerMask))
        {
            Destroy(hit.transform.gameObject);
            Debug.Log("Destroyed item");
        }
    }

    public void SpawnGame()
    {
        for (int i = 0; i < 5; i++)
        {
            int posx = Random.Range(0, Screen.width);
            int posy = Random.Range(0, Screen.height);

            if (_raycastManager.Raycast(new Vector2(posx, posy), _hitList, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitpose = _hitList[0].pose;
                Instantiate(prefab, hitpose.position, hitpose.rotation);
                Debug.Log("Spawned item");
            }
        }
    }
}
