using UnityEngine;
using System.Collections;


//This script monitors the list created by the pathfinder. Once the list is filled it starts gridPlayer

public class BuildARoad : MonoBehaviour {
	public int amountOfTowns = 6;
	public Pathfinder pathFinder;
	public GridPlayer gridPlayer;
	public CurveInjector curveInjector;
	public SplineMesh splineMesh;
	public Verge verge;
	public PlaceAlongVerge placeAlongVerge;
	public TerrainPlanes terrainPlanes;
	public Transform connectingARoad;


	public Vector3 target;

	public bool instantiateARoad;

	// Use this for initialization


	void Start () {
		target += transform.position;


		//gridPlayer.transform.position = 
	}
	
	// Update is called once per frame
	void Update () {

	
	
		if (pathFinder.enabled)
			gridPlayer.enabled = true;
	

		if (gridPlayer.Path.Count != 0)
			curveInjector.enabled = true;
		    
		if (verge.enabled)
		{
			//terrainPlanes.enabled = true; //maybe converting to voronoi mesh

		}

		if( verge.finished)
		{
			instantiateARoad = true;

			verge.finished = false;
		}
		if (splineMesh.enabled)
		{
			verge.enabled = true;
			placeAlongVerge.enabled = true;
			//splineMesh.enabled = false;

		}

		if (terrainPlanes.scriptFinished)
		{
			//verge.gameObject.SetActive(false);
			terrainPlanes.scriptFinished = false;
			instantiateARoad = true;
		}

		if (instantiateARoad)
		{
			StartCoroutine("PlaceTownX");

			//Transform item = Instantiate(connectingARoad)as Transform;
			//item.position =new  Vector3(900f,0f,500f);

			//Transform item2 = Instantiate(connectingARoad)as Transform;
			//item2.position = target;

			instantiateARoad = false;
			//this.enabled = false;
		}




	}	

	IEnumerator PlaceTownX()
	{

			target = new Vector3(500f,0f,100f);
			Transform item = Instantiate(connectingARoad)as Transform;
			item.position = target;
			
		yield return new WaitForSeconds(0.5f);
			StartCoroutine("PlaceTownY");
	
	}

	IEnumerator PlaceTownY()
	{
		
		target = new Vector3(500f,0f,900f);
		Transform item = Instantiate(connectingARoad)as Transform;
		item.position = target;
		yield return new WaitForSeconds(0.5f);

		GameObject.FindWithTag("VoronoiMesh").GetComponent<AddToVoronoi>().enabled = true;
		
	}
}
