using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class CameraManager : Singleton<CameraManager> {
    private bool _isTransitioning = false;

    [SerializeField]
    private CameraControlller _adultCameraZoom;

    [SerializeField]
    private CameraControlller _kidCameraZoom;

    public void ZoomOut() {
        if (SceneManager.Instance.Level == Level.Adult || _isTransitioning) {
            return;
        }

        _isTransitioning = true;

        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetInstantZoom(3f);
        _adultCameraZoom.SetInstantZoom(.01f);
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.SetZoom(60f);
        _kidCameraZoom.onCompleteZoom += handleZoomOutKidComplete;
    }

    private void handleZoomOutKidComplete() {
        Debug.Log("Finished zooming out of kid");

        _kidCameraZoom.onCompleteZoom -= handleZoomOutKidComplete;
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetZoom(3f);
        _adultCameraZoom.onCompleteZoom += handleZoomOutAdultComplete;

        SceneManager.Instance.CompletedZoomOutKid();
    }

    private void handleZoomOutAdultComplete() {
        Debug.Log("Finished zooming out of adult");

        _adultCameraZoom.onCompleteZoom -= handleZoomOutAdultComplete;
        _isTransitioning = false;

        SceneManager.Instance.CompletedZoomOutAdult();
    }

    public void ZoomIn() {
        if (SceneManager.Instance.Level == Level.Kid || _isTransitioning) {
            return;
        }

        _isTransitioning = true;

        _adultCameraZoom.EnableCamera();
        _adultCameraZoom.SetInstantZoom(3f);
        _kidCameraZoom.SetInstantZoom(60f);
        _kidCameraZoom.DisableCamera();
        _adultCameraZoom.SetZoom(.01f);
        _adultCameraZoom.onCompleteZoom += handleZoomInAdultComplete;
    }

    private void handleZoomInAdultComplete() {
        Debug.Log("Finished zooming in of adult");

        _adultCameraZoom.onCompleteZoom -= handleZoomInAdultComplete;
        _adultCameraZoom.DisableCamera();
        _kidCameraZoom.EnableCamera();
        _kidCameraZoom.SetZoom(3f);
        _kidCameraZoom.onCompleteZoom += handleZoomInKidComplete;

        SceneManager.Instance.CompletedZoomInAdult();
    }

    private void handleZoomInKidComplete() {
        Debug.Log("Finished zooming in of kid");

        _kidCameraZoom.onCompleteZoom -= handleZoomInKidComplete;
        _isTransitioning = false;

        SceneManager.Instance.CompletedZoomInKid();
    }
}