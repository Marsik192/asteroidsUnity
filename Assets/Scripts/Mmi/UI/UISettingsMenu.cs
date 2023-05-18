using UnityEngine;
using UnityEngine.UI;

namespace Mmi.UI
{

    public class UISettingsMenu : MonoBehaviour
    {
        public Toggle toggleDebugManager;

        private DebugManager debugManager;

        private void Start() {
        }

        private void OnEnable()
        {

            debugManager = FindObjectOfType<DebugManager>();

            if (debugManager)
            {
                toggleDebugManager.isOn = debugManager.gameObject.activeSelf;
            }

            SetEvents();
        }

        private void OnDisable() {
            debugManager = null;

            ClearEvents();
        }

        private void SetEvents()
        {
            toggleDebugManager.onValueChanged.AddListener(OnToggleDebugManager);
        }

        private void ClearEvents()
        {
            toggleDebugManager.onValueChanged.RemoveListener(OnToggleDebugManager);
        }

        private void OnToggleDebugManager(bool pValue)
        {
            debugManager.gameObject.SetActive(pValue);
        }
    }

}