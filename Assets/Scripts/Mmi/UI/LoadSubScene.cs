using Unity.Entities;
using Unity.Scenes;
using UnityEngine.SceneManagement;

public class LoadSubScene : ComponentSystem {
    public static bool DisplaySubScene = false;
    public static LoadSubScene Instance;

    private SceneSystem sceneSystem;    
    private Hash128 guid;

    protected override void OnCreate()
    {
        Instance = this;
        sceneSystem = World.GetExistingSystem<SceneSystem>();
        guid = sceneSystem.GetSceneGUID("Assets//Scenes//Main//ConvertedSubScene.unity");
    }

    protected override void OnUpdate()
    {
        if (DisplaySubScene) {

            sceneSystem.LoadSceneAsync(guid);
        } else {
            // sceneSystem.UnloadScene(guid);
        }
    }
}