using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RingRoadMesh : MonoBehaviour {
	public float roadWidth = 2;
	public List<Vector3> ringRoadList;
    public BezierSpline spline;
    public BezierSpline updatedSpline;
    public List<Vector3> vergePoints = new List<Vector3>();
    public BezierSpline splineL;
	public BezierSpline splineR;
    public bool firstRing;
	public List<Vector3> waypointListM = new List<Vector3>();
	public List<Vector3> waypointListL = new List<Vector3>();
	public List<Vector3> waypointListR = new List<Vector3>();
    public List<Vector3> updatedSplinePoints = new List<Vector3>();
    public List<BezierControlPointMode> updatedSplineModes = new List<BezierControlPointMode>();
    //This script creates a mesh for the ring road
    //It also creates curves at the edge of the road
    private BezierSpline detailedSpline;

    public float frequency;
    public float stepSize;

    

    //remember where we stopped building this road. Used to build end junction piece
    float prevIndex = 0;

    public GameObject roadMeshGameObject;
    void Awake()
	{
		//enabled = false;
	}
	void Start () {

        

       // if(ringRoadList.Count ==0)//imporing directly in voronoiroadbuilder //not using
       // ringRoadList = GetComponent<PlaceWaypointPlayer>().roadPointsForCell;


        //grab the waypoint pathfinder list from its script
        //		waypointListM = GameObject.Find("Agents").GetComponent<WaypointInserter>().waypointListM;
        //		waypointListL = GameObject.Find("Agents").GetComponent<WaypointInserter>().waypointListL;
        //		waypointListR = GameObject.Find("Agents").GetComponent<WaypointInserter>().waypointListR;

        //	CreateRingRoadMesh();

        //smooth out curve so it smooths out any sharp turns
        // CreateUpdatedSpline();

        //instantiate Splines
        spline = gameObject.AddComponent<BezierSpline>();
        detailedSpline = gameObject.AddComponent<BezierSpline>();
        


        //create bezierSpline from these points

        //Create new updated spline which has any sharp corners smoothed out
        //instantiate new spline

        //Smooth curve function takes points from spline, smooths them out and fills updatedSpline


        //SmoothCurve();//



        //add control points to the list made by ring road list to tell the bezier class how to create curve
        List<Vector3> splinePoints = AddControlPointsToList(gameObject, ringRoadList);
        /////
        Modes(splinePoints, spline);
        
        RoadMesh();

        AgentWaypoints();

        //StartCoroutine("DetailedSpline");     //needs optimised(bucket list///not really working...
       

    }
    public BezierSpline Modes(List<Vector3> points, BezierSpline spline)
    {
       
        spline.points = points.ToArray();

        List<BezierControlPointMode> modes = new List<BezierControlPointMode>();
        for (int i = 0; i < spline.points.Length; i++)
        {
            modes.Add(BezierControlPointMode.Free);

         //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = spline.points[i];
         //   cube.name = i.ToString();
        }

        spline.modes = modes.ToArray();

        return spline;

    }

    void CreateUpdatedSpline()
    {
        updatedSpline = gameObject.AddComponent<BezierSpline>();

       
        //add control points to the updated curve created by ringroadmesh

        updatedSpline.points = GetComponent<RingRoadMesh>().updatedSplinePoints.ToArray();
        updatedSpline.modes = GetComponent<RingRoadMesh>().updatedSplineModes.ToArray();
    }

    void SmoothCurve()
    {
        //create frequency fraction
        //this governs how often (at what fraction) we iterate through curve and grab what we need

        frequency = ringRoadList.Count * 50;//was 50
        stepSize = frequency;
        stepSize = 1f / (stepSize);

        //use this variable to make sure raod flows smoothly
        //If the angle changes too abrubtly, we will smooth the change
        float previousAngle = 0f;
        //Now, run through the curve and add points to the temporary lists
        for (int i = 0; i <= frequency; i++)
        {
            //use the function "GetPoint" in the bezier class
            Vector3 position = spline.GetPoint((i) * stepSize);
            Vector3 forward = spline.GetPoint((i + 1) * stepSize);
            Vector3 backward = spline.GetPoint((i - 1) * stepSize);

            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 direction = spline.GetDirection((i) * stepSize);

            Vector3 forwardDir = (forward - position);//.normalized;
            forward = position + forwardDir;

            Vector3 backDirection = (position - backward);//.normalized;
            backward = position + backDirection;

            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;

            //using the direction to n the next point, not normalized. GetDirection returns normalized point
            Vector3 angleRotation = Quaternion.Euler(0, -90, 0) * forwardDir;

            //Use function SignedAngle(at the bottom of this script) to return a float with a minus or plus
            float angle = SignedAngle(forwardDir, backDirection);
            Vector3 offsetFromAngle = Vector3.zero;

            //limit the tightness of the turn
            float maxTurn = 0.2f;//
            //using clamp to dampen it
            angle = Mathf.Clamp(angle, previousAngle - maxTurn, previousAngle + maxTurn);

            offsetFromAngle = angle * rot2;
            //save last angle to compare it on next iteration for dampening/hairpin protection
            previousAngle = angle;

            //create directions to add points each side of the curve for mesh
            rot1 *= roadWidth;
            rot2 *= roadWidth;

            Vector3 offset1 = rot1 + position + offsetFromAngle;
            Vector3 offset2 = rot2 + position + offsetFromAngle;

            //add to points to create a new updated spline, now that we have skewed the road
            //create a central point between offsets
            Vector3 centre = Vector3.Lerp(offset1, offset2, 0.5f);
            updatedSplinePoints.Add(centre);
            //add the mode now because it is easy to do so
            updatedSplineModes.Add(BezierControlPointMode.Mirrored);
            //actual bezier created in houses for voronoi

           GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.name = angle.ToString();

        }

        //Feed updatedSpline this data

        

        updatedSpline.points = GetComponent<RingRoadMesh>().updatedSplinePoints.ToArray();

        //make as many modes as points

        //clear whatever was done above -- new test
        updatedSplineModes = new List<BezierControlPointMode>();
        for(int i = 0; i < updatedSplinePoints.Count;i++)
        {
          //  updatedSplineModes.Add(BezierControlPointMode.Free);
        }

        updatedSpline.modes = GetComponent<RingRoadMesh>().updatedSplineModes.ToArray();
    }

    void DetailedSpline()
    {
        //run through our smooth bezierspline and create an array of lots of points

        float detailedFrequency = frequency * 10;
        float detailedStepsize = 1f / detailedFrequency;

        List<Vector3> detailedList = new List<Vector3>();

        float totalDistance = 0;
        for (int i = 0; i < detailedFrequency; i++)
        {

            if (i == 0)
                continue;

            Vector3 pos = updatedSpline.GetPoint(i * detailedStepsize);
            detailedList.Add(pos);

            Vector3 prevPos = updatedSpline.GetPoint( (i-1) * detailedStepsize);

            float distance = Vector3.Distance(pos, prevPos);

            totalDistance += distance;

        }

        //Now run through a new spline and look for the nearest point in this detailed list
        //this should give us a roughly even curve
       
    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //    cube.transform.position = position;

        List<Vector3> evenList = new List<Vector3>();
        List<int> evenInt = new List<int>();

        evenList.Add(updatedSpline.GetPoint(0));
        evenInt.Add(0);

        float stepSizeForConstant = 1f / totalDistance;
        for (int i = 0; i < detailedList.Count;)
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            int lastInt = evenInt[evenInt.Count - 1];

            Vector3 p1 = updatedSpline.GetPoint(lastInt * detailedStepsize);
            Vector3 dir = updatedSpline.GetDirection(lastInt * detailedStepsize).normalized;

            Vector3 nextPoint = p1 + dir;

            //find closest point in updated list to position
            float distance = Mathf.Infinity;
            Vector3 closest = Vector3.zero;
            int closestInt = 0;
            for(int j = 0; j < detailedList.Count; j++)
            {

                Vector3 thisPosition = detailedList[j];
                float temp = Vector3.Distance(nextPoint, thisPosition);

                if(temp < distance)
                {
                    distance = temp;
                    closest = thisPosition;
                    closestInt = j;
                }
            }

            // i is == to the difference between the last int and this int
            i += closestInt - lastInt;

         //   Debug.Log(closestInt);
            //add the closest point to this point in the detailed list to this position
            evenList.Add(closest);
            evenInt.Add(closestInt);

            
           
        }

        foreach(Vector3 v3 in evenList)
        {
          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = v3;

          //  yield return new WaitForEndOfFrame();
        }

        //create spline
        Modes(evenList, updatedSpline);

        //create road mesh from this new curve
        RoadMesh();
        //yield break;
    }

    //takes a list of points and adds control point to it
    public static List<Vector3> AddControlPointsToList(GameObject gameObject, List<Vector3> originalPoints)
    {
        List<Vector3> tempList = new List<Vector3>();

        //        float threshold = gameObject.GetComponent<PlaceWaypointPlayer>().threshold;
        float threshold = 30f;//global?
                              //  bool loop = gameObject.GetComponent<PlaceWaypointPlayer>().loopRoad;//**removed
        bool loop = gameObject.GetComponent<RingRoadMesh>().firstRing;
        int start = 0;
        int end = originalPoints.Count - 2;

        if (loop)
        {
            start = 0;
            //end = originalPoints.Count - 1;
        }
        //normal roads miss start and end cos we will build junctions later on, loop roads just feeed in to each themselves
        for (int i = start; i < end; i++)
        {
            //don't add bunched up road points at end, the junction function will create a nice smooth transition usign as few as points as posiible which makes for nice smooth curves
            //if(!loop)
              //  if (Vector3.Distance(originalPoints[i], originalPoints[originalPoints.Count - 1]) < threshold)
                //    break;

            //insert first point
            Vector3 currentPoint = originalPoints[i];
            tempList.Add(currentPoint);

            //add the first control point, this is half waytowards the next point
            Vector3 nextPoint = originalPoints[i + 1];

            //Linear interpolation at 0.5 gives us the half waypoint
            Vector3 pos = Vector3.Lerp(currentPoint, nextPoint, 0.5f);
            //tempList.Add(pos);
            //clamp  distance - distance set by threshold in waypoint placer
            Vector3 dirToNext = (nextPoint - currentPoint).normalized;
            //Vector3 pos = currentPoint + dirToNext;//add to use threshold
            //clamp
            float distanceToPos = Vector3.Distance(currentPoint, pos);
            if (distanceToPos > threshold)
                pos = currentPoint + dirToNext * threshold;

            tempList.Add(pos);

            //third point
            Vector3 nextPointTarget = originalPoints[i + 2];
            Vector3 nextPointHalfWay = Vector3.Lerp(nextPoint, nextPointTarget, 0.5f);

            //Work out the directional vector between the next point and halfway to its target
            //this subtraction will give us a direction point at "nextPointHalfWay"
            Vector3 direction = (nextPointTarget - nextPointHalfWay);

            //Now use this direction to move nextPoint back a half step
            Vector3 newPos = nextPoint - direction;
            direction.Normalize();
            float distance = Vector3.Distance(currentPoint, nextPoint);
            distance *= 0.5f;

            //clamp
            if (distance > threshold)
                distance = threshold;

             direction *= distance;
            //direction *= threshold;//add to use threshold
            Vector3 junctionCp = nextPoint - direction;
            tempList.Add(junctionCp);

           /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = currentPoint;
            c.name = "i";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = pos;
            c.name = "cp1";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = junctionCp;
            c.name = "cp2";
         */
        }        
        
        if (loop)
        {
            Vector3 currentPoint =originalPoints[originalPoints.Count - 2];
            tempList.Add(currentPoint);

            //add the first control point, this is half waytowards the next point
            Vector3 nextPoint = originalPoints[originalPoints.Count - 1];

            //Linear interpolation at 0.5 gives us the half waypoint
            //choose if we should use halfway or clamp it at threshold distance
            
            Vector3 pos = Vector3.Lerp(currentPoint, nextPoint, 0.5f);
            Vector3 dirToNext = (nextPoint - currentPoint).normalized;
            float distanceToPos = Vector3.Distance(currentPoint, pos);
            if (distanceToPos > threshold)
                pos = currentPoint + dirToNext * threshold;

            tempList.Add(pos);

            //now for the second control point. This point needs to be aligned with the first point's control point so it makes a smooth join
            //Vector3 nextPointTarget = originalPoints[0]; //the start fo the road
            Vector3 nextPointAligned = (tempList[0] - tempList[1]) + originalPoints[0];
            tempList.Add(nextPointAligned);            

            //now complete the loop
            tempList.Add(originalPoints[0]);
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = currentPoint;
            c.name = "i last";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = pos;
            c.name = "cp1";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = nextPointAligned;
            c.name = "cp2";
            */
        }
        
        else if (!loop)
        {
           
            //We skipped the last two point in the list so the index did not go out of range, so enter them in now
            tempList.Add(originalPoints[originalPoints.Count - 2]);    

            Vector3 secondlast = originalPoints[originalPoints.Count - 2];
            Vector3 last = originalPoints[originalPoints.Count - 1];
            //first control point is halfway to the last point in the list
            Vector3 cp1 = Vector3.Lerp(secondlast, last, 0.5f);

            tempList.Add(cp1);         

            //second control point

            //now for the second control point. This point is actually linked with the next main point that we are adding,
            //but we can add it now
            //We need this point to face away from the next point's target, so 2 up in the ringRooadList
            Vector3 nextPointHalfWay = Vector3.Lerp(secondlast, last, 0.5f);

            //Work out the directional vector between the next point and halfway to its target
            //this subtraction will give us a direction point at "nextPointHalfWay"
            Vector3 direction = (last - cp1);
            direction.Normalize();
            float distance = Vector3.Distance(secondlast, last);
            distance *= 0.5f;
            direction *= distance;
            Vector3 junctionCp = last - direction;

            tempList.Add(junctionCp);
          
            //finally the last point
            tempList.Add(originalPoints[originalPoints.Count - 1]);
            
        }
        
        return tempList;
    }

    void RoadMesh()
    {
        frequency = ringRoadList.Count * 50;//this controls field density
        float meshDensity = 0.5f;
        //frequency *= 2;
        //stepSize = frequency;
        stepSize = 1f / (frequency);
        float vergeDepth = 4f;
        int vergeDensity = 5;
        //uses updated spline to create road mesh
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> vergeVerticesL = new List<Vector3>();
        List<Vector3> vergeVerticesR = new List<Vector3>();

        //used to measure how far we have travelled up the curve. Creates a uniform curve - could add angle in to make sharp turns more detailed
        Vector3 prevPos = Vector3.zero;

        //start at 1 , we will do the start junction piece after all roads are built
        //if loop build fropm start

        bool loop = false;// GetComponent<PlaceWaypointPlayer>().loopRoad;
        

        Vector3 lastPoint = ringRoadList[ringRoadList.Count - 1];
        for (float i = 2f; i < frequency; i+= 0.1f)//2f to make a gap for junction - 0.1f++ ? could be bigger for optimisation? need to make it similiar to meshDensity distance check below
        {

            //ensures mesh is roughly even between - loop steps through at a small pace, but only paces a point every 1m(metere?) what is this?
            Vector3 position = spline.GetPoint((i) * stepSize);

            if (prevPos != Vector3.zero)
                if (Vector3.Distance(position, prevPos) < meshDensity)
                    continue;

            //skip at end, we are close nough now
            if (!loop)
            {
                if (Vector3.Distance(position, lastPoint) < roadWidth)
                {
                    prevPos = position;
                    prevIndex = i;
                    break;
                }
            }

            prevPos = position;
            prevIndex = i;

            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            //Vector3 direction = updatedSpline.GetDirection((i) * stepSize);
            Vector3 direction = spline.GetDirection((i) * stepSize);

            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;

            //create directions to add points each side of the curve for mesh
            rot1 *= roadWidth;
            rot2 *= roadWidth;


            Vector3 offset1 = rot1 + position;
            Vector3 offset2 = rot2 + position;

            //add to temp vertices list for mesh
            vertices.Add(offset1);
            vertices.Add(offset2);

            //add points to a list for agents use //** if we make mesh more setailed this is bad, seperate lop for agents - could use verge list below?********************************
            waypointListM.Add(position);
            waypointListL.Add(Vector3.Lerp(position,offset1,0.5f));
            waypointListR.Add(Vector3.Lerp(position, offset2, 0.5f));

            //add to verge now
           
            float step = vergeDepth / vergeDensity;
            for (float x = 0; x < vergeDepth; x+=step)
            {
                //start at road width and plot points beyond
                Vector3 left = position + (Quaternion.Euler(0, -90, 0) * direction) * (roadWidth +  (x*step));
                Vector3 right = position + (Quaternion.Euler(0, 90, 0) * direction) * (roadWidth + (x * step));

                vergeVerticesL.Add(left);
                vergeVerticesR.Add(right);

              //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  c.transform.position = left;
              //  c.transform.localScale *= 0.25f;
            }

            
        }

        

        if (firstRing)
        {
            //Now add the first two vertices to be the last two as well to complete the mesh
            //step towards destination in case we are slighlty adrift
           
            
            //this isn't perfect but happy enough atm///skipping, loops close enough?
            Vector3 midStart = Vector3.Lerp(vertices[0], vertices[1],0.5f);
            float distance = Vector3.Distance(spline.GetPoint(prevIndex), midStart);
            Vector3 dir = (midStart - spline.GetPoint(prevIndex)).normalized;
            
            for (float i = 0; i <= distance; i+=0.1f)
            {
                Vector3 p = spline.GetPoint(prevIndex) + dir * i;

               // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  c.transform.position = p;
              //  c.transform.localScale *= 0.1f;
              //  c.name = "loop";

                Vector3 rot1 = Quaternion.Euler(0, -90, 0) * dir;
                Vector3 rot2 = Quaternion.Euler(0, 90, 0) * dir;

                //create directions to add points each side of the curve for mesh
                rot1 *= roadWidth;
                rot2 *= roadWidth;

              //  vertices.Add(p + rot1);
              //  vertices.Add(p + rot2);

              //  c = GameObject.CreatePrimitive(PrimitiveType.Cube);
             //   c.transform.position = p+rot2;
             //   c.transform.localScale *= 0.1f;
              //  c.name = "loop1";

             //   c = GameObject.CreatePrimitive(PrimitiveType.Cube);
             //   c.transform.position = p+rot2;
             //   c.transform.localScale *= 0.1f;
             //   c.name = "loop2";
            }

            vertices.Add(vertices[0]);
            vertices.Add(vertices[1]); 

            //to make the start of the road curve in towards the last pint we must adjust its control point
            //the first point's control point is the second point in the array
            //
        }

        //instantiate Beziers and input points and control modes
        splineL = gameObject.AddComponent<BezierSpline>();
        splineR = gameObject.AddComponent<BezierSpline>();


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        List<int> indices = new List<int>();
        for (int i = 0; i < vertices.Count - 2; i += 2)
        {
            indices.Add(i + 2);
            indices.Add(i + 1);
            indices.Add(i);
            indices.Add(i + 3);
            indices.Add(i + 1);
            indices.Add(i + 2);
        }

        mesh.triangles = indices.ToArray();

        GameObject child = new GameObject();
        child.name = "RingRoad";
    
        child.transform.position = this.gameObject.transform.position;
        
        child.transform.parent = this.gameObject.transform;
        child.layer = 9;
        child.tag = "Road";

        mesh.name = "Ring Road Mesh";
        MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshFilter.sharedMesh = mesh;

        Material newMat = Resources.Load("Brown", typeof(Material)) as Material;
        meshRenderer.material = newMat;

        //save for other scripts to access
        roadMeshGameObject = child;

        //make meshes for vergees - this is simpler than it was before- in verges script, raycasts are done to check for stuff. below function doesn't do this. voronoi mesh script and skirt joining script are checking for heights nowS
        VergeMeshes(vergeVerticesL, vergeVerticesR, vergeDensity);
        
    }

    public void VergeMeshes(List<Vector3> vergeVerticesLeft, List<Vector3> vergeVerticesRight,int vergeDensity)
    {

        
        

        
        for (int i = 0; i < 2; i++)
        {
            List<int> triangles = new List<int>();

            if (i == 0)
            {
                for (int a = 0; a < vergeVerticesLeft.Count-vergeDensity; a+=vergeDensity)
                {
                    for (int b = 0; b < vergeDensity-1; b++)
                    {
                        triangles.Add(a + b);                        
                        triangles.Add(a + b +1);
                        triangles.Add(a + b + vergeDensity);

                        triangles.Add(a + b + vergeDensity);
                        triangles.Add(a + b + +1);
                        triangles.Add(a + b + vergeDensity+1);
                    }
                }
            }

            if (i == 1)
            {
                for (int a = 0; a < vergeVerticesRight.Count - vergeDensity; a += vergeDensity)
                {
                    for (int b = 0; b < vergeDensity - 1; b++)
                    {
                        triangles.Add(a + b);
                        
                        triangles.Add(a + b + vergeDensity);
                        triangles.Add(a + b + 1);

                        triangles.Add(a + b + vergeDensity);
                        
                        triangles.Add(a + b + vergeDensity + 1);
                        triangles.Add(a + b + +1);
                    }
                }
            }

            Mesh mesh = new Mesh();
            if (i == 0)
                mesh.vertices = vergeVerticesLeft.ToArray();
            else
                mesh.vertices = vergeVerticesRight.ToArray();

            mesh.triangles = triangles.ToArray();

            GameObject verge = new GameObject();
            verge.transform.parent = this.gameObject.transform;
            verge.name = "Verge";
            verge.tag = "TerrainSecondLayer";
            verge.layer = 10;
            MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
            meshFilterInstance.mesh = mesh;
            MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            meshRenderer.sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
            MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

    }

    /// <summary>
    /// Junctions are built after all roads have been built. Called directly from CellManager
    /// </summary>
    public void JunctionMeshStart()
    { 
        
        //find line intersection where each side of the road connects with target road
        Vector3 directionForStart = -spline.GetDirection((1) * stepSize);
        Vector3 positionForStart = spline.GetPoint((1) * stepSize);
        Vector3 rot1ForStart = Quaternion.Euler(0, -90, 0) * directionForStart;
        Vector3 rot2ForStart = Quaternion.Euler(0, 90, 0) * directionForStart;

        //create directions to add points each side of the curve for mesh
        rot1ForStart *= roadWidth;
        rot2ForStart *= roadWidth;

        Vector3 offset1ForStart = rot1ForStart + positionForStart;
        Vector3 offset2ForStart = rot2ForStart + positionForStart;

        int closestIndexOnStartMesh = GetComponent<PlaceWaypointPlayer>().closestIndexOnMeshStart;
        float threshold = GetComponent<PlaceWaypointPlayer>().threshold;
        Vector3 posStart = GetComponent<PlaceWaypointPlayer>().roadPointsForCell[0] + Vector3.up * threshold * 2;
        Vector3 roadEdge = Vector3.zero;

        //always finds start in waypoint player
        //RaycastHit hitStart;
        //if (!Physics.SphereCast(posStart, threshold, Vector3.down, out hitStart, threshold * 3, LayerMask.GetMask("Road")))
        //{
          //  Debug.Log("Start road Junction never hit road");
            //return;
        //}
        GameObject roadTargetStart = GetComponent<PlaceWaypointPlayer>().targetRoadStart;
        if(roadTargetStart == null)
        {
              
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = positionForStart;
            c.transform.localScale *= 50;
            c.name = "Start with no road";
            return;
        }
        PlaceWaypointPlayer.ClosestPointOnMesh(out closestIndexOnStartMesh, positionForStart, roadTargetStart);

        List<int> nearByIndexesStart = new List<int>();

        Vector3[] targetVerticesStart = roadTargetStart.GetComponent<MeshFilter>().mesh.vertices;

        for (int i = -20; i < 20; i += 2)//+=2 so we run up one side of the road mesh
        {
            int indexToUse = i + closestIndexOnStartMesh;
            if (indexToUse < 0)
            {
                Debug.Log("Moved up");
                indexToUse += targetVerticesStart.Length;
            }
            else if (indexToUse >= targetVerticesStart.Length)
            {
                indexToUse -= targetVerticesStart.Length;
                Debug.Log("Moved down");
            }

            Vector3 meshPoint = targetVerticesStart[indexToUse];
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = meshPoint;
            c.name = "mesh point";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = spline.GetPoint(1);
            c.name = " spline.GetPoint(stepSize)";
            */
            if (Vector3.Distance(meshPoint, targetVerticesStart[closestIndexOnStartMesh]) < roadWidth * 1.5f)//?
            {
                nearByIndexesStart.Add(indexToUse);
            }
        }
        int indexToAdd = nearByIndexesStart.Count - 1 + 2;
        if (indexToAdd > targetVerticesStart.Length)
            indexToAdd -= targetVerticesStart.Length;
        //nearByIndexesStart.Add(indexToAdd);
        //worked out the end first, but because we are reversing the spline direction to go back the way from 2/frequency, spin the other points too
        nearByIndexesStart.Reverse();

        List<Vector3> endVertices = new List<Vector3>();
        List<int> endTriangles = new List<int>();

        for (int i = 0; i < nearByIndexesStart.Count - 1; i++)
        {

            Vector3 p0 = targetVerticesStart[nearByIndexesStart[i]];
            Vector3 p1 = targetVerticesStart[nearByIndexesStart[i + 1]];
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p0;
            c.name = "p0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p1;
            c.name = "p1";
            */
            Vector3 start = offset1ForStart;
            Vector3 end = offset2ForStart;

            Vector3 r0 = start + ((end - start) / (nearByIndexesStart.Count - 1)) * i;
            Vector3 r1 = start + ((end - start) / (nearByIndexesStart.Count - 1)) * (i + 1);
            /*
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r0;
            c.name = "r0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r1;
            c.name = "r1";
            */
            //not end tri order will be different-mirrored, building other way round
            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(p0);
            endTriangles.Add(endVertices.Count - 1);


            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r1);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

        }
        Mesh endMesh = new Mesh();
        endMesh.vertices = endVertices.ToArray();
        endMesh.triangles = endTriangles.ToArray();


        GameObject child0 = new GameObject();
        child0.name = "RoadStart";
        child0.transform.position = this.gameObject.transform.position;

        child0.transform.parent = this.gameObject.transform;
        child0.layer = 9;
        child0.tag = "Road";

        endMesh.name = "Start Mesh";
        MeshFilter endMeshFilter = child0.gameObject.AddComponent<MeshFilter>();
        MeshRenderer endMeshRenderer = child0.gameObject.AddComponent<MeshRenderer>();
        endMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        endMesh.RecalculateBounds();
        endMesh.RecalculateNormals();

        MeshCollider endMeshCollider = child0.gameObject.AddComponent<MeshCollider>();
        endMeshCollider.sharedMesh = endMesh;
        endMeshFilter.sharedMesh = endMesh;

        Material endNewMat = Resources.Load("Brown", typeof(Material)) as Material;
        endMeshRenderer.material = endNewMat;
        
    }

    public void JunctionMeshEnd(bool forStart)
    {
        //do joining mesh for last      

        // Vector3 directionForEnd = spline.GetDirection((prevIndex) * stepSize);
        //if(forStart)
        //     directionForEnd = spline.GetDirection((1f) * stepSize);

        Vector3 positionForEnd = spline.GetPoint(prevIndex);
        if (forStart)
            positionForEnd = spline.GetPoint(1f * stepSize);

       // Vector3 rot1ForEnd = Quaternion.Euler(0, -90, 0) * directionForEnd;
       // Vector3 rot2ForEnd = Quaternion.Euler(0, 90, 0) * directionForEnd;

        //create directions to add points each side of the curve for mesh
      //  rot1ForEnd *= roadWidth;
      //  rot2ForEnd *= roadWidth;

       // Vector3 offset1ForEnd = rot1ForEnd + positionForEnd;
       // Vector3 offset2ForEnd = rot2ForEnd + positionForEnd;

        
        int closestOverallIndex = 0;
        //we need to cast fro a road to be able to stitch to its vertices
        float threshold = GetComponent<PlaceWaypointPlayer>().threshold;
        //RaycastHit[] hits = Physics.SphereCastAll(positionForEnd + Vector3.up * threshold * 2, threshold, Vector3.down, threshold * 3, LayerMask.GetMask("Road"));
        //will hit our own, skip that, can hit more than one? 
     //   if (hits.Length > 2)
     //   {
     //       Debug.Log("Multiple roads hit for end of junction");
     //       return;
     //   }
     //   else if (hits.Length == 1)
       
            
            
        //now we need to add a curve and smooth our way towards road
        BezierSpline junctionSpline = gameObject.AddComponent<BezierSpline>();
        List<Vector3> junctionSplinePoints = new List<Vector3>();
        List<Vector3> junctionVertices = new List<Vector3>();


        List<Vector3> roadPointsForCell = GetComponent<PlaceWaypointPlayer>().roadPointsForCell;
        RaycastHit hit;
        if (!forStart)
        {
            //if targetroadend in placewaypoint was found
            GameObject targetRoad = null;
            if (GetComponent<PlaceWaypointPlayer>().targetRoadEnd != null)
            {
                targetRoad = GetComponent<PlaceWaypointPlayer>().targetRoadEnd;
            }
            else
            {
                Debug.Log("End from junction function");//ever here now? - hopefully not
                //if no road found already, loook for road now and add curve
                if (Physics.SphereCast(roadPointsForCell[roadPointsForCell.Count - 1] + Vector3.up * threshold * 2, threshold, Vector3.down, out hit, threshold * 2f, LayerMask.GetMask("Road")))
                {
                    targetRoad = hit.transform.gameObject;

                    //shoot ray and find road we are supposed to be building to

                    Vector3 lastRoadPoint = roadPointsForCell[roadPointsForCell.Count - 1];

                    Vector3 dirBack = (roadPointsForCell[roadPointsForCell.Count - 2] - roadPointsForCell[roadPointsForCell.Count - 1]).normalized;
                    Vector3 checkFrom = roadPointsForCell[roadPointsForCell.Count - 1] + (dirBack * roadWidth);

                    int closestIndex;
                    Vector3 closestV3 = PlaceWaypointPlayer.ClosestPointOnMesh(out closestIndex, checkFrom, targetRoad.transform.gameObject);
                    Vector3 cp1 = positionForEnd + spline.GetDirection(prevIndex).normalized * Vector3.Distance(positionForEnd, closestV3) * 0.5f;
                    Vector3 secondLastPoint = spline.points[spline.points.Length - 1];
                    //get direction
                    Vector3 dirToLastCp = (secondLastPoint - positionForEnd).normalized;
                    //subtract this direction so we get amirrored point back the way//multiply it by distance from closest point on road tofirst point /2
                    Vector3 cp2 = Vector3.Lerp(positionForEnd, closestV3, 0.5f);

                    junctionSplinePoints.Add(positionForEnd);

                    junctionSplinePoints.Add(cp1);
                    junctionSplinePoints.Add(cp2);
                    junctionSplinePoints.Add(closestV3);

                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = positionForEnd;
                    c.name = "position for end from Junction function";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = cp1;
                    c.name = "cp1 from Junction function";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = cp2;
                    c.name = "cp2 from Junction function";


                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = closestV3;
                    c.name = "closest from Junction function";

                }
                else
                {
                    Debug.Log("NO target road found");
                    GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadPointsForCell[roadPointsForCell.Count-1];
                    d.name = "No end Found";
                    d.transform.localScale *= 20;
                }
            }
            
        }
        else if (forStart)
        {
            Vector3 closestOverall = Vector3.zero;
            //find control point to allow junction to T shaped
            Vector3 towardsFirstPoint = (positionForEnd - closestOverall).normalized;
            Vector3 towardsPrev = spline.GetDirection(prevIndex - stepSize);
            Vector3 cp1 = Vector3.Lerp(closestOverall, spline.points[0], 0.5f);
            //we need to line the next cp up with the first control pointin the main curve for a smooth transition

            //third point
            //get first cp in main curve
            Vector3 firstCpMainCurve = spline.points[1];
            //get direction
            Vector3 dirToFirstCp = spline.points[1] - spline.points[0];
            //subtract this direction so we get amirrored point back the way//multiply it by distance from closest point on road tofirst point /2
            Vector3 cp2 = spline.points[0] - (dirToFirstCp.normalized * Vector3.Distance(positionForEnd, closestOverall) * 0.5f);




            junctionSplinePoints.Add(closestOverall);
            junctionSplinePoints.Add(cp1);
            junctionSplinePoints.Add(cp2);
            junctionSplinePoints.Add(positionForEnd);
        }



            
            
        Mesh mesh = new Mesh();
        mesh.vertices = junctionVertices.ToArray();

        List<int> indices = new List<int>();
        for (int i = 0; i < junctionVertices.Count - 2; i += 2)
        {
            indices.Add(i + 2);
            indices.Add(i + 1);
            indices.Add(i);
            indices.Add(i + 3);
            indices.Add(i + 1);
            indices.Add(i + 2);
        }

        mesh.triangles = indices.ToArray();

        GameObject child = new GameObject();
        if (forStart)
            child.name = "JunctionMeshStart";
        else
            child.name = "JunctionMeshEnd";

        child.transform.position = this.gameObject.transform.position;

        child.transform.parent = this.gameObject.transform;
        child.layer = 9;
        child.tag = "Road";

        mesh.name = "Junction Mesh";
        MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshFilter.sharedMesh = mesh;

        Material newMat = Resources.Load("Brown", typeof(Material)) as Material;
        meshRenderer.material = newMat;


        List<int> nearByIndexes = new List<int>();
        Vector3[] targetVertices = GetComponent<PlaceWaypointPlayer>().targetRoadEnd.GetComponent<MeshFilter>().mesh.vertices;
        // Debug.Log(targetVertices.Length);

        //find closest points to each end of road we are building
        Vector3 closest1 = Vector3.zero;
        Vector3 closest2 = Vector3.zero;

        int lengthToUse = 20;
        if (targetVertices.Length < 20)
            lengthToUse = targetVertices.Length/2;
        for (int i = -lengthToUse; i < lengthToUse; i += 2)//+=2 so we run up one side of the road mesh
        {
            int indexToUse = i + closestOverallIndex;
            if (indexToUse < 0)
            {
                indexToUse += targetVertices.Length;
                // Debug.Log("Moved up for end");

                // Debug.Log("i = " + i.ToString());
                //  Debug.Log("index to use = " + indexToUse);
                //  Debug.Log("length = " + targetVertices.Length);

            }
            else if (indexToUse >= targetVertices.Length)
            {
                indexToUse -= targetVertices.Length;
                //   Debug.Log("Moved down for end");
                //   Debug.Log("i = " + i.ToString());
                //   Debug.Log("index to use = " + indexToUse);
                //   Debug.Log("length = " + targetVertices.Length);
            }

            Vector3 meshPoint = targetVertices[indexToUse];


            if (Vector3.Distance(meshPoint, targetVertices[closestOverallIndex]) < roadWidth * 1.5f)//?
            {
                nearByIndexes.Add(indexToUse);
            }
        }

        //nearByIndexes.Add(nearByIndexes[nearByIndexes.Count - 1]+2);
        
        List<Vector3> endVertices = new List<Vector3>();
        List<int> endTriangles = new List<int>();

        //switch round, seems to always need this?
        nearByIndexes.Reverse();

        for (int i = 0; i < nearByIndexes.Count - 1; i++)
        {


            Vector3 p0 = targetVertices[nearByIndexes[i]];
            int nextIndex = i + 1;
            if (nextIndex > targetVertices.Length)
                nextIndex -= targetVertices.Length;

            Vector3 p1 = targetVertices[nearByIndexes[nextIndex]];
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p0;
            c.name = "p0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p1;
            c.name = "p1";
            */
            Vector3 start = junctionVertices[1];
            Vector3 end = junctionVertices[0];
            if (!forStart)
            {
                start = junctionVertices[junctionVertices.Count - 2];
                end= junctionVertices[junctionVertices.Count - 1];
            }
            Vector3 r0 = start + ((end - start) / (nearByIndexes.Count - 1)) * i;
            Vector3 r1 = start + ((end - start) / (nearByIndexes.Count - 1)) * (i + 1);
            /*
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r0;
            c.name = "r0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r1;
            c.name = "r1";
            */
            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r1);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

        }

        Mesh endMesh = new Mesh();
        endMesh.vertices = endVertices.ToArray();
        endMesh.triangles = endTriangles.ToArray();


        GameObject child0 = new GameObject();
        child0.name = "RoadEnd";
        child0.transform.position = this.gameObject.transform.position;

        child0.transform.parent = this.gameObject.transform;
        child0.layer = 9;
        child0.tag = "Road";

        endMesh.name = "End Mesh";
        MeshFilter endMeshFilter = child0.gameObject.AddComponent<MeshFilter>();
        MeshRenderer endMeshRenderer = child0.gameObject.AddComponent<MeshRenderer>();
        endMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        endMesh.RecalculateBounds();
        endMesh.RecalculateNormals();

        MeshCollider endMeshCollider = child0.gameObject.AddComponent<MeshCollider>();
        endMeshCollider.sharedMesh = endMesh;
        endMeshFilter.sharedMesh = endMesh;

        Material endNewMat = Resources.Load("Brown", typeof(Material)) as Material;
        endMeshRenderer.material = endNewMat;
        

       // else if (hits.Length == 0)
      //  {
     //       Debug.Log("foun zero");
      //      GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
     //       c.transform.position = positionForEnd;
     //       c.transform.localScale *= 10;
      //      c.name = "no hit";
      //  }
            

    }

    public static void JoiningMeshEnd(GameObject gameObject,Vector3[] targetVertices,List<Vector3> roadPointsForCell,int closestIndex, Vector3[] mainVertices)
    {
        float roadWidth = 3f;///is 2 check ringroadmeah.roadwidth

        List<int> nearByIndexes = new List<int>();
        //Vector3[] targetVertices = gameObject.GetComponent<PlaceWaypointPlayer>().targetRoadEnd.GetComponent<MeshFilter>().mesh.vertices;
        // Debug.Log(targetVertices.Length);

        //find closest points to each end of road we are building
        Vector3 closest1 = Vector3.zero;
        Vector3 closest2 = Vector3.zero;
       // List<Vector3> roadPointsForCell = gameObject.GetComponent<PlaceWaypointPlayer>().roadPointsForCell;
        //direction towards second last point from last point * roadwidth
        Vector3 checkFrom = (roadPointsForCell[roadPointsForCell.Count - 2] - roadPointsForCell[roadPointsForCell.Count - 1]).normalized* roadWidth + roadPointsForCell[roadPointsForCell.Count - 1];

       // GameObject cf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
      //  cf.name = "Check From";
      //  cf.transform.position = checkFrom;

        

        // int closestIndex = gameObject.GetComponent<PlaceWaypointPlayer>().closestIndexOnMesh;
        //Vector3 closestV3 = PlaceWaypointPlayer.ClosestPointOnMesh(out closestIndex, checkFrom, gameObject.GetComponent<PlaceWaypointPlayer>().targetRoadEnd.transform.gameObject);        
        int lengthToUse = 20;
        if (targetVertices.Length < 20)
            lengthToUse = targetVertices.Length / 2;
        for (int i = -lengthToUse; i < lengthToUse; i += 2)//+=2 so we run up one side of the road mesh
        {
            int indexToUse = i + closestIndex;
            if (indexToUse < 0)
            {
                indexToUse += targetVertices.Length;
                // Debug.Log("Moved up for end");

                // Debug.Log("i = " + i.ToString());
                //  Debug.Log("index to use = " + indexToUse);
                //  Debug.Log("length = " + targetVertices.Length);

            }
            else if (indexToUse >= targetVertices.Length)
            {
                indexToUse -= targetVertices.Length;
                //   Debug.Log("Moved down for end");
                //   Debug.Log("i = " + i.ToString());
                //   Debug.Log("index to use = " + indexToUse);
                //   Debug.Log("length = " + targetVertices.Length);
            }

            Vector3 meshPoint = targetVertices[indexToUse];


            if (Vector3.Distance(meshPoint, targetVertices[closestIndex]) < roadWidth * 1.5f)//?
            {
                nearByIndexes.Add(indexToUse);
            }
        }

        //nearByIndexes.Add(nearByIndexes[nearByIndexes.Count - 1]+2);

        List<Vector3> endVertices = new List<Vector3>();
        List<int> endTriangles = new List<int>();

        //switch round, if enetering from one side of the road
        if (nearByIndexes[0] < nearByIndexes[1])
            nearByIndexes.Reverse();

        for (int i = 0; i < nearByIndexes.Count - 1; i++)
        {


            Vector3 p0 = targetVertices[nearByIndexes[i]];
            int nextIndex = i + 1;
            if (nextIndex > targetVertices.Length)
                nextIndex -= targetVertices.Length;

            Vector3 p1 = targetVertices[nearByIndexes[nextIndex]];
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p0;
            c.name = "p0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p1;
            c.name = "p1";
            */

            //Grab last two vertices from main road
          //  Vector3[] mainVertices = gameObject.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertices;//passing from voronoi builders
            Vector3 start = mainVertices[mainVertices.Length - 2];
            Vector3 end = mainVertices[mainVertices.Length - 1];
            
            Vector3 r0 = start + ((end - start) / (nearByIndexes.Count - 1)) * i;
            Vector3 r1 = start + ((end - start) / (nearByIndexes.Count - 1)) * (i + 1);
            /*
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r0;
            c.name = "r0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r1;
            c.name = "r1";
            */
            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r1);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

        }

        Mesh endMesh = new Mesh();
        endMesh.vertices = endVertices.ToArray();
        endMesh.triangles = endTriangles.ToArray();


        GameObject child0 = new GameObject();
        child0.name = "RoadEnd";
        child0.transform.position = gameObject.transform.position;

        child0.transform.parent = gameObject.transform;
        child0.layer = 9;
      //  child0.tag = "Road";

        endMesh.name = "End Mesh";
        MeshFilter endMeshFilter = child0.gameObject.AddComponent<MeshFilter>();
        MeshRenderer endMeshRenderer = child0.gameObject.AddComponent<MeshRenderer>();
        endMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        endMesh.RecalculateBounds();
        endMesh.RecalculateNormals();

        MeshCollider endMeshCollider = child0.gameObject.AddComponent<MeshCollider>();
        endMeshCollider.sharedMesh = endMesh;
        endMeshFilter.sharedMesh = endMesh;

        Material endNewMat = Resources.Load("Brown", typeof(Material)) as Material;
        endMeshRenderer.material = endNewMat;

    }

    public static void JoiningMeshStart(GameObject gameObject, Vector3[] targetVertices, List<Vector3> roadPointsForCell, int closestIndex, Vector3[] mainVertices)
    {

        float roadWidth = 2f;//2 is global?
        List<int> nearByIndexes = new List<int>();
      //  Vector3[] targetVertices = gameObject.GetComponent<PlaceWaypointPlayer>().targetRoadStart.GetComponent<MeshFilter>().mesh.vertices;
        // Debug.Log(targetVertices.Length);

        //find closest points to each end of road we are building
        Vector3 closest1 = Vector3.zero;
        Vector3 closest2 = Vector3.zero;
       // List<Vector3> roadPointsForCell = gameObject.GetComponent<PlaceWaypointPlayer>().roadPointsForCell;
        //direction towards second  point from 1st point * roadwidth
        Vector3 checkFrom = (roadPointsForCell[1] - roadPointsForCell[0]).normalized * 3f + roadPointsForCell[0];
       // int closestIndex = gameObject.GetComponent<PlaceWaypointPlayer>().closestIndexOnMeshStart;
        
        int lengthToUse = 20;
        if (targetVertices.Length < 20)
            lengthToUse = targetVertices.Length / 2;
        for (int i = -lengthToUse; i < lengthToUse; i += 2)//+=2 so we run up one side of the road mesh
        {
            int indexToUse = i + closestIndex;
            if (indexToUse < 0)
            {
                indexToUse += targetVertices.Length;
                 Debug.Log("Moved up for end");

                // Debug.Log("i = " + i.ToString());
                //  Debug.Log("index to use = " + indexToUse);
                //  Debug.Log("length = " + targetVertices.Length);

            }
            else if (indexToUse >= targetVertices.Length)
            {
                indexToUse -= targetVertices.Length;
                   Debug.Log("Moved down for end");
                //   Debug.Log("i = " + i.ToString());
                //   Debug.Log("index to use = " + indexToUse);
                //   Debug.Log("length = " + targetVertices.Length);
            }

            Vector3 meshPoint = targetVertices[indexToUse];


            if (Vector3.Distance(meshPoint, targetVertices[closestIndex]) < roadWidth*2)//?
            {
                nearByIndexes.Add(indexToUse);
            }
        }

        //nearByIndexes.Add(nearByIndexes[nearByIndexes.Count - 1]+2);

        List<Vector3> endVertices = new List<Vector3>();
        List<int> endTriangles = new List<int>();

        //switch round, this depends on what side of the road we attach to- sometimes not working...junction spun
        if(nearByIndexes[0] < nearByIndexes[1])
            nearByIndexes.Reverse();

        for (int i = 0; i < nearByIndexes.Count - 1; i++)
        {
            Vector3 p0 = targetVertices[nearByIndexes[i]];
            int nextIndex = i + 1;
            if (nextIndex > targetVertices.Length)
                nextIndex -= targetVertices.Length;

            Vector3 p1 = targetVertices[nearByIndexes[nextIndex]];
            /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p0;
            c.name = "p0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = p1;
            c.name = "p1";
            */

            //Grab last two vertices from main road
           // Vector3[] mainVertices = gameObject.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertices;
            Vector3 start = mainVertices[1];
            Vector3 end = mainVertices[0];

            Vector3 r0 = start + ((end - start) / (nearByIndexes.Count - 1)) * i;
            Vector3 r1 = start + ((end - start) / (nearByIndexes.Count - 1)) * (i + 1);
            /*
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r0;
            c.name = "r0";
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = r1;
            c.name = "r1";
            */
            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p0);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(p1);
            endTriangles.Add(endVertices.Count - 1);
            endVertices.Add(r1);
            endTriangles.Add(endVertices.Count - 1);

            endVertices.Add(r0);
            endTriangles.Add(endVertices.Count - 1);

        }

        Mesh endMesh = new Mesh();
        endMesh.vertices = endVertices.ToArray();
        endMesh.triangles = endTriangles.ToArray();


        GameObject child0 = new GameObject();
        child0.name = "RoadStart";
        child0.transform.position = gameObject.transform.position;

        child0.transform.parent = gameObject.transform;
        child0.layer = 9;
       // child0.tag = "Road";

        endMesh.name = "Start Mesh";
        MeshFilter endMeshFilter = child0.gameObject.AddComponent<MeshFilter>();
        MeshRenderer endMeshRenderer = child0.gameObject.AddComponent<MeshRenderer>();
        endMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        endMesh.RecalculateBounds();
        endMesh.RecalculateNormals();

        MeshCollider endMeshCollider = child0.gameObject.AddComponent<MeshCollider>();
        endMeshCollider.sharedMesh = endMesh;
        endMeshFilter.sharedMesh = endMesh;

        Material endNewMat = Resources.Load("Brown", typeof(Material)) as Material;
        endMeshRenderer.material = endNewMat;

    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.001f && crossVec1and2.sqrMagnitude > 0.001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

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

        else
        {
            return false;
        }
    }

    void AgentWaypoints()
    {
        GameObject agentsGO = GameObject.Find("Agents");
        agentsGO.GetComponent<WaypointInserter>().waypointlistListM.Add(waypointListM);
        agentsGO.GetComponent<WaypointInserter>().waypointlistListL.Add(waypointListL);
        agentsGO.GetComponent<WaypointInserter>().waypointlistListR.Add(waypointListR);
    }

	
    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b); // calculate angle
                                         // assume the sign of the cross product's Y component:
        return angle * Mathf.Sign(Vector3.Cross(a, b).y);
    }
}

