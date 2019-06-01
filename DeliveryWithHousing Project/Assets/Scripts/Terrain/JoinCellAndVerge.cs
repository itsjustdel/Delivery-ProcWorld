using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoinCellAndVerge : MonoBehaviour {

 //   private GameObject road;
    private GameObject roadCell;
    private BezierSpline spline;
    private float closestInt;
    private float stepSize;
    private Vector3 centralPoint;
    List<Edge> edgeList = new List<Edge>();
    public bool forJunction = false;

    [HideInInspector]
    public List<Vector3> tri = new List<Vector3>();
    // Use this for initialization

    public bool createVerge;
    public bool findRoad;
    void Awake()
    {
        enabled = false;
    }
    void Start ()
    {

    //    FindRoad(); //starts daisy chain
    //    SmoothVerge();
        
       StartCoroutine("AddEdgesNearestRoad");
        
        
        //SphereCastForVerge();
       // CreateMesh();         //coroutines chain 
	}

    void Update()
    {
        if (findRoad)
        {
            FindRoad();
            findRoad = false;
        }

        if (createVerge)
        {
            StartCoroutine("AddEdgesNearestRoad");
            createVerge = false;
        }
    }

     public void FindRoad()
    {
        //spherecast from the central mesh point looking for a road. cnetral mesh point is always close to the road because this is the point
        //that was given to the voronoi mesh genereator. The centre of the cell.
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        centralPoint = vertices[0];
        GameObject road = null;
        RaycastHit[] hits = Physics.SphereCastAll(centralPoint + (Vector3.up * 10), 7f, Vector3.down, 20f, LayerMask.GetMask("Road"));
        foreach(RaycastHit hit in hits)
        {
            road = hit.collider.gameObject;

            SmoothVerge(road);
        }

        //StartCoroutine("SmoothVerge");
     //   SmoothVerge(road);
    }

    void SmoothVerge(GameObject road)
    {

     //   Debug.Log("Smoothing verge");
      //  yield return new WaitForEndOfFrame();

        //do not do(daisy chaining functions, last function calls building end)
      //  GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>().BuildingFinished();

        if (road == null)
        {
            Debug.Log("road null");
        }


        //get curve from road gameObject

        spline = road.transform.parent.transform.GetComponent<RingRoadMesh>().updatedSpline;

        if (spline == null)
            Debug.Log("null");

        //find point nearest central point on spline  

        float distance = Mathf.Infinity;
        Vector3 closestV3 = Vector3.zero;
        //closestInt = 0;
        float frequency = road.transform.parent.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount ;

        stepSize = 1f / frequency;
        for (float i = 0; i < frequency; i++)
        {

            Vector3 point = spline.GetPoint(i * stepSize);

            float temp = Vector3.Distance(centralPoint, point);
            if (temp < distance)
            {
                distance = temp;
                closestV3 = point;
                closestInt = i;
            }
        }

        //find which cell we are working on. The cell which overlaps the road
        RaycastHit roadCheck;
        if (Physics.Raycast(closestV3 + (Vector3.up * 10), Vector3.down, out roadCheck, 20f, LayerMask.GetMask("TerrainCell")))
        {
            roadCell = roadCheck.collider.gameObject;

            //if there is a child called Join
            if(roadCell.transform.Find("Join") != null)
            {
                //work has already begun on building the verge. Jump to stitching this together
                StartCoroutine("AddEdgesNearestRoad");
                return;
            }
            
        }
        else
        {
            //if it doesnt hit, it is because the terrain cell we are looking for has already been changed to a smooth verge
            //we do not need anything from this function
            StartCoroutine("AddEdgesNearestRoad");
            return;
        }

        
        //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //      cube.transform.position = closestV3;
        //      cube.transform.localScale *= 0.1f;

        int ySize = 0;

        List<Vector3> points = new List<Vector3>();

        //check to see if a smoothverge has been created at either side of the cell we are working on. This liets us know how wide to make this SmoothVerge

        

        int width = 3;
        //from the closest point on the curve, move back and run through a small section
        int spread = 3;
        int rowsToMiss = 0;
      //  int scale = 4;
        for (float i = closestInt - spread ; i <= closestInt + spread ; i +=0.5f)
        {


            Vector3 nextPoint = spline.GetPoint((i+1) * stepSize);
            Vector3 thisPoint = spline.GetPoint(i * stepSize);
            //check to see if a smooth verge has been built here, if it has, skip this row
        /*
            RaycastHit[] hitsNext = Physics.RaycastAll(nextPoint + Vector3.up, Vector3.down * 2, LayerMask.GetMask("SmoothVerge"));

            foreach(RaycastHit hit in hitsNext)
            {                
                if (hit.collider.gameObject.name == "Join")
                {
                 //   rowsToMiss++;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = hit.point;
                    cube.transform.localScale *= 0.1f;
                    cube.GetComponent<BoxCollider>().enabled = false;
                    cube.transform.parent = roadCell.transform;
                    cube.name = "next";
                    nextPointHit = true;
                }
            }

            RaycastHit[] hitsThis = Physics.RaycastAll(thisPoint + Vector3.up, Vector3.down * 2, LayerMask.GetMask("SmoothVerge"));         trying to stop overlaps. not working

            foreach (RaycastHit hit in hitsThis)
            {
                if (hit.collider.gameObject.name == "Join")
                {
                  //  rowsToMiss++;

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = hit.point;
                    cube.transform.localScale *= 0.1f;
                    cube.GetComponent<BoxCollider>().enabled = false;
                    cube.transform.parent = roadCell.transform;
                    cube.name = "this";

                    thisPointHit = true;
                }
            }
            */
                Vector3 direction = spline.GetDirection(i * stepSize);
            //direction.Normalize();

            Vector3 rot1 = Quaternion.Euler(0f, 90f, 0f) * direction;

            Vector3 offToTheSide = thisPoint - (rot1 * width);

            List<Vector3> temp = new List<Vector3>();

            for (float j = 0; j <= width * 2; j +=0.25f)
            {
                Vector3 pos = offToTheSide + (rot1 * j);

                temp.Add(pos);
        
                
            }
            
            //if list has not been cleared by a missed raycast, enter in to main points list//
            foreach (Vector3 v3 in temp)
            {
                points.Add(v3);
/*
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                     cube.transform.position = v3;
                     cube.transform.localScale *= 0.1f;
                    cube.GetComponent<BoxCollider>().enabled = false;
                cube.transform.parent = roadCell.transform;

                */
            }
        }


        int acrossRoad = width *2 * 4;
        int alongRoad = ((spread * 2) - rowsToMiss) * 2;


        int xSize = acrossRoad;
        ySize = alongRoad;

        int[] triangles = new int[(xSize * ySize) * 6];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

       

        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        //mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        ;

        GameObject verge = new GameObject();
        verge.transform.parent = roadCell.gameObject.transform;
        verge.name = "Join";

        verge.layer = 21;
        MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
        meshFilterInstance.mesh = mesh;
        MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
        // meshRenderer.enabled = false;
        meshRenderer.sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
        MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();
     
        //add cell Y adjust

        verge.AddComponent<CellYAdjust>();

        //disable road cell
        roadCell.GetComponent<MeshRenderer>().enabled = false;
        roadCell.GetComponent<MeshCollider>().enabled = false;

        //disable this mesh collider //cell of field
        GetComponent<MeshCollider>().enabled = false;

        /*
        //add this object the build list  so it can complete its second phase- build the joining verge to the field
        
        
        BuildList.BuildObject bo = new BuildList.BuildObject();
        bo.gameObject = gameObject;
        bo.type = "join";
        GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>().buildObjectList.Add(bo);
        */

        StartCoroutine("AddEdgesNearestRoad");

    //    yield break;
    }

    void DisableRoadCell()
    {
        roadCell.GetComponent<MeshRenderer>().enabled = false;
        roadCell.GetComponent<MeshCollider>().enabled = false;
    }

    IEnumerator AddEdgesNearestRoad()
    {
        //while (transform.FindChild("Join") == null)
        //{
        yield return new WaitForEndOfFrame();
   //     yield return new WaitForEndOfFrame();
   //     yield return new WaitForEndOfFrame();
        //}

        //     Debug.Log("adding edges nearest road");
        //   yield return new WaitForEndOfFrame();
        //find edge nearest road
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {

            //skip centre point
            if (i == 0)
                continue;

            //spherecast looking for a road hit from each vertice

            Vector3 position = vertices[i] + transform.position;
            RaycastHit hit;

            //if this vertice and next vertice hit a road, we ahve an edge we need to stitch
            if (Physics.SphereCast(position + (Vector3.up * 5), 2f, Vector3.down, out hit, 10f, LayerMask.GetMask("RoadSkirt")))
            {
                //adjust vertice index. skip 0, that is the central point
                int index = i + 1;
                if (index >= vertices.Length)
                    index = 1;

                Vector3 nextPosition = vertices[index];
                if (Physics.SphereCast(nextPosition + (Vector3.up * 5), 2f, Vector3.down, out hit, 10f, LayerMask.GetMask("RoadSkirt")))
                {
                    //if we have hit two points, add to an edge list
                    Edge edge = new Edge();
                    edge.p1 = vertices[i];
                    edge.p2 = vertices[index];

                    edgeList.Add(edge);
                }
            }

        //    yield return new WaitForEndOfFrame();
        }
        /*
        foreach(Edge edge in edgeList)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = edge.p1;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.transform.position = edge.p2;
        }
        */

       // StartCoroutine("SphereCastForVerge");
        SphereCastForVerge();
        yield break;
    }
    void SphereCastForVerge()
    {
          //every hit on a vertice is an outer point for the triangulator (1)
          //lerp between succesful hits to create inner points (0)

        if (edgeList.Count == 0)
            return;
        


        for (int i = 0; i < edgeList.Count; i++)
        {

            csTriangulation.Triangulation meshTriangle = new csTriangulation.Triangulation();
            List<Vector3> v3List = new List<Vector3>();
            List<int> intList = new List<int>();

            List<Vector3> originsList = new List<Vector3>();
            List<Vector3> hitsList = new List<Vector3>();

            Vector3 position = edgeList[i].p1;
            Vector3 direction = (edgeList[i].p2 - edgeList[i].p1).normalized;
            float distance = Vector3.Distance(edgeList[i].p1, edgeList[i].p2);

          

            for (float j = 0; j < distance; j+=0.05f)
            {
                RaycastHit[] hitsLast = (Physics.SphereCastAll(position + (direction * j) + (Vector3.up * 5), 1f, Vector3.down, 10f));
                
                foreach (RaycastHit hit in hitsLast)
                {
                    if (!forJunction)
                    {
                        //only looking for verges
                        if (hit.collider.gameObject.layer != 10 || hit.collider == null) //10 is roadskirt layer ..21 smoothverge
                        {
                            continue;
                        }
                    }
                    else if(forJunction)
                    {
                        //only looking for junction area
                        if (hit.collider.gameObject.tag != "CombinedMesh" || hit.collider == null) //10 is roadskirt layer ..21 smoothverge
                        {
                            continue;
                        }
                    }
                   
                    //add the nearest vertic from the triangle we hit
                    MeshCollider meshCollider = hit.collider as MeshCollider;

                    Mesh hitMesh = meshCollider.sharedMesh;
                    Vector3[] hitVertices = hitMesh.vertices;
                    int[] triangles = hitMesh.triangles;

                    Vector3 hit0 = hitVertices[triangles[hit.triangleIndex * 3 + 0]];
                    Vector3 hit1 = hitVertices[triangles[hit.triangleIndex * 3 + 1]];
                    Vector3 hit2 = hitVertices[triangles[hit.triangleIndex * 3 + 2]];

                    List<Vector3> hitList = new List<Vector3>();
                    hitList.Add(hit0);
                    hitList.Add(hit1);
                    hitList.Add(hit2);

                    float distanceToHit = Mathf.Infinity;
                    Vector3 closest = Vector3.zero;
                    for (int k = 0; k < hitList.Count; k++)
                    {
                        float temp = Vector3.Distance(hit.point, hitList[k]);

                        if (temp < distanceToHit)
                        {
                            closest = hitList[k];
                            distanceToHit = temp;
                        }
                    }


                    //add the point on the outline we are using
                    // Vector3 thisPoint = position + direction;

                    //if there isnt a duplicate on the x
                    if (!xChecker(closest,v3List))
                    {
                        v3List.Add(closest);
                        intList.Add(0);

                        hitsList.Add(closest);

                        //add the point we are casting from if we get a new hit
                        if (!xChecker((position + (direction * j)),v3List))
                        {
                            v3List.Add(position + (direction * j));
                            intList.Add(1);

                       //     originsList.Add(position + (direction * j));

                        }
                    }                   
                }            
            }

            //raycast looking for last point on the verge

            RaycastHit[] hits = (Physics.SphereCastAll(edgeList[i].p2 + (Vector3.up * 5), 1f, Vector3.down, 10f));

            foreach (RaycastHit hit in hits)
            {
                //only looking for smoothverge
                if (hit.collider.gameObject.layer != 10|| hit.collider == null)
                {
                    continue;
                }

                //add the nearest vertice from the triangle we hit
                MeshCollider meshCollider = hit.collider as MeshCollider;

                Mesh hitMesh = meshCollider.sharedMesh;
                Vector3[] hitVertices = hitMesh.vertices;
                int[] triangles = hitMesh.triangles;

                Vector3 hit0 = hitVertices[triangles[hit.triangleIndex * 3 + 0]];
                Vector3 hit1 = hitVertices[triangles[hit.triangleIndex * 3 + 1]];
                Vector3 hit2 = hitVertices[triangles[hit.triangleIndex * 3 + 2]];

                List<Vector3> hitList = new List<Vector3>();
                hitList.Add(hit0);
                hitList.Add(hit1);
                hitList.Add(hit2);

                float distanceToHit = Mathf.Infinity;
                Vector3 closest = Vector3.zero;
                for (int k = 0; k < hitList.Count; k++)
                {
                    float temp = Vector3.Distance(hit.point, hitList[k]);

                    if (temp < distanceToHit)
                    {
                        closest = hitList[k];
                        distanceToHit = temp;
                    }
                }


                //add the point on the outline we are using
                // Vector3 thisPoint = position + direction;
                if (!xChecker(closest,v3List))
                {
                    v3List.Add(closest);
                    intList.Add(0);

                 //   hitsList.Add(closest);
                }

                //add the last point
                if (!xChecker(edgeList[i].p2,v3List))
                {
                    v3List.Add(edgeList[i].p2);
                    intList.Add(1);

                //    originsList.Add(edgeList[i].p2);
                }
            }


            //we have created an outline above. Each point in the outline is part of a pair. We now need to find its partner and add a central point to assist triangulator script
            /*
            for (int h = 0; h < originsList.Count;h++)
            {
                Vector3 closest = Vector3.zero;
                distance = Mathf.Infinity;
      
                //find the closest point in the hit list to this point in origins list
                for(int j = 0; j < hitsList.Count; j++)
                {
                    float temp = Vector3.Distance(originsList[h], hitsList[j]);
                    if(temp < distance)
                    {
                        closest = hitsList[j];
                        distance = temp;              
                    }
                }

                //now we have the closest point, lerp halfway and insert point
                Vector3 middle = Vector3.Lerp(originsList[h], closest, 0.5f);
                v3List.Add(middle);
                intList.Add(0);
            }
            */
        // create list for triangulator, points and type of point
         for (int k = 0; k < v3List.Count; k++)
                {
                    meshTriangle.Add(v3List[k], intList[k]);
                }

            /*
            for (int k = 0; k < v3List.Count; k++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = v3List[k];
                cube.transform.localScale *= 0.1f;
                cube.transform.parent = this.transform;

                if (intList[k] == 0)
                {
                    cube.name = "0";
                    cube.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue", typeof(Material)) as Material;
                }
                else
                    cube.name = "1";
            }
            */

            /*
            if (hitsList.Count == 3)
            {
                foreach (Vector3 v3 in hitsList)
                {
                    tri.Add(v3);
                    CreateMesh();
                }

            }
            */
            //find triangles and call createMesh from cs Triangle
            if (v3List.Count > 3)
            {
                StartCoroutine(meshTriangle.Calculate(this));
            }
        }

        

    }

    bool xChecker (Vector3 v3, List<Vector3> v3List)
    {
        //returns true if there is a duplicate
        bool result = false;

        float x = v3.x;
        float z = v3.z;
        //sometimes a duplicate can slip through due to Y coordinate innacuracy. Due to spherecasting?
        //anyway, lets look for duplicates on the x coord and remove any doubles

        for (int i = 0; i < v3List.Count; i++)
        {

            double roundedX = x;
            double roundedZ = z;

            roundedX = System.Math.Round(roundedX, 3);
            roundedZ = System.Math.Round(roundedZ, 3);

            double roundedV3x = v3List[i].x;
            double roundedV3z = v3List[i].z;

            roundedV3x = System.Math.Round(roundedV3x, 3);
            roundedV3z = System.Math.Round(roundedV3z, 3);


            if (roundedV3x == roundedX && roundedV3z == roundedZ)
                result = true;
        }

        return result;
    }

    Vector2[] PreparePointsForTriangulation(List<Vector3> v3List)
    {
        //create vector2 array for triangulator script
        List<Vector2> v2List = new List<Vector2>();

        for (int i = 0; i < v3List.Count; i++ )
        {
            Vector2 v2 = new Vector2(v3List[i].x,v3List[i].z);
            v2List.Add(v2);
        }

        return v2List.ToArray();
    }
    public void CreateMesh()
    {

      //  Debug.Log("creating mesh");
        //called from tirangulation class when completed

        Mesh mesh = new Mesh();
        Mesh mesh2 = new Mesh();
        mesh.vertices = tri.ToArray();
        mesh2.vertices = tri.ToArray();
        //create triangles
        List<int> triangles = new List<int>();
        List<int> triangles2 = new List<int>();

        for (int i = 0; i < tri.Count - 2; i += 3)
        {
            triangles.Add(i + 0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);

            //make triangles double sided. Seems triangulator script gives out random results.
            //could raycast at centre of each triangle and flip ones that do hit? Does it really matter?//yes, for normals

            triangles2.Add(i + 0);
            triangles2.Add(i + 2);
            triangles2.Add(i + 1);
        }

        mesh.triangles = triangles.ToArray();
        mesh2.triangles = triangles2.ToArray();

        
        
        GameObject join = new GameObject();
        join.name = "Stitch";
        join.transform.parent = this.transform;
        join.layer = 16;
        MeshFilter meshFilter = join.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = join.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load("Green", typeof(Material)) as Material;

        MeshCollider meshCollider = join.AddComponent<MeshCollider>();

        join.AddComponent<CellYAdjust>();

        GameObject join2 = new GameObject();
        join2.name = "Stitch2";
        join2.transform.parent = this.transform;
        join2.layer = 16;
        MeshFilter meshFilter2 = join2.AddComponent<MeshFilter>();
        meshFilter2.mesh = mesh2;

        MeshRenderer meshRenderer2 = join2.AddComponent<MeshRenderer>();
        meshRenderer2.material = Resources.Load("Green", typeof(Material)) as Material;

        MeshCollider meshCollider2 = join2.AddComponent<MeshCollider>();

        join.AddComponent<CellYAdjust>();
        join2.AddComponent<CellYAdjust>();
        //tell build list we are finished

        //reset list
        tri.Clear();

      //  StartCoroutine("TopsideMesh" , join);
    }

    IEnumerator TopsideMesh(GameObject join)
    {
        yield return new WaitForEndOfFrame();

        //raycast from centre of each triangle in mesh
        Mesh mesh = join.GetComponent<MeshFilter>().mesh;

        int[] triangles = mesh.triangles;        
        Vector3[] vertices = mesh.vertices;

        List<int> tempTris = new List<int>();
        List<Vector3> tempVerts = new List<Vector3>();

        List<Vector3> centres = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            Vector3 centre = (p1 + p2 + p3) / 3;

            centres.Add(centre);
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = centre;
            cube.transform.localScale *= 0.1f;
            
        }

        for (int i = 0; i < centres.Count; i++)
        {
            RaycastHit hit;

            if (Physics.Raycast(centres[i] + (Vector3.up * 10), Vector3.down, out hit, 20f,LayerMask.GetMask("TerrainStitch")))
            {

                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                    yield break;

            //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.transform.position = hit.point;
            //    cube.transform.localScale *= 0.1f;
            //    cube.name = "from top";
            //    Debug.Log(hit.triangleIndex * 3 + 0);
                //get vertices of this triangle
                Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
                Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

                //add to list of vertices to be used for mesh creation
                tempVerts.Add(p0);
                tempVerts.Add(p1);
                tempVerts.Add(p2);

                //add triangle
                tempTris.Add(tempVerts.Count - 2);
                tempTris.Add(tempVerts.Count - 1);
                tempTris.Add(tempVerts.Count - 0);
            }

            
        }


        //assign new mesh

        GameObject upMesh = new GameObject();
        upMesh.transform.parent = this.transform;
        upMesh.name = "UpMesh";

        Mesh newMesh = new Mesh();
        newMesh.vertices = tempVerts.ToArray();
        newMesh.triangles = tempTris.ToArray();
        newMesh.name = "Upfacing Mesh";

        MeshFilter meshFilter = upMesh.AddComponent<MeshFilter>();
        meshFilter.mesh = newMesh;

        MeshRenderer meshRenderer = upMesh.AddComponent<MeshRenderer>();


        yield break;

    }

    class Edge
    {
        public Vector3 p1;
        public Vector3 p2;
        /*
        public Edge(int point1, int point2)
        {
            p1 = point1;
            p2 = point2;
        }
      */
      
    }
    /*
    void Update()
    {
        //  Debug.Log(tri.Length);
        for (int i = 0; i < tri.Count - 2; i += 3)
        {

            Debug.DrawLine(tri[i], tri[i + 1], Color.red);
            Debug.DrawLine(tri[i], tri[i + 2], Color.red);
            Debug.DrawLine(tri[i + 1], tri[i + 2], Color.red);

        }

    }
    */
}
