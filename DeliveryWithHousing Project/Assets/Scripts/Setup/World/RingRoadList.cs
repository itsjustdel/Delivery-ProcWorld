using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RingRoadList : MonoBehaviour {
	
	public List<Vector3> ringRoadPoints = new List<Vector3>();
	public List<Vector3> roadMasterList = new List<Vector3>();
	public List<Vector3> duplicatesThatNeedToBeAdded = new List<Vector3>();
//	public GameObject cubePrefab;
	// Use this for initialization
	public bool doCubes;
	public bool firstRing;
	public int junctionWidth = 0;
	public GameObject targetCell;
	public GameObject targetCell2;

	public int startInd;
	public int endInd;
	public Mesh startMesh;
	public Mesh endMesh;
	//this script gathers all the gridplayers paths and inserts them in to one list which the road mesh uses

	public List<CellManager.PositionAndOwner> masterList;
	public WaypointPlayer[] childArray;
	public int closestInd = 0;
    public Mesh otherRoadMesh;// = new Mesh();
	public Vector3 closestPointOnHex;

    
	void Start () {






        //childArray  = gameObject.GetComponentsInChildren<WaypointPlayer>();
        //
        

        //CreateMasterList();

		//AddMiddleOfRoadFromMasterList();


        //AddStartLink();
        
		//UpdateMasterList();
        //AdjustStartAndEnd();    //update masterlist first then skim off points?
        // WindyRoadRemove();
        //FillInGaps();

        // AddJunctionTriggers();



        //new

        SimpleAdd();


    }

	

        
	void CreateMasterList()
	{
		//masterList =  GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList;	


		for (int i = 0; i < childArray.Length;i++)
		{			
			for (int j = 0 ; j< childArray[i].returnedPath.Count; j++)
			{
				roadMasterList.Add( childArray[i].returnedPath[j] );
			}

		}

	}

    void SimpleAdd()
    {
        //grab waypoint placer list
        List<Vector3> waypointList = GetComponent<PlaceWaypointPlayer>().roadPointsForCell;
        for (int i = 0; i < waypointList.Count; i++)
        {
            ringRoadPoints.Add(waypointList[i]);

           // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  c.name = "Simple Add " + i;
           // c.transform.parent = this.transform;
          //  c.transform.position = waypointList[i];
        }
    }

	void AddMiddleOfRoadFromMasterList()
	{
		//ringroadpoints = the templist we are creating
		//roadmasterlist = all the road points in this cell
		//hexpoints used = global positions of roads already built
		
        bool lastOneWasDuplicateChild = false;

       // Debug.Log(roadMasterList.Count);
		for (int i = 0; i < roadMasterList.Count; i++)
		{
			bool duplicate = false;
			//check the global positions //
			if(!firstRing)
			{
                
				//check for master list duplicate
				if(HexPointsUsed.hexPointsUsed.Contains(roadMasterList[i]))
				{
                  //  GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                  //  c.name = "Duplicate Master" + i;
					//c.transform.parent = this.transform;
                  //  c.transform.position = roadMasterList[i];

					if(i > 2)
					{
						ringRoadPoints.Add(roadMasterList[i] );
					}
					else if(i <= 2)
					{
						//clear the list and start again, we have tripped over a road
						ringRoadPoints.Clear();
					}

					duplicate = true;

					//do not abort if on the first point
					if(i > 2)
					{
						return;
					}
				}
			}

			//check the local positions
			if(ringRoadPoints.Contains( roadMasterList[i] ))
			{

                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.name = "Duplicate Child" + i;
				c.transform.parent = this.transform;
                c.transform.position = roadMasterList[i];
				//even though we are not including this point in our road,we stillneed to let the global list know there is a road near here
				duplicatesThatNeedToBeAdded.Add(roadMasterList[i]);

	//			ringRoadPoints.Add(roadMasterList[i] );
				duplicate=true;

				if (lastOneWasDuplicateChild)
				{
					ringRoadPoints.RemoveAt(ringRoadPoints.Count-1);
				}

				lastOneWasDuplicateChild = true;
	//			if(!firstRing)
	//				return;
			
			}


			if(!duplicate)
			{	
				lastOneWasDuplicateChild = false;
               

				//if not already in list, add it			
				ringRoadPoints.Add(roadMasterList[i]);
			}
		}

		//if we made it to the end (not hitting another road or winding back on this road we are plotting)
		//find the closest point on the hex grid
		if(!firstRing)
		{
			Vector3 lastPoint = roadMasterList[roadMasterList.Count-1];

		//	Transform newCube = Instantiate(cube,lastPoint,Quaternion.identity) as Transform; 
		//	newCube.name = "lastPoint";
		//	newCube.transform.parent = this.transform;

			RaycastHit hit;
			if(!Physics.Raycast(lastPoint+Vector3.up,Vector3.down,out hit,2f,LayerMask.GetMask("Road")))
				{
                /*
				Transform newCube2 = Instantiate(cube,lastPoint,Quaternion.identity) as Transform; 
				newCube2.name = "RAYCAST GAMEOBJECT NULL";
				newCube2.transform.parent = this.transform;
				neCube2.localScale *= 10;
                */
                Debug.Log("Raycast Null - ringroadlist");
				}

//			GameObject targetGameobject= hit.transform.gameObject;


			//TODO if hit is null here, it is completely fucked - reload?



			//FindClosestHexPoint(roadMasterList[roadMasterList.Count-1]);
//			Vector3 closestMeshPoint = FindPointOnMesh(lastPoint,targetGameobject);
//			ringRoadPoints.Add(closestMeshPoint);


			//find point half way to junction to add a controlling point, stopping one stretch of road becoming too long
	//		Vector3 halfway = Vector3.Lerp(roadMasterList[roadMasterList.Count-1],closestPointOnHex,0.5f);
			//add halfway point
	//		ringRoadPoints.Add(halfway);

	//		Transform newCube = Instantiate(cubePrefab,halfway,Quaternion.identity) as Transform; 
	//		newCube.name = "childCube OK";
	//		newCube.transform.parent = this.transform;


			//add final point, the point we found on hex grid/existing road network
			//ringRoadPoints.Add(closestPointOnHex);//being done with emsh search above now, dec 29
		}
        if (firstRing)
            if (ringRoadPoints.Count > 0)
                ringRoadPoints.Add(ringRoadPoints[0]);
            else
                Debug.Log("no ring road points for first loop");
	}


	void UpdateMasterList()
	{
		//now we can add our local points to the master list

	//	ringRoadPoints = ringRoadPoints.Distinct().ToList();

		foreach(Vector3 v3 in ringRoadPoints)
		{
			HexPointsUsed.hexPointsUsed.Add(v3 + transform.position);
		}

		//also the duplicate child list
		foreach(Vector3 v3 in duplicatesThatNeedToBeAdded)
		{
			HexPointsUsed.hexPointsUsed.Add(v3 + transform.position);
		}

		//Debug.Log(HexPointsUsed.hexPointsUsed.Count + "HEXCOUNT");
	}

	void AddStartLink()
	{

		///Add the start

		if(!firstRing)
		{
			
			FindClosestHexPoint(ringRoadPoints[0]);

			Vector3 middle = Vector3.Lerp(ringRoadPoints[0],closestPointOnHex,0.5f);

			ringRoadPoints.Reverse();
			ringRoadPoints.Add(middle);
			ringRoadPoints.Add(closestPointOnHex);
			ringRoadPoints.Reverse();

			//		Transform newCube = Instantiate(cubePrefab,closestPointOnHex,Quaternion.identity)as Transform;
			//		newCube.name = "StartHex";

		}
	}

	void AddEndLink()
	{

		///////////////////////
		/// Add the last section

		if (firstRing)
		{
			//Add the first element to the end to create a closed loop
			Vector3 middle = Vector3.Lerp( ringRoadPoints[ringRoadPoints.Count-1],ringRoadPoints[0],0.5f);

			ringRoadPoints.Add(middle);
			ringRoadPoints.Add(ringRoadPoints[0]);

		}
		else if (!firstRing)
		{
			//find the closest point on the mesh that we are connecting this road to
			//now search the mesh for the closest point to the end of the road			

			//			Vector3 lastPoint = transform.GetComponent<RingRoadList>().ringRoadPoints[ringRoadPoints.Count-1];
			//			FindClosestHexPoint(lastPoint);
			//			ringRoadPoints.Add(closestPointOnHex);

		}
	}

	void WindyRoadRemove()
	{
		Transform cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;

		//runs through finished list and checks for and unnecessary kinks. Kinks happen when duplicates are removed in a certain way.

		for (int i = 0; i < ringRoadPoints.Count-3; i++) //dont do it for the last 3 points was 
		{
			int current = i;
			int first = i + 1;
			int second = i + 2;
			int third = i + 3;

			//is the first point closer to the third one, or is the second point closer to the third one
			float distanceFromFirst = Vector3.Distance(ringRoadPoints[ first ] ,ringRoadPoints[ third ]);
			float distanceFromSecond = Vector3.Distance(ringRoadPoints[ second ],ringRoadPoints[ third ]);
           

            if ( distanceFromFirst > distanceFromSecond)
            //take away points that are too close to each other
            
            //if (distanceFromFirst < distanceFromSecond)
            {

                ///	Transform newCube = Instantiate(cubePrefab,ringRoadPoints[second],Quaternion.identity) as Transform; 
                //	newCube.name = "naughty hairpin";
                //	newCube.transform.parent = this.transform;

                // we have found a hairpin, remove the naughty point - (the second one)
                ringRoadPoints.RemoveAt(second);

            }


		}
	}

    void FillInGaps()
    {

    }

	void AdjustStartAndEnd()
	{		
		if(!firstRing)
			{
			//search the nearest point on the mesh at the start and the end, change the start and the end to these new points
		

			Vector3 first = ringRoadPoints[0];
			//RaycastHit hit;
			//Physics.Raycast(first+(Vector3.up*100),Vector3.down,out hit,200f,LayerMask.GetMask("Road"));

			RaycastHit[] hits = Physics.SphereCastAll(first + (Vector3.up*100),100f,Vector3.down,200f,LayerMask.GetMask("Road")); //100 twice the hex size
			//

			List<Vector3> closestPoints = new List<Vector3>();

			foreach (RaycastHit hit in hits)
			{
				if(hit.transform.tag != "Road")
					continue;

				GameObject targetGameobject= hit.transform.gameObject;
				//search for the closest point to the second one in the list. it is the first point we are moving
				Vector3 closestMeshPointFirst = FindPointOnMesh(ringRoadPoints[1],targetGameobject);

				//insert in to a list which is checked belowe
				closestPoints.Add(closestMeshPointFirst);
			}	

			float distance = Mathf.Infinity;
			Vector3 closestPoint = Vector3.zero;

			for(int i = 0; i < closestPoints.Count; i++) // -1 to save the index for +1 for lerping below
			{
				float diff = Vector3.Distance(ringRoadPoints[1], closestPoints[i]);

				if (diff < distance)
				{				
					closestPoint = closestPoints[i];
					distance = diff;
				}
			}

			if (closestPoints.Count != 0)
				ringRoadPoints[0] = closestPoint;

			//Last Point

			//check to see if last point is on top of a road already, if it is remove it, it will look unnatural
			Vector3 last = ringRoadPoints[ringRoadPoints.Count-1];

			RaycastHit check;
			LayerMask lm = LayerMask.GetMask("Road");

			if(Physics.Raycast(last+ (Vector3.up*100), Vector3.down, out check,200f,lm))
			{
				if(check.transform.tag == "Road")
					ringRoadPoints.RemoveAt(ringRoadPoints.Count-1);
			}			
			
			RaycastHit[] hits2 = Physics.SphereCastAll(last + (Vector3.up*500),160f,Vector3.down,1000,lm); //100 is twice hex size //hex size is 80 now
			Vector3 closest2 = Vector3.zero;
			float distance2 = Mathf.Infinity;

			foreach(RaycastHit hit2 in hits2)
			{
				if(hit2.transform.tag != "Road")
					continue;
				
				GameObject targetGameobject2 = hit2.transform.gameObject;
				Vector3 closestMeshPointLast = FindPointOnMesh(ringRoadPoints[ringRoadPoints.Count-1],targetGameobject2);

				float diff2 = Vector3.Distance(closestMeshPointLast, closest2);
				if(diff2 < distance2)
				{
					distance2 = diff2;
					closest2 = closestMeshPointLast;
				}
			}

	//		ringRoadPoints[ringRoadPoints.Count-1] = closest2;
			ringRoadPoints.Add(closest2);

		}
	}

	Vector3 FindPointOnMesh(Vector3 point, GameObject target)
	{
		
		Mesh mesh = target.GetComponent<MeshFilter>().mesh;

		float distance = Mathf.Infinity;
		Vector3 closestPoint = Vector3.zero;

		for(int i = 0; i < mesh.vertexCount-1; i++) // -1 to save the index for +1 for lerping below
		{
			float diff = Vector3.Distance(point, mesh.vertices[i]);

			if (diff < distance)
			{				
				closestPoint = Vector3.Lerp( mesh.vertices[i],mesh.vertices[i+1],0.5f);
				distance = diff;
			}
		}

		return closestPoint;
	}

	void FindClosestHexPoint(Vector3 pos)
	{
		//finds the closest point (pos) on the static hex grid list

		float distance = Mathf.Infinity;
		Vector3 closestPoint = Vector3.zero;

		for( int i = 0; i < HexPointsUsed.hexPointsUsed.Count; i++)
		{
			//we dont wwant to find a point on this cell's road,pass over if it is in our local list
			if(ringRoadPoints.Contains(HexPointsUsed.hexPointsUsed[i]))
				continue;

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

	void FindClosestPointOnMesh(Vector3 firstPoint)
	{
		//Vector3 firstPoint = transform.TransformPoint( childArray[0].Path[0] );//+ transform.position; //previously path[0] //if we do it from thesecond point, it joins in with road better


		firstPoint = new Vector3(firstPoint.x,0f,firstPoint.z);
		Debug.Log(firstPoint + "FIRSTP");
		//search for the nearest point in the master list
		float distance = Mathf.Infinity;
		Vector3 roadStart = Vector3.zero;
		GameObject theCellWeWant  = gameObject;
		for(int i = 0; i < masterList.Count; i++)
		{

			if ( masterList[i].owner.transform != gameObject.transform)
			{
				Transform child = masterList[i].owner.transform;//.FindChild("RingRoad");
				targetCell = child.gameObject;
				Transform ringRoad = targetCell.transform.Find("RingRoad");
				Mesh mesh= ringRoad.GetComponent<MeshFilter>().mesh;
				for(int j = 0; j < mesh.vertexCount; j++)
				{
					
					float diff = Vector3.Distance(firstPoint,mesh.vertices[j]);
					if (diff < distance)
					{
						distance = diff;
						theCellWeWant = masterList[i].owner;
						//targetCell = masterList[i].owner;
						closestInd = j;	
					}
				}
			}
		}
		targetCell = theCellWeWant;
		otherRoadMesh = theCellWeWant.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh;
	}

	void AddJunctionTriggers()
	{
        if (!firstRing)
        {
            AddJunctionTrigger(ringRoadPoints[0]);
            AddJunctionTrigger(ringRoadPoints[ringRoadPoints.Count - 1]);
        }
	}

	void AddJunctionTrigger(Vector3 middlePoint)
	{
        

		//Instantiate a junctionTrigger at this point
		GameObject jTrigger = new GameObject();
		jTrigger.name = "JunctionTrigger";
		jTrigger.tag = "JunctionTrigger";
        jTrigger.transform.parent = transform;
        jTrigger.layer = 23;
		SphereCollider sphere = jTrigger.AddComponent<SphereCollider>();
		sphere.gameObject.AddComponent<JunctionChecker2>();
        sphere.transform.position = middlePoint;
		//Vector3 size = new Vector3(20,20,20);
        sphere.radius = 5;
        sphere.isTrigger = true;

        

        /*
		//Instantiate a larger junctionTrigger to let the cars adjust for an upcoming junction
		GameObject jTrigger2 = new GameObject();
		jTrigger2.name = "JunctionTriggerLarge";
		jTrigger2.tag = "JunctionTriggerLarge";
        jTrigger2.transform.parent = transform;
        BoxCollider box2 = jTrigger2.AddComponent<BoxCollider>();
		box2.gameObject.AddComponent<JunctionCheckerLarge>();
		box2.transform.position = middlePoint;
		Vector3 size2 = new Vector3(50,50,50);
		box2.size = size2;
		box2.isTrigger = true;
        */

        //Add This point to a list to create mesh around junctions
     //   GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<JunctionCells>().points.Add(middlePoint);    not used
	}


    void RemoveDuplicatePointsAtJoins()
	{
		//remove shitty pathfinders
		/*
		for (int i = 0; i < childArray.Length;i++)
		{
			if (childArray[i].Path.Count < 3)
			{
				Destroy(childArray[i]);
			}
		}
		*/
		//Because there are multiple pathfinders finding their way to each other, sometimes they can go the slightly wrong way when starting and finishing their routes
		//This function checks for any duplicates at rthe start/ends of the paths the pathfinders return and leave just one point, leaving the road with a imple smooth turn

		Transform cubePrefab = Resources.Load("Cube",typeof (Transform)) as Transform;

		for (int i = 0; i < childArray.Length - 1; i++)
		{
			for (int j = 0; j < childArray[i].Path.Count; j++)
			{			
				bool duplicate = false;
				List<int> tempList = new List<int>();
				for (int k = 0; k < childArray[i+1].Path.Count; k++)
				{						
					if(childArray[i].Path[j] == childArray[i+1].Path[k])
					{
						Transform newCube2 = Instantiate(cubePrefab,childArray[i].Path[j],Quaternion.identity) as Transform; 
						newCube2.name = "Removed Point" + i;
						newCube2.transform.parent = this.transform;

						duplicate = true;
						tempList.Add(k);
					}
				}

				if(duplicate)
					childArray[i].Path.RemoveAt(j);

				foreach(int index in tempList)
				{
					childArray[i+1].Path.RemoveAt(index);
				}
			}
		}
	}
    
	void PlaceNodes()
	{

		//Places Nodes and creates Map

		WaypointPathfinder waypointPathFinder = gameObject.AddComponent<WaypointPathfinder>();

		List<WaypointNode> nodeListForMap = new List<WaypointNode>();
		//create nodes for this to use at our road points

		foreach (Vector3 v3 in roadMasterList)
		{
			GameObject nodeGo = new GameObject();
			nodeGo.transform.parent= this.transform;
			nodeGo.transform.position = v3;
			nodeGo.name = "SmoothingRoadPoint";
			nodeGo.tag = "SmoothingRoadPoint";
			nodeGo.layer = 21;
			WaypointNode node = nodeGo.AddComponent<WaypointNode>();
			node.position = v3;
			node.isActive = true;
			BoxCollider box = nodeGo.AddComponent<BoxCollider>();
			Vector3 size = new Vector3(0.1f,0.1f,0.1f);
			box.size = size;

			//add to a list to be used for the map
			nodeListForMap.Add(node);
		}

		//insert in to Map
		//set array sizes
		waypointPathFinder.Map = new WaypointNode[nodeListForMap.Count];
		for (int i = 0; i < nodeListForMap.Count ; i++)
		{
			waypointPathFinder.Map[i] = nodeListForMap[i];
			waypointPathFinder.Map[i].ID = i;
		}

		waypointPathFinder.openList = new WaypointListNode[waypointPathFinder.Map.Length];
		waypointPathFinder.closedList = new WaypointListNode[waypointPathFinder.Map.Length];

	}

	void FindNeighbours()
	{
		HexGridRectangle hexGridRectangle = GameObject.Find("HexGrid").GetComponent<HexGridRectangle>();
		float rayWidth = hexGridRectangle.hexSize + 1f;
		int heightLimit = 1;
		WaypointNode[] nodes = transform.GetComponentsInChildren<WaypointNode>();
		Debug.Log(nodes.Length);
		LayerMask lm = LayerMask.GetMask("HexWaypoint");

		foreach(WaypointNode node in nodes)
		{

			//casts a thick ray of width 'rayWidth' downwards,looking for other waypoints
			RaycastHit[] results = Physics.SphereCastAll(node.transform.position + (heightLimit*Vector3.down*0.5f),
				rayWidth,
				Vector3.down,
				heightLimit,
				lm);

			//			Debug.Log(results.Length);

			for (int j = 0; j < results.Length; j++)
			{

				if(results[j].transform != node.transform)
				{

					//check for height
					float heightDiff =  results[j].transform.position.y - node.transform.position.y;
					heightDiff = Mathf.Abs(heightDiff);
					//					Debug.Log(heightDiff);
					if(heightDiff < heightLimit)
					{

						//add the 'hits' to the neighbours of this waypoint node	
						//			if(!results[j].transform.GetComponent<WaypointNode>().neighbors.Contains(node))	
						node.neighbors.Add(results[j].transform.GetComponent<WaypointNode>());
						//Now we must also add this waypoint we are working on to the neighbours of the waypoints we just hit
						//waypoints need a two way neighbour relationship, each on referencing the other
						//ony do this if the neighbour isn not in the list already
						//			if(!results[j].transform.GetComponent<WaypointNode>().neighbors.Contains(node))						
						results[j].transform.GetComponent<WaypointNode>().neighbors.Add(node);
					}
				}			
			}
		}
	}

	void FindBestRoute()
	{
		//plot a course through the new nodes
		//add pathfinder to cell

		WaypointPlayer waypointPlayer = gameObject.AddComponent<WaypointPlayer>();
		waypointPlayer.waypointPathFinder = gameObject.GetComponent<WaypointPathfinder>();
	}
    /*
	void Cubes()
	{
		foreach ( Vector3 v3 in ringRoadPoints)
			Instantiate(cube,v3,Quaternion.identity);
	}
    */

	

}
