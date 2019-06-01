using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    CharacterControllerProc ccp;

    public bool drawDebug;
    public bool printLogs;
    public float walkSpeed = 3f;
    public float targetLerpSpeed = 1f;//not using
    public float footRotationSpeed = 2f;
    public float maxClimbHeight = 1f;
    public float bounce = 0.1f;
    public float lean = 0f;
    public float maxAngle = 30;//bump detect
    public float maxAngleSlope = 45;
    //List<GameObject> allBones = new List<GameObject>();

    private GameObject hips;

    public GameObject leftPelvis;
    public GameObject rightPelvis;

    public GameObject rightFoot;
    public GameObject leftFoot;
    public GameObject rightAnkle;
    public GameObject leftAnkle;
    GameObject leftKnee;
    GameObject rightKnee;
    GameObject root;
    Vector3 rootStart;

    private GameObject spine0;
    private GameObject spine1;
    private GameObject spine2;
    private GameObject spine3;
    private GameObject leftShoulder;
    private GameObject rightShoulder;
    private GameObject head;
    private GameObject leftElbow;
    private GameObject rightElbow;
    private GameObject leftWrist;
    private GameObject rightWrist;
    private GameObject gyro;
    private GameObject rightAnkleWeight;
    private GameObject leftAnkleWeight;

    public int rightPelvisState = 0;

    //float kneeLiftHeight = .05f;
    public float m = 0f;

    float rightKneeStartHeight;
    float footHeight;

    public float strideLengthVar = 1f;
    Vector3 rightAnkleStart;
    Vector3 leftAnkleStart;
    Vector3 leftKneeStart;
    Vector3 rightKneeStart;
    Vector3 rightFootTarget;
    Vector3 leftFootTarget;
    Vector3 leftPelvisStart;
    Vector3 rightPelvisStart;
    ///Vector3 lastStrideStart;
    public bool lastStrideStartSet = false;

    Quaternion rightPivotStartRot;
    Quaternion LeftPivotStartRot;
    public GameObject rightFootPivot;
    public GameObject leftFootPivot;


    Vector3 leftFootTargetDir;
    public Vector3 rightToeTarget;
    public Vector3 leftToeTarget;
    public bool stopRightToeRotation = false;
    // public bool stopRightToeRotationPrevious = false;
    public bool stopLeftToeRotation = false;
    // public bool stopLeftToeRotationPrevious = false;
    // bool halfStride = true;//start with a halfStride
    public int rightPhase = 0;
    public int leftPhase = 0;
    public float rightLerp = 0f;
    public float leftLerp = 0f;
    public float idleLerp = 0f;
    public bool edgeNearOtherFootFound = false;
    private Vector3 edgeNearOtherFoot;
    public bool moveRight = true;
    public bool firstStrideLeft = true;
    public bool firstStrideRight = true;
    public bool lastStrideLeft = false;
    public bool lastStrideRight = false;
    public bool rightFootAtDrop = false;
    public bool leftFootAtDrop = false;
    public bool stopping = false;
    //public bool idle = true;
    public bool leftIdle = true;
    public bool rightIdle = true;


    private float footLength;
    
    private float shinLength;
    private float thighLength;
    private float waistWidth;
    private float spineLength;
    private float spineCurve;
    private float shoulderWidth;
    private float upperArmLength;
    private float forearmLength;
    private float gaitWidth;
    public float twistAmount;
    public float topHalfTwistRatio;//can make top half swing less than bottom

    [Range(0, 5)]
    public int topHalfTwistAdd;

    // float distanceToToeHitRight = 0f;
    Vector3 rightLiftTarget = Vector3.zero;
    Vector3 leftLiftTarget = Vector3.zero;
    public bool rightFirstHalf = true;
   // public bool rightLift = false;
    public bool leftLift = false;
    public bool rightDrop = false;
    public bool leftDrop = false;

    public float distanceBetweenHeels;

    public List<float> leftStrideLengths = new List<float>();
    public List<float> righttStrideLengths = new List<float>();

    public float lastStrideLengthRight = 0f;

    public float testDistance;

    public float leftHipLerp;
    public float rightHipLerp;

    private BindPoseExample bpe;

    public List<Vector3> lastHits;

    public float headSway = 0.5f;


    private void Awake()
    {
        enabled = false;    
    }

    // Use this for initialization
    void Start()
    {
        
       
        ccp = GetComponent<CharacterControllerProc>();

         bpe = GetComponent<BindPoseExample>();
        //  hips = transform.GetChild(0).Find("hips").gameObject;
        // rightPelvis = transform.GetChild(0).Find("hips").Find("Right Pelvis").gameObject;
        // rightKnee = rightPelvis.transform.Find("Right Knee").gameObject;
        footLength = bpe.footLength;
        footHeight = bpe.footHeight;
        shinLength = bpe.shinLength;
        thighLength = bpe.thighLength;
        waistWidth = bpe.waistWidth;
        spineLength = bpe.spineLength;
        spineCurve = bpe.spineCurve;
        shoulderWidth = bpe.shoulderWidth;
        upperArmLength = bpe.upperArmLength;
        forearmLength = bpe.forearmLength;
        gaitWidth = bpe.gaitWidth;
        twistAmount = bpe.twistAmount;
        topHalfTwistRatio = bpe.topHalfTwistRatio;

        root = bpe.root;

        rightAnkle = bpe.rightAnkle;
        rightFoot = bpe.rightFoot;

        leftAnkle = bpe.leftAnkle;
        leftFoot = bpe.leftFoot;

        leftKnee = bpe.leftKnee;
        rightKnee = bpe.rightKnee;

        leftPelvis = bpe.leftPelvis;
        rightPelvis = bpe.rightPelvis;

        rightElbow = bpe.rightElbow;
        rightWrist = bpe.rightWrist;

        leftWrist = bpe.leftWrist;
        leftShoulder = bpe.leftShoulder;

        rightAnkleWeight = bpe.rightAnkleWeight;
        leftAnkleWeight = bpe.leftAnkleWeight;

        for (int i = 0; i < bpe.spine.Count; i++)//daft
        {
            if (i == 0)
                spine0 = bpe.spine[i];
            if (i == 1)
                spine1 = bpe.spine[i];
            if (i == 2)
                spine2 = bpe.spine[i];
            if (i == 3)
                spine3 = bpe.spine[i];
            if (i == 4)
                head = bpe.spine[i];

            if(i>0)
                bpe.spine[i].transform.parent = bpe.spine[i-1].transform;

        }
        
        

        leftShoulder = bpe.leftShoulder;
        rightShoulder = bpe.rightShoulder;
        rightElbow = bpe.rightElbow;
        leftElbow = bpe.leftElbow;

      //  root.transform.position = transform.position + Vector3.up * (shinLength + thighLength + footHeight);

     //   leftPelvis.transform.position = root.transform.position - root.transform.right * waistWidth*.5f;
     //   rightPelvis.transform.position = root.transform.position + root.transform.right * waistWidth*.5f;

        //  rightKneeStartHeight = rightKnee.transform.position.z - hips.transform.position.z;

        //set which part of the foot to lean on
        leftFootPivot = leftFoot;
        //set so if the pivot rotates, the other part of the foot comes with it
        // leftAnkle.transform.parent = null;
        // leftFoot.transform.parent = null;
        // leftAnkle.transform.parent = leftFoot.transform;
        // leftFoot.transform.parent = transform.Find("Rig");

        //set which part of the foot to lean on
        rightFootPivot = rightAnkle;

        
        RaycastHit hit;
        Vector3 rayStart = rightAnkle.transform.position + gameObject.transform.position;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
         //   rightAnkle.transform.position = hit.point;// + Vector3.up * 0.05f;
                                                      // Debug.Log("HIT");
        }


        rayStart = rightFoot.transform.position + gameObject.transform.position;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
          //  rightFoot.transform.position = hit.point;// + Vector3.up * 0.05f;
        }

        rayStart = leftFoot.transform.position + gameObject.transform.position;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
         //   leftFoot.transform.position = hit.point;// + Vector3.up * 0.05f;
        }

        rayStart = leftAnkle.transform.position + gameObject.transform.position;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
         //   leftAnkle.transform.position = hit.point;// + Vector3.up*0.05f;
        }
        

       // leftKnee.transform.position = leftAnkle.transform.position + Vector3.up * shinLength + root.transform.forward * footLength * 0.5f;
       // rightKnee.transform.position = rightAnkle.transform.position + Vector3.up * shinLength + root.transform.forward * footLength * 0.5f;



        leftAnkleStart = leftAnkle.transform.position;
        rightAnkleStart = rightAnkle.transform.position;
        leftKneeStart = leftKnee.transform.position;
        rightKneeStart = rightKnee.transform.position;
        leftPelvisStart = leftPelvis.transform.position;
        rightPelvisStart = rightPelvis.transform.position;

        LeftPivotStartRot = leftAnkle.transform.rotation;


        //set target for left to for first rotation of ankle //still using?
        leftFootTarget = leftFoot.transform.position + (root.transform.forward * strideLengthVar) ;
        rayStart = leftFootTarget + Vector3.up;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
            leftFootTarget = hit.point;
        }
        //check toe target too


        rayStart = leftFootTarget + root.transform.forward * footLength + Vector3.up;

        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
            leftToeTarget = hit.point;
            // Debug.Log("hit on start");
        }

        //leftKnee.GetComponent<MeshRenderer>().enabled = false;
        //rightKnee.GetComponent<MeshRenderer>().enabled = false;
        

        //rightPelvis.transform.position = rightKnee.transform.position + Vector3.up * thighLength;
        //leftPelvis.transform.position = leftKnee.transform.position + Vector3.up * thighLength;
       // root.transform.position = Vector3.Lerp(rightPelvis.transform.position, leftPelvis.transform.position, 0.5f);
        
        rootStart = root.transform.position;

        rightAnkle.transform.parent = transform;
        rightFoot.transform.parent = rightAnkle.transform;


        leftAnkle.transform.parent = leftFoot.transform;
        leftFoot.transform.parent = transform;

        rightKnee.transform.parent = transform;
        leftKnee.transform.parent = transform;

        leftPelvis.transform.parent = transform;
        rightPelvis.transform.parent = transform;

        //used for working out roations for hips
        gyro = bpe.gyro;
        gyro.transform.position = root.transform.position;
        gyro.transform.parent = root.transform.transform;


        //where should these be decided?
        bounce = footLength/2;
        headSway = Random.Range(0.2f, 0.8f); //*** ??
    }
    private void Update()
    {
        if (drawDebug)
            DrawDebug();

        BindPoseExample bpe = GetComponent<BindPoseExample>();
        footLength = GetComponent<BindPoseExample>().footLength;
        footHeight = GetComponent<BindPoseExample>().footHeight;
        shinLength = GetComponent<BindPoseExample>().shinLength;
        thighLength = GetComponent<BindPoseExample>().thighLength;
        waistWidth = bpe.waistWidth;
        gaitWidth = bpe.gaitWidth;

        shoulderWidth = bpe.shoulderWidth;
        upperArmLength = bpe.upperArmLength;
        forearmLength = bpe.forearmLength;

        twistAmount = bpe.twistAmount;
        topHalfTwistRatio = bpe.topHalfTwistRatio;

        

        //strideLengthVar = (shinLength + thighLength) * 3;
        // strideLengthVar = (shinLength + thighLength)+ footLength;

    

    }
    // Update is called once per frame


    void DrawDebug()
    {
        Debug.DrawLine(leftAnkleStart, leftLiftTarget);
        Debug.DrawLine(leftLiftTarget, leftFootTarget, Color.cyan);
        Debug.DrawLine(rightAnkleStart, rightLiftTarget);
        Debug.DrawLine(rightLiftTarget, rightFootTarget, Color.blue);

        // Debug.DrawLine(rightAnkleStart, rightLiftTarget);
        // Debug.DrawLine(rightLiftTarget, rightFootTarget, Color.blue);


        Debug.DrawLine(leftFoot.transform.position, leftAnkle.transform.position);
        Debug.DrawLine(rightFoot.transform.position, rightAnkle.transform.position);

        Debug.DrawLine(leftAnkle.transform.position+leftAnkle.transform.up*footHeight, leftKnee.transform.position);
        Debug.DrawLine(rightAnkle.transform.position+rightAnkle.transform.up * footHeight, rightKnee.transform.position);

        Debug.DrawLine(leftPelvis.transform.position, leftKnee.transform.position);
        Debug.DrawLine(rightPelvis.transform.position, rightKnee.transform.position);

        Debug.DrawLine(leftPelvis.transform.position, root.transform.position);
        Debug.DrawLine(rightPelvis.transform.position, root.transform.position);

        //arms

        Debug.DrawLine(leftShoulder.transform.position, bpe.spine[6].transform.position);
        Debug.DrawLine(rightShoulder.transform.position, bpe.spine[6].transform.position);

        Debug.DrawLine(leftShoulder.transform.position, leftElbow.transform.position);
        Debug.DrawLine(leftElbow.transform.position, leftWrist.transform.position);

        Debug.DrawLine(rightShoulder.transform.position, rightElbow.transform.position);
        Debug.DrawLine(rightElbow.transform.position, rightWrist.transform.position);
;

    }
    private void FixedUpdate()
    {
        
        UnParentElements();

        Feet();
        
        Pelvises();

        Knees();

        Shoulders();

        Arms();
        
        GatherElements();

    }

    void Feet()
    {
        
        //at the start of each stride, find out where we will move foot to

        //two phases for each stride, at the end of second phase, moveRight is switched
        for (int i = 0; i < 2; i++)
        {

            //force direction if idle, means one foot doesn't need to step over other if starting from a stopped position.
            //The leg closest to the target direction will be used

            if(leftIdle && rightIdle)
            {
                if (ccp.leadWithRight)
                    moveRight = true;
                else
                    moveRight = false;

                //idle animations
                RotateStandingFoot(leftFootPivot, leftFootTarget, leftToeTarget, 0);
                RotateStandingFoot(rightFootPivot, rightFootTarget, rightToeTarget, 0);
            }

            if (moveRight)
            {

                //find where to move foot to

                //only do when we need to, timers and flags can probably be simplified now using only one lerp value**TODO
                if (i == 0)
                    CheckForGradientChange(i, rightPhase);
             
                if (rightIdle)
                {
                    //dont rotate
                }
                //rotate moving foot
                else
                {
                    RotateWalkingFoot(rightFootPivot, rightFootTarget, rightPhase, moveRight);
                }


                if (rightIdle)
                {
                    //dont move
                }
                else if (rightFootAtDrop && !rightIdle)
                    //move in to position before idle is set true
                    MoveFoot(rightFootPivot, rightLiftTarget, rightAnkleStart, rightFootTarget, rightPhase, rightLerp);
                else if (!rightFootAtDrop && !rightIdle)
                    //walk normally
                    MoveFoot(rightFootPivot, rightLiftTarget, rightAnkleStart, rightFootTarget, rightPhase, rightLerp);

                //always rotate
                //rotate standing foot (other/ left)
                if (leftIdle)
                {
                    RotateStandingFoot(leftFootPivot, leftFootTarget, leftToeTarget, 0);
                }
                else if (rightFootAtDrop && !leftIdle)
                {
                    RotateStandingFoot(leftFootPivot, leftFootTarget, leftToeTarget, 0);
                }
                else
                {
                    //just rotate first half( get foot flat)
                    RotateStandingFoot(leftFootPivot, leftFootTarget, leftToeTarget, rightPhase);
                    // Debug.DrawLine(leftToeTarget, leftToeTarget + Vector3.up, Color.magenta);
                }
                    

            }
            else if (!moveRight)
            {
                

                //detect a change in direction

                //find where to move foot to
                //only do when we need to, timers and flags can probably be simplified now using only one lerp value
                if (i == 1)
                    CheckForGradientChange(i, leftPhase);
              
                //rotate moving foot
                if (leftIdle)
                {
                    //dont rotate
                }
                else
                {
                    RotateWalkingFoot(leftFootPivot, leftFootTarget, leftPhase, moveRight);
                }

                //now move foot if needed

                if (leftIdle)
                {
                    //dont move
                }
                else if (leftFootAtDrop && !leftIdle)
                    //move in to position before idle is set true
                    MoveFoot(leftFootPivot, leftLiftTarget, leftAnkleStart, leftFootTarget, leftPhase, leftLerp);
                else if (!leftFootAtDrop && !leftIdle)
                    //walk normally
                    MoveFoot(leftFootPivot, leftLiftTarget, leftAnkleStart, leftFootTarget, leftPhase, leftLerp);

                
                //  //rotate standing foot
                if (rightFootAtDrop)
                {
                    RotateStandingFoot(rightFootPivot, rightFootTarget, rightToeTarget, 0);
                }
                else if (!rightIdle)
                {
                    RotateStandingFoot(rightFootPivot, rightFootTarget, rightToeTarget, leftPhase);
                }
                else if (rightIdle)
                {
                    RotateStandingFoot(rightFootPivot, rightFootTarget, rightToeTarget, 0);
                }
                    
                

               
            }



            //update lerps/check for end of stride
            if (leftIdle && rightIdle)
            {
             
            }
            else
                AdjustTimersAndFlags(i);



        }
       
    }

    void Knees()//needs optimsied -IK target and lerp?
    {
        //hip points and foot points have already been found. Find the knee point by finding the intersect point of ankle plus shin length and hip plus thig length
        //create two circles of shin and thig length and find the two closest points

        for (int x = 0; x < 2; x++)
        {
            //switch between legs
            GameObject ankle = leftAnkle;
            GameObject knee = leftKnee;
            GameObject pelvis = leftPelvis;
            GameObject pivot = leftFootPivot;
            
            if (x == 1)
            {
                ankle = rightAnkle;
                knee = rightKnee;
                pelvis = rightPelvis;
                pivot = rightFootPivot;
            }

            List<Vector3> shinCirclePoints = new List<Vector3>();
            List<Vector3> thighCirclePoints = new List<Vector3>();
            //large step with smoothed movemetns below, better performance and smoother movements!
            float step = 5f;
            int kneeBend = 90;//lower for performance? 60 seems ok..

            //if we are rotating, limit knee bend
            if (x == 0)
            {
                if(ccp.leftAmount == 1f)
                {
                   // kneeBend = 10;  
                }
            }
            else if(x == 1)
            {
                if (ccp.rightAmount == 1f)
                {
                   // kneeBend = 10;
                }
            }


            for (float i =0; i <= kneeBend; i += step)////starting at step stops knee going slightly behind
            {
                Vector3 arm = (pelvis.transform.position - ankle.transform.position +ankle.transform.up*footHeight).normalized*shinLength;
                arm = Vector3.RotateTowards(arm, root.transform.forward, i*Time.deltaTime,0.0f);
                
                Vector3 p = ankle.transform.position + arm + Vector3.up*footHeight;// * (arm * shinLength));

                 if(x==1 && drawDebug)
                   Debug.DrawLine(ankle.transform.position + ankle.transform.up * footHeight, p,Color.cyan);

                shinCirclePoints.Add(p);
            }

            for (float i = 0; i <= kneeBend; i += step)
            {

                Vector3 arm = (ankle.transform.position - pelvis.transform.position).normalized * thighLength;
                arm = Vector3.RotateTowards(arm, root.transform.forward, i * Time.deltaTime, 0.0f);
                Vector3 p = pelvis.transform.position + arm;// * (arm * shinLength));
                if (x==1 && drawDebug)
                  Debug.DrawLine(pelvis.transform.position, p, Color.magenta);

                thighCirclePoints.Add(p);
            }

            //now find the knee point. The mid point of the two points which are closest to each other but are not closer to the other bone than they are to their own bone
            int closestShin = 0;
            int closestThigh = 0;
            float closestDistance = Mathf.Infinity;

            List<Vector3> possiblePoints = new List<Vector3>();
            Vector3 midPoint = Vector3.zero;
            float denominator = thighLength + shinLength;
            float lerp = shinLength / denominator;
            Vector3 kneeLock = Vector3.Lerp(ankle.transform.position + Vector3.up*footHeight, pelvis.transform.position, lerp);
            for (int i = 0; i < shinCirclePoints.Count; i++)
            {
                for (int j = 0; j < thighCirclePoints.Count; j++)
                {
                    float tempDistance = Vector3.Distance(shinCirclePoints[i], thighCirclePoints[j]);

                    if (tempDistance < closestDistance)
                    {
                        //float shinPointToPelvisDistance = Vector3.Distance(shinCirclePoints[i], pelvis.transform.position);
                        //float thighPointToAnkleDistance = Vector3.Distance(thighCirclePoints[i], ankle.transform.position);

                        // if(shinPointToPelvisDistance >= thighLength && thighPointToAnkleDistance >= shinLength )
                        {
                            //make sure it is not behing the knee lock point
                            //find thigh/shin ratio/percentage of shin to whole
                            // to get whole


                            //is it ahead, multiply by transform.fwd?
                            // midPoint = Vector3.Lerp(shinCirclePoints[i], thighCirclePoints[j], 0.5f);
                            // Debug.DrawLine(midPoint, kneeLock, Color.blue);
                            // if (midPoint.z > kneeLock.z)//not checking this anymore, only rotating arm forward now on checks above
                            {
                                closestShin = i;
                                closestThigh = j;
                                closestDistance = tempDistance;

                                // possiblePoints.Add(midPoint);
                            }
                        }
                    }
                }
            }

            // Debug.DrawLine(midPoint, kneeLock, Color.blue);
            Vector3 kneeTarget = Vector3.Lerp(shinCirclePoints[closestShin], thighCirclePoints[closestThigh], 0f);//forcing on on ankle side
            //smooth movements
             knee.transform.position = Vector3.Lerp(knee.transform.position, kneeTarget, walkSpeed / 4);
           // knee.transform.position = kneeTarget;//non lerped
            //now we need to make sure pelvis is correct position//doing this in pelvis() need to do again?
           //Vector3 clamped = knee.transform.position + (pelvis.transform.position - knee.transform.position).normalized * thighLength;
            //pelvis.transform.position = clamped;

            //clamp 
             //clamped = pelvis.transform.position + (knee.transform.position - pelvis.transform.position).normalized * thighLength;
            //knee.transform.position = clamped;

            // Debug.DrawLine(shinCirclePoints[closestShin], thighCirclePoints[closestThigh]);
        }

        //rotation - look forward

       // leftKnee.transform.rotation = Quaternion.Euler(new Vector3(0f,leftAnkle.transform.rotation.eulerAngles.y,0f));        //twisting and causing z fighting
       // rightKnee.transform.rotation = Quaternion.Euler(new Vector3(0f, rightAnkle.transform.rotation.eulerAngles.y, 0f));

        leftKnee.transform.rotation = root.transform.rotation;
        rightKnee.transform.rotation = root.transform.rotation;

        //set weights for skirt, basically the ankle's position without the rotation
        rightAnkleWeight.transform.position = rightAnkle.transform.position;
        leftAnkleWeight.transform.position = leftAnkle.transform.position;

        rightAnkleWeight.transform.rotation = root.transform.rotation;
        leftAnkleWeight.transform.rotation = root.transform.rotation;

    }

    void Pelvises()
    {
        //roate the gyro and get pelvis places off of this - apply target rotation to pelvises

        //place pelvis to each side of root - then lerp and move

        //start by creating a point between the feet and at waist height
        Vector3 centre = Vector3.Lerp(rightFoot.transform.position, leftFoot.transform.position, 0.5f) + Vector3.up * (shinLength + thighLength + footHeight);
        gyro.transform.position = centre;

        //rotate gyro
        float spinTarget = twistAmount;//stridelength and walkspeed infuence?
        float zTilt =  leftFoot.transform.position.y - rightFoot.transform.position.y;
        zTilt *= 50f;

        //look down and the forward 
        //Vector3 leftY0 = new Vector3(leftAnkle.transform.position.x, 0, leftAnkle.transform.position.z);
        //Vector3 rightY0 = new Vector3(rightAnkle.transform.position.x, 0, rightAnkle.transform.position.z);
        Vector3 centreOfFeet = Vector3.Lerp(leftAnkle.transform.position, rightAnkle.transform.position, 0.5f);
        
        gyro.transform.LookAt(centreOfFeet);
        gyro.transform.localRotation *= Quaternion.Euler(90, 180, 0);

        Debug.DrawLine(centreOfFeet, gyro.transform.position);
        //rotate by how much character is moving left or right

        if (moveRight)
        {
            /*
            float thisLerp = rightLerp/2;
            if (rightPhase == 1)
                thisLerp = -rightLerp/2 + 0.5f;
            float spinAmount = spinTarget * (thisLerp);

            spinAmount = zTilt*10;
            */
            //add to existing rotation
            //float spinY = root.transform.rotation.eulerAngles.y + spinTarget*Time.deltaTime;// - spinAmount * 2 - spinTarget;
            if(rightPhase == 0)
                gyro.transform.rotation = gyro.transform.rotation * Quaternion.Euler(0, 0, -zTilt);//was roo
            if (rightPhase == 1)
                gyro.transform.rotation = gyro.transform.rotation * Quaternion.Euler(0, 0, -zTilt);//was root

        }
        else
        {
            /*
             * was using timers and lerps but have now changed to being influenced only by feet positions
            float thisLerp = leftLerp/2;
            if (leftPhase == 1)
                thisLerp = -leftLerp / 2 + 0.5f;
            float spinAmount = spinTarget * (thisLerp);
            float spinY = root.transform.rotation.eulerAngles.y + spinTarget * Time.deltaTime;// * 2 - spinTarget;
            */
            if(leftPhase == 0)
                gyro.transform.rotation = gyro.transform.rotation * Quaternion.Euler(0, 0, -zTilt);//was -spinAmount * 2 + spinTarget for y
            if(leftPhase == 1)
                gyro.transform.rotation = gyro.transform.rotation * Quaternion.Euler(0, 0, -zTilt);//was -spinAmount * 2 + spinTarget for y
        }

        //place pelvis to each side of gyro 
        
        //rotate toawards root but ony on yaxis
        //float gyroCatchUpSpeed = 1;
        //Quaternion towardsRoot = Quaternion.RotateTowards(gyro.transform.rotation, root.transform.rotation, gyroCatchUpSpeed);
        //towardsRoot = Quaternion.Euler(0, towardsRoot.eulerAngles.y, 0);
        //gyro.transform.rotation = root.transform.rotation;

        if(!leftIdle)
            leftPelvis.transform.position = centre - gyro.transform.right * waistWidth * .5f + lean * root.transform.forward;//can be gyro right too?
        if (!rightIdle)
            rightPelvis.transform.position = centre + gyro.transform.right * waistWidth * .5f + lean * root.transform.forward;

        //clamp
        if (!leftIdle)
            leftPelvis.transform.position = (leftPelvis.transform.position - leftKnee.transform.position).normalized * thighLength + leftKnee.transform.position;
        if (!rightIdle)
            rightPelvis.transform.position = (rightPelvis.transform.position - rightKnee.transform.position).normalized * thighLength + rightKnee.transform.position;
        
        if (!rightIdle)
            rightPelvis.transform.rotation = gyro.transform.rotation;
        if (!leftIdle)
            leftPelvis.transform.rotation = gyro.transform.rotation;
        
        root.transform.position = Vector3.Lerp(leftPelvis.transform.position, rightPelvis.transform.position, 0.5f);
    }  

    void Shoulders()
    {
        //and spine

        //4 parts to the spine
        //stomach
        //solar plex
        //chest
        float spineSegment = spineLength / GetComponent<BindPoseExample>().spine.Count;
        //neck
        spine0.transform.position = root.transform.position + Vector3.up * spineSegment*1;

        
        //spine0.transform.rotation = root.transform.rotation;
        //spine0.transform.localRotation = Quaternion.Euler(0, spine0.transform.rotation.eulerAngles.y, 0);
        //lets's get this body rocking

       // float spinSpeed = topHalfTwistRatio;//this is how quickly the body straightens out the hip swing. Value needs to be between 0 and 1, we multiply by i for exponential curve
        
        for (int i = 0; i < 3; i++)
        {
            //bpe.spine[i].transform.rotation = Quaternion.RotateTowards(bpe.spine[i].transform.rotation,gyro.transform.rotation, twistAmount*(i+1));
            
            bpe.spine[i].transform.rotation = Quaternion.Lerp(root.transform.rotation, gyro.transform.rotation, 1f/(i+1));

        }

        int twisty = 4 + topHalfTwistAdd;
        //top half is inverted ( left shoulder goes forweard when right pelvis is back)
        for (int i = 3, j =  twisty   ; i < 7; i++,j--)
        {
            Quaternion inverted = Quaternion.Inverse(gyro.transform.rotation );
            inverted = Quaternion.Euler(inverted.eulerAngles.x, root.transform.rotation.eulerAngles.y, inverted.eulerAngles.z);
            //bpe.spine[i].transform.rotation = Quaternion.RotateTowards(bpe.spine[i].transform.rotation, inverted, 1f);// topHalfTwistRatio * (j));
            float lerpAmount = 1f / (j);

           // lerpAmount /= topHalfTwistRatio;

            bpe.spine[i].transform.rotation = Quaternion.Lerp(root.transform.rotation, inverted,lerpAmount);
          //  if (i == 7)
            //    bpe.spine[i].transform.rotation = root.transform.rotation;
           // bpe.spine[i].transform.rotation = Quaternion.Lerp(root.transform.rotation, inverted, lerpAmount);
        }
        Quaternion fullTargetRot = Quaternion.Euler(gyro.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, gyro.transform.rotation.eulerAngles.z);

        
        bpe.spine[7].transform.rotation = Quaternion.Lerp(root.transform.rotation, fullTargetRot, headSway);
        //bpe.spine[2].transform.rotation = root.transform.rotation;

        return;
        //spine2.transform.position = root.transform.position + Vector3.up * spineSegment * 2;
        // spine2.transform.localRotation = Quaternion.Euler(spine1.transform.localRotation.eulerAngles.x / 2, 0, 0);

        // spine3.transform.localRotation = Quaternion.Euler(spine2.transform.localRotation.eulerAngles.x / 2, 0, 0);
        // bpe.spine[4].transform.localRotation = Quaternion.Euler(spine3.transform.localRotation.eulerAngles.x / 2, 0, 0);
        /// bpe.spine[5].transform.localRotation = Quaternion.Euler(bpe.spine[4].transform.localRotation.eulerAngles.x / 2, 0, 0);
        // bpe.spine[6].transform.localRotation = Quaternion.Euler(bpe.spine[5].transform.localRotation.eulerAngles.x / 2, 0, 0);


        

        float spinTarget = twistAmount;
        Vector3 liftAmountRight = Vector3.up * ((rightPelvis.transform.position.y - root.transform.position.y));
        Vector3 liftAmountLeft = Vector3.up * ((leftPelvis.transform.position.y - root.transform.position.y));
        //rotate last spine, will rotate shoulders

        

        if (moveRight)
        {
            float thisLerp = rightLerp / 2;
            if (rightPhase == 1)
                thisLerp = rightLerp / 2 + 0.5f;
            float spinAmount = spinTarget * (thisLerp);
            spine3.transform.rotation = Quaternion.Euler(0, spinAmount * 2 - spinTarget, (spinAmount * 2 - spinTarget) / topHalfTwistRatio);

        }
        else
        {
            float thisLerp = leftLerp / 2;
            if (leftPhase == 1)
                thisLerp = leftLerp / 2 + 0.5f;
            float spinAmount = spinTarget * (thisLerp);
            spine3.transform.rotation = Quaternion.Euler(0, -spinAmount * 2 + spinTarget, (-spinAmount * 2 + spinTarget) / topHalfTwistRatio);

        }

        return;
        //match shoulder drop with hip lift/drop - perhaps could halve this movement?
        //add shoulder/drop/lift
        leftShoulder.transform.position = spine3.transform.position - spine3.transform.right * shoulderWidth + liftAmountRight;
        rightShoulder.transform.position = spine3.transform.position + spine3.transform.right * shoulderWidth + liftAmountLeft;

        //now clamp // necessary?
        Vector3 dirFromNeck = (leftShoulder.transform.position - spine3.transform.position).normalized * shoulderWidth;
        leftShoulder.transform.position = spine3.transform.position + dirFromNeck;

        //now clamp
        dirFromNeck = (rightShoulder.transform.position - spine3.transform.position).normalized * shoulderWidth;
        rightShoulder.transform.position = spine3.transform.position + dirFromNeck;

    }

    void Shoulders2()
    {
        float spineSegment = spineLength / GetComponent<BindPoseExample>().spine.Count;
        //neck
        spine0.transform.position = root.transform.position + Vector3.up * spineSegment * 1;

        
        for (int i = 0; i < bpe.spine.Count; i++)
        {

            float spinTarget =-(((bpe.spine.Count/2)-i+1) * twistAmount/2);
            float spinTargetZ = 0;// (topHalfTwistRatio/(bpe.spine.Count - i + 1));// * (i);
            if (moveRight)
            {
                float thisLerp = rightLerp / 2;
                if (rightPhase == 1)
                    thisLerp = -rightLerp / 2 + 0.5f;
                thisLerp = rightLerp;
                float spinAmount = spinTarget * (thisLerp);

                
                float spinAmountZ = (spinTargetZ * (thisLerp));
                //add to existing rotation
                if (i > bpe.spine.Count/2)
                {
                    spinAmount = -spinAmount;
                    spinTarget = -spinTarget;
                }


                if (rightPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount - spinTarget, spinAmountZ - spinTargetZ);//was root
                if (rightPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount, spinAmountZ);//was root

            }
            else
            {
                float thisLerp = leftLerp / 2;
                if (leftPhase == 1)
                    thisLerp = -leftLerp / 2 + 0.5f;
                thisLerp =leftLerp;

                float spinAmount = spinTarget * (thisLerp);
                float spinAmountZ = (spinTargetZ * (thisLerp));

                if (i > bpe.spine.Count/2)
                {
                    spinAmount = -spinAmount;
                    spinTarget = -spinTarget;
                }


                if (leftPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount + spinTarget, -spinAmountZ + spinTargetZ);//was -spinAmount * 2 + spinTarget for y
                if (leftPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount, -spinAmountZ);//was -spinAmount * 2 + spinTarget for y
            }
        }

    }

    void Shoulders3()
    {
        float spineSegment = spineLength / GetComponent<BindPoseExample>().spine.Count;
        //neck
        spine0.transform.position = root.transform.position + Vector3.up * spineSegment * 1;
        for (int i = 0; i < bpe.spine.Count / 2; i++)
        {
            float spinTarget = -(((bpe.spine.Count / 2) - i + 1) * twistAmount / 2);
            float spinTargetZ = -(((bpe.spine.Count / 2) - i + 1) * twistAmount / 2);
            if (moveRight)
            {
                float thisLerp = rightLerp / 2;
                if (rightPhase == 1)
                    thisLerp = -rightLerp / 2 + 0.5f;
                thisLerp = rightLerp;
                float spinAmount = spinTarget * (thisLerp);


                float spinAmountZ = (spinTargetZ * (thisLerp));
                //add to existing rotation
            

                if (rightPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount - spinTarget, spinAmountZ - spinTargetZ);//was root
                if (rightPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount, spinAmountZ);//was root

            }
            else
            {
                float thisLerp = leftLerp / 2;
                if (leftPhase == 1)
                    thisLerp = -leftLerp / 2 + 0.5f;
                thisLerp = leftLerp;

                float spinAmount = spinTarget * (thisLerp);
                float spinAmountZ = (spinTargetZ * (thisLerp));


                if (leftPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount + spinTarget, -spinAmountZ + spinTargetZ);//was -spinAmount * 2 + spinTarget for y
                if (leftPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount, -spinAmountZ);//was -spinAmount * 2 + spinTarget for y
            }
        }
    
        for (int i = bpe.spine.Count / 2; i < bpe.spine.Count; i++)
        {
            float spinTarget = twistAmount * i *0.5f*topHalfTwistRatio;///10;// -(((bpe.spine.Count / 2) - i + 1) * topHalfTwistRatio / 2);
            float spinTargetZ = i * topHalfTwistRatio;
            if (moveRight)
            {
                float thisLerp = rightLerp / 2;
                if (rightPhase == 1)
                    thisLerp = -rightLerp / 2 + 0.5f;
                thisLerp = rightLerp;
                float spinAmount = spinTarget * (thisLerp);


                float spinAmountZ = (spinTargetZ * (thisLerp));
                //add to existing rotation
             

                if (rightPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount - spinTarget, spinAmountZ - spinTargetZ);//was root
                if (rightPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, spinAmount, spinAmountZ);//was root

            }
            else
            {
                float thisLerp = leftLerp / 2;
                if (leftPhase == 1)
                    thisLerp = -leftLerp / 2 + 0.5f;
                thisLerp = leftLerp;

                float spinAmount = spinTarget * (thisLerp);
                float spinAmountZ = (spinTargetZ * (thisLerp));

             

                if (leftPhase == 0)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount + spinTarget, -spinAmountZ + spinTargetZ);//was -spinAmount * 2 + spinTarget for y
                if (leftPhase == 1)
                    bpe.spine[i].transform.rotation = root.transform.rotation * Quaternion.Euler(0, -spinAmount, -spinAmountZ);//was -spinAmount * 2 + spinTarget for y
            }

        }
    }

    void Arms()
    {
        //match arm swing with opposite leg, knee to hip angle
        //right arm
        //match with left leg
        // Vector3 toKneeFromPelvis = (leftKnee.transform.position - leftPelvis.transform.position).normalized;
        //move it behind the//roate to look backwards
        // toKneeFromPelvis = Quaternion.Euler(0, 180, 0)*toKneeFromPelvis;

        // rightElbow.transform.position = rightShoulder.transform.position + toKneeFromPelvis * upperArmLength;

        //  toKneeFromPelvis = (rightKnee.transform.position - rightPelvis.transform.position).normalized;
        //move it behind the//roate to look backwards
        //  toKneeFromPelvis = Quaternion.Euler(0, 180, 0) * toKneeFromPelvis;        
        //  leftElbow.transform.position = leftShoulder.transform.position + toKneeFromPelvis * upperArmLength;

        //come up with a method to match arm bend to other knee bend

        for (int i = 0; i < 2; i++)
        {
            // GameObject thisAnkle = leftAnkle;
            GameObject thisKnee = leftKnee;
            GameObject thisWrist = leftWrist;
            GameObject thisElbow = leftElbow;
            GameObject thisShoulder = leftShoulder;
            GameObject thisPelvis = leftPelvis;
            GameObject thisFoot = leftFoot;

            //   GameObject otherElbow = rightElbow;
            GameObject otherKnee = rightKnee;
            GameObject otherPelvis = rightPelvis;
            GameObject otherFoot = rightFoot;
            //  GameObject otherAnkle = rightAnkle;
            if (i == 1)
            {
                // thisAnkle = rightAnkle;
                thisKnee = rightKnee;
                thisWrist = rightWrist;
                thisElbow = rightElbow;
                thisShoulder = rightShoulder;
                thisPelvis = rightPelvis;
                thisFoot = rightFoot;

                // otherElbow = leftElbow;
                otherKnee = leftKnee;
                otherPelvis = leftPelvis;
                otherFoot = leftFoot;
                
                // otherAnkle = leftAnkle;
            }

            //upper arm
            Vector3 thisPelvisToThisKnee = ((thisKnee.transform.position - thisPelvis.transform.position)).normalized;
            Vector3 thisKneeToThisFoot = (thisFoot.transform.position - thisKnee.transform.position).normalized;

            Vector3 otherPelvisToOtherKnee = ((otherKnee.transform.position - otherPelvis.transform.position)).normalized;
            otherPelvisToOtherKnee = Quaternion.Euler(0, 180, 0) * otherPelvisToOtherKnee;
            Vector3 otherKneeToOtherFoot = ((otherFoot.transform.position - otherKnee.transform.position)).normalized;
            otherKneeToOtherFoot = Quaternion.Euler(0, 180, 0) * otherKneeToOtherFoot;

            
            Vector3 thisPelvisToThisFoot = ((thisFoot.transform.position - thisPelvis.transform.position)).normalized;
            Vector3 otherPelvisToOtherFoot = ((otherFoot.transform.position - otherPelvis.transform.position)).normalized;
            thisPelvisToThisFoot = Quaternion.Euler(0, 180, 0) * thisPelvisToThisFoot;

            thisPelvisToThisKnee = Quaternion.Euler(0, 180, 0) * thisPelvisToThisKnee;
            thisKneeToThisFoot = Quaternion.Euler(0, 180, 0) * thisKneeToThisFoot;

            thisElbow.transform.position = thisShoulder.transform.position + otherPelvisToOtherFoot * upperArmLength;
            //thisElbow.transform.LookAt(bpe.spine[6].transform.position);
            //thisElbow.transform.rotation *= Quaternion.Euler(90,0 , 0);

            //lower


            thisWrist.transform.position = thisElbow.transform.position + thisKneeToThisFoot * forearmLength;
            //thisWrist.transform.position = thisElbow.transform.position;

            
        }



      //  rightElbow.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
      //  leftAnkle.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Brown") as Material;
      //  leftFoot.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Brown") as Material;
    }

    void UnParentElements()
    {
        //parent de parent
        List<GameObject> tempBones = new List<GameObject>();
        for (int i = 0; i < root.transform.childCount; i++)
        {
            if (root.transform.GetChild(i).name == "RootCube" || root.transform.GetChild(i).gameObject == gyro)
                //skip debug cube and gyro rotaion cube
                continue;

            tempBones.Add(root.transform.GetChild(i).gameObject);
            root.transform.GetChild(i).transform.parent = null;
        }

        //debug cube
        if(GetComponent<BindPoseExample>().skipDebugBones == false)
            if (root.transform.GetChild(0) != null)
                root.transform.GetChild(0).transform.parent = root.transform;
    }

    void GatherElements()
    {
        //makes sure all bones are parented to the parent object and that the parent object itself is relative to the bones' positions
        //the reason for this is we change foot and ankle parents around and de parent them- they move independently from the rig or root, we then match the root up to the feet

        //transform is always at vec3.zero. I tried to get it move with the other bones but glitches were happening. Does it matter if it stays at vec3.zero? can use root.position for character position

        leftPelvis.transform.parent = transform;
        rightPelvis.transform.parent = transform;

        leftKnee.transform.parent = transform;
        rightKnee.transform.parent = transform;

        if (leftFootPivot == leftAnkle)
            leftAnkle.transform.SetParent(transform);
        else if (leftFootPivot == leftFoot)
            leftFoot.transform.SetParent(transform);

        if (rightFootPivot == rightAnkle)
            rightAnkle.transform.SetParent(transform);
        else if (rightFootPivot == rightFoot)
            rightFoot.transform.SetParent(transform);
    }

    public Vector3 StandardWalkLiftPoint(Vector3 ankleStart, Vector3 footTarget,int i, int phase)
    {
        //work out where to place centre of arc for foot lift
        //always make lift point dead ahead, this way the knee wont try and go through the other


        //half way between start and target plus half a footheight-old
        //Vector3 temp = Vector3.Lerp(ankleStart, footTarget, 0.5f); //-old
        //Vector3 liftTarget = new Vector3(temp.x, (temp.y + footHeight / 2), temp.z);..-old
        Vector3 liftTarget = new Vector3();

        GameObject pivot = rightFootPivot;
        if (i == 0)
            pivot = leftFootPivot;

        float yPos = pivot.transform.position.y + (footLength);
        //consider if feet are on different y levels
        float yDiff = 0f;
        if (i == 0)
        {
            if (phase == 0)
            {
                liftTarget = leftFootPivot.transform.position + footLength * Vector3.up + root.transform.right * gaitWidth * 2;
                //create offset
                yDiff = (rightFootTarget.y - leftAnkleStart.y);
                liftTarget.y = yPos + yDiff; ;
            }
            else
                //dont change on second half of stride
                liftTarget = rightLiftTarget;

        }
        else if ( i==1)
        {
            if (phase == 0)
            {
                liftTarget = rightFootPivot.transform.position + footLength * Vector3.up - root.transform.right * gaitWidth * 2;
                //create offset
                yDiff = (leftFootTarget.y - rightAnkleStart.y);
                liftTarget.y = yPos + yDiff; ;
            }
            else
                //dont change on second half of stride
                liftTarget = leftLiftTarget;
        }

        //make sure lift target can't go below ankle start - will go through flors/edges
        //catch
        if(liftTarget.y < ankleStart.y)
        {
            if (printLogs)
                Debug.Log("Altering Y to match ankle start on lift target");

            liftTarget.y = (ankleStart.y + footTarget.y) * 0.5f + (footLength * 0.5f);
        }


        

        return liftTarget;

    }
   
    public Vector3 SetTargetToe(out bool targetFound, Vector3 footTarget,int phase)
    {
        bool returnFound = false;

         
        Vector3 toeTarget = new Vector3();
        //predict user input and alter dection
        Vector3 forwardDir = root.transform.forward;

        //spin less on heel, more on toe
        float spin = ccp.rotSpeed * 60;// ccp.sideStepAmount + ccp.sideStepAmount;
        if (phase == 1)
            spin /= 3;

        //apply user input- stops toe pointing inwards after user lets go of button 
        //left or right
        /*
        Vector3 startingPoint = new Vector3();
        if (footTarget == rightFootTarget)
        {
            spin *= ccp.rightAmount;
            startingPoint = rightAnkle.transform.position;
        }
        if (footTarget == leftFootTarget)
        {
            spin *= ccp.leftAmount;
            startingPoint = leftAnkle.transform.position;
        }
        
        if (!ccp.targetSameAsRoot)
        {
            if (ccp.leadWithRight)
                forwardDir = Quaternion.Euler(0, spin, 0) * forwardDir;
            else
                forwardDir = Quaternion.Euler(0, -spin, 0) * forwardDir;
        }
        */
        Vector3 rayStart = footTarget + forwardDir * footLength + Vector3.up * (footLength+shinLength+thighLength);//was using foot target, trying foot position - to put back, replace starting point with foot target
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, (footLength + shinLength + thighLength) * 2)) 
        {
            toeTarget = hit.point;
            returnFound = true;
        }
        else
        {
            Debug.Log("no toe target");
         
         
        }

      //  if (drawDebug)
        {
            Debug.DrawLine(toeTarget, toeTarget + Vector3.up, Color.magenta);
        }

        targetFound = returnFound;

        return toeTarget;
    }

    public void RotateStandingFoot(GameObject pivot, Vector3 footTarget, Vector3 toeTarget, int phase)
    {
        //perhaps make this linked to hip swivel?
        float heelFlickAmount = footLength*.5f;

        //point towards ground on first half of swing
        Vector3 targetDir = Vector3.zero;

        bool lastStride = false;
        if (phase == 0)
            lastStride = lastStrideRight;
        else
            lastStride = lastStrideLeft;

        if (phase == 0)
        {
            if(!lastStride)
                targetDir = toeTarget - (pivot.transform.position);
            else if (lastStride)
                targetDir =  toeTarget - (footTarget);
        }
        else if (phase == 1)
        {
            if (!lastStride)
                targetDir = - Vector3.up * heelFlickAmount + root.transform.forward*heelFlickAmount;
            else if (lastStride)
                targetDir = toeTarget - (footTarget);
        }
        //how fast to rotate foot, slower on heel raise phase, faster on phase where planting foot
        float step = walkSpeed*.5f * Time.deltaTime;//was 0.5f
        if (phase == 0)
            step *= 2;

        Vector3 newDir = Vector3.RotateTowards(pivot.transform.forward, targetDir, step, 0.0F);
        pivot.transform.rotation = Quaternion.LookRotation(newDir);

       

        //dont need now we are checking evry frame for target
        //point to look at root's rotation
        // Quaternion newRot = root.transform.rotation;
        // pivot.transform.rotation = Quaternion.Euler(pivot.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, pivot.transform.rotation.eulerAngles.z);

        // Debug.DrawRay(toeTarget,Vector3.up);

         //Debug.DrawRay(pivot.transform.position, newDir, Color.red);
    }
    public void RotateWalkingFoot(GameObject pivot, Vector3 footTarget, int phase,bool moveRight)
    {
        //perhaps make this linked to hip swivel?
        
        //point towards ground on first half of swing
        Vector3 targetDir = (footTarget + Vector3.up) - leftAnkleStart;

        bool lastStride = false;
        if (phase == 0)
            lastStride = lastStrideRight;
        else
            lastStride = lastStrideLeft;

        if (phase == 0 && !lastStride)
        {
            if (moveRight)
            {
                targetDir = (rightLiftTarget - Vector3.up * footLength * 10) - pivot.transform.position; //just make it point down on first half on normal stride
            }
            else
            {
                targetDir = (leftLiftTarget - Vector3.up * footLength * 10) - pivot.transform.position;
            }

        }
        else if (phase == 0 && lastStride)
        {
            targetDir = footTarget - (footTarget - root.transform.forward);           
            
        }
        else if (phase == 1 && !lastStride)
        {
            targetDir = footTarget + Vector3.up * footLength * 2f - pivot.transform.position + root.transform.forward * footLength ;
        }
        else if(phase == 1 && lastStride)
        {
            if(moveRight)
                targetDir = rightAnkle.transform.position - (rightAnkle.transform.position - root.transform.forward);
            else if (!moveRight)
                targetDir = leftAnkle.transform.position - (leftAnkle.transform.position - root.transform.forward);
        }

        float step = walkSpeed* Time.deltaTime ;
        if (lastStride && phase == 0)
            step *= 2;

        Vector3 newDir = Vector3.RotateTowards(pivot.transform.forward, targetDir, step, 0.0F);

        //predict user input and alter dection
        Vector3 forwardDir = newDir;

        //spin this with user input -- needed?
        float spin = ccp.rotSpeed * ccp.sideStepAmount + ccp.sideStepAmount;

        if (ccp.rightAmount > 0f)
            forwardDir = Quaternion.Euler(0, spin, 0) * forwardDir;
        if (ccp.leftAmount > 0f)
            forwardDir = Quaternion.Euler(0, -spin, 0) * forwardDir;
        //pivot.transform.rotation = Quaternion.Euler(pivot.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, root.transform.rotation.eulerAngles.z);
        pivot.transform.rotation = Quaternion.LookRotation(newDir);//using old

        //point to look at root's rotation
        //        Quaternion newRot = root.transform.rotation;
        //  pivot.transform.rotation = Quaternion.Euler(pivot.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, pivot.transform.rotation.eulerAngles.z);

    }
    public void MoveFoot(GameObject footPivot, Vector3 liftTarget, Vector3 ankleStart, Vector3 footTarget, int phase, float lerp)
    {
       // if (lastStride)
       //     return;

        //if (lastStride)
       //     ankleStart = lastStrideStart;
        //adjust how high to lift
        float liftAmount = bounce;// bounce;//?0.1?bounce var?

        Vector3 ankleTemp = Vector3.up * lerp * liftAmount;
        if (phase == 1)
            ankleTemp = Vector3.up * lerp * liftAmount;

        //if (lastStride && phase == 1)
        //    liftAmount = 0f;

      // if (lastStride)
      //      Debug.Break();
       // if(lastStride)
      //      liftAmount = 0;//test // maybe we need to raycast for target? - this pus foot on even level as other?

        Vector3 target = new Vector3();
        if (phase == 0)
        {
            target = Vector3.Lerp(ankleStart + ankleTemp, liftTarget + Vector3.up * liftAmount, lerp);
        }
        else
        {
            target = Vector3.Lerp(liftTarget + Vector3.up * liftAmount, footTarget, lerp);
        }

        footPivot.transform.position = target;
    }
    public void AdjustTimersAndFlags(int i)
    {
        //add lerps and reset any flags we need to
        //this controls which phase we are in and what leg is swinging
        if (moveRight && i == 0)
        {
            if (!rightIdle)//was just idle
            {
              //  if foot needs pulled back towards other foot, do it slower
              //  if (lastStride && rightPhase == 1)
              //      rightLerp += Time.deltaTime * walkSpeed*2f;
            //    else
                    //add to timer normally
                    rightLerp += Time.deltaTime * walkSpeed;
            }

            
            if (rightLerp >= 1f + Time.deltaTime * walkSpeed)//testing, allows stride to complete
            {
                if (rightPhase == 0)
                {
                    //move to next phase of this step
                    rightPhase++;
                    rightLerp = 0f;

                    //make other foot swivel on toe
                 
                    MakeToeParent(leftAnkle, leftFoot, leftFootPivot);
                    leftFootPivot = leftFoot;

                    if (lastStrideRight)
                    {
                        lastStrideRight = false;
                        firstStrideRight = true;
                        //   idle = true;
                        
                        rightIdle = true;
                        leftIdle = true;
                        rightPhase = 0;
                        rightAnkleStart = rightAnkle.transform.position;
                        
                    }

                    if (rightIdle)
                    {
                        //if idle, only do first phase then swap
                       // moveRight = false;
                       // rightPhase = 0;
                       // rightLerp = 0f;
                    }
                    
                }
                else if(rightPhase == 1)
                {
                    //switch to other foot moving
                    moveRight = false;
                    rightPhase = 0;
                    rightLerp = 0f;

                    leftAnkleStart = leftAnkle.transform.position;
                    //swap pivot on foot to ankle                                    
                    MakeAnkleParent(leftAnkle, leftFoot, leftFootPivot);
                    leftFootPivot = leftAnkle;
                    //save left ankle position for next stride
                    rightAnkleStart = rightAnkle.transform.position;
                    

                    if (firstStrideRight)
                        firstStrideRight = false;

                    if (lastStrideRight)
                    {
                        lastStrideRight = false;
                        firstStrideRight = true;
                      //  idle = true;
                        leftIdle = true;
                        rightIdle = true;
                        rightAnkleStart = rightAnkle.transform.position;
                    }
                    if (rightFootAtDrop)
                        rightIdle = true;


                    edgeNearOtherFootFound = false;

                }
                //this is used for last stride, we can reset now
                stopping = false;
                lastStrideStartSet = false;
            }
        }
        else if (!moveRight && i == 1)
        {
            if (!leftIdle)
            {
                //if foot needs pulled back towards other foot, do it slower
                //  if(lastStride && leftPhase ==1)
                //      leftLerp += Time.deltaTime * walkSpeed*2f;
                //  else
                //add to timer normally
                leftLerp += Time.deltaTime  * walkSpeed;
            }

            if (leftLerp >= 1f + Time.deltaTime*walkSpeed) //testing, allows stride to fully complete
            {
                if (leftPhase == 0)
                {
                    leftLerp = 0f;
                    leftPhase++;
                    //make other foot swivel on toe
                   
                    MakeToeParent(rightAnkle, rightFoot, rightFootPivot);
                    rightFootPivot = rightFoot;

                    if (lastStrideLeft)
                    {
                        lastStrideLeft = false;
                        firstStrideLeft = true;
                      //  idle = true;
                        rightIdle = true;
                        leftIdle = true;
                        leftPhase = 0;
                        leftAnkleStart = leftAnkle.transform.position;
                    }
                    if (leftIdle)
                    {
                        //only  do first phase then swap foot
                      //  moveRight = true;
                      //  leftPhase = 0;
                      //  leftLerp = 0f;
                    }

                }
                else if(leftPhase == 1)
                {
                    moveRight = true;
                    leftPhase = 0;
                    leftLerp = 0f;

                    rightAnkleStart = rightAnkle.transform.position;                   
                    MakeAnkleParent(rightAnkle, rightFoot, rightFootPivot);
                    rightFootPivot = rightAnkle;
                    leftAnkleStart = leftAnkle.transform.position;

                    if (firstStrideLeft)
                        firstStrideLeft = false;

                    if (lastStrideLeft)
                    {
                        lastStrideLeft = false;
                        firstStrideLeft = true;
                       // idle = true;
                        rightIdle = true;
                        leftIdle = true;
                        leftAnkleStart = leftAnkle.transform.position;                        
                    }

                    

                    if (leftFootAtDrop)
                        leftIdle = true;

                    edgeNearOtherFootFound = false;
                }

                //this is used for last stride, we can reset now
                stopping = false;
                lastStrideStartSet = false;
            }
        }
    }
    public void MakeToeParent(GameObject ankle, GameObject foot, GameObject footPivot)
    {
        //make other foot swivel on toe
        ankle.transform.parent = null;
        foot.transform.parent = null;
        foot.transform.parent = transform;
        ankle.transform.parent = foot.transform;

    }
    public void MakeAnkleParent(GameObject ankle, GameObject foot, GameObject footPivot)
    {
        //swap pivot on foot to ankle
        ankle.transform.parent = null;
        foot.transform.parent = null;
        ankle.transform.parent = transform;
        foot.transform.parent = ankle.transform;
    }

    public Vector3 SetTargetHeel(Vector3 footTarget)
    {
        //used when we need to move foot back a bit if we are about o overhang on an edge
        Vector3 heelTarget = new Vector3();

        //allows space for tow to look at a point in front of foot
        float spaceBehind = 1.01f;
        Vector3 rayStart = footTarget - root.transform.forward * footLength*spaceBehind + Vector3.up *( shinLength+thighLength+footHeight);//??10
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
            heelTarget = hit.point;
        }
        else
        {
            //we ahve an overhang too steep a hill, stop at the top
            //heelTarget = footTarget - root.transform.forward * footLength;
            Debug.Log("no heel target");
        }
            

        return heelTarget;
    }

    public void CheckForObjectBetweenHeelAndToe(GameObject ankle, Vector3 footTarget, Vector3 toeTarget)
    {
        //do a raycast between foot and toe target. If it hits, we have the edge of a stair or a bump.
        //Place toe on stair or bump and make toe the pivot
        RaycastHit hit;
        Vector3 dir = toeTarget - footTarget;
        Debug.Break();

        // Debug.DrawLine(footTarget, footTarget + Vector3.up, Color.blue);
        // Debug.DrawLine(toeTarget, toeTarget + Vector3.up, Color.magenta);
        if (Physics.Raycast(footTarget, dir, out hit, dir.magnitude))
        {
            Debug.Log("hit");


            // Debug.DrawLine(hit.point, hit.point + Vector3.up);


            if (ankle == rightAnkle)
            {
                ///rightToeTarget = rightFootTarget;//umm, it doesn't need this line, if we make the toe he pivot it does it already. If we put this line in it moves it back another foot?
                //rightFootTarget = rightToeTarget - root.transform.forward * footLength;
                MakeToeParent(rightAnkle, rightFoot, rightFootPivot);
                rightFootPivot = rightFoot;
               // rightAnkleStart = rightFoot.transform.position;
            }
            else if (ankle == leftAnkle)
            {
                // leftToeTarget = leftFootTarget;
                // leftFootTarget = leftToeTarget - root.transform.forward * footLength;
                MakeToeParent(leftAnkle, leftFoot, leftFootPivot);
                leftFootPivot = leftFoot;
              //  leftAnkleStart = leftFoot.transform.position;
            }
        }


    }

    void CheckForGradientChange(int i,int phase)
    {
        
       // Debug.Break();

        float strideLength = strideLengthVar;
      //  if (firstStride)
      //      strideLength /= 2;//dont need now

        //check to see if we need to do a final half stride - do this before we bother raycasting ahead for objects
       // if (lastStride)
      //  {
            if (i == 0 && lastStrideRight)
            {
                if (!lastStrideStartSet)
                {
                    //make current positoin the start of a new lerp from this position to standing position
                   // lastStrideStart = rightFootPivot.transform.position;
                    rightAnkleStart = rightFootPivot.transform.position;
                    rightLerp = 0f;
                    lastStrideStartSet = true;
                }
                //target is set to stand beside other foot
                rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2; //aws left target + .
                if (phase == 0)
                {
                    //first half target is lift target, so this becomes our target
                    rightLiftTarget = rightFootTarget;// StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
                }
                else if (phase == 1)
                {
                    //this forces the final stride walk, move function checks for final stride but only uses phase 0 (To be cleared up)
                    rightPhase = 0;
                }
                bool found = false;
                rightToeTarget = SetTargetToe(out found, rightFootTarget, phase);
                return;
            }
            else if(i == 1 && lastStrideLeft)
            {                
                if (!lastStrideStartSet)
                {
                   // lastStrideStart = leftFootPivot.transform.position;
                    leftAnkleStart = leftFootPivot.transform.position;
                    leftLerp = 0f;
                    lastStrideStartSet = true;

                  //  Debug.DrawLine(lastStrideStart, lastStrideStart + Vector3.up, Color.red);
                  //  Debug.DrawLine(leftFootTarget, leftFootTarget + Vector3.up, Color.magenta);
                  //  Debug.Break();
                }

                leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2;
                if (phase == 0)
                {
                    leftLiftTarget = leftFootTarget;// StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);
                }
                else if (phase == 1)
                {
                    leftPhase = 0;
                }
            bool found = false;
            leftToeTarget = SetTargetToe(out found, leftFootTarget, phase);
                return;
            }
           
          
       // }

        

        Vector3 footTarget = Vector3.zero;
        //shoot rays from root/centre of body/pelvis and rotate downwards until we hit something

        List<Vector3> hits = new List<Vector3>();
        RaycastHit hit;

        //this loop moves target down, and the towards player
        for (float j = 0, k = 0; k < strideLength;)
        {

            Vector3 rayStart = new Vector3();  //root.transform.position - thighLength * root.transform.forward;
            //left or right foot?
            if (i == 0)
                rayStart = leftAnkleStart + root.transform.right * gaitWidth * 2;// - strideLengthVar*0.25f*root.transform.forward;//+= root.transform.right * gaitWidth;
            else
                rayStart = rightAnkleStart - root.transform.right * gaitWidth * 2;// - strideLengthVar*0.25f * root.transform.forward;//-= root.transform.right * gaitWidth;

           // rayStart -= root.transform.forward * footLength;
            //use other foot pivot as height (root, moves about) - oither pivot is planted
            GameObject pivot = rightFootPivot;
            GameObject otherPivot = leftFootPivot;
            if (i == 0)
            {
                pivot = leftFootPivot;
                otherPivot = rightFootPivot;
            }

            //use other foot's height

            if(i==0)
                rayStart.y = leftAnkleStart.y + (shinLength + thighLength + footHeight);
            else if( i==1)
                rayStart.y = rightAnkleStart.y + (shinLength + thighLength + footHeight);

            //consider where the user is trying to go, add any input to the forward ray (rotate the ray more thank just "forward")

            Vector3 forwardDir = root.transform.forward;
            
            //tank controls, set var to 0 in control script atm
            //spin this with user input -- needed?
            float spin = ccp.rotSpeed*ccp.sideStepAmount + ccp.sideStepAmount;

            if (ccp.rightAmount > 0f)
                forwardDir = Quaternion.Euler(0, spin, 0)*forwardDir;
            if (ccp.leftAmount > 0f)
                forwardDir = Quaternion.Euler(0, -spin, 0) * forwardDir;


            Vector3 rayEnd = rayStart + forwardDir * (strideLength*1f);// + thighLength));
            rayEnd -= Vector3.up * j;
            rayEnd -= forwardDir * k;
            Vector3 rayDirection = (rayEnd - rayStart).normalized;// Quaternion.Euler(j, 0, 0) * root.transform.forward;
            float rayLength = (rayEnd - rayStart).magnitude;

            //check if ray end is behind foot, if it is stop.
            Vector3 dirToAnkle = (otherPivot.transform.position - rayEnd).normalized;
            float dot = Vector3.Dot(dirToAnkle, root.transform.forward);
            //Debug.Log("dot = " + dot);
            if (dot > 0)
            {
              //  break;
            }
            if(drawDebug)
                Debug.DrawLine(rayStart, rayStart + (rayDirection * rayLength),Color.grey);
            if (Physics.Raycast(rayStart, rayDirection, out hit, rayLength))
            {
              //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //   c.transform.position = hit.point;
            //    c.transform.localScale *= 0.05f;
             //    c.GetComponent<BoxCollider>().enabled = false;

                hits.Add(hit.point);

                //if hits count i> 2 work out ablge here for optimisiation****
            }
            

            //drop amount
            float drop = thighLength * 2 + shinLength * 2;
            //move down, and then once down, move back
            if (j >= drop)
                k += 0.1f;
            else
                j += 0.1f;
        }

        //calculate angle between the three last hits(including this one)
        //create dir vectors going away from middle point(2nd hit)

        if (hits.Count == 0)
        {
            //save list to check for next step
            lastHits = hits;

            if (printLogs)
                Debug.Log("hits count = 0, setting beside other foot and returning");

            //if we were at an edge previously with this foot
            //don't try and move, player needs to spin character more
            

            //we are at an edge - disable other foot (kinda)
            if (i == 1)
                rightIdle = true;
            else if (i == 0)
                leftIdle = true;

            //put foot beside other
            if (i == 0 && !rightIdle)
            {
                Debug.Log("putting foot beside other");
                //target is set to stand beside other foot
                rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2;                    
                
                bool found = false;
                rightToeTarget = SetTargetToe(out found, rightFootTarget, phase);
                if(found == false)
                {
                    Debug.Log("moving target back");
                    rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2 - root.transform.forward * footLength;
                    rightToeTarget = SetTargetToe(out found, rightFootTarget-root.transform.forward*footLength, phase);
                }
                if (found == false)
                {
                    Debug.Log("moving target back 2");
                    rightToeTarget = SetTargetToe(out found, rightFootTarget - root.transform.forward * footLength*2, phase);
                    rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2 - root.transform.forward * footLength*2;
                }

                if (found == false)
                {
                    Debug.Log("moving target back 3");
                    rightToeTarget = SetTargetToe(out found, rightFootTarget - root.transform.forward * footLength * 3, phase);
                    rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2 - root.transform.forward * footLength * 3;
                }
                if (found == false)
                {
                    Debug.DrawLine(rightFootTarget, rightFootTarget + Vector3.up, Color.blue);
                    //Debug.Break();
                    Debug.Log("still not found toe target - idling right");
                    rightIdle = true;
                }

                rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
            }
            else if (i == 1 &&!leftIdle)
            {
                Debug.Log("putting foot beside other");
                leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2;               
                
                bool found = false;
                leftToeTarget = SetTargetToe(out found,leftFootTarget, phase);
                if (found == false)
                {
                    Debug.Log("moving target back");
                    leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2 - root.transform.forward*footLength;
                    leftToeTarget = SetTargetToe(out found, leftFootTarget - root.transform.forward * footLength, phase);
                }
                if (found == false)
                {
                    leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2 - root.transform.forward * footLength*2;
                    leftToeTarget = SetTargetToe(out found, leftFootTarget - root.transform.forward * footLength, phase);
                    Debug.Log("still no toe target 2");
                }
                if (found == false)
                {
                    leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 3 - root.transform.forward * footLength * 3;
                    leftToeTarget = SetTargetToe(out found, leftFootTarget - root.transform.forward * footLength, phase);
                    Debug.Log("still no toe target 3");
                }
                if (found == false)
                {
                 //   Debug.Break();
                    Debug.Log("still not found toe target - idling left");

                    leftIdle = true;
                }
                leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);
            }
            
            return;
        }
        else if(hits.Count > 0)
        {
            //if we have hits and foot is not at drop, and we are trying to lead with the correct foot(foot nearest target direction)
            if (i == 0)
            {
                if (rightFootAtDrop)
                {
                    if (ccp.leadWithRight && !ccp.targetSameAsRoot)///*******
                    {
                        
                      //  rightIdle = false;
                      //  Debug.Break();
                        
                        
                    }
                }
                else if(!rightFootAtDrop && !leftFootAtDrop)//was jsut !rightfoot drop
                {
                //    rightIdle = false;
                }
            }
            else if(i == 1)
            {
                if(leftFootAtDrop)
                {
                    if(!ccp.leadWithRight && !ccp.targetSameAsRoot)
                    {
                       // leftIdle = false;
                        
                    }
                }
                else if (!rightFootAtDrop && !leftFootAtDrop)//was jsut !leffoot drop
                {
                    
                  //  leftIdle = false;
                }
            }

         
            //then create lift and target points
            CalculateSteps4(hits, i,phase);

            return;
        }
    }


    void CalculateSteps3(List<Vector3> hits, int i, int phase)
    {

        ccp.climbing = true;
        bool edgeFound = false;
        List<Vector3> edges = new List<Vector3>();
        List<Vector3> pointBeforeEdges = new List<Vector3>();

        //Vector3 edgeNearOtherFoot = Vector3.zero;
        //work out if there any edges we need to consider
        bool wallFound = false;
        Vector3 wallPoint = Vector3.zero;
        for (int j = 0; j < hits.Count - 2; j++)
        {
            Vector3 dir1 = hits[j] - hits[j + 1];
            Vector3 dir2 = hits[j + 2] - hits[j + 1];

            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
            // Debug.Log(angle);
            //180 is flat

            //if we find an edge first
            if (angle > 0 && angle < 180 - maxAngle)
            {
                edgeFound = true;

                RaycastHit hit;
                //find the actual edge point within another raycast
                float sphereSize = 0.01f; //if we get more misses, make larger?
                if (Physics.SphereCast(hits[j + 1] + Vector3.up * footLength, sphereSize, Vector3.down, out hit, footLength * 2))
                {
                 //   if (i == 0)
                 //       rightFootTarget = Vector3.Lerp(rightFootTarget, hit.point, targetLerpSpeed);
                 //   else
                 //       leftFootTarget = Vector3.Lerp(leftFootTarget, hit.point, targetLerpSpeed);
                }
                else
                {
                    if (printLogs)
                        Debug.Log("miss");
                    Debug.Break();
                }
                //add edge if not too close to other standing foot
                Vector3 otherFootPos = leftAnkleStart;
                Vector3 thisFootPos = rightAnkleStart;
                if (i == 1)
                {
                    otherFootPos = rightAnkleStart;
                    thisFootPos = leftAnkleStart;
                }
                float distanceToOther = Vector3.Distance(hit.point, otherFootPos);
                float distanceToThis = Vector3.Distance(hit.point, thisFootPos);
                // Debug.Log("distance" + distanceToOther);

                if (distanceToOther > gaitWidth * 2 + bpe.footWidth && distanceToThis > gaitWidth * 2 + bpe.footWidth)//accuracy, ankle width?
                {
                    edges.Add(hit.point);
                    pointBeforeEdges.Add(hits[j]);
                    if (drawDebug)
                    {
                        Debug.DrawLine(hits[j], hits[j + 1], Color.magenta);
                        Debug.DrawLine(hits[j + 2], hits[j + 1], Color.magenta);
                    }
                }
                else
                {
                    if (drawDebug)
                    {
                        Debug.DrawLine(hits[j], hits[j + 1], Color.green);
                        Debug.DrawLine(hits[j + 2], hits[j + 1], Color.green);
                    }
                    //we have found the other foot, we don't need to ckeep looking behind, we want to go forward
                    //remmeber this point, we might need it**//not raycasting behind foot now - probs could remove the distance checks**OPTIMISATION

                    //set flag at start of step
                    if (i == 0 && rightLerp == 0f || i == 1 && leftLerp == 0f)
                    {
                        edgeNearOtherFoot = hit.point;
                        edgeNearOtherFootFound = true;
                    }

                    break;
                }
            }
            if (angle < 0 && angle > -180 + maxAngle && edges.Count == 0)
            {
                //if acute angle was found 
                //we have found a wall basically. save point

                wallFound = true;
                wallPoint = hits[j + 1];
                if (printLogs)
                    Debug.Log("wall found");

            }
        }
        Debug.Log("Edge Found = " + edgeFound);
        if (edgeFound)
        {

            //if we found an edge but no suitable edges were found, it means the only edge we came across was the edge the other foot was standing on. We can step over this foot and on to the original target point
            if (edges.Count == 0)
            {

                if (i == 0)
                {
                    //Debug.Break();
                    // Debug.Log("Broke here");

                    if (wallFound)
                    {
                        if (wallPoint.y > rightAnkleStart.y)
                        {
                            //going up
                            if (printLogs)
                                Debug.Log("edges count = 0,wall found, going up");
                            //place foot at a nice distance between the wall and the other foot
                            rightToeTarget = Vector3.Lerp(edgeNearOtherFoot, wallPoint, 0.5f);
                            rightFootTarget = Vector3.Lerp(rightFootTarget, SetTargetHeel(rightToeTarget), targetLerpSpeed);
                            bool found = false;
                            rightToeTarget = SetTargetToe(out found, rightFootTarget, rightPhase);
                            rightLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;


                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0,wall found, going down");
                            //land with heel
                            rightToeTarget = Vector3.Lerp(rightToeTarget, hits[0], targetLerpSpeed);
                            rightFootTarget = SetTargetHeel(rightToeTarget);
                            rightLiftTarget = edgeNearOtherFoot + Vector3.up * footLength * 1.5f;


                            return;
                        }
                    }
                    else
                    {
                        //place toe at original target                        
                        //check if edge nearest other foot is below this foot, if above use edge point, if not, we ahve most likely
                        //cleared the edge already
                        if (edgeNearOtherFoot.y > rightAnkleStart.y)
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0, wall not found,going up");

                            //check if slope is not too much ont he other side of the edge

                            //check if angle is too step to rey and put foot on
                            //check if hill is too steep
                            Vector3 dir1 = root.transform.forward;
                            Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                            if (i == 1)
                                dir2 = (rightToeTarget - rightFootTarget).normalized;

                            Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                            Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);

                            if (angle > maxAngleSlope || rightFootAtDrop)
                            {
                                //Debug.Break();
                                //place foot at the edge
                                rightToeTarget = edgeNearOtherFoot;
                                rightFootTarget = SetTargetHeel(rightToeTarget);
                                rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);

                                rightFootAtDrop = true;

                                Debug.Log("1");
                            }
                            else
                            {
                                rightLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;
                                rightToeTarget = Vector3.Lerp(leftToeTarget, hits[0], targetLerpSpeed);
                                rightFootTarget = SetTargetHeel(rightToeTarget);

                                Debug.Log("2");
                            }

                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0,wall not found, going down");

                            //check if angle is too step to rey and put foot on
                            //check if hill is too steep
                            Vector3 dir1 = root.transform.forward;
                            Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                            if (i == 1)
                                dir2 = (rightToeTarget - rightFootTarget).normalized;

                            Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                            Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                            if (angle > maxAngleSlope || rightFootAtDrop)
                            {
                                Debug.Log("slope too steep, placing beside other");
                                //place next to other foot, it looks like we found a drop too large
                                // target is set to stand beside other foot
                                rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2;
                                rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
                                bool found = false;
                                rightToeTarget = SetTargetToe(out found, rightFootTarget, phase);

                                rightFootAtDrop = true;
                                return;
                            }
                            else
                            {

                                rightToeTarget = Vector3.Lerp(rightToeTarget, hits[0], targetLerpSpeed);
                                rightFootTarget = SetTargetHeel(rightToeTarget);

                                rightLiftTarget = Vector3.Lerp(hits[0], rightAnkleStart, 0.5f);// edgeNearOtherFoot + Vector3.up * footLength * 1.5f;
                                rightLiftTarget.y = rightAnkleStart.y;
                                Debug.Log("no edges, using hit point");

                                return;
                            }
                        }
                    }
                }
                else if (i == 1)
                {
                    //  Debug.Break();
                    // Debug.Log("Broke here");
                    if (wallFound)
                    {
                        if (wallPoint.y > leftAnkleStart.y)
                        {
                            //going up
                            if (printLogs)
                                Debug.Log("edges count = 0,wall found, going up");
                            //place foot at a nice distance between the wall and the other foot
                            leftToeTarget = Vector3.Lerp(leftToeTarget, Vector3.Lerp(edgeNearOtherFoot, wallPoint, 0.5f), targetLerpSpeed);
                            leftFootTarget = SetTargetHeel(leftToeTarget);
                            bool found = false;
                            leftToeTarget = SetTargetToe(out found, leftFootTarget, leftPhase);
                            leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;

                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0,wall found, going down");
                            //land with heel
                            leftToeTarget = Vector3.Lerp(leftToeTarget, hits[0], targetLerpSpeed);
                            leftFootTarget = SetTargetHeel(leftToeTarget);
                            leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength * 1.5f;


                            return;
                        }
                    }
                    else
                    {
                        //place toe at original target

                        //check if edge nearest other foot is below this foot, if above use edge point, if not, we ahve most likely
                        //cleared the edge already
                        if (edgeNearOtherFoot.y > leftAnkleStart.y)
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0, wall not found,going up");

                            //check if slope is not too much ont he other side of the edge

                            //check if angle is too step to rey and put foot on
                            //check if hill is too steep
                            Vector3 dir1 = root.transform.forward;
                            Vector3 dir2 = (leftToeTarget - leftFootTarget).normalized;
                            if (i == 1)
                                dir2 = (leftToeTarget - leftFootTarget).normalized;

                            Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                            Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);

                            if (angle > maxAngleSlope || leftFootAtDrop)
                            {
                                //Debug.Break();
                                //place foot at the edge
                                leftToeTarget = edgeNearOtherFoot;
                                leftFootTarget = SetTargetHeel(leftToeTarget);
                                leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);

                                leftFootAtDrop = true;

                                Debug.Log("1");
                            }
                            else
                            {
                                leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;
                                leftToeTarget = Vector3.Lerp(leftToeTarget, hits[0], targetLerpSpeed);
                                leftFootTarget = SetTargetHeel(leftToeTarget);

                                Debug.Log("2");
                            }


                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("edges count = 0,wall not found, going down");

                            //check if angle is too step to rey and put foot on
                            //check if hill is too steep
                            Vector3 dir1 = root.transform.forward;
                            Vector3 dir2 = (leftToeTarget - leftFootTarget).normalized;
                            if (i == 1)
                                dir2 = (leftToeTarget - leftFootTarget).normalized;

                            Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                            Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                            if (angle > maxAngleSlope || leftFootAtDrop)
                            {
                                Debug.Log("slope too steep, placing beside other");
                                //place next to other foot, it looks like we found a drop too large
                                // target is set to stand beside other foot
                                leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2;
                                leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);
                                bool found = false;
                                leftToeTarget = SetTargetToe(out found, leftFootTarget, phase);

                                leftFootAtDrop = true;
                                return;
                            }
                            else
                            {

                                leftToeTarget = Vector3.Lerp(leftToeTarget, hits[0], targetLerpSpeed);
                                leftFootTarget = SetTargetHeel(leftToeTarget);

                                leftLiftTarget = Vector3.Lerp(hits[0], leftAnkleStart, 0.5f);// edgeNearOtherFoot + Vector3.up * footLength * 1.5f;
                                leftLiftTarget.y = leftAnkleStart.y;
                                Debug.Log("no edges, using hit point");
                                //  Debug.Break();
                                return;
                            }
                        }
                    }
                }
            }
            else//edges.count>0
            {
                if (i == 0)
                {
                    //  Debug.Break();
                    //  Debug.Log("Broke here");

                    ///if (rightIdle)
                    //    leftIdle = false;

                    if (rightPhase == 0 && rightLerp == 0)
                        rightAnkleStart = rightFootPivot.transform.position;

                    //if we going up, use toe grab
                    if (edges[0].y > rightAnkleStart.y)
                    {
                        if (printLogs)
                            Debug.Log("edges count = " + edges.Count + ", going up");

                        //check if edge is behind other foot, if it is, just step over it, don't use toe grab
                        Vector3 dirToAnkle = (leftFootPivot.transform.position - edges[0]).normalized;
                        float dot = Vector3.Dot(dirToAnkle, root.transform.forward);
                        //Debug.Log("dot = " + dot);
                        if (dot > 0)
                        //if (edges[0].z < rightAnkleStart.z)//figure out with rot
                        {
                            if (printLogs)
                                Debug.Log("still up 1 - right");
                            rightLiftTarget = edges[edges.Count - 1] + footLength * Vector3.up * 1.5f;
                            rightToeTarget = Vector3.Lerp(rightToeTarget, Vector3.Lerp(hits[0], edges[edges.Count - 1], 0.5f), targetLerpSpeed);//experimental
                            rightFootTarget = SetTargetHeel(rightToeTarget);


                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("still up 2 - right");

                            //make toe grab on to edge
                            rightFootTarget = Vector3.Lerp(rightFootTarget, edges[edges.Count - 1], targetLerpSpeed);
                            rightToeTarget = rightFootTarget + footLength * root.transform.forward;
                            //make toe the pvit
                            MakeToeParent(rightAnkle, rightFoot, rightFootPivot);
                            rightFootPivot = rightFoot;
                            rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);
                            // rightLiftTarget.y = rightFootTarget.y;


                            return;
                        }
                    }
                    else
                    //land with heel
                    {

                        rightToeTarget = edges[edges.Count - 1];
                        rightFootTarget = SetTargetHeel(rightToeTarget);

                        if (printLogs)
                            Debug.Log("edges count = " + edges.Count + ", going down, land with heel");

                        Vector3 dir1 = root.transform.forward;
                        Vector3 dir2 = rightToeTarget - rightFootTarget;

                        float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                        Debug.Log(angle);

                        if (angle > maxAngleSlope)
                        {
                            if (printLogs)
                                Debug.Log("down 1 - ang;e > slope");

                            Debug.Break();

                            Debug.Log("EMPTY");


                            return;
                        }
                        else //angle is < slope
                        {
                            rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                            if (leftFootAtDrop)
                                rightFootAtDrop = true;

                            Debug.Break();
                            return;
                        }

                    }
                }
                else if (i == 1)
                {
                    // Debug.Break();
                    // Debug.Log("Broke here");

                

                    if (leftPhase == 0 && leftLerp == 0)
                        leftAnkleStart = leftFootPivot.transform.position;

                    if (printLogs)
                        Debug.Log("edges count = " + edges.Count + ", going up");

                    if (edges[0].y > leftAnkleStart.y)
                    {
                        //check if edge is behind other foot, if it is, just step over it, don't use toe grab
                        //figure out if behind
                        //https://answers.unity.com/questions/202937/how-to-tell-if-the-player-is-behind-a-enemy.html
                        Vector3 dirToAnkle = (rightFootPivot.transform.position - edges[0]).normalized;
                        float dot = Vector3.Dot(dirToAnkle, root.transform.forward);
                        //Debug.Log("dot = " + dot);
                        if (dot > 0)
                        //if (edges[0].z < rightAnkleStart.z)//****figure out with rot
                        {
                            if (printLogs)
                                Debug.Log("still up 1 - left");
                            leftLiftTarget = edges[edges.Count - 1] + footLength * Vector3.up * 1.5f;
                            leftToeTarget = Vector3.Lerp(leftToeTarget, Vector3.Lerp(hits[0], edges[edges.Count - 1], 0.5f), targetLerpSpeed);//experimental
                            leftFootTarget = SetTargetHeel(leftToeTarget);



                            return;
                        }
                        else
                        {
                            if (printLogs)
                                Debug.Log("still up 2 - left");
                            // Debug.Break();

                            //make toe grab on to edge
                            leftFootTarget = Vector3.Lerp(leftFootTarget, edges[0], targetLerpSpeed);
                            leftToeTarget = leftFootTarget + footLength * root.transform.forward;
                            //make toe the pvit
                            // MakeToeParent(leftAnkle, leftFoot, leftFootPivot);
                            //  leftFootPivot = leftFoot;
                            leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);
                            // leftLiftTarget.y = leftFootTarget.y;

                            //if other foot is at a drop, make this foot at a drop too
                            leftFootAtDrop = true;

                            return;
                        }
                    }
                    else//downwards
                    //land with heel
                    {
                        leftToeTarget = edges[edges.Count - 1];
                        leftFootTarget = SetTargetHeel(leftToeTarget);

                        if (printLogs)
                            Debug.Log("edges count = " + edges.Count + ", going down, land with heel");

                        Vector3 dir1 = root.transform.forward;
                        Vector3 dir2 = leftToeTarget - leftFootTarget;

                        float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                        Debug.Log(angle);
                        
                        if (angle > maxAngleSlope)
                        {
                            if (printLogs)
                                Debug.Log("down 1 - ang;e > slope");

                            Debug.Break();
                            Debug.Log("EMPTY");
                            

                            return;
                        }
                        else //angle is < slope
                        {                            
                            leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);

                            if (rightFootAtDrop)
                                leftFootAtDrop = true;

                            Debug.Break();
                            return;
                        }
                    }
                }
            }
        }
        else if (!edgeFound)
        {
            if (i == 0)
            {
                //set to then check against
               // rightToeTarget = hits[0];
                rightFootTarget = hits[0]   ;

                bool found = false;
                rightToeTarget = SetTargetToe(out found, rightFootTarget, i);
                if(found == false)
                {
                   
                    Debug.Log("moving toe back from edge - right");
                    //we are at an edge
                    rightFootAtDrop = true;

                    //move foot back slightly - make toe go to hit instead of ankle
                    rightToeTarget = hits[0];
                    rightFootTarget = SetTargetHeel(rightToeTarget);

                    Debug.DrawLine(rightToeTarget, rightToeTarget + Vector3.up, Color.magenta);
                }
                else
                {
                    Debug.Log("No edge - safe to move ");

                    //check if hill is too steep
                    Vector3 dir1 = root.transform.forward;
                    Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                    if (i == 1)
                        dir2 = (leftToeTarget - leftFootTarget).normalized;

                    Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                    Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                    float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);

                    if(angle > maxAngleSlope)
                    {
                        rightFootAtDrop = true;
                        //move foot back slightly - make toe go to hit instead of ankle
                        rightToeTarget = hits[0];
                        rightFootTarget = SetTargetHeel(rightToeTarget);

                        
                    }
                    else
                        rightFootAtDrop = false;
                }


                rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                return;
            }
            if (i == 1)
            {
                //set to then check against
                
                leftFootTarget = hits[0];//changed from toe target


                bool found = false;
                leftToeTarget = SetTargetToe(out found, leftFootTarget, i);
                if (!found)
                {
                    
                    Debug.Log("moving toe back from edge - left");
                    leftFootAtDrop = true;

                    leftToeTarget = hits[0];
                    leftFootTarget = SetTargetHeel(leftToeTarget);

                    Debug.DrawLine(leftToeTarget, leftToeTarget + Vector3.up, Color.magenta);
                    
                }
                else if (found)
                {

                    
                    Debug.Log("No edge - safe to move - left");

                    //check if hill is too steep
                    Vector3 dir1 = root.transform.forward;
                    Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                    if (i == 1)
                        dir2 = (leftToeTarget - leftFootTarget).normalized;

                    Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                    Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                    float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);

                    if (angle > maxAngleSlope)
                    {
                      
                        leftFootAtDrop = true;
                        //move foot back slightly - make toe go to hit instead of ankle
                        leftToeTarget = hits[0];                        
                        leftFootTarget = SetTargetHeel(leftToeTarget);


                    }
                    else
                        leftFootAtDrop = false;
                }
                leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);

                return;
            }

            //is last target the same? at an edge we can't drop down
            if (i == 0)
            {
                //check only at start of stride
                if (rightLerp == 0 && rightPhase == 0)
                {
                    //put foot beside other?
                    if (Vector3.Distance(hits[0], rightFoot.transform.position) < footLength)
                    {
                        Debug.Log("No target close to current target - right");


                        //target is set to stand beside other foot
                        rightFootTarget = leftFootTarget + root.transform.right * gaitWidth * 2;
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
                        bool found = false;
                        rightToeTarget = SetTargetToe(out found, rightFootTarget, phase);

                        //rightIdle = true;
                        rightFootAtDrop = true;
                        return;
                    }
                }
            }
            else if (i == 1)
            {
                if (leftLerp == 0 && leftPhase == 0)
                {
                    if (Vector3.Distance(hits[0], leftFoot.transform.position) < footLength)
                    {
                        Debug.Log("No target close to current target - left");
                        leftFootTarget = rightFootTarget - root.transform.right * gaitWidth * 2;
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);
                        bool found = false;
                        leftToeTarget = SetTargetToe(out found, leftFootTarget, phase);

                        //leftIdle = true;
                        leftFootAtDrop = true;
                        //Debug.Break();
                        //make toe the pvit


                        return;
                    }
                }
            }

           

            if (i == 0)
            {
                //check if hill is too steep
                Vector3 dir1 = root.transform.forward;
                Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                if (i == 1)
                    dir2 = (leftToeTarget - leftFootTarget).normalized;

                Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                Debug.Log(angle + " - angle on normal step");
                //if (rightFootTarget.y - rightToeTarget.y > footLength)
                if (angle > maxAngleSlope)
                {
                    //put foot beside other if other foot is already at an edge
                    if (leftFootAtDrop)
                    {
                        //target is set to stand beside other foot
                        rightFootTarget = leftAnkle.transform.position + root.transform.right * gaitWidth * 2;
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
                        bool found = false;
                        rightToeTarget = SetTargetToe(out found, rightFootTarget, phase);
                        Debug.Log("right too steep");

                        rightFootAtDrop = true;
                        leftFootAtDrop = true;
                        //  Debug.Break();
                    }
                    else if (!leftFootAtDrop)
                    {
                        Debug.Log("moving to edge and switching other foot drop status");
                        //move to the edge
                        rightToeTarget = hits[0];
                        rightFootTarget = SetTargetHeel(rightToeTarget);
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, phase);
                        // Debug.Break();

                        rightFootAtDrop = true;
                    }



                    return;
                }
                else
                {
                    Debug.Log("Normal Right Move");
                    
                    return;
                }

            }

            if (i == 1)
            {
                //check if hill is too steep
                Vector3 dir1 = root.transform.forward;
                Vector3 dir2 = (rightToeTarget - rightFootTarget).normalized;
                if (i == 1)
                    dir2 = (leftToeTarget - leftFootTarget).normalized;

                Debug.DrawLine(Vector3.zero, dir1, Color.magenta);
                Debug.DrawLine(Vector3.zero, dir2, Color.magenta);

                float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
                Debug.Log(angle + " - angle on normal step");
                //if (leftFootTarget.y - leftToeTarget.y > footLength)
                if (angle > maxAngleSlope)
                {
                    if (rightFootAtDrop)
                    {
                        leftFootTarget = rightAnkle.transform.position - root.transform.right * gaitWidth * 2;
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);
                        bool found = false;
                        leftToeTarget = SetTargetToe(out found, leftFootTarget, phase);
                        Debug.Log("left too steep");
                        //Debug.Break();
                        leftFootAtDrop = true;
                        rightFootAtDrop = true;
                        // Debug.Break();
                    }
                    else if (!rightFootAtDrop)
                    {
                        //  Debug.Break();
                        //move to edge
                        Debug.Log("moving to edge and switching other foot drop status");
                        leftToeTarget = hits[0];
                        leftFootTarget = SetTargetHeel(leftToeTarget);
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, phase);

                        leftFootAtDrop = true;
                    }


                    return;

                }
                else
                {
                    Debug.Log("Normal Left Move");
                    
                    return;
                }


            }

            Debug.Log("END?");
        }
    }

    void CalculateSteps4(List<Vector3> hits, int i, int phase)
    {
      //  bool edgeFound = false;
        List<Vector3> edges = new List<Vector3>();
        List<Vector3> pointBeforeEdges = new List<Vector3>();

        //Vector3 edgeNearOtherFoot = Vector3.zero;
        //work out if there any edges we need to consider
        bool wallFound = false;
        Vector3 wallPoint = Vector3.zero;
        for (int j = 0; j < hits.Count - 2; j++)
        {
            Vector3 dir1 = hits[j] - hits[j + 1];
            Vector3 dir2 = hits[j + 2] - hits[j + 1];

            float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);
            // Debug.Log(angle);
            //180 is flat

            //if we find an edge first
            if (angle > 0 && angle < 180 - maxAngle)
            {
               // edgeFound = true;

                RaycastHit hit;
                //find the actual edge point within another raycast
                float sphereSize = 0.01f; //if we get more misses, make larger?
                if (Physics.SphereCast(hits[j + 1] + Vector3.up * footLength, sphereSize, Vector3.down, out hit, footLength * 2))
                {
                    //   if (i == 0)
                    //       rightFootTarget = Vector3.Lerp(rightFootTarget, hit.point, targetLerpSpeed);
                    //   else
                    //       leftFootTarget = Vector3.Lerp(leftFootTarget, hit.point, targetLerpSpeed);
                }
                else
                {
                    
                        Debug.Log("miss");
                    Debug.Break();
                }
                //add edge if not too close to other standing foot
                Vector3 otherFootPos = leftAnkleStart;
                Vector3 thisFootPos = rightAnkleStart;
                if (i == 1)
                {
                    otherFootPos = rightAnkleStart;
                    thisFootPos = leftAnkleStart;
                }
                float distanceToOther = Vector3.Distance(hit.point, otherFootPos);
                float distanceToThis = Vector3.Distance(hit.point, thisFootPos);
                // Debug.Log("distance" + distanceToOther);

                if (distanceToOther > gaitWidth * 2 + bpe.footWidth && distanceToThis > gaitWidth * 2 + bpe.footWidth)//accuracy, ankle width?
                {
                    edges.Add(hit.point);
                    pointBeforeEdges.Add(hits[j]);
                    if (drawDebug)
                    {
                        Debug.DrawLine(hits[j], hits[j + 1], Color.magenta);
                        Debug.DrawLine(hits[j + 2], hits[j + 1], Color.magenta);
                    }
                }
                else
                {
                    if (drawDebug)
                    {
                        Debug.DrawLine(hits[j], hits[j + 1], Color.green);
                        Debug.DrawLine(hits[j + 2], hits[j + 1], Color.green);
                    }
                    //we have found the other foot, we don't need to ckeep looking behind, we want to go forward
                    //remmeber this point, we might need it**//not raycasting behind foot now - probs could remove the distance checks**OPTIMISATION

                    //set flag at start of step
                    if (i == 0 && rightLerp == 0f || i == 1 && leftLerp == 0f)
                    {
                        edgeNearOtherFoot = hit.point;
                        edgeNearOtherFootFound = true;
                    }

                    break;
                }
            }
            if (angle < 0 && angle > -180 + maxAngle && edges.Count == 0)
            {
                //if acute angle was found 
                //we have found a wall basically. save point ----not working?

                wallFound = true;
                wallPoint = hits[j + 1];
              //  if (printLogs)
                    Debug.Log("wall found");

            }
            else
            {
                //no edge/bump/foothold found
                //move to next hit
            }
        }

        //if we found no edges, we have nothing to step over
        if(edges.Count == 0)
        {
            //right
            if(i == 0)
            {
                //set target initially at first hit, as far away a place we have found from root
                rightToeTarget = hits[0];
                rightFootTarget = SetTargetHeel(hits[0]);

                //now check if this placement is on a slope which is too steep for our character to stand on
                //if angle is positive, we are going downhill, if angle is negative, uphill
                float angle = SlopeAngle(rightToeTarget, rightFootTarget);
                Debug.Log(angle);
                if (angle > maxAngleSlope || angle < -maxAngleSlope)
                {
                    Debug.Log("Angle > Slope");
                    //too steep, move alongside other foot
                    if (edgeNearOtherFootFound)
                    {
                        Debug.Log("Edges = 0, Edge near other foot found");
                        rightToeTarget = edgeNearOtherFoot;
                        rightFootTarget = SetTargetHeel(edgeNearOtherFoot);
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                        leftIdle = true;
                        rightFootAtDrop = true;

                        return;
                    }
                    else
                    {
                        if (wallFound)
                        {
                            Debug.Log("No edge found for other foot - using wall point");
                            //use "wallPoint" we found this by looking for acute angles when scanning for bumps
                            rightToeTarget = wallPoint;
                            rightFootTarget = SetTargetHeel(rightToeTarget);
                            rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);
                        }
                        else
                        {
                            Debug.Log("No edge found for other foot - NOT using wall point");
                            //use "wallPoint" we found this by looking for acute angles when scanning for bumps
                            rightToeTarget = hits[0];
                            rightFootTarget = SetTargetHeel(rightToeTarget);
                            rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                            if (!rightFootAtDrop)
                            {
                                leftIdle = false;
                                leftFootAtDrop = false;
                            }
                        }
                        

                        return;
                    }
                }
                //if it looks like an ok place to set foot down
                else 
                {
                    Debug.Log("Angle < Slope");
                    //keep original targets and create lift point
                    if (!edgeNearOtherFootFound)
                    {
                        Debug.Log("no edge near other foot");
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                        if (!firstStrideRight)
                        {
                            if (Vector3.Distance(leftFootTarget, rightFootTarget) < gaitWidth * 2 + bpe.footWidth)
                            {
                                //set flag to at drop, also, if other foot is already at drop, idle other leg, we don't need it to move if we both legs are at drop already
                                Debug.Log("close to other foot, not first stride");
                                leftIdle = true;
                                rightFootAtDrop = true;
                            }
                            else
                            {

                                if (Vector3.Distance(rightAnkleStart, rightFootTarget) < footLength)
                                {
                                    Debug.Log("At edge?");
                                    rightIdle = true;
                                    rightFootAtDrop = true;                                    
                                }
                                else
                                {
                                    Debug.Log("not edge");
                                    rightIdle = false;
                                    rightFootAtDrop = false;
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("normal right");                            
                            rightIdle = false;
                        }
                    }
                    //step over edge at other foot if we found one
                    else if (edgeNearOtherFootFound)
                    {
                        Debug.Log("edge near other foot");
                        rightLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;

                        rightIdle = false;
                    }

                    return;
                }
                
            }
            //left
            else if (i == 1)
            {
                //set target initially at first hit, as far away a place we have found from root
                leftToeTarget = hits[0];
                leftFootTarget = SetTargetHeel(hits[0]);

                //now check if this placement is on a slope which is too steep for our character to stand on
                float angle = SlopeAngle(leftToeTarget, leftFootTarget);
                if (angle > maxAngleSlope || angle < -maxAngleSlope)
                {
                    Debug.Log("Angle > Slope");
                    //too steep, move alongside other foot
                    if (edgeNearOtherFootFound)
                    {
                        Debug.Log("Edges = 0,Edge near other foot found");
                        leftToeTarget = edgeNearOtherFoot;
                        leftFootTarget = SetTargetHeel(edgeNearOtherFoot);
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);

                        rightIdle = true;
                        leftFootAtDrop = true;

                        return;
                    }
                    else
                    {
                        
                        Debug.Log("No edge found for other foot");
                        if (wallFound)
                        {
                            //use "wallPoint" we found this by looking for acute angles when scanning for bumps
                            leftToeTarget = wallPoint;
                            leftFootTarget = SetTargetHeel(leftToeTarget);
                            leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);
                           
                        }
                        else
                        {
                            Debug.Log("NOT using wall point");
                            leftToeTarget = hits[0];
                            leftFootTarget = SetTargetHeel(leftToeTarget);
                            leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);
                            if (!leftFootAtDrop)
                            {
                                rightIdle = false;
                                rightFootAtDrop = false;
                            }
                        }

                        return;
                    }
                }
                //if it looks like an ok place to set foot down
                else 
                {
                    Debug.Log("Angle < Slope");
                    Debug.Log(angle);
                    
                    //keep original targets and create lift point
                    if (!edgeNearOtherFootFound)
                    {
                        Debug.Log("no edge near other foot");
                        //
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);

                        //check if we are at a drop, if foot is being placed next to other foot we are
                        if (!firstStrideLeft)
                        {
                            
                            if (Vector3.Distance(leftFootTarget, rightFootTarget) < gaitWidth * 2 + bpe.footWidth)
                            {
                                Debug.Log("Close to other foot - not first stride");
                                //set flag true to at drop, also , idle other leg, we don't need it to move if we both legs are at drop already
                                rightIdle = true;
                                leftFootAtDrop = true;
                            }
                            else
                            {
                                if (Vector3.Distance(leftAnkleStart, leftFootTarget) < footLength)
                                {
                                    Debug.Log("At Edge?");
                                    leftIdle = true;
                                    leftFootAtDrop = true;
                                }
                                else
                                {
                                    Debug.Log("Not close to other foot - not first stride");
                                    leftIdle = false;
                                    leftFootAtDrop = false;
                                }
                            }
                        }
                        else
                        {
                            leftIdle = false;
                            Debug.Log("normal left");
                        }
                    }
                    //step over edge at other foot if we found one
                    else if(edgeNearOtherFootFound)
                    {
                        Debug.Log("edge near other foot");
                        leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;
                    }

                    return;
                }
            }
        }
        //we have to consider an edge to traverse
        else if (edges.Count > 0)
        {
            Debug.Log("edges > 0");
            if (i == 0)
            {
                //if other foot is not near an edge, always put foot at nearest edge
                if (!edgeNearOtherFootFound)
                {
                    //if going up
                    if (hits[0].y > rightAnkleStart.y)
                    {
                        rightFootTarget = edges[edges.Count - 1];
                        rightToeTarget = rightFootTarget + root.transform.forward * footLength;
                        rightLiftTarget = Vector3.Lerp(rightAnkleStart, rightFootTarget, 0.5f);
                        rightLiftTarget.y = rightToeTarget.y;

                        Debug.Log("Stepping on edge");
                        
                        //make toe the pivot only at start of stride
                        if (phase == 0 && rightLerp == 0)
                        {
                            MakeToeParent(rightAnkle, rightFoot, rightFootPivot);
                            rightFootPivot = rightFoot;
                            rightAnkleStart = rightFoot.transform.position;
                        }
                        
                    }
                    else
                    {
                        Debug.Log("Going down");
                        rightToeTarget = edges[edges.Count - 1];
                        rightFootTarget = SetTargetHeel(rightToeTarget);
                        rightLiftTarget = StandardWalkLiftPoint(rightAnkleStart, rightFootTarget, i, rightPhase);

                        
                    }

                    rightIdle = false;
                    return;
                }
                else if(edgeNearOtherFootFound)
                {

                    Debug.Log("edge near other foot");
                    //if going up
                    if (hits[0].y > rightAnkleStart.y)
                    {

                        //we need to find out if we can step over this edge
                        //set target initially at first hit, as far away a place we have found from root
                        rightFootTarget = edges[edges.Count - 1];
                        rightToeTarget = rightFootTarget + root.transform.forward * footLength;
                        rightLiftTarget = edgeNearOtherFoot + footLength * Vector3.up;

                        //make toe the pivot only at start of stride
                        if (phase == 0 && rightLerp == 0)
                        {
                            MakeToeParent(rightAnkle, rightFoot, rightFootPivot);
                            rightFootPivot = rightFoot;

                            rightAnkleStart = rightFoot.transform.position;
                        }
                    }
                    //if going down
                    else
                    {
                        Debug.Log("found steps going down");
                        if (rightFootAtDrop)
                        {
                            Debug.Log("not using steps");
                            
                        }
                        else
                        {
                            
                            rightToeTarget = edges[edges.Count - 1];
                            rightFootTarget = SetTargetHeel(rightToeTarget);
                            rightLiftTarget = edgeNearOtherFoot + Vector3.up*footLength;
                        }
                    }
                    rightIdle = false;
                    return;
                }                
            }
            else if (i == 1)
            {
                //if other foot is not near an edge, always put foot at nearest edge
                if (!edgeNearOtherFootFound)
                {
                    //if goind down, place toe at edge
                    if (hits[0].y > leftAnkleStart.y)
                    {
                        Debug.Log("Stepping on edge");
                        leftFootTarget = edges[edges.Count - 1];
                        leftToeTarget = leftFootTarget + root.transform.forward * footLength;
                        leftLiftTarget = Vector3.Lerp(leftAnkleStart, leftFootTarget, 0.5f);
                        leftLiftTarget.y = leftToeTarget.y;

                        //make toe the pivot only at start of stride
                        if (phase == 0 && leftLerp == 0)
                        {
                            MakeToeParent(leftAnkle, leftFoot, leftFootPivot);
                            leftFootPivot = leftFoot;
                            leftAnkleStart = leftFoot.transform.position;
                        }
                    }
                    else
                    {
                        Debug.Log("Going down");
                        leftToeTarget = edges[edges.Count - 1];
                        leftFootTarget = SetTargetHeel(leftToeTarget);
                        leftLiftTarget = StandardWalkLiftPoint(leftAnkleStart, leftFootTarget, i, leftPhase);
                    }

                    
                    leftIdle = false;
                    return;
                    
                }
                else if (edgeNearOtherFootFound)
                {
                    Debug.Log("edge near other foot");
                    //if going up
                    if (hits[0].y > leftAnkleStart.y)
                    {
                        Debug.Log("Going up");
                        //we need to find out if we can step over this edge
                        //set target initially at first hit, as far away a place we have found from root
                        leftFootTarget = edges[edges.Count - 1];
                        leftToeTarget = leftFootTarget + root.transform.forward * footLength;
                        leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;

                        //make toe the pivot only at start of stride
                        if (phase == 0 && leftLerp == 0)
                        {
                            MakeToeParent(leftAnkle, leftFoot, leftFootPivot);
                            leftFootPivot = leftFoot;

                            leftAnkleStart = leftFoot.transform.position;
                        }
                    }
                    //if going down
                    else
                    {
                        Debug.Log("found steps going down");
                        if (leftFootAtDrop)
                        {
                            Debug.Log("not using steps");
                            
                        }
                        else
                        {
                            leftToeTarget = edges[edges.Count - 1];
                            leftFootTarget = SetTargetHeel(leftToeTarget);
                            leftLiftTarget = edgeNearOtherFoot + Vector3.up * footLength;

                           
                        }
                        
                    }
                    //if idle, unidle
                    leftIdle = false;
                    return;
                    
                }
            }
        }
    }



    float SlopeAngle(Vector3 toeTarget,Vector3 footTarget)
    {
        //check if hill is too steep
        Vector3 dir1 = root.transform.forward;
        Vector3 dir2 = (toeTarget - footTarget).normalized;
        float angle = Vector3.SignedAngle(dir1, dir2, root.transform.right);

        return angle;
    }

    bool CheckForEdge(Vector3 toeTarget)
    {
        //scan ahead of toe placement to check for edge or slope
        bool edge = false;
        float legLength = (footHeight + shinLength + thighLength);
        Vector3 rayStart = toeTarget + root.transform.forward * footLength + Vector3.up * (legLength);

        RaycastHit hit;
        if(!Physics.Raycast(rayStart,Vector3.down,out hit,legLength*2))
        {
            //we are at a sudden drop
            return edge = false;
        }



        return edge;
    }



}

