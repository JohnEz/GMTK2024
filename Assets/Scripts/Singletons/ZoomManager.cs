using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoomManager : Singleton<ZoomManager>
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SwitchScene();
        }
    }

    private void SwitchScene()
    {
        if (GameStateManager.Instance.State == GameState.Kid) {
            GameStateManager.Instance.TrySetState(GameState.TransitionToWork);
        }
        else if (GameStateManager.Instance.State == GameState.Adult)
        {
            GameStateManager.Instance.TrySetState(GameState.TransitionToPlay);
        }
    }

    public void CompletedZoomOutKid()
    {
        Debug.Log("Finished zooming out of kid scene");
        UnloadSceneOptions unloadSceneOptions = new UnloadSceneOptions();
        SceneManager.UnloadSceneAsync("toy", unloadSceneOptions);

        LoadSceneParameters loadSceneParameters = new LoadSceneParameters();
        loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;
        SceneManager.LoadSceneAsync("os", loadSceneParameters);
    }

    public void CompletedZoomInAdult()
    {
        Debug.Log("Finished zooming in of adult scene");
        UnloadSceneOptions unloadSceneOptions = new UnloadSceneOptions();
        SceneManager.UnloadSceneAsync("os", unloadSceneOptions);

        LoadSceneParameters loadSceneParameters = new LoadSceneParameters();
        loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;
        SceneManager.LoadSceneAsync("toy", loadSceneParameters);
    }
}