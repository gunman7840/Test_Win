using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

    class DrawDelayLine : MonoBehaviour
    {
        public Vector2 x1 {get;set;}
        public Vector2 x2 {get;set;}
        public Color color;
        
        public void Initialize (Vector2 _x1, Vector2 _x2, Color _color)
        {
            x1 = _x1;
            x2 = _x2;
            color = _color;
        }
        
        void Start()
        {
            Destroy(gameObject, 2f);
        }

        void Update()
        {
            Debug.DrawLine(x1, x2, color);   
        }

        void OnDrawGizmos()
        {
            
        }




    }
