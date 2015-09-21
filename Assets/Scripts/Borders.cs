using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Borders : MonoBehaviour
{

    // The Collider itself
    private EdgeCollider2D thisCollider;
    // array of collider points
    private Vector2[] points;
    // the transform position of the collider
    private Vector3 _t;

    void Start()
    {
        thisCollider = GetComponent("EdgeCollider2D") as EdgeCollider2D;
        points = thisCollider.points;
        _t = thisCollider.transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // for every point (except for the last one), draw line to the next point
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(new Vector3(points[i].x + _t.x, points[i].y + _t.y), new Vector3(points[i + 1].x + _t.x, points[i + 1].y + _t.y));
        }
    }

}