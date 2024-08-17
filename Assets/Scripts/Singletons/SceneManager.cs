using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Level {
    Kid,
    Adult
}

public class SceneManager : Singleton<SceneManager> {
    private bool _isTransitioning = false;

    public Level Level { get; private set; }

    private void Update() {
        if (Input.GetKey(KeyCode.Space) && !_isTransitioning) {
            SwitchScene();
        }
    }

    private void SwitchScene() {
        if (_isTransitioning) {
            return;
        }

        if (Level == Level.Kid) {
            ZoomOut();
        } else {
            ZoomIn();
        }
    }

    private void ZoomOut() {
        if (Level == Level.Adult) {
            return;
        }

        _isTransitioning = true;

        CameraManager.Instance.ZoomOut();
    }

    public void CompletedZoomOutKid() {
        Debug.Log("Finished zooming out of kid scene");
    }

    public void CompletedZoomOutAdult() {
        Debug.Log("Finished zooming out of adult scene");
        _isTransitioning = false;
        Level = Level.Adult;
    }

    private void ZoomIn() {
        if (Level == Level.Kid) {
            return;
        }

        _isTransitioning = true;

        CameraManager.Instance.ZoomIn();
    }

    public void CompletedZoomInAdult() {
        Debug.Log("Finished zooming in of adult scene");
    }

    public void CompletedZoomInKid() {
        Debug.Log("Finished zooming in of kid scene");
        _isTransitioning = false;
        Level = Level.Kid;
    }
}