using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointInserter : MonoBehaviour {
	
    //called from addToCells in Fields

	//As the ring roads are being built, they pass lists of v3s in to these lists
	public List<List<Vector3>> waypointlistListM = new List<List<Vector3>>();
	public List<List<Vector3>> waypointlistListL = new List<List<Vector3>>();
	public List<List<Vector3>> waypointlistListR = new List<List<Vector3>>();

	//Gameobjects lists
	public List<GameObject> waypointGameobjectListM = new List<GameObject>();

	public bool check;
	public Transform cubePrefab;
	// Use this for initialization
	void Start () {
					
	//StartCoroutine("InstantiateMiddleWaypointObjects");
	StartCoroutine("InstantiateWaypointObjectsLeft");
        //	InstantiateWaypointObjectsRight();
        //StartCoroutine("CheckForNeighbours");
        //	CheckForNeighbours();
        //	AddNeighboursFromHouses();


     //   PlaceWaypointsAtHouseCells();


        //CreateMaps();

		gameObject.GetComponent<BuildingUseAsign>().enabled = true;
		//trigger agents
	//	StartCoroutine("StartAgents");

	}
	
	void Update()
	{
		if(check)
		{
			
			check = false;
		}
	}

	IEnumerator StartAgents()
	{
		//stagger so agents do not have same home
		GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
		foreach (GameObject agent in agents)
		{
			agent.GetComponent<AgentBehaviour>().enabled = true;
			yield return new WaitForFixedUpdate();
		}
	}

    
	void AddNeighboursFromHouses()
	{
		//now we have created the neighbours we need to add the houses' waypoints to this

		//get all the waypoint gameobjects attached to the houses which have been built 
		GameObject[] waypointHouses = GameObject.FindGameObjectsWithTag("WaypointHouse");
        //now enable the neighbour checking script
        //this is the first and last child on the prefab
        float radiusSize = 3f;
        foreach (GameObject house in waypointHouses)
		{
			WaypointNode waypointNode = house.transform.GetChild(0).GetComponent<WaypointNode>();
            //spherecast looking for other waypoints
            LayerMask lm = LayerMask.GetMask("Waypoint");
            
			RaycastHit[] hits = Physics.SphereCastAll(waypointNode.transform.position + (Vector3.up *10),radiusSize, Vector3.down,20f,lm);
			//add this node to each hit waypoint as neighbour
			foreach(RaycastHit hit in hits)
			{                
                
                WaypointNode nodeHit = hit.transform.GetComponent<WaypointNode>();
                //do not not add other 'bus stops'
                if (nodeHit.transform.name != "WaypointNormal")
                    continue;

                waypointNode.neighbors.Add(nodeHit);

                //also, for each waypoint added to this waypoint's neighbor list
                //we need to add a node in the hit's list too, so it becomes a two way relationship                
                
                nodeHit.neighbors.Add(waypointNode);
			}

            waypointNode = house.transform.Find("WaypointLast").GetComponent<WaypointNode>();
			//spherecast looking for other waypoints
			//LayerMask lm = LayerMask.GetMask("WaypointL","WaypointR");

			hits = Physics.SphereCastAll(waypointNode.transform.position + (Vector3.up *10),radiusSize, Vector3.down,20f,lm);
			//add this node to each hit waypoint as neighbour
			foreach(RaycastHit hit in hits)
			{
                WaypointNode nodeHit = hit.transform.GetComponent<WaypointNode>();

                //do not not add other 'bus stops'
                if (nodeHit.transform.name != "WaypointNormal")
                    continue;

                waypointNode.neighbors.Add(nodeHit);

                //also, for each waypoint added to this waypoint's neighbor list
                //we need to add a nide in the hit's list too, so it becomes a two way relationship

                nodeHit.neighbors.Add(waypointNode);
            }

			//house.transform.GetChild(5).GetComponent<WaypointAdder>().AddWaypoints();
		}

        CreateMaps();

	}

    void AddNeighboursFromHouses2(List<GameObject> waypointParents)
    {
        //now we have created the neighbours we need to add the houses' waypoints to this
        
        foreach (GameObject houseCell in waypointParents)
        {
            //each waypoint is put inside its own game object. Go through these and add neighbours to them

            //waypoint gameobjects are held within "houseCell"
            for(int j = 0; j < houseCell.transform.childCount; j++)
            {
                WaypointNode waypointNode = houseCell.transform.GetChild(j).GetComponent<WaypointNode>();

                LayerMask lm = LayerMask.GetMask("Waypoint");
                RaycastHit[] hits = Physics.SphereCastAll(waypointNode.transform.position + (Vector3.up * 10), 3f, Vector3.down, 20f, lm);

                foreach (RaycastHit hit in hits)
                {

                    WaypointNode nodeHit = hit.transform.GetComponent<WaypointNode>();
                    //do not not add other 'bus stops'
                    if (nodeHit.transform.name != "WaypointNormal")
                        continue;

                    waypointNode.neighbors.Add(nodeHit);

                    //also, for each waypoint added to this waypoint's neighbor list
                    //we need to add a node in the hit's list too, so it becomes a two way relationship                

                    nodeHit.neighbors.Add(waypointNode);
                }

            }
        }

        CreateMaps();

    }

    void PlaceWaypointsAtHouseCells()
    {
        //grab all house cells
        GameObject[] houseCells = GameObject.FindGameObjectsWithTag("HouseCell");

        //keep a track of waypoint parents
        List<GameObject> parents = new List<GameObject>();
        for(int i = 0; i < houseCells.Length;i++)
        {
            //create a game object to hold the waypoints

            //drop a waypoint for every small cell the house cell is made of

            //check to see if this cell has already created a game object to hold the waypoints
            GameObject waypointsParent = null;
            if (houseCells[i].transform.parent.transform.Find("Waypoints Parent") == null)
            {
                //if no parent is found, create one
                waypointsParent = new GameObject();
                waypointsParent.transform.parent = houseCells[i].transform.parent.transform;
                waypointsParent.name = "Waypoints Parent";

                //add to list, to work on later
                parents.Add(waypointsParent);
            }
            else
                waypointsParent = houseCells[i].transform.parent.transform.Find("Waypoints Parent").gameObject;
                        
            //only do for the main combined mesh of the house plot. House plots are made up of smaller cells with the same tag

            if (houseCells[i].name == "Combined mesh")
            {
                continue;
            }            

            //find road edge by dropping a sphere collider looking for road layer
            RaycastHit hit;
            //centre of mesh
            Vector3 centre = houseCells[i].GetComponent<MeshRenderer>().bounds.center;
          
            if (Physics.SphereCast(centre+Vector3.up*20,10f,Vector3.down,out hit,40f,LayerMask.GetMask("Road")))
            {
                GameObject waypoint = new GameObject();
                waypoint.transform.parent = waypointsParent.transform;
                waypoint.transform.position = hit.point;
                waypoint.name = "WaypointHouse";
                waypoint.tag = "Waypoint";
                waypoint.layer = 17;

                WaypointNode wpn = waypoint.AddComponent<WaypointNode>();
                wpn.position = waypoint.transform.position;
                wpn.isActive = true;
                BoxCollider box = waypoint.AddComponent<BoxCollider>();
                box.enabled = true;
            }
        }

        AddNeighboursFromHouses2(parents);
    }

	//for each list of v3s in the master list, create waypoint objects for them
	IEnumerator InstantiateMiddleWaypointObjects()      ///not using central line
	{
        Debug.Log(waypointlistListM.Count);
		foreach(List<Vector3> waypointListM in waypointlistListM)
		{
			// create GameObject for waypoints to live
			GameObject waypoints = new GameObject();
			waypoints.name = "WaypointsMiddle";
			waypoints.transform.parent = this.transform;
			//create gameobjects for the waypoint script to grab 

				for (int i = 0; i < waypointListM.Count;i++)
				{

             //   if (i % 10 == 0)
               //     yield return new WaitForEndOfFrame();

					GameObject waypoint = new GameObject();
					waypoint.transform.parent = waypoints.transform;
					waypoint.transform.position = waypointListM[i];
					waypoint.name = "WaypointMiddle";
					waypoint.tag = "WaypointMiddle";
					waypoint.layer = 19;
					
					WaypointNode wpn = waypoint.AddComponent<WaypointNode>();
					wpn.position = waypoint.transform.position;
					wpn.isActive = true;
					BoxCollider box = waypoint.AddComponent<BoxCollider>();
					box.enabled =true;
                
				}
		}
        StartCoroutine("InstantiateWaypointObjectsLeft");
        yield break;
	}

	IEnumerator InstantiateWaypointObjectsLeft()
	{
		foreach(List<Vector3> waypointListL in waypointlistListL)
		{            
            // create GameObject for waypoints to live
            GameObject waypoints = new GameObject();
			waypoints.name = "Waypoints Left";
			waypoints.transform.parent = this.transform;
			//create gameobjects for the waypoint script to grab 
			for (int i = 0; i < waypointListL.Count;i++)
			{
               

                GameObject waypoint = new GameObject();
				waypoint.transform.parent = waypoints.transform;
				waypoint.transform.position = waypointListL[i];
                waypoint.name = "WaypointNormal";
				waypoint.tag = "Waypoint"; //used to be WaypointL - same with R, below
				waypoint.layer = 17;
				
				WaypointNode wpn = waypoint.AddComponent<WaypointNode>();
				wpn.position = waypoint.transform.position;
				wpn.isActive = true;
				BoxCollider box = waypoint.AddComponent<BoxCollider>();
				box.enabled =true;
			}
		}
        StartCoroutine("InstantiateWaypointObjectsRight");
        yield break;
	}

	IEnumerator InstantiateWaypointObjectsRight()
	{
		foreach(List<Vector3> waypointListR in waypointlistListR)
		{
			// create GameObject for waypoints to live
			GameObject waypoints = new GameObject();
			waypoints.name = "Waypoints Right";
			waypoints.transform.parent = this.transform;
			//create gameobjects for the waypoint script to grab 
			for (int i = 0; i < waypointListR.Count;i++)
			{
                //if (i % 100 == 0)
                //    yield return new WaitForEndOfFrame();

                GameObject waypoint = new GameObject();
				waypoint.transform.parent = waypoints.transform;
				waypoint.transform.position = waypointListR[i];
				waypoint.name = "WaypointNormal";
				waypoint.tag = "Waypoint";
				waypoint.layer = 17;
				
				WaypointNode wpn = waypoint.AddComponent<WaypointNode>();
				wpn.position = waypoint.transform.position;
				wpn.isActive = true;
				BoxCollider box = waypoint.AddComponent<BoxCollider>();
				box.enabled =true;
			}
		}

        StartCoroutine("CheckForNeighbours");

        yield break;
	}

	IEnumerator CheckForNeighbours()
	{
            //yield return new WaitForEndOfFrame();yield return new WaitForEndOfFrame();
            GameObject firstRingRoad = GameObject.Find("FirstRingRoad");
            GameObject ringChild = firstRingRoad.transform.Find("RingRoad").gameObject;
            //for every child of Agents Gameobject
            foreach (Transform child in transform)
		{	
			//create array of waypoint nodes in this particular child
			WaypointNode[] waypointNodes = child.GetComponentsInChildren<WaypointNode>();	
		//	LayerMask lm = LayerMask.GetMask("WaypointL","WaypointR");
			LayerMask lm = LayerMask.GetMask("Waypoint");
			LayerMask lmM = LayerMask.GetMask("WaypointMiddle");
                LayerMask lmRoad = LayerMask.GetMask("Road");
			for (int i = 0; i < waypointNodes.Length; i++)
			{
              //  Debug.Log(waypointNodes.Length);
              //  if (i % 100 == 0)
              //      yield return new WaitForEndOfFrame();

                //if first or last waypoint in chain //
                if (i == 0 || i == waypointNodes.Length - 1)
				{

                        //look to see if it is over the first ringroad
                        RaycastHit[] lookingForRingRoadHits = Physics.SphereCastAll(waypointNodes[i].transform.position + (Vector3.up),
                                                                                                                     4f,
                                                                                                                     Vector3.down,
                                                                                                                     2f,
                                                                                                                     lmRoad);

                    bool twoRoadsFound = false; 
                    foreach(RaycastHit hit in lookingForRingRoadHits)
                    {
                        if (hit.transform != ringChild.transform)
                            twoRoadsFound = true;
                    }

                    if(!twoRoadsFound)
                    {
                        //add the first to the last and vice versa

                        //join first point to last point
                        // waypointNodes[0].neighbors.Add(waypointNodes[1]);
                        if (child.name == "Waypoints Left")
                        {
                            waypointNodes[waypointNodes.Length - 1].neighbors.Add(waypointNodes[0]);
                            waypointNodes[0].neighbors.Add(waypointNodes[1]);
                        }
                        if (child.name == "Waypoints Right")
                        {
                            waypointNodes[0].neighbors.Add(waypointNodes[waypointNodes.Length -1]);
                            waypointNodes[waypointNodes.Length - 1].neighbors.Add(waypointNodes[waypointNodes.Length - 2]);
                        }
                    }

                    else if (twoRoadsFound)
                    {                        
                        RaycastHit[] results = Physics.SphereCastAll(waypointNodes[i].transform.position + (Vector3.up),
                                                                                                                     4f,
                                                                                                                     Vector3.down,
                                                                                                                     2f,
                                                                                                                     lm);
                        for (int j = 0; j < results.Length; j++)
                        {
                            //do not add "bus stop"2 house waypoints
                        //    if (results[j].transform.parent.parent.name == "House")
                        //        continue;
                            //add the 'hits' to the neighbours of this waypoint node	
                            waypointNodes[i].neighbors.Add(results[j].transform.GetComponent<WaypointNode>());
                            //Now we must also add this waypoint we are working on to the neighbours of the waypoints we just hit
                            //waypoints need a two way neighbour relationship, each on referencing the other
                            results[j].transform.GetComponent<WaypointNode>().neighbors.Add(waypointNodes[i]);
                        }
                    }
                    
                }
                    

					else if (i!=0 || i!= waypointNodes.Length - 1)
					{
						if(child.name == "Waypoints Left")
						{
							//else add the previous gameobject and the next gameobject from the game object list
							//this list is in the same order as the waypoints v3 list
							//WaypointNode  wlnPrevious = waypointNodes[i-1];
							WaypointNode  wlnNext = waypointNodes[i+1];
							//waypointNodes[i].neighbors.Add(wlnPrevious);
							waypointNodes[i].neighbors.Add(wlnNext);
						}

						if(child.name == "Waypoints Right")
						{
							//else add the previous gameobject and the next gameobject from the game object list
							//this list is in the same order as the waypoints v3 list
							WaypointNode  wlnPrevious = waypointNodes[i-1];
							//WaypointNode  wlnNext = waypointNodes[i+1];
							waypointNodes[i].neighbors.Add(wlnPrevious);
							//	waypointNodes[i].neighbors.Add(wlnNext);
						}

                        if (child.name == "WaypointsMiddle")
                        {
                        /* //do not add middle?
                            //make the middle waypoints two ways
                            WaypointNode wlnPrevious = waypointNodes[i - 1];                        
                            waypointNodes[i].neighbors.Add(wlnPrevious);
                            WaypointNode wlnNext = waypointNodes[i + 1];
                            waypointNodes[i].neighbors.Add(wlnNext);
                         */
                        }
					}
				}
//			}
			/*
//			WaypointNode[] waypointNodesL = gameObject.transform.FindChild("Waypoints Left").GetComponentsInChildren<WaypointNode>();
			LayerMask lmL = LayerMask.GetMask("WaypointL");
			for (int i = 0;i < waypointListL.Count; i++)
			{
		//		if(waypointNodes[i].gameObject.name !="WaypointL")
		//			continue;
				RaycastHit[] results = Physics.SphereCastAll(waypointListL[i] + (Vector3.up),4f,Vector3.down,2f,lmL);
				//			Debug.Log("results length " + results.Length); 
				
				//add the 'hits' to the neighbours of this waypoint node			
				for (int j = 0; j < results.Length; j++)
				{
					waypointNodesL[i].neighbors.Add(results[j].transform.GetComponent<WaypointNode>());
					//	yield return new WaitForFixedUpdate();
					
				}
			}

			WaypointNode[] waypointNodesR = gameObject.transform.FindChild("Waypoints Right").GetComponentsInChildren<WaypointNode>();
			LayerMask lmR = LayerMask.GetMask("WaypointR");
			for (int i = 0;i < waypointListR.Count; i++)
			{
				//		if(waypointNodes[i].gameObject.name !="WaypointL")
				//			continue;
				RaycastHit[] results = Physics.SphereCastAll(waypointListR[i] + (Vector3.up),4f,Vector3.down,2f,lmR);
				//			Debug.Log("results length " + results.Length); 
				
				//add the 'hits' to the neighbours of this waypoint node
				
				for (int j = 0; j < results.Length; j++)
				{
					waypointNodesR[i].neighbors.Add(results[j].transform.GetComponent<WaypointNode>());
					//	yield return new WaitForFixedUpdate();
					
				}
			}
			*/
		}//end of foreach


        //AddNeighboursFromHouses();
        PlaceWaypointsAtHouseCells();
		yield break;
	}

	void CreateMaps()
    {
        //create map for the normal, most used waypoints
        gameObject.GetComponent<WaypointPathfinder>().enabled = true;
        gameObject.GetComponentInParent<WaypointPathfinder>().MakeNewMap();

        //create a map for the central points
        //      gameObject.transform.FindChild("MiddleWaypoints").GetComponent<WaypointPathfinder>().MakeNewMap();

        SpawnAgents();
    }

    void SpawnAgents()
    {
        gameObject.AddComponent<SpawnAgents>();
    }
	
	
}
