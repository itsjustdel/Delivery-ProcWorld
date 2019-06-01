using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {

    public bool ignoreClicks = true;

    float inputX;
    float inputZ;

   // Vector3 pos1 = new Vector3();
   // Vector3 pos2 = new Vector3();

    Quaternion rot1 = new Quaternion();
    Quaternion rot2 = new Quaternion();

    
    

    public int lowYClamp = 10;
    // Use this for initialization
    void Start () {
        //pos1 = transform.position;
        rot1 = transform.rotation;

      //  pos2 = transform.GetChild(0).position;
       // rot2 = transform.GetChild(0).rotation;
        Cursor.visible = false;
    }

    void Update()
    {
        
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        if (Input.GetMouseButton(1))
            move();
        
/*
        if (Input.GetKey(KeyCode.LeftControl))
            if(transform.position.y >lowYClamp)
                transform.position -= Vector3.up*0.2f;
        if (Input.GetKey(KeyCode.LeftShift))
            if (transform.position.y < 50f)
                transform.position += Vector3.up*0.2f;

  */     

        

        if (Input.GetKey(KeyCode.LeftAlt))
            rotate();
        else
            transform.localRotation = rot1;

        //if (Input.GetMouseButtonDown(2))
        //    Debug.Log("Pressed middle click.");


        if (Input.GetKey("escape"))
            Application.Quit();


        
    }

    void rotate()
    {
        // transform.GetChild(0).Rotate(transform.up, inputX*0.1f);
        
        transform.localRotation *= Quaternion.Euler(0, inputX*0.5f, 0);
        //transform.localRotation *= Quaternion.Euler(-inputZ*0.1f, 0, 0);
        //transform.GetChild(0).Rotate(transform.right, inputZ * 0.1f);
    }


    void move()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward *0.2f;
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * 0.2f;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right* 0.2f;
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right* 0.2f;
    }
}
