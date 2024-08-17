using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Compass
{
    // order's important
    North,
    East,
    South,
    West,
}

public static class CompassExtensions {
    public static Compass Rotate(this Compass orig, Rotation rot) {
        var add = 0;

        switch (rot) {
            case Rotation.None:
                // duh
                break;

            case Rotation.Deg90:
                add = 1;
                break;

            case Rotation.Deg180:
                add = 2;
                break;

            case Rotation.Deg270:
                add = 3;
                break;
        }

        var newVal = (int)orig + add;
        newVal %= 4;
        return (Compass)newVal;
    }
}
