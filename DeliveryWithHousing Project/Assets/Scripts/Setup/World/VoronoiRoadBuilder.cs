using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoronoiRoadBuilder : MonoBehaviour {

    public List<JunctionArea> junctions = new List<JunctionArea>();

    //this script creates roads using a voronoi mesh

    // Use this for initialization
    void Start ()
    {
        //get cells we wish to create a road layout from
        List<List<GameObject>> cells = GatherCells();

     //   Debug.Log("cells = " + cells.Count);
        //create layout
        RoadLayout(cells);

        //build road meshes
	}
	
    List<List<GameObject>> GatherCells()
    {
        List<List<GameObject>> cells = new List<List<GameObject>>();

        //use a big spherecast to touch all the cells
        int radius = 50;
        for (int i = 0; i <3; i++)
        {
            List<GameObject> cellsInner = new List<GameObject>();
            for (int j = 5; j < radius; j += 5)
            {

                float slice = 2 * Mathf.PI / 100;
                Vector2 center = new Vector2(radius * i*3, 0);//**
                for (int x = 0; x < 100; x++)
                {
                    float angle = slice * x;
                    int newX = (int)(center.x + j * Mathf.Cos(angle));
                    int newY = (int)(center.y + j * Mathf.Sin(angle));
                    Vector2 p = new Vector2(newX, newY);

                    //zero the y co ordinate and make a vector 3
                    Vector3 p3 = new Vector3(p.x, 0f, p.y);

                    //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   // c.transform.position = p3;

                    RaycastHit[] hits = Physics.SphereCastAll(p3 + Vector3.up * 500, j, Vector3.down, 1000f, LayerMask.GetMask("WorldPlan"));

                    foreach (RaycastHit hit in hits)
                    {
                        if (!cellsInner.Contains(hit.transform.gameObject))
                            if (hit.transform.tag == "Cell")
                                cellsInner.Add(hit.transform.gameObject);
                    }
                }

                 

                

                foreach (GameObject g in cellsInner)
                {
                    g.tag = "Countryside";

                    if (i == 0)
                        g.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                    if (i == 1)
                        g.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                    if (i == 2)
                        g.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("RosePink") as Material;

                    //g.GetComponent<MeshRenderer>().enabled = false;
                }
                
            }
            //Debug.Log(cellsInner.Count);
            cells.Add(cellsInner);

        }
        return cells;
    }

    void RoadLayout(List<List<GameObject>> cells)
    {
        //build a road around each cell - if we hit another road before we complete the cell, attach to that road

        //autoweld cells and find outside edge
        List<GameObject> combinedCells = new List<GameObject>();
        List<List<Vector3>> roadLayouts = new List<List<Vector3>>();
        //remember where we ahve biult our roads
        List<Vector3> usedPoints = new List<Vector3>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Count == 0)
                continue;

            //for each cell place pathfinder way points on each out vertice
            //combine cells in to one
            GameObject combined = Combine(cells[i]);
            combined.GetComponent<MeshRenderer>().enabled = false;
            //name first ring road for junctions
            if(i==0)
                combined.name = ("FirstRingRoad");
            combinedCells.Add(combined);
            //autoweld
            Mesh mesh = AutoWeld.AutoWeldFunction(combined.GetComponent<MeshFilter>().mesh, 0.001f, 1000);
            //get outer edge
            List<int> edgeIndexes = FindEdges.EdgeVertices(mesh, 0.0011f);

            //make v3 list from these
            List<Vector3> edgePoints = new List<Vector3>();
            foreach (int j in edgeIndexes)
                edgePoints.Add(mesh.vertices[j]);

            List<Vector3> listForMesh = new List<Vector3>();
            if (i == 0)
            {
                // RingRoadMesh rrm = combined.AddComponent<RingRoadMesh>();
                // rrm.ringRoadList = edgePoints;
                // rrm.firstRing = true;

                roadLayouts.Add(edgePoints);

                foreach (Vector3 v3 in edgePoints)
                    usedPoints.Add(v3);
            }
            else
            {
                List<Vector3> road1 = new List<Vector3>();
                List<Vector3> road2 = new List<Vector3>();
                //look for a shared point in usedPointsList                

                //make loop 
                edgePoints.Add(edgePoints[0]);
                //find shared edge
                int roadStart = 1000;
                for (int j = 0; j < edgePoints.Count - 1; j++)
                {
                    if (usedPoints.Contains(edgePoints[j]))
                    {
                        //check if next point is usedlist
                        if (usedPoints.Contains(edgePoints[j + 1]))
                        {

                        }
                        //if it isn't, road start
                        else
                        {
                            roadStart = j;
                        }
                    }



                }
                if (roadStart == 1000)
                    continue;

                List<Vector3> roadPoints = new List<Vector3>();
                /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = edgePoints[roadStart];
                c.name = "start";
                */

                roadPoints.Add(edgePoints[roadStart]);
                //now from start, find finish
                for (int j = roadStart + 1; j < edgePoints.Count + roadStart; j++)
                {
                    int indexToUse = j;
                    if (j > edgePoints.Count - 2)
                        indexToUse -= edgePoints.Count - 1;

                    if (usedPoints.Contains(edgePoints[indexToUse]))
                    {

                      //  c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      //  c.transform.position = edgePoints[indexToUse];
                      //  c.name = "end";

                        roadPoints.Add(edgePoints[indexToUse]);
                        break;
                    }
                    else
                    {
                       // c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      //  c.transform.position = edgePoints[indexToUse];
                      //  c.name = "road point";

                        usedPoints.Add(edgePoints[indexToUse]);

                        roadPoints.Add(edgePoints[indexToUse]);
                    }
                }

                //RingRoadMesh rrm = combined.AddComponent<RingRoadMesh>();
                //rrm.ringRoadList = roadPoints;

                roadLayouts.Add(roadPoints);
            }
        }

        //now we have the roads in seperate lists in the road layouts list
        //the vornonoi mesh can ahve some points too close togather so, let's run through these lists and weld points together with a threshold
        float threshold = 20f;

        for (int i = 0; i < roadLayouts.Count; i++)
        {
            for (int j = 0; j < roadLayouts[i].Count; j++)
            {
                for (int x = 0; x < roadLayouts.Count; x++)
                {
                    for (int y = 0; y < roadLayouts[x].Count; y++)
                    {
                        if (Vector3.Distance(roadLayouts[i][j], roadLayouts[x][y]) < threshold)
                        {
                            //second on to first
                            roadLayouts[x][y] = roadLayouts[i][j];
                        }
                    }
                }
            }
        }

        //remove duplicates in lists//can happen because we glued them
        for (int i = 0; i < roadLayouts.Count; i++)
        {
            List<Vector3> myList = roadLayouts[i];
            roadLayouts[i] = myList.Distinct().ToList();//linqy lazy
        }

        //we have worked this out on a flat plane, move the positions on the Y axis by raycasting looking for our terrain base

        for (int i = 0; i < roadLayouts.Count; i++)
        {
            for (int j = 0; j < roadLayouts[i].Count; j++)
            {
                RaycastHit hit;
                if (Physics.Raycast(roadLayouts[i][j] + Vector3.up * 1000, Vector3.down, out hit, 2000f, LayerMask.GetMask("TerrainBase")))
                {
                    roadLayouts[i][j] = hit.point;
                    
                }
                else
                    Debug.Log("no hit on voronoi road build");
            }
        }

        StartCoroutine(BuildRoads (roadLayouts,combinedCells));
    }

    IEnumerator BuildRoads(List<List<Vector3>> roadLayouts,List<GameObject> combinedCells)
    {

        List<GameObject> targetRoadsForEnd = new List<GameObject>();
        List<GameObject> targetRoadsForStart = new List<GameObject>();
        List<int> closestIndexesEnd = new List<int>();
        List<int> closestIndexesStart = new List<int>();
        //now send to mesh maker
        for (int i = 0; i < roadLayouts.Count; i++)
        {
            GameObject targetRoadForEnd = null;
            GameObject targetRoadForStart = null;
            int closestIndexOnTarget = 0;
            int closestIndexOnStart = 0;
            //move road ends
            if (i > 0)
            {
                roadLayouts[i] = AdjustStartAndEnd(out targetRoadForEnd,out targetRoadForStart, out closestIndexOnTarget,out closestIndexOnStart, roadLayouts[i], combinedCells[i]);
            }

            targetRoadsForEnd.Add(targetRoadForEnd);
            targetRoadsForStart.Add(targetRoadForStart);
            closestIndexesEnd.Add(closestIndexOnTarget);
            closestIndexesStart.Add(closestIndexOnStart);

            //build main mesh
            RingRoadMesh rrm = combinedCells[i].AddComponent<RingRoadMesh>();
            rrm.ringRoadList = roadLayouts[i];

            if (i == 0)
                rrm.firstRing = true;


            //we need to wait for each road to be built
            yield return new WaitForEndOfFrame();
        }

        //build junction joins
        for (int i = 0; i < roadLayouts.Count; i++)
        {

            if (i > 0)
            {
                //build mesh joing section between two roads - END
                Vector3[] targetVertices = targetRoadsForEnd[i].transform.gameObject.GetComponent<MeshFilter>().mesh.vertices;

                Vector3[] mainVertices = combinedCells[i].GetComponent<RingRoadMesh>().roadMeshGameObject.GetComponent<MeshFilter>().mesh.vertices;

                RingRoadMesh.JoiningMeshEnd(gameObject, targetVertices, roadLayouts[i], closestIndexesEnd[i], mainVertices);

                //START
                targetVertices = targetRoadsForStart[i].transform.gameObject.GetComponent<MeshFilter>().mesh.vertices;

                //mainVertices = combinedCells[i].GetComponent<RingRoadMesh>().roadMeshGameObject.GetComponent<MeshFilter>().mesh.vertices;

                RingRoadMesh.JoiningMeshStart(gameObject, targetVertices, roadLayouts[i], closestIndexesStart[i], mainVertices);

               // Debug.Log("closesindex start = " + closestIndexesStart[i]);
            }
        }

        /*
        for (int i = 0; i < roadLayouts.Count; i++)
        {

            Verge verge = combinedCells[i].AddComponent<Verge>();
            verge.forRingRoad = true;
            verge.frequency = roadLayouts[i].Count * 200;
        }
        */

        yield return new WaitForEndOfFrame();
        for (int i = 0; i < roadLayouts.Count; i++)
        {

            HousesForVoronoi hFV = combinedCells[i].AddComponent<HousesForVoronoi>();

            if (i == 0)
                continue;
        }
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < roadLayouts.Count; i++)
        {
            //add to list for enabling later once mesh gen has finished
            JunctionArea ja = combinedCells[i].AddComponent<JunctionArea>();
            junctions.Add(ja);
        }
        yield return new WaitForEndOfFrame();


        GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().Start();
        yield break;
    }

    List<Vector3> AdjustStartAndEnd(out GameObject targetRoadEnd , out GameObject targetRoadStart, out int closestIndexOnTargetEnd,out int closestIndexOnStart, List<Vector3> ringRoadPoints,GameObject thisCell)
    {
      

        Vector3 first = ringRoadPoints[0];

        RaycastHit[] hits = Physics.SphereCastAll(first + (Vector3.up * 1000), 100f, Vector3.down, 2000f, LayerMask.GetMask("Road")); //100 twice the hex size
                                                                                                                                        //

        List<Vector3> closestPoints = new List<Vector3>();
        List<GameObject> closestRoads = new List<GameObject>();
        int indexStart = 0;
        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag != "Road")
                continue;

            

            GameObject targetGameobject = h.transform.gameObject;
            //search for the closest point to the second one in the list. it is the first point we are moving
            Vector3 next;
            Vector3 closest;
            
            Vector3 closestMeshPointFirst = FindPointOnMesh(out closest,out next,out indexStart,ringRoadPoints[1], targetGameobject);

            //insert in to a list which is checked below
            closestPoints.Add(closestMeshPointFirst);
            closestRoads.Add(h.transform.gameObject);
        }

        float distance = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;
        targetRoadStart = null;
        for (int i = 0; i < closestPoints.Count; i++) // -1 to save the index for +1 for lerping below
        {
            float diff = Vector3.Distance(ringRoadPoints[1], closestPoints[i]);

            if (diff < distance)
            {
                closestPoint = closestPoints[i];
                distance = diff;
                targetRoadStart = closestRoads[i];
            }
        }

        if (closestPoints.Count != 0)
            ringRoadPoints[0] = closestPoint;


        Vector3 last = ringRoadPoints[ringRoadPoints.Count - 1];
        Vector3 secondLast= ringRoadPoints[ringRoadPoints.Count - 2];//was -1

        
        LayerMask lm = LayerMask.GetMask("Road");


        //find target road
        RaycastHit hit;
        //closesty index on target to retrun
        int indexEnd = 0;
        if (Physics.SphereCast(last + (Vector3.up * 1000), 10f, Vector3.down, out hit, 2000f, LayerMask.GetMask("Road")))
        {
            GameObject targetGameobject = hit.transform.gameObject;
            Vector3 nextMeshPoint;
            Vector3 meshPoint;
            
            Vector3 closestMeshPointLast = FindPointOnMesh(out meshPoint, out nextMeshPoint,out indexEnd, secondLast, targetGameobject);

            Vector3 roadDir = (nextMeshPoint - meshPoint).normalized;
            //spin the road running direction towards second last point
            //create points each side of the line - use "world" coordinates, because "centre" is a world co-ordinate
            Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
            Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
            Vector3 lookDir = Vector3.zero;
            //check which is closest - use that rotation to build wall
            if (Vector3.Distance(closestMeshPointLast + lookDir1, secondLast) > Vector3.Distance(closestMeshPointLast + lookDir2, secondLast))
                lookDir = Quaternion.Euler(0, -90, 0) * roadDir;    //save as local direction
            else
                lookDir = Quaternion.Euler(0, 90, 0) * roadDir; //save as local direction



            float distanceBetweeLastTwo = Vector3.Distance(ringRoadPoints[ringRoadPoints.Count - 1], ringRoadPoints[ringRoadPoints.Count - 2]);

            ringRoadPoints[ringRoadPoints.Count - 1] = closestMeshPointLast;// + lookDir * distanceBetweeLastTwo*0.5f;
                                                                            //ringRoadPoints.Add( closestMeshPointLast + lookDir * distanceBetweeLastTwo * 0.25f);
                                          
            /*// ringRoadPoints.Add( closestMeshPointLast);

            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = ringRoadPoints[ringRoadPoints.Count - 4];
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = ringRoadPoints[ringRoadPoints.Count - 3];
            // c.transform.localScale *= .5f;

            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = ringRoadPoints[ringRoadPoints.Count - 2];
            // c.transform.localScale *= .5f;
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = ringRoadPoints[ringRoadPoints.Count - 1];
            // c.transform.localScale *= .5f;
            */

        }

        //ringRoadPoints.Add(closest2);

        // }
        targetRoadEnd = hit.transform.gameObject;
        closestIndexOnStart = indexStart;
        closestIndexOnTargetEnd = indexEnd;
        return ringRoadPoints;
    }
    Vector3 FindPointOnMesh(out Vector3 closestPoint, out Vector3 nextPoint, out int index, Vector3 point, GameObject target)
    {

        Mesh mesh = target.GetComponent<MeshFilter>().mesh;

        float distance = Mathf.Infinity;
        
        nextPoint = Vector3.zero;
        closestPoint = Vector3.zero;
        index = 0;
        Vector3 midPoint = Vector3.zero;
        for (int i = 0; i < mesh.vertexCount - 2; i++) // -2 to save the index for +2 for lerping below
        {
            float diff = Vector3.Distance(point, mesh.vertices[i]);

            if (diff < distance)
            {
                midPoint = Vector3.Lerp(mesh.vertices[i], mesh.vertices[i + 1], 0.5f);
                distance = diff;
                //next point in line
                nextPoint = mesh.vertices[i + 2];
                closestPoint = mesh.vertices[i];
                index = i;
            }
        }
        
        return midPoint;
    }
    GameObject Combine(List<GameObject> cells)
    {
        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();

        List<MeshRenderer> listRenderers = new List<MeshRenderer>();
        foreach (GameObject cell in cells)
            listRenderers.Add(cell.GetComponent<MeshRenderer>());

        MeshRenderer[] meshRenderers = listRenderers.ToArray();

        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.sharedMaterials)
                if (material != null && !combines.ContainsKey(material))
                    combines.Add(material, new List<CombineInstance>());
        }

        List<MeshFilter> listFilters = new List<MeshFilter>();
        foreach (GameObject cell in cells)
            listFilters.Add(cell.GetComponent<MeshFilter>());

        MeshFilter[] meshFilters = listFilters.ToArray();
        foreach (var filter in meshFilters)
        {
            if (filter.sharedMesh == null)
                continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = filter.sharedMesh;

            ci.transform = myTransform * filter.transform.localToWorldMatrix;
            if (filter.GetComponent<MeshRenderer>() != null)
                combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
           
        }

        //should only be one
        GameObject toReturn = null;
        foreach (Material m in combines.Keys)
        {
            var go = new GameObject("Combined mesh");

            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var filter = go.AddComponent<MeshFilter>();
            filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);

            var renderer = go.AddComponent<MeshRenderer>();
            renderer.material = m;

            toReturn = go;
        }

        return toReturn;
    }
	
}
