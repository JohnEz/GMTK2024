using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Assertions;

public class TrainControllerTests {
    // General rules
    // 1 route has a duration of 1

    private const float BASE_TRACK_DURATION = 1;

    /// Adding Track
    //////////////////

    [Test]
    public void AddingTrackAtEndNotReturningNotLoop() {
        bool isAdding = true;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void AddingTrackAtStartNotReturningNotLoop() {
        bool isAdding = true;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.3f);
    }

    [Test]
    public void AddingTrackAtEndReturningNotLoop() {
        bool isAdding = true;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = 1.3f; // on return journey by .3f
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 3.3f);
    }

    [Test]
    public void AddingTrackAtStartReturningNotLoop() {
        bool isAdding = true;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = 1.3f; // on return journey by .3f
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 2.3f);
    }

    // Adding becoming Loop
    ////////////////////////
    [Test]
    public void AddingTrackAtEndNotReturningNowLoop() {
        bool isAdding = true;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void AddingTrackAtStartNotReturningNowLoop() {
        bool isAdding = true;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.3f);
    }

    [Test]
    public void AddingTrackAtEndReturningNowLoop() {
        bool isAdding = true;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = 1.3f; // on return journey by .3f
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .7f);
    }

    [Test]
    public void AddingTrackAtStartReturningNowLoop() {
        bool isAdding = true;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = 1.3f; // on return journey by .3f
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.7f);
    }

    // Adding becoming None Loop
    ////////////////////////

    [Test]
    public void AddingTrackAtEndWasLoop() {
        bool isAdding = true;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void AddingTrackAtStartWasLoop() {
        bool isAdding = true;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION;
        float initialElapsed = .3f; // not on return journey
        float newDuration = initialDuration + BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.3f);
    }

    /// Removing Track
    //////////////////

    // Not On Track
    //////////////////

    [Test]
    public void RemovingTrackAtEndNotReturningNotLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = .3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void RemovingTrackAtStartNotReturningNotLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 1.3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void RemovingTrackAtEndReturningNotLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 3.3f; // on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.3f);
    }

    [Test]
    public void RemovingTrackAtStartReturningNotLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 2.3f; // on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1.3f);
    }

    // On Track
    ///////////////

    [Test]
    public void RemovingTrackAtEndNotReturningOnTrackNotLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 1.3f; // not on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1);
    }

    [Test]
    public void RemovingTrackAtStartNotReturningOnTrackNotLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = .3f; // not on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 0f);
    }

    [Test]
    public void RemovingTrackAtEndReturningOnTrackNotLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 2.3f; // on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1f);
    }

    [Test]
    public void RemovingTrackAtStartReturningOnTrackNotLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 3.3f; // on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 0f);
    }

    /// Removing Track was Loop
    //////////////////

    // Not On Track
    //////////////////

    [Test]
    public void RemovingTrackAtEndWasLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = .3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void RemovingTrackAtStartWasLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 1.3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        // Scenario broken
        AssertFloatEqual(newElapsed, .3f);
    }

    // On Track
    ///////////////

    [Test]
    public void RemovingTrackAtEndOnTrackWasLoop() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 1.3f; // not on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 1);
    }

    [Test]
    public void RemovingTrackAtStartOnTrackWasLoop() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = .3f; // not on return journey, on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = true;
        bool isNowLooped = false;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, 0f);
    }

    // Removing Track is now loop
    ////////////////////////////////

    [Test]
    public void RemovingTrackAtEndNowLoopNotReturning() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = .3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void RemovingTrackAtStartNowLoopNotReturning() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 1.3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        // Scenario broken
        AssertFloatEqual(newElapsed, .3f);
    }

    [Test]
    public void RemovingTrackAtEndNowLoopReturning() {
        bool isAdding = false;
        bool isAtEnd = true;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 3.3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .7f);
    }

    [Test]
    public void RemovingTrackAtStartNowLoopReturning() {
        bool isAdding = false;
        bool isAtEnd = false;
        float initialDuration = BASE_TRACK_DURATION * 2;
        float initialElapsed = 2.3f; // not on return journey, not on track
        float newDuration = initialDuration - BASE_TRACK_DURATION;
        bool isPreviouslyLooped = false;
        bool isNowLooped = true;

        float newElapsed = TrainUtils.GetNewElapsedTime(isAdding, isAtEnd, initialDuration, initialElapsed, newDuration, isPreviouslyLooped, isNowLooped);

        AssertFloatEqual(newElapsed, .7f);
    }

    // Test Utils
    private void AssertFloatEqual(float a, float b) {
        UnityEngine.Assertions.Assert.AreApproximatelyEqual(a, b);
    }
}