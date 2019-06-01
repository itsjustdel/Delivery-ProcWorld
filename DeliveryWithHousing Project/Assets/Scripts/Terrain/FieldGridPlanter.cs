using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGridPlanter : MonoBehaviour {

    //triggered by player, checks if inside it's field's bounds - if so, plants crops.trees etc
    public int inside = 0;
    public List<Vector3> points = new List<Vector3>();

    // Use this for initialization
    void Start ()
    {
        RayCastForHeight();
        
        //check if all corners are inside field's polygon
        List<Vector2> polyPoints = new List<Vector2>();
        List<Vector3> edgePoints = transform.parent.GetComponent<FindEdges>().pointsOnEdge;
        //convert to 2d
        foreach(Vector3 v3 in edgePoints)        
            polyPoints.Add( new Vector2(v3.x, v3.z));

        Vector2[] polyArray = polyPoints.ToArray();

        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v3 = transform.rotation * (new Vector3(vertices[i].x*transform.localScale.x, vertices[i].y * transform.localScale.y, vertices[i].z * transform.localScale.z));
            v3 += transform.position;

            Vector2 v2 = new Vector2(v3.x, v3.z );

            if (ContainsPoint(polyArray, v2))
                inside++;
        }
        if (inside < 4)//if all outside
        {
            //tell building pipeline we are finished and get rid of this quad


            //Destroy and do not wait for frame
            GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished(true, true);
            

        }
        else if (inside == 4 )//if all inside
        {
            points = GridInsidePoly();
            
            //StartCoroutine("PlantFromArrayInBatches");
            //PlantFromArrayInBatches();
            PlantFromArrayDirectly();
        }
        //if 0 destroy

        // if > 1 send to build list then just raycast grid
        else
        {
            //BuildList buildList = GameObject.FindWithTag("Code").GetComponent<BuildList>();
            //buildList.components.Add(this);
        }
    }

    void PlantFromArrayInBatches()
    {
        int batchSize = 10;//how many plants in one mesh
        int batchCount = 0;

        //make a game oject with a collider and batch script - when the player triggers this object, it will plant the plants
        GameObject batchParent = new GameObject();
        batchParent.transform.position = points[0 + batchSize / 2];
        batchParent.name = "Batch Parent";
        //for trigger to activate
        batchParent.tag = "Flora";

        batchParent.AddComponent<BatchPlanter>();//enabled is false on Awake()
        BoxCollider bc = batchParent.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        List<BatchPlanter> planterList = new List<BatchPlanter>();

        planterList.Add(batchParent.GetComponent<BatchPlanter>());

        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                //check if we start a new row
                float distanceToNext = Vector3.Distance(points[i], points[i + 1]);

                if (batchCount == batchSize)// || distanceToNext > 2)
                {
                    //make new parent
                    batchParent = new GameObject();

                    //plant in centre of batch unless we are at the end of the points array
                    int middleOfBatch = batchSize / 2;
                    //check if we near the end
                    if (i + middleOfBatch >= points.Count)
                        //middle of what's left
                        middleOfBatch = (int)((points.Count - i) / 2);

                    batchParent.transform.position = points[i + middleOfBatch];
                    batchParent.name = "Batch Parent";
                    batchParent.tag = "Flora";
                    batchParent.AddComponent<BatchPlanter>();
                    bc = batchParent.AddComponent<BoxCollider>();
                    bc.isTrigger = true;

                    batchCount = 0;

                    planterList.Add(batchParent.GetComponent<BatchPlanter>());
                }
            }

            //place individual game objects 
            GameObject plant = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Cube);/
            plant.name = "Plant Single";
            plant.transform.position = points[i];
            plant.transform.parent = batchParent.transform;


            batchCount++;

            //if (i % 500 == 0)
            //    yield return new WaitForEndOfFrame();
        }

        //fir allat once - builds whooe grid in one go
        //foreach (BatchPlanter bp in planterList)
         //   bp.Start();

        //tell building pipeline we are finished
        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
        transform.name = "Quad worked";
        //yield break;
    }
    void PlantFromArrayDirectly()
    {
        int batchSize = 100;//how many plants in one mesh
        int batchCount = 0;

        //make a game oject with a collider and batch script - when the player triggers this object, it will plant the plants
        GameObject batchParent = new GameObject();
        batchParent.transform.position = points[0 + batchSize / 2];
        batchParent.name = "Batch Parent";
        //for trigger to activate
        batchParent.tag = "Flora";

        batchParent.AddComponent<BatchPlanter>();//enabled is false on Awake()
        BoxCollider bc = batchParent.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        //List<BatchPlanter> planterList = new List<BatchPlanter>();

        //planterList.Add(batchParent.GetComponent<BatchPlanter>());
        BranchArray branchArray = GameObject.FindWithTag("Code").GetComponent<BranchArray>();
        List<GameObject> parents = new List<GameObject>();
        parents.Add(batchParent);
        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                //check if we start a new row
                float distanceToNext = Vector3.Distance(points[i], points[i + 1]);

                if (batchCount == batchSize)// || distanceToNext > 2)
                {
                    //make new parent
                    batchParent = new GameObject();

                    //plant in centre of batch unless we are at the end of the points array
                    int middleOfBatch = batchSize / 2;
                    //check if we near the end
                    if (i + middleOfBatch >= points.Count)
                        //middle of what's left
                        middleOfBatch = (int)((points.Count - i) / 2);

                    batchParent.transform.position = points[i + middleOfBatch];
                    batchParent.name = "Batch Parent";
                    batchParent.tag = "Flora";
                    batchParent.AddComponent<BatchPlanter>();
                    bc = batchParent.AddComponent<BoxCollider>();
                    bc.isTrigger = true;

                    batchCount = 0;

                    parents.Add(batchParent);

                   // planterList.Add(batchParent.GetComponent<BatchPlanter>());
                }
            }

            //place individual game objects 
            GameObject plant = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Cube);/
            plant.name = "Plant Single";
            plant.transform.position = points[i];
            plant.transform.parent = batchParent.transform;


            batchCount++;

            //if (i % 500 == 0)
            //    yield return new WaitForEndOfFrame();
        }

        //plant in placed objects
        foreach (GameObject parent in parents)            
            parent.GetComponent<BatchPlanter>().enabled = true;

        //tell building pipeline we are finished
        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
        transform.name = "Quad worked";
        //yield break;
    }
    
    void RayCastForHeight()
    {
        Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
        //now raycast to on each corner to find y height
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = transform.position + transform.rotation*(vertices[i]*transform.localScale.x);//x and z scale the same
            pos += Vector3.up * 1000;
           
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 2000f, LayerMask.GetMask("TerrainBase")))
            {
                vertices[i] = hit.point - transform.position;

                

            }
        }

        //we used world cor-ords from hit.point so we can reset quad
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.localScale = Vector3.one;

        gameObject.GetComponent<MeshFilter>().mesh.vertices = vertices;
        gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();        
    }

    List<Vector3> GridInsidePoly()
    {
        List<Vector3> ps = new List<Vector3>();
        //test a grid from bounds.extents and clip pints that land outside field's polyline(edge)
        //create 2d array from bounds
        List<Vector2> grid = new List<Vector2>();
        float xSize = GetComponent<MeshRenderer>().bounds.size.x;
        float zSize = GetComponent<MeshRenderer>().bounds.size.z;
        Vector3 startV3 = GetComponent<MeshRenderer>().bounds.center - ((zSize * .5f) * Vector3.forward)-((xSize * .5f) * Vector3.right); ;

        int gap = 1;
        
        for (int i = 0; i < xSize; i+=gap)
        {
            for (int j = 0; j < zSize-1; j+=gap)
            {
                
                Vector3 v= startV3 + Vector3.right * i + Vector3.forward * j;
                v += Vector3.right * 0.5f + Vector3.forward*0.5f;//remember this is *1 if wanted to change gap


                //find Y height by checking quad plane height at this point
                Vector3 intersection;
                Vector3 normal = gameObject.GetComponent<MeshFilter>().mesh.normals[0];//two triangles in wuad both face exactly same way
                if (LinePlaneIntersection(out intersection, v + Vector3.up * 500, Vector3.down, normal, gameObject.GetComponent<MeshRenderer>().bounds.center))
                {
                    ///GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //c.transform.position = intersection;

                    v = intersection;
                }
                else
                    Debug.Log("No interesect");


                ps.Add(v);
            }
        }

        return ps;
    }

    //Get the intersection between a line and a plane. 
    //If the line and plane are not parallel, the function outputs true, otherwise false.
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {

        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = SetVectorLength(lineVec, length);

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    //create a vector of direction "vector" with length "size"
    public static Vector3 SetVectorLength(Vector3 vector, float size)
    {

        //normalize the vector
        Vector3 vectorNormalized = Vector3.Normalize(vector);

        //scale the vector
        return vectorNormalized *= size;
    }

    static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        int j = polyPoints.Length - 1;
        bool inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            if (((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) &&
               (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
                inside = !inside;
        }
        return inside;
    }

}
