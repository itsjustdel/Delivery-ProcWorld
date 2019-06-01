using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlaceBRoads : MonoBehaviour {

	private List<FindFurthestCorner.StartAndEndOfBRoads> bRoadInfo;
	private List<Transform> roadList = new List<Transform>();
	// Use this for initialization
	void Start () {
		bRoadInfo = transform.GetComponent<FindFurthestCorner>().bRoadInfo;

		PlaceRoads();
	}
	void PlaceRoads()
	{
		FindFurthestCorner ffc = gameObject.GetComponent<FindFurthestCorner>();

		Vector3 junction1 = ffc.p1; 
		Vector3 junction2 = ffc.p2;

		Transform bRoadPrefab = Resources.Load("Prefabs/Setup/BRoad", typeof(Transform)) as Transform;
		for (int i = 0; i < bRoadInfo.Count; i++)
		{

			//if B Road isn't too close to Junction, instantiate

			float distance = Vector3.Distance(bRoadInfo[i].end, junction1);
			float distance2 = Vector3.Distance(bRoadInfo[i].end, junction2);


			if (distance > 100 && distance2 > 100)
			{
				Transform bRoad = Instantiate(bRoadPrefab,bRoadInfo[i].end,Quaternion.identity) as Transform;
				bRoad.GetComponent<BRoadPlotter>().target = bRoadInfo[i].start;
				bRoad.parent = this.transform;
				bRoad.tag = "Road";
				roadList.Add(bRoad);
				//add prefab to a list so we can access them at all once
			}
			else if (distance < 100 || distance2 < 100)
			{
				Debug.Log(distance + distance2 + "did not place B Road, too close to junction"); 
			}
		}

	StartCoroutine("ActivatePathFinders");
	}

	IEnumerator ActivatePathFinders()
	{
		for (int i = 0; i < roadList.Count; i++)
		{


			roadList[i].GetComponent<Pathfinder>().enabled =true;
			yield return new WaitForEndOfFrame();
			roadList[i].GetComponent<BRoadPlotter>().enabled =true;
			yield return new WaitForEndOfFrame();
			roadList[i].GetComponent<BRoadMesher>().enabled =true;
			yield return new WaitForEndOfFrame();
			roadList[i].GetComponent<Verge>().enabled = true;
			yield return new WaitForEndOfFrame();
			roadList[i].GetComponent<TJunctionBuildings>().enabled = true;
		}

		//Now the road network is completed, we can lay the voronoi meshover the top to create the visual landscape
		yield return new WaitForEndOfFrame();
		GameObject vM =  GameObject.Find("VoronoiMesh");
		vM.transform.position = this.transform.position;
		vM.GetComponent<AddToVoronoi>().enabled = true;
	}
}
