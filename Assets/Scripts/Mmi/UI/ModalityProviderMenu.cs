using UnityEngine;
using UnityEngine.UI;

namespace Mmi.UI
{

    public class ModalityProviderMenu : MonoBehaviour
    {
        public Toggle[] toggles;

        private void Start() {
        }

        private void OnEnable()
        {
            if (toggles.Length > 0)
            {
                toggles[0].isOn = true;
            }
            SetEvents();
        }

        private void OnDisable() {
            ClearEvents();
        }

        private void SetEvents()
        {
        }
        private void ClearEvents()
        {
        }
    }

}