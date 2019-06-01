using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseCellInfo : MonoBehaviour {

    public float area;
    public Vector3 centroid;
    public Vector3 pointOnRoad;
  //  public List<Vector3> pointsOnEdge = new List<Vector3>();
    public List<EdgeHelpers.Edge> boundaryPath;
    public Vector3 forwardDir;
    public float fenceIndentation = 0f; //build fence around edge
    public float bushesIndentation = 1f;
    //  public Vector3[] mVertices;
    void Awake()
    {
        //this.enabled = false;
    }
	
	void Start ()
    {
        
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<int> edgeInts = FindEdges.EdgeVertices(mesh, 0.01f);
        Vector3[] meshVerts = mesh.vertices;
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < edgeInts.Count; i++)
        {
            vertices.Add(meshVerts[i]);
        }


        centroid = FindCentralPointByAverage(vertices.ToArray()) + transform.position;
        
       // PlaceHouse(); //old

        // transform.FindChild("ProcHouse(Clone)").FindChild("Meshes").GetComponent<StretchQuads>().enabled = true;
        //for garden centre?

        if (transform.name == "GardenCentreBuildingPlot")
        {
            //set building typ
            //transform.FindChild("ProcHouse(Clone)").FindChild("Meshes").GetComponent<AddFeaturesToHouse2>().gardenCentre = true;//using unique script buildgardencentre

           //find out if we need to build new meshes
            GameObject previousGardenCentre = GameObject.FindGameObjectWithTag("Code").GetComponent<NewWorld>().gardenCentre;

            if (previousGardenCentre == null)
            {
                transform.Find("ProcHouse(Clone)").Find("Meshes").GetComponent<StretchQuads>().enabled = true;
                //save this house
                previousGardenCentre = transform.Find("ProcHouse(Clone)").gameObject;
            }
            else
            //if a a garden centre has been built 
            {
                //move the saved centre to this new spot
                previousGardenCentre.transform.Find("Meshes").transform.position = centroid;

                //rotate so the front door faces the road. As long as the garden centre size which is given to the voronoi mesh pattern is the same, it should fit in the space
                Vector3 door = previousGardenCentre.transform.Find("Meshes").Find("Foyer").Find("Door").transform.position;

                RaycastHit hit;
                Vector3 centre = centroid;
                Physics.SphereCast(centre + (Vector3.up * 200), 100f, Vector3.down, out hit, 200f, LayerMask.GetMask("Road"));

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = hit.point;

                Vector3 road = hit.point;
                //save to public var
                pointOnRoad = road;

                float distance = Mathf.Infinity;
                Quaternion closestRot = Quaternion.identity;

                GameObject meshes = previousGardenCentre.transform.Find("Meshes").gameObject;

                for (int i = 0; i < 360; i++)
                {
                    door = previousGardenCentre.transform.Find("Meshes").Find("Foyer").Find("Door").transform.position;
                    float temp = Vector3.Distance(road, door);
                    if (temp < distance)
                    {
                        distance = temp;
                        closestRot = meshes.transform.rotation;
                    }

                    //spin round and measure on next loop
                    meshes.transform.rotation = Quaternion.Euler(0, i, 0);
                }

                //move back to the closest position
                meshes.transform.rotation = closestRot;


            }
        }
        else //all other buildings atm
            transform.Find("ProcHouse(Clone)").Find("Meshes").GetComponent<StretchQuads>().enabled = true;

    }


    void PlaceHouse()
    {

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

        //raycast in circle getting larger until a cell which is not this one is hit
        StretchQuads sq = transform.GetComponentInChildren<StretchQuads>();
        float brickSize = sq.brickSize;

        RaycastHit hit;
        Physics.SphereCast(centroid + (Vector3.up * 50), 20f, Vector3.down, out hit, 100f, LayerMask.GetMask("TerrainCell"));

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       cube.transform.position = hit.point;
       cube.name = "Z";

        float limit1 = Vector3.Distance(centroid, hit.point) / brickSize;
   //     limit1 *= 0.66f;
        limit1 = Mathf.Round(limit1);
        
    //    Debug.Log(limit1);
        

        //we have found one limit. 
        //now we need to find the limit at right angle to this
        //take the direction from the centroid to the limit point we have found, rotate it 90 degrees, and check for edge of cells using raycast
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


                      GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      cube2.transform.position = hitL.point;
                      cube2.name = "X";
            }


            if (Physics.Raycast(centroid + (dirR * i) + Vector3.up, Vector3.down, out hitR, 10f, LayerMask.GetMask("TerrainCell")))
            {
                hasHitTerrainCell = true;
                limit2pos = hitR.point;


                      GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      cube2.transform.position = hitR.point;
                      cube2.name = "X";
            }

            i +=brickSize;
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

        //   bricks = bricks / 5;
        //    bricks *= 4;

        float limit2 = bricks * 2;

        //limit x to 2/3 of z
        //        limit2 = Mathf.Clamp(limit2, limit1, limit1 * 0.66f);

        //     GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //     cube2.transform.position = limit2pos;
        //     cube2.name = "X";

        // now we have our two limits, assign the short one to Z on the procHouse, and the longer one on X (sometimes switch it?)
        //this means the roof will be built with the spine along the longest axis

        //    float length1 = Vector3.Distance(centroid, hit.point);
        //    float length2 = Vector3.Distance(centroid, limit2pos);

        if (limit2 > limit1)
        {
         //   sq.targetSizeX = limit2;
         //   sq.targetSizeZ = limit1;
        }
        else
        {
         //   sq.targetSizeX = limit1;
         //   sq.targetSizeZ = limit2;
        }
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

    public static Vector3 FindCentralPointByAverage(Vector3[] mVertices)
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
        Vector3 from = new Vector3(centroid.x, 1000f, centroid.z);// - transform.position;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        cube.transform.position = from;
        cube.name = "FROM";
        RaycastHit hit;
        if (Physics.Raycast(from, Vector3.down, out hit, 2000f, LayerMask.GetMask("HouseCell"), QueryTriggerInteraction.Ignore)) 
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
