using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

public class PlaceWaypointPlayerSimple : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlaceWaypointPlayers();
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
			for(int i =0; i < mesh.vertexCount; i++)
			{
				//skip first mesh vertices- this is always the centre point of the mesh for the cell. 
				if(i==0)
					continue;

				//Place pathfinders round the edges of the mesh
				GameObject waypointPlayerGo = new GameObject();
				//attach to this cell
				waypointPlayerGo.transform.parent =this.transform;
				//the transform position is where the pathfinder starts from
				waypointPlayerGo.transform.position = mesh.vertices[i];
				waypointPlayerGo.name = "Pathfinder"+i;

				WaypointPlayer waypointPlayer = waypointPlayerGo.AddComponent<WaypointPlayer>();
				waypointPlayer.forHex = true;
				waypointPlayer.PathType = PathfinderType.WaypointBased;

				//make sure the index does not go out of range if it meets thened of the mesh. Loop it back to the start
				int index = i+1;
				if(index < mesh.vertexCount)
				{
					waypointPlayer.target = mesh.vertices[index];
				}
				else
				{
					index -= mesh.vertexCount;
					//jump over central vertice ( i = 0 )
					index += 1;
					waypointPlayer.target = mesh.vertices[index];
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

			//last item is not the start of the road

			//iterate through mesh and add to roadPoints list if a valid point
			List<int> roadPoints = new List<int>();
			List<int> roadPointsTargets = new List<int>();

			//add the first point on the road
			//roadPoints.Add(lastItem);									//taken out for simple

			int endIndex = 100;

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
//				if(!sharedIndices.Contains(index))
//				{
					roadPoints.Add(index);

					//work out what the current end ind is
					//this will get rewritten until the last time the loop runs
					endIndex = index + 1;
					if (endIndex < mesh.vertexCount)											//the if statement here is taken out for simple
					{
						//leave
					}
					else
					{
						endIndex -= mesh.vertexCount;
						endIndex += 1;
					}
				}
//			}
			//now add the target for the road
//			roadPoints.Add(endIndex);															//taken out for simple

			//Create objects for waypointPlayers and place them at the positions in the roadPoints list
			//don't place for the last point, this is the last rp's target
			for (int i = 0; i < roadPoints.Count-1; i++) // 
			{
				GameObject wppgo = new GameObject();
				wppgo.transform.parent = this.transform;
				wppgo.transform.position = mesh.vertices[ roadPoints[i] ];
				wppgo.name = "PathFinder"+i;

				WaypointPlayer wppPlayer = wppgo.AddComponent<WaypointPlayer>();
				wppPlayer.target = mesh.vertices[ roadPoints[i+1] ]; //TODO out of range error here ?
				wppPlayer.forHex = true;
				wppPlayer.PathType = PathfinderType.WaypointBased;
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
}
