using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionInfo : MonoBehaviour {

    public float area;
    public Vector3 centroid;
    public List<Vector3> pointsOnEdge = new List<Vector3>();
    public List<EdgeHelpers.Edge> boundaryPath;
    public Vector3 forwardDir;

    void Awake()
    {
        this.enabled = false;
    }
	
	void Start ()
    {
        

        CreateListOfEdgePoints();
        //Vector3[] mVertices = transform.FindChild("Junction Corner").GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] mVertices = pointsOnEdge.ToArray();
        // area = Area(mVertices);

        // centroid = FindCentroid(mVertices);
        centroid = compute3DPolygonCentroid(mVertices);
        
        PlaceHouse();

       //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       //  cube.transform.position = centroid;
       //  cube.name = "Centroid";
         StartCoroutine("RaycastForHouseSize");
         
    }


    void CreateListOfEdgePoints()
    {
        if (transform.childCount == 0)
        {
            Debug.Log("returning");
            return;
        }

        Mesh mesh = transform.Find("Combined mesh").GetComponent<MeshFilter>().mesh;
        //use the EdgeHelpers static class to work out the boundary could also be called in BushesForCell?
        boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges();
        //creates a list of points round the edge of a field/cell with no duplicates or poitns within one unity of each other
        
        //centre point on renderer
        //Used to find out which way the bushes should move towards so that they leave a space around the edge of the field
        Vector3 centrePoint = transform.Find("Combined mesh").GetComponent<MeshRenderer>().bounds.center;
        //use temporary array
        Vector3[] vertices = mesh.vertices;


        for (int i = 0; i < boundaryPath.Count; i++)
        {
            if (i == 0)
            {
                pointsOnEdge.Add(vertices[boundaryPath[i].v1]);
                pointsOnEdge.Add(vertices[boundaryPath[i].v2]);
            }
            if (i > 0)
            {
                Vector3 pos = vertices[boundaryPath[i].v2];
                //if this does not match the previous entry, add it
                if (Vector3.Distance(pos, pointsOnEdge[pointsOnEdge.Count - 1]) > 0.5f)
                    pointsOnEdge.Add(vertices[boundaryPath[i].v2]);
            }

            //the autoweld script leaves some vertices at the end sometimes, so we need ot check if we have finished our loop
            //do this by checking the distance to the first vertice
            //do not do on first 2 points

            if (i < 2)
                continue;

            if (Vector3.Distance(pointsOnEdge[0], pointsOnEdge[pointsOnEdge.Count - 1]) < 0.5f)
            {
                //jump out the for loop
                break;
            }
        }


        foreach (Vector3 v3 in pointsOnEdge)
        {
            //     GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
           //      cube2.name = "FenceCube";
            //    cube2.transform.position = v3;
        }
    }


    void PlaceHouse()
    {

        if (transform.childCount == 0)
        {
            Debug.Log("returning");
            return;
        }

        GameObject procHouse = Instantiate(Resources.Load("Prefabs/Housing/ProcHouse") as GameObject, centroid, Quaternion.identity) as GameObject;
        procHouse.transform.parent = transform;

    }

    IEnumerator RaycastForHouseSize()
    {

        if (transform.childCount == 0)
        {
            Debug.Log("returning");
           yield break;
        }
        //find closest edge of polygon to centre of cell

        //raycast in circle getting larger until a cell which is no this one is hit
        StretchQuads sq = transform.GetComponentInChildren<StretchQuads>();
        float brickSize = sq.brickSize;

        RaycastHit hit;
        Physics.SphereCast(centroid + (Vector3.up * 50), 20f, Vector3.down, out hit, 100f, LayerMask.GetMask("TerrainCell"));

    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    ///    cube.transform.position = hit.point;
    //    cube.name = "Z";

        float limit1 = Vector3.Distance(centroid, hit.point) / brickSize;
        limit1 *= 0.66f;
        limit1 = Mathf.Round(limit1);
        
    //    Debug.Log(limit1);
        

        //we have found one limit. 
        //now we need to find the limit at right angle to this
        //take the direction from the centroid to the limit point we ahve found, rotate it 90 degrees, and check for edge of cells using raycast
        //grab the brickSize from the house prefab and raycast every brick size to find edge

        
      //  Debug.Log(brickSize);
        Vector3 dir = centroid - hit.point;
        //store in public var for rotation after meshes are built
        forwardDir = dir;

        RaycastHit hitL;
        RaycastHit hitR;
        Vector3 limit2pos = Vector3.zero;
        bool hasHitTerrainCell = false;
        float i = 0f;
        int bricks = 0;
        while (hasHitTerrainCell == false)
        {
            Vector3 dirR = Quaternion.Euler(0, 90f, 0) * dir;
            Vector3 dirL = Quaternion.Euler(0, -90f, 0) * dir;

            if(Physics.Raycast(centroid + (dirL * i) + Vector3.up, Vector3.down, out hitL, 10f, LayerMask.GetMask("TerrainCell")))
            {
                hasHitTerrainCell = true;
                limit2pos = hitL.point;
            }
            

            if (Physics.Raycast(centroid + (dirR * i) + Vector3.up, Vector3.down, out hitR, 10f, LayerMask.GetMask("TerrainCell")))
            {
                hasHitTerrainCell = true;
                limit2pos = hitR.point;
            }

            i+=brickSize;
            bricks++;


            if (i==1000)
            {
                Debug.Log("Stuck in while loop - breaking");
                hasHitTerrainCell = true;
                break;
            }
            
            yield return new WaitForEndOfFrame();
        }

   //     Debug.Log("out of while");
        yield return new WaitForEndOfFrame();


        //limit to 60%

        bricks = bricks / 5;
        bricks *= 4;

        float limit2 = bricks * 2;

        
        
        //limit x to 2/3 of z
        limit2 = Mathf.Clamp(limit2, limit1, limit1 * 0.66f);
        
   //     GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
   //     cube2.transform.position = limit2pos;
   //     cube2.name = "X";

        // now we have our two limits, assign the short one to Z on the procHouse, and the longer one on X (sometimes switch it?)
        //this means the roof will be built with the spine along the longest axis

        float length1 = Vector3.Distance(centroid, hit.point);
        float length2 = Vector3.Distance(centroid, limit2pos);

      //  if(length1 >= length2)
      //  {
 //           sq.targetSizeX = limit2;
 //           sq.targetSizeZ = limit1;
        //  }
        //  else
        //  {
        //      sq.targetSizeX = limit1;
        //     sq.targetSizeZ = limit2;
        //}

         transform.Find("ProcHouse(Clone)").Find("Meshes").GetComponent<StretchQuads>().enabled = true;
        yield break;
    }
   

    public float Area(Vector3[] mVertices)
    {
        float result = 0;
        for (int p = mVertices.Length - 1, q = 0; q < mVertices.Length; p = q++)
        {
            result += (Vector3.Cross(mVertices[q], mVertices[p])).magnitude;
        }
        return result * 0.5f;
    }

    public Vector3 FindCentralPointByAverage(Vector3[] mVertices)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        for (int i = 0; i < mVertices.Length; i++)
        {
            x += mVertices[i].x;
            y += mVertices[i].y;
            z += mVertices[i].z;
        }

        x = x / mVertices.Length;
        y = y / mVertices.Length;
        z = z / mVertices.Length;

        Vector3 centre = new Vector3(x, y, z);

        return centre;
    }

    Vector3 compute3DPolygonCentroid(Vector3[] vertices)//, int vertexCount)
{
    Vector3 centroid = new Vector3();
    float signedArea = 0.0f;
    float x0 = 0.0f; // Current vertex X
    float y0 = 0.0f; // Current vertex Y
    float x1 = 0.0f; // Next vertex X
    float y1 = 0.0f; // Next vertex Y
   
    float a = 0.0f;  // Partial signed area

    // For all vertices
    int i = 0;
    for (i=0; i<vertices.Length -1; i++)
    {
        x0 = vertices[i].x;
        y0 = vertices[i].z;
        
        x1 = vertices[(i + 1) % vertices.Length].x;
        y1 = vertices[(i + 1) % vertices.Length].z;
       
        a = (x0* y1) - (x1* y0) ;
        signedArea += a;
        centroid.x += (x0 + x1)*a;
        centroid.z += (y0 + y1)*a;
    }

    signedArea *= 0.5f;
    centroid.x /= (6.0f* signedArea);
    centroid.z /= (6.0f* signedArea);

        //raycast for y position
        Vector3 from = new Vector3(centroid.x, 1000f, centroid.z);
        RaycastHit hit;
        if (Physics.Raycast(from, Vector3.down, out hit, 2000f, LayerMask.GetMask("Junction"), QueryTriggerInteraction.Ignore)) 
        {
            centroid = hit.point;
        }
        else
        {
            Debug.Log("Centroid Y finder failed");
        }

    return centroid;
}

}
