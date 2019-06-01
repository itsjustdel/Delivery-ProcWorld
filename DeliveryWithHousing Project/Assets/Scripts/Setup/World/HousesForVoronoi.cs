using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HousesForVoronoi : MonoBehaviour {

    //Instantiates houses along its cell's mesh and feeds position data to voronoi mesh script

    
    public BezierSpline updatedSpline;
	public Transform housePrefab;
	public List<AddToVoronoi.MeshAndVertice> voronoiList;
    public int roadDensity = 10; // used to be 20. could use adaptive function in check for bends? more points for bendier road
	public float roadSpreadL = 4f;
    public float roadSpreadR = 4f;
    public int houseSpread = 6;
    public int houseCellMinSize = 4;
    public int houseCellMaxSize = 6;
    public int gardenCentreLength = 10;//width of plot
    private int GardenCentreCellSize = 10;
    private int HouseCellSize = 10;
    
    private int housePlotLength = 20;
    private int uniformStepSize = 5;

    int spaceBetweenHouses = 200;
    private float roadWidth;
    public Vector3 start;
    public Vector3 end;

    public List<List<Vector3>> houseCellPositions = new List<List<Vector3>>();
    public List<Vector3> fieldCellPositionsL = new List<Vector3>();
    public List<Vector3> fieldCellPositionsR = new List<Vector3>();
    public List<List<Vector3>> gardenCentrePositions = new List<List<Vector3>>();

    void Awake()
	{
		//enabled =false;
	}
	void Start () {

        roadWidth = GetComponent<RingRoadMesh>().roadWidth;

        CreateUpdatedSpline();
        //#StartCoroutine("PlaceWithBezier");
       // StartCoroutine("CheckForBends");
        StartCoroutine("PlaceUniform");
	}

    void CreateUpdatedSpline()
    {
        /*
        //grab points from ring road mesh 
        updatedSpline = gameObject.AddComponent<BezierSpline>();

        updatedSpline.points = GetComponent<RingRoadMesh>().updatedSplinePoints.ToArray();
        updatedSpline.modes = GetComponent<RingRoadMesh>().updatedSplineModes.ToArray();

        */

        //updatedSpline = GetComponent<RingRoadMesh>().updatedSpline;
        updatedSpline = GetComponent<RingRoadMesh>().spline;
    }

    IEnumerator PlaceUniform()
    {
        //grab update bezier which ringRoadMesh populated and aboce function created
        BezierSpline spline = updatedSpline;

        //list which script will inject in to mesh generator script
        List<Vector3> pointsForVoronoi = GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().pointsForVoronoi;

        //house prefab
     //   GameObject housePrefab = Resources.Load("Prefabs/Housing/ProcHouse 1") as GameObject;

        //find out how long the road is
        float frequency = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount;
        //how quickly we will step thrgouh the curve
        float stepSize = frequency;
        stepSize = 1 / stepSize;
        int space = 10;

        //calculate start and end positions for junctions scripts
        start = spline.GetPoint(0 * stepSize);
        end = spline.GetPoint(frequency * stepSize);

        //add to voronoi here because it easy
        pointsForVoronoi.Add(start);
        pointsForVoronoi.Add(end);

        //move through the curve adding building
        //housing vars
        bool placeBuilding = false;
        int houseCounter = 0; //used to count ho wmany cells we make the house plot
        int houseCellSize = Random.Range(houseCellMinSize, houseCellMaxSize);
        // Random.Range(50, 200);
        int spaceBetweenHousesCounter = 0;//used to count space between houses
        int side = Random.Range(0, 2);
        
        
        List<Vector3> fieldPoints = new List<Vector3>();

        //variable to remember what type of plot we are building
        bool gardenCentre = false;  //not using 
        bool house = true; //forcing this true, so 1st building type is always house

        Vector3 prevPos = Vector3.zero;
        for (float i = 0; i < frequency;)//  moving on manually at the bottom of the loop i+= 5)
        {



            //use the bezier class to extract point from the curve i times along
            Vector3 position = spline.GetPoint(i * stepSize);

            //stops adding too many points to the voronnoi mesh, if points are too numerous/close, it crashes
            if (prevPos != Vector3.zero)
            {
                if (Vector3.Distance(position, prevPos) < 0.5f)
                {
                    i += uniformStepSize;
                    continue;
                }
            }
            prevPos = position;
            
            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 direction = spline.GetDirection((i) * stepSize);


            //add anchor if not loop road

            if (!GetComponent<RingRoadMesh>().firstRing)
            {
                //dont go too close to end or start, makes junction too complicated
                if (i > frequency / 2)
                    if (Vector3.Distance(position, spline.GetPoint(frequency * stepSize)) < 5f)                    
                        break;                    

                if (i < frequency / 2)
                    if (Vector3.Distance(position, spline.GetPoint(0)) < 5f)
                    {
                        i++;
                        continue;
                    }

                #region AnchorAtStart

                if (i == 0)
                {
                    //drop a point before the road to influence the voronoi to create a symmetrical pattern around the junction

                    Vector3 normalizedDir = direction.normalized;

                    for (int j = 0; j < 1; j += 20) //j is 0 //cut code for creating a sphere. for loop only goes once
                    {
                        Vector3 point = position - (normalizedDir * space);
                        Vector3 pivot = position;
                        Vector3 dir = point - pivot;
                        dir = Quaternion.Euler(0f, j, 0f) * dir;


                        Vector3 anchor = pivot + dir;
                        Vector3 anchor2 = pivot + (dir * 4); //randomise
                        Vector3 anchor3 = pivot + (dir * 8);
                        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //    cube.transform.position = anchor;
                        //     cube.name = "Anchor Start";
                        pointsForVoronoi.Add(anchor);
                        pointsForVoronoi.Add(anchor2);
                        pointsForVoronoi.Add(anchor3);
                    }


                    i += uniformStepSize;
                    continue;
                }

            }

            #endregion
        
            #region Junction

            //leave space for junction. 
            /*
            RaycastHit hitJunction;//looking for junciton sphere trigger which is placed in ringroadlist
            if (Physics.Raycast(position + (Vector3.up * 1000), Vector3.down, out hitJunction, 2000f, LayerMask.GetMask("Junction"), QueryTriggerInteraction.Collide))
            {
                i += uniformStepSize;
                continue;
            }
            */
            #endregion

            #region MainBody
            //fill in the body of the road
            
            //if out space counter has reached the preset house space variable, place a building
            if (spaceBetweenHousesCounter >= spaceBetweenHouses)
                placeBuilding = true;

       

            Vector3 forward = spline.GetPoint((i + 1) * stepSize);
           // Vector3 backward = spline.GetPoint((i - 1) * stepSize);

            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction.normalized;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction.normalized;

            //randomise width --can break gridding of plots/cells?
          //  roadSpreadL += Random.Range(-0.5f, 0.5f);
          //  roadSpreadL = Mathf.Clamp(roadSpreadL, 5f, 7f);
            Vector3 rotRoad1 = rot1 * (roadWidth + roadSpreadL);// * previousRandom1;

            //randomise width
           // roadSpreadR += Random.Range(-0.5f, 0.5f);
           // roadSpreadR = Mathf.Clamp(roadSpreadR,5f, 7f);
            Vector3 rotRoad2 = rot2 * (roadWidth + roadSpreadR);
            
      //      Vector3 rotRoad3 = rot1 * roadSpread * 5;// Random.Range(5f,15f);//radomise
      //      Vector3 rotRoad4 = rot2 * roadSpread * Random.Range(5f, 15f);


            Vector3 offsetRoad1 = rotRoad1 + position;
            Vector3 offsetRoad2 = rotRoad2 + position;
      
            pointsForVoronoi.Add(position);

            pointsForVoronoi.Add(offsetRoad1);
            pointsForVoronoi.Add(offsetRoad2);

          
            
            #endregion

            #region Buildings
            //house placement

            //Debug.Log("Place Building = " + placeBuilding);

            if (placeBuilding)
            {

                

                #region House
                if (house)
                {
                    //add this point to a list, from which we will match from to add the cells to a house plot in Fields.cs
                    
                    Vector3 thisSideRot = rot1;
                    if (side == 1)
                        thisSideRot = rot2;

                    

                    //create end point
                    Vector3 endPoint = Vector3.zero;
                    float nextIndex = i + 1f;
                    while(Vector3.Distance(position,spline.GetPoint(nextIndex*stepSize)) < 20f)
                    {

                        endPoint = spline.GetPoint(nextIndex * stepSize);
                        nextIndex += 1f;
                        

                        if (nextIndex > i + 100f)
                        {
                            Debug.Log("safety"); //??
                            break;
                        }
                    }

                    //create starting point
                    float vergeSpace = 10.5f;
                    //get centre point and work our rotation from there
                    float centreIndex = i +( (nextIndex - i) / 2);
                    Vector3 centrePoint = spline.GetPoint(centreIndex * stepSize);
                    Vector3 rotFromCentrePoint = spline.GetDirection(centreIndex * stepSize);
                    if (side == 0)
                        rotFromCentrePoint = Quaternion.Euler(0, -90, 0) * rotFromCentrePoint;
                    else if(side ==1)
                        rotFromCentrePoint = Quaternion.Euler(0, 90, 0) * rotFromCentrePoint;
                    
                    //move back from centre 
                    Vector3 startingPoint = centrePoint + (rotFromCentrePoint * vergeSpace);
                    //move towards start
                    Vector3 dir1 = (endPoint - position).normalized;


                    startingPoint -= dir1 * (housePlotLength*0.5f);
                    
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = position;
                    c.name = "Start of plot";
                    c.transform.localScale *= 10;

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = startingPoint;
                    c.name = "starting point";
                    c.transform.localScale *= 5;

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = endPoint;
                    c.name = "End of plot";
                    c.transform.localScale *= 10;
                    
                    //we need to check if this chosen postiion will hit a road
                    Vector3 dir2 = Quaternion.Euler(0, -90, 0) * dir1.normalized;
                    if (side == 1)
                        dir2 = Quaternion.Euler(0, 90, 0) * dir1.normalized;

                    //plot points
                    List<Vector3> tempVoronoiPoints = new List<Vector3>();
                    List<Vector3> plotPoints = new List<Vector3>();
                    bool skip = false;
                    for (float l = 0; l <= housePlotLength; l += 5f)//5 = houseplotlength/4?
                    {
                        for (float p = 0; p <= housePlotLength; p += 5f)
                        {

                            Vector3 pos = startingPoint + (dir1 * l) + (dir2 * p);
                            RaycastHit hit;
                            if (Physics.SphereCast(pos + Vector3.up*10, 5f/2, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
                            {
                               // GameObject c1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                               // c1.transform.position = hit.point;
                              //  c1.name = "house sphere hit";

                                //bad spot for house, overlaps a road, jump out
                                l = Mathf.Infinity;
                                //reset flags
                                placeBuilding = false;
                                spaceBetweenHousesCounter = 0;
                                side = Random.Range(0, 2);
                                
                                skip = true;

                                
                                break;    
                            }

                            tempVoronoiPoints.Add(pos);

                            //dont add border/outside points, we use them for the field
                            if((l>0 && l < housePlotLength) && (p>0 && p < housePlotLength))
                                plotPoints.Add(pos);
                            //add border to fields
                            else if((l==0 && p==0) || (l==housePlotLength && p==0) || p > 0)
                            {
                                if (side == 0)
                                    fieldCellPositionsL.Add(pos);
                                else if(side ==1)
                                    fieldCellPositionsR.Add(pos);
                            }
                            
                        }
                    }

                    //last 4 need reversed so it flows around house plot in order//
                    List<Vector3> listToUse = new List<Vector3>(fieldCellPositionsL);
                    if (side == 1)
                        listToUse = new List<Vector3>(fieldCellPositionsR);

                    List<Vector3> toReverse = new List<Vector3>();
                    for (int a = listToUse.Count - 1; a > listToUse.Count - 6; a--) //this will need reviewed when house plot size changes, 20/4=5
                    {
                        toReverse.Add(listToUse[a]);                        
                    }
                    
                    listToUse.RemoveRange(listToUse.Count - 5, 5);
                    foreach (Vector3 v3 in toReverse)
                        listToUse.Add(v3);
                    if (side == 0)
                        fieldCellPositionsL = new List<Vector3>(listToUse);
                    else
                        fieldCellPositionsR = new List<Vector3>(listToUse);

                    if (skip)
                    {
                        //push loop on
                        i += uniformStepSize;

                        //add to field positions
                        if (side == 1)
                        {
                            //save field position for opposite of road
                            fieldCellPositionsL.Add(offsetRoad1);

                            // c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            ////  c.transform.position = offsetRoad1;
                           //   c.name = "test 1";
                           //   c.transform.localScale *= 1;

                        }
                        else if (side == 0)
                        {
                            fieldCellPositionsR.Add(offsetRoad2);


                          //   c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                           //  c.transform.position = offsetRoad2;
                           //  c.name = "test 2";
                          //   c.transform.localScale *= 1;

                        }

                        continue;
                    }

                    //if we get here, add the temp points ( the border )
                    foreach (Vector3 v3 in tempVoronoiPoints)
                        pointsForVoronoi.Add(v3);
                   
                    //we also need to add the road and it's verge up til this nextindex 
                    for (float a = i; a <= nextIndex; a+=uniformStepSize)
                    {
                        Vector3 p0 = spline.GetPoint(a * stepSize);
                        Vector3 d0 = spline.GetDirection(a * stepSize);
                        
                        rot1 = Quaternion.Euler(0, -90, 0) * d0.normalized;
                        rot2 = Quaternion.Euler(0, 90, 0) * d0.normalized;
                        rotRoad1 = rot1 * (roadWidth + roadSpreadL);
                        rotRoad2 = rot2 * (roadWidth + roadSpreadR);
                        
                        offsetRoad1 = rotRoad1 + p0;
                        offsetRoad2 = rotRoad2 + p0;

                        pointsForVoronoi.Add(p0);
                        pointsForVoronoi.Add(offsetRoad1);
                        pointsForVoronoi.Add(offsetRoad2);
                        if (side == 0)
                        {
                            //save field position for opposite of road
                            fieldCellPositionsR.Add(offsetRoad2);                            
                            
                          // GameObject  c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                          //  c.transform.position = offsetRoad2;
                          //  c.name = "field 2 house plot";
                          //  c.transform.localScale *= 1;
                            
                        }
                        else if (side == 1)
                        {
                            fieldCellPositionsL.Add(offsetRoad1);
                            

                           // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                           // c.transform.position = offsetRoad1;
                           // c.name = "field 1 house plt";
                           // c.transform.localScale *= 1;
                            
                        }

                        /*
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.position = p0;
                        c.name = "pos";
                        c.transform.localScale *= 1;
                        */
                       

                    
                        
                    }
                    //jump to end of plot, dont build over it again
                    i = nextIndex;
                    //reset flags
                    placeBuilding = false;
                    spaceBetweenHousesCounter = 0;
                    side = Random.Range(0, 2);
                    //add this house plot to a list of all house plots for this road
                    houseCellPositions.Add(plotPoints);
                    
                }
                #endregion
            }

            //+ the for loop manually, depending on how detailed we need the grid

            if (placeBuilding)
            {
                if (gardenCentre)
                    i += GardenCentreCellSize;
                else if (house)
                    i += houseCellSize;

                //reset space between house counter
                spaceBetweenHousesCounter = 0;
            }
            else if (!placeBuilding)
            {
                
                    fieldCellPositionsL.Add(offsetRoad1);
                    fieldCellPositionsR.Add(offsetRoad2);

                /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = offsetRoad1;
                c.name = "field 1 ";
                c.transform.localScale *= 1;


                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = offsetRoad2;
                c.name = "field 2 ";
                c.transform.localScale *= 1;
                */
                i += uniformStepSize;

                //if we aren't placeing a building, count how far away the last building was
                spaceBetweenHousesCounter += uniformStepSize;
                side = Random.Range(0, 2);
            }
            #endregion
        }

        foreach(Vector3 v3 in fieldCellPositionsL)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = v3;
            c.name = "field L";
        }

        foreach (Vector3 v3 in fieldCellPositionsR)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = v3;
            c.name = "field R";
        }

        #region AnchorAtEnd
        //add anchor at the end
        if (!GetComponent<RingRoadMesh>().firstRing)
        {
            //use the bezier class to extract point from the curve i times along
            Vector3 positionEnd = spline.GetPoint(frequency * stepSize);
            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 directionEnd = spline.GetDirection((frequency) * stepSize);
            Vector3 normalizedDirEnd = directionEnd.normalized;

            Vector3 anchorEnd = positionEnd + (normalizedDirEnd * space);
            Vector3 anchorEnd2 = positionEnd + (normalizedDirEnd * space * 4); //4 can be randomised creating diff sizes probs
            Vector3 anchorEnd3 = positionEnd + (normalizedDirEnd * space * 8);
            //GameObject cubeEnd = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cubeEnd.transform.position = anchorEnd;
            //cubeEnd.name = "Anchor End";
            pointsForVoronoi.Add(anchorEnd);
            pointsForVoronoi.Add(anchorEnd2);
            pointsForVoronoi.Add(anchorEnd3);
        }
        #endregion
        yield break;
    }

    IEnumerator CheckForBends()
    {

        //grab update bezier which ringRoadMesh populated and aboce function created
        BezierSpline spline = updatedSpline;

        //list which script will inject in to mesh generator script
        List<Vector3> pointsForVoronoi = GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().pointsForVoronoi;

        //house prefab
        GameObject housePrefab = Resources.Load("Prefabs/Housing/ProcHouse 1") as GameObject;

        //find out how long the road is
        float frequency = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount ;
        //how quickly we will step thrgouh the curve
        float stepSize = frequency;
        stepSize = 1 / stepSize;

        List<float> pointsForAverage = new List<float>();
        List<Vector3> lastPoints = new List<Vector3>();
        float placeLimit = 0f;
        //move through the curve adding building
        for (float i = 0; i < frequency; i++)
        {
         
            //use the bezier class to extract point from the curve i times along
            Vector3 position = spline.GetPoint(i * stepSize);

            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 direction = spline.GetDirection((i) * stepSize);

            Vector3 forward = spline.GetPoint((i + 1) * stepSize);
            Vector3 backward = spline.GetPoint((i - 1) * stepSize);

            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;

            Vector3 rotRoad1 = rot1 * (roadWidth+ roadSpreadL);
            Vector3 rotRoad2 = rot2 * (roadWidth + roadSpreadR);

            Vector3 offsetRoad1 = rotRoad1 + position;
            Vector3 offsetRoad2 = rotRoad2 + position;
            
            float angle = calculateAngle(backward.x, backward.z, position.x, position.z, forward.x, forward.z);

            //make angle a positive value - we do not need to know if it is going left or right at the moment
            angle = Mathf.Abs(angle);

           // Debug.Log(angle);


            placeLimit += angle;

         

            if (placeLimit > 3f)
            {
                    
           /*     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.name = "Angle check";
            */
                placeLimit = 0f;
            
                pointsForVoronoi.Add(position);
                pointsForVoronoi.Add(offsetRoad1);
                pointsForVoronoi.Add(offsetRoad2);
                
            }

            lastPoints.Clear();
            pointsForAverage.Clear();
            
            


            
          //  yield return new WaitForEndOfFrame();

            
        }
        yield break;
    }

    public static float calculateAngle(float P1X, float P1Y, float P2X, float P2Y, float P3X, float P3Y)
    {
        float numerator = P2Y * (P1X - P3X) + P1Y * (P3X - P2X) + P3Y * (P2X - P1X);
        float denominator = 1 + (P2Y - P1Y) * (P1Y - P3Y);
        float ratio = numerator / denominator;

        float angleRad = Mathf.Atan(ratio);
        float angleDeg = (angleRad * 180) / Mathf.PI;

        if (angleDeg < 0)
        {
         //   angleDeg = 180 + angleDeg;
        }

        return angleDeg;

    }
        //uses the curve which built the road to place houses. Feeds Voronoi too
    IEnumerator PlaceWithBezier()
    {
        //grab bezier which ringRoadMesh script has already created
       // BezierSpline spline = GetComponent<RingRoadMesh>().spline;
        BezierSpline spline = updatedSpline;

        //list which script will inject in to mesh generator script
        List<Vector3> pointsForVoronoi = GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().pointsForVoronoi;

        //house prefab
        GameObject housePrefab = Resources.Load("Prefabs/Housing/ProcHouse 1") as GameObject;

        //find out how long the road is
        float frequency = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount;
        //how quickly we will step thrgouh the curve
        float stepSize = frequency;
        stepSize = 1 / stepSize;

        //move through the curve adding building
        for (int i = 0; i < frequency;)
        {

            //skip the very start and the very end of the curve to avoid placing house on adjoining road
            if (i < 5 || i > frequency - 5)
            {
                i++;
                continue;
            }
                

            //use the bezier class to extract point from the curve i times along
            Vector3 position = spline.GetPoint(i * stepSize);
            
            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 direction = spline.GetDirection((i) * stepSize);

            

            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;

            Vector3 rotHouse1 = rot1 * houseSpread;
            Vector3 rotHouse2 = rot2 * houseSpread;

            Vector3 offsetHouse1 = rotHouse1 + position;
            Vector3 offsetHouse2 = rotHouse2 + position;

            Vector3 rotRoad1 = rot1 * (roadWidth + roadSpreadL);
            Vector3 rotRoad2 = rot2 * (roadWidth + roadSpreadR);

            Vector3 offsetRoad1 = rotRoad1 + position;
            Vector3 offsetRoad2 = rotRoad2 + position;

            //decide whether we add a house here or just normal road points

            //towards the start and the end of the road (the first and last tenth, make the chance of house a higher as this will be near a junction
            float firstTenth = (frequency/10);
            //firstTenth *= 2;
            float lastTenth = frequency - firstTenth;
            //first tenth
            if(i < firstTenth || i > lastTenth)
            {
                pointsForVoronoi.Add(position);
                //add this point to voronoi mesh
                pointsForVoronoi.Add(offsetHouse1);
                //place point across the road to to influence the voronoi
                pointsForVoronoi.Add(offsetHouse2);

                
                //randomise which side it is instantiated on
                int side = Random.Range(0, 3);//minimum is inclusive, max is exclusive (won't ever return 2)
                if (side == 0)
                {
                    //instantiate house                
                    GameObject house = Instantiate(housePrefab);
                    house.transform.position = offsetHouse1;
                    //face the centre of the road
                    house.transform.LookAt(position);
                }
                else if (side == 1)
                {
                    //instantiate house                
                    GameObject house = Instantiate(housePrefab);
                    house.transform.position = offsetHouse2;
                    //face the centre of the road
                    house.transform.LookAt(position);
                }
                else if (side == 2)
                {
                    //instantiate house                
                    GameObject house = Instantiate(housePrefab);
                    house.transform.position = offsetHouse1;
                    //face the centre of the road
                    house.transform.LookAt(position);
                    
                    //make another one face it
                    GameObject house2 = Instantiate(housePrefab);
                    house2.transform.position = offsetHouse2;
                    house2.transform.LookAt(position);
                }



                //decide how far up to place the next house
                int randomInt = Random.Range(10,50);
                
                i += randomInt;
                continue;
            }
           
            //middle section
            else
            {
                //road points
                pointsForVoronoi.Add(position);
                pointsForVoronoi.Add(offsetRoad1);
                pointsForVoronoi.Add(offsetRoad2);
                i += roadDensity;
                continue;
            }            
        }

        //give list back to voronoi
// GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().meshAndVerticeList = voronoiList;

        yield break;
    }

    IEnumerator PlaceWithMeshPoints()
    {
        //load prefab from folder
        Transform cube = Resources.Load("Cube", typeof(Transform)) as Transform;
        //grab current voronoi list
        voronoiList = GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().meshAndVerticeList;

        //listwhich addToVoronoi script will inject in to mesh generator script//could be put straight in to mesg generator
        //but want it all injecting from the same place
        List<Vector3> pointsForVoronoi = GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().pointsForVoronoi;

        housePrefab = Resources.Load("Prefabs/Housing/ProcHouse 1", typeof(Transform)) as Transform;

        Mesh mesh = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;


        for (int i = 0; i < mesh.vertexCount - 1; i++)
        {          

            //For the first part of the mesh, place houses at a high density
            if (i < 100)
            {
                //	int randomNumber = Random.Range(0,10);
                //	if(randomNumber < 1)
                //		break;
                Vector3 middle = Vector3.Lerp(mesh.vertices[i], mesh.vertices[i + 1], 0.5f);

                //Vector3 direction = mesh.vertices[i] - mesh.vertices[i + 1];
                Vector3 direction = middle - mesh.vertices[i];
                direction.Normalize();
                direction *= 7;

                Vector3 position = middle + direction;
                Transform house = Instantiate(housePrefab, position, Quaternion.identity) as Transform;
                house.name = "House";
                house.parent = this.transform;

                Vector3 closestPoint = FindClosestPointInMesh(house.position);

                Vector3 lookPos = closestPoint - house.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                //house.transform.FindChild("Cube").rotation = rotation;
                house.transform.rotation = rotation;
                //height
                RaycastHit hit;
                LayerMask myLayerMask = LayerMask.GetMask("Road");

                //create list object to give to voronoi
                ColliderCheck cc = house.Find("Cube").GetComponent<ColliderCheck>();
                cc.cell = this.gameObject;
                cc.ind = i;
                cc.mesh = mesh;
                /* //made a prefab
                                //create an object for waypoints to go in and place in front of house
                                GameObject waypoints = new GameObject();
                                waypoints.name = "Waypoints";
                                waypoints.tag = "WaypointsHouse";
                                waypoints.layer = 22;
                                waypoints.transform.parent = house.transform;
                                waypoints.transform.position = house.transform.position;
                                waypoints.transform.localPosition += rotation*(Vector3.forward*5);
                                //add waypoint node to object
                                WaypointNode waypointNode = waypoints.AddComponent<WaypointNode>();
                                waypointNode.position = waypoints.transform.position;
                                //add script to attach waypoints to the rest of the network once built
                                waypoints.AddComponent<WaypointsForHouses>();
                */

                i += 10; //made in to 11 because of i++ which means it jumps across the road
            }

            else if (i >= mesh.vertexCount - 100)
            {
                //		int randomNumber = Random.Range(0,10);
                //		if(randomNumber < 1)
                //			break;
                Vector3 middle = Vector3.Lerp(mesh.vertices[i], mesh.vertices[i + 1], 0.5f);

                //Vector3 direction = mesh.vertices[i] - mesh.vertices[i + 1];
                Vector3 direction = middle - mesh.vertices[i];
                direction.Normalize();
                direction *= 7;

                Vector3 position = middle + direction;
                Transform house = Instantiate(housePrefab, position, Quaternion.identity) as Transform;
                house.name = "House";
                house.parent = this.transform;

                Vector3 closestPoint = FindClosestPointInMesh(house.position);
                Vector3 lookPos = closestPoint - house.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                //house.transform.FindChild("Cube").rotation = rotation;
                house.transform.rotation = rotation;
                //height
                RaycastHit hit;
                LayerMask myLayerMask = LayerMask.GetMask("Road");
                //create list object to give to voronoi
                ColliderCheck cc = house.Find("Cube").GetComponent<ColliderCheck>();
                cc.cell = this.gameObject;
                cc.ind = i;
                cc.mesh = mesh;


                i += 10;
            }

            else
            {
                
               
                Vector3 middle = Vector3.Lerp(mesh.vertices[i], mesh.vertices[i + 1], 0.5f);

                
                Vector3 direction = middle - mesh.vertices[i];
                direction.Normalize();
                direction *= 7;

                Vector3 position = middle + direction;
                Transform house = Instantiate(housePrefab, position, Quaternion.identity) as Transform;
                house.name = "House";
                house.parent = this.transform;

                Vector3 closestPoint = FindClosestPointInMesh(house.position);

                Vector3 lookPos = closestPoint - house.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                house.transform.rotation = rotation;
                //house.transform.FindChild("Cube").rotation = rotation;
                //height
                RaycastHit hit;
                LayerMask myLayerMask = LayerMask.GetMask("Road");
                //create list object to give to voronoi
                ColliderCheck cc = house.Find("Cube").GetComponent<ColliderCheck>();
                cc.cell = this.gameObject;
                cc.ind = i;
                cc.mesh = mesh;             
                cc.townHouse = false;

                
                int previous = i;
                int randomNumber = Random.Range(0, 500);

                //randomNumber = 50;

                i += randomNumber;

                //fills the in between houses with road points to create fields
                for (int j = previous; j < previous + randomNumber; j += roadDensity)
                {
                    bool inRange = false;
                    if (j >= (previous + randomNumber) - 50 && j <= (previous + randomNumber) + 50)
                    {
                        inRange = true;
                    }

                    if (!inRange)
                    {
                        //		Transform item = Instantiate(cube) as Transform;
                        //		item.name = "Test Cube";
                        //		item.position = mesh.vertices[j+25];

                        int index = j + 200; //how wide the fields are
                        if (index > mesh.vertexCount)
                            index -= mesh.vertexCount;

                        Vector3 pos = mesh.vertices[index];//TODO can be out of range still
                        pointsForVoronoi.Add(pos);

                        Vector3 dirL = mesh.vertices[index] - mesh.vertices[index - 1];
                        dirL *= roadSpreadL;
                        Vector3 posL = dirL + pos;
                        pointsForVoronoi.Add(posL);

                        Vector3 dirR = mesh.vertices[index - 1] - mesh.vertices[index];
                        dirR *= roadSpreadR;
                        Vector3 posR = dirR + pos;
                        pointsForVoronoi.Add(posR);
                    }

                }
            }
          
        }

        //give list back to voronoi
        GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().meshAndVerticeList = voronoiList;

        yield break;
    }
    
    Vector3 FindClosestPointInMesh(Vector3 housePoint)
    {
        Vector3 closestPoint = Vector3.zero;
        Mesh mesh = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;

        //find nearest point on A road
        float minDistanceSqr = Mathf.Infinity;

        //create temp vertlist
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 diff = housePoint - vertices[i];
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestPoint = vertices[i];

            }

        }
        return closestPoint;
    }

}

