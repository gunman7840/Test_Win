using UnityEngine;
using System.Collections;

public class TargetPoint
{
    public Vector2 position;
    public float distance;
    public int fails;
    public string edgeType;

    public TargetPoint(Vector2 _position, string _edgeType)
    {
        position = _position;
        edgeType = _edgeType;
    }
}