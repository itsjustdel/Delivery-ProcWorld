using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddToHitRoadCell : MonoBehaviour {

    public bool working;
    public int subDivision = 16;
    public float stepSize = 0.2f;
    public Mesh testMesh;
    void Awake()
    {
        enabled = false;
    }
    // Use this for initialization
    void Start()
    {

        // Add();
        //StartCoroutine("Add");
        //StartCoroutine("GridInsidePolygon");
        SubdivideFromOuter();
    }

    void SubdivideFromOuter()
    {
        

        Mesh newMesh = SubdivideMesh.SubdivideUsingOuterPoints(GetComponent<MeshFilter>().mesh,stepSize);

        if (newMesh == null)
        {
            //Debug.Log("here");
            MeshFilter MF = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = MF.mesh;
            //create instance of Mesh helper class
            MeshHelper meshHelper = new MeshHelper();
            int amount = subDivision;
            //send this object's mesh, how many subdivisions, and this gameObject to subdivision script
            StartCoroutine(meshHelper.Subdivide(mesh, amount, gameObject, true)); //96 max  // divides a single quad into 6x6 quads // normally 32
            MF.mesh = mesh;
        }
        else
        {

            GameObject subdivided = new GameObject();
            subdivided.transform.parent = transform;
            MeshFilter meshfilter = subdivided.AddComponent<MeshFilter>();
            meshfilter.mesh = newMesh;
            MeshRenderer meshRenderer = subdivided.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;

            subdivided.AddComponent<MeshCollider>();

            
            CellYAdjust cya = subdivided.AddComponent<CellYAdjust>();
            cya.addAutoWeld = true;
            cya.rayYAmount = 1000f;//super overkill

            GetComponent<MeshCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            
        }
    }

    IEnumerator GridInsidePolygon()
    {

        gameObject.AddComponent<GridInsidePolygon>();

        yield break;
    }

    void Add()
    {  
        //add subdivide
        Subdivide(subDivision); //if for deform mesh. 8,16, doesnt work. confuses triangles/vertices
       
       // working = true;

        //wait for subdivide to complete // Once subdivide is finished, it flags this finsihed e.g working = false;
       // while (working)
       //     yield return new WaitForEndOfFrame();


        //get list of outer points
        float threshold = 0.0001f;
        Mesh thisMesh = GetComponent<MeshFilter>().mesh;
        List<int> outerPoints = FindEdges.EdgeVertices(thisMesh, threshold);
        foreach(int i in outerPoints)
        {
           // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
           // c.transform.localScale *= 0.1f;
           // c.transform.position = thisMesh.vertices[i];
        }

        //add auto weld
        // AutoWeld(0.01f);

        //autoWeldedMesh = mesh;
        threshold = 1f;
        thisMesh = AutoWeld.AutoWeldFunctionIgnoringOuter(thisMesh,threshold,2000,outerPoints);
        GetComponent<MeshFilter>().mesh = thisMesh;
        //wait for unity to update
        //  yield return new WaitForEndOfFrame();

        // AutoWeld(0.001f);

        //wait for unity to update
        // yield return new WaitForEndOfFrame();
        //    gameObject.AddComponent<DeformController>();

        //yield break;
    }

    

    void Subdivide(int amount)
    {
        SubdivideMesh sm = gameObject.AddComponent<SubdivideMesh>();
        sm.amount = amount;
        // sm.uniqueVertices = true;
        
    }

    void CombineMeshes()
    {
        CombineChildren cc = gameObject.AddComponent<CombineChildren>();
        cc.addMeshCollider = true;
    }
    
}
