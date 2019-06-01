using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeformController : MonoBehaviour {

    public List<MeshAndIndex> partners = new List<MeshAndIndex>();
    private List<Vector3> centres = new List<Vector3>();
    private List<List<int>> verticesForCentre = new List<List<int>>();

    public bool subdivide;
    public int divisionAmount = 4;
    public GameObject van;
    public bool updateCollider;
    public Vector3[] deformedVertices;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public bool hitByWheel;
    public bool mouse;
    float limit = 0.1f;//how far the vertices can fall
    // Use this for initialization
    void Start ()
    {


        meshFilter = gameObject.GetComponent<MeshFilter>();
        //speed things up
        meshFilter.mesh.MarkDynamic();

        meshCollider = gameObject.GetComponent<MeshCollider>();



        if (subdivide)
        {
            SubdivideMesh sm = gameObject.AddComponent<SubdivideMesh>();
            sm.blockYAdjust = true;
            sm.amount = divisionAmount;
            
        }

        StartCoroutine("Wait");




    }

    void Update()
    {
        if (hitByWheel || mouse)
        {
            meshFilter.mesh.vertices = deformedVertices;
            meshFilter.mesh.RecalculateNormals();

            updateCollider = true;

            hitByWheel = false;
        }

    }

    void FixedUpdate()
    {
        if (updateCollider)
        {
            meshCollider.sharedMesh = meshFilter.mesh;                    
        }

        updateCollider = false;
    }

    public void DoUpdate()
    {
        
        meshCollider.sharedMesh = meshFilter.mesh;
     
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        gameObject.AddComponent<OriginalMeshInfo>();

   //     gameObject.AddComponent<ApplyCollider>();

        gameObject.AddComponent<FindEdges>();
        //     gameObject.AddComponent<StitchDeformableMesh>();  
        CreateCentresList();

    //    if(!mouse)
    //        van.SetActive(true);
    }

    //update array which is then slipped in on fixed update - called from Deform mesh which is attached to tires
    public void updateDeformedVertices(int i, Vector3 position,bool updatePartner)
    {
        //insert value in to array
        deformedVertices[i] = position;

        if (!updatePartner)
            return;

        //check to see if this is an edge vertice for the mesh attached to this transform

        List<int> edgeVertices = GetComponent<FindEdges>().edgeVertices;

        for(int j = 0; j < edgeVertices.Count; j++)
        {
            if(i == edgeVertices[j])
            {
                //we have a match
                //each vertice on the edge has a partner/partners. When one of these moves, we need to move its partner too

                //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube.transform.position = meshFilter.mesh.vertices[i] + transform.position;
                //    cube.transform.localScale *= 0.2f;
                //    cube.transform.parent = transform;      //working places on edge vertice

                //make the partner the same as the point we just moved

                //find partner
          //      Debug.Log(partners.Count);
                for(int k = 0; k < partners.Count;k++)
                {
                    if(partners[k].thisVerticeNumber == i)
                    {
                        //adjust partners' vertice array
                        int otherIndex = partners[k].otherVerticeNumber;
                        Mesh otherMesh = partners[k].otherTransform.GetComponent<MeshFilter>().mesh;
                        DeformController otherDeformController = partners[k].otherTransform.GetComponent<DeformController>();
                        FindEdges otherFindEdges = partners[k].otherTransform.GetComponent<FindEdges>();
                        Vector3 otherPosition = partners[k].otherTransform.GetComponent<MeshFilter>().mesh.vertices[otherIndex] + partners[k].otherTransform.position;

                        //only alter the y co-ordinate, the x and y are relatice to their own mesh/gameobject, so cant be used.
                        //If we want to match the x and y, perhaps creating a delta/amount to move variable instead of passing a full vector3
               
                        //correct vertice being found -- find out how to adjust this vertice in its mesh
                   //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   //     cube.transform.position = partners[k].otherTransform.GetComponent<MeshFilter>().mesh.vertices[otherIndex] + partners[k].otherTransform.position;
                   //     cube.transform.localScale *= 0.2f;
                   //     cube.transform.parent = partners[k].otherTransform;

                        Vector3 correctWorldPosition = partners[k].otherTransform.GetComponent<MeshFilter>().mesh.vertices[otherIndex];// + partners[k].otherTransform.position;
                        correctWorldPosition.y = position.y;

                   //     Mesh otherOriginalMesh = partners[k].otherTransform.GetComponent<OriginalMeshInfo>().originalMesh;
                   //     correctWorldPosition.y = Mathf.Clamp(correctWorldPosition.y, otherOriginalMesh.vertices[otherMesh.triangles[otherIndex]].y - 0.2f, otherOriginalMesh.vertices[otherMesh.triangles[otherIndex]].y - 0.2f + 0.2f);


                        otherDeformController.updateDeformedVertices(otherIndex, correctWorldPosition, false);
                        otherDeformController.hitByWheel = true;
                    }
                }

            }
        }
    }

    public void displayCorrectTriangle(Vector3 position, bool updatePartner)
    {
        //top section done at start? just a lookup table?

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //hit point of ray is passed through
        List<Vector3> centres = new List<Vector3>();
        List<List<int>> verticesForCentre = new List<List<int>>();
       
        //Mesh data calls can be VERY expensive so cache it first
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length;)
        {
            List<int> localvertices = new List<int>();

            Vector3 P1 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;
            Vector3 P2 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;
            Vector3 P3 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;

            centres.Add((P1 + P2 + P3) / 3);

            if (i != 0 && i %3 == 0)
            {
             
                verticesForCentre.Add(localvertices);
            }
            
        }
        //find closest triangle     //factoring reduces GC?
        
        float distance = Mathf.Infinity;
        int closest = 0;

        for(int i = 0; i < centres.Count; i++)
        {
            float temp = Vector3.Distance(centres[i], position);
            if( temp< distance)
            {
                distance = temp;
                closest = i;
            }
        }

        //centre
       // Debug.DrawLine(centres[closest], centres[closest] + Vector3.up,Color.blue);

        //each vertice

        List<int> closestInts = verticesForCentre[closest];

        Vector3 p0 = vertices[triangles[closestInts[0]]];
        Vector3 p1 = vertices[triangles[closestInts[1]]];
        Vector3 p2 = vertices[triangles[closestInts[2]]];
        p0.y -= 0.1f;
        p1.y -= 0.1f;
        p2.y -= 0.1f;

        Mesh originalMesh = GetComponent<OriginalMeshInfo>().originalMesh;
        Vector3[] originalMeshVertices = originalMesh.vertices;
        
        p0.y = Mathf.Clamp(p0.y, originalMeshVertices[triangles[closestInts[0]]].y - limit, originalMeshVertices[triangles[closestInts[0]]].y + limit);
        p1.y = Mathf.Clamp(p1.y, originalMeshVertices[triangles[closestInts[1]]].y - limit, originalMeshVertices[triangles[closestInts[1]]].y + limit);
        p2.y = Mathf.Clamp(p2.y, originalMeshVertices[triangles[closestInts[2]]].y - limit, originalMeshVertices[triangles[closestInts[2]]].y + limit);


        deformedVertices[triangles[closestInts[0]]] = p0;
        deformedVertices[triangles[closestInts[1]]] = p1;
        deformedVertices[triangles[closestInts[2]]] = p2;

        if (!updatePartner)
            return;

        //spherecast from centre point looking for other deform controllers

        List<DeformController> deformControllers = new List<DeformController>();

        //spherecast - may be enough to do a ring of raycasts to reduce GC
        RaycastHit[] hits = Physics.SphereCastAll(centres[closest] + Vector3.up, 0.2f, Vector3.down, LayerMask.GetMask("TerrainCell"));
        //for each deform controller, create an barycentre array like above

        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.GetComponent<DeformController>() != null && hit.transform.GetComponent<DeformController>() != this)
            {
                deformControllers.Add(hit.transform.GetComponent<DeformController>());
            }
        }

        foreach (DeformController dc in deformControllers)
        {
            //find the closest triangle to the hit point of the hit.point
            Mesh otherMesh = dc.transform.GetComponent<MeshFilter>().mesh;
            
            List<Vector3> otherCentres = new List<Vector3>();
            List<List<int>> otherVerticesForCentre = new List<List<int>>();

            //Mesh data calls can be VERY expensive so cache it first
            Vector3[] otherVertices = otherMesh.vertices;
            int[] otherTriangles = otherMesh.triangles;
            for (int i = 0; i < otherTriangles.Length;)
            {
                List<int> localvertices = new List<int>();

                Vector3 P1 = otherVertices[otherTriangles[i]];
                localvertices.Add(i);
                i++;
                Vector3 P2 = otherVertices[otherTriangles[i]];
                localvertices.Add(i);
                i++;
                Vector3 P3 = otherVertices[otherTriangles[i]];
                localvertices.Add(i);
                i++;

                otherCentres.Add((P1 + P2 + P3) / 3);

                if (i != 0 && i % 3 == 0)
                {

                    otherVerticesForCentre.Add(localvertices);
                }

            }
            //find closest triangle     //factoring reduces GC?

            float otherDistance = Mathf.Infinity;
            int otherClosest = 0;

            for (int i = 0; i < otherCentres.Count; i++)
            {
                float temp = Vector3.Distance(otherCentres[i], position);
                if (temp < otherDistance)
                {
                    otherDistance = temp;
                    otherClosest = i;
                }
            }

       //     Debug.DrawLine(otherCentres[otherClosest], otherCentres[otherClosest] + Vector3.up, Color.cyan);

            //move the verts which beling to this triangle

            List<int> otherClosestInts = otherVerticesForCentre[otherClosest];

            Vector3 p3 = otherVertices[otherTriangles[otherClosestInts[0]]];
            Vector3 p4 = otherVertices[otherTriangles[otherClosestInts[1]]];
            Vector3 p5 = otherVertices[otherTriangles[otherClosestInts[2]]];
            p3.y -= 0.1f;
            p4.y -= 0.1f;
            p5.y -= 0.1f;

            Mesh otherOriginalMesh = dc.transform.GetComponent<OriginalMeshInfo>().originalMesh;
            Vector3[] otherOriginalMeshVertices = otherOriginalMesh.vertices;
            
            p3.y = Mathf.Clamp(p3.y, otherOriginalMeshVertices[otherTriangles[otherClosestInts[0]]].y - limit, otherOriginalMeshVertices[otherTriangles[otherClosestInts[0]]].y + limit);
            p4.y = Mathf.Clamp(p4.y, otherOriginalMeshVertices[otherTriangles[otherClosestInts[1]]].y - limit, otherOriginalMeshVertices[otherTriangles[otherClosestInts[1]]].y + limit);
            p5.y = Mathf.Clamp(p5.y, otherOriginalMeshVertices[otherTriangles[otherClosestInts[2]]].y - limit, otherOriginalMeshVertices[otherTriangles[otherClosestInts[2]]].y + limit);
            
            dc.deformedVertices[otherTriangles[otherClosestInts[0]]] = p3;
            dc.deformedVertices[otherTriangles[otherClosestInts[1]]] = p4;
            dc.deformedVertices[otherTriangles[otherClosestInts[2]]] = p5;
            dc.hitByWheel = true;

        }
    }

    void CreateCentresList()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh; //fed from call
        //hit point of ray is passed through
       // List<Vector3> centres = new List<Vector3>();
       // List<List<int>> verticesForCentre = new List<List<int>>();

        //Mesh data calls can be VERY expensive so cache it first       ///create this array at Start()
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length;)
        {
            List<int> localvertices = new List<int>();

            Vector3 P1 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;
            Vector3 P2 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;
            Vector3 P3 = vertices[triangles[i]];
            localvertices.Add(i);
            i++;

            centres.Add((P1 + P2 + P3) / 3);

            if (i != 0 && i % 3 == 0)
            {

                verticesForCentre.Add(localvertices);
            }

        }
    }

    public void MoveTriangle(GameObject go,Vector3 position,bool updatePartner)
    {
        DeformController dc = go.GetComponent<DeformController>();

        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        //find closest triangle     //factoring reduces GC?

        float distance = Mathf.Infinity;
        int closest = 0;

        for (int i = 0; i < dc.centres.Count; i++)
        {
            float temp = Vector3.Distance(dc.centres[i], position);
            if (temp < distance)
            {
                distance = temp;
                closest = i;
            }
        }

        //centre
 //       Debug.DrawLine(dc.centres[closest], dc.centres[closest] + Vector3.up, Color.blue);

        //each vertice
        
        List<int> closestInts = dc.verticesForCentre[closest];

        Vector3 p0 = vertices[triangles[closestInts[0]]];
        Vector3 p1 = vertices[triangles[closestInts[1]]];
        Vector3 p2 = vertices[triangles[closestInts[2]]];
        p0.y -= 0.1f;
        p1.y -= 0.1f;
        p2.y -= 0.1f;

        Mesh originalMesh = go.GetComponent<OriginalMeshInfo>().originalMesh;
        Vector3[] originalMeshVertices = originalMesh.vertices;
        
        p0.y = Mathf.Clamp(p0.y, originalMeshVertices[triangles[closestInts[0]]].y - limit, originalMeshVertices[triangles[closestInts[0]]].y + limit);
        p1.y = Mathf.Clamp(p1.y, originalMeshVertices[triangles[closestInts[1]]].y - limit, originalMeshVertices[triangles[closestInts[1]]].y + limit);
        p2.y = Mathf.Clamp(p2.y, originalMeshVertices[triangles[closestInts[2]]].y - limit, originalMeshVertices[triangles[closestInts[2]]].y + limit);

        

        dc.deformedVertices[triangles[closestInts[0]]] = p0;
        dc.deformedVertices[triangles[closestInts[1]]] = p1;
        dc.deformedVertices[triangles[closestInts[2]]] = p2;

        if(updatePartner)
        {
            //spherecast - may be enough to do a ring of raycasts to reduce GC
            RaycastHit[] hits = Physics.SphereCastAll(dc.centres[closest] + Vector3.up, 0.2f, Vector3.down, LayerMask.GetMask("TerrainCell"));
            //for each deform controller, create an barycentre array like above

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<DeformController>() != null && hit.transform.GetComponent<DeformController>() != this)
                {
                    DeformController hitDc = hit.transform.GetComponent<DeformController>();
                    dc.MoveTriangle(hitDc.gameObject, hit.point, false);
                }
            }
        }
    }



    public class MeshAndIndex
    {
    //public Mesh thisMesh;
    public int thisVerticeNumber;

    public Transform otherTransform;
    public int otherVerticeNumber;

    }
}


