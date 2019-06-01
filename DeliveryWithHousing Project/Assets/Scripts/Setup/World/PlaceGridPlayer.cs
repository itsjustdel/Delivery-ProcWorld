using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class PlaceGridPlayer : MonoBehaviour {

	//this script places a gridplayer ( a pathfinding object) at each of the vertices round the p[olygon.
	public bool ringRoad;
	public List<Vector3> sharedPoints = new List<Vector3>();
	public List<Vector3> sharedPointsDistinct;
	void Awake()
	{
		//enabled = false;
	}

	// Use this for initialization
	void Start () {
	//	if(ringRoad)
	//		PlaceGridPlayersForInitialLoop();
	//	else
	//		PlaceGridPlayersForJoiningLoop();
		PlaceGridPlayers();

	}

	void PlaceGridPlayers()
	{
		//place gridplayers round the edge of the mesh, stopping at any "shared" points (junctions)

		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		//masterlist of all shared points. Each cell updates this after it builds its road
		List<CellManager.PositionAndOwner> sharedList = GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList;
		//a list of the indices which share a position with another road point
		List<int> sharedVertices = new List<int>();

		//Find what vertices from this cell's mesh match the position of any v3s in the master list of all shared points

		//don't do if list is empty ( this will be the first cell)
		if (sharedList.Count !=0)
		{

	//		Debug.Log(sharedList.Count);
	//		Debug.Log(mesh.vertexCount);
			for (int i = 0; i < sharedList.Count;i++)
			{
				for(int j = 0; j < mesh.vertexCount;j++)
				{
					if (sharedList[i].position == mesh.vertices[j])
					{
						//if we have a match, add to the cell's sharedVertices list
						sharedVertices.Add(j);
//						Debug.Log("adding shared vertice");
					}
				}
			}

		}
	

		//Now Find where to start and finish the road

		//create a ring for the first cell
		if (sharedList.Count == 0)
		{

			//if it is the first ring road created, it does not matter where we start the raod from
			//because it is a singular loop
	//		Debug.Log("innn");
			for (int i = 0; i < mesh.vertexCount; i++)
			{
//				Debug.Log("placing grid player");
				//skip first mesh vertice as it is the always the center of the mesh
				if(i==0)
					continue;

				//Place pathfinders round the edges of the mesh
				GameObject gpgo = new GameObject();
				gpgo.transform.parent = this.transform;
				gpgo.transform.position = mesh.vertices[i];
				gpgo.name = "PathFinder"+i;
				
				GridPlayer gridPlayer = gpgo.AddComponent<GridPlayer>();

				//this piece of code makes sure the vertices dont go out of range, instead
				//it loops the index number round back to the start of the list/array to make a loop

				//target is the next vertice on the mesh
				int index =  i + 1;
				if (index < mesh.vertexCount)
				{
					gridPlayer.target = mesh.vertices[index];
				}
				else
				{
					index -= mesh.vertexCount;
					//jump over central vertice
					index += 1;
					gridPlayer.target = mesh.vertices[index];
				}

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

				//sharedList.Add(mesh.vertices[i]);
			}
			//feed the updated list back to cell manager
			GameObject.Find("WorldPlan").GetComponent<CellManager>().masterRoadList  = sharedList;

		}

		//create a joining ring for all others
		else
		{

			//Find a vertice in this cell's mesh which is in the shared list, and start the road from there

			//first, create a list of indices which share their position with any positions in the master road list
			List<int> sharedIndices = new List<int>();
			for (int i = 0; i < mesh.vertexCount; i++)
			{
				for (int j = 0; j < sharedList.Count; j++)
				{
					if( mesh.vertices[i] == sharedList[j].position)
						sharedIndices.Add(i);
				}
			}

			//sort this list in to numerical order
			sharedIndices.Sort();

			 
			//Look for the start of the road
			int lastItem = sharedIndices[sharedIndices.Count-1];

			//make sure the index "wraps back round" to the start of the mesh array insetad of going out of index
			if (lastItem < mesh.vertexCount)
			{
				//leave
			}
			else
			{
				Debug.Log("changing last item"); //is this needed??
				lastItem -= mesh.vertexCount;
				//add 1 to skip central mesh point - 1st mesh in array is always the central one
				lastItem += 1;
			}

		//	bool changed = false;
			int tempint = 0;

			//find start of the road
			//check to see if next vertice is in shared list, if it is start the road from the next vertice instead

			//TODO what if next vertice is in the list, but also this new vertice is in the list too? //maybe this is it below???? :(r5

	//		Debug.Log(sharedIndices.Count);
			for (int j = 0; j < sharedIndices.Count; j++)
			{
				bool changed = false;
				for(int i = 0; i < sharedList.Count; i++)
				{

						//alter the index so it is not outwith array size(loop it)
						int index = lastItem + 1;

						if (index < mesh.vertexCount)
						{
							//leave index the same
						}
						else
						{
							index -= mesh.vertexCount;
							index +=1;
						}	

						if (mesh.vertices[index] == sharedList[i].position)
						{
							tempint = index;
							changed = true;				
						}
				}

				if (changed)
				{
					lastItem = tempint;
					Debug.Log("changing last item " + j);
				}
			}


			//last item is not the start of the road

			//iterate through mesh and add to roadPoints list if a valid point
			List<int> roadPoints = new List<int>();
			List<int> roadPointsTargets = new List<int>();
			//add the first point on the road
			roadPoints.Add(lastItem);
			int endIndex = 100;
		//	Debug.Log("mesh vertex count" + mesh.vertexCount);
			for (int i = 0; i < mesh.vertexCount-1 ; i++) //- 1 to ignore centre of mesh
			{
				//make sure insex is not out of range
				int index = lastItem + i;
				if (index < mesh.vertexCount)
				{
					//leave index as is
				}
				else
				{
					index -= mesh.vertexCount;
					index += 1;	
				}

				//only add points on the polygon which are not in the shared list
				if(!sharedIndices.Contains(index))
				{
					roadPoints.Add(index);

					//work out what the current end ind is
					//this will get rewritten until the last time the loop runs
					endIndex = index + 1;
					if (endIndex < mesh.vertexCount)
					{
						//leave
					}
					else
					{
						endIndex -= mesh.vertexCount;
						endIndex += 1;
					}
				}
			}
			//now add the target for the road
			roadPoints.Add(endIndex);

	//		Transform cube4 = Instantiate(cubePrefab,mesh.vertices[endIndex],Quaternion.identity) as Transform;
	//		cube4.name = "end " + endIndex;
	//		cube4.localScale *=20;
	//		cube4.parent =this.transform;

			//Create objects for gridPlayers and place them at the positions in the roadPoints list
			//don't place for the last point, this is the last rp's target
			for (int i = 0; i < roadPoints.Count-1; i++)
			{
				GameObject gpgo = new GameObject();
				gpgo.transform.parent = this.transform;
				gpgo.transform.position = mesh.vertices[ roadPoints[i] ];
				gpgo.name = "PathFinder"+i;
				GridPlayer gridPlayer = gpgo.AddComponent<GridPlayer>();

				//add the gridplayer's targets
				//if(i!=roadPoints.Count-1)
					gridPlayer.target = mesh.vertices[ roadPoints[i+1] ]; //TODO out of range error here

			//	if(i==roadPoints.Count-1)
			//		gridPlayer.target = mesh.vertices[ roadPoints[endIndex] ];
			}


		}

		//////////////////////////////////////////////////

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

	void PlaceGridPlayersForInitialLoop()
	{
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		List<FindEdges.Edge> orderedEdges = gameObject.GetComponent<FindEdges>().orderedEdges;

		for (int i = 0; i < orderedEdges.Count; i++)
		{
			GameObject gpgo = new GameObject();
			gpgo.transform.parent = this.transform;
			gpgo.transform.position =mesh.vertices[ orderedEdges[i].vertexIndex[0] ]; //end of first edge
			gpgo.name = "PathFinder"+i;

			GridPlayer gridPlayer = gpgo.AddComponent<GridPlayer>();
			gridPlayer.target = mesh.vertices [ orderedEdges[i].vertexIndex[1] ];
		}

	}

	void PlaceGridPlayersForJoiningLoop()
	{
		//place grid players only from vertices which have not been used by other roads

		FindSharedPoints();
		FindOpenVertice();

	}

	void FindSharedPoints()
	{
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		List<FindEdges.Edge> orderedEdges = gameObject.GetComponent<FindEdges>().orderedEdges;
		List<int> sharedIndices = new List<int>();
		Transform cubePrefab = Resources.Load("Cube",typeof(Transform)) as Transform;
		
		//find shared vertices on this polygon with other countryside polygons//
		GameObject[] countrysides = GameObject.FindGameObjectsWithTag("Countryside");
		
			
		//for each cell
		for(int i = 0; i < countrysides.Length; i++)
		{
			//if(i==0)
			//{
				//dont check our own gameobject, only others
				if (countrysides[i] == this.gameObject)
					continue;
				//the list in the other game object we are checking against
				List<FindEdges.Edge> otherEdges = countrysides[i].GetComponent<FindEdges>().orderedEdges;
				Mesh otherMesh = countrysides[i].GetComponent<MeshFilter>().mesh;
				//for each edge in our cell
				for (int j = 1; j < mesh.vertexCount; j++) //miss the first vertice because it is in the center
				{
					//for each edge in other cell
					for (int k = 0; k < otherEdges.Count; k++)
					{
						
						if(mesh.vertices[j] == otherMesh.vertices[otherEdges[k].vertexIndex[0]])
						{
						//	Instantiate(cubePrefab,mesh.vertices[orderedEdges[j].vertexIndex[0]],Quaternion.identity);// as Transform;
							//Add to list of shared cells (vector3s)
						//	sharedPoints.Add(mesh.vertices[orderedEdges[j].vertexIndex[0]]);
							sharedIndices.Add(j);
						}
					}
				}
			//}
		}
	//	Debug.Log(sharedPoints.Count);
		//remove any doubles from sharedPoints

		 sharedPointsDistinct = sharedPoints.Distinct().ToList<Vector3>();

		sharedIndices.Sort();

		foreach (Vector3 v3 in sharedPointsDistinct)
		{
		//	Object cube = Instantiate(cubePrefab,v3,Quaternion.identity);
		//	cube.name ="shared v3";
		}

		foreach (int i in sharedIndices)
		{
		//	Transform cube = Instantiate(cubePrefab,mesh.vertices[i],Quaternion.identity) as Transform;
		//	cube.name = "shared Ind" + i;
		//	cube.parent  =this.transform;			
		}

		int lastItem = sharedIndices[sharedIndices.Count-1];
		List<int> roadPoints = new List<int>();
		//the starting points to feed to the pathfinder placement function
		int firstPoint = lastItem;
//		roadPoints.Add(firstPoint);
		int secondPoint = (lastItem) +1;
//		roadPoints.Add(secondPoint);

		Debug.Log("mesh vertex count" + mesh.vertexCount);
		for (int i = 0; i < mesh.vertexCount - 1; i++) //- 1 to ignore centre of mesh
		{

			int index = lastItem + i;
			if (index < mesh.vertexCount)
			{
				roadPoints.Add(index);
				Debug.Log(index);
			}
			else
			{
				index -= mesh.vertexCount;
				index += 1;
				Debug.Log(index);
				roadPoints.Add(index);
			}
		}

		foreach ( int i in roadPoints)
		{
			Transform cube3 = Instantiate(cubePrefab,mesh.vertices[i],Quaternion.identity) as Transform;
			cube3.name = "point" + i;
			cube3.localScale *=10;
			cube3.parent =this.transform;
		}


	//		Debug.Log(sharedPointsDistinct.Count);	
	}

	void FindOpenVertice()
	{
		Transform cubePrefab = Resources.Load("Cube",typeof(Transform)) as Transform;
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		List<FindEdges.Edge> orderedEdges = gameObject.GetComponent<FindEdges>().orderedEdges;
	
		foreach(Vector3 v3 in sharedPointsDistinct)
		{

		}
		
	}
}
