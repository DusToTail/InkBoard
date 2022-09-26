using System;
using UnityEngine;

public static class BezierCurveUtilities
{
    public static Vector3 LerpPosition(Transform[] points, float t)
    {
        if (points == null) { return Vector3.zero; }
        if (points.Length == 1) { return points[0].position; }
        if (points.Length == 2) { return Vector3.Lerp(points[0].position, points[1].position, t); }
        Vector3[] positions = Array.ConvertAll(points, item => item.position);
        return LerpPosition(positions, t);
    }

    public static Vector3 LerpPosition(Vector3[] points, float t)
    {
        if (points == null) { return Vector3.zero; }
        if(points.Length == 1) { return points[0]; }
        if(points.Length == 2) { return Vector3.Lerp(points[0], points[1], t); }
        Vector3[] newPoints = new Vector3[points.Length - 1];
        for(int i = 0; i < points.Length - 1; i++)
        {
            newPoints[i] = Vector3.Lerp(points[i], points[i+1], t);
        }
        return LerpPosition(newPoints, t);
    }

    public static Quaternion LerpRotation(Transform[] points, float t)
    {
        if (points == null) { return Quaternion.identity; }
        if (points.Length == 1) { return points[0].rotation; }
        if (points.Length == 2) { return Quaternion.Lerp(points[0].rotation, points[1].rotation, t); }
        Quaternion[] rotations = Array.ConvertAll(points, item => item.rotation);
        return LerpRotation(rotations, t);
    }

    public static Quaternion LerpRotation(Quaternion[] points, float t)
    {
        if (points == null) { return Quaternion.identity; }
        if (points.Length == 1) { return points[0]; }
        if (points.Length == 2) { return Quaternion.Lerp(points[0], points[1], t); }
        Quaternion[] newPoints = new Quaternion[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            newPoints[i] = Quaternion.Lerp(points[i], points[i + 1], t);
        }
        return LerpRotation(newPoints, t);
    }
}
