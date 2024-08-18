using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlller : MonoBehaviour {
    private float _targetZoom;
    private float _velocity = 0f;
    private float _smoothTime = 0.2f;

    private CinemachineVirtualCamera cam;

    private bool _isTransitioning = true;

    public event Action onCompleteZoom;

    private void Awake() {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start() {
        _targetZoom = cam.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    private void Update() {
        if (_isTransitioning) {
            cam.m_Lens.OrthographicSize = Mathf.SmoothDamp(cam.m_Lens.OrthographicSize, _targetZoom, ref _velocity, _smoothTime);
        }

        if (_isTransitioning && Mathf.Abs(Camera.main.orthographicSize - _targetZoom) < 0.05f) {
            cam.m_Lens.OrthographicSize = _targetZoom;
            _isTransitioning = false;
            onCompleteZoom?.Invoke();
        }
    }

    public void SetZoom(float targetZoom) {
        _targetZoom = targetZoom;
        _isTransitioning = true;
    }

    public void SetInstantZoom(float zoom) {
        cam.m_Lens.OrthographicSize = zoom;
        _targetZoom = zoom;
    }

    public void EnableCamera() {
        cam.Priority = 10;
    }

    public void DisableCamera() {
        cam.Priority = 0;
    }
}