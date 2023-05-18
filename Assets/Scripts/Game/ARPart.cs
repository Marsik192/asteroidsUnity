using UnityEngine;

namespace Game
{

    public class ARPart : MonoBehaviour
    {

        public ARPartDefinition definition;

        private void Awake() {
            ARPartManager.Add(this);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
    

}