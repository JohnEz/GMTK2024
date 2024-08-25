using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainUtils {

    public static int DurationMod(bool isLoop) {
        return isLoop ? 1 : 2;
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