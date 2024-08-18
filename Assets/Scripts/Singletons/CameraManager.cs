using UnityEngine;

public class CameraManager : Singleton<CameraManager> {
    private float MIN_ZOOM = 0.1f;
    private float DEFAULT_ZOOM = 3f;
    private float MAX_ZOOM = 200f;

    [SerializeField]
    private CameraControlller _adultCameraZoom;

    [SerializeField]
    private CameraControlller _kidCameraZoom;

    void Awake()
    {
        GameStateManager.Instance.OnStateChange.AddListener(OnStateChange);
    }

    private void OnStateChange(GameState newState)
    {
        if (newState == GameState.TransitionToWork)
        {
            ZoomOut();
        }
        else if (newState == GameState.TransitionToPlay)
        {
            ZoomIn();
        }
    }

    public void ZoomOut() {
        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetInstantZoom(DEFAULT_ZOOM);
        _adultCameraZoom.SetInstantZoom(MIN_ZOOM);
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.SetZoom(MAX_ZOOM, 0.2f);
        _kidCameraZoom.onCompleteZoom += handleZoomOutKidComplete;
        ChrisMorrison.Instance.SetTargetBlur(10f, 0.2f);
    }

    private void handleZoomOutKidComplete() {
        Debug.Log("Finished zooming out of kid");

        _kidCameraZoom.onCompleteZoom -= handleZoomOutKidComplete;
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetZoom(DEFAULT_ZOOM, .2f);
        _adultCameraZoom.onCompleteZoom += handleZoomOutAdultComplete;
        ChrisMorrison.Instance.SetTargetBlur(0, 0.2f);

        ZoomManager.Instance.CompletedZoomOutKid();
    }

    private void handleZoomOutAdultComplete() {
        Debug.Log("Finished zooming out of adult");

        _adultCameraZoom.onCompleteZoom -= handleZoomOutAdultComplete;

        GameStateManager.Instance.TrySetState(GameState.Adult);
    }

    public void ZoomIn() {
        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetInstantZoom(DEFAULT_ZOOM);
        _kidCameraZoom.SetInstantZoom(MAX_ZOOM);
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.SetZoom(MIN_ZOOM, 0.2f);
        _adultCameraZoom.onCompleteZoom += handleZoomInAdultComplete;
        ChrisMorrison.Instance.SetTargetBlur(10f, 0.2f);
    }

    private void handleZoomInAdultComplete() {
        Debug.Log("Finished zooming in of adult");

        _adultCameraZoom.onCompleteZoom -= handleZoomInAdultComplete;
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetZoom(DEFAULT_ZOOM, 0.2f);
        _kidCameraZoom.onCompleteZoom += handleZoomInKidComplete;
        ChrisMorrison.Instance.SetTargetBlur(0, 0.2f);

        ZoomManager.Instance.CompletedZoomInAdult();
    }

    private void handleZoomInKidComplete() {
        Debug.Log("Finished zooming in of kid");

        _kidCameraZoom.onCompleteZoom -= handleZoomInKidComplete;

        GameStateManager.Instance.TrySetState(GameState.Kid);
    }
}