using UnityEngine;
using System.Collections;


public class Platform : MonoBehaviour {

	// Use this for initialization
    private Rigidbody2D body;
    private Vector2 StartPosition;
    private int direction;



	void Start () 
    {
        body=GetComponent<Rigidbody2D>();
        StartPosition = body.position;


	}

    void FixedUpdate()
    {

        if (body.position.x <= StartPosition.x)
            direction = 1;

        if (body.position.x >= StartPosition.x + 10)
        { direction = -1; }
        
        body.velocity = new Vector2(6*direction, 0);


    }


}
