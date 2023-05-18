using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XR.CloudAnchors;
using Google.XR.ARCoreExtensions;

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
        if (CloudAnchorPrefab != null)
        {
            Instantiate(CloudAnchorPrefab, _anchor.transform);

            // Attach map quality indicator to this anchor.
            var indicatorGO =
                Instantiate(MapQualityIndicatorPrefab, _anchor.transform);
            _qualityIndicator = indicatorGO.GetComponent<MapQualityIndicator>();
            _qualityIndicator.DrawIndicator(planeType, MainCamera);

        }
    }
    private void HostingCloudAnchor()
    {
        // There is no anchor for hosting.
        if (_anchor == null)
        {
            return;
        }
        // Update map quality:
        int qualityState = 2;
        // Can pass in ANY valid camera pose to the mapping quality API.
        // Ideally, the pose should represent users’ expected perspectives.
        FeatureMapQuality quality =
            AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        qualityState = (int)quality;
        _qualityIndicator.UpdateQualityState(qualityState);

        // Hosting instructions:
        var cameraDist = (_qualityIndicator.transform.position -
            MainCamera.transform.position).magnitude;

        if (_qualityIndicator.ReachQualityThreshold)
        {
            AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());

            // Creating a Cloud Anchor with lifetime = 1 day.
            // This is configurable up to 365 days when keyless authentication is used.
            ARCloudAnchor cloudAnchor = AnchorManager.HostCloudAnchor(_anchor, 1);
            if (cloudAnchor == null)
            {
                Debug.LogFormat("Failed to create a Cloud Anchor.");
            }
        }
        Pose GetCameraPose()
        {
            return new Pose(MainCamera.transform.position,
                MainCamera.transform.rotation);
        }
    }


}
