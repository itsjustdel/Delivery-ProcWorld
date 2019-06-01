using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour {

	public Pathfinder pathfinder;
	public MeshCollider terrainCollider;
	public CurveInjector curveInjector;
	public GridPlayer roadMapper;
	public SplineMesh splineMesh;
	public CreateKerb createKerb;
	public Verge verge;
	public TerrainPlanes terrainPlanes;
	public PlaceAlongVerge placeAlongVerge;

	public GameObject van;

	public bool movedVan = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		//If the road Curve is not empty//roadmnapper fills this array as it goes along
		if (curveInjector.list.Count!=0)
		{

			//curveInjector.enabled=true; //this starts splineMesh


		}

//		if(terrainPlanes.terrianPlanesFinished == true)
//		{
//			placeAlongCurve.enabled = true;
//		}

		if (verge.enabled)
		{
			terrainPlanes.enabled = true;
		}

		if (splineMesh.enabled)
		{
			createKerb.enabled=true;

			verge.enabled =true;

		}




		
		if (!pathfinder.enabled)
		{
			if(terrainCollider.sharedMesh != null)
			{
			
				//pathfinder.enabled = true;

			}
		}

		if(pathfinder.enabled)
		{
			roadMapper.enabled = true;

		}




	}
}
