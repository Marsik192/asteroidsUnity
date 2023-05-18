using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Game
{

    public class GameMenu : MonoBehaviour
    {
        public Toggle togglePointClouds;
        public Toggle togglePlanes;

        private ARPointCloudParticleVisualizer pointCloudParticleVisualizer;
        private ARPlaneManager planeManager;
        private GameObject planePrefab;

        private void Start() {
            planeManager = FindObjectOfType<ARPlaneManager>();
            planePrefab = planeManager.planePrefab;
        }

        private void OnEnable()
        {
            pointCloudParticleVisualizer = FindObjectOfType<ARPointCloudParticleVisualizer>();
            if (pointCloudParticleVisualizer && planeManager)
            {
                togglePointClouds.isOn = pointCloudParticleVisualizer.enabled;
                togglePlanes.isOn = planeManager.planePrefab != null;
            }

            SetEvents();
        }

        private void OnDisable() {
            pointCloudParticleVisualizer = null;
            ClearEvents();
        }

        private void SetEvents()
        {
            togglePointClouds.onValueChanged.AddListener(OnTogglePointClouds);
            togglePlanes.onValueChanged.AddListener(OnTogglePlanes);
        }
        private void ClearEvents()
        {
            togglePointClouds.onValueChanged.RemoveListener(OnTogglePointClouds);
            togglePlanes.onValueChanged.RemoveListener(OnTogglePlanes);
        }
        private void OnTogglePointClouds(bool pValue)
        {
            pointCloudParticleVisualizer.enabled = pValue;
        }
        private void OnTogglePlanes(bool pValue)
        {
            ARPlaneMeshVisualizer[] planes = FindObjectsOfType<ARPlaneMeshVisualizer>();
            for (int i=0; i<planes.Length; i++)
            {
                planes[i].enabled = pValue;
            }
            planeManager.planePrefab = pValue ? planePrefab : null;
        }
    }

}