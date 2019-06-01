using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Fields : MonoBehaviour {


    //called from mesh generator - voronoi mesh
    //calls agents waypoint inserter


    public List<GameObject> cellsToAddStuffTo = new List<GameObject>();


    public int minFieldSize = 10;
    public int maxFieldSize = 50;
    public int tempFieldSize = 10;
    void Awake()
    {
        enabled = false;
    }

    GameObject[] roads;
    // Use this for initialization
    public void Start()
    {

        //cellsToAddStuffTo = new List<GameObject>();
        StartCoroutine("Houses2");
        
        
        
        //StartCoroutine("AddFields");//from houses2 - chain

    }

    IEnumerator Houses()
    {
        Material[] materials = GameObject.FindWithTag("Materials").GetComponent<Materials>().myMaterials;
        //find all roads
        roads = GameObject.FindGameObjectsWithTag("Road");

        //go through each road and "stamp" fields. Groups cells in to individual gameobjects

        for (int i = 0; i < roads.Length; i++)
        {
            //central spline that road is built from
            BezierSpline spline = roads[i].transform.parent.GetComponent<HousesForVoronoi>().updatedSpline;

            float frequency = roads[i].GetComponent<MeshFilter>().mesh.vertexCount;
            //   frequency = 20f;
            float stepSize = frequency;
            stepSize = 1f / (stepSize);

            //bool grabCell;
            for (int j = 0; j <= frequency; j++)
            {
                if (j == 0)
                    continue;

                //every 100
                if(j % Random.Range(50,500) ==0)
                {

                    Vector3 position = spline.GetPoint((j) * stepSize);
                    //use the function "GetDirection" - this gives us the normalized velocity at the given point
                    Vector3 direction = spline.GetDirection((j) * stepSize);

                    Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
                    Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;
                    rot1 *= 10;
                    rot2 *= 10;
                    Vector3 offset1 = rot1 + position;
                    Vector3 offset2 = rot2 + position;

                    RaycastHit[] hitsL = Physics.SphereCastAll(offset1 + (Vector3.up * 200), 10f, Vector3.down, 400f,LayerMask.GetMask("TerrainCell")); //10 is plot size
                    RaycastHit[] hitsR = Physics.SphereCastAll(offset2 + (Vector3.up * 200), 10f, Vector3.down, 400f,LayerMask.GetMask("TerrainCell"));

                    foreach(RaycastHit hit in hitsL)
                    {
                        if (hit.transform.GetComponent<PolygonTester>().hitRoad == true)
                            continue;
                        
                        hit.transform.gameObject.tag = "HouseCell";
                        hit.transform.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Path") as Material;

                       // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      //  cube.transform.position = hit.point;
                       // cube.transform.name = "HouseCube";
                    }

                    foreach (RaycastHit hit in hitsR)
                    {
                        if (hit.transform.GetComponent<PolygonTester>().hitRoad == true)
                            continue;

                        hit.transform.gameObject.tag = "HouseCell";
                        hit.transform.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Path") as Material;

                    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = hit.point;
                    //    cube.transform.name = "HouseCube";
                    }




                }
                
            }
        }
        StartCoroutine("AddFields");
        yield break;
    }
    IEnumerator Houses2()
    {
        Material[] materials = GameObject.FindWithTag("Materials").GetComponent<Materials>().myMaterials;
        //find all roads
        roads = GameObject.FindGameObjectsWithTag("Road");

        //go through each road and "stamp" fields. Groups cells in to individual gameobjects

        for (int i = 0; i < roads.Length; i++)
        {

            HousesForVoronoi hfv = roads[i].transform.parent.GetComponent<HousesForVoronoi>();

            #region Houses
            //position of houses have been saved before voronoi was created
            List<List<Vector3>> housePlots = hfv.houseCellPositions;
           // Debug.Log(housePlots.Count);

            RaycastHit hit;
            for (int j = 0; j < housePlots.Count; j++)
            {
                //create new game object to put all house cells in
                GameObject plot = new GameObject();
                plot.name = "House Plot"; 
                plot.transform.parent = roads[i].transform;

                /*
                //post office
                
                //make first plot in list
                if(i == 0 && j ==0)
                {
                    postOffice = true;
                    plot.name = "Post Office";
                }
                //not using post offics, force false
                postOffice = false;
                */
                bool postOffice = false;
                List<Vector3> plotPoints = housePlots[j];
                
                for (int k = 0; k < plotPoints.Count; k++)
                {
                    
                   // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    ////cube.transform.position = plotPoints[k];
                  //  cube.transform.name = "HouseAttempt";
                    
                    if (Physics.Raycast(plotPoints[k] + Vector3.up*10, Vector3.down, out hit, 20f, LayerMask.GetMask("TerrainCell")))
                    {
                        //add to plot
                        if (postOffice)
                        {

                            hit.transform.parent = plot.transform;
                            hit.transform.gameObject.tag = "PostOffice";
                            hit.transform.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Path") as Material;                           
                        }
                        else
                        {
                            hit.transform.parent = plot.transform;
                            hit.transform.gameObject.tag = "HouseCell";
                            hit.transform.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Green") as Material;
                        }
                        //disable collider, we will add one to the combined mesh

                        hit.transform.GetComponent<MeshCollider>().enabled = false;
                        /*
                        GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube1.transform.position = hit.point;
                        cube1.transform.name = "HouseCube";
                        */
                    }
                }

                
                //all cells now added to plot gameobject, add combine meshes to this now

                CombineChildren cc = plot.gameObject.AddComponent<CombineChildren>();
                if(postOffice)
                {
                    cc.postOffice = true;
                }
                else
                    cc.houseCell = true;

            }
            #endregion

            #region GardenCentre
            List<List<Vector3>> gardenCentrePlots = hfv.gardenCentrePositions;
            //RaycastHit hit;
            for (int j = 0; j < gardenCentrePlots.Count; j++)
            {
                //create new game object to put all house cells in
                GameObject plot = new GameObject();
                plot.name = "GardenCentrePlot";
                plot.transform.parent = roads[i].transform;
                
                List<Vector3> plotPoints = gardenCentrePlots[j];

                for (int k = 0; k < plotPoints.Count; k++)
                {
                    if (Physics.Raycast(plotPoints[k] + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("TerrainCell")))
                    {                      
                        hit.transform.parent = plot.transform;
                        hit.transform.gameObject.tag = "GardenCentreCell";
                        hit.transform.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Green") as Material;
                        //not for garden centre, needed in script  //disable collider, we will add one to the combined mesh
                        //hit.transform.GetComponent<MeshCollider>().enabled = false;
                    }
                }

                //all cells now added to plot gameobject, add combine meshes to this now
                CombineChildren cc = plot.gameObject.AddComponent<CombineChildren>();
                cc.gardenCentre = true;

            }
            #endregion
        }
        StartCoroutine("AddFields2");
        yield break;
    }
    IEnumerator AddFields()
    {
        //create tempfield size
        tempFieldSize = Random.Range(minFieldSize, maxFieldSize);

        Material[] materials = GameObject.FindWithTag("Materials").GetComponent<Materials>().myMaterials;
        //find all roads
        roads = GameObject.FindGameObjectsWithTag("Road");

        //go through each road and "stamp" fields. Groups cells in to individual gameobjects

        for (int i = 0; i <roads.Length; i++ )
        {
            //central spline that road is built from
            BezierSpline spline = roads[i].transform.parent.GetComponent<HousesForVoronoi>().updatedSpline;

            float frequency = roads[i].GetComponent<MeshFilter>().mesh.vertexCount*5;                     
            float stepSize = frequency;
            stepSize = 1f / (stepSize);

            //do twice, one for left side of road, one for right. Rotation is altered below
            for (int side = 0; side < 2; side++)
            {
                List<RaycastHit> hitsL = new List<RaycastHit>();
                //Now, run through the curve and add points to the temporary lists
                for (int j = 0; j <= frequency; j++)
                {
                    if (j % 200 == 0)
                        yield return new WaitForEndOfFrame();
                    //use the function "GetPoint" in the bezier class
                    Vector3 position = spline.GetPoint((j) * stepSize);
                    //use the function "GetDirection" - this gives us the normalized velocity at the given point
                    Vector3 direction = spline.GetDirection((j) * stepSize);

                    for (float k = 3; k < 30; k+=1f)//k = 5, to skip road and verge //fixxx
                    {
                        //create rotation for each side of road
                        Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
                        if(side == 0)
                        {
                            rot1 = Quaternion.Euler(0, 90, 0) * direction;
                        }

                        rot1 *= k;
                        Vector3 offset1 = rot1 + position;

                        RaycastHit hit;
                        if (Physics.Raycast(offset1 + Vector3.up * 100, Vector3.down, out hit, 200f, LayerMask.GetMask("Road", "TerrainCell","HouseCell")))//if (Physics.SphereCast(offset1 + Vector3.up * 100, 1f, Vector3.down, out hit, 200f, LayerMask.GetMask("Road", "TerrainCell","HouseCell")))
                        {
                            //force a new field
                            if (hit.transform.tag == "Road")
                            {
                                //if the road we hit isnt the road we are shooting from
                                //stop the current field
                                if (hit.transform != roads[i].transform)
                                {
                                    GameObject field = new GameObject();
                                    field.transform.parent = roads[i].transform.parent;
                                    field.name = "Field";

                                    int random = Random.Range(0, 4);

                                    for (int h = 0; h < hitsL.Count; h++)
                                    {
                                        hitsL[h].transform.GetComponent<MeshRenderer>().sharedMaterial = materials[random];
                                        hitsL[h].transform.GetComponent<MeshRenderer>().enabled = false;
                                        hitsL[h].transform.parent = field.transform;

                                    }
                                    cellsToAddStuffTo.Add(field);
                                    hitsL.Clear();
                                }
                                //if it is this road's collider, we are at a hairpin,
                                //stop this current branch and jump to the next step
                                else if(hit.transform == roads[i].transform)
                                {
                                    //don't add anything
                                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                    //cube.transform.position = hit.point;
                                    break;
                                }
                            }
                            else if(hit.transform.tag == "HouseCell" || hit.transform.tag == "PostOffice" || hit.transform.tag == "GardenCentreCell")
                            {
                                //go round edge of house cell and do spherecast looking for outlying cells, add these to the field                           

                                Vector3[] vertices = hit.transform.GetComponent<MeshFilter>().mesh.vertices;

                                for(int v = 0; v < vertices.Length; v++)
                                {
                                    //look for terrain cells
                                    RaycastHit sphereCast;
                                    if(Physics.SphereCast(vertices[v] + Vector3.up * 5,3f,Vector3.down,out sphereCast,10f,LayerMask.GetMask("TerrainCell")))
                                    {
                                        if(sphereCast.transform.GetComponent<PolygonTester>().hitRoad == false)
                                        {
                                            //add cell to hitlist
                                            hitsL.Add(sphereCast);

                                            //disable collider, dont need it now
                                            sphereCast.transform.GetComponent<MeshCollider>().enabled = false;
                                            sphereCast.transform.GetComponent<MeshRenderer>().enabled = false;
                                        }
                                    }
                                }
                            }

                            else if (hit.transform.GetComponent<PolygonTester>().hitRoad == false)
                            {
                                //add cell to hitlist
                                hitsL.Add(hit);

                                //disable collider, dont need it now
                                hit.transform.GetComponent<MeshCollider>().enabled = false;
                                hit.transform.GetComponent<MeshRenderer>().enabled = false;

                               //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                             //   cube.transform.position = hit.transform.GetComponent<MeshFilter>().mesh.vertices[0];
                            }
                        }
                        //it has not hit a road cell or a terrain cell. This happens when another field has laready diabled the cell's collider
                        //finish the field
                        else
                        {
                        }
                    }

                    
                    if (j % tempFieldSize == 0 || j >= frequency)
                    {
                        //make new field size for next field
                        tempFieldSize = Random.Range(minFieldSize, maxFieldSize);

                        GameObject field = new GameObject();
                        field.transform.parent = roads[i].transform.parent;
                        field.name = "Field";

                        int random = Random.Range(0, 4);

                        for (int k = 0; k < hitsL.Count; k++)
                        {
                            hitsL[k].transform.GetComponent<MeshRenderer>().sharedMaterial = materials[random];
                            hitsL[k].transform.parent = field.transform;
                        }

                        cellsToAddStuffTo.Add(field);
                        hitsL.Clear();
                    }
                }
            }

            StartCoroutine("AddToCells");

            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    IEnumerator AddFields2()
    {
        //create tempfield size
        //tempFieldSize = Random.Range(minFieldSize, maxFieldSize);

        //grab lists from house for voronoi- that script populated two list L and R, with points which were passed to the voronoi mesh generator to create cells with
        //we can gather these cells now they have been produced in to fields

        //first we must find the cells that match the field positions
        GameObject vMesh = GameObject.FindWithTag("VoronoiMesh");
        //we will use the first vertice in each mesh to check point for, this is the point where the voronoi cell is built from
        List<Vector3> centrePoints = new List<Vector3>();
        MeshFilter[] meshFilters = vMesh.GetComponentsInChildren<MeshFilter>();
        List<GameObject> matchParents = new List<GameObject>();
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.mesh.vertexCount == 0)
                continue;


            centrePoints.Add(mf.mesh.vertices[0]);
            //remembering parent, should make a class/struct
            matchParents.Add(mf.gameObject);
        }

        //find all roads
        roads = GameObject.FindGameObjectsWithTag("Road");
        float roadWidth = 2f; //should be a global somehwere
        
        for (int x = 0; x < roads.Length; x++)
        {
            //skip little joining meshes which i tagged as road too (untagged now)
            if (roads[x].name == "RoadStart" || roads[x].name == "RoadEnd")
                continue;

            for (int a = 0; a < 2; a++)
            {
                //two lists, one for each side
                List<Vector3> cellPoints = roads[x].transform.parent.GetComponent<HousesForVoronoi>().fieldCellPositionsL;
                if(a==1)
                    cellPoints = roads[x].transform.parent.GetComponent<HousesForVoronoi>().fieldCellPositionsR;

                List<GameObject> cellsToCombine = new List<GameObject>();
                for (int i = 0; i < cellPoints.Count; i++)
                {
                    //gather all cells in to one gameobject until we find a road
                    for (int j = 0; j < centrePoints.Count; j++)
                    {
                        if (Mathf.Abs(cellPoints[i].x - centrePoints[j].x) < 0.1f && Mathf.Abs(cellPoints[i].z - centrePoints[j].z) < 0.1f)
                        {
                            RaycastHit hit;
                            if (Physics.SphereCast(centrePoints[j] + Vector3.up*100,roadWidth,Vector3.down, out hit, 200f, LayerMask.GetMask("Road")) || cellsToCombine.Count > tempFieldSize)
                            {

                                if (cellsToCombine.Count > tempFieldSize)
                                    Debug.Log("cells > size");

                                //put all found cells so far in to a parent, we will combine all these cells in to one field object
                                if (cellsToCombine.Count != 0)
                                {
                                    GameObject field = new GameObject();
                                    field.transform.parent = roads[x].transform.parent;
                                    field.name = "Field";

                                    int random = Random.Range(0, 4);
                                    Material[] materials = GameObject.FindWithTag("Materials").GetComponent<Materials>().myMaterials;
                                    for (int k = 0; k < cellsToCombine.Count; k++)
                                    {
                                        cellsToCombine[k].transform.GetComponent<MeshRenderer>().sharedMaterial = materials[random];
                                        cellsToCombine[k].transform.parent = field.transform;
                                    }

                                    cellsToAddStuffTo.Add(field);

                                    //reset list
                                    cellsToCombine = new List<GameObject>();

                                    //was missing the next start, pop it back one so it checks properly
                                    j--;

                                    //make new field size
                                    tempFieldSize = Random.Range(minFieldSize, maxFieldSize);
                                }
                                else
                                {
                                    Debug.Log("cells > 0");
                                }

                                continue;
                            }
                            else
                            {
                                cellsToCombine.Add(matchParents[j].gameObject);
                            }
                        }
                    }
                }

                //put all found cells so far in to a parent, we will combine all these cells in to one field object
                if (cellsToCombine.Count != 0)
                {
                    GameObject field = new GameObject();
                    field.transform.parent = roads[x].transform.parent;
                    field.name = "Field";

                    int random = Random.Range(0, 4);
                    Material[] materials = GameObject.FindWithTag("Materials").GetComponent<Materials>().myMaterials;
                    for (int k = 0; k < cellsToCombine.Count; k++)
                    {
                        cellsToCombine[k].transform.GetComponent<MeshRenderer>().sharedMaterial = materials[random];
                        cellsToCombine[k].transform.parent = field.transform;
                    }

                    cellsToAddStuffTo.Add(field);

                    //make new field size
                    //create tempfield size
                    //tempFieldSize = Random.Range(minFieldSize, maxFieldSize);
                }
            }
        }

        StartCoroutine("AddToCells");

        yield break;
    }


    IEnumerator AddToCells()
    {
        for (int i = 0; i < cellsToAddStuffTo.Count; i++)
        {
            //the list contains duplciates, only add the script once
            if (cellsToAddStuffTo[i].GetComponent<CombineChildren>() == null)
            {
               CombineChildren combineChildren = cellsToAddStuffTo[i].AddComponent<CombineChildren>();
                combineChildren.fieldCell = true;
                

            }
        }

        //basic world is complete. Start populating with agents
       // GameObject.Find("Agents").GetComponent<WaypointInserter>().enabled = true;
        yield break;
    }
}
