using UnityEngine;

public class GlobalMethods
{
    public static float RoundToNearestPI(float angleInRadians)
    {
        // Convert the angle to degrees for easier comparison
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        // Calculate the remainder when divided by 90 degrees
        float remainder = angleInDegrees % 90f;

        // Round to the nearest multiple of 90 degrees
        float roundedAngleInDegrees = Mathf.Round(angleInDegrees / 90f) * 90f;

        // Convert back to radians
        float roundedAngleInRadians = roundedAngleInDegrees * Mathf.Deg2Rad;

        return roundedAngleInRadians;
    }

}
