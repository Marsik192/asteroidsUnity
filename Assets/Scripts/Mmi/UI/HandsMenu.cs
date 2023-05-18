using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Mmi.UI
{

    public class HandsMenu : MonoBehaviour
    {
        public Toggle toggleMock;
        public Toggle toggleXRIO;
        public Toggle toggleOff;

        private void Start() {
        }

        private void OnEnable()
        {
            toggleOff.isOn = true;
            SetEvents();
        }

        private void OnDisable() {
            ClearEvents();
        }

        private void SetEvents()
        {
            toggleMock.onValueChanged.AddListener(OnToggleMock);
            toggleXRIO.onValueChanged.AddListener(OnToggleXRIO);
            toggleOff.onValueChanged.AddListener(OnToggleOff);
        }
        private void ClearEvents()
        {
            toggleMock.onValueChanged.RemoveListener(OnToggleMock);
            toggleXRIO.onValueChanged.RemoveListener(OnToggleXRIO);
            toggleOff.onValueChanged.RemoveListener(OnToggleOff);
        }
        private void OnToggleMock(bool pValue)
        {
            if (pValue)
                Debug.Log("OnToggleMock()");
        }
        private void OnToggleXRIO(bool pValue)
        {
            if (pValue)
                Debug.Log("OnToggleXRIO()");
        }
        private void OnToggleOff(bool pValue)
        {
            if (pValue)
                Debug.Log("OnToggleOff()");
        }
    }

}