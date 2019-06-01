using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerProc : MonoBehaviour
{
    public bool tankControls = false;
    //public GameObject camera;
    public ProceduralAnimator pA;
    public BindPoseExample bPE;
    public GameObject root;
    public float rotSpeed = 3f;
    public float sideStepVar = 1f;
    public float sideStepAmount = 0f;
    public float leftAmount;
    public float rightAmount;
    public float forwardAmount;
    public float backwardsAmount;
    public float smooth = 0.5f;
    public bool climbing;
    public bool autoWalk = false;
    public bool autoRotate = false;
    public float stretch = 1f;
    public Vector3 targetDir;
    public bool leadWithRight = true;
    public bool targetSameAsRoot = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // if (transform.Find("Rig").gameObject != null)
        //    root = transform.Find("Rig").gameObject;

        //reset every time
        targetDir = Vector3.zero;
       // float angle = 0f;
        if (tankControls)
        {
            if (Input.GetKey("a"))                  //digital input
                leftAmount += smooth;
            else
                leftAmount -= smooth;

            if (Input.GetKey("d") || autoRotate)
                rightAmount += smooth;
            else
                rightAmount -= smooth;


            if (Input.GetKey("w") || autoWalk)                  //digital input
                forwardAmount += smooth;
            else
                forwardAmount -= smooth;

            if (Input.GetKey("s"))
                backwardsAmount += smooth;
            else
                backwardsAmount -= smooth;

            leftAmount = Mathf.Clamp(leftAmount, 0f, 1f);
            rightAmount = Mathf.Clamp(rightAmount, 0f, 1f);
            forwardAmount = Mathf.Clamp(forwardAmount, 0f, 1f);
            backwardsAmount = Mathf.Clamp(backwardsAmount, 0f, 1f);

            if (climbing)
            {
                //stop lots of rotation, breaks it
                leftAmount = Mathf.Clamp(leftAmount, 0f, 1f);
                rightAmount = Mathf.Clamp(rightAmount, 0f, 1f);
                sideStepAmount = 0f;
            }
            else
                sideStepAmount = sideStepVar;

            if (Input.GetKey("a"))
            {
                root.transform.rotation = root.transform.rotation * Quaternion.Euler(0, -rotSpeed * leftAmount, 0f);
            }
            if (Input.GetKey("d"))
            {
                root.transform.rotation = root.transform.rotation * Quaternion.Euler(0, rotSpeed * rightAmount, 0f);
            }

        }
        else
        {
            //we need to create a target vector set by the controls and mvoe character towards this
            
            if (Input.GetKey("a"))
                targetDir -= Vector3.right;
            if (Input.GetKey("d"))
                targetDir += Vector3.right;
            if (Input.GetKey("w"))
                targetDir += Vector3.forward;
            if (Input.GetKey("s"))
                targetDir -= Vector3.forward;

            //don't know the maths behind this, multiplying rotation? - working anyway
            targetDir = Camera.main.transform.TransformDirection(targetDir);
            targetDir.y = 0;
            targetDir.Normalize();
           // angle = Vector3.Angle(targetDir, root.transform.forward);
            
            targetDir = Vector3.RotateTowards(root.transform.forward, targetDir, rotSpeed * Time.deltaTime, 0.0f);

            Debug.DrawLine(root.transform.position, root.transform.position + targetDir*2, Color.magenta);
            //try and get character to spin a natural way, so if they are on their right foot, try to spin right? -only from idle - check this in ProcAnimator - using targetDir?
            //only spin if foot can move? other wise spin head?-not doing atm need ot make another objet as target

            //figure out if targetDir is going left or right of character
            //find out what way feet are facing
            

            float dir = AngleDir(root.transform.forward, targetDir, Vector3.up);
            if (dir < 0f)
                leadWithRight = false;
            else
                leadWithRight = true;

            


            if (root.transform.rotation == Quaternion.LookRotation(targetDir))
                targetSameAsRoot = true;
            else
                targetSameAsRoot = false;

            root.transform.rotation = Quaternion.LookRotation(targetDir);
        }

        
        if (tankControls)
            pA.strideLengthVar = (bPE.shinLength + bPE.thighLength+ bPE.footHeight) * stretch * forwardAmount;
        else
        {


            if (Input.GetKey("a") || (Input.GetKey("d")) || (Input.GetKey("w")) || (Input.GetKey("s")))
            {
                forwardAmount += smooth;

                pA.strideLengthVar = (bPE.shinLength + bPE.thighLength + bPE.footHeight) * stretch * targetDir.magnitude;
                
                //pA.strideLengthVar = (bPE.waistWidth) * stretch * targetDir.magnitude;


            }
            else
            {
                forwardAmount -= smooth;
                pA.strideLengthVar = 0;
            }

            forwardAmount = Mathf.Clamp(forwardAmount, 0f, 1f);

        }
        //**chaing to right and left idle?

        if (pA.strideLengthVar == 0 && !pA.rightIdle && !pA.leftIdle)
        {
            pA.lastStrideLeft = true;
            pA.lastStrideRight = true;
        }
        else
        {
            pA.lastStrideLeft = false;//does this need to chck phase/lerp?
            pA.lastStrideRight = false;
        }

    }

    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }
}
