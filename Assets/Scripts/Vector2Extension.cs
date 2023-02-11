using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        Vector2 newVector = vector;

        float tx = newVector.x;
        float ty = newVector.y;
        newVector.x = (cos * tx) - (sin * ty);
        newVector.y = (sin * tx) + (cos * ty);
        return newVector;
    }
}