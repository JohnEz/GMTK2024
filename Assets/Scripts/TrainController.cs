using UnityEngine;
using UnityEngine.Splines;

public enum TrainVisual {
    OS,
    Toy
}

public class TrainController : MonoBehaviour {
    private SplineAnimate _splineAnimate;

    [SerializeField]
    private Sprite _toyTrainSprite;

    public LineController Line { get; private set; }

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private void Awake() {
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    private void OnEnable() {
        _splineAnimate.Updated += onSplineUpdate;
    }

    public void SetTrainVisual(TrainVisual trainVisual) {
        if (trainVisual == TrainVisual.Toy) {
            _spriteRenderer.sprite = _toyTrainSprite;
        }
    }

    public void SetLine(LineController lineController) {
        if (Line != null) {
            Line.OnChange -= (bool a, bool b) => SetupSplineAnimate(a, b, false);
        }

        Line = lineController;
        _spriteRenderer.color = Line.Line.Color;

        SetupSplineAnimate(true, true, true);
        Line.OnChange += (bool a, bool b) => SetupSplineAnimate(a, b, false);

        _splineAnimate.Restart(true);
    }

    private void Update() {
        // maybe should only do this if i need to?
        _splineAnimate.ElapsedTime = _splineAnimate.ElapsedTime % (_splineAnimate.Duration * 2);

        if (_splineAnimate.ElapsedTime > _splineAnimate.Duration) {
            _splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.NegativeYAxis;
        } else {
            _splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.YAxis;
        }
    }

    private void SetupSplineAnimate(bool isAddition, bool isAtEnd, bool isForced) {
        if (Line == null || Line.SplineContainer.Spline.Count == 0) {
            if (_splineAnimate != null) {
                _splineAnimate.Container = null;
            }
            return;
        }

        float initialDuration = _splineAnimate.Duration;
        // TODO its only *2 if its pingpong i think
        float initialElapsed = _splineAnimate.ElapsedTime % (initialDuration * 2);

        _splineAnimate.Container = Line.SplineContainer;
        _splineAnimate.Loop = Line.Line.IsLoop ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.PingPong;

        bool isOnReturnJourney = initialElapsed > initialDuration;
        float durationOfAddedOrRemovedRoute = Mathf.Abs(_splineAnimate.Duration - initialDuration);

        bool isOnRemovedTrack = false;

        // calculate if the train was on the removed track
        if (!isAddition) {
            if (isAtEnd) {
                // check if on middle segments
                // find the end of the first half minus the removedDuration
                float startOfRemovedRoute = Mathf.Abs(initialDuration - durationOfAddedOrRemovedRoute);
                float endOfRemovedRoute = startOfRemovedRoute + (durationOfAddedOrRemovedRoute * 2);

                // endOfRemovedRoute - return journey was bugged!!!

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
    }
}