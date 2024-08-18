using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrainController : MonoBehaviour {
    private SplineAnimate _splineAnimate;

    private bool _isFollowingTrack = false;

    private OSLineController _line;

    private void Awake() {
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    private void OnEnable() {
        _splineAnimate.Updated += onSplineUpdate;
    }

    public void SetLine(OSLineController lineController) {
        if (_line != null) {
            _line.OnChange -= SetupSplineAnimate;
        }

        _line = lineController;

        SetupSplineAnimate();
        _line.OnChange += SetupSplineAnimate;
    }

    private void SetupSplineAnimate() {
        if (_line == null || _line.SplineContainer.Spline.Count == 0) {
            if (_splineAnimate != null) {
                _splineAnimate.Container = null;
            }
            return;
        }

        _splineAnimate.Container = _line.SplineContainer;
        _splineAnimate.Loop = _line.Line.IsLoop ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.PingPong;
        _splineAnimate.Restart(true);
    }

    private void OnDisable() {
        _splineAnimate.Updated -= onSplineUpdate;
    }

    private void onSplineUpdate(Vector3 arg1, Quaternion arg2) {
        if (!_splineAnimate.IsPlaying) {
            if (_isFollowingTrack) {
                HandleCompletedSpline();
            }
        } else {
            _isFollowingTrack = true;
        }
    }

    private void HandleCompletedSpline() {
        _isFollowingTrack = false;
    }
}