using UnityEngine;
using System.Collections;


/// <summary>
/// Build B road. This script is in charge of executing the scripts at the right time to create
/// and mesh a BRoad from a point to the "A Road"
/// </summary>

public class BuildBRoad : MonoBehaviour {
	public bool isHouse;

	public HousePlacement housePlacement;
	public Pathfinder pathfinder;
	public BRoadPlotter bRoadPlotter;
	public BRoadMesher bRoadMesher;
	public Verge verge;
	public TerrainPlanes terrainPlanes;
	public TJunctionBuildings tJunctionBuildings;
	public BuildingsAlongCurve buildingsAlongCurve;
	public PlaceAlongVerge placeAlongVerge;
	public MeshGenerator meshGenerator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!isHouse)
		{


			if (terrainPlanes.scriptFinished)
			{

			//get verges and disable them
		//	transform.GetChild(2).gameObject.SetActive(false);
		//	transform.GetChild(3).gameObject.SetActive(false);
			}
		}



		if(!isHouse)
			{	
			if (terrainPlanes.enabled)
			{
		//		placeAlongVerge.enabled = true;

				//get bush and tree placer object and activate
			//	GameObject.FindWithTag("ARoadBuilder").transform.GetChild(4).gameObject.SetActive(true);
				//Debug.Log("activated");
			}
		}

		if(!isHouse)
		{
			if (verge.enabled)
			{
			//	terrainPlanes.enabled = true;
			}
		}
		if(!isHouse)
		{
			if (bRoadMesher.enabled && !isHouse)
			{
		//		verge.enabled = true;
			}
		}

		if (bRoadPlotter.enabled)
		{
			bRoadMesher.enabled=true;
		}


		if (pathfinder.enabled)
		{
			bRoadPlotter.enabled = true;
		}

		if (housePlacement.enabled)
		{
		
	//		pathfinder.enabled = true;
		}



		if (!isHouse)
		{

			if (bRoadMesher.enabled)
			{
			//	tJunctionBuildings.enabled = true;
				//meshGenerator.enabled = true;
			}

			if ( tJunctionBuildings.enabled)
			{
			//	buildingsAlongCurve.enabled = true;
			}
		}


	}
}
