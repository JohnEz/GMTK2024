using UnityEngine;

public class CameraManager : Singleton<CameraManager> {
    public const float MIN_ZOOM = 0.01f;
    public const float DEFAULT_ZOOM = 3f;
    public const float DEFAULT_ZOOM_2 = 5f;
    public const float DEFAULT_ZOOM_3 = 8f;
    public const float MAX_ZOOM = 200f;

    private float fadeTime = 2f;

    [SerializeField]
    private AudioClip _zoomOutSfx;

    [SerializeField]
    private AudioClip _zoomInSfx;

    [SerializeField]
    private AudioClip _kidMusic;

    [SerializeField]
    private AudioClip _adultMusic;

    [SerializeField]
    private CameraControlller _adultCameraZoom;

    [SerializeField]
    private CameraControlller _kidCameraZoom;

    [SerializeField]
    private UICanvasControlller _kidCanvas;

    [SerializeField]
    private UICanvasControlller _adultCanvas;

    private bool _isTransitioning = false;

    private float _currentDefaultZoom = DEFAULT_ZOOM;

    private void SetAudioOptions()
    {
        
    }

    private void Awake() {
        GameStateManager.Instance.OnStateChange += OnStateChange;
        SetAudioOptions();
    }

    private void Start() {
        AudioClipOptions options = new AudioClipOptions();

        options.Loop = true;
        options.Persist = true;

        if (GameStateManager.Instance.State == GameState.Kid) {
            _kidCameraZoom.EnableCamera();
            _adultCameraZoom.DisableCamera();
            AudioManager.Instance.PlaySound(_kidMusic, options);
        }
        else {
            _kidCameraZoom.DisableCamera();
            _adultCameraZoom.EnableCamera();
            AudioManager.Instance.PlaySound(_adultMusic, options);
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
        _isTransitioning = true;

        StartCoroutine(AudioManager.Instance.FadeOut(
                _kidMusic, fadeTime));
        StartCoroutine(AudioManager.Instance.FadeIn(
            _adultMusic, fadeTime, 1f));

        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetInstantZoom(_currentDefaultZoom);
        _adultCameraZoom.SetInstantZoom(MIN_ZOOM);
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.SetZoom(MAX_ZOOM, 0.2f);
        _kidCameraZoom.onCompleteZoom += handleZoomOutKidComplete;

        ChrisMorrison.Instance.SetTargetBlur(2.5f, 0.2f);

        _kidCanvas.HideUI();

        AudioManager.Instance.PlaySound(_zoomOutSfx);
    }

    private void handleZoomOutKidComplete() {
        _kidCameraZoom.onCompleteZoom -= handleZoomOutKidComplete;
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetZoom(_currentDefaultZoom, .2f);
        _adultCameraZoom.onCompleteZoom += handleZoomOutAdultComplete;
        ChrisMorrison.Instance.SetTargetBlur(0, 0.2f);

        ZoomManager.Instance.CompletedZoomOutKid();
    }

    private void handleZoomOutAdultComplete() {
        _isTransitioning = false;

        _adultCameraZoom.onCompleteZoom -= handleZoomOutAdultComplete;

        _adultCanvas.ShowUI();

        GameStateManager.Instance.TrySetState(GameState.Adult);
    }

    public void ZoomIn() {
        _isTransitioning = true;

        StartCoroutine(AudioManager.Instance.FadeOut(
                _adultMusic, fadeTime));
        StartCoroutine(AudioManager.Instance.FadeIn(
            _kidMusic, fadeTime, 1f));

        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetInstantZoom(_currentDefaultZoom);
        _kidCameraZoom.SetInstantZoom(MAX_ZOOM);
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.SetZoom(MIN_ZOOM, 0.2f);
        _adultCameraZoom.onCompleteZoom += handleZoomInAdultComplete;

        ChrisMorrison.Instance.SetTargetBlur(2.5f, 0.2f);

        _adultCanvas.HideUI();

        AudioClipOptions options = new AudioClipOptions();
        options.Delay = 0.3f;
        AudioManager.Instance.PlaySound(_zoomInSfx, options);
    }

    private void handleZoomInAdultComplete() {
        _adultCameraZoom.onCompleteZoom -= handleZoomInAdultComplete;
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetZoom(_currentDefaultZoom, 0.2f);
        _kidCameraZoom.onCompleteZoom += handleZoomInKidComplete;
        ChrisMorrison.Instance.SetTargetBlur(0, 0.2f);

        ZoomManager.Instance.CompletedZoomInAdult();
    }

    private void handleZoomInKidComplete() {
        _isTransitioning = false;

        _kidCameraZoom.onCompleteZoom -= handleZoomInKidComplete;

        _kidCanvas.ShowUI();

        GameStateManager.Instance.TrySetState(GameState.Kid);
    }

    public void UpdateCameraDefaultZoom(float newDefaultZoom) {
        _currentDefaultZoom = newDefaultZoom;

        if (!_isTransitioning) {
            _kidCameraZoom.SetZoom(_currentDefaultZoom, 0.75f);
            _adultCameraZoom.SetZoom(_currentDefaultZoom, 0.75f);
        }
    }
}