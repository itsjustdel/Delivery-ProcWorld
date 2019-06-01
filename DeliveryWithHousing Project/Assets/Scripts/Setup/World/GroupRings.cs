using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class GroupRings : MonoBehaviour {

	//searches for cell and groups them by tag type
	//This script also instantiates and fires elements required to build roads on the cell

	public List<GameObject> countrysideGameObjectsList = new List<GameObject>();

	void Start () {
	
		GroupCells();
		CreateParentObject();
	}


	void GroupCells()
	{
		//grabs all gameobjects with countryside tag and enters them in to a list
		GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Countryside");
		//use .Linq to convert in to list
		countrysideGameObjectsList = tempArray.ToList();
	}

	void CreateParentObject()
	{
		GameObject combinedCells = new GameObject();
		combinedCells.transform.parent = this.transform;
		combinedCells.name = "Combined Cells";

		//move cells in the list to be a child of the new gamobject so we can run combine meshes script
		//combine meshes script needs all target meshes under one parent

		for (int i=0; i < countrysideGameObjectsList.Count;i++)
		{
			countrysideGameObjectsList[i].transform.parent = combinedCells.transform;
			//turn off the collider on individual cells, collider will be added to the combined mesh which
			//all the cells in this list will make(possibly just remove this collider)
			countrysideGameObjectsList[i].GetComponent<MeshCollider>().enabled = false;
		}

		//now the cells are childs of the new gameObject, add the combine meshes script
		//we are need these meshes to be one mesh so the findEdges script can locate the points for
		//the ring road 

		combinedCells.AddComponent<CombineChildren>();

	

		//need to wait until combine children does its job, so call this to execture next frame
		StartCoroutine("AddAutoWeld",combinedCells);
	}
	/// <summary>
	/// Welds the vertices together so mesh is as one to find the outside edges
	/// </summary>
	/// <returns>The auto weld.</returns>
	/// <param name="combinedCells">Combined cells.</param>
	IEnumerator AddAutoWeld(GameObject combinedCells)
	{
		yield return new WaitForEndOfFrame();

		Transform child = combinedCells.transform.Find("Combined mesh");
		AutoWeld autoWeld = child.gameObject.AddComponent<AutoWeld>();

	//	StartCoroutine("AddMeshCollider",combinedCells);

		StartCoroutine("DisableCellColliders");
		StartCoroutine("AddFindEdges",combinedCells);		//
	}

	IEnumerator AddMeshCollider(GameObject combinedCells)
	{
		//adds to a mesh collider to newly glued together mesh made from individual voronoi cells
		yield return new WaitForFixedUpdate();
		Transform child = combinedCells.transform.Find("Combined mesh");
		child.gameObject.AddComponent<MeshCollider>();



	}

	/// <summary>
	/// Disables the cell colliders. Runs through GameObjects tagged with "countryside" and disables collider
	/// </summary>
	IEnumerator DisableCellColliders()
	{

		MeshCollider[] colliders = transform.GetComponentsInChildren<MeshCollider>();
		foreach (MeshCollider c in colliders)
		{
			c.enabled = false;
		}

		yield break;
	}



	IEnumerator AddFindEdges(GameObject combinedCells)
	{
		yield return new WaitForEndOfFrame();

		Transform child = combinedCells.transform.Find("Combined mesh");
		child.gameObject.AddComponent<FindEdges>();
        Debug.Log("adding edges");
		StartCoroutine("AddPlaceGridPlayer",combinedCells);

	}

	IEnumerator AddPlaceGridPlayer(GameObject combinedCells)
	{
		yield return new WaitForEndOfFrame();
		Transform child = combinedCells.transform.Find("Combined mesh");
		child.gameObject.AddComponent<PlaceGridPlayer>();

		StartCoroutine("AddRoadPrefab",combinedCells);

	}


	IEnumerator AddRoadPrefab(GameObject combinedCells)
	{
		yield return new WaitForSeconds(1);


		Transform roadPrefab = Resources.Load("Prefabs/Setup/Aroad2",typeof (Transform)) as Transform;
		Transform child = combinedCells.transform.Find("Combined mesh");

		Transform roadNetwork = Instantiate(roadPrefab) as Transform;
		roadNetwork.transform.parent = child.transform;
		roadNetwork.position = transform.position;

		//Wait for unity to instantiate and asign parents
		yield return new WaitForEndOfFrame();
		//now the pathfinder script has everythin in the right order so it can execute
		roadNetwork.GetComponent<Pathfinder>().enabled = true;

		yield return new WaitForEndOfFrame();
		//now plot the points the road mesh uses
		roadNetwork.GetComponent<BRoadPlotter>().enabled = true;

		yield return new WaitForEndOfFrame();
		//now build the mesh
		roadNetwork.GetComponent<BRoadMesher>().enabled = true;

		yield return new WaitForEndOfFrame();
		//activate the verge on b road
		roadNetwork.GetComponent<Verge>().enabled = true;

	}


}
