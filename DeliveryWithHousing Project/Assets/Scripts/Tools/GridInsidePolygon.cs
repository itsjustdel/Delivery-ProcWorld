using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GridInsidePolygon : MonoBehaviour
{
    public List<Vector3> tri = new List<Vector3>();
    public List<Vector3> tri2 = new List<Vector3>();
    
    public List<Vector3> forGizmo = new List<Vector3>();
    public bool drawInsideTriangles;
    public bool drawOutsideTriangles;
    public bool drawCornersWith1Point;

    private Mesh skirtMesh;// = new Mesh();
    private Mesh gridMesh;// = new Mesh();


    //This script creates a grid inside a polygon, and then creates a bordering skirt between the grid and the outline of the polygon
     

    void Start()
    {
        StartCoroutine("GridAlongSpline");
    }

    IEnumerator GridRayCast()
    {

        //change this filed's layer to Field Test. This will be th eonly object in the scene with this layer
        int initialLayer = gameObject.layer;
        gameObject.layer = 29;

        float gap = 1f;

        //raycast in parallel lines, move mesh points hit upwards to create bumps

        Vector3 startPoint = GetComponent<MeshRenderer>().bounds.center;// - GetComponent<MeshRenderer>().bounds.extents;
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = startPoint;
        Vector3 topCorner = GetComponent<MeshRenderer>().bounds.max;
        Vector3 bottomCorner = GetComponent<MeshRenderer>().bounds.min;

        //grab edge points
        List<Vector3> edgePointsOriginal = GetComponent<FindEdges>().pointsOnEdge;
        //create list of evenly spaced border points
        List<Vector3> edgePoints = new List<Vector3>();

        for (int i = 0; i < edgePointsOriginal.Count - 1; i++)
        {
            Vector3 dir = (edgePointsOriginal[i + 1] - edgePointsOriginal[i]).normalized;
            float distance = Vector3.Distance(edgePointsOriginal[i + 1], edgePointsOriginal[i]);

            for (float j = 0; j < distance; j += 1f)
            {
                Vector3 p = edgePointsOriginal[i] + dir * j;
                edgePoints.Add(p);
                
            }
        }



        //   float size = Vector3.Distance(startPoint, topCorner);
        float size = Vector3.Distance(bottomCorner, topCorner);// 200; //could do while inifite loop. safer this way

        //final list
        List<Vector3> points = new List<Vector3>();

        for (int d = 0; d < 4; d++)
        {
            Vector3 dir = Vector3.zero;
            Vector3 sideDir = Vector3.zero;
            if (d == 0)
            {
                dir = Vector3.forward;
                sideDir = Quaternion.Euler(0, 90, 0) * dir;
            }
            else if (d == 1)
            {
                dir = Vector3.forward;
                sideDir = Quaternion.Euler(0, -90, 0) * dir;
            }
            else if (d == 2)
            {
                dir = Vector3.back;
                sideDir = Quaternion.Euler(0, 90, 0) * dir;
            }
            else if (d == 3)
            {
                dir = Vector3.back;
                sideDir = Quaternion.Euler(0, -90, 0) * dir;
            }

            // Vector3 sideDir = Quaternion.Euler(0, 90, 0) * dir;
            int startIndexI = 0;

            //stop overlaps by starting a little later
            if (d == 2 || d == 3)
                startIndexI++;

            //  int hitsPrevious = 0;

            

            for (int i = startIndexI; i < size; i += 2)
            {

                //bottleneck protection
                if (i % 20 == 0)
                    yield return new WaitForEndOfFrame();

                List<Vector3> thisRow = new List<Vector3>();

                float startIndexJ = 0;
                //stop overlaps by starting a little later  //working?
                if (d == 1 || d == 3)
                    startIndexJ += 1f;
                bool startedRow = false;
                bool overShot = false;
                bool addedLastChunk = false;
                Vector3 previousHitPoint = Vector3.zero;
                List<Vector3> provisionalPoints = new List<Vector3>();

                for (float j = startIndexJ; j < size;)// j += 1f)
                {
                    //cast a set of rays in a circle downwards, better performance than spherecast, and is all we need for this task
                    //looking for any other cell that isnt ours. Only adding points if only hitting our own cell

                    bool add = false;

                    Vector3 hitPoint = Vector3.zero;
                    List<RaycastHit> hits = new List<RaycastHit>();
                    for (float r = 0f; r < 360f; r += 360f)
                    {
                        Vector3 direction = Quaternion.Euler(0f, r, 0f) * Vector3.left;
                        Vector3 shootFrom = startPoint + (dir * i) + (sideDir * j) + direction;


                        RaycastHit hit;
                        if (Physics.Raycast(shootFrom + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("FieldTest")))
                        {


                            hitPoint = hit.point;



                            add = true;
                            /*
                            //if any hit hits another field, do not add hit
                            if (hit.transform.tag == "Field" && hit.transform != transform)
                            {

                                add = false;
                            }

                            //if it hits a terrain cell( the cells which over lap the road, do not add
                            if (hit.transform.tag == "Cell")
                            {
                                add = false;
                            }
                            */
                        }




                    }

                    if (add)
                    {
                        startedRow = true;

                        if (!overShot)
                        {
                            //we have not overshot

                            //dont do first one since we add points post second ray
                            if (previousHitPoint == Vector3.zero)
                            {
                                previousHitPoint = hitPoint;
                                //    yield return new WaitForEndOfFrame();
                                //    if (j == 2)
                                //       Debug.Log("previous hit point not being assigned");
                                continue;
                            }

                            //add the list filled from the previous jump//
                            foreach (Vector3 v3 in provisionalPoints)
                                points.Add(v3);

                            //clear list
                            provisionalPoints = new List<Vector3>();

                            //if we have not overshot,m we ahve succesfully jumped 10 steps in the field. Lerp these points in.
                            //This is so we save on 10 raycasts

                            //  float start = 0f;
                            for (float k = 0; k <= 10; k++)
                            {

                                //Vector3 projectedPosition = startPoint + (dir * i) + (sideDir * (j + k));
                                //lerp works form 0 to 1
                                float lerpAmount = k / 10f;
                                Vector3 projectedPosition = Vector3.Lerp(previousHitPoint, hitPoint, lerpAmount);

                                bool tooClose = false;
                                //check of point is too close to an edge point
                                foreach (Vector3 v3 in edgePoints)
                                {
                                    if (Vector3.Distance(projectedPosition, v3) < gap)
                                        tooClose = true;
                                }

                                if (!tooClose)
                                {
                                    //randomise slightly
                                    projectedPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));
                                    provisionalPoints.Add(projectedPosition);
                                }
                            }
                            //push it on a bit
                            j += 10f;
                            // Debug.Log(j);
                            // yield return new WaitForEndOfFrame();
                        }
                        else if (overShot)
                        {

                            if (!addedLastChunk)
                            {

                                //addedLastChunk = true;
                            }

                            //add points on the way to the field edge

                            Vector3 point = startPoint + (dir * i) + (sideDir * j);
                            point = hitPoint;


                            bool tooClose = false;
                            //check of point is too close to an edge point
                            foreach (Vector3 v3 in edgePoints)
                            {
                                if (Vector3.Distance(point, v3) < gap)
                                    tooClose = true;
                            }

                            if (!tooClose)
                            {
                                //randomise slightly
                                point += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));

                                points.Add(point);
                            }

                            //edge our way to the edge 
                            j++;
                            // yield return new WaitForEndOfFrame();
                        }
                        //   hitsPrevious++;
                    }
                    else if (!add)
                    {
                        //if have not started row yet, keep looking
                        if (!startedRow)
                        {
                            /*
                            //if we are on the last point in the row, we ahve found the end of the field, jump out of current direction
                            if (j >= size)
                            {
                                //force stop this whole row
                                i = (int)size;

                                continue;
                            }
                          *///  else if (j < size)
                            //  {
                            j++;

                            //  }
                        }
                        else if (startedRow)
                        {
                            //we have shot past the edge of the field//reverse
                            //addd our last chunk of points
                            //add the list filled from the previous jump
                            foreach (Vector3 p in provisionalPoints)
                            {
                                Vector3 point = p;

                                bool tooClose = false;
                                //check of point is too close to an edge point
                                foreach (Vector3 v3 in edgePoints)
                                {
                                    if (Vector3.Distance(point, v3) < gap)
                                        tooClose = true;
                                }

                                if (!tooClose)
                                {
                                    //randomise slightly
                                    point += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));

                                    points.Add(point);
                                }
                            }



                            //clear list
                            provisionalPoints = new List<Vector3>();

                            //move 11 back,  and baby step, j++
                            if (!overShot)
                            {
                                overShot = true;

                                j -= 11f; //10f +1
                                          // yield return new WaitForEndOfFrame();
                            }
                            //if we have already overshot and baby stepped back to the edge of the field
                            else if (overShot)
                            {
                                //we have found the end. stop, move on to next row.
                                break;
                            }
                        }
                    }
                    //used to lerp on next hit
                    previousHitPoint = hitPoint;
                }
            }
        }

        // BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        // buildList.BuildingFinished();

        //reset layer now we have finished. We can do this because we only build one field at a time in the scene
        gameObject.layer = initialLayer;

        foreach(Vector3 v3 in points)
        {
            forGizmo.Add(v3);
        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.position = v3;
        }

        yield break;

    }    
    IEnumerator Grid()
    {
        
        //change this filed's layer to Field Test. This will be th eonly object in the scene with this layer
        int initialLayer = gameObject.layer;
        gameObject.layer = 29;

        float gap = 1f;

        //raycast in parallel lines, move mesh points hit upwards to create bumps

        Vector3 startPoint = GetComponent<MeshRenderer>().bounds.center;// - GetComponent<MeshRenderer>().bounds.extents;
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = startPoint;
        Vector3 topCorner = GetComponent<MeshRenderer>().bounds.max;
        Vector3 bottomCorner = GetComponent<MeshRenderer>().bounds.min;

        //grab edge points
        List<int> edgePointsOriginalInts = FindEdges.EdgeVertices(GetComponent<MeshFilter>().mesh,0f);
        //create v3
        List<Vector3> edgePointsOriginal = new List<Vector3>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i < edgePointsOriginalInts.Count;i++)
        {
            edgePointsOriginal.Add(vertices[edgePointsOriginalInts[i]]);
        }

        List<Vector2> edge2d = new List<Vector2>();
        for (int i = 0; i < edgePointsOriginal.Count;i++)
        {
            edge2d.Add(new Vector2(edgePointsOriginal[i].x, edgePointsOriginal[i].z));    

        }

        Vector2[] edge2dArray = edge2d.ToArray();

        foreach (Vector3 v3 in edgePointsOriginal)
        {

           // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
           // cube.transform.position = v3;
        }

     

        //   float size = Vector3.Distance(startPoint, topCorner);
        float size = Vector3.Distance(bottomCorner, topCorner);// 200; //could do while inifite loop. safer this way

        //final list
        List<Vector3> points = new List<Vector3>();

        Vector3 dir = Vector3.forward;
        Vector3 sideDir = Quaternion.Euler(0, 90, 0) * dir;

        //  int hitsPrevious = 0;

        //list of quads position, inside a list of rows,
        List<List<List<Vector3>>> quadList = new List<List<List<Vector3>>>();

        for (int i = 0; i < size; i ++)
        {
            //bottleneck protection
            if (i % 20 == 0)
                yield return new WaitForEndOfFrame();

            for (float j = 0f; j < size; j++)// j += 1f)
            {
                List<List<Vector3>> rowList = new List<List<Vector3>>();

                Vector3 hitPoint = Vector3.zero;
                List<RaycastHit> hits = new List<RaycastHit>();
                for (float r = 0f; r < 360f; r += 360f)
                {
                    Vector3 direction = Quaternion.Euler(0f, r, 0f) * Vector3.left;
                    Vector3 shootFrom = bottomCorner + (dir * i) + (sideDir * j) + direction;

                 
                    // RaycastHit hit;
                    //    if (Physics.Raycast(shootFrom + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("FieldTest")))
                    //    {
                    Vector2 point2d = new Vector2(shootFrom.x, shootFrom.z);

                    //check if quad is inside

                    
                    if (PointInPolygon.IsPointInPolygon(point2d,edge2dArray))
                    {
                        Vector3 nextPointX = shootFrom + dir;
                        Vector2 nextPoint2dX = new Vector2(nextPointX.x, nextPointX.z);
                        if (PointInPolygon.IsPointInPolygon(nextPoint2dX, edge2dArray))
                        {
                            //now check above
                            Vector3 nextPointY = shootFrom + sideDir;
                            Vector2 nextPoint2dY = new Vector2(nextPointY.x, nextPointY.z);
                            if (PointInPolygon.IsPointInPolygon(nextPoint2dY, edge2dArray))
                            {
                                //now check far corner
                                Vector3 pointXandY = shootFrom + dir + sideDir;
                                Vector2 point2dXandY = new Vector2(pointXandY.x, pointXandY.z);
                                if (PointInPolygon.IsPointInPolygon(point2dXandY, edge2dArray))
                                {
                                    //if we get here,we have found space for a quad
                                    //enter quad position in to a list and add to master list for mesh
                                    List<Vector3> quadPositions = new List<Vector3>();
                                    quadPositions.Add(shootFrom);
                                    quadPositions.Add(nextPointX);
                                    quadPositions.Add(nextPointY);
                                    quadPositions.Add(pointXandY);
                                    //add to row list
                                    rowList.Add(quadPositions);
                                }
                            }
                        }


                    points.Add(shootFrom);
                    }
                }
                //once row has been completed, add the row list to master list
                quadList.Add(rowList);

            }                  
        }

        //
        /*
        for(int i = 0; i < quadList.Count;i++)
        {
            GameObject group = new GameObject();
            group.transform.parent = transform;

            List<Vector3> quads = quadList[i];
            //for each list in the list
            for(int j =0; j < quads.Count;j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = group.transform;
                cube.transform.position = quads[j];
                cube.transform.localScale *= 0.2f;
            }
        }
        */
        //create mesh
        List<Vector3> verticesForNewMesh = new List<Vector3>();
        List<int> tris = new List<int>();
        for (int i = 0; i < quadList.Count; i++)
        {
            List<List<Vector3>> row = quadList[i];
            //for each list in the list
            //add the four quad points
            for (int j = 0; j < row.Count; j++)
            {
                List<Vector3> quads = row[j];
                for (int k = 0; k < quads.Count; k++)
                {                   
                    verticesForNewMesh.Add(quads[k]);
                }

                //add the tris
                int lastAdded = verticesForNewMesh.Count;

                //go back 4 vertices andd add triangle
                tris.Add(lastAdded - 4);
                tris.Add(lastAdded - 3);
                tris.Add(lastAdded - 2);
                //second tri
                tris.Add(lastAdded - 3);
                tris.Add(lastAdded - 1);
                tris.Add(lastAdded - 2);
            } 
            
            //split in to smaller meshes    
            /*
            if(tris.Count > 4000)
            {
                Mesh newMesh = new Mesh();
                newMesh.vertices = verticesForNewMesh.ToArray();
                newMesh.triangles = tris.ToArray();

                newMesh = AutoWeld.AutoWeldFunction(newMesh, 0.1f, 100);

                GameObject grid = new GameObject();
                grid.name = "Grid";
                grid.transform.parent = transform;
                MeshFilter mf = grid.AddComponent<MeshFilter>();
                mf.mesh = newMesh;

                MeshRenderer mr = grid.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load("GrassBillBoard") as Material;

                //clear lists
                verticesForNewMesh = new List<Vector3>();
                tris = new List<int>();
            } 
            */      
        }
  
        Mesh newMeshLast = new Mesh();
        newMeshLast.vertices = verticesForNewMesh.ToArray();
        newMeshLast.triangles = tris.ToArray();

        newMeshLast = AutoWeld.AutoWeldFunction(newMeshLast, 0.001f, 100);

        GameObject gridLast = new GameObject();
        gridLast.name = "Grid";
        gridLast.transform.parent = transform;
        MeshFilter mfLast = gridLast.AddComponent<MeshFilter>();
        mfLast.mesh = newMeshLast;

        MeshRenderer mrLast = gridLast.AddComponent<MeshRenderer>();
        mrLast.sharedMaterial = Resources.Load("GrassBillBoard") as Material;


        // BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        // buildList.BuildingFinished();

        //reset layer now we have finished. We can do this because we only build one field at a time in the scene
        gameObject.layer = initialLayer;

        foreach (Vector3 v3 in points)
        {
            forGizmo.Add(v3);
          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = v3;
        }


        //what mesh to pass? create daddy mesh ? --not splitting atm
        SkirtingMesh(newMeshLast,mesh);
        yield break;



    }
    IEnumerator GridAlongSpline()
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 startPoint = HouseCellInfo.FindCentralPointByAverage(vertices);
        //change to bounds,center?
        //GameObject sp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //sp.transform.position = startPoint;
        //sp.name = "SP";
        RaycastHit hit;
        GameObject road = null;
        if (Physics.Raycast(startPoint + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
        {
            road = hit.transform.gameObject;
        }
        else
        {
            Debug.Log("Ray Missed");
           // StartCoroutine("Grid");
            yield break;
        }

        BezierSpline spline = road.transform.parent.GetComponent<RingRoadMesh>().updatedSpline;

        //run through spline looking for closest point to centre of this mesh

        //mesh is built with shared vertices with 1 at each side of the road. This will need change if road mesh is gridded/more detail added.
        //RingRoadMesh defines the frequency used. Needs cleaned up --not dividing by 2 for higher accuracy
        float frequency = road.GetComponent<MeshFilter>().mesh.vertexCount;
        //one fraction of frequency to run through spline with
        float stepSize = 1f / frequency;

        int closest = 0;
        float distance = Mathf.Infinity;
        for (int i = 0; i < frequency; i++)
        {
            float temp = Vector3.Distance(spline.GetPoint(i * stepSize), startPoint);
            if (temp < distance)
            {
                distance = temp;
                closest = i;
            }
        }
       // int roadWidth = 5 * 2; //find longest edge and use this for distance

        float edgeDistance = longestEdgeDistance(mesh) -1;

        //TODO put check for small distance. Means there is an anomaly in the voronoi pattern
        //instead of gridding this seection. subdivide mesh using old meshHelper


        //float roadWidth = edgeDistance /2;
        //roadWidth += 1;
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        int xSize = 0;
        for (float i = closest - 2; i <= closest + 2; i++)
        {

            Vector3 point = spline.GetPoint(i * stepSize);


            Vector3 dir = spline.GetDirection(i * stepSize);

            //make gris size smaller if cell is smaller - removing
            /*
            float distanceNext = Vector3.Distance(spline.GetPoint(i * stepSize), spline.GetPoint((i + 1) * stepSize));
            //not sure if needed. just making sure
            dir = dir.normalized * distanceNext;
            */
            Vector3 sideDir = Quaternion.Euler(0, 90, 0) * dir;

          
            for (float j = -(edgeDistance/2); j <= (edgeDistance/2); j++)// j += 1f)
            {

                verticesList.Add(point + (sideDir * j));


              //  GameObject cubey = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  cubey.transform.position = point + (sideDir * j);
                

                if (i == closest-2)
                    xSize++;

            }
        }

        List<int> triangles = new List<int>();

        // int xSize = (roadWidth *2) -1;
        int ySize = 4;//4;//  for (float i = closest - 2; i <= closest + 2; i++) //how many lines above for loop makes//density var?
        for (int y = 0; y < ySize; y++)
        {
            //if (y > 1)
              //  break;
            for (int x = 0; x < xSize-1; x++)
            {
                //one ring attached to the next

  //              if (triangles.Count > 6)
//                    break;
                //start
                triangles.Add(x + (y * xSize));
                //next row
                triangles.Add(x + ((y+1) * (xSize)));
                //this row, next along
                triangles.Add((x+1) + (y *xSize));

                //this row, next along
                triangles.Add((x + 1) + (y * xSize));
                //next row
                triangles.Add(x + ((y + 1) * (xSize)));
                //next row, next alng
                triangles.Add((x+1) + ((y + 1) * (xSize)));

              

            }


        }

        Mesh newMeshLast = new Mesh();
        newMeshLast.vertices = verticesList.ToArray();
        newMeshLast.triangles = triangles.ToArray();// trianglesList.ToArray();
                                                    //used to combine
        gridMesh = newMeshLast;

        // newMeshLast = AutoWeld.AutoWeldFunction(newMeshLast, 0.1f, 100);
        /*
        GameObject gridLast = new GameObject();
        gridLast.name = "Grid";
        gridLast.transform.parent = transform;
        MeshFilter mfLast = gridLast.AddComponent<MeshFilter>();
        mfLast.mesh = newMeshLast;

        MeshRenderer mrLast = gridLast.AddComponent<MeshRenderer>();
        mrLast.sharedMaterial = Resources.Load("Green") as Material;
        */
        SkirtingMesh2(newMeshLast, mesh);
      
        

        foreach (Vector3 v3 in verticesList)
        {
         //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = v3;
        }

        yield break;
    }
    IEnumerator RotatedGrid()
    {

        //start in centre and create two sides, direction reversed for each side

        //find the longest edge, we can use this for our x direction. Alternatively we can find the two longest edges and vector3.rotateTowards
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        
        //dir= -dir;

        //change this filed's layer to Field Test. This will be th eonly object in the scene with this layer
        int initialLayer = gameObject.layer;
        gameObject.layer = 29;

        float gap = 1f;

        //raycast in parallel lines, move mesh points hit upwards to create bumps

        Vector3 startPoint = HouseCellInfo.FindCentralPointByAverage(vertices); //GetComponent<MeshRenderer>().bounds.center;// - GetComponent<MeshRenderer>().bounds.extents;
    //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    cube1.transform.position = startPoint;
        //find closest point on road bezier
        //find bezier through raycast :(
        RaycastHit hit;
        GameObject road = null;
        if (Physics.Raycast(startPoint + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
        {
            road = hit.transform.gameObject;
        }
        else
            Debug.Log("Ray Missed");

        BezierSpline spline = road.transform.parent.GetComponent<RingRoadMesh>().updatedSpline;

        //run through spline looking for closest point to centre of this mesh

        //mesh is built with shared vertices with 1 at each side of the road. This will need change if road mesh is gridded/more detail added.
        //RingRoadMesh defines the frequency used. Needs cleaned up --not dividing by 2 for higher accuracy
        float frequency = road.GetComponent<MeshFilter>().mesh.vertexCount ;
        //one fraction of frequency to run through spline with
        float stepSize = 1f / frequency;

        int closest = 0;
        float distance = Mathf.Infinity;
        for(int i = 0; i < frequency; i++)
        {
            float temp = Vector3.Distance( spline.GetPoint(i * stepSize), startPoint);
            if(temp < distance)
            {
                distance = temp;
                closest = i;
            }
        }
        
        int startIndex = closest - 2;
        int endIndex = closest + 2;

        for( float i = startIndex; i < endIndex-startIndex; i++)
        {
            //put below for loop here
            //use get direction for direction forward
            //euler spin for side direction
        }
        
        


        Vector3 bottomCorner = Vector3.zero;

        //grab edge points
        List<int> edgePointsOriginalInts = FindEdges.EdgeVertices(GetComponent<MeshFilter>().mesh, 0f);
        //create v3
        List<Vector3> edgePointsOriginal = new List<Vector3>();
        
        //prepare polygon in 2d to do PointInsidePolygon test
        for (int i = 0; i < edgePointsOriginalInts.Count; i++)
        {
            edgePointsOriginal.Add(vertices[edgePointsOriginalInts[i]]);
        }

        List<Vector2> edge2d = new List<Vector2>();
        for (int i = 0; i < edgePointsOriginal.Count; i++)
        {
            edge2d.Add(new Vector2(edgePointsOriginal[i].x, edgePointsOriginal[i].z));

        }

        Vector2[] edge2dArray = edge2d.ToArray();

        foreach (Vector3 v3 in edgePointsOriginal)
        {

            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // cube.transform.position = v3;
        }



        //   float size = Vector3.Distance(startPoint, topCorner);
        float size = 10f;//roadsize Vector3.Distance(bottomCorner, topCorner);// 200; //could do while inifite loop. safer this way

        //final list
        List<Vector3> points = new List<Vector3>();

        Vector3 dir = Vector3.forward;
        
        Vector3 sideDir = Quaternion.Euler(0, 90, 0) * dir;

        //  int hitsPrevious = 0;

        //list of quads position, inside a list of rows,
        List<List<List<Vector3>>> quadList = new List<List<List<Vector3>>>();

        for (float i = startIndex; i <  startIndex +( endIndex - startIndex); i++)
        {

            int roadWidth = 4*2;
            for (float j = -roadWidth/2f; j < roadWidth/2; j++)// j += 1f)
            {
                List<List<Vector3>> rowList = new List<List<Vector3>>();

                Vector3 hitPoint = Vector3.zero;
                List<RaycastHit> hits = new List<RaycastHit>();

                //support for sphere test etc- force skip atm
                for (float r = 0f; r < 360f; r += 360f)
                {
                    Vector3 spinDirection = Quaternion.Euler(0f, r, 0f) * Vector3.left; //not using
                    //Vector3 shootFrom = bottomCorner + (dir * i) + (sideDir * j) + spinDirection;
                    Vector3 splinePoint = spline.GetPoint(i * stepSize);
                    Vector3 splineDir = spline.GetDirection(i * stepSize);
                    Vector3 splineDirSpun = Quaternion.Euler(0, 90, 0) * splineDir;
                    Vector3 shootFrom = splinePoint + (splineDirSpun * j);

                 //   GameObject sf = GameObject.CreatePrimitive(PrimitiveType.Cube);
                 //   sf.transform.position = shootFrom;



                    // RaycastHit hit;
                    //    if (Physics.Raycast(shootFrom + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("FieldTest")))
                    //    {
                    Vector2 point2d = new Vector2(shootFrom.x, shootFrom.z);

                    //check if quad is inside


                    if (PointInPolygon.IsPointInPolygon(point2d, edge2dArray))
                    {
                        Vector3 nextPointX = shootFrom + splineDir;
                        Vector2 nextPoint2dX = new Vector2(nextPointX.x, nextPointX.z);
                        if (PointInPolygon.IsPointInPolygon(nextPoint2dX, edge2dArray))
                        {
                            //now check above
                            Vector3 nextPointY = shootFrom + splineDirSpun;
                            Vector2 nextPoint2dY = new Vector2(nextPointY.x, nextPointY.z);
                            if (PointInPolygon.IsPointInPolygon(nextPoint2dY, edge2dArray))
                            {
                                //now check far corner
                                Vector3 pointXandY = shootFrom + splineDir + splineDirSpun;
                                Vector2 point2dXandY = new Vector2(pointXandY.x, pointXandY.z);
                                if (PointInPolygon.IsPointInPolygon(point2dXandY, edge2dArray))
                                {
                                    //if we get here,we have found space for a quad
                                    //enter quad position in to a list and add to master list for mesh
                                    List<Vector3> quadPositions = new List<Vector3>();
                                    quadPositions.Add(shootFrom);
                                    quadPositions.Add(nextPointX);
                                    quadPositions.Add(nextPointY);
                                    quadPositions.Add(pointXandY);
                                    //add to row list
                                    rowList.Add(quadPositions);
                                }
                            }
                        }


                        points.Add(shootFrom);
                    }
                }
                //once row has been completed, add the row list to master list
                quadList.Add(rowList);

            }
        }

        //
        /*
        for(int i = 0; i < quadList.Count;i++)
        {
            GameObject group = new GameObject();
            group.transform.parent = transform;

            List<Vector3> quads = quadList[i];
            //for each list in the list
            for(int j =0; j < quads.Count;j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = group.transform;
                cube.transform.position = quads[j];
                cube.transform.localScale *= 0.2f;
            }
        }
        */
        //create mesh
        List<Vector3> verticesForNewMesh = new List<Vector3>();
        List<int> tris = new List<int>();
        for (int i = 0; i < quadList.Count; i++)
        {
            List<List<Vector3>> row = quadList[i];
            //for each list in the list
            //add the four quad points
            for (int j = 0; j < row.Count; j++)
            {
                List<Vector3> quads = row[j];
                for (int k = 0; k < quads.Count; k++)
                {
                    verticesForNewMesh.Add(quads[k]);
                }

                //add the tris
                int lastAdded = verticesForNewMesh.Count;

                //go back 4 vertices andd add triangle
                tris.Add(lastAdded - 4);
                tris.Add(lastAdded - 3);
                tris.Add(lastAdded - 2);
                //second tri
                tris.Add(lastAdded - 3);
                tris.Add(lastAdded - 1);
                tris.Add(lastAdded - 2);
            }

            //split in to smaller meshes    
            /*
            if(tris.Count > 4000)
            {
                Mesh newMesh = new Mesh();
                newMesh.vertices = verticesForNewMesh.ToArray();
                newMesh.triangles = tris.ToArray();

                newMesh = AutoWeld.AutoWeldFunction(newMesh, 0.1f, 100);

                GameObject grid = new GameObject();
                grid.name = "Grid";
                grid.transform.parent = transform;
                MeshFilter mf = grid.AddComponent<MeshFilter>();
                mf.mesh = newMesh;

                MeshRenderer mr = grid.AddComponent<MeshRenderer>();
                mr.sharedMaterial = Resources.Load("GrassBillBoard") as Material;

                //clear lists
                verticesForNewMesh = new List<Vector3>();
                tris = new List<int>();
            } 
            */
        }

        Mesh newMeshLast = new Mesh();
        newMeshLast.vertices = verticesForNewMesh.ToArray();
        newMeshLast.triangles = tris.ToArray();

       newMeshLast = AutoWeld.AutoWeldFunction(newMeshLast, 0.001f, 100);

        GameObject gridLast = new GameObject();
        gridLast.name = "Grid";
        gridLast.transform.parent = transform;
        MeshFilter mfLast = gridLast.AddComponent<MeshFilter>();
        mfLast.mesh = newMeshLast;

        MeshRenderer mrLast = gridLast.AddComponent<MeshRenderer>();
        mrLast.sharedMaterial = Resources.Load("Green") as Material;


        // BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        // buildList.BuildingFinished();

        //reset layer now we have finished. We can do this because we only build one field at a time in the scene
        gameObject.layer = initialLayer;

        foreach (Vector3 v3 in points)
        {
            forGizmo.Add(v3);
            //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //  cube.transform.position = v3;
        }


        //what mesh to pass? create daddy mesh ? --not splitting atm
       SkirtingMesh2(newMeshLast, mesh);

        
        yield break;



    }


    Vector3 longestEdgeDirection(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        //returns a list of edges
        List<EdgeHelpers.Edge> boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges();
        //go through these edges and find longest set(edges have two points,p1 and p2)

        int longest = 0;
        float longestDistance = 0;
        for(int i = 0; i < boundaryPath.Count; i++)
        {
            Vector3 p1 = vertices[boundaryPath[i].v1];
            Vector3 p2 = vertices[boundaryPath[i].v2];
            float temp = Vector3.Distance(p1, p2);
            if(temp > longestDistance)
            {
                longest = i;
                longestDistance = temp;
            }
        }

        //get direction of longest edge

        Vector3 l1 = vertices[boundaryPath[longest].v1];
        Vector3 l2 = vertices[boundaryPath[longest].v2];


        Vector3 dir = (l2 - l1).normalized;

        return dir;
    }

    float longestEdgeDistance(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        //returns a list of edges
        List<EdgeHelpers.Edge> boundaryPath = EdgeHelpers.GetEdges(mesh.triangles).FindBoundary().SortEdges();
        //go through these edges and find longest set(edges have two points,p1 and p2)

        int longest = 0;
        float longestDistance = 0;
        for (int i = 0; i < boundaryPath.Count; i++)
        {
            Vector3 p1 = vertices[boundaryPath[i].v1];
            Vector3 p2 = vertices[boundaryPath[i].v2];
            float temp = Vector3.Distance(p1, p2);
            if (temp > longestDistance)
            {
                longest = i;
                longestDistance = temp;
            }
        }
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = vertices[boundaryPath[longest].v1];

        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.position = vertices[boundaryPath[longest].v2];
        */
        return longestDistance;
    }

    void SkirtingMesh(Mesh inner, Mesh outer)
    {
        //create a border between the inner and out meshes

        outer = AutoWeld.AutoWeldFunction(outer,0.0001f, 100f);

        List<int> innerPoints = FindEdges.EdgeVertices(inner,0.1f);
        List<Vector3> innerV3 = new List<Vector3>();
        Vector3[] innerVertices = inner.vertices;
        for(int i = 0; i < innerPoints.Count; i++)
        {
            innerV3.Add(innerVertices[innerPoints[i]]);
            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = innerVertices[innerPoints[i]];
            cube.transform.localScale *= 0.1f;
            cube.name = "inner";
            */
        }
        //outer mesh points
        float threshHold = 0.01f;
        List<int> outerPoints = FindEdges.EdgeVertices(outer,threshHold);
        List<Vector3> outerV3 = new List<Vector3>();
        Vector3[] outerVertices = outer.vertices;
        for (int i = 0; i < outerPoints.Count; i++)
        {
            
                outerV3.Add(outerVertices[outerPoints[i]]);
            
           // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = outerVertices[outerPoints[i]];
          //  cube.transform.localScale *= 0.1f;
          //  cube.name = "outer";
            
        }
        //outer mesh points are far from each other, create a point every unit. the grid we have built is 1 unit 
        List<Vector3> outerPointsLerped = new List<Vector3>();
        
        for(int i = 0; i < outerV3.Count -1; i++)
        {
            Vector3 dir = (outerV3[i + 1] - outerV3[i]).normalized;
            float distance = Vector3.Distance(outerV3[i], outerV3[i + 1]);

            for(float j = 0; j < distance - 1f; j+=0.25f) //half a step gives enough options, dont place too close to next point
            {
                //move towards next point
                Vector3 point = outerV3[i] + (dir * j);
                outerPointsLerped.Add(point);
            }

            if(distance < 0.5f)
            {
                //the for loop will have been skipped. Add first point and move on
                outerPointsLerped.Add(outerV3[i]);
            }

            //on last one, add a closing loop
            if (i == outerV3.Count - 2)
            {
                Vector3 dirLast = (outerV3[0] - outerV3[i+1]).normalized;
                float distanceLast = Vector3.Distance(outerV3[0], outerV3[i+1]);
                for(int j = 0; j < distanceLast; j++)
                {
                    Vector3 pointLast = outerV3[i + 1] + (dirLast * j);
                    outerPointsLerped.Add(pointLast);
                }
            }
        }

        foreach(Vector3 v3 in outerPointsLerped)
        {
            //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //  cube.transform.position = v3;
            //  cube.transform.localScale *= 0.1f;
            //  cube.name = "outerLerped";
        }

        CreateTriangles(innerV3, outerPointsLerped,outerV3);

    }
    void SkirtingMesh2(Mesh inner, Mesh outer)
    {
        //create a border between the inner and out meshes

        outer = AutoWeld.AutoWeldFunction(outer, 0.0001f, 100f);//tiny threshold necessary

        List<int> innerPoints = FindEdges.EdgeVertices(inner, 0.01f);
        List<Vector3> innerV3 = new List<Vector3>();
        Vector3[] innerVertices = inner.vertices;
        for (int i = 0; i < innerPoints.Count; i++)
        {
            innerV3.Add(innerVertices[innerPoints[i]]);
            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = innerVertices[innerPoints[i]];
            cube.transform.localScale *= 0.1f;
            cube.name = "inner";
            */
        }
        //outer mesh points
        float threshHold = 0.001f;
        List<int> outerPoints = FindEdges.EdgeVertices(outer, threshHold);
        List<Vector3> outerV3 = new List<Vector3>();
        Vector3[] outerVertices = outer.vertices;
        for (int i = 0; i < outerPoints.Count; i++)
        {

            outerV3.Add(outerVertices[outerPoints[i]]);
            /*
             GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
              cube.transform.position = outerVertices[outerPoints[i]];
              cube.transform.localScale *= 0.1f;
              cube.name = "outer";
              */
        }
        //outer mesh points are far from each other, create a point every unit. the grid we have built is 1 unit 
        List<Vector3> outerPointsLerped = new List<Vector3>();
        //make loop for lerp
        outerV3.Add(outerV3[0]);

        for (int i = 0; i < outerV3.Count - 1; i++)
        {
            Vector3 dir = (outerV3[i + 1] - outerV3[i]).normalized;
            float distance = Vector3.Distance(outerV3[i], outerV3[i + 1]);

            for (float j = 0; j < distance ; j += 0.01f) //??what size. This governs how many options there are on the outside
            {
                //move towards next point
                Vector3 point = outerV3[i] + (dir * j);
                outerPointsLerped.Add(point);
            }

            if (distance < 0.5f)
            {
                //the for loop will have been skipped. Add first point and move on
                outerPointsLerped.Add(outerV3[i]);
            }

            //on last one, add a closing loop
            if (i == outerV3.Count - 2)
            {
                Vector3 dirLast = (outerV3[0] - outerV3[i + 1]).normalized;
                float distanceLast = Vector3.Distance(outerV3[0], outerV3[i + 1]);
                for (int j = 0; j < distanceLast; j++)
                {
                    Vector3 pointLast = outerV3[i + 1] + (dirLast * j);
                    outerPointsLerped.Add(pointLast);

                }
            }
        }

        foreach (Vector3 v3 in outerPointsLerped)
        {
       //       GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //     cube.transform.position = v3;
           //   cube.transform.localScale *= 0.1f;
             // cube.name = "outerLerped";
         
        }

        CreateTriangles(innerV3, outerPointsLerped, outerV3);
       // CreateTrianglesOuterFirst(outerPointsLerped, innerV3);
    }
    void CreateTriangles(List<Vector3> innerEdgePoints, List<Vector3> outerEdgeOptions,List<Vector3> outsideNonLerped)
    {
        //Uses the two list given and creates a zigzagging triangle pattern
        //create list for corners script to use
        Vector3 startPoint = GetComponent<MeshRenderer>().bounds.center;

        //make lists have same y point
        for(int i = 0; i <innerEdgePoints.Count;i++)
        {
            Vector3 temp = innerEdgePoints[i];
            temp.y = startPoint.y;
            innerEdgePoints[i] = temp;
        }
        for (int i = 0; i < outerEdgeOptions.Count; i++)
        {
            Vector3 temp = outerEdgeOptions[i];
            temp.y = startPoint.y;
            outerEdgeOptions[i] = temp;
        }
        for (int i = 0; i < outsideNonLerped.Count; i++)
        {
            Vector3 temp = outsideNonLerped[i];
            temp.y = startPoint.y;
            outsideNonLerped[i] = temp;
        }

        //inner

        List<Vector3> usedOuterPoints = new List<Vector3>();

        //create loop by adding first index to end
        innerEdgePoints.Add(innerEdgePoints[0]);
      
        for (int i = 0; i < innerEdgePoints.Count - 1; i++)
        {
            //first inner triangle

            float distance = Mathf.Infinity;
            int closest = 0;

            Vector3 center = Vector3.Lerp(innerEdgePoints[i], innerEdgePoints[i + 1], 0.5f);

            //move midpoint in to middle of triangle
            Vector3 dir = (innerEdgePoints[i + 1] - innerEdgePoints[i]).normalized;
            dir = Quaternion.Euler(0f, -90f, 0f) * dir;
            dir *= 0.5f; //half a step-all steps are units
            Vector3 midPoint = center + dir;

            //mid point needs to become the intersection point from dir and the edge we are looking for
            //find intersection points from all edges. compare. closet is the correct edge.
            //then use this point as "midpoint"
            List<Vector3> intersections = new List<Vector3>();
            //outsidenon lerped is out original mesh
            for (int j = 0; j < outsideNonLerped.Count - 1; j++)
            {
                //an edge is from one point to the next

                Vector3 p1 = outsideNonLerped[j];
                Vector3 p2 = outsideNonLerped[j + 1];

                Vector3 edgeVectorDir = p2 - p1;
                Vector3 midPointVectorDir = midPoint - center;

                Vector3 intersectPoint = Vector3.zero;
                Vector3 intersectPoint2 = Vector3.zero;
                //find where edge vector and midPoint rotated(above) intersect
                if (BushesForCell.ClosestPointsOnTwoLines(out intersectPoint, out intersectPoint2, p1, edgeVectorDir, center, midPointVectorDir))
                {
                    //we only need to use the second

                    if (intersectPoint != Vector3.zero)
                    {
                        

                        //add
                        intersections.Add(intersectPoint);
                    }
                }
            }

            //compare all edge intersection in intersections list and only use the closest point to "center"
            float distanceIntersect = Mathf.Infinity;
            Vector3 closestV3 = Vector3.zero;

            for (int j = 0; j < intersections.Count;j++)
            {
                float temp = Vector3.Distance(intersections[j], center);
                if (temp < distanceIntersect)
                {
                    distanceIntersect = temp;
                    closestV3 = intersections[j];
                }                
            }
            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = closestV3;
            cube.transform.localScale *= 0.1f;
            cube.name = "intersect";
            */

            //make midpoint variable out found intersect point
            midPoint = closestV3;

            /*
            for (int j = 0; j < outerEdgeOptions.Count; j++)
            {
                float temp = Vector3.Distance(midPoint, outerEdgeOptions[j]);

                if (temp < distance)
                {
                    distance = temp;
                    closest = j;
                }
           
            }
            */

            tri.Add(innerEdgePoints[i]);
            tri.Add(innerEdgePoints[i + 1]);
            //tri.Add(outerEdgeOptions[closest]);
            tri.Add(closestV3);

            //add to a list of used outers, used by corners function

            //usedOuterPoints.Add(outerEdgeOptions[closest]);
            usedOuterPoints.Add(closestV3);
        }

        //outer


        //create loop
        outerEdgeOptions.Add(outerEdgeOptions[0]);

        //now we need to create a new list which includes the corners and the edge options used by the inner tris
        //it needs to be in order, so cornerPoint , edgeOption, edgeOption, cornerPoint, edgeOption etc

        //from corner to corner
        //add corner
        //move in direction to next corner
        //iterate in small amounts
        //if edge option is close to corner + dir add, then remove edgeoption

        //make loop
        outsideNonLerped.Add(outsideNonLerped[0]);

        List<Vector3> outerWithCorners = new List<Vector3>();
        List<int> cornerPoints = new List<int>();

        for (int i = 0; i < outsideNonLerped.Count -1; i++)
        {
            //add the corner from which we are working from
            if(!outerWithCorners.Contains(outsideNonLerped[i]))
                outerWithCorners.Add(outsideNonLerped[i]);

            Vector3 dir = (outsideNonLerped[i + 1] - outsideNonLerped[i]).normalized;
            float distance = Vector3.Distance(outsideNonLerped[i], outsideNonLerped[i + 1]);

            for (float j = 0; j < distance; j += 0.1f)
            {
                Vector3 pointToCheck = outsideNonLerped[i] + (dir * j);

                //check for nearby edge option
                for (int k = 0; k < usedOuterPoints.Count; k++)
                {
                    if(Vector3.Distance(pointToCheck,usedOuterPoints[k]) < 0.1f)
                    {
                        //we have found a point
                        //add and remove (or check for duplicates)
                        if(!outerWithCorners.Contains(usedOuterPoints[k]))
                            outerWithCorners.Add(usedOuterPoints[k]);
                        //add this last addded int to a a list of cornerInts                        
                        cornerPoints.Add(outerWithCorners.Count - 1);
                        //remove pointwe found, we dont wwant to add it again
                        //usedOuterPoints.RemoveAt(k);
                    }
                }
            }
        }

        //make loop //add the first two again because we check two ahead looking for corners
        outerWithCorners.Add(outerWithCorners[0]);
        outerWithCorners.Add(outerWithCorners[1]);
        outerWithCorners.Add(outerWithCorners[2]);

        foreach (Vector3 v3 in outerWithCorners)
        {
         //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         ///                 cube.transform.position = v3;
         //               cube.transform.localScale *= 0.1f;
         //             cube.name = "outerwithcorners";
        }
        for (int i = 1; i < outerWithCorners.Count-2; i++)
        {
            float distance = Mathf.Infinity;
            int closest = 0;

            Vector3 midPoint = Vector3.Lerp(outerWithCorners[i], outerWithCorners[i + 1], 0.5f);

            //move midpoint in to middle of triangle
            Vector3 dir = (outerWithCorners[i + 1] - outerWithCorners[i]);//.normalized; //dont normalise, distances canbe diff
            
            
        //if a corner point, extend the range of the vector
        // if(cornerPoints.Contains(i))
        // {
            //some corners point do not go round right angles. Find if this is a right angles corner(or close)
            Vector3 direction1 = outerWithCorners[i+1] - outerWithCorners[i];
            Vector3 direction2 = outerWithCorners[i+2] - outerWithCorners[i + 1];
            float angle = Vector3.Angle( direction1, direction2);
         //   Debug.Log(angle);
            if (angle > 45f)
            {
                dir *= 0f;//0.5

         //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = outerWithCorners[i];
         //   cube.transform.localScale *= 0.1f;
          //  cube.name = "angle over";

            }

            else
            dir *= 0f; //0.25
           // }
            /*
            else
                //smaller range
                dir *= 0.0f;
            */

            dir = Quaternion.Euler(0f, 90f, 0f) * dir;
            midPoint += dir;

            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = midPoint;
            cube.transform.localScale *= 0.1f;
            cube.name = "mid";
            */
            for (int j = 0; j < innerEdgePoints.Count; j++)
            {
          
            
                //do not allow for an outer edge point to be used twice
                //       if (!usedOuterPoints.Contains(outerEdgeOptions[j]))
                //     {
                float temp = Vector3.Distance(midPoint, innerEdgePoints[j]);

                if (temp < distance)
                {
                    distance = temp;
                    closest = j;
                }
                //   }
            }
            tri2.Add(outerWithCorners[i]);            
            tri2.Add(innerEdgePoints[closest]);
            tri2.Add(outerWithCorners[i + 1]);
        }

        MakeSkirtingMesh();
    }

    void CreateTrianglesOuterFirst(List<Vector3> outerEdgePoints, List<Vector3> innerEdgePoints)
    {
        Vector3 startPoint = GetComponent<MeshRenderer>().bounds.center;

        //make lists have same y point
        for (int i = 0; i < innerEdgePoints.Count; i++)
        {
            Vector3 temp = innerEdgePoints[i];
            temp.y = startPoint.y;
            innerEdgePoints[i] = temp;
        }
        for (int i = 0; i < outerEdgePoints.Count; i++)
        {
            Vector3 temp = outerEdgePoints[i];
            temp.y = startPoint.y;
            outerEdgePoints[i] = temp;
        }
        
        //addd corners to list
        //create loop --
        outerEdgePoints.Add(outerEdgePoints[0]);
        outerEdgePoints.Add(outerEdgePoints[1]);

        foreach (Vector3 v3 in outerEdgePoints)
        {
          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = v3;
        }

        //list to make inner tris
        List<Vector3> usedInnerPoints = new List<Vector3>();

        for (int i = 0; i < outerEdgePoints.Count-2; i++) //-2 if checking angle need
        {
                  
            float distance = Mathf.Infinity;
            int closest = 0;

            Vector3 midPoint = Vector3.Lerp(outerEdgePoints[i], outerEdgePoints[i + 1], 0.5f);

            //move midpoint in to middle of triangle
            Vector3 dir = (outerEdgePoints[i + 1] - outerEdgePoints[i]);//.normalized; //dont normalise, distances canbe diff


            //if a corner point, extend the range of the vector
            // if(cornerPoints.Contains(i))
            // {
            //some corners point do not go round right angles. Find if this is a right angles corner(or close)
            Vector3 direction1 = outerEdgePoints[i + 1] - outerEdgePoints[i];

            //angle check - removed
            
            Vector3 direction2 = outerEdgePoints[i + 2] - outerEdgePoints[i + 1];
            float angle = Vector3.Angle(direction1, direction2);
           // Debug.Log(angle);
            if (angle > 45f)
            {
                dir *= 0f;//0.5

          //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.transform.position = outerEdgePoints[i];
              //  cube.transform.localScale *= 0.1f;
                //cube.name = "angle over";

            }

            else
                dir *= 0f; //0.25
                           // }
                           /*
                           else
                               //smaller range
                               dir *= 0.0f;
            */
            

            dir = Quaternion.Euler(0f, 90f, 0f) * dir;
            midPoint += dir;

            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = midPoint;
            cube.transform.localScale *= 0.1f;
            cube.name = "mid";
            */
            for (int j = 0; j < innerEdgePoints.Count; j++)
            {


                //do not allow for an outer edge point to be used twice
                //       if (!usedOuterPoints.Contains(outerEdgeOptions[j]))
                //     {
                float temp = Vector3.Distance(midPoint, innerEdgePoints[j]);

                if (temp < distance)
                {
                    distance = temp;
                    closest = j;
                }
                //   }
            }
            tri2.Add(outerEdgePoints[i]);
            tri2.Add(innerEdgePoints[closest]);
            tri2.Add(outerEdgePoints[i + 1]);

            //add to a list so we know what inner points were used so we can make inner tris from this
            if(!usedInnerPoints.Contains(innerEdgePoints[closest])) //no duplicates
                usedInnerPoints.Add(innerEdgePoints[closest]);
        }


        //loop
        usedInnerPoints.Add(usedInnerPoints[0]);

        for(int i = 0; i < usedInnerPoints.Count-1; i++)
        {

        
            //first inner triangle

            float distance = Mathf.Infinity;
            int closest = 0;

            Vector3 midPoint = Vector3.Lerp(usedInnerPoints[i], usedInnerPoints[i + 1], 0.5f);

            //move midpoint in to middle of triangle
            Vector3 dir = (usedInnerPoints[i + 1] - usedInnerPoints[i]).normalized;
            dir = Quaternion.Euler(0f, -90f, 0f) * dir;
           // dir *= 0.5f; //half a step-all steps are units
            midPoint += dir;

            /*
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = midPoint;
            cube.transform.localScale *= 0.1f;
            cube.name = "mid";
            */
            for (int j = 0; j < outerEdgePoints.Count; j++)
            {

                //do not allow for an outer edge point to be used twice
                //       if (!usedOuterPoints.Contains(outerEdgeOptions[j]))
                //     {
                float temp = Vector3.Distance(midPoint, outerEdgePoints[j]);

                if (temp < distance)
                {
                    distance = temp;
                    closest = j;
                }
                //   }
            }

            tri.Add(usedInnerPoints[i]);
            tri.Add(usedInnerPoints[i + 1]);
            tri.Add(outerEdgePoints[closest]);

            //add to a list of used outers, used by corners function

            //usedOuterPoints.Add(outerEdgeOptions[closest]);
        }

        MakeSkirtingMesh();
    }

    void MakeSkirtingMesh()
    {

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();

        foreach(Vector3 v3 in tri)
        {
            vertices.Add(v3);
        }

        foreach (Vector3 v3 in tri2)
        {
            vertices.Add(v3);
        }

        List<int> triangles = new List<int>();
        
        //no shared vertices, so add away
        for(int i = 0; i < vertices.Count; i+=3)
        {
            triangles.Add(i);
            triangles.Add(i+2);
            triangles.Add(i+1);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        /*
        mesh = CellYAdjust.AdjustYForLayer(transform, mesh, "Field", 10f);

        GameObject skirt = new GameObject();
        skirt.transform.parent = transform;
        skirt.name = "Skirt";

        MeshRenderer mr = skirt.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load("Green") as Material;

        MeshFilter mf = skirt.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        */
        skirtMesh = mesh;
        CombineMeshes();

    }

    void CombineMeshes()
    {

        //we dont need the original cell's mesh renderer, (or collider)
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        //create one mesh from the two built
        Mesh mesh = new Mesh();
        
        List<Vector3> vertices = new List<Vector3>();

     //   Debug.Log("Grid Vs - " + gridMesh.vertexCount);
        Vector3[] verticesGrid = gridMesh.vertices;

        List<int> triangles = new List<int>();
        
        for(int i = 0; i < verticesGrid.Length; i++)
        {
            vertices.Add(verticesGrid[i]);
        }

        
        int[] trianglesGrid = gridMesh.triangles;
        for(int i = 0; i < trianglesGrid.Length; i++)
        {
            triangles.Add(trianglesGrid[i]);
        }

        
        Vector3[] verticesSkirt = skirtMesh.vertices;
        for (int i = 0; i < verticesSkirt.Length; i++)
        {
            vertices.Add(verticesSkirt[i]);
        }

        int[] trianglesSkirt = skirtMesh.triangles;
        int previousTriAmount = gridMesh.vertexCount;

        for (int i = 0; i < trianglesSkirt.Length; i++)
        {
            triangles.Add(trianglesSkirt[i] + previousTriAmount) ;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
     

        GameObject combined = new GameObject();
        combined.transform.parent = transform;
        combined.name = "Combined";

        MeshRenderer mr = combined.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load("Green") as Material;

        MeshFilter mf = combined.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshCollider mc = combined.AddComponent<MeshCollider>();

        //subdivide this a little. SubdivideMesh automatically adds cellYAdjust. Could call all from here, this works fine
        SubdivideMesh sm = combined.AddComponent<SubdivideMesh>();
        sm.amount = 4;
        //combined.AddComponent<CellYAdjust>();
        //StartCoroutine(("WaitAndAdjustY"), combined);
        
    }

    IEnumerator WaitAndAdjustY(GameObject combined)
    {
        yield return new WaitForEndOfFrame();
        combined.AddComponent<CellYAdjust>();

    }

    void Update()
    {


        if (drawInsideTriangles)
        {
            for (int i = 0; i < tri.Count - 2; i += 3)
            {

                Debug.DrawLine(tri[i], tri[i + 1], Color.red);
                Debug.DrawLine(tri[i], tri[i + 2], Color.red);
                Debug.DrawLine(tri[i + 1], tri[i + 2], Color.red);

            }
        }
        if (drawOutsideTriangles)
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
