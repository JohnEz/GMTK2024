using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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
            _line.OnChange -= (bool a, bool b) => SetupSplineAnimate(a, b, false);
        }

        _line = lineController;

        SetupSplineAnimate(true, true, true);
        _line.OnChange += (bool a, bool b) => SetupSplineAnimate(a, b, false);

        _splineAnimate.Restart(true);
    }

    private void SetupSplineAnimate(bool isAddition, bool isAtEnd, bool isForced) {
        if (_line == null || _line.SplineContainer.Spline.Count == 0) {
            if (_splineAnimate != null) {
                _splineAnimate.Container = null;
            }
            return;
        }

        float initialDuration = _splineAnimate.Duration;
        // TODO its only *2 if its pingpong i think
        float initialElapsed = _splineAnimate.ElapsedTime % (initialDuration * 2);

        _splineAnimate.Container = _line.SplineContainer;
        _splineAnimate.Loop = _line.Line.IsLoop ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.PingPong;

        bool isOnReturnJourney = initialElapsed > initialDuration;
        float durationOfAddedOrRemovedRoute = Mathf.Abs(_splineAnimate.Duration - initialDuration);

        bool isOnRemovedTrack = false;

        // calculate if the train was on the removed track
        if (!isAddition) {
            if (isAtEnd) {
                // check if on middle segments
                // find the end of the first half minus the removedDuration
                float startOfRemovedRoute = Mathf.Abs(initialDuration - durationOfAddedOrRemovedRoute);
                float endOfRemovedRoute = startOfRemovedRoute + durationOfAddedOrRemovedRoute;

                isOnRemovedTrack = initialElapsed > startOfRemovedRoute && initialElapsed < endOfRemovedRoute;
            } else {
                // check if on first or last segment
                bool isOnFirstSegment = initialElapsed < durationOfAddedOrRemovedRoute;
                bool isOnLastSegment = initialElapsed > (initialDuration * 2) - durationOfAddedOrRemovedRoute;

                isOnRemovedTrack = isOnFirstSegment || isOnLastSegment;
            }
        }

        if (isOnRemovedTrack) {
            _splineAnimate.ElapsedTime = isAtEnd ? _splineAnimate.Duration : 0;

            return;
        }

        int timesPassed = 0;
        if (!isAtEnd) {
            timesPassed = 1;
        } else if (isAtEnd && isOnReturnJourney) {
            timesPassed = 2;
        }

        float newElapsedTime;

        if (isAddition) {
            newElapsedTime = initialElapsed + (durationOfAddedOrRemovedRoute * timesPassed);
        } else {
            // track removed
            newElapsedTime = initialElapsed - (durationOfAddedOrRemovedRoute * timesPassed);
        }

        _splineAnimate.ElapsedTime = newElapsedTime;
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