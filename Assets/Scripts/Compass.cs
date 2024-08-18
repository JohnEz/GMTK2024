using UnityEngine;

public enum Compass
{
    // order's important - see below methods
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

    public static Rotation ToRotation(this Compass direction) {
        return direction switch
        {
            Compass.North => Rotation.None,
            Compass.East => Rotation.Deg90,
            Compass.South => Rotation.Deg180,
            Compass.West => Rotation.Deg270,
            _ => throw new System.Exception("Compass didn't have a value"),
        };
    }

    public static Compass Reversed(this Compass orig) {
        switch (orig) {
            case Compass.North: return Compass.South;
            case Compass.East: return Compass.West;
            case Compass.South: return Compass.North;
            case Compass.West: return Compass.East;
            default:
                Debug.Log("invalid compass");
                return Compass.North;
        }
    }
}
