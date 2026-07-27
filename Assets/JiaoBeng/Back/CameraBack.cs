using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBack : MonoBehaviour
{
    public Transform target;
    public Transform[] Back;
    private Vector2 lastPos;
    public float parallax;
    private void Start()
    {
        lastPos=transform.position;
    }
    private void Update()
    {
        transform.position=new Vector3(target.position.x,target.position.y,0);
        Vector2 amuountToMove=new Vector2(transform.position.x-lastPos.x,transform.position.y-lastPos.y);
        for(int i=0;i<Back.Length;i++)
        {
            Back[i].position += new Vector3(amuountToMove.x * parallax * i / Back.Length, amuountToMove.y * parallax * i / Back.Length,0f);
            if (i == Back.Length)
            {
                Back[i].position+=new Vector3(amuountToMove.x,amuountToMove.y,0f);
            }
        }
        lastPos = transform.position;
    }
}
