using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiChunk : MonoBehaviour
{
    public List<Vector3> tri = new List<Vector3>();
    public List<Vector3> tri2 = new List<Vector3>();
    public float radius = 1f;

    public bool drawInsideTri;
    public bool drawOutsideTri;

    public bool corners;

   
    // Use this for initialization

    void Start()
    {

     //   Scale();

        AddMeshGenerator();

        Border();


    }

    void Scale()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            verts[i] *= 5f;
            verts[i].y = 0;
        }

        mesh.vertices = verts;
    }
  
    void Realign()
    {

        //makes the transform position the centre of the mesh and moves the mesh vertices so the stay the same in world space
        Mesh mesh = GetComponent<MeshFilter>().mesh;


        transform.position = mesh.vertices[0];

        Vector3[] verts = mesh.vertices;
        List<Vector3> vertsList = new List<Vector3>();

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 point = verts[i] - transform.position;
            //    point.y = 0;
            vertsList.Add(point);
        }



        mesh.vertices = vertsList.ToArray();
        mesh.RecalculateBounds();



    }

    void AddMeshGenerator()
    { 


        MeshGenerator mg = gameObject.AddComponent<MeshGenerator>();
        mg.fillWithPoints = true;
        mg.renderCells = true;
        mg.useSortedGeneration = false;
        mg.test = true;
        mg.volume = new Vector3(1000.0f, 0.0f, 1000.0f);      
        mg.combineCells = true;
       
    }


    void Border()
    {
        //adds the shape of mesh to the voronoi mesh generator script
        //also fills the shape with points by making the shape smaller a few times

        FindEdges findEdges = GetComponent<FindEdges>();
        List<Vector3> pointsOnEdge = findEdges.pointsOnEdge;
        Vector3 centre = GetComponent<MeshFilter>().mesh.vertices[0];
        MeshGenerator mg = GetComponent<MeshGenerator>();
        Vector3 p = centre + transform.position;
        p.y = 0f;
        mg.yardPoints.Add(p);
        //place between intersection points
        for (int i = 0; i < pointsOnEdge.Count - 1; i++)
        {
            Vector3 p0 = pointsOnEdge[i];
            Vector3 p1 = pointsOnEdge[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (int j = 0; j < distance; j++)
            {
                Vector3 pos = p0 + (j * dir);

                //   GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube1.transform.position = pos;


                //  mg.yardPoints.Add(pos + transform.position);

                //add extended border

                //direction away from centre mesh point
                // Vector3 dirTowardsCentre = (centre - pos).normalized;
                //   dirTowardsCentre *= Vector3.Distance(pos, centre);
                float divisionAmount = 3f;
                float fraction = 1f/divisionAmount;
                for (float k = fraction; k <= 1; k += fraction)
                {
                    Vector3 halfWay = Vector3.Lerp(centre + transform.position, pos + transform.position, k);
                    //outline point
                    halfWay.y = 0f;
                    mg.yardPoints.Add(halfWay);
                    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube1.transform.position = halfWay;
                    cube1.transform.localScale *= 0.01f;
                }

                //populate inside
                //mg.yardPoints.Add(pos + dirTowardsCentre * 2 + transform.position);
                //    mg.yardPoints.Add(pos + dirAway * 3 + transform.position);
                //     mg.yardPoints.Add(pos + dirAway * 4 + transform.position);
            }
        }
    }

    public void JoinBorder()
    {
        //called from mesh gen when finished. 
        StartCoroutine("WaitForCombinedMeshToAddFindEdges");


    }
    IEnumerator WaitForCombinedMeshToAddFindEdges()
    {

        // yield return new WaitForEndOfFrame();

        GameObject combinedVoronoi = transform.GetChild(transform.childCount - 1).gameObject;
        // find edges added added automatically from combine children > auto weld > find edges
        while (combinedVoronoi.GetComponent<FindEdges>() == null)
        {
            yield return new WaitForEndOfFrame();
        }

        //create a border of points round the edge of the chunk
        List<Vector3> outerEdgeOptions = OuterEdgeOptions();

        //create a list of edge points for inner voronoi mesh
        List<Vector3> innerEdgePoints = InnerEdgePoints();

        //the voronoi mesh is created at y.zero because mesh gen script ony works in 2d
        //add Y adjust
        combinedVoronoi.AddComponent<MeshCollider>();
        combinedVoronoi.AddComponent<CellYAdjust>();

        //find suitable outer points
        CreateTriangles(innerEdgePoints, outerEdgeOptions);
    }

    List<Vector3> InnerEdgePoints()
    {
        List<Vector3> innerEdgePoints = new List<Vector3>();
        //create points round the edge of voronoi mesh

        //List<EdgeHelpers.Edge> boundaryPath;

        //the voronoi mesh that is created is always the last child. // The mesh from the parent is the second last
        GameObject combinedVoronoi = transform.GetChild(transform.childCount - 1).gameObject;

        // Mesh mesh = combinedVoronoi.GetComponent<MeshFilter>().mesh;

        //boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges();


        List<Vector3> pointsOnEdge = combinedVoronoi.GetComponent<FindEdges>().pointsOnEdge;

        //place between intersection points
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {

          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = pointsOnEdge[i] + transform.position;
          // cube.transform.localScale *= 0.1f;
          //  cube.name = ("inner");
          //  cube.transform.parent = combinedVoronoi.transform;

            innerEdgePoints.Add(pointsOnEdge[i] + transform.position);

            //  edgeOptions.Add(pos + transform.position);
        }
        return innerEdgePoints;
    }

    List<Vector3> OuterEdgeOptions()
    {
        //***********need to put in corners and not let them be removed************

        //create points round the edge of the border for options for the inner voronoi mesh to crate triangles from
        List<Vector3> edgeOptions = new List<Vector3>();
        List<EdgeHelpers.Edge> boundaryPath;

        //the voronoi mesh that is created is always the last child. // The mesh from the parent is the second last
        //Mesh mesh = transform.GetChild(transform.childCount - 2).GetComponent<MeshFilter>().mesh;
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges();

        FindEdges findEdges = GetComponent<FindEdges>();
        List<Vector3> pointsOnEdge = findEdges.pointsOnEdge;

        //place between intersection points
        for (int i = 0; i < pointsOnEdge.Count - 1; i++)
        {
            Vector3 p0 = pointsOnEdge[i];
            Vector3 p1 = pointsOnEdge[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (float j = 0; j < distance; j += 0.25f)
            {
                Vector3 pos = p0 + (j * dir);
                Vector3 zeroY = pos + transform.position;
                zeroY.y = 0f;

            //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.transform.position = zeroY;
            //    cube.transform.localScale *= 0.1f;

                edgeOptions.Add(zeroY);
            }
        }



       
        return edgeOptions;
    }

    void CreateTriangles(List<Vector3> innerEdgePoints, List<Vector3> outerEdgeOptions)
    {
        //Uses the two list given and creates a zigzagging triangle pattern
        //create list for corners script to use

     List<Vector3> usedOuterPoints = new List<Vector3>();

        for (int i = 0; i < innerEdgePoints.Count-1;i++)
        {
            //first inner triangle

            float distance = Mathf.Infinity;
            int closest = 0;
            
            for(int j = 0; j < outerEdgeOptions.Count; j++)
            {
                //half way bewtween innder edge points
                Vector3 midPoint = Vector3.Lerp(innerEdgePoints[i], innerEdgePoints[i + 1], 0.5f);
                
          //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //      cube.transform.position = midPoint;
          //      cube.transform.localScale *= 0.1f;
          //      cube.name = "mid";

                float temp = Vector3.Distance(midPoint, outerEdgeOptions[j]);
                
                if(temp < distance)
                {
                    distance = temp;
                    closest = j;
                }                
            }

            tri.Add(innerEdgePoints[i]);
            tri.Add(innerEdgePoints[i+1]);
            tri.Add(outerEdgeOptions[closest]);

            //add to a list of used outers, used by corners function
           
            usedOuterPoints.Add(outerEdgeOptions[closest]);

            //second outer triangle

            distance = Mathf.Infinity;
            int closestForNext = 0;

            for (int j = 0; j < outerEdgeOptions.Count; j++)
            {
                //to create a loop, the last point is the first point
                Vector3 secondPoint = innerEdgePoints[0];
                //second point will change to below, if not the last one
                if(i < innerEdgePoints.Count - 2)
                    secondPoint = innerEdgePoints[i + 2];

                //half way bewtween innder edge points
                Vector3 midPoint = Vector3.Lerp(innerEdgePoints[i+1], secondPoint, 0.5f);

                //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //      cube.transform.position = midPoint;
                //      cube.transform.localScale *= 0.1f;
                //      cube.name = "mid";

                float temp = Vector3.Distance(midPoint, outerEdgeOptions[j]);

                if (temp < distance)
                {
                    distance = temp;
                    closestForNext = j;
                }
            }

            tri2.Add(outerEdgeOptions[closest]);
            tri2.Add(outerEdgeOptions[closestForNext]);
            tri2.Add(innerEdgePoints[i + 1]);
        }

        Corners(usedOuterPoints);
    }
    
    void Corners(List<Vector3> usedOuterPoints)
    {


        //check to see if there are twoo unconnected points in the corner, or one
        //original basic mesh outline has all the corners
        FindEdges findEdges = GetComponent<FindEdges>();
        List<Vector3> pointsOnEdge = new List<Vector3>();
        pointsOnEdge = findEdges.pointsOnEdge;


        //remove duplicate at the ned of findEdges

        pointsOnEdge.RemoveAt(pointsOnEdge.Count - 1);
        //zero points
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {
            Vector3 temp = pointsOnEdge[i];
            temp.y = 0f;
            pointsOnEdge[i] = temp;
        }

        //if another 'corner' is closer than an outside edgePoint being used by a triangle
        for( int i = 0; i < pointsOnEdge.Count; i++)
        {
            float distanceToClosestEdgePoint = Mathf.Infinity;
            int closestEdgeVertice = 0;

            //look through other corners and find the closest one
            for (int j = 0; j < pointsOnEdge.Count; j++)
            {
                //don't check our our own point
                if (i == j)
                    continue;

                float temp = Vector3.Distance(pointsOnEdge[i], pointsOnEdge[j]);

                if (temp < distanceToClosestEdgePoint)
                {
                    distanceToClosestEdgePoint = temp;
                    closestEdgeVertice = j;
                }
            }

            //find the closest point which a triangle is using
            float distanceToClosestTriPoint = Mathf.Infinity;
            int closestTriVertice = 0;

            //look through used tri points and find the closest one
            for (int j = 0; j < usedOuterPoints.Count; j++)
            {
                float temp = Vector3.Distance(pointsOnEdge[i], usedOuterPoints[j]);

                if (temp < distanceToClosestEdgePoint)
                {
                    distanceToClosestTriPoint = temp;
                    closestTriVertice = j;
                }
            }

            bool twoPointCorner; //is it possible to have a three?            
            //if a corner point is closer than a triangle point, we have to corner points to consider
            if(distanceToClosestEdgePoint < distanceToClosestTriPoint )
            {
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = pointsOnEdge[i] + transform.position;

                twoPointCorner = true;
            }


        }        
    }
  

    void Update()
    {
       

        if (drawInsideTri)
        {
            for (int i = 0; i < tri.Count - 2; i += 3)
            {

                Debug.DrawLine(tri[i], tri[i + 1], Color.red);
                Debug.DrawLine(tri[i], tri[i + 2], Color.red);
                Debug.DrawLine(tri[i + 1], tri[i + 2], Color.red);

            }
        }
        if (drawOutsideTri)
        {
            for (int i = 0; i < tri2.Count - 2; i += 3)
            {

                Debug.DrawLine(tri2[i], tri2[i + 1], Color.blue);
                Debug.DrawLine(tri2[i], tri2[i + 2], Color.blue);
                Debug.DrawLine(tri2[i + 1], tri2[i + 2], Color.blue);

            }
        }
    }
}

