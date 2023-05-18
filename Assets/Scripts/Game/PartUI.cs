using UnityEngine;
using TMPro;

namespace Game
{

    public class PartUI : MonoBehaviour
    {
        // Editor Fields
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;

        // Variables
        private ARPart part;
        private Vector3 worldPosition;

        public void Set(ARPart pPart, Vector3 pWorldPosition)
        {
            part = pPart;
            worldPosition = pWorldPosition;
            title.text = part.definition.title;
            description.text = part.definition.description;
            gameObject.SetActive(true);
            Position();
        }

        private void Update() {
            if (part != null)
            {
                Position();
            }
        }

        private void Position()
        {
            Vector3 position = GameManager.instance.sessionOrigin.camera.WorldToScreenPoint(worldPosition);
            position = Mmi.UI.MenuManager.instance.camera.ScreenToWorldPoint(position);
            position.z = 0;
            transform.position = position;
        }

        public void Hide()
        {
            part = null;
            gameObject.SetActive(false);
        }

        public void OnClose()
        {
            GameManager.instance.DeselectPart();
            Hide();
        }

        public void OnHidePart()
        {
            GameManager.instance.HideSelectedPart();
            Hide();
        }
    }
    

}