using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Wheel : MonoBehaviour
{
    //Работает нестабильно, маленькие объекты застревают и очень плохо перетягиваются
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            DistanceJoint2D myJoint = (DistanceJoint2D)gameObject.AddComponent<DistanceJoint2D>();
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            //HingeJoint myJoint = (HingeJoint)gameObject.AddComponent("HingeJoint");
            other.gameObject.SendMessage("SettoSleep");
            myJoint.connectedBody = rb;
            myJoint.enableCollision = true;
            myJoint.distance = 0.3f;         
            myJoint.anchor = new Vector2(-0.15f,0);
            myJoint.connectedAnchor = new Vector2(0, -0.5f);
            StartCoroutine(DestroyJoint(myJoint,other));
        }
    }

    IEnumerator DestroyJoint(DistanceJoint2D _joint, Collision2D enemy)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(_joint);
            enemy.gameObject.SendMessage("SettoAwake");

        }
    }

}