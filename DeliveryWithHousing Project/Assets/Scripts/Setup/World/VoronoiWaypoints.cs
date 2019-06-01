using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiWaypoints : MonoBehaviour {

    //This script places waypoint nodes on each point on a voronoi mesh which has already been split up in to seperte cells
    //It then neightbours the waypoints to each other. The central point to all out esges, and all outer edges to another cell which
    //has a vertice at the same position
    //palce on voronoi mesh parent
    public MeshFilter[] meshFilters;
    public float ratioAllowed = 100000f;//10f;
    public GameObject worldPlan;

    void Start ()
    {      

        PlaceNodes();
        //StartCoroutine("YAdjust");

        StartCoroutine("LocalNeighbours");

        //local neighbours in to Foreign Neighbours
        //Foreign neighbours in to Y Adjust

    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
    }

    void PlaceNodes()
    {
        //create array of meshFilters that are childed to this transform
        meshFilters = transform.GetComponentsInChildren<MeshFilter>();

        //go through each of the meshes in array and place nodes at each vertice

        for (int i = 0; i < meshFilters.Length; i++)
        {
            //array has parent's filter in it, skip this//always the first one
            if (i == 0)
                continue;

            Mesh mesh = meshFilters[i].mesh;
            //add a waypoint to the first/central vertice

            //go through each vertices and drop waypoints
            for (int j = 0; j < mesh.vertexCount; j++)
            {
                if (j == 0)
                    continue;

                GameObject nodeGameObject = new GameObject();
                nodeGameObject.transform.parent = meshFilters[i].transform;
                nodeGameObject.name = j.ToString();
                nodeGameObject.tag = "VoronoiWaypoint";
                nodeGameObject.layer = 20;
                nodeGameObject.transform.position = mesh.vertices[j];

                WaypointNode wpn = nodeGameObject.AddComponent<WaypointNode>();
                wpn.position = mesh.vertices[j];
                wpn.isActive = true;

                //no need for a collider in central position
                if (j == 0)
                    continue;

                //add box colliders used for searching for closest point in ForeignNeighbours()
                BoxCollider box = nodeGameObject.AddComponent<BoxCollider>();
                box.center = mesh.vertices[j];
                Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
                box.size = size;
            }
            
        }
    }

    IEnumerator LocalNeighbours()
    {
        
        //for each cell, add a link between the central point and the outers
        //and a link between each outer and the next outer on the edge, back and forth

        for (int i = 0; i < meshFilters.Length; i++)
        {

            //array has parent's filter in it, skip this//always the first one
            if (i == 0)
                continue;

            //create array of nodes in this cell
            WaypointNode[] nodesInChildren = meshFilters[i].transform.GetComponentsInChildren<WaypointNode>();

            for (int j = 0; j < nodesInChildren.Length; j++)
            {
                if (j == 10000)//not entering middle node anymore -keeping for now
                {
                    continue;//not using centre point
                    //the first node is always the centre point
                    WaypointNode centralNode = nodesInChildren[j];
                    //create links between this nad all other nodes in cell
                    foreach (WaypointNode waypointNode in nodesInChildren)
                    {
                        //do not join central node to itself
                        if (waypointNode == centralNode)
                            continue;

                        //use slope ration (height/distance) * 100 gives us slope percentage
                        float height = centralNode.position.y - waypointNode.position.y;
                        height = Mathf.Abs(height);
                        float distance = Vector3.Distance(centralNode.position,  waypointNode.position);
                        float ratio = (height / distance) * 100;

                       // Debug.Log(ratio);



                        //check angle //height difference
                        float angle = Vector3.Angle(centralNode.position - waypointNode.position, Vector3.down);


                        //if angle is too steep, skip, do not add

                        if (ratio < ratioAllowed)
                        {
                            //join one way
                            centralNode.neighbors.Add(waypointNode);
                            //and the other, we now have a back and forth link
                            waypointNode.neighbors.Add(centralNode);
                        }
                    }

                    if(centralNode.neighbors.Count > nodesInChildren.Length/2)
                    {
                        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                       // sphere.transform.position = centralNode.position;
                       // sphere.transform.localScale *= 50;
                        
                    }
                }
                //first outer point
                else if (j == 0)// was one, not making inner node now
                {
                    //add the last point to this first outer point, to create a loop around the outside of the cell
                   
                    float angle1 = Vector3.Angle(nodesInChildren[nodesInChildren.Length - 1].position - nodesInChildren[j].position, Vector3.down);
                    float height1 = nodesInChildren[nodesInChildren.Length - 1].position.y - nodesInChildren[j].position.y;
                    height1 = Mathf.Abs(height1);
                    float distance1 = Vector3.Distance(nodesInChildren[nodesInChildren.Length - 1].position, nodesInChildren[j].position);
                    float ratio1 = (height1 / distance1) * 100;

                    if (ratio1 < ratioAllowed)                    
                        nodesInChildren[j].neighbors.Add(nodesInChildren[nodesInChildren.Length - 1]);
                  
                    float angle2 = Vector3.Angle(nodesInChildren[j+1].position - nodesInChildren[j].position, Vector3.down);
                    float height2 = nodesInChildren[j+1].position.y - nodesInChildren[j].position.y;
                    height2 = Mathf.Abs(height2);
                    float distance2 = Vector3.Distance(nodesInChildren[j + 1].position, nodesInChildren[j].position);
                    float ratio2 = (height2 / distance2) * 100;

                   if (ratio2 < ratioAllowed)                    
                        nodesInChildren[j].neighbors.Add(nodesInChildren[j + 1]);
                     
                }

                //last point
                else if (j == nodesInChildren.Length - 1)
                {   
                    //point behind
                    float angle1 = Vector3.Angle(nodesInChildren[j].position - nodesInChildren[nodesInChildren.Length-2].position, Vector3.down);
                    float height1 = nodesInChildren[j].position.y - nodesInChildren[nodesInChildren.Length-2].position.y;
                    height1 = Mathf.Abs(height1);
                    float distance1 = Vector3.Distance(nodesInChildren[nodesInChildren.Length - 2].position, nodesInChildren[j].position);
                    float ratio1 = (height1 / distance1) * 100;

                    if (ratio1 < ratioAllowed)
                        nodesInChildren[j].neighbors.Add(nodesInChildren[nodesInChildren.Length - 2]);
                    else
                        Debug.Log(ratio1 + " allowed = " + ratioAllowed);


                    //in front
                    float angle2 = Vector3.Angle(nodesInChildren[0].position - nodesInChildren[j].position, Vector3.down);
                    float height2 = nodesInChildren[0].position.y - nodesInChildren[j].position.y;
                    height2 = Mathf.Abs(height2);
                    float distance2 = Vector3.Distance(nodesInChildren[0].position, nodesInChildren[j].position);
                    float ratio2 = (height2 / distance2) * 100;

                    if (ratio2 < ratioAllowed)
                        nodesInChildren[j].neighbors.Add(nodesInChildren[0]);
                    else
                        Debug.Log(ratio2);
                }

                //all other outer points
                else
                {
                   
                    //point behind
                    float angle1 = Vector3.Angle(nodesInChildren[j-1].position - nodesInChildren[j].position, Vector3.down);
                    float height1 = nodesInChildren[j-1].position.y - nodesInChildren[j].position.y;
                    height1 = Mathf.Abs(height1);
                    float distance1 = Vector3.Distance(nodesInChildren[j-1].position, nodesInChildren[j].position);
                    float ratio1 = (height1 / distance1) * 100;

                    if (ratio1 < ratioAllowed)
                        nodesInChildren[j].neighbors.Add(nodesInChildren[j-1]);
                    else
                        Debug.Log(ratio1);

                    //in front
                    float angle2 = Vector3.Angle(nodesInChildren[j+1].position - nodesInChildren[j].position, Vector3.down);
                    float height2 = nodesInChildren[j+1].position.y - nodesInChildren[j].position.y;
                    height2 = Mathf.Abs(height2);
                    float distance2 = Vector3.Distance(nodesInChildren[j+1].position, nodesInChildren[j].position);
                    float ratio2 = (height2 / distance2) * 100;

                    if (ratio2 < ratioAllowed)
                        nodesInChildren[j].neighbors.Add(nodesInChildren[j+1]);
                    else
                        Debug.Log(ratio2);
                }

            }
        }

        yield return new WaitForEndOfFrame();
        
        //StartCoroutine("ForeignNeighbours");
        StartCoroutine("ForeignNeighbours2");

        yield break;
    }

    IEnumerator ForeignNeighbours()
    {
        //Joins neighbouring cells together

        //use already filled nodes array

        //for each cell


        for (int i = 0; i < meshFilters.Length; i++)
        {

            //array has parent's filter in it, skip this//always the first one
            if (i == 0)
                continue;

            //create array of nodes in this cell
            WaypointNode[] nodesInChildren = meshFilters[i].transform.GetComponentsInChildren<WaypointNode>();
            //for each vertice in cell, skip centre/first
            for (int j = 0; j < nodesInChildren.Length; j++)
            {
                if (j == 0)
                    continue;

                //raycast down
                //use the node's position rather than transform's. transform.position is vector3.zero
                Vector3 position = nodesInChildren[j].position;
                RaycastHit[] hits = Physics.RaycastAll(position + Vector3.up, Vector3.down, 2f, LayerMask.GetMask("HexWaypoint"));
                
                //if more than one hit
                if (hits.Length > 1)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        nodesInChildren[j].transform.position = hit.point;

                        //do not add the node we are checking from
                        if (hit.transform == nodesInChildren[j].transform)
                            continue;

                        //add any other nodes that were hit
                        nodesInChildren[j].neighbors.Add(hit.transform.GetComponent<WaypointNode>());
                        
                    }
                }

                

                //yield return new WaitForEndOfFrame();
                //join all hits except this node
            }

         //   yield return new WaitForEndOfFrame();
        }

        StartCoroutine("DisableBoxColliders");

        yield break;
    }

    IEnumerator ForeignNeighbours2()
    {
        
        int count = 0;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            
            if(count == 50)
            {
                count = 0;
                yield return new WaitForEndOfFrame();
            }
            count++;
            //array has parent's filter in it, skip this//always the first one
            if (i == 0)
                continue;

            //create array of nodes in this cell
            WaypointNode[] nodesInChildren = meshFilters[i].transform.GetComponentsInChildren<WaypointNode>();

            //for each node look for a matching node in another cell
            for (int j = 0; j < nodesInChildren.Length; j++)
            {
                //all other meshfilters
                for (int x = 0; x < meshFilters.Length; x++)
                {
                    if (x == 0)
                        continue;
                    //dont check same meshfilter
                    if (i == x)
                        continue;

                    WaypointNode[] nodesInChildrenOther = meshFilters[x].transform.GetComponentsInChildren<WaypointNode>();

                    for (int y = 0; y < nodesInChildrenOther.Length; y++)
                    {
                        if (nodesInChildren[j].position == nodesInChildrenOther[y].position)
                        {                            
                            //grab found nodes neighbours and add to this nodes
                            List<WaypointNode> neighborsOther = nodesInChildrenOther[y].neighbors;
                            for (int q = 0; q < neighborsOther.Count; q++)
                            {
                                if (neighborsOther[q] != nodesInChildren[j])
                                    if(!nodesInChildren[j].neighbors.Contains(neighborsOther[q]))
                                        nodesInChildren[j].neighbors.Add(neighborsOther[q]);

                                

                               
                            }
                        }
                    }
                }
            }
        }
        WaypointPathfinder wpf = gameObject.GetComponent<WaypointPathfinder>();
        wpf.forVoronoiWaypoints = true;
        wpf.MakeNewMap();

        worldPlan.SetActive(true);
        yield break;
    }

    IEnumerator YAdjust()
    {
        
        //for each waypoint node, raycast looking for terrrain base and adjust height accordingly;

        for (int i = 0; i < meshFilters.Length; i++)
        {

            //array has parent's filter in it, skip this//always the first one
            if (i == 0)
                continue;

            //create array of nodes in this cell
            WaypointNode[] nodesInChildren = meshFilters[i].transform.GetComponentsInChildren<WaypointNode>();

            for (int j = 0; j < nodesInChildren.Length; j++)
            {
                RaycastHit hit;
                if(Physics.Raycast(nodesInChildren[j].position + (Vector3.up * 2000f), Vector3.down, out hit, 4000f, LayerMask.GetMask("TerrainBase")))
                {
       //             Debug.Log("hit");

                    nodesInChildren[j].position = hit.point;
                }
                
                

            }
          //  yield return new WaitForEndOfFrame();
        }
        StartCoroutine("LocalNeighbours");
        yield break;
    }

    IEnumerator DisableBoxColliders()
    {

        BoxCollider[] boxes = gameObject.GetComponentsInChildren<BoxCollider>();

        //Debug.Log(boxes.Length);
        foreach (BoxCollider box in boxes)
            box.enabled = false;

        WaypointPathfinder wppf = gameObject.AddComponent<WaypointPathfinder>();
        wppf.forVoronoiWaypoints = true;
        wppf.MakeNewMap();

        //gameObject.GetComponent<RingRaycast>().enabled = true;
        yield break;
    }
}
