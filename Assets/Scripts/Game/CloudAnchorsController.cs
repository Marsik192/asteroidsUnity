using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XR.CloudAnchors;

public class CloudAnchorsController : MonoBehaviour
{
    // Public variables

    public GameObject CloudAnchorPrefab;
    public Camera MainCamera;
    public GameObject MapQualityIndicatorPrefab;
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    public ARAnchorManager AnchorManager;

    // Private variables 
    private MapQualityIndicator _qualityIndicator = null;
    private ARAnchor _anchor = null;
    private bool gameIsActive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsActive)
        {
            if (CloudAnchorPrefab == null)
            {
                Touch touch;
                if (Input.touchCount < 1 ||
                (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                {
                    return;
                }

                PerformHitTest(touch.position);
            }
        }
    }

    public void OnCreateAnchorsClick()
    {
        Mmi.UI.MenuManager.instance.HideAllMenus();
        Game.ARPartManager.HideAll();
        gameIsActive = true;
    }

    private void PerformHitTest(Vector2 touchPosition)
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
        RaycastManager.Raycast(
            touchPosition, hitResults, TrackableType.PlaneWithinPolygon);
        Debug.Log("59");
        // If there was an anchor placed, then instantiate the corresponding object.
        var planeType = PlaneAlignment.HorizontalUp;
        if (hitResults.Count > 0)
        {
            ARPlane plane = PlaneManager.GetPlane(hitResults[0].trackableId);
            if (plane == null)
            {
                Debug.LogWarningFormat("Failed to find the ARPlane with TrackableId {0}",
                    hitResults[0].trackableId);
                return;
            }

            planeType = plane.alignment;
            var hitPose = hitResults[0].pose;

            _anchor = AnchorManager.AttachAnchor(plane, hitPose);
        }
        Debug.Log("77");
        if (CloudAnchorPrefab != null)
        {
            Instantiate(CloudAnchorPrefab, _anchor.transform);

            // Attach map quality indicator to this anchor.
            var indicatorGO =
                Instantiate(MapQualityIndicatorPrefab, _anchor.transform);
            _qualityIndicator = indicatorGO.GetComponent<MapQualityIndicator>();
            _qualityIndicator.DrawIndicator(planeType, MainCamera);

        }
        Debug.Log("89");
    }
}
