using UnityEngine;
using UnityEngine.SceneManagement;

public class ZoomManager : Singleton<ZoomManager> {

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            SwitchScene();
        }
    }

    private void SwitchScene() {
        if (GameStateManager.Instance.State == GameState.Kid) {
            GameStateManager.Instance.TrySetState(GameState.TransitionToWork);
        } else if (GameStateManager.Instance.State == GameState.Adult) {
            GameStateManager.Instance.TrySetState(GameState.TransitionToPlay);
        }
    }

    public void CompletedZoomOutKid() {
    }

    public void CompletedZoomInAdult() {
    }
}