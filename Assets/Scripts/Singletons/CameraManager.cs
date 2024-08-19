using UnityEngine;

public class CameraManager : Singleton<CameraManager> {
    private float MIN_ZOOM = 0.01f;
    private float DEFAULT_ZOOM = 8f;
    private float MAX_ZOOM = 200f;

    [SerializeField]
    private AudioClip _zoomOutSfx;

    [SerializeField]
    private AudioClip _zoomInSfx;

    [SerializeField]
    private CameraControlller _adultCameraZoom;

    [SerializeField]
    private CameraControlller _kidCameraZoom;

    private void Awake() {
        GameStateManager.Instance.OnStateChange += OnStateChange;
    }

    private void Start() {
        if (GameStateManager.Instance.State == GameState.Kid) {
            _kidCameraZoom.EnableCamera();
            _adultCameraZoom.DisableCamera();
        } else {
            _kidCameraZoom.DisableCamera();
            _adultCameraZoom.EnableCamera();
        }
    }

    private void OnStateChange(GameState newState) {
        if (newState == GameState.TransitionToWork) {
            ZoomOut();
        } else if (newState == GameState.TransitionToPlay) {
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

        ChrisMorrison.Instance.SetTargetBlur(2.5f, 0.2f);

        AudioManager.Instance.PlaySound(_zoomOutSfx);
    }

    private void handleZoomOutKidComplete() {
        _kidCameraZoom.onCompleteZoom -= handleZoomOutKidComplete;
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetZoom(DEFAULT_ZOOM, .2f);
        _adultCameraZoom.onCompleteZoom += handleZoomOutAdultComplete;
        ChrisMorrison.Instance.SetTargetBlur(0, 0.2f);

        ZoomManager.Instance.CompletedZoomOutKid();
    }

    private void handleZoomOutAdultComplete() {
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
        ChrisMorrison.Instance.SetTargetBlur(2.5f, 0.2f);

        AudioClipOptions options = new AudioClipOptions();
        options.Delay = 0.3f;
        AudioManager.Instance.PlaySound(_zoomInSfx, options);
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