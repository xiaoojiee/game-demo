using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooThing;
    
    void Start()
    {
        
    }
    private void LateUpdate()
    {
        if(target != null)
        {
            if (transform.position != target.position)
            {
                Vector3 TargetPos=new Vector3(target.position.x, target.position.y, transform.position.z);
                
                transform.position = Vector3.Lerp(transform.position, TargetPos, smooThing);
            }
        }
    }

    void Update()
    {
        
    }
}
