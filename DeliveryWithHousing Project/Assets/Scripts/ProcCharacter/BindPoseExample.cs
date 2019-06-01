using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// this example creates a quad mesh from scratch, creates bones
// and assigns them, and animates the bones motion to make the
// quad animate based on a simple animation curve.
public class BindPoseExample : MonoBehaviour
{
    public float footHeight = 0.1f;
    public float footLength = 0.2f;
    public float footWidth = 0.1f;

    //public float ankleLength = 0.1f;
    public float kneeWidth = 0.1f;
    public float ankleWidthTrouser = 0.5f;
    public float shinLength = 0.4f;

    public float thighLength = 0.8f;
    public float thighWidth = 0.2f;
    public float curveAmount;
    public float waistWidth = 0.25f;
    public float waistWidth2 = 0.1f;
    public float waistWidth3 = 0.4f;
    public float waistWidth4 = 0.4f;
    public float waistWidth5= 0.4f;
    public float waistWidth6 = 0.4f;
    public float waistWidth7 = 0.4f;
    public float upperBodyGeneralWeight = 0.1f;
    public float booty = 0.2f;
    public float belly = .2f;
    public float boob = .2f;
    public float perk = 0f;
    public float shoulderBallWidth = 0f;//disabled
    public float elbowWidth = 0.1f;
    public float wristWidth = 0.1f;
    public float neckWidth = .1f;
    public float headSize = 2f;
    public float faceYMultiplier;// = Random.Range(1f, 2f);
    public float skirtShorten = 0.5f;
    public bool skirt = true;
    public bool longSkirt = true;

    // public float balls = .1f;
    public float gaitWidth = 0.25f;

    public float spineLength = 0.5f;
    public float spineCurve = 0.01f;

    public float shoulderWidth = 0.4f;

    public float forearmLength = 0.5f;
    public float upperArmLength = .6f;

    public float twistAmount = 10f;

    public float topHalfTwistRatio = 1f;

    Material[] materials = new Material[6];

    public GameObject rightFoot;
    public GameObject leftFoot;

    public GameObject rightAnkle;
    public GameObject leftAnkle;

    public GameObject leftKnee;
    public GameObject rightKnee;

    public GameObject leftPelvis;
    public GameObject rightPelvis;

    public List<GameObject> spine = new List<GameObject>();

    public GameObject leftShoulder;
    public GameObject rightShoulder;

    public GameObject leftElbow;
    public GameObject leftWrist;

    public GameObject rightElbow;
    public GameObject rightWrist;

    public GameObject gyro;

    public GameObject root;

    public GameObject rightAnkleWeight;
    public GameObject leftAnkleWeight;

    public bool skipDebugBones = true;

    public Vector3 spawnPoint;
    void Start()
    {        
        //asign character values
        SetValues();

        materials[0] = Resources.Load("Brown") as Material;
        materials[1] = Resources.Load("Blue") as Material;
        materials[2] = Resources.Load("Pink") as Material;
        materials[3] = Resources.Load("Skin1") as Material;
        materials[4] = Resources.Load("Yellow") as Material;
        materials[5] = Resources.Load("Blue") as Material;


        Transform[] bones = MakeBones().ToArray();
        
        Mesh mesh;        
      
        SkinnedMeshRenderer rend = gameObject.AddComponent<SkinnedMeshRenderer>();
        mesh = ProcClothes.Mesh(this);
        BoneWeight[] weights = SetWeights(mesh);
        mesh.bindposes = BindPoses(bones);
        mesh.boneWeights = weights;
        rend.bones = bones;
        rend.sharedMesh = mesh;
        rend.sharedMaterials = materials;
        rend.rootBone = root.transform;
        
        bool meshCubes = false;
        if (meshCubes)
        {
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = mesh.vertices[i];
                c.transform.localScale *= .05f;
                c.name = i.ToString();
            }
        }

        gameObject.transform.position = spawnPoint;
        
