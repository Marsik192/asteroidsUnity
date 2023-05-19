
namespace XR.CloudAnchors
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System;


    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;


    using Google.XR.ARCoreExtensions;

    public class CloudAnchorsController : MonoBehaviour
    {
        public static CloudAnchorsController instance;

        // Public variables
        public GameObject CloudAnchorPrefab;
        public Camera MainCamera;
        public GameObject MapQualityIndicatorPrefab;
        public ARRaycastManager RaycastManager;
        public ARPlaneManager PlaneManager;
        public ARAnchorManager AnchorManager;
        public GameObject NamePanel;
        public InputField NamePanelText;

        // Private variables 
        private MapQualityIndicator _qualityIndicator = null;
        private ARAnchor _anchor = null;
        private State state;
        private List<ARCloudAnchor> _pendingCloudAnchors = new List<ARCloudAnchor>();
        private List<ARCloudAnchor> _cachedCloudAnchors = new List<ARCloudAnchor>();
        private CloudAnchorHistory _hostedCloudAnchor;

        // private const
        private const int _storageLimit = 40;
        private const string _persistentCloudAnchorsStorageKey = "PersistentCloudAnchors";


        private enum State
        {
            Idle,
            Creating,
            Playing
        }


        void Start()
        {
            instance = this;
            state = State.Idle;
            NamePanel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (state == State.Creating)
            {
                if (_anchor == null)
                {
                    Touch touch;
                    if (Input.touchCount < 1 ||
                    (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                    {
                        return;
                    }

                    PerformHitTest(touch.position);
                }
                HostingCloudAnchor();
            }
            UpdatePendingCloudAnchors();
        }

        public void OnCreateAnchorsClick()
        {
            Mmi.UI.MenuManager.instance.HideAllMenus();
            Mmi.UI.MenuManager.instance.HideMenuButton();
            Mmi.UI.MenuManager.instance.ExitCreateAnchorModeButton.SetActive(true);
            Game.ARPartManager.HideAll();
            state = State.Creating;
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
                state = State.Playing;
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
                else
                {
                    _pendingCloudAnchors.Add(cloudAnchor);
                }
            }
            Pose GetCameraPose()
            {
                return new Pose(MainCamera.transform.position,
                    MainCamera.transform.rotation);
            }
        }
        private void UpdatePendingCloudAnchors()
        {
            foreach (var cloudAnchor in _pendingCloudAnchors)
            {
                if (cloudAnchor.cloudAnchorState == CloudAnchorState.Success)
                {
                    if (state == State.Creating)
                    {
                        Debug.LogFormat("Succeed to host the Cloud Anchor: {0}.",
                            cloudAnchor.cloudAnchorId);
                        NamePanel.SetActive(true);
                    }
                    _cachedCloudAnchors.Add(cloudAnchor);
                }
                else if (cloudAnchor.cloudAnchorState != CloudAnchorState.TaskInProgress)
                {
                    if (state == State.Creating)
                    {
                        Debug.LogFormat("Failed to host the Cloud Anchor with error {0}.",
                            cloudAnchor.cloudAnchorState);
                    }
                    _cachedCloudAnchors.Add(cloudAnchor);
                }
            }
            _pendingCloudAnchors.RemoveAll(x => x.cloudAnchorState != CloudAnchorState.TaskInProgress);
        }

        public void ExitCreateAnchorMode()
        {
            Mmi.UI.MenuManager.instance.ExitCreateAnchorModeButton.SetActive(false);
            Mmi.UI.MenuManager.instance.ShowMenuButton();
            state = State.Idle;
            Game.ARPartManager.ShowAll();
        }

        public void OnSaveButtonClicked()
        {
            _hostedCloudAnchor.Name = NamePanelText.text;
            SaveCloudAnchorHistory(_hostedCloudAnchor);

            Debug.Log("SaVeD!");
        }

        public void SaveCloudAnchorHistory(CloudAnchorHistory data)
        {
            var history = LoadCloudAnchorHistory();

            // Sort the data from latest record to oldest record which affects the option order in
            // multiselection dropdown.
            history.Collection.Add(data);
            history.Collection.Sort((left, right) => right.CreatedTime.CompareTo(left.CreatedTime));

            // Remove the oldest data if the capacity exceeds storage limit.
            if (history.Collection.Count > _storageLimit)
            {
                history.Collection.RemoveRange(
                    _storageLimit, history.Collection.Count - _storageLimit);
            }

            PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey, JsonUtility.ToJson(history));
        }

        public CloudAnchorHistoryCollection LoadCloudAnchorHistory()
        {
            if (PlayerPrefs.HasKey(_persistentCloudAnchorsStorageKey))
            {
                var history = JsonUtility.FromJson<CloudAnchorHistoryCollection>(
                    PlayerPrefs.GetString(_persistentCloudAnchorsStorageKey));

                // Remove all records created more than 24 hours and update stored history.
                DateTime current = DateTime.Now;
                history.Collection.RemoveAll(
                    data => current.Subtract(data.CreatedTime).Days > 0);
                PlayerPrefs.SetString(_persistentCloudAnchorsStorageKey,
                    JsonUtility.ToJson(history));
                return history;
            }

            return new CloudAnchorHistoryCollection();
        }



    }
}
