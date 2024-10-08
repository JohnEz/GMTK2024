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

    private bool _isPreviousSplineLoop = false;

    private void Awake() {
        _splineAnimate = GetComponent<SplineAnimate>();
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
        _splineAnimate.ElapsedTime = _splineAnimate.ElapsedTime % (_splineAnimate.Duration * CurrentDurationMod());

        if (_splineAnimate.ElapsedTime > _splineAnimate.Duration && !Line.Line.IsLoop()) {
            _splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.NegativeYAxis;
        } else {
            _splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.YAxis;
        }
    }

    public static int DurationMod(bool isLoop) {
        return isLoop ? 1 : 2;
    }

    private int CurrentDurationMod() {
        return DurationMod(Line.Line.IsLoop());
    }

    private int PreviousDurationMod() {
        return DurationMod(_isPreviousSplineLoop);
    }

    private void SetupSplineAnimate(bool isAddition, bool isAtEnd, bool isForced) {
        if (Line == null || Line.SplineContainer.Spline.Count == 0) {
            if (_splineAnimate != null) {
                _splineAnimate.Container = null;
            }
            return;
        }

        float initialDuration = _splineAnimate.Duration;
        float initialElapsed = _splineAnimate.ElapsedTime;

        _splineAnimate.Container = Line.SplineContainer;
        _splineAnimate.Loop = Line.Line.IsLoop() ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.PingPong;

        float newElapsedTime = GetNewElapsedTime(isAddition, isAtEnd, initialDuration, initialElapsed, _splineAnimate.Duration, _isPreviousSplineLoop, Line.Line.IsLoop());

        _splineAnimate.ElapsedTime = newElapsedTime;
        _isPreviousSplineLoop = Line.Line.IsLoop();
    }

    public static float GetNewElapsedTime(bool isAddition, bool isAtEnd, float initialDuration, float initialElapsed, float newDuration, bool isPreviouslyLooped, bool isNowLooped) {
        bool isAtStart = !isAtEnd;

        bool isOnReturnJourney = initialElapsed > initialDuration;
        float durationOfAddedOrRemovedRoute = Mathf.Abs(newDuration - initialDuration);

        bool isOnRemovedTrack = false;

        // calculate if the train was on the removed track
        if (!isAddition) {
            if (isAtEnd) {
                // find the end of the first half minus the removedDuration
                float startOfRemovedRoute = Mathf.Abs(initialDuration - durationOfAddedOrRemovedRoute);
                float endOfRemovedRoute = startOfRemovedRoute + (durationOfAddedOrRemovedRoute * DurationMod(isPreviouslyLooped));

                isOnRemovedTrack = initialElapsed > startOfRemovedRoute && initialElapsed < endOfRemovedRoute;
            } else {
                // check if on first or last segment
                bool isOnFirstSegment = initialElapsed < durationOfAddedOrRemovedRoute;
                bool isOnLastSegment = !isPreviouslyLooped && initialElapsed > (initialDuration * DurationMod(isPreviouslyLooped)) - durationOfAddedOrRemovedRoute;

                isOnRemovedTrack = isOnFirstSegment || isOnLastSegment;
            }
        }

        if (isOnRemovedTrack) {
            // if its on the removed track, place it at the end or start station
            return isAtEnd ? newDuration : 0;
        }

        int timesPassed = 0;
        if (!isAtEnd) {
            timesPassed = 1;
        } else if (isAtEnd && isOnReturnJourney) {
            timesPassed = 2;
        }

        float newElapsedTime;

        if (isPreviouslyLooped != isNowLooped && isNowLooped) {
            if (isAtStart) {
                if (isOnReturnJourney) {
                    newElapsedTime = newDuration - (initialElapsed - initialDuration);
                } else {
                    if (isAddition) {
                        newElapsedTime = initialElapsed + durationOfAddedOrRemovedRoute;
                    } else {
                        newElapsedTime = initialElapsed - durationOfAddedOrRemovedRoute;
                    }
                }
            } else {
                if (isOnReturnJourney) {
                    newElapsedTime = (initialDuration * 2) - initialElapsed;
                } else {
                    newElapsedTime = initialElapsed;
                }
            }
        } else {
            // if it now not a loop or never was
            if (isAddition) {
                newElapsedTime = initialElapsed + (durationOfAddedOrRemovedRoute * timesPassed);
            } else {
                // track removed
                newElapsedTime = initialElapsed - (durationOfAddedOrRemovedRoute * timesPassed);
            }
        }

        return newElapsedTime;
    }
}