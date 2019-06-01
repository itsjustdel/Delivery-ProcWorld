using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BushesForCell : MonoBehaviour
{

    public List<Vector3> positionList = new List<Vector3>();
    List<string> typeList = new List<string>();
    BuildList buildList;
    //Places the Bush prefab around edges of the cell, doesn't if it hits the road
    private GameObject bush;//
    private GameObject shrub;//
    private GameObject tree;
    public bool yard = false;
    public List<EdgeHelpers.Edge> boundaryPath;
    public List<Vector3> pointsOnEdge = new List<Vector3>();
    public List<Vector3> fencePoints = new List<Vector3>();
    public List<GameObject> bushesToBatch = new List<GameObject>();
    

    public float pathSize = 1f;
    void Start()
    {
        //get path sizes
        if (gameObject.tag == "HouseCell")
            pathSize = GetComponent<HouseCellInfo>().bushesIndentation;

        if (gameObject.tag == "Field")
            pathSize = GetComponent<FieldManager>().bushesIndentation;


        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //use the EdgeHelpers static class to work out the boundary could also be called in BushesForCell?

        boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges(); //needs smoothing//being called in find edges too?

        shrub = Resources.Load("Prefabs/Flora/PottedPlant") as GameObject;
        bush = (GameObject)Resources.Load("Prefabs/Flora/Objects/Bush", typeof(GameObject));
        tree = (GameObject)Resources.Load("Prefabs/Flora/Objects/Tree", typeof(GameObject));
        buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        //StartCoroutine("PlaceBushes");
        //	StartCoroutine("PlaceTrees");
        //	StartCoroutine("EnableFlora");

        if (yard)
        {
            //   PlaceBushes();
        }
        else
        {

            //Fence();///old
        }

        //StartCoroutine("CreateListOfEdgePoints");     //find edges does this already
        pointsOnEdge = GetComponent<FindEdges>().pointsOnEdge;
        StartCoroutine("ShrubsIntersect");

      //  gameObject.AddComponent<FenceAroundCell>();

    }

    IEnumerator CreateListOfEdgePoints()
    {
        //creates a list of points round the edge of a field/cell with no duplicates or poitns within one unit of each other
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //centre point on renderer
        //Used to find out which way the bushes should move towards so that they leave a space around the edge of the field
        Vector3 centrePoint = GetComponent<MeshRenderer>().bounds.center;
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
                 //GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // cube2.name = "FenceCube";
                // cube2.transform.position = v3;
        }
        StartCoroutine("ShrubsIntersect");
        yield break;
    }   //done in bushes intersect
    void CreateBezierFromPoints()
    {
        BezierSpline bezierSpline = gameObject.AddComponent<BezierSpline>();
        List<Vector3> pointsForBezier = new List<Vector3>();
        List<BezierControlPointMode> splineControlPointList = new List<BezierControlPointMode>();
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {
            pointsForBezier.Add(pointsOnEdge[i]);
            splineControlPointList.Add(BezierControlPointMode.Free);
            //don't do on last one
            if (i < pointsOnEdge.Count - 1)
            {
                Vector3 controlPointPosition1 = Vector3.Lerp(pointsOnEdge[i], pointsOnEdge[i + 1], 0.25f);
                Vector3 controlPointPosition2 = Vector3.Lerp(pointsOnEdge[i], pointsOnEdge[i + 1], 0.75f);

                pointsForBezier.Add(controlPointPosition1);
                splineControlPointList.Add(BezierControlPointMode.Free);
                pointsForBezier.Add(controlPointPosition2);
                splineControlPointList.Add(BezierControlPointMode.Free);
            }
            else
            {
                //pointsForBezier.Add(pointsOnEdge[i]);
                //splineControlPointList.Add(BezierControlPointMode.Free);
            }
        }

        bezierSpline.points = pointsForBezier.ToArray();
        bezierSpline.modes = splineControlPointList.ToArray();

        float frequency = 1000;
        float stepSize = frequency;
        stepSize = 1f / (stepSize);

        for (int i = 0; i < frequency; i++)
        {
            Vector3 pos = bezierSpline.GetPoint(i * stepSize);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = pos;
            cube.name = "CurveCube";
            cube.transform.parent = transform;
        }

    }
    IEnumerator BushesIntersect()
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>();
        //create vectors inside polygon
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {
            Vector3 p0 = pointsOnEdge[i];
            //points on edge is [1] here to create a vector to intersect with from the last point to the second point
            //this gives us the final intersect point at the first vector
            Vector3 p1 = pointsOnEdge[1];

            if (i != pointsOnEdge.Count - 1)
                p1 = pointsOnEdge[i + 1];


            Vector3 dir = p1 - p0;

            //always builds clockwise so we only need to rotate right(to the inside of the polygon)
            Vector3 normal = (Quaternion.Euler(0f, 90f, 0f) * dir);
            normal.Normalize();
            normal *= pathSize;

            //move points inside
            p0 += normal;
            p1 += normal;

            //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     cube.transform.position = p0;

            //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube1.transform.position = p1;


            points.Add(p0);
            directions.Add(dir);


        }

        //get intersection points

        List<Vector3> intersectionPoints = new List<Vector3>();
        //check for intersections and add to list
        for (int i = 0; i < points.Count; i++)
        {


            //miss the last point
            if (i < points.Count - 1)
            {
                Vector3 closestP1;
                Vector3 closestP2;
                ClosestPointsOnTwoLines(out closestP1, out closestP2, points[i], directions[i], points[i + 1], directions[i + 1]);

                //we only need to use the second
                intersectionPoints.Add(closestP1);

                //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //  cube.transform.position = closestP1;
            }
        }

        //place between intersection points
        for (int i = 0; i < intersectionPoints.Count - 1; i++)
        {
            Vector3 p0 = intersectionPoints[i];
            Vector3 p1 = intersectionPoints[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (int j = 0; j < distance; j++)
            {
                Vector3 pos = p0 + (j * dir);

                //  GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube1.transform.position = pos;

                //add to a list for building
                positionList.Add(pos);
                typeList.Add("Bush");

            }

        }
        //complete the loop

        Vector3 last = intersectionPoints[intersectionPoints.Count - 1];
        Vector3 first = intersectionPoints[0];
        float distanceLast = Vector3.Distance(first, last);
        Vector3 dirLast = (first - last).normalized;

        for (int j = 0; j < distanceLast; j++)
        {
            Vector3 pos = last + (j * dirLast);

            //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube1.transform.position = pos;

            positionList.Add(pos);
            typeList.Add("Bush");
        }

        StartCoroutine("BuildList");
        yield break;
    }
    IEnumerator ShrubsIntersect()
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>();
        //create vectors inside polygon
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {
            Vector3 p0 = pointsOnEdge[i];
            //points on edge is [1] here to create a vector to intersect with from the last point to the second point
            //this gives us the final intersect point at the first vector
            Vector3 p1 = pointsOnEdge[1];

            if (i != pointsOnEdge.Count - 1)
                p1 = pointsOnEdge[i + 1];


            Vector3 dir = p1 - p0;

            //always builds clockwise so we only need to rotate right(to the inside of the polygon)
            Vector3 normal = (Quaternion.Euler(0f, 90f, 0f) * dir);
            normal.Normalize();
            normal *= pathSize;

            //move points inside
            p0 += normal;
            p1 += normal;

            //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     cube.transform.position = p0;

            //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube1.transform.position = p1;


            points.Add(p0);
            directions.Add(dir);


        }


      //  yield return new WaitForEndOfFrame();
        //get intersection points

        List<Vector3> intersectionPoints = new List<Vector3>();
        //check for intersections and add to list
        for (int i = 0; i < points.Count; i++)
        {


            //miss the last point
            if (i < points.Count - 1)
            {
                Vector3 closestP1;
                Vector3 closestP2;
                ClosestPointsOnTwoLines(out closestP1, out closestP2, points[i], directions[i], points[i + 1], directions[i + 1]);

                //we only need to use the second
    
                //cover for bug
                if(closestP1 != Vector3.zero)
                    intersectionPoints.Add(closestP1);

                //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //  cube.transform.position = closestP1;
            }

          //  if(i % 10 == 0)
          //      yield return new WaitForEndOfFrame();
        }

     //   yield return new WaitForEndOfFrame();

        //place between intersection points
        for (int i = 0; i < intersectionPoints.Count - 1; i++)
        {
            Vector3 p0 = intersectionPoints[i];
            Vector3 p1 = intersectionPoints[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (int j = 0; j < distance; j++)
            {
                Vector3 pos = p0 + (j * dir);

                //  GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube1.transform.position = pos;

                //add to a list for building
                positionList.Add(pos + transform.position);
                typeList.Add("Shrub");

            }
         //   if (i % 10 == 0)
         //       yield return new WaitForEndOfFrame();
        }
        //complete the loop
     //   yield return new WaitForEndOfFrame();

        Vector3 last = intersectionPoints[intersectionPoints.Count - 1];
        Vector3 first = intersectionPoints[0];
        float distanceLast = Vector3.Distance(first, last);
        Vector3 dirLast = (first - last);

        //testing to stop overlap at end
        if(distanceLast>0)
            dirLast.Normalize();

        for (int j = 0; j < distanceLast; j++)
        {
            Vector3 pos = last + (j * dirLast);

            //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube1.transform.position = pos;

            positionList.Add(pos + transform.position);
            typeList.Add("Shrub");
        }

        StartCoroutine("BuildList");
        yield break;
    }
   
    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
    //to each other. This function finds those two points. If the lines are not parallel, the function 
    //outputs true, otherwise false.
    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else {
            return false;
        }
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else {
            return 0f;
        }
    }

    void PlaceBushesOnEdge()
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //centre point on renderer
        //Used to find out which way the bushes should move towards so that they leave a space around the edge of the field
        Vector3 centrePoint = GetComponent<MeshRenderer>().bounds.center;
        //use temporary array
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < boundaryPath.Count; i++)
        {

            Vector3 p1 = vertices[boundaryPath[i].v1];
            Vector3 p2 = vertices[boundaryPath[i].v2];

            //move the border in edges in from the corners a little to stop bushes overlapping in corners
            Vector3 directionToCentre1 = centrePoint - p1;
            Vector3 directionToCentre2 = centrePoint - p2;
            //make it a unit vector - 1 will probably be enough. -sholuld it be pathWidth?
            directionToCentre1.Normalize();
            directionToCentre2.Normalize();

            p1 -= directionToCentre1;
            p2 -= directionToCentre2;

            //measures the distance between the two points in the edge array
            float distance = Vector3.Distance(p1, p2);
            //skip if tiny edge
            //    if (distance < 1)
            //        continue;


            //  working - can overlap if triangular // A wayto fix this is work out how the angle between this and next edge, 
            //shorten if acute, lengthen distance if obtuse

            //move in from edge a little to allow space for path etc
            //find half way point from edge
            Vector3 halfWay = Vector3.Lerp(p2, p1, 0.5f);
            //direction vector to this point
            Vector3 directionToHalfWay = halfWay - p2;
            directionToHalfWay.Normalize();
            directionToHalfWay *= 2; //pathWidth 
            //rotate this vector
            Vector3 directionToHalfWayL = Quaternion.Euler(0f, 90f, 0f) * directionToHalfWay;
            Vector3 directionToHalfWayR = Quaternion.Euler(0f, -90f, 0f) * directionToHalfWay;
            //add this to half way point
            Vector3 halfWayL = halfWay + directionToHalfWayL;
            Vector3 halfWayR = halfWay + directionToHalfWayR;
            //this has created to point each side of the halfway point
            //Now measure which one is closest to the mesh renderer's centre point
            float distanceL = Vector3.Distance(centrePoint, halfWayL);
            float distanceR = Vector3.Distance(centrePoint, halfWayR);

            Vector3 directionToMiddle = Vector3.zero;
            if (distanceL < distanceR)
            {
                directionToMiddle = directionToHalfWayL;
                //              Debug.Log("left");
            }
            else if (distanceR < distanceL)
            {
                directionToMiddle = directionToHalfWayR;
                //               Debug.Log("right");
            }
            else
                Debug.Log("Bushes Direction To Middle Did Not Work");

            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = halfWay + (directionToMiddle*0.5f);
            cube.name = "Halfway";
            cube.transform.parent = transform;
            */

            float random = Random.Range(1f, 5f);

            for (float j = 0; j < distance; j++)//was random, overshooting?
            {
                //do not place in the corners
                if (j < 2 || j > (distance - 2))//pathsize
                    continue;

                Vector3 dir = p2 - p1;
                dir.Normalize();
                //   float randomDir = Random.Range(0.8f, 1.2f);
                //  dir *= randomDir;               

                Vector3 pos = p1 + (dir * j) + directionToMiddle;
                positionList.Add(pos);
                typeList.Add("Bush");
            }




        }
    }
    


    void PathOnEdge(List<EdgeHelpers.Edge> path)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < path.Count; i++)
        {

            List<Vector3> pathVertices = new List<Vector3>();
            List<int> pathTriangles = new List<int>();

            Vector3 p1 = vertices[path[i].v1];
            Vector3 p2 = vertices[path[i].v2];


            float distance = Vector3.Distance(p1, p2);
            int pathWidth = 2;
            for (float j = 0; j <= distance; j++) //plus here grid size  e.g j+gridscale
            {
                for (float k = 0; k < 2; k++)//plus here grid size  e.g j+gridscale
                {
                    ////  if (j > distance - 2 || j < 2)//skip last part to stop overlap
                    ////      continue;

                    Vector3 dirZ = p2 - p1;
                    dirZ.Normalize();

                    Vector3 dirX = Quaternion.Euler(0f, 90f, 0f) * dirZ;//this might need to point to middle instead of just right(fields)

                    Vector3 pos = p1 + (dirZ * j) + (dirX * k);
                    Vector3 pos2 = p1 + (dirZ * (j + 1)) + (dirX * k);

                    Vector3 pos3 = p1 + (dirZ * j) + (dirX * (k + 1));
                    Vector3 pos4 = p1 + (dirZ * (j + 1)) + (dirX * (k + 1));

                    //we can add some of of these points for the fence fucntin to create the posts from
                    if (k == pathWidth - 1)//ony add the most interior row
                    {
                        //we can get duplicates because of the way the mesh is welded (i think)
                        //so check if value is different from the previous. If it different, put it in

                        //do not check on the first value
                        if (fencePoints.Count > 4)
                        {
                            if (Vector3.Distance(fencePoints[fencePoints.Count - 1], pos3) > 0.5f)
                            {
                                fencePoints.Add(pos3);
                            }
                            else
                            {
                                Debug.Log("removing");
                                //just get rid of this one- bad egg
                                fencePoints.RemoveAt(fencePoints.Count - 1);
                                fencePoints.RemoveAt(fencePoints.Count - 2);
                                fencePoints.RemoveAt(fencePoints.Count - 3);
                                fencePoints.RemoveAt(fencePoints.Count - 4);
                            }
                        }
                        else if (fencePoints.Count == 0)
                            fencePoints.Add(pos3);
                    }
                    //add a quad's worth of vertices
                    pathVertices.Add(pos);
                    pathVertices.Add(pos2);
                    pathVertices.Add(pos3);
                    pathVertices.Add(pos4);


                    /*grrr
                    //add triangles for the quad we jsut added for loop below is +=4
                    int index = (int)((j*4)+(k*4));
                    pathTriangles.Add(index + 0);
                    pathTriangles.Add(index + 1);
                    pathTriangles.Add(index + 2);

                    pathTriangles.Add(index + 3);
                    pathTriangles.Add(index + 2);
                    pathTriangles.Add(index + 1);
                    */
                }


            }

            for (int t = 0; t < pathVertices.Count - 2; t += 4)
            {
                pathTriangles.Add(t + 0);
                pathTriangles.Add(t + 1);
                pathTriangles.Add(t + 2);

                pathTriangles.Add(t + 3);
                pathTriangles.Add(t + 2);
                pathTriangles.Add(t + 1);
            }


            Mesh pathMesh = new Mesh();
            pathMesh.vertices = pathVertices.ToArray();
            pathMesh.triangles = pathTriangles.ToArray();

            GameObject pathway = new GameObject();
            pathway.transform.parent = transform;
            MeshFilter meshFilter = pathway.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = pathway.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load("Path", typeof(Material)) as Material;

            meshFilter.mesh = pathMesh;
        }


    }


    void PlaceTrees()
    {
        GameObject tree = (GameObject)Resources.Load("Prefabs/Flora/Objects/Tree", typeof(GameObject));
        //grab the non subdiveded mesh from the polygon tester which saves the simple mesh for us to use
        Mesh mesh = gameObject.GetComponent<PolygonTester>().originalMesh;
        List<Vector3> list = new List<Vector3>();

        for (int i = 0; i < mesh.vertexCount - 1; i++)
        {
            float distance = 0f;
            if (i != 0)
            {
                distance = Vector3.Distance(mesh.vertices[i], mesh.vertices[i + 1]);
            }
            else if (i == 0)
            {
                distance = Vector3.Distance(mesh.vertices[i + 1], mesh.vertices[mesh.vertexCount - 1]);
            }
            //Debug.Log(distance);
            bool hitRoad = false;
            List<Vector3> tempList = new List<Vector3>();

            float random = Random.Range(1f, 20f);
            for (float j = 0; j < distance; j += random) //j+number is how often they are placed (randomise this)//TODO
            {

                if (i == 0)
                {
                    Vector3 dir = mesh.vertices[i + 1] - mesh.vertices[mesh.vertexCount - 1];
                    dir.Normalize();
                    float randomDir = Random.Range(0.8f, 1.2f);
                    dir *= randomDir;

                    Vector3 pos = mesh.vertices[mesh.vertexCount - 1] + (dir * j);


                    //check for raycast
                    RaycastHit hit;
                    LayerMask lM = LayerMask.GetMask("Road", "House", "Flora");


                    //if it doesnt hit a road or a building, add
                    if (!Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out hit, 20f, lM))
                    {
                        tempList.Add(pos);
                    }
                    else
                    {
                        //if it hits a road
                        if (hit.transform.gameObject.layer == 9)
                        {
                            tempList.Clear();
                            hitRoad = true;
                        }
                        //if it hits a building or another bush/tree
                        if (hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 27)
                        {
                            //just skip over it// do nothing
                        }
                    }

                }

                else if (i != 0)
                {


                    Vector3 dir = mesh.vertices[i + 1] - mesh.vertices[i];
                    dir.Normalize();
                    float randomDir = Random.Range(0.8f, 1.2f);
                    dir *= randomDir;
                    Vector3 pos = mesh.vertices[i] + (dir * j);

                    //check for raycast
                    RaycastHit hit;
                    LayerMask lM = LayerMask.GetMask("Road", "House", "Flora");


                    //if it doesnt hit a road, add
                    if (!Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out hit, 20f, lM))
                    {
                        tempList.Add(pos);
                        //	continue;
                    }
                    //	if (Physics.Raycast(pos + Vector3.up*1000,Vector3.down, out hit,2000f,lM))
                    else
                    {
                        //if it hits a road
                        if (hit.transform.gameObject.layer == 9)
                        {
                            tempList.Clear();
                            hitRoad = true;
                        }
                        //if it hits a building or a bush/tree
                        if (hit.transform.gameObject.layer == 10 || hit.transform.gameObject.layer == 27)
                        {
                            // Debug.Log("hit building or tree - not placing bush");
                            //just skip over it// do nothing
                        }
                    }
                }

                //yield return new WaitForFixedUpdate();
            }

            if (!hitRoad)
            {
                if (!hitRoad)
                {
                    //Clear the list and start on new edge
                    for (int l = 0; l < tempList.Count; l++)
                    {
                        positionList.Add(tempList[l]);
                        typeList.Add("Tree");
                    }
                }
            }
            if (hitRoad)
            {
                //this has been done in "Polygon Tester"
                //enable subdivision script 
                //		gameObject.GetComponent<SubdivideMesh>().enabled = true;
                //		Destroy(gameObject.GetComponent<CellYAdjust>());
                //		
                //		GameObject.Find("VoronoiMesh").GetComponent<BuildController>().gOList.Add(this.gameObject);

            }
            tempList.Clear();

        }

        /*

        in buildlist now

        //Instantiate the bushes at the points inserted in list

        for (int i = 0; i < list.Count; i++)
        {
            GameObject treeInstance = Instantiate(tree, list[i], Quaternion.identity) as GameObject;
            //	treeInstance.GetComponent<PlaceFlora>().enabled = true;

            //parent them to the cell
            treeInstance.transform.parent = this.transform;

            //	yield return new WaitForFixedUpdate();
        }
        */

        //StartCoroutine("EnableFlora");
        // yield break;
    }

    IEnumerator EnableFlora()
    {
        PlaceFlora[] flora = gameObject.GetComponentsInChildren<PlaceFlora>();
        //yield return new WaitForSeconds(1);
        for (int i = 0; i < flora.Length; i++)
        {
            PlaceFlora pf = flora[i].GetComponent<PlaceFlora>();
            pf.enabled = true;
            if (pf.gameObject.name == "Bush(Clone)")
            {
                pf.isTree = false;
                //add collider
                // yield return new WaitForEndOfFrame();
                //   pf.transform.GetChild(2).gameObject.AddComponent<SphereCollider>();
            }
            else if (pf.gameObject.name == "Tree(Clone)")
                pf.isTree = true;

            //  AddCollider(pf.gameObject);

            //another one for procedural rose bush to build
            //yield return new WaitForEndOfFrame();
        }
        //   StartCoroutine("AddColliders");
        yield break;
    }

    void AddCollider(GameObject go)
    {
        if (go.gameObject.name == "Bush(Clone)")
        {

        }
    }

    IEnumerator AddColliders()
    {
        //put children in to an array
        int childCount = transform.childCount;
        GameObject[] children = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        //the third child in each gameobject is the bush's green mesh. add a collider to this
        for (int i = 0; i < childCount; i++)
        {

            if (children[i].name == "Bush(Clone)")
            {
                GameObject child = children[i].transform.GetChild(2).gameObject;
                child.gameObject.AddComponent<SphereCollider>();
                //flora layer
                child.gameObject.layer = 27;
            }

            if (children[i].name == "Tree(Clone)")
            {
                GameObject child = children[i].transform.GetChild(1).gameObject;
                child.gameObject.AddComponent<BoxCollider>();
                //flora layer
                child.gameObject.layer = 27;
            }


            //   yield return new WaitForEndOfFrame();
        }
        yield break;

    }

    IEnumerator BuildList()
    {
        //

        GameObject batched = new GameObject();
        batched.transform.parent = transform;
        batched.name = "Batched";

        for (int i = 0; i < positionList.Count; i++)
        {

           
            if (typeList[i] == "Bush")
            {
                //  yield return new WaitForEndOfFrame();

                //raycast to check for any other bushes
                RaycastHit hit;
                LayerMask lM = LayerMask.GetMask("Road", "House", "Flora");
                //if it hits anything, jump to next 
                if (Physics.Raycast(positionList[i] + Vector3.up * 10, Vector3.down, out hit, 20f, lM))
                {
                    continue;
                }

                GameObject bushInstance = Instantiate(bush, positionList[i], Quaternion.identity) as GameObject;
                //	bushInstance.GetComponent<PlaceFlora>().enabled = true;

                //parent them to the cell
                bushInstance.transform.parent = this.transform;

                //enable flora
                bushInstance.GetComponent<PlaceFlora>().isTree = false;

                //moved to main build list in 
                bushInstance.GetComponent<PlaceFlora>().enabled = true; //don't enable here//main build list
                                                                        // buildList.Add(bushInstance);

                //   yield return new WaitForEndOfFrame(); //buildlist waits for frame
            }

            if (typeList[i] == "Tree")
            {
                //    yield return new WaitForEndOfFrame();

                //raycast to check for any other bushes
                RaycastHit hit;
                LayerMask lM = LayerMask.GetMask("Road", "House", "Flora");
                //if it hits anything, jump to next 
                if (Physics.Raycast(positionList[i] + Vector3.up * 10, Vector3.down, out hit, 20f, lM))
                {
                    continue;
                }

                GameObject treeInstance = Instantiate(tree, positionList[i], Quaternion.identity) as GameObject;
                //	bushInstance.GetComponent<PlaceFlora>().enabled = true;

                //parent them to the cell
                treeInstance.transform.parent = this.transform;

                //enable flora
                treeInstance.GetComponent<PlaceFlora>().isTree = true;
                treeInstance.GetComponent<PlaceFlora>().enabled = true; //moved to main build list
                                                                        //  buildList.Add(treeInstance);
                                                                        //  yield return new WaitForEndOfFrame();//build list waits for frame
            }

            if (typeList[i] == "Shrub")
            {
                //  yield return new WaitForEndOfFrame();

                //raycast to check for any other bushes
         //       RaycastHit hit;
         //       LayerMask lM = LayerMask.GetMask("Road", "House", "Flora");
                //if it hits anything, jump to next 
                /*
                if (Physics.Raycast(positionList[i] + Vector3.up * 10, Vector3.down, out hit, 20f, lM))
                {
                    Debug.Log("skipping");
                    continue;       //necessary?
                }
                */
               // GameObject shrubInstance = Instantiate(shrub, positionList[i], Quaternion.identity) as GameObject;
                //	bushInstance.GetComponent<PlaceFlora>().enabled = true;


                GameObject shrub = new GameObject();
                shrub.transform.parent = batched.transform;
                shrub.transform.position = positionList[i];
                shrub.name = "Shrub";
                shrub.tag = "Flora";
                BoxCollider box = shrub.AddComponent<BoxCollider>();
                box.isTrigger = true;


                //parent them to the cell
               // shrubInstance.transform.parent = this.transform;

                //enable flora
               // shrubInstance.GetComponent<PlantController>().enabled = true;
                /*
                BuildList.BuildObject bo = new BuildList.BuildObject();
                bo.gameObject = shrubInstance;
                bo.type = "Shrub";
                buildList.buildObjectList.Add(bo);                
                */

                if(i% 10 == 0 && i != 0)
                {
                    batched = new GameObject();
                    batched.transform.parent = transform;
                    batched.name = "Batched";
                }
            }

            if (i % 100 == 0)
                yield return new WaitForEndOfFrame();  
        }

        //report to build list
        BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        buildList.BuildingFinished();

        yield break;
    }

}
