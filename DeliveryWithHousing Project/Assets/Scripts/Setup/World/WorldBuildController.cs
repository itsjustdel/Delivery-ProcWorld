using UnityEngine;
using System.Collections;

public class WorldBuildController : MonoBehaviour {

	public GameObject terrainBase;
	public GameObject hexGrid;
    public GameObject voronoiMap;
	public GameObject worldPlan;
	// Use this for initialization
	void Start () {
	
		StartCoroutine("BuildOrder2");
	}	

	IEnumerator BuildOrder()
	{
		terrainBase.SetActive(true);
        
		yield return new WaitForEndOfFrame();

		hexGrid.GetComponent<HexGridRectangle>().enabled = true;

		yield return new WaitForEndOfFrame();

		hexGrid.GetComponent<AutoWeld>().enabled = true;

		yield return new WaitForEndOfFrame();

		hexGrid.GetComponent<DropWaypoints>().enabled = true;

		yield return new WaitForEndOfFrame();

		hexGrid.GetComponent<WaypointPathfinder>().enabled = true;
		hexGrid.GetComponent<WaypointPathfinder>().MakeNewMap();
		yield return new WaitForEndOfFrame();

        worldPlan.GetComponent<MeshGenerator>().Start();

		yield return new WaitForEndOfFrame();

		//worldPlan.GetComponent<RingRaycast>().enabled = true; //enabling in mesh gen script

		yield return new WaitForEndOfFrame();

	//	worldPlan.GetComponent<CellManager>().enabled = true;
	}

    IEnumerator BuildOrder2()
    {
        worldPlan.SetActive(true);

        yield break;
    }
}
