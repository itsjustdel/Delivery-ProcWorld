using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlaceWaypointPlayer : MonoBehaviour {

    //minimum distance between road points for mesh
    public float roadWidth = 3f;
    public float threshold = 30f;
	public Vector3 closestPointOnHex;
    public List<Vector3> roadPointsForCell = new List<Vector3>();
    //thse vars are saved for mesh script, we will need them to join the roads
    public GameObject targetRoadEnd;
    public GameObject targetRoadStart;
    public int closestIndexOnMesh;
    public int closestIndexOnMeshStart;
    public bool loopRoad = false;
    public List<GameObject> roadsBuilt = new List<GameObject>();
    public BezierSpline spline;
    // Use this for initialization
    void Start () {
		//PlaceWaypointPlayers();
        PlaceWaypointPlayersForVoronoi();
    }
	
	void PlaceWaypointPlayers()
	{
		//place waypointPlayers around edges of this mesh, stopping at any points deemed to be junctions.
		//These points are determined by the points inserted in to the shared point master list after we build a road.
		//So, the first road made will not have to encounter any of these points.

		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		//master list of all shared points. Shared points are inserted after we build a road
		List<CellManager.PositionAndOwner> sharedList = GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList;
		//A list if the vertices which share a position with another road point
		List<int> sharedVertices = new List<int>();

		//Find what vertices from this cell's mesh match the position of any v3s in the master list of all shared points

		//don'd do if list is empty ( this swill be the first cell)
		if(sharedList.Count !=0)
		{
			for (int i = 0; i < sharedList.Count; i++)
			{
				for (int j = 0; j < mesh.vertexCount; j++)
				{
					if(sharedList[i].position == mesh.vertices[j])
					{
						//if we have a match, add to the cell's local sharedVertices list
						sharedVertices.Add(j);
					}
				}
			}
		}

		//Now Find where to start and finish the road

		//create a ring for the first cell
		if (sharedList.Count == 0)
		{
			//if it is the first ring road created, it does not matter where the start and finish is because it
			//becomes a singular loop, attaching to itself
			for(int i = 0; i < mesh.vertexCount; i++)
			{
				// add the last pathfinder. the target is the first vertice so we can make the final section of the loop

				if(i == mesh.vertexCount)
				{
					GameObject finalWaypointPlayerGo = new GameObject();
					//attach to this cell

					//target will always be 1, first mesh vertice is 0 and we do it in normal order for first ring road
					finalWaypointPlayerGo .transform.position = Vector3.Lerp( mesh.vertices[ i ] ,mesh.vertices[1],0.25f); 
					finalWaypointPlayerGo .transform.parent =this.transform;			
					finalWaypointPlayerGo .name = "Pathfinder"+i;

					WaypointPlayer finalWaypointPlayer = finalWaypointPlayerGo.AddComponent<WaypointPlayer>();
					finalWaypointPlayer.forVoronoi = true;
					finalWaypointPlayer.PathType = PathfinderType.WaypointBased;
					//waypointPlayer.target = mesh.vertices[index];//original
					finalWaypointPlayer .target = Vector3.Lerp(mesh.vertices[ i ], mesh.vertices[ 1 ], 0.75f);

					//now add last linking corner
					continue;
				}
				//skip first mesh vertices- this is always the centre point of the mesh for the cell. 
				if(i==0)
					continue;

				//make sure the index does not go out of range if it meets thened of the mesh. Loop it back to the start
				int nextIndex = i+1;
				if(nextIndex < mesh.vertexCount)
				{
					//waypointPlayer.target = mesh.vertices[index]; //TODO should be lerped like below?? - working right now
				}
				else
				{
					nextIndex -= mesh.vertexCount;
					//jump over central vertice ( i = 0 )
					nextIndex += 1;

				}

				//Place pathfinders round the edges of the mesh
				GameObject waypointPlayerGo = new GameObject();
				//attach to this cell
				waypointPlayerGo.transform.position = Vector3.Lerp( mesh.vertices[ i ] ,mesh.vertices[nextIndex ],0.25f);
				waypointPlayerGo.transform.parent =this.transform;			
				waypointPlayerGo.name = "Pathfinder"+i;

				WaypointPlayer waypointPlayer = waypointPlayerGo.AddComponent<WaypointPlayer>();
                waypointPlayer.forVoronoi = true;
                waypointPlayer.PathType = PathfinderType.WaypointBased;
				//waypointPlayer.target = mesh.vertices[index];//original
				waypointPlayer.target = Vector3.Lerp(mesh.vertices[i], mesh.vertices[nextIndex], 0.75f);

				//add pathfinder for corners

				int nextNextIndex = nextIndex+1;
				if(nextNextIndex < mesh.vertexCount)
				{
					//waypointPlayer.target = mesh.vertices[index]; //TODO should be lerped like below?? - working right now
				}
				else
				{
					nextNextIndex -= mesh.vertexCount;
					//jump over central vertice ( i = 0 )
					nextNextIndex += 1;
				}

				//if not the last one
				//create mini waypoint player to join corners smoothly
		//		if( i != mesh.vertexCount -2)
		//		{
					GameObject wppgo2 = new GameObject();
					wppgo2.transform.parent = this.transform;
					wppgo2.transform.position = Vector3.Lerp(mesh.vertices[i], mesh.vertices[nextIndex], 0.75f);
					wppgo2.name = "Corner PathFinder";

					WaypointPlayer wppPlayer2 = wppgo2.AddComponent<WaypointPlayer>();
					wppPlayer2.target =Vector3.Lerp(mesh.vertices[nextIndex], mesh.vertices[nextNextIndex], 0.25f);
					wppPlayer2.forVoronoi = true;
					wppPlayer2.PathType = PathfinderType.WaypointBased;

		//		}
			}

			//update masterlist with points from the edge of the mesh
			for (int i = 0; i < mesh.vertexCount;i++)
			{
				//1st vertices is always the middle, we dont need this
				if(i==0)
					continue;

				CellManager.PositionAndOwner posAndOwner = new CellManager.PositionAndOwner();
				posAndOwner.position = transform.TransformPoint( mesh.vertices[i] );
				posAndOwner.owner = this.gameObject;
				sharedList.Add(posAndOwner);

			}
			//feed the updated list back to cell manager
			GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList  = sharedList;
		}

		//create a joining ring for all the secondary rings
		else
		{

	//		List<int> sharedIndices = new List<int>();
			List<int> roadPoints = new List<int>();
	//		List<int> roadPointsTargets = new List<int>();
			int startingIndex = 100;

			//run through mesh and check for a match in the sharedLIst(masterList)
			//if there is a match, check to see if next point in the mesh is in the master list
			//if the next point is not in the master list, start the road from current index
			int startRoadAt = 100;
			for (int i = 1; i < mesh.vertexCount; i++)
			{
				//for the bulk of the points on the mesh(not the last one basically)
				if (i!= mesh.vertexCount-1)
				{
					bool thisPointInList = false;
					bool secondPointIsInList = false;
					for (int j = 0; j < sharedList.Count; j++)
					{
						//if(mesh.vertices[i] == sharedList[j].position)		//changed to distance check because of autoweld,moves mesh vertices around slightly
						//{
						if(Vector3.Distance( mesh.vertices[i],sharedList[j].position) < 45) //hex size - 5
						{
							thisPointInList = true;
						}
					}

					if(thisPointInList)
					{
						for(int k = 0; k < sharedList.Count; k++)
						{
							//if (mesh.vertices[i+1] != sharedList[k].position)
							//{
							if(Vector3.Distance( mesh.vertices[i+1],sharedList[k].position) < 45)
							{
								secondPointIsInList = true;
								//startRoadAt = i;
							}
						}
					}

					if (thisPointInList == true && secondPointIsInList == false)
					{
						startRoadAt = i;
					}

				//	if(startRoad)
				//	{
				//		roadPoints.Add(i);
				//		startingIndex = i;
				//		i = mesh.vertexCount;
				//	}
				}

				else if(i == mesh.vertexCount-1)
				{
					bool thisPointInList = false;
					bool secondPointIsInList = false;

					//last joins to mesh[1] -- that's just the way it is
					for (int j = 0; j < sharedList.Count; j++)
					{
						//if it is in the list
						//if(mesh.vertices[i] == sharedList[j].position)
						//{
						if(Vector3.Distance( mesh.vertices[i],sharedList[j].position) < 45)
						{
							thisPointInList = true;
						}
					}

					if(thisPointInList)
					{
						//if 1st point is not in list
						for (int k = 0; k < sharedList.Count; k++)
						{
							if(Vector3.Distance( mesh.vertices[1],sharedList[k].position) < 45)//TODO this line is wrong. at some point in the master/shared list, mesh vertices 1 will def be further away than some point in the list-same for above
							{
								//inList = true;
								//startRoadAt = i;
								secondPointIsInList = true;
							}
						}
					}

					if (thisPointInList == true && secondPointIsInList == false)
					{
						startRoadAt = i;
					}
				}
			}

			roadPoints.Add(startRoadAt);
			startingIndex = startRoadAt;

			//now we have found the start of our road, run through the rest of the mesh's vertices and add points until we hit another point
			//which is in the shared/master list
			for (int i = startingIndex + 1; i < mesh.vertexCount + startingIndex; i++)
			{				
				int fixedIndex = i;

				if ( i < mesh.vertexCount)
				{
					fixedIndex = i;
				}
				else
				{
					fixedIndex = i - mesh.vertexCount;
					//skip central mesh vertice (always 0)
					fixedIndex += 1;
				}
				bool addToList = false;

				for (int j = 0; j < sharedList.Count; j++)
				{			
//					Debug.Log(startingIndex + " start index");
//					Debug.Log(fixedIndex + " fixed index");
//					Debug.Log(mesh.vertexCount + " vertexCount");
					//if this point is not in the list, it is ok to add
					if( mesh.vertices[fixedIndex] != sharedList[j].position) //***************out of range error sometimes
					{
						addToList = true;
					}
					//if it is in the list, we have found another road, add this position and stop
					else if (mesh.vertices[fixedIndex] == sharedList[j].position)
					{						
					//	roadPoints.Add(fixedIndex);
					//	i = 100;
					}
				}
				if(addToList)
					roadPoints.Add(fixedIndex);
			}

			//Create objects for waypointPlayers and place them at the positions in the roadPoints list
			//don't place for the last point, this is the last rp's target
			for (int i = 0; i < roadPoints.Count-1; i++) //was - 1 with endIndex added from above
			{
				GameObject wppgo = new GameObject();
				wppgo.transform.parent = this.transform;
				wppgo.transform.position = Vector3.Lerp( mesh.vertices[ roadPoints[i] ] ,mesh.vertices[ roadPoints[i+1] ],0.25f);
				wppgo.name = "PathFinder"+i;

				//waypoint player component
				WaypointPlayer wppPlayer = wppgo.AddComponent<WaypointPlayer>();
				wppPlayer.forVoronoi = true;
				wppPlayer.PathType = PathfinderType.WaypointBased;

				//set target
				Vector3 halfwayHouse = Vector3.Lerp( mesh.vertices[ roadPoints[i] ] ,mesh.vertices[ roadPoints[i+1] ],0.75f);
				wppPlayer.target = halfwayHouse;
			
				//if not the last one
				//create mini waypoint player to join corners smoothly
				if( i != roadPoints.Count -2)
				{
					GameObject wppgo2 = new GameObject();
					wppgo2.transform.parent = this.transform;
					wppgo2.transform.position = halfwayHouse;
					wppgo2.name = "Corner PathFinder";

					WaypointPlayer wppPlayer2 = wppgo2.AddComponent<WaypointPlayer>();
					wppPlayer2.target = Vector3.Lerp( mesh.vertices[ roadPoints[i+1] ], mesh.vertices[ roadPoints[i+2] ] ,0.25f );
					wppPlayer2.forVoronoi = true;
					wppPlayer2.PathType = PathfinderType.WaypointBased;

				}
			}
		}

		//update masterlist with points from the edge of the mesh
		GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList  = sharedList;
		for (int i = 0; i < mesh.vertexCount;i++)
		{
			//1st vertices is always the middle, we dont need this
			if(i==0)
				continue;

			CellManager.PositionAndOwner posAndOwner = new CellManager.PositionAndOwner();
			posAndOwner.position = mesh.vertices[i];
			posAndOwner.owner = this.gameObject;
			sharedList.Add(posAndOwner);
			//sharedList.Add(mesh.vertices[i]);
		}
		//feed the updated list back to cell manager
		GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList  = sharedList;
	}

    void PlaceWaypointPlayersForVoronoi()
    {

        WaypointPathfinder waypointPathFinder = GameObject.Find("VoronoiPathfinderMap").GetComponent<WaypointPathfinder>();

        List<Vector3> voronoiPointsUsed = transform.parent.GetComponent<CellManager>().voronoiPointsUsed;
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        
        Vector3[] vertices = mesh.vertices;
        //lool for start of road. We need a point that is in the list with the next point free
        List<Vector3> pathAroundCell = new List<Vector3>();
        for (int i = 1; i < mesh.vertexCount; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex > mesh.vertexCount - 1)
                //take back to ) but plus to skip central vertex which remains unused
                nextIndex = 1;

            GameObject wppgo = new GameObject();
            wppgo.transform.parent = transform;
            wppgo.transform.position = vertices[i];
            wppgo.name = "PathFinder" + i;

            //waypoint player component
            WaypointPlayer wpp = wppgo.AddComponent<WaypointPlayer>();
            wpp.enabled = false;
            wpp.forVoronoi = true;
            wpp.PathType = PathfinderType.WaypointBased;
            //tell way point play which nav map to use
            wpp.waypointPathFinder = GameObject.Find("VoronoiPathfinderMap").GetComponent<WaypointPathfinder>();
            //find closest node to start on
            WaypointListNode startNode = wpp.waypointPathFinder.FindClosestNode(vertices[i]);
            wppgo.transform.position = startNode.position;
            WaypointListNode endNode = wpp.waypointPathFinder.FindClosestNode(vertices[nextIndex]);

            /*
                c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                c.transform.position = startNode.position;
                c.name = "Start Voronoi"; c.transform.localScale *= 15;
                c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                c.transform.position = endNode.position;
                c.name = "End Voronoi"; c.transform.localScale *= 15;
                */
            //wppgo.transform.position = wpp.target;

            //tell the agent to find a path using it's map
            List<Vector3> path = wpp.waypointPathFinder.FindPath(startNode.position, endNode.position);
            //check for duplicates on this path - this can happen if the roadswings back round on itself
            List<Vector3> newPath = new List<Vector3>();

            for (int j = 0; j < path.Count; j++)
            {
                //sometimes pathfinder gives path back where it keeps goin after it finds the end?
                if (path[j] == endNode.position)
                    break;

                if (roadPointsForCell.Contains(path[j]))
                {
                    //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    // c.transform.position = path[j];
                    // c.name = "road points for cell contains j";
                    // c.transform.localScale *= 15;
                    roadPointsForCell.Remove(path[j]);
                    break;
                }


                if (roadPointsForCell.Contains(path[j]))//getting here ver now it breaks above?
                {
                    // c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //c.transform.position = path[j];
                    //c.name = "to remove ????? CHECK";
                    newPath.Add(path[j]);

                    Debug.Log("path j = " + path[j] + ", wpp.target = " + wpp.target);

                }
                else
                {
                    newPath.Add(path[j]);
                }
            }
            
            for (int j = 0; j < newPath.Count; j++)            
                pathAroundCell.Add(newPath[j]);
        }

        //look for path start
        


        //we now have the points where we want to build the road - but we must take out the points that overlap with other roads
        //as we build the orads we enter the points used in to a global voronoiPointsUsed list, it is this list we check against

    
        int startsFound = 0;
      
        //it is possible we can find tow roads, new roads are split by matches on the voronoi graph
        List<List<Vector3>> possibleRoads = new List<List<Vector3>>();
        for (int j = 0; j < pathAroundCell.Count-1; j++)
        {
           // if (endFound)
           //     continue;

            if(voronoiPointsUsed.Contains(pathAroundCell[j]))
            {
                List<Vector3> possibleRoad = new List<Vector3>();
                
                if (!voronoiPointsUsed.Contains(pathAroundCell[j+1]))
                {
                  //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                  //  c.transform.position = pathAroundCell[j];
                  //  c.transform.localScale *= 15;
                        
                  //  c.name = "start of road?";
                    possibleRoad.Add(pathAroundCell[j]);

                    startsFound++;

                    //add til end of list
                    for (int k = j+1; k < pathAroundCell.Count; k++)
                    {
                        if (!voronoiPointsUsed.Contains(pathAroundCell[k]))
                        {
                          //  c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                          //  c.transform.position = pathAroundCell[k];
                          //  c.transform.localScale *= 10;
                          //  c.name = "road piece 0 + " + startsFound.ToString();

                            possibleRoad.Add(pathAroundCell[k]);
                        } 
                        else
                        {
                            //we hit a point, end this possible road and start a new one
                            //add end stub
                            possibleRoad.Add(pathAroundCell[k]);

                            possibleRoads.Add(possibleRoad);
                            possibleRoad = new List<Vector3>();
                        }
                    }
                    //add from start of list to "start"
                    //if the list lined up, this loop won't fire, we ahve found the end of the raod
                    if (j == 0)
                    {
                      //  c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                      //  c.transform.position = pathAroundCell[j];
                      //  c.transform.localScale *= 10;
                      //  c.name = "j == 0 BREAK " + startsFound.ToString();
                     
                    }
                    else
                    {
                        for (int k = 0; k < j; k++)
                        {
                            if (!voronoiPointsUsed.Contains(pathAroundCell[k]))
                            {
                             //   c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            //    c.transform.position = pathAroundCell[k];
                            //    c.transform.localScale *= 10;
                            //    c.name = "road piece 1 + " + startsFound.ToString();

                                possibleRoad.Add(pathAroundCell[k]);//break?

                            }
                            else
                            {
                            //    c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            //    c.transform.position = pathAroundCell[k];
                            //    c.transform.localScale *= 10;
                            //    c.name = "road piece 1 + BREAK " + startsFound.ToString();
                                //we hit a point, end this possible road and start a new one
                                //add end 
                                possibleRoad.Add(pathAroundCell[k]);
                                //add to list of possibles
                                possibleRoads.Add(possibleRoad);
                                possibleRoad = new List<Vector3>();
                                

                                //endFound = true;
                                //break;
                            }
                        }
                    }
                }

                possibleRoads.Add(possibleRoad);
            }
        }
        //choose larger road out of all roads found
        for (int i = 0; i < possibleRoads.Count; i++)
        {
            if (roadPointsForCell.Count < possibleRoads[i].Count)
                roadPointsForCell = new List<Vector3>(possibleRoads[i]);
        }

        //if we don't het a match for this cell on used oronoi points- we need to make a self enclosed ring road
        //just add all the points, it's cool
        
        if (roadPointsForCell.Count == 0)
        {
            roadPointsForCell = new List<Vector3>(pathAroundCell);
            //make loop
            roadPointsForCell.Add(roadPointsForCell[0]);
            
        }
        
        //take out hair pins
        for (int a = 0  ; a < roadPointsForCell.Count-1; a++)//**do last?
        {
            
            //doing this to check the first two points aginst the last two in case there is a hairpin at the start and end
            int nextIndex = a + 1;
            if (nextIndex > roadPointsForCell.Count - 1)
                nextIndex -= roadPointsForCell.Count;
            int nextIndex2 = a + 2;
            if (nextIndex2 > roadPointsForCell.Count - 1)
                nextIndex2 -= roadPointsForCell.Count;

            if (Vector3.Distance(roadPointsForCell[a], roadPointsForCell[nextIndex]) < .1f)
            {
                //reset and go over loop until all close points are removed
                //remove

                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                c.transform.position = roadPointsForCell[nextIndex];
                c.name = "hair pin remove, a =" + a + ", next index = " + nextIndex + ", nextIndex2 = " + nextIndex2;
                c.transform.localScale *= 1;

                roadPointsForCell.RemoveAt(nextIndex);
                a = 0;
            }
        }
       


        //remove duplos //order is important, take out hairpins first

        for (int a = 0; a < roadPointsForCell.Count - 1; a++)//**do last? no - it equals first
        {
            for (int b = a + 1; b < roadPointsForCell.Count - 1; b++)
            {
                if (roadPointsForCell[a] == roadPointsForCell[b])
                {
                    roadPointsForCell.RemoveAt(a);
                    a = 0;
                    break;
                }
            }
        }
       

        //now a final smoothing to make sure tiny curves dont overlap on each other when the road is building. 
        for (int a = 0; a < roadPointsForCell.Count - 1; a++)//**do last?
        {
            if (Vector3.Distance(roadPointsForCell[a], roadPointsForCell[a + 1]) < threshold)
            {
                roadPointsForCell.RemoveAt(a + 1);
                a = 0;
            }
        }
       


        //check to see if road is aloop - mesh script needs to know if it is joining on to itself or trying to find other roads
        if (Vector3.Distance( roadPointsForCell[0],roadPointsForCell[roadPointsForCell.Count-1])<threshold)
        {
            loopRoad = true;
        }

        //make road smarter? //possibly could be in hairpin removal loop
        for (int a = 0; a < roadPointsForCell.Count - 2; a++)//**do last?...
        {
            //check first against last//not doing, can remove whole road?
            int nextIndex = a + 1;
            int nextNextIndex = a + 2;
            if (nextIndex >= roadPointsForCell.Count)
                nextIndex -= roadPointsForCell.Count;
            if (nextNextIndex >= roadPointsForCell.Count)
                nextNextIndex -= roadPointsForCell.Count;

            if (Vector3.Distance(roadPointsForCell[a], roadPointsForCell[nextNextIndex]) < Vector3.Distance(roadPointsForCell[a], roadPointsForCell[nextIndex]))
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = roadPointsForCell[nextIndex];
                c.name = "smart remove";

                roadPointsForCell.RemoveAt(nextIndex);
                a = 0;
            }
        }

        //smooth again
        for (int a = 0; a < roadPointsForCell.Count - 1; a++)//**do last?
        {
            if (Vector3.Distance(roadPointsForCell[a], roadPointsForCell[a + 1]) < threshold)
            {
                roadPointsForCell.RemoveAt(a + 1);
                a = 0;
            }
        }

        //trying to sniop loop road to stop crazy ends - not sure if this is working?
        //if last point is closer to 2nd point rather than first, remove 1st
        if (loopRoad)
        {
            //if second and second last are closer to each other than last and first, remove last and first
            if (Vector3.Distance(roadPointsForCell[roadPointsForCell.Count - 2], roadPointsForCell[1]) < Vector3.Distance(roadPointsForCell[roadPointsForCell.Count - 2], roadPointsForCell[0]))
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = roadPointsForCell[0];
                c.name = "smart remove last loop road";
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                c.name = "smart remove last loop road";

                //ast first and last are the same
                roadPointsForCell.RemoveAt(0);
                roadPointsForCell.RemoveAt(roadPointsForCell.Count - 1);

                //now we need make sure the road is "smart" - basically just doing what we did to the main section of but just for the end
                if (Vector3.Distance(roadPointsForCell[roadPointsForCell.Count - 2], roadPointsForCell[0]) < Vector3.Distance(roadPointsForCell[roadPointsForCell.Count - 1], roadPointsForCell[0]))
                {
                    //if last point is further away from first than second last point is, remove last point
                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                    c.name = "smart remove last loop Smart";
                    roadPointsForCell.RemoveAt(roadPointsForCell.Count - 1);

                }
            }
            else
                //start and end points re the same, if we have two points the same, the curve can get folded over
                roadPointsForCell.RemoveAt(0);
        }

        //reset loop road bool - we are going to check it again with spherecasts when making the road spline
        loopRoad = false;

        //now we ahve an idea of a road, create a curve from this these points and test if we overlap any other roads

        
                           


        foreach (Vector3 v3 in roadPointsForCell)
        {
            //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
             // c.transform.position = v3;
            //  c.name = "Road point";
               voronoiPointsUsed.Add(v3);
        }
        //now we ahve the main part of the road sorted, we need to think about the start and the end. Raycast through the path from middle to end, then middle to start and make sure
        //the road is approaching its target in the correct way. i.e not overshooting and bennding back round. This can happen due to the voronoi pathfinding mesh having points close to each other
        //start junction
        
         MakeSpline();

      
    }

    float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle(a, b); // calculate angle
                                           // assume the sign of the cross product's Y component:
        return angle * Mathf.Sign(Vector3.Cross(a, b).y);
    }
    void MakeSpline()
    {
        //from global
        spline = gameObject.AddComponent<BezierSpline>();
        if(roadPointsForCell.Count>2)
        spline.points = RingRoadMesh.AddControlPointsToList(gameObject, roadPointsForCell).ToArray();
        
        List<BezierControlPointMode> modes = new List<BezierControlPointMode>();
        for (int i = 0; i < spline.points.Length; i++)
        {
            modes.Add(BezierControlPointMode.Mirrored);

            //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //   cube.transform.position = spline.points[i];
            //   cube.name = i.ToString();
        }

        spline.modes = modes.ToArray();


        List<Vector3> nextSplinePoints = new List<Vector3>();

        //use this list to avoid duplicates
        List<Vector3> roadPointsUsed = new List<Vector3>(roadPointsForCell);

        float f = spline.points.Length * 50f;
        float s = 1f / f;
        Vector3 prevPos = Vector3.zero;
        //first half of mesh - go to centre and work our way backwards to find a road to join to
        RaycastHit hit = new RaycastHit();
        for (float i = (f/2) + 1; i >=0; i-= 0.1f)
        {
            if (prevPos != Vector3.zero)
                if (Vector3.Distance(spline.GetPoint(i * s), prevPos) < 1f)
                    continue;

            prevPos = spline.GetPoint(i * s);

            GameObject c = null;
          //  c = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  c.transform.position = spline.GetPoint(i*s);
          //  c.name = "test start";

            //check for any other road at road's width
            Vector3 offset1 = spline.GetPoint(i * s) + Quaternion.Euler(0, 90, 0) * -spline.GetDirection(i * s) * 4f;
            Vector3 offset2 = spline.GetPoint(i * s) + Quaternion.Euler(0, -90, 0) * -spline.GetDirection(i * s) * 4f;//4, 1 more than road width
            
            bool didHit = false;
            if (Physics.Raycast(offset1 + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Road")))
            {
                didHit = true;
            }
            if (!didHit)
            {
                if (Physics.Raycast(offset2 + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Road")))
                {
                    didHit = true;
                }
            }
            if (didHit)
            {
                //find the closest point on hit mesh
                targetRoadStart = hit.transform.gameObject;
                //use the hit point from the ray to find the closest point on the target mesh
                Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMeshStart, hit.point, hit.transform.gameObject);
                /*
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = closestPointOnMesh;
                c.transform.name = "closest on mesh";
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMeshStart + 2];
                c.transform.name = "closest on mesh 2 ";
                */
                //get direction of the way the road is running
                Vector3 roadDir = (hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMeshStart + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                              //find which direction to create control point for the t junction in
                Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                Vector3 lookDir = Quaternion.Euler(0, 90, 0) * roadDir;
                //check which is closest - use that rotation to build 
                if (Vector3.Distance(closestPointOnMesh + lookDir1, nextSplinePoints[nextSplinePoints.Count - 1]) < Vector3.Distance(closestPointOnMesh + lookDir2, nextSplinePoints[nextSplinePoints.Count - 1]))//what happens if it finds another raod instantly? list isnt populated
                    lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                else
                    lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction

                //not suing clamp, always making threshold *.5
                /*
                //clamp distance? or if it is too close, don't add cp?
                float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(hit.point, closestPointOnMesh);
                if (distanceBetweenThisAndClosestPointOnMesh < threshold * 0.5f)
                    distanceBetweenThisAndClosestPointOnMesh = threshold * 0.5f;
                

                Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;
                */
                Vector3 controlPoint = closestPointOnMesh + lookDir * threshold * .5f;

                //remove last ponts if too close, we need to leave space for the junction
                for (int j = nextSplinePoints.Count - 1; j >0 ; j--)
                {
                    if(Vector3.Distance(hit.point, nextSplinePoints[nextSplinePoints.Count - 1]) < threshold)
                    {
                        nextSplinePoints.RemoveAt(nextSplinePoints.Count - 1);
                        j = nextSplinePoints.Count - 1;
                    }
                }

                //level y height- smooths junction approach
                controlPoint = new Vector3(controlPoint.x, closestPointOnMesh.y, controlPoint.z);
                nextSplinePoints.Add(controlPoint);
                nextSplinePoints.Add(closestPointOnMesh);

                
                /*
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = controlPoint;
                c.name = "cp start";
                */
                break;
            }

            

            //only add to the main curve every threshold point. or change to close to road cell point?
            if (nextSplinePoints.Count == 0)
            {
                nextSplinePoints.Add(spline.GetPoint(i * s));
            }
            else
            {
                //check if close to an orignal voronoi point, we want to keep using the shape the voronoi creates
                foreach (Vector3 v3 in roadPointsUsed)
                {
                    if (Vector3.Distance(v3, spline.GetPoint(i * s)) < 1f)
                    {
                        nextSplinePoints.Add(spline.GetPoint(i * s));
                        roadPointsUsed.Remove(v3);
                        break;
                    }
                }
            }
        }
        bool startSphereMissed = false;
        if (hit.transform == null && loopRoad == false)
        {
           // Debug.Log("Start of road not found");
            //Spherecast and and thr hit to the curve, save the hit object too
            float radius = 10f;
            while (radius < 500)
            {
                if (Physics.SphereCast(roadPointsForCell[0] + Vector3.up * radius * 2, radius, Vector3.down, out hit, radius * 3, LayerMask.GetMask("Road")))
                {
                    //we have found another road - but we must first check that this road is not a loop road. Check if first point is closer to hit or last point
                    if(Vector3.Distance(roadPointsForCell[0],hit.point) > Vector3.Distance(roadPointsForCell[0],roadPointsForCell[roadPointsForCell.Count-1]))
                    {
                        //the end of the road is closer than the hit point so flag as loop road and exit
                        Debug.Log("Ring road detected?");
                        loopRoad = true;
                        return;
                    }

                    //find the closest point on hit mesh
                    targetRoadStart = hit.transform.gameObject;
                    //use the hit point from the ray to find the closest point on the target mesh
                    Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMeshStart, hit.point, hit.transform.gameObject);
                    /*
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = closestPointOnMesh;
                    c.transform.name = "closest on mesh spherecast";
                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.name = "closest on mesh 2 spherecast";
                    */
                    int nextNextIndex = closestIndexOnMeshStart + 2;
                    Vector3[] verticesHit = hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices;
                    if (nextNextIndex >= verticesHit.Length)
                    {
                        nextNextIndex -= verticesHit.Length;
                        //c.transform.name = "closest on mesh 2 spherecast - Index changed";
                    }

                   // c.transform.position = verticesHit[nextNextIndex];//can be out of range
                    
                    //get direction of the way the road is running
                    Vector3 roadDir = (hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMeshStart + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                                       //find which direction to create control point for the t junction in
                    Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                    Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                    Vector3 lookDir = Quaternion.Euler(0, 90, 0) * roadDir;
                    //check which is closest - use that rotation to build 
                    if (Vector3.Distance(closestPointOnMesh + lookDir1, nextSplinePoints[nextSplinePoints.Count - 1]) < Vector3.Distance(closestPointOnMesh + lookDir2, nextSplinePoints[nextSplinePoints.Count - 1]))
                        lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                    else
                        lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                    //clamp distance? or if it is too close, don't add cp?
                    float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(hit.point, closestPointOnMesh);
                    if (distanceBetweenThisAndClosestPointOnMesh < threshold * 0.5f)
                        distanceBetweenThisAndClosestPointOnMesh = threshold * 0.5f;

                    Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;

                    //remove last ponts if too close, we need to leave space for the junction
                    for (int j = nextSplinePoints.Count - 1; j > 0; j--)
                    {
                        if (Vector3.Distance(hit.point, nextSplinePoints[nextSplinePoints.Count - 1]) < threshold)
                        {
                            nextSplinePoints.RemoveAt(nextSplinePoints.Count - 1);
                            j = nextSplinePoints.Count - 1;
                        }
                    }

                    nextSplinePoints.Add(controlPoint);
                    nextSplinePoints.Add(closestPointOnMesh);

                    //we have hit, break loop
                    break;
                }
                else
                    radius += 10f;
            }
            if (radius >= 500)
            {
                startSphereMissed = true;
               // Debug.Log("Start spherecast missed");
            }
        }



        nextSplinePoints.Reverse();



        //now do second half
        hit = new RaycastHit();
        prevPos = Vector3.zero;
        for (float i = f / 2; i < f; i += 0.1f)
        {
            if (prevPos != Vector3.zero)
                if (Vector3.Distance(spline.GetPoint(i * s), prevPos) < 1f)
                    continue;

            prevPos = spline.GetPoint(i * s);

            GameObject c = null;
            //  c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //  c.transform.position = spline.GetPoint(i*s);
            //  c.name = "test start";

            //check for any other road at road's width
            Vector3 offset1 = spline.GetPoint(i * s) + Quaternion.Euler(0, 90, 0) * spline.GetDirection(i * s) * 4f;
            Vector3 offset2 = spline.GetPoint(i * s) + Quaternion.Euler(0, -90, 0) * spline.GetDirection(i * s) * 4f;//4, 1 more than road width
            //RaycastHit hit;
            bool didHit = false;
            if (Physics.Raycast(offset1 + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Road")))
            {
                didHit = true;
            }
            if (!didHit)
            {
                if (Physics.Raycast(offset2 + Vector3.up * 5f, Vector3.down, out hit, 10f, LayerMask.GetMask("Road")))
                {
                    didHit = true;
                }
            }
            if (didHit)
            {
                //find the closest point on hit mesh
                targetRoadEnd = hit.transform.gameObject;
                //use the hit point from the ray to find the closest point on the target mesh
                Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, hit.point, hit.transform.gameObject);
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                /*
                c.transform.position = closestPointOnMesh;
                c.transform.name = "closest on mesh";
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                c.transform.name = "closest on mesh 2 ";
                */
                //get direction of the way the road is running
                Vector3 roadDir = (hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                                   //find which direction to create control point for the t junction in
                Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                Vector3 lookDir = Quaternion.Euler(0, 90, 0) * roadDir;
                //check which is closest - use that rotation to build 
                if (Vector3.Distance(closestPointOnMesh + lookDir1, nextSplinePoints[nextSplinePoints.Count - 1]) < Vector3.Distance(closestPointOnMesh + lookDir2, nextSplinePoints[nextSplinePoints.Count - 1]))
                    lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                else
                    lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction

                /*
                //clamp distance? or if it is too close, don't add cp?
                float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(hit.point, closestPointOnMesh);
                if (distanceBetweenThisAndClosestPointOnMesh < threshold * 0.5f)
                    distanceBetweenThisAndClosestPointOnMesh = threshold * 0.5f;

                Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;
                */
                Vector3 controlPoint = closestPointOnMesh + lookDir * threshold * .5f;
                //remove last ponts if too close, we need to leave space for the junction
                for (int j = nextSplinePoints.Count - 1; j > 0; j--)
                {
                    if (Vector3.Distance(hit.point, nextSplinePoints[nextSplinePoints.Count - 1]) < threshold)
                    {
                        nextSplinePoints.RemoveAt(nextSplinePoints.Count - 1);
                        j = nextSplinePoints.Count - 1;
                    }
                }

                nextSplinePoints.Add(controlPoint);
                nextSplinePoints.Add(closestPointOnMesh);


                /*
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = controlPoint;
                c.name = "cp end";

                */
                break;
            }

            //only add to the main curve very threshold point. or change to close to road cell point?

            if (nextSplinePoints.Count == 0)
            {
                nextSplinePoints.Add(spline.GetPoint(i * s));
            }
            else
            {
                //check if close to an orignal voronoi point, we want to keep using the shape the voronoi creates
                foreach(Vector3 v3 in roadPointsUsed)
                {
                    if (Vector3.Distance(v3, spline.GetPoint(i * s)) < 1f)
                    {
                        nextSplinePoints.Add(spline.GetPoint(i * s));
                        roadPointsUsed.Remove(v3);
                        break;
                    }
                }
            }
        }
        bool endSphereMissed = false;
        if (hit.transform == null && loopRoad == false)
        {
            // Debug.Log("End Road not found");
            float radius = 10f;
            while (radius < 500)
            {
                if (Physics.SphereCast(roadPointsForCell[roadPointsForCell.Count - 1] + Vector3.up * radius * 2, radius, Vector3.down, out hit, radius * 3, LayerMask.GetMask("Road")))
                {
                    //we have found another road - but we must first check that this road is not a loop road. Check if last point is closer to hit or first point
                    if (Vector3.Distance(roadPointsForCell[roadPointsForCell.Count-1], hit.point) > Vector3.Distance(roadPointsForCell[0], roadPointsForCell[roadPointsForCell.Count - 1]))
                    {
                        //the end of the road is closer than the hit point so flag as loop road and exit
                        Debug.Log("Ring road detected? From End");
                        loopRoad = true;
                        return;
                    }

                    //find the closest point on hit mesh
                    targetRoadEnd = hit.transform.gameObject;
                    //use the hit point from the ray to find the closest point on the target mesh
                    Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, hit.point, hit.transform.gameObject);
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = closestPointOnMesh;
                    c.transform.name = "closest on mesh spherecast end";
                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                    c.transform.name = "closest on mesh 2 spherecast end";
                    //get direction of the way the road is running
                    Vector3 roadDir = (hit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                                  //find which direction to create control point for the t junction in
                    Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                    Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                    Vector3 lookDir = Quaternion.Euler(0, 90, 0) * roadDir;
                    //check which is closest - use that rotation to build 
                    if (Vector3.Distance(closestPointOnMesh + lookDir1, nextSplinePoints[nextSplinePoints.Count - 1]) < Vector3.Distance(closestPointOnMesh + lookDir2, nextSplinePoints[nextSplinePoints.Count - 1]))
                        lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                    else
                        lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                    //clamp distance? or if it is too close, don't add cp?
                    float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(hit.point, closestPointOnMesh);
                    if (distanceBetweenThisAndClosestPointOnMesh < threshold * 0.5f)
                        distanceBetweenThisAndClosestPointOnMesh = threshold * 0.5f;

                    Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;

                    //remove last ponts if too close, we need to leave space for the junction
                    for (int j = nextSplinePoints.Count - 1; j > 0; j--)
                    {
                        if (Vector3.Distance(hit.point, nextSplinePoints[nextSplinePoints.Count - 1]) < threshold)
                        {
                            nextSplinePoints.RemoveAt(nextSplinePoints.Count - 1);
                            j = nextSplinePoints.Count - 1;
                        }
                    }

                    nextSplinePoints.Add(controlPoint);
                    nextSplinePoints.Add(closestPointOnMesh);

                    //we have hit, break loop
                    break;
                }
                else
                    radius += 10f;
            }
            if (radius >= 500)
            {
                endSphereMissed = true;
               // Debug.Log("End spherecast missed");
            }
        }

        if (startSphereMissed && endSphereMissed)
            loopRoad = true;

        


        //make road smarter? //possibly could be in hairpin removal loop
        for (int a = 0; a < nextSplinePoints.Count - 2; a++)//**do last?...
        {
            //check first against last//not doing, can remove whole road?
            int nextIndex = a + 1;
            int nextNextIndex = a + 2;
            if (nextIndex >= nextSplinePoints.Count)
                nextIndex -= nextSplinePoints.Count;
            if (nextNextIndex >= nextSplinePoints.Count)
                nextNextIndex -= nextSplinePoints.Count;

            if (Vector3.Distance(nextSplinePoints[a], nextSplinePoints[nextNextIndex]) < Vector3.Distance(nextSplinePoints[a], nextSplinePoints[nextIndex]))
            {
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.position = nextSplinePoints[nextIndex];
                c.name = "smart remove spline points";

                nextSplinePoints.RemoveAt(nextIndex);
                a = 0;
            }
        }

        foreach (Vector3 v3 in roadPointsForCell)
        {
           // GameObject c1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  c1.transform.position = v3;
          //  c1.name = "road point";
        }


        //make temp points the main points now
        spline.points = RingRoadMesh.AddControlPointsToList(gameObject, nextSplinePoints).ToArray();      

        float frequency = spline.points.Length*50;
        List<Vector3> vertices = new List<Vector3>();
        prevPos = Vector3.zero;
        float stepSize = 1f / frequency;
        
        for (float i = 0f; i < frequency; i += .1f)//half a road
        { 

            //ensures mesh is roughly even between - loop steps through at a small pace, but only paces a point every 1m(metere?) what is this?
            Vector3 position = spline.GetPoint((i) * stepSize);

           // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  c.transform.position = position;
          //  c.name = "curve p ";

            if (prevPos != Vector3.zero)
                if (Vector3.Distance(position, prevPos) < 1f)
                    continue;

            prevPos = position;
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

            vertices.Add(offset1);
            vertices.Add(offset2);
        }

        //chop off first two vertices, leaves a little room for joining junction mesh - could probably build that here and now
        if (!loopRoad)
        {
            vertices.RemoveAt(vertices.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);

            vertices.RemoveAt(0);
            vertices.RemoveAt(0);
        }
        else
        {
            vertices.Add(vertices[0]);
            vertices.Add(vertices[1]);
        }

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
        

    }

    void StartRoadCheck(bool onlyCheckJunction)
    {
        
        bool endFound = false;
        int safety = 0;
        float radius = 0f;

        for (int i = roadPointsForCell.Count / 2; i > 0; i--)
        {
            if (endFound)
                break;

            //skip past if not right at the end
            if (onlyCheckJunction)
                if (i > 4)
                    continue;

            Vector3 dirToNext = (roadPointsForCell[i - 1] - roadPointsForCell[i]).normalized;
            float distanceToNext = Vector3.Distance(roadPointsForCell[i], roadPointsForCell[i - 1]);
            float step = roadWidth * 0.1f;//quarter because it needs to hit on closest side of road

            for (float j = 0; j < distanceToNext; j += step)
            {
                Vector3 p = roadPointsForCell[i] + dirToNext * j;

                //GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //d.transform.position = p;
                //d.name = "shot from";
                RaycastHit roadHit;

                if (Physics.Raycast(p + Vector3.up * 10, Vector3.down, out roadHit, 20f, LayerMask.GetMask("Road")))
                {
                    if(onlyCheckJunction)
                        Debug.Log("Junction Check Hit!");

                    //save target for junction mesh
                    targetRoadStart = roadHit.transform.gameObject;
                    //use the hit point from the ray to find the closest point on the target mesh
                    Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, roadHit.point, roadHit.transform.gameObject);
                    //get direction of the way the road is running
                    Vector3 roadDir = (roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                    //find which direction to create control point for the t junction in
                    Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                    Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                    Vector3 lookDir = Vector3.zero;
                    //check which is closest - use that rotation to build 
                    if (Vector3.Distance(closestPointOnMesh + lookDir1, roadPointsForCell[i]) < Vector3.Distance(closestPointOnMesh + lookDir2, roadPointsForCell[i]))
                        lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                    else
                        lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                    //clamp distance? or if it is too close, don't add cp?
                    float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(roadPointsForCell[i], closestPointOnMesh);
                    Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;



                    //now alter end of road, insert the control point to try and make road approach target road at a perpendicular angle

                    //remove rest of list from the start, keeping i
                    for (int k = i - 1; k >= 0; k--)
                    {
                        roadPointsForCell.RemoveAt(k);
                    }
                    if (distanceBetweenThisAndClosestPointOnMesh < threshold / 2)
                    {
                        //can be oor cos list is altered 
                        //d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //d.transform.position = roadPointsForCell[i];
                        //d.name = "Removed at i";
                        //roadPointsForCell.RemoveAt(i);
                    }
                    //now put in the points we just found
                    //match y height of hit point to make junction smooth
                    controlPoint = new Vector3(controlPoint.x, roadHit.point.y, controlPoint.z);
                    roadPointsForCell.Insert(0, controlPoint);
                    roadPointsForCell.Insert(0, closestPointOnMesh);//too close?

                    endFound = true;

                    /*
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                    d.name = "count -1 Start";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = closestPointOnMesh;
                    d.name = "Closest on mesh Start";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                    d.name = "Closest on mesh 2 Start";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = controlPoint;
                    d.name = "CP Start";
                    
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadHit.point;
                    d.name = "road hit Start";

                   
                    if(onlyCheckJunction)
                        d.name = "road hit Start Junction Check";
                    
                     */
                    break;
                }
            }
        }

        while (!endFound && safety < 100)
        {
            safety++;
            radius += 1f;

            Vector3 p = roadPointsForCell[0] + Vector3.up * radius * 2;

           // GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
           // d.transform.position = p;
           // d.name = "shot from End";
            RaycastHit roadHit;
            if (Physics.SphereCast(p, radius, Vector3.down, out roadHit, radius * 3, LayerMask.GetMask("Road")))
            {
                //save target for junction mesh
                targetRoadEnd = roadHit.transform.gameObject;
                //use the hit point from the ray to find the closest point on the target mesh
                Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, roadHit.point, roadHit.transform.gameObject);
                //get direction of the way the road is running
                Vector3 roadDir = (roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                                  //find which direction to create control point for the t junction in
                Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                Vector3 lookDir = Vector3.zero;
                //check which is closest - use that rotation to build 
                if (Vector3.Distance(closestPointOnMesh + lookDir1, roadPointsForCell[0]) < Vector3.Distance(closestPointOnMesh + lookDir2, roadPointsForCell[0]))
                    lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                else
                    lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                //clamp distance? or if it is too close, don't add cp?
                float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(roadPointsForCell[0], closestPointOnMesh);
                distanceBetweenThisAndClosestPointOnMesh = Mathf.Clamp(distanceBetweenThisAndClosestPointOnMesh, threshold / 2, threshold);
                Vector3 controlPoint = roadPointsForCell[0] + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;

                //now put in the points we just found
                //match y height of hit point to make junction smooth
                controlPoint = new Vector3(controlPoint.x, roadHit.point.y, controlPoint.z);
                roadPointsForCell.Add(controlPoint);
                roadPointsForCell.Add(closestPointOnMesh);//too close?

                endFound = true;

                /*
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                d.name = "count -1 from Sphere Start";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = closestPointOnMesh;
                d.name = "Closest on mesh from Sphere Start";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                d.name = "Closest on mesh 2 from Sphere Start";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = controlPoint;
                d.name = "CP from Sphere Start";
                */
            }
        }
       // Debug.Log("Safety For End = " + safety);
    }

  
    void EndRoadCheck(bool onlyCheckJunction)
    {
        
        bool endFound = false;
        for (int i = roadPointsForCell.Count / 2; i < roadPointsForCell.Count - 1; i++)
        {
            if (endFound)
                break;

            //skip first half, only check very end
            if (onlyCheckJunction)
                if (i < roadPointsForCell.Count-5)
                    continue;

            Vector3 dirToNext = (roadPointsForCell[i + 1] - roadPointsForCell[i]).normalized;
            float distanceToNext = Vector3.Distance(roadPointsForCell[i], roadPointsForCell[i + 1]);
            float step = roadWidth * 0.1f;//quarter because it needs to hit on closest side of road
            for (float j = 0; j < distanceToNext; j += step)
            {
                Vector3 p = roadPointsForCell[i] + dirToNext * j;
               // GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
               // d.transform.position = p;
               // d.name = "shot from end";
                RaycastHit roadHit;

                if (Physics.Raycast(p + Vector3.up * 10, Vector3.down, out roadHit, 20f, LayerMask.GetMask("Road")))
                {
                    //save target for junction mesh
                    targetRoadEnd = roadHit.transform.gameObject;
                    //use the hit point from the ray to find the closest point on the target mesh
                    Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, roadHit.point, roadHit.transform.gameObject);
                    //get direction of the way the road is running
                    Vector3 roadDir = (roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                    //find which direction to create control point for the t junction in
                    Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                    Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                    Vector3 lookDir = Vector3.zero;
                    //check which is closest - use that rotation to build 
                    if (Vector3.Distance(closestPointOnMesh + lookDir1, roadPointsForCell[i]) < Vector3.Distance(closestPointOnMesh + lookDir2, roadPointsForCell[i]))
                        lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                    else
                        lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                    //clamp distance? or if it is too close, don't add cp?
                    float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(roadPointsForCell[i], closestPointOnMesh);
                    Vector3 controlPoint = closestPointOnMesh + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;



                    //now alter end of road, insert the control point to try and make road approach target road at a perpendicular angle

                    //remove rest of list,keeping i
                    for (int k = roadPointsForCell.Count - 1; k > i; k--)
                    {
                        roadPointsForCell.RemoveAt(k);
                    }

                    if (distanceBetweenThisAndClosestPointOnMesh < threshold / 2)
                    {
                        //can be oor cos list is altered 
                        //d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //d.transform.position = roadPointsForCell[i];
                        //d.name = "Removed at i";
                        //roadPointsForCell.RemoveAt(i);
                    }

                    //now put in the points we just found

                    roadPointsForCell.Add(controlPoint);
                    roadPointsForCell.Add(closestPointOnMesh);//too close?

                    endFound = true;

                    /*
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                    d.name = "count -1";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = closestPointOnMesh;
                    d.name = "Closest on mesh";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                    d.name = "Closest on mesh 2";
                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = controlPoint;
                    d.name = "CP";

                    d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.transform.position = roadHit.point;
                    d.name = "road hit";
                    */
                    break;
                }
            }
        }
        //if we never found and end, we need to spherecast for a nearby road - will never find on first ring - safety catches this
        int safety = 0;
        float radius = 0f;
        while (!endFound && safety < 100)
        {
            safety++;
            radius += 1f;

            Vector3 p = roadPointsForCell[roadPointsForCell.Count - 1] + Vector3.up * radius * 2;
            RaycastHit roadHit;
            if (Physics.SphereCast(p, radius, Vector3.down, out roadHit, radius * 3, LayerMask.GetMask("Road")))
            {
                //save target for junction mesh
                targetRoadEnd = roadHit.transform.gameObject;
                //use the hit point from the ray to find the closest point on the target mesh
                Vector3 closestPointOnMesh = ClosestPointOnMesh(out closestIndexOnMesh, roadHit.point, roadHit.transform.gameObject);
                //get direction of the way the road is running
                Vector3 roadDir = (roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2] - closestPointOnMesh).normalized;//maybe can go out of index
                                                                                                                                                                  //find which direction to create control point for the t junction in
                Vector3 lookDir1 = Quaternion.Euler(0, 90, 0) * roadDir;
                Vector3 lookDir2 = Quaternion.Euler(0, -90, 0) * roadDir;
                Vector3 lookDir = Vector3.zero;
                //check which is closest - use that rotation to build 
                if (Vector3.Distance(closestPointOnMesh + lookDir1, roadPointsForCell[roadPointsForCell.Count - 1]) < Vector3.Distance(closestPointOnMesh + lookDir2, roadPointsForCell[roadPointsForCell.Count - 1]))
                    lookDir = Quaternion.Euler(0, 90, 0) * roadDir;    //save as local direction
                else
                    lookDir = Quaternion.Euler(0, -90, 0) * roadDir; //save as local direction


                //clamp distance? or if it is too close, don't add cp?
                float distanceBetweenThisAndClosestPointOnMesh = Vector3.Distance(roadPointsForCell[roadPointsForCell.Count - 1], closestPointOnMesh);
                //clamp this to a min value, sphere can be very close and it doesnt allow enough space for a good cp
                distanceBetweenThisAndClosestPointOnMesh = Mathf.Clamp(distanceBetweenThisAndClosestPointOnMesh, threshold / 2, threshold);
                Vector3 controlPoint = roadPointsForCell[roadPointsForCell.Count - 1] + lookDir * distanceBetweenThisAndClosestPointOnMesh * .5f;

                //now put in the points we just found

                roadPointsForCell.Add(controlPoint);
                roadPointsForCell.Add(closestPointOnMesh);//too close?

                endFound = true;

                GameObject d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = roadPointsForCell[roadPointsForCell.Count - 1];
                d.name = "count -1 from Sphere";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = closestPointOnMesh;
                d.name = "Closest on mesh from Sphere";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = roadHit.transform.gameObject.GetComponent<MeshFilter>().mesh.vertices[closestIndexOnMesh + 2];
                d.name = "Closest on mesh 2 from Sphere";
                d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                d.transform.position = controlPoint;
                d.name = "CP from Sphere";

            }
        }
        //Debug.Log("Safety For End = " + safety);
    }

    void FindClosestHexPoint(Vector3 pos)
	{
		//finds the closest point (pos) on the static hex grid list

		float distance = Mathf.Infinity;
		Vector3 closestPoint = Vector3.zero;

		for( int i = 0; i < HexPointsUsed.hexPointsUsed.Count; i++)
		{
			float diff = Vector3.Distance(pos,HexPointsUsed.hexPointsUsed[i]);
			if (diff < distance)
			{
				distance = diff;
				closestPoint = HexPointsUsed.hexPointsUsed[i];
			}
		}

		closestPointOnHex = closestPoint;

	//	Transform cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;
	//	Transform newCube = Instantiate(cubePrefab,closestPointOnHex,Quaternion.identity) as Transform;
	//	newCube.name = "ClosestHex";

	}

    public static Vector3 ClosestPointOnMesh(out int index,Vector3 point, GameObject target)
    {
        Vector3 closestPoint = Vector3.one * Mathf.Infinity;

        Vector3[] vertices = target.GetComponent<MeshFilter>().mesh.vertices;
       
        float distance = Mathf.Infinity;
        int tempIndex = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            float temp = Vector3.Distance(point, vertices[i]);

            if (temp < distance)
            {
                distance = temp;
                closestPoint = vertices[i];
                tempIndex= i;
            }
        }
        index = tempIndex;
        return closestPoint;
    }
}
