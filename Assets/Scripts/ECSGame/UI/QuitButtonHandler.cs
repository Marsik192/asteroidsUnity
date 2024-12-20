using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class QuitButtonHandler : MonoBehaviour
{
    //This is the UI Document from the Hierarchy in NavigationScene
    public UIDocument m_TitleUIDocument;
    private VisualElement m_titleScreenManagerVE;
    //Button we will set by querying the parent UI Document
    private Button m_QuitButton;

    void OnEnable()
    {
        //This will put callback on "Quit Game" button
        //This triggers the clean up function (ClickedQuitGame)
        m_titleScreenManagerVE = m_TitleUIDocument.rootVisualElement;
        m_titleScreenManagerVE.Q("quit-button")?.RegisterCallback<ClickEvent>(ev => ClickedQuit());

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ClickedQuit()
    {
        SceneManager.LoadScene("Main");
    }
}