        GetComponent<ProceduralAnimator>().enabled = true;
        GetComponent<CharacterControllerProc>().enabled = true;

    }

    IEnumerator WaitAndPlace()
    {
        yield return new WaitForEndOfFrame();
        
        //root.transform.position = Vector3.right * 5;
        yield break;
    }
    void SetValuesForTest()
    {
        curveAmount = 1f;// Random.Range(1f, 10f); //male to female slider kinda
        curveAmount *= curveAmount;
        curveAmount /= 100;

        //curveAmount = 0.1f;
        //try and create a random character but in proportion with itself
        waistWidth = 1f;// Random.Range(1f, 2f); //can controla ge with this
        //waistWidth = 2f;


        //hip bone width same as waist width
        waistWidth2 = waistWidth;
        //start to curve body if curve modifier high
        waistWidth3 = waistWidth2 - (curveAmount * 0.12f);
        //slimmest part of body (waist)
        waistWidth4 = waistWidth3 - (curveAmount * 0.12f);
        //start of ribs
        waistWidth5 = waistWidth4 - (curveAmount * 0.1f);
        //chest
        waistWidth6 = waistWidth4 - (curveAmount * 0.1f);//perhaps this is too slim?
        //armpit
        waistWidth7 = shoulderWidth;

        upperBodyGeneralWeight = waistWidth / Random.Range(1.5f, 1.5f);
        shoulderWidth = waistWidth - curveAmount * 0.33f;//can make shoulder width as small as 2/3 of waistwidth
        shoulderBallWidth = upperBodyGeneralWeight * .2f;
        neckWidth = upperBodyGeneralWeight * 0.1f;

        headSize = waistWidth * Random.Range(.5f, .5f);//not sure.. if this is randomised, characters can start to look as if from a different design

        faceYMultiplier = Random.Range(1f, 1f);

        booty = curveAmount * (Random.Range(0f, waistWidth * .5f));
        belly = curveAmount * (Random.Range(0f, booty));
        belly = (upperBodyGeneralWeight * .33f * (1f - curveAmount)) * Random.Range(0.1f, 1f); //this generally amkes men fat on the belly, and women fat on the bum
        boob = (waistWidth / 3) * curveAmount;

        //perk cant be bigger than boob, it draws the boob back in
        perk = Random.Range(0f, boob / 2);//maybe this couldbe affected by age


        footLength = waistWidth / 3;//influence by height?
        footWidth = waistWidth / 4;
        footHeight = footWidth / 2;
        shinLength = waistWidth * Random.Range(.5f, .5f);
        thighLength = shinLength + footHeight;
        kneeWidth = footWidth * Random.Range(.7f, 1f);
        elbowWidth = footWidth;
        //spine twice as long as wait width with randomness
        spineLength = waistWidth * Random.Range(1.5f, 1.8f);        //1.5

        upperArmLength = spineLength / 2;
        forearmLength = spineLength / 3;
        elbowWidth = upperBodyGeneralWeight / 4;
        wristWidth = elbowWidth / 2;

        twistAmount = Random.Range(3f, 3f);
        topHalfTwistRatio = Random.Range(1f, 1f);

        //flares?
        ankleWidthTrouser = footWidth;

        gaitWidth = (waistWidth * -curveAmount);//, waistWidth * .75f);// *curveAmount;
        gaitWidth = Mathf.Clamp(gaitWidth, 0.2f, waistWidth);
        gaitWidth = waistWidth * .2f;

        skirt = false;// (Random.value < 0.5f * curveAmount);
        longSkirt = (Random.value < 0.5f);
        skirtShorten = Random.Range(0f, .8f);
    }

    void SetValues()
    {
        curveAmount = Random.Range(1f, 10f); //male to female slider kinda
        curveAmount *= curveAmount;
        curveAmount /= 100;
        
        //curveAmount = 0.1f;
        //try and create a random character but in proportion with itself
        waistWidth = Random.Range(1f, 2f); //can controla ge with this
        //waistWidth = 2f;


        //hip bone width same as waist width
        waistWidth2 = waistWidth;
        //start to curve body if curve modifier high
        waistWidth3 = waistWidth2 - (curveAmount*0.12f);
        //slimmest part of body (waist)
        waistWidth4 = waistWidth3 - (curveAmount * 0.12f);
        //start of ribs
        waistWidth5 = waistWidth4 - (curveAmount * 0.1f);
        //chest
        waistWidth6 = waistWidth4 - (curveAmount * 0.1f);//perhaps this is too slim?
        //armpit
        waistWidth7 = shoulderWidth;

        upperBodyGeneralWeight = waistWidth/Random.Range(1.5f,1.5f);
        shoulderWidth = waistWidth - curveAmount * 0.33f ;//can make shoulder width as small as 2/3 of waistwidth
        shoulderBallWidth = upperBodyGeneralWeight*.2f;
        neckWidth = upperBodyGeneralWeight * 0.1f;

        headSize = waistWidth*Random.Range(.5f, .5f);//not sure.. if this is randomised, characters can start to look as if from a different design

        faceYMultiplier = Random.Range(1f, 1.5f);

        booty = curveAmount * (Random.Range(0f, waistWidth*.5f));
        belly = curveAmount * (Random.Range(0f, booty));
        belly = (upperBodyGeneralWeight*.33f *(1f-curveAmount))*Random.Range(0.1f,1f); //this generally amkes men fat on the belly, and women fat on the bum
        boob = (waistWidth / 3) * curveAmount;
       
        //perk cant be bigger than boob, it draws the boob back in
        perk = Random.Range( 0f, boob/2);//maybe this couldbe affected by age

        
        footLength = waistWidth /3;//influence by height?
        footWidth = waistWidth / 4;
        footHeight = footWidth/2;
        shinLength = waistWidth * Random.Range(.5f,.5f);
        thighLength = shinLength + footHeight;
        kneeWidth = footWidth*Random.Range(.7f,1f);
        elbowWidth = footWidth;
        //spine twice as long as wait width with randomness
        spineLength = waistWidth*Random.Range(1.5f, 1.8f);        //1.5

        upperArmLength = spineLength/2;
        forearmLength = spineLength/3;
        elbowWidth = upperBodyGeneralWeight / 4;
        wristWidth = elbowWidth / 2;

        twistAmount = Random.Range(1f, 6f);
        topHalfTwistRatio = Random.Range(0.5f, 1f);

        //flares?
        ankleWidthTrouser = footWidth;

        gaitWidth = (waistWidth * -curveAmount);//, waistWidth * .75f);// *curveAmount;
        gaitWidth = Mathf.Clamp(gaitWidth, 0.2f, waistWidth);
        gaitWidth = waistWidth *.2f;

        skirt = (Random.value < 0.5f *curveAmount);
        longSkirt =  (Random.value < 0.5f);
        skirtShorten = Random.Range(0f, .8f);
    }

    private void Update()
    {
      //  rend.sharedMesh = MakeMesh();
      
    }

    BoneWeight[] SetWeights(Mesh mesh)
    {
        BoneWeight[] weights =new BoneWeight[mesh.vertexCount];//new BoneWeight[38];// 

        //right foot
        //for (int i = 0; i < 2; i++)
        {
            //all on ankle - keeping like this in case I add toe
            weights[0].boneIndex0 = 1;
            weights[0].weight0 = 1;
            weights[1].boneIndex0 = 1;
            weights[1].weight0 = 1;

            weights[4].boneIndex0 = 1;
            weights[4].weight0 = 1;
            weights[5].boneIndex0 = 1;
            weights[5].weight0 = 1;

        }
        {
            weights[2].boneIndex0 = 1;
            weights[2].weight0 = 1;
            weights[3].boneIndex0 = 1;
            weights[3].weight0 = 1;

            weights[6].boneIndex0 = 1;
            weights[6].weight0 = 1;
            weights[7].boneIndex0 = 1;
            weights[7].weight0 = 1;
        }
        //right trouser bottom
        for (int i = 8; i < 12; i++)
        {
            weights[i].boneIndex0 = 1;
            weights[i].weight0 = 1;
        }
        //right shin

       
        weights[12].boneIndex0 = 2;
        weights[12].weight0 = 1;

        weights[13].boneIndex0 = 2;
        weights[13].weight0 = 1;

        weights[14].boneIndex0 = 2;
        weights[14].weight0 = 1;

        weights[15].boneIndex0 = 2;
        weights[15].weight0 = 1;
        //inside thigh? stick to spine, outside? stick to pelvis bone

        //lower hip
        weights[16].boneIndex0 = 22;// 8;//lower hip front //was 3 hip bone, all on spine0 atm
        weights[16].weight0 = 1f;
        
        //groin
        weights[17].boneIndex0 = 8;//front gront lower on spine 0
        weights[17].weight0 = 1;

        //rear groin
        weights[18].boneIndex0 = 8;//rear lower groin sticks to spin 0
        weights[18].weight0 = 1;

        weights[19].boneIndex0 = 22;//8;//lower hip rear on pelvis        
        weights[19].weight0 = 1f;
        
        //hip
        weights[20].boneIndex0 = 8;//front hip high on spin 0
        weights[20].weight0 = 1;

        weights[21].boneIndex0 = 8;//
        weights[21].weight0 = 1;
        
        weights[22].boneIndex0 = 8;//
        weights[22].weight0 = 1;

        weights[23].boneIndex0 = 8;//groin
        weights[23].weight0 = 1;

        //spine 1
        for (int i = 24; i < 28; i++)
        {
            weights[i].boneIndex0 = 9;
            weights[i].weight0 = 1;
         
        }
        //spine 2
        for (int i = 28; i < 32; i++)
        {
            weights[i].boneIndex0 = 10;
            weights[i].weight0 = 1;
        }
        //spine3
        for (int i = 32; i < 36; i++)
        {
            weights[i].boneIndex0 = 11;
            weights[i].weight0 = 1;
        }
        //spine4
        for (int i = 36; i < 40; i++)
        {
            weights[i].boneIndex0 = 12;
            weights[i].weight0 = 1;
        }
        //spine5
        for (int i = 40; i < 44; i++)
        {
            weights[i].boneIndex0 = 13;
            weights[i].weight0 = 1;
        }
      
      
        //shoulder //just outside edge points
        for (int i = 44; i < 46; i++)
        {
            weights[i].boneIndex0 = 16;
            weights[i].weight0 = 1;
        }
        //elbow
        for (int i = 46; i < 50; i++)
        {
            weights[i].boneIndex0 = 17;
            weights[i].weight0 = 1;
        }
        //hand and wrist
        for (int i = 50; i < 58; i++)
        {
            weights[i].boneIndex0 = 18;
            weights[i].weight0 = 1;
        }

        //spine 6(got added later so numbers aren't in order
        for (int i = 58; i < 62; i++)
        {
            weights[i].boneIndex0 = 14;
            weights[i].weight0 = 1;
        }
        //spine 7 (got added later so numbers aren't in order
        for (int i = 62; i < 66; i++)
        {
            weights[i].boneIndex0 = 15;
            weights[i].weight0 = 1;
        }

        //left ankle and trouse bottom
        for (int i = 66; i < 78; i++)
        {
            weights[i].boneIndex0 = 5;
            weights[i].weight0 = 1;
        }

        //left knee
        for (int i = 78; i < 82; i++)
        {
            weights[i].boneIndex0 = 6;
            weights[i].weight0 = 1;
        }
        
        //left hip
        weights[78+4].boneIndex0 = 22;//8//putting it all on spine 0
        weights[78+4].weight0 = 1;
        weights[81+4].boneIndex0 = 22;//8
        weights[81+4].weight0 = 1;
        //left groin//
        weights[79+4].boneIndex0 = 8;
        weights[79+4].weight0 = 1;
        weights[80+4].boneIndex0 = 8;
        weights[80+4].weight0 = 1;

        

        //spine 1
        for (int i = 86; i < 90; i++)
        {
            weights[i].boneIndex0 = 8;
            weights[i].weight0 = 1;
        }
        //2
        for (int i = 90; i < 94; i++)
        {
            weights[i].boneIndex0 = 9;
            weights[i].weight0 = 1;
        }
        //3
        for (int i =94; i < 98; i++)
        {
            weights[i].boneIndex0 = 10;
            weights[i].weight0 = 1;
        }
        //4
        for (int i = 98; i < 102; i++)
        {
            weights[i].boneIndex0 = 11;
            weights[i].weight0 = 1;
        }
        //5
        for (int i = 102; i < 106; i++)
        {
            weights[i].boneIndex0 = 12;
            weights[i].weight0 = 1;
        }
        //6
        for (int i = 106; i < 110; i++)
        {
            weights[i].boneIndex0 = 13;
            weights[i].weight0 = 1;
        }

        //shoulder
        for (int i = 110; i < 112; i++)
        {
            weights[i].boneIndex0 = 19;
            weights[i].weight0 = 1;
        }
        //elbow
        for (int i = 112; i < 116; i++)
        {
            weights[i].boneIndex0 = 20;
            weights[i].weight0 = 1;
        }
        //wrist and hand
        for (int i = 116; i < 128; i++)
        {
            weights[i].boneIndex0 = 21;
            weights[i].weight0 = 1;
        }
        //neck
        for (int i = 124; i < 128; i++)
        {
            weights[i].boneIndex0 = 14;
            weights[i].weight0 = 1;
        }
        for (int i = 128; i < 132; i++)
        {
            weights[i].boneIndex0 = 15;
            weights[i].weight0 = 1;
        }

        //skirt is always made, set weights
        for (int i = 132; i < 136; i++)//pelvis
        {
            weights[i].boneIndex0 = 8;
            weights[i].weight0 = 1;
        }
        

        for (int i = 136; i < 140; i++)//pelvis
        {
            weights[i].boneIndex0 = 8;//8
            weights[i].weight0 = 1;
        }
        for (int i = 140; i < 144; i++)//knee
        {
            //blend weights between knee and pelvis depending on how shirt skirt is - if we dont do this skirt will go through leg
            if (!longSkirt)
            {
                weights[i].boneIndex0 = 2;
                weights[i].weight0 = 1f - skirtShorten;

                weights[i].boneIndex1 = 3;
                weights[i].weight1 = skirtShorten;
            }
            else if(longSkirt)
            {
                //just attach to knoee, no problem
                weights[i].boneIndex0 = 2;
                weights[i].weight0 = 1f;//
            }
        }

        for (int i = 144; i < 148; i++)
        {
            //bottom skirt - add weights even tho triangles mught not get used

            weights[i].boneIndex0 = 23;
            weights[i].weight0 = 1f - skirtShorten;

            weights[i].boneIndex1 = 2;
            weights[i].weight1 = skirtShorten;

        }

        for (int i = 148; i < 152; i++)//pelvis
        {
            weights[i].boneIndex0 = 8;
            weights[i].weight0 = 1;
        }

        for (int i = 152; i < 156; i++)
        {
            weights[i].boneIndex0 = 8;
            weights[i].weight0 = 1;
        }

        for (int i = 156; i < 160; i++)
        {
            if (!longSkirt)
            {
                //blend weights between knee and pelvis depending on how shirt skirt is - if we dont do this skirt will go through leg
                weights[i].boneIndex0 = 6;
                weights[i].weight0 = 1f - skirtShorten;

                weights[i].boneIndex1 = 7;
                weights[i].weight1 = skirtShorten;
            }
            else if(longSkirt)
            {
                weights[i].boneIndex0 = 6;
                weights[i].weight0 = 1f;
            }
        }

        for (int i = 160; i < 164; i++)
        {
            //bottom skirt - add weights even tho triangles mught not get used

            weights[i].boneIndex0 = 24;
            weights[i].weight0 = 1f - skirtShorten;

            weights[i].boneIndex1 = 6;
            weights[i].weight1 = skirtShorten;
        }


     //   Debug.Log("weights " + weights.Length);
        return weights;
    }

    List<Transform> MakeBones()
    {
        //right ankle
        Vector3 rightAnklePos = Vector3.right * (gaitWidth * 0.5f);// - Vector3.forward* ankleWidth;// + Vector3.forward * ankleWidth * .5f;
        rightAnkle = new GameObject();
        rightAnkle.transform.position = rightAnklePos;
        rightAnkle.name = "RightAnkle";
        MakeCubeBone(rightAnkle);


        Vector3 leftAnklePos = rightAnklePos - Vector3.right * ((gaitWidth)); // - Vector3.forward * ankleWidth;
        leftAnkle = new GameObject();       
        leftAnkle.transform.position = leftAnklePos;
        leftAnkle.name = "LeftAnkle";
        MakeCubeBone(leftAnkle);

        rightFoot = new GameObject();        
        rightFoot.transform.position = rightAnklePos + Vector3.forward * footLength;        
        rightFoot.name = "RightFoot";
        MakeCubeBone(rightFoot);


        leftFoot = new GameObject();        
        leftFoot.transform.position = leftAnklePos + Vector3.forward * footLength;        
        leftFoot.name = "LeftFoot";
        MakeCubeBone(leftFoot);
        
        //pelvisii
        leftPelvis = new GameObject();
        leftPelvis.name = "LeftPelvis";
        leftPelvis.transform.position = Vector3.up * (shinLength + thighLength + footHeight) + Vector3.left * waistWidth * 0.5f + Vector3.forward*footWidth*.0f;
        MakeCubeBone(leftPelvis);

        rightPelvis = new GameObject();
        rightPelvis.name = "RightPelvis";
        rightPelvis.transform.position = Vector3.up * (shinLength + thighLength + footHeight) + Vector3.right * waistWidth * 0.5f + Vector3.forward * footWidth*.0f;
        MakeCubeBone(rightPelvis);

        Vector3 dirToAnkle = (rightAnkle.transform.position - rightPelvis.transform.position).normalized;
        Vector3 rightKneePos = rightPelvis.transform.position + (dirToAnkle * thighLength);

        rightKnee = new GameObject();
        rightKnee.transform.position = rightKneePos;
        rightKnee.name = "RightKnee";
        MakeCubeBone(rightKnee);

        dirToAnkle = (leftAnkle.transform.position - leftPelvis.transform.position).normalized;
        Vector3 leftKneePos = leftPelvis.transform.position + (dirToAnkle * thighLength);

        leftKnee = new GameObject();
        leftKnee.transform.position = leftKneePos;
        leftKnee.name = "LeftKnee";
        MakeCubeBone(leftKnee);

        //
        //   Vector3 hipsPos = Vector3.up * (shinLength + thighLength + footHeight);
        //   GameObject hips = new GameObject();
        //   hips.transform.position = hipsPos;
        //   hips.name = "hips";
        //  hips.transform.localScale *= 0.1f;

        GameObject genericRigParent = new GameObject();
        genericRigParent.name = "Rig";
        genericRigParent.transform.parent = transform;
        GameObject rootCube = MakeCubeBone(genericRigParent);
        if(!skipDebugBones)
            rootCube.name = "RootCube";

        root = genericRigParent;
        root.transform.position = Vector3.Lerp(rightPelvis.transform.position, leftPelvis.transform.position, 0.5f);

        //used to figure out and counter balnace hips rotation
        gyro = new GameObject();
        gyro.name = "Gyro";
        gyro.transform.position = root.transform.position;
        gyro.transform.parent = root.transform.transform;

        //spine
        int spineParts = 8;
        for (int i = 0; i < spineParts; i++)
        {
            GameObject spine0 = new GameObject();
            string name = "Spine" + i.ToString();
            spine0.name = name;
            spine0.transform.parent = transform;
           // spine0.transform.localScale *= .1f;
            MakeCubeBone(spine0);
            //4 parts in spine, place parts equally
            if(i < 5)
                spine0.transform.position = root.transform.position + Vector3.up * (spineLength / spineParts)*(i+1) + Vector3.forward*spineCurve*i;
            if(i>=5)
                spine0.transform.position = root.transform.position + Vector3.up * (spineLength / spineParts) * (i + 1f) + Vector3.forward * spineCurve * i;

            spine.Add(spine0);

            if (i == spineParts-1)
            {
                GameObject head = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Cube);             
                head.name = "Head";


                //head.GetComponent<BoxCollider>().enabled = false;
                //4 parts in spine, place parts equally
                head.transform.parent = spine0.transform;
                head.transform.position = spine0.transform.position + Vector3.up * ((headSize*faceYMultiplier*0.9f));
                
                MeshRenderer mr= head.AddComponent<MeshRenderer>();
                mr.sharedMaterials = new Material[2] { materials[3],materials[0]}; 
               // mr.sharedMaterials[0] = materials[3];
                //mr.sharedMaterials[1] = materials[4];
               

                Mesh mesh = IcoSphere.Create(head,true);

                

                head.transform.GetComponent<MeshFilter>().mesh = HeadMesh1Rev(mesh,head);
                
                head.transform.localScale = new Vector3(headSize, headSize*faceYMultiplier, headSize);
                
            }
            
        }

        //shoulders
        leftShoulder = new GameObject();
        leftShoulder.name = "LeftShoulder";
        leftShoulder.transform.parent = transform;
        MakeCubeBone(leftShoulder);

        //4 parts in spine, place parts equally
        leftShoulder.transform.position = spine[spine.Count - 2].transform.position + Vector3.left * shoulderWidth*.5f;
        leftShoulder.transform.parent = spine[spine.Count - 2].transform;

        rightShoulder = new GameObject();
        rightShoulder.name = "RightShoulder";
        rightShoulder.transform.parent = transform;
         

        //4 parts in spine, place parts equally
        rightShoulder.transform.position = spine[spine.Count - 2].transform.position + Vector3.right * shoulderWidth*.5f;
        rightShoulder.transform.parent = spine[spine.Count - 2].transform;
        MakeCubeBone(rightShoulder);
        //arms
        leftElbow = new GameObject();
        leftElbow.transform.position = leftShoulder.transform.position - Vector3.up * upperArmLength;
        leftElbow.name = "LeftElbow";
        leftElbow.transform.parent = leftShoulder.transform;
        MakeCubeBone(leftElbow);


        rightElbow = new GameObject();
        rightElbow.transform.position = rightShoulder.transform.position - Vector3.up * upperArmLength;
        rightElbow.name = "RightElbow";
        rightElbow.transform.parent = rightShoulder.transform;
        MakeCubeBone(rightElbow);


        leftWrist = new GameObject();
        leftWrist.transform.position = leftElbow.transform.position - Vector3.up * forearmLength;
        leftWrist.name = "LeftWrist";
        leftWrist.transform.parent = leftElbow.transform;
        MakeCubeBone(leftWrist);

        rightWrist = new GameObject();
        rightWrist.transform.position = rightElbow.transform.position - Vector3.up * forearmLength;
        rightWrist.name = "RightWrist";
        rightWrist.transform.parent = rightElbow.transform;
        MakeCubeBone(rightWrist);

        List<Transform> bones = new List<Transform>();
        //bones.Add(genericRigParent.transform);
        //rightfoot/leg
        bones.Add(rightFoot.transform);//0
        bones.Add(rightAnkle.transform);//1
        bones.Add(rightKnee.transform);//2
        bones.Add(rightPelvis.transform);//3
        //left
        bones.Add(leftFoot.transform); //4
        bones.Add(leftAnkle.transform); //5
        bones.Add(leftKnee.transform);//6
        bones.Add(leftPelvis.transform);//7

        //spine
        for (int i = 0; i < spineParts; i++)
        {
            bones.Add(spine[i].transform);//8//9//10//11//12//13//14//15
        }
        bones.Add(rightShoulder.transform);//16
        bones.Add(rightElbow.transform);//17
        bones.Add(rightWrist.transform);//18

        bones.Add(leftShoulder.transform);//19
        bones.Add(leftElbow.transform);//20
        bones.Add(leftWrist.transform);//21

        bones.Add(gyro.transform);//22 (afterthought)

        //weights for skirt 
        
        rightAnkleWeight = new GameObject();
        rightAnkleWeight.transform.position = rightAnkle.transform.position;
        rightAnkleWeight.transform.name = "RightAnkleWeight";
        rightAnkleWeight.transform.parent = rightAnkle.transform;

        leftAnkleWeight = new GameObject();
        leftAnkleWeight.transform.position = leftAnkle.transform.position;
        leftAnkleWeight.transform.name = "LeftAnkleWeight";
        leftAnkleWeight.transform.parent = leftAnkle.transform;
        //23        
        bones.Add(rightAnkleWeight.transform);
        //24
        bones.Add(leftAnkleWeight.transform);

        rightAnkle.transform.parent = transform;
        rightFoot.transform.parent = rightAnkle.transform;


        leftAnkle.transform.parent = leftFoot.transform;
        leftFoot.transform.parent = transform;

        rightKnee.transform.parent = transform;
        leftKnee.transform.parent = transform;

        leftPelvis.transform.parent = transform;
        rightPelvis.transform.parent = transform;

        GetComponent<CharacterControllerProc>().root = root;

        return bones;
    }

    GameObject MakeCubeBone(GameObject bone)
    {
        if (skipDebugBones)
            return null;

        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.transform.position = bone.transform.position;
        c.transform.localScale *= 0.1f;
        c.transform.parent = bone.transform;
        c.GetComponent<BoxCollider>().enabled = false;

        return c;
    }

    Matrix4x4[] BindPoses(Transform[] bones)
    {
        Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];
        for (int i = 0; i < bindPoses.Length; i++)
        {
            bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }
        return bindPoses;
    }
   
    public Mesh HeadMesh1Rev(Mesh mesh, GameObject gameObject)
    {
        Vector3[] vertices = mesh.vertices;

        //nose is 88 bottom, 25 mid, 85 top
        float noseSizeZ = Random.Range(0.1f, 0.6f);
        float eyeSink = .2f;
        float brow = .1f;//needed, putting other mesh on top anyway?
        float chin = .3f;
        float lips = .1f;
        float cheek = .1f;
        float jaw = 0.1f;

        //nose
        vertices[25] += Vector3.forward * noseSizeZ + Vector3.down*noseSizeZ*0.5f;
        vertices[5] += Vector3.forward * noseSizeZ * .66f + Vector3.down * noseSizeZ * 0.5f; ;

        //chin
        vertices[34] += Vector3.forward * chin;
        vertices[35] += Vector3.forward * chin;

        //jaw 27 32 
        vertices[27] += Vector3.right* jaw;


        //hair
        //top forehead 14  15 23 12 21
        vertices[14] += Vector3.right * jaw;

        // nape 37 38 40 26

        //rear 18 10 20 30 31 
        //top 16 1 0
        bool debugCubes = false;
        if (debugCubes)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 p = (vertices[i]);

                //p.x /= gameObject.transform.localScale.x;
                //  p.y *= gameObject.transform.localScale.y;
                // p.z *= gameObject.transform.localScale.z;

                p += gameObject.transform.position;
                c.transform.parent = gameObject.transform;

                c.transform.position = p;
                c.transform.localScale *= 0.1f;
                c.transform.name = i.ToString();
            }
        }
        //15/14/16 is top tri vertices
        mesh.subMeshCount = 2;
        //create new triangle list without tese triangles
        List<int> tris1 = new List<int>();
        List<int> tris2 = new List<int>();
        int[] tempTrisOriginal = mesh.triangles;

        //male to female, 0 male, 4 female
        int[] hairList0 = new int[] { 16, 20, 10, 29, 6, 39, 8, 30 };//receeding 2 (bald patch)
        int[] hairList1 = new int[] { 17, 18, 28, 19, 31, 16, 20, 10, 29, 6, 39, 8, 30 };//receeding 1
        int[] hairList2 = new int[] { 17, 18, 28, 19, 31 };//skull cap
        int[] hairList3 = new int[] { 17, 18, 28, 19, 31,16,0,20,10,29,6,39,8,30,1,7 };//normal 
        int[] hairList4 = new int[] { 17, 18, 28, 19, 31, 16, 0, 20, 10, 29, 6, 39, 8, 30, 1,40,26,7 };//bangs//needs to be short?
        int[][] hairOptions = new int[][] { hairList0, hairList1, hairList2, hairList3,hairList4 };

        int hairChoice = (int)((curveAmount/2)*Random.Range(0f,5f) * 10);
        hairChoice = Mathf.Clamp(hairChoice, 0, 4);

        

        //Debug.Log(hairChoice + " hair choice");
        //int[] hairList = hairOptions[Random.Range(0, hairOptions.Length)];
        int[] hairList = hairOptions[hairChoice];
        //add beard?
        List<int> tempHair = new List<int>(hairList);
        if (Random.value < 0.5f * curveAmount)
        {
            tempHair.Add(34);
            tempHair.Add(35);
        }
        hairList = tempHair.ToArray();

        for (int i = 0; i < tempTrisOriginal.Length-2; i+=3)
        {
            bool addedToHair = false;
            for (int j = 0; j < hairList.Length; j++)
            {
                if (tempTrisOriginal[i] == hairList[j] || tempTrisOriginal[i + 1] == hairList[j] || tempTrisOriginal[i + 2] == hairList[j])
                {
                    tris2.Add(tempTrisOriginal[i]);
                     tris2.Add(tempTrisOriginal[i + 1]);
                     tris2.Add(tempTrisOriginal[i + 2]);

                    addedToHair = true;
                }
            }
            if (!addedToHair)
            {
                tris1.Add(tempTrisOriginal[i]);
                tris1.Add(tempTrisOriginal[i + 1]);
                tris1.Add(tempTrisOriginal[i + 2]);
            }
        }

        //move hair vertice
        float length = Random.Range(0f, 4f);
        int[] parting = new int[] { 14, 15, 16,23,12 };
        //choose random start and create random direction by using two points around the forehead
        Vector3 p1 = vertices[parting[Random.Range(0, parting.Length)]];
        Vector3 p2 = vertices[parting[Random.Range(0, parting.Length)]];
        Vector3 growFrom = Vector3.Lerp(p1, p2, Random.Range(0f,1f));//above forehead
        for (int i = 0; i < hairList.Length; i++)
        {
            //grab beard first, we can make this not as long as the rest of the hair
            float tempLength = length;
            if (hairList[i] == 34 || hairList[i] == 35)
            {
                //make x^2 so shorter hair is more common
                tempLength *= Random.Range(0f, 1f);
                tempLength *= tempLength;
                tempLength /= 10;
            }

            Vector3 dir = (vertices[hairList[i]] - growFrom).normalized;
            float distance = Vector3.Distance(vertices[hairList[i]], growFrom);
            dir = dir * (distance * tempLength);
            //flatten?
            dir.x *=  Random.Range(0f,1f);
            //dir.y *= 0.1f;
            dir.z *= Random.Range(0f, 1f);
            vertices[hairList[i]] += dir;
        }

        //eyes, we can use primitives
        //13 and 22
        float eyeSize = Random.Range( 0.4f,0.4f);
        float eyeMove = Random.Range(0.2f, -0.2f);
        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftEye.transform.position = vertices[13] + gameObject.transform.position - Vector3.forward*eyeSize*.0f + Vector3.right*eyeMove;
        leftEye.transform.parent = gameObject.transform;
        leftEye.name = "LeftEye";
        //leftEye.GetComponent<MeshRenderer>().sharedMaterial = materials[1];
        leftEye.transform.localScale *= eyeSize;
        

        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.transform.position = vertices[22] + gameObject.transform.position - Vector3.forward * eyeSize*.0f - Vector3.right * eyeMove;
        rightEye.transform.parent = gameObject.transform;
        rightEye.name = "RighEye";
        //rightEye.GetComponent<MeshRenderer>().sharedMaterial = materials[1];
        rightEye.transform.localScale *= eyeSize;

        GameObject irisR = Instantiate(rightEye, rightEye.transform.position,Quaternion.identity,rightEye.transform);
        float irisSize = Random.Range(eyeSize * 1f, eyeSize * 1.5f);
        irisR.transform.localScale = Vector3.one * irisSize;
        irisR.transform.position += Vector3.forward * (eyeSize*.4f );
        irisR.GetComponent<MeshRenderer>().sharedMaterial = materials[1];

        GameObject irisL = Instantiate(leftEye, leftEye.transform.position, Quaternion.identity, leftEye.transform);        
        irisL.transform.localScale = Vector3.one * irisSize;
        irisL.transform.position += Vector3.forward * (eyeSize * .4f);
        irisL.GetComponent<MeshRenderer>().sharedMaterial = materials[1];

        float eyeRaise = Random.Range(0.02f, 0.1f);
        GameObject rightEyeBrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightEyeBrow.name = "RightEyebrow";
        rightEyeBrow.GetComponent<MeshRenderer>().sharedMaterial = materials[0];
        rightEyeBrow.transform.position = (vertices[15] + vertices[23] + vertices[22]) / 3;
        rightEyeBrow.transform.position += gameObject.transform.position;
        Vector3 temp = rightEyeBrow.transform.position;
        temp.x = rightEye.transform.position.x;
        rightEyeBrow.transform.position = temp;        
        rightEyeBrow.transform.LookAt(vertices[5]);
        rightEyeBrow.transform.rotation *= Quaternion.Euler(90, 0, 0);
        rightEyeBrow.transform.localScale = new Vector3(eyeSize*1.5f, 0.05f, eyeSize*1.5f);
        rightEyeBrow.transform.position -= rightEyeBrow.transform.right * eyeSize*.5f;
        rightEyeBrow.transform.parent = gameObject.transform;


        GameObject leftEyeBrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftEyeBrow.name = "LeftEyebrow";
        leftEyeBrow.GetComponent<MeshRenderer>().sharedMaterial = materials[0];
        leftEyeBrow.transform.position = (vertices[15] + vertices[23] + vertices[22]) / 3;
        leftEyeBrow.transform.position += gameObject.transform.position;
        temp = leftEyeBrow.transform.position;
        temp.x = leftEye.transform.position.x;
        leftEyeBrow.transform.position = temp;
        leftEyeBrow.transform.LookAt(vertices[5]);
        leftEyeBrow.transform.rotation *= Quaternion.Euler(90, 0, 0);
        leftEyeBrow.transform.localScale = new Vector3(eyeSize * 1.5f, 0.05f, eyeSize * 1.5f);
        leftEyeBrow.transform.position += leftEyeBrow.transform.right * eyeSize * .5f;
        leftEyeBrow.transform.parent = gameObject.transform;


        // mesh.RecalculateNormals();//don#'t recalculate normals, it makes it look more flowy if we keep the lighting as a smooth head
        mesh.SetTriangles(tris1, 0);
        mesh.SetTriangles(tris2, 1);

        mesh.vertices = vertices;

        

        return mesh;
    }

    public static Mesh HeadMesh2Rev(Mesh mesh,GameObject gameObject)
    {
        Vector3[] vertices = mesh.vertices;

        //nose is 88 bottom, 25 mid, 85 top
        float noseSizeZ = .4f;
        float eyeSink = .2f;
        float brow = .1f;//needed, putting other mesh on top anyway?
        float chin = .3f;
        float lips = .1f;
        float cheek = .1f;

        vertices[85] += Vector3.forward * noseSizeZ * .33f;
        vertices[25] += Vector3.forward * noseSizeZ * .66f;
        vertices[88] += Vector3.forward * noseSizeZ;

        //eye Right // 22
        //eye Left // 13
        vertices[22] -= Vector3.forward * eyeSink;
        vertices[13] -= Vector3.forward * eyeSink;

        //eyebrow right 79 80 82
        vertices[79] += Vector3.forward * brow*.66f;
        vertices[80] += Vector3.forward * brow + Vector3.down*brow*.33f;//mid//change .66f for eyebrow expression max /5
        vertices[82] += Vector3.forward * brow;

        //eyebro left 46 49 50
        vertices[46] += Vector3.forward * brow;
        vertices[50] += Vector3.forward * brow * .66f;
        vertices[49] += Vector3.forward * brow + Vector3.down * brow * .33f;

        //chin 124 34 35
        vertices[124] += Vector3.forward * chin;//mid
        vertices[34] += Vector3.forward * chin*.66f;
        vertices[35] += Vector3.forward * chin*.66f;

        //lips 118 123
        vertices[118] += Vector3.forward * lips;
        vertices[123] += Vector3.forward * lips;
        vertices[119] += Vector3.forward * lips*.66f;
        vertices[151] += Vector3.forward * lips*.66f;

        //right cheek 33 115 148 116
        vertices[33]  += Vector3.forward * cheek + Vector3.right * cheek * .11f; ; //most central cheek point
        vertices[119] += Vector3.forward * cheek * .66f;//lip point//should we combine this with lip var?
        vertices[148] += Vector3.forward * cheek*.33f;//eye socket bottom
        //cheek point under right corner of eye
        vertices[115] += Vector3.forward * cheek * .66f + Vector3.right * cheek * .11f;
        //continuing down
        vertices[116] += Vector3.forward * cheek * .66f + Vector3.right * cheek * .33f;
        vertices[113] += Vector3.forward * cheek * .11f;// + Vector3.right * cheek * .33f;


        // vertices[117] += Vector3.forward * cheek*.2f + Vector3.right * cheek * .5f;
        // vertices[145] += Vector3.forward * cheek * .5f + Vector3.right * cheek * .5f;
        // vertices[113] += Vector3.forward * cheek * .5f + Vector3.right * cheek * .5f;
       // vertices[115] += Vector3.forward * cheek*.0f + Vector3.right * cheek * .5f;
       // vertices[32]  += Vector3.forward * cheek + Vector3.right * cheek * .5f;
       // vertices[112] += Vector3.forward * cheek + Vector3.right * cheek * .5f;
       // vertices[136] += Vector3.forward * cheek + Vector3.right * cheek * .5f;
       // vertices[142] += Vector3.forward * cheek + Vector3.right * cheek * .5f;
      //  vertices[9]   += Vector3.forward * cheek*.0f + Vector3.right * cheek * .5f;

        //left cheek 24 86 153
        vertices[24] += Vector3.forward * cheek;
        vertices[86] += Vector3.forward * cheek;
        vertices[87] += Vector3.forward * cheek*.66f;//eye socket
                                                     //vertices[153] += Vector3.forward * cheek;

        bool debugCubes = false;
        if (debugCubes)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Vector3 p = (vertices[i]);

                //p.x /= gameObject.transform.localScale.x;
                //  p.y *= gameObject.transform.localScale.y;
                // p.z *= gameObject.transform.localScale.z;

                p += gameObject.transform.position;
                c.transform.parent = gameObject.transform;

                c.transform.position = p;
                c.transform.localScale *= 0.1f;
                c.transform.name = i.ToString();
            }
        }

        mesh.RecalculateNormals();
        mesh.vertices = vertices;

        return mesh;
    }//too many vertices, abandoned

    private static class IcoSphere
    {
        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;

            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        // return index of point in the middle of p1 and p2
        private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
        }

        public static Mesh Create(GameObject gameObject,bool debugCubes)
        {
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.Clear();

            List<Vector3> vertList = new List<Vector3>();
            Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
            //int index = 0;

            int recursionLevel = 1;
            float radius = 1f;

            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

            vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

            vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));


            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                    int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                    int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            mesh.vertices = vertList.ToArray();

            List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }
            mesh.triangles = triList.ToArray();
            mesh.uv = new Vector2[mesh.vertices.Length];

            Vector3[] normales = new Vector3[vertList.Count];
            for (int i = 0; i < normales.Length; i++)
                normales[i] = vertList[i].normalized;


            mesh.normals = normales;

            mesh.RecalculateBounds();
          
           

            return mesh;
        }
    }
}
