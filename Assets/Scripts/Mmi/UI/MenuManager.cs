using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mmi.UI
{

    public class MenuManager : MonoBehaviour
    {
        public enum Menus {
            MainMenu = 0,
            SettingsMenu = 1,
            SpectatorMenu = 2,
            HandsMenu = 3,
            PoseMenu = 4,
            VoiceMenu = 5,
            ControllerMenu = 6,
            SlamMenu = 7,
            GameMenu = 8,
            
        }

        public static MenuManager instance;

        public GameObject[] menus;
        public GameObject MenuButton;
        public GameObject ExitCreateAnchorModeButton;
        public Camera camera;

        private void Awake()
        {
            instance = this;
            HideAllMenus();
            ExitCreateAnchorModeButton.SetActive(false);
        }

        public void ShowMenu(Menus pMenu)
        {
            for (int i=0; i<menus.Length; i++)
            {
                menus[i].SetActive(i == (int)pMenu);
            }
        }

        public void HideAllMenus()
        {
            for (int i=0; i<menus.Length; i++)
            {
                menus[i].SetActive(false);
            }
        }

        public bool IsShowing()
        {
            bool isShowing = false;
            for (int i=0; i<menus.Length; i++)
            {
                if (menus[i].activeSelf)
                {
                    isShowing = true;
                    break;
                }
            }

            return isShowing;
            //return isShowing || Game.GameManager.instance.partUI.gameObject.activeSelf;
        }

        public void OnExitCreateAnchorModeButtonClicked()
        {
            XR.CloudAnchors.CloudAnchorsController.instance.ExitCreateAnchorMode();
        }

        public void HideMenuButton()
        {
            MenuButton.SetActive(false);
        }

        public void ShowMenuButton()
        {
            MenuButton.SetActive(true);
        }

        public void OnMainMenu()
        {
            ShowMenu(Menus.MainMenu);
        }
        public void OnSettingsMenu()
        {
            ShowMenu(Menus.SettingsMenu);
        }
        public void OnHandsMenu()
        {
            ShowMenu(Menus.HandsMenu);
        }
        public void OnPoseMenu()
        {
            ShowMenu(Menus.PoseMenu);
        }
        public void OnVoiceMenu()
        {
            ShowMenu(Menus.VoiceMenu);
        }
        public void OnControllerMenu()
        {
            ShowMenu(Menus.ControllerMenu);
        }
        public void OnSlamMenu()
        {
            ShowMenu(Menus.SlamMenu);
        }
        public void OnSpectatorMenu()
        {
            ShowMenu(Menus.SpectatorMenu);
        }
        public void OnGameMenu()
        {
            ShowMenu(Menus.GameMenu);
        }
        public void OnCloseAllMenus()
        {
            HideAllMenus();
        }

        public void LoadSubSceneECSGame()
        {
            SceneManager.LoadScene("NavigationScene");
        }

        public void UnloadSubSceneECSGame()
        {
            LoadSubScene.DisplaySubScene = false;
        }
    }

}