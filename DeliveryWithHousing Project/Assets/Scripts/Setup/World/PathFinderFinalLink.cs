using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinderFinalLink : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		List<Vector3> ringRoadList = gameObject.GetComponent<RingRoadList>().ringRoadPoints;

		Debug.Log("old list"+ringRoadList.Count);

		GameObject gpgoSecondLast = new GameObject();
		gpgoSecondLast.transform.parent = this.transform;
		gpgoSecondLast.transform.position = ringRoadList[ringRoadList.Count-1];//last entry in list
		gpgoSecondLast.name = "PathFinderLink";
		//the target is the start of the first edge
		GridPlayer gridPlayerSecondLast = gpgoSecondLast.AddComponent<GridPlayer>();
		gridPlayerSecondLast.target = ringRoadList[0];//first entry (ties list together)//aft



		gameObject.GetComponent<RingRoadList>().ringRoadPoints.Add(gameObject.GetComponent<RingRoadList>().ringRoadPoints[0]);
//		StartCoroutine("AddLink",gpgoSecondLast);
		StartCoroutine("AddMesh");
	}

	IEnumerator AddLink(GameObject go)
	{
		yield return new WaitForSeconds(1);

		//go fetch this last link's v3 data and giev to ring road list
		//this links the last and first together
		List<Vector3> path = go.GetComponent<GridPlayer>().Path;

		foreach ( Vector3 v3 in path)
		{
			//ringRoadList.Add(v3);
			gameObject.GetComponent<RingRoadList>().ringRoadPoints.Add(v3);
		}

	//	gameObject.GetComponent<RingRoadList>().ringRoadPoints.Add(gameObject.GetComponent<RingRoadList>().ringRoadPoints[0]);
		gameObject.GetComponent<RingRoadList>().ringRoadPoints.RemoveAt(0);
		StartCoroutine("AddMesh");

	}

	IEnumerator AddMesh()
	{
		yield return new WaitForFixedUpdate();
		
		gameObject.AddComponent<RingRoadMesh>();
	}

}
