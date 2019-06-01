using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlantController : MonoBehaviour {

    public int stems = 2;
    public int stemsFinished = 0;
    public bool roses;
    public bool tulips;
    public bool bonzai;
    public bool shrub;
    public bool gardenTree;
    public bool cabbage;
    public bool addPot;
    public Material flowerMaterial;
    public Material leavesMaterial;
    public GameObject leafPrefab;
    public GameObject rosePrefab;
    public List<Vector3> leavesPositions = new List<Vector3>();
    public List<Vector3> rosesPositions = new List<Vector3>();
    public List<Vector3> branchPositions = new List<Vector3>();
    private PlantPot pot;
    private float segLength;
    public float gardenTreeRadius;
    public Vector3 intendedPosition;

    void Awake()
    {
       // this.enabled = false;
    }
    // Use this for initialization
    void Start()
    {
        rosePrefab = Resources.Load("Prefabs/Flora/IcoPrefab") as GameObject;
       // leafPrefab = Resources.Load("Prefabs/Flora/BushLeaves") as GameObject;
        pot = GetComponent<PlantPot>();
        GrabMaterials();


        if (addPot)
            StartCoroutine("BuildPot");
        else
            StartCoroutine("Plant");



    }

    void GrabMaterials()
    {
        //random material from predifined selection. Located in folders in project
  //      MaterialSetup ms = GameObject.FindGameObjectWithTag("Code").GetComponent<MaterialSetup>();
  //      flowerMaterial = ms.flowerMaterials[Random.Range(0, ms.flowerMaterials.Length)];
  //      leavesMaterial = ms.leavesMaterials[Random.Range(0, ms.leavesMaterials.Length)];
    }

    IEnumerator BuildPot()
    {
        gameObject.GetComponent<PlantPot>().enabled = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine("Plant");
    }

    IEnumerator Plant()
    {
       
        // pot.enabled = true;


        if(roses)
        {
            stems = Random.Range(1, 10);
            
        }
        else if (tulips)
        {
            stems = Random.Range(1, 10);
        }
        else if (bonzai)
        {
            stems = Random.Range(1, 4);
        }
        else if (shrub)//only builds 1 stem at a time and inserts in to branch array
        {
            stems = 1;
        }
        else if( gardenTree)
        {
            stems = 1;
        }
        else if(cabbage)
        {
            stems = 1;
        }
        else
        {
            Debug.Log("no plant type");
            yield break;
               
        }

        
        AddNextStem();        //bush built from array of pre built branches now ("Code".branchArray)
    }
    
    public void AddNextStem()
    {
        if (roses)
            StartCoroutine("AddRose");
        else if (tulips)
            StartCoroutine("AddTulip");
        else if (bonzai)
            StartCoroutine("AddBonzai");
        else if (shrub)
            StartCoroutine("AddShrub");
        else if (gardenTree)
            StartCoroutine("AddGardenTree");
        else if (cabbage)
            StartCoroutine("AddCabbage");
    }

    
    IEnumerator AddRose()
    {
        yield return new WaitForEndOfFrame();

        ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();
        pp.enabled = false;

        pp.MaxNumVertices = 1024;
        pp.NumberOfSides = 3;
        pp.BaseRadius = 0.4f;
        pp.RadiusStep = 0.8f;   //how wild it looks
        pp.MinimumRadius = 0.02f; //controls how large the plant is
        pp.BranchRoundness = 1f;
        pp.SegmentLength = 2f;
        pp.Twisting = 40f;
        pp.leavesUpStem = true;
        pp.addRoses = true;
        pp.enabled = true;
      //  pp.renderBranches = true; //needing to build on this script. pass positions etc. same way as branches and roses

        yield break;
    }
    IEnumerator AddTulip()
    {
        yield return new WaitForEndOfFrame();
        ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();
        pp.enabled = false;

        pp.MaxNumVertices = 1024;
        pp.NumberOfSides = 3;
        pp.BranchProbability = 0f;
        pp.BaseRadius = 0.2f;
        pp.RadiusStep = 0.8f;   //how wild it looks
        pp.MinimumRadius = 0.02f; //controls how large the plant is
        pp.Twisting = 10f;
        pp.BranchRoundness = 1f;
        pp.SegmentLength = Random.Range(1f, 2f);        
        pp.enabled = true;
        pp.leavesUpStem = false;
        pp.addRoses = true;
        GetComponent<Leaves>().enabled = true;
        //combine script will render
        GetComponent<Leaves>().render = false;

        yield break;
    }
    IEnumerator AddBonzai()
    {
        yield return new WaitForEndOfFrame();

        ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();
        pp.enabled = false;

        pp.MaxNumVertices = 1024;
        pp.NumberOfSides = 4;
        if (addPot)
        {
            pp.BaseRadius = pot.topRadius1 * 10; //dontknow why its times 10, is scalimg ok
        }
        else
        {
            //should bonzai aloways be in pot? probably
            pp.BaseRadius = 0.5f;
        }

        pp.RadiusStep = 0.8f;   //how wild it looks//randomise for bonzai?
        pp.MinimumRadius = 0.01f;
        pp.BranchRoundness = 1f;
        pp.SegmentLength = Random.Range(0.3f, 0.5f);
        pp.Twisting = 20f;//randomise
        pp.BranchProbability = 0.25f;
        pp.leavesUpStem = true;
        pp.addRoses = true;
        pp.enabled = true;
        //pp.renderBranches = true;
               

        yield break;
    }
    IEnumerator AddShrub()
    {

        yield return new WaitForEndOfFrame();
        ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();
        pp.enabled = false;

        pp.MaxNumVertices = 1024;
        pp.NumberOfSides = 3;
        pp.BaseRadius = 0.25f;
        pp.RadiusStep = 0.9f;
        pp.MinimumRadius = 0.02f;
        pp.BranchRoundness = 0.8f;
        pp.SegmentLength = 0.8f;// Random.Range(0.3f, 0.5f);
        segLength = pp.SegmentLength;
        pp.Twisting = 40f;//randomise
        pp.BranchProbability = 0.1f;// .1f;

        pp.shrub = true;
        pp.addRoses = false;
        pp.enabled = true;
        pp.leavesUpStem = true;

        yield break;
    }
    IEnumerator AddGardenTree() //OAK
    {

        yield return new WaitForEndOfFrame();
        
        ProceduralPlant pp = gameObject.AddComponent<ProceduralPlant>();
        pp.enabled = false;

        pp.MaxNumVertices = 5000;
        pp.NumberOfSides = 5;
        pp.BaseRadius = 0.4f;
        pp.RadiusStep = 0.95f;
        pp.MinimumRadius = 0.02f;
      //  pp.BranchRoundness = 0.8f;
        pp.SegmentLength = 0.4f;// Random.Range(0.3f, 0.5f);
        segLength = pp.SegmentLength;
        pp.Twisting = 10f;//randomise
        pp.BranchProbability = .25f;// .1f;

        pp.gardenTree = true;
        pp.addRoses = true;
        pp.enabled = true;
        pp.leavesUpStem = true;

        yield break;
    }

    IEnumerator AddCabbage()
    {
        yield return new WaitForEndOfFrame();

        GameObject head = new GameObject();
        head.name = "Cabbage Head";
        head.transform.parent = transform;
        head.transform.position = transform.position;
        head.transform.position += Vector3.up * 0.2f;//radius random
        head.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        MeshFilter meshFilter = head.AddComponent<MeshFilter>();
        meshFilter.mesh = Resources.Load("Prefabs/Flora/IcoMesh") as Mesh;
      //  IcoMesh icoMesh = head.AddComponent<IcoMesh>();
      //  icoMesh.randomScale = 0.1f;

        MeshRenderer meshRenderer = head.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load("Yellow") as Material;

        //combinemeshes script will render a combined version of the emsh with others from the field
        meshRenderer.enabled = false;


        
  
        Leaves leaves = gameObject.AddComponent<Leaves>();
        leaves.radius = 0.2f;
        leaves.enabled = true;

        yield break;
    }
    public IEnumerator AddLeavesMaster()//changed to cube prefab - can use leafcombined mesh - copy of function in BranchArray
    {

        if (shrub)
            yield break;
        // yield return new WaitForEndOfFrame();
        /*
        foreach (Vector3 v3 in leavesPositions)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = v3;
        }
        */
        GameObject leaves = new GameObject();
        leaves.transform.parent = transform;
        Vector3 pos = leaves.transform.position;
        if(addPot)
            pos.y += (GetComponent<PlantPot>().height) - (GetComponent<PlantPot>().potThickness / 2);

        leaves.transform.localPosition = pos;
        leaves.name = "Leaves";

        //   GameObject leavesInstance = Instantiate(leafPrefab);

        Mesh newMesh = new Mesh();
        newMesh.name = "Leaves As One Cubes";

        GameObject leavesParent = Resources.Load("Prefabs/Flora/LeavesParent") as GameObject;
        //create tempLists
        List<Vector3> leavesVerts = new List<Vector3>();
        List<int> leavesTris = new List<int>();
        for (int x = 0; x < leavesParent.transform.childCount; x++)
        {

            //grab each cube in prefab
            Mesh leavesPrefabMesh = leavesParent.transform.GetChild(x).GetComponent<MeshFilter>().mesh;




            List<Vector3> tempVerts = new List<Vector3>();
            foreach (Vector3 v in leavesPrefabMesh.vertices)
                tempVerts.Add(v);

            List<int> tempInts = new List<int>();

            foreach (int i in leavesPrefabMesh.triangles)
                tempInts.Add(i);


            //add vertices from prefab to new mesh
            for (int i = 0; i < leavesPositions.Count; i++)  //minus count number stops leaves growing at bottom
            {
                //rotate (if)

                Quaternion q = Quaternion.identity;
                if (roses || bonzai)
                {
                    q = Random.rotation; //Quaternion.Euler(0, Random.Range(80f, 100f), 0);//
                }

                if (shrub)
                    q = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(0f, 360f), Random.Range(-10f, 10f));//

                List<Vector3> alteredVerts = new List<Vector3>();
                for (int j = 0; j < tempVerts.Count; j++)
                {
                    Vector3 v = tempVerts[j];
                    //scale
                    if (bonzai)
                        v *= 0.25f; //segment length?pot size?
                                    //rotate
                    v = q * v;
                    alteredVerts.Add(v);
                }

                for (int j = 0; j < alteredVerts.Count; j++)
                {
                    Vector3 position = alteredVerts[j] + leavesPositions[i];
                    leavesVerts.Add(position);
                }
            }

            //add triangles

            //run through verts and add triangles for each prefab/combined mesh
            for (int i = 0; i < leavesPositions.Count; i++)
            {
                //foreach prefab mesh
                for (int j = 0; j < tempInts.Count; j++)
                {
                    leavesTris.Add(tempInts[j] + (tempVerts.Count * i));
                }
            }
        }


        //create mesh
        newMesh.vertices = leavesVerts.ToArray();
        newMesh.triangles = leavesTris.ToArray();
        newMesh.RecalculateNormals();
        MeshFilter meshFilter = leaves.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = newMesh;
        MeshRenderer meshRenderer = leaves.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Green") as Material;
        meshRenderer.enabled = false;


        AddRosesMaster();
        //Scale();
       // gameObject.AddComponent<CombineChildren>();

           yield break;
    }

    void AddRosesMaster()
    {
        
        for(int i = 0; i < rosesPositions.Count; i++)
        {
            GameObject roses = new GameObject();
            roses.transform.parent = transform;
            Vector3 rosepos = roses.transform.position;
            if(addPot)
                rosepos.y += (GetComponent<PlantPot>().height) - (GetComponent<PlantPot>().potThickness / 2);

            roses.transform.localPosition = rosepos;
            roses.name = "Roses";

            //GameObject roseInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject roseInstance = Instantiate(rosePrefab);
            roseInstance.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("RosePink") as Material;
            roseInstance.GetComponent<MeshRenderer>().enabled = false;
            roseInstance.transform.localPosition = rosesPositions[i] + transform.position;// + random;
            
            roseInstance.transform.transform.parent = roses.transform;
            if (roses)
            {
                roseInstance.transform.localScale = Vector3.one * 2;
            }
            if (bonzai)
            {
                roseInstance.transform.localScale = Vector3.one/2;
            }
            
       //     roseInstance.transform.localScale *= segLength;
        }

        Scale();

       
    }

    public void Scale()
    {
        StartCoroutine("WaitAndScale");
        
    }

    IEnumerator WaitAndScale()
    {
        yield return new WaitForEndOfFrame();
        if (shrub)
            yield break;

       
       

        float random = Random.Range(0.03f, 0.06f);

        int childCount = transform.childCount;
     //   Debug.Log(childCount);
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (addPot)
            {
                if (child.name == "Roses" || child.name == "Branches" || child.name == "Leaves")
                {
                //    if (child.transform.localScale == Vector3.one)
                //    {
                //        if (!shrub)
                //        {
                //if not shrub, size is governed by plant pot size
                child.localScale *= GetComponent<PlantPot>().topRadius1 / 4;
                }
            }
            else
            {
                //if not in a pot, scaled to a nice looking size
                child.localScale *= random;
                //transform.localScale *= random;
            }

            //}
        }
        gameObject.AddComponent<CombineChildren>();
        //  GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>().BuildingFinished();

        yield break;
    }
}



