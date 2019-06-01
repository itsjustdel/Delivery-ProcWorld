using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HousePlacer : MonoBehaviour {

	public int maxHouses = 10;
	public float mapSize = 1000f;
//	private bool placeAnother =false;
	public Transform housePrefab;
	public List<Transform> houseList = new List<Transform>();
	// Use this for initialization
	void Start () {

		StartCoroutine("PlaceHouses");

	}
	
	// Update is called once per frame
	void Update () {

	
	
	}

	IEnumerator PlaceHouses()
	{
		int houseAmt = Random.Range(2,maxHouses);
		
		
		//Add random postions in to house positions list
		for (int i = 0; i < houseAmt; i++)
		{
			Vector3 position = new Vector3(Random.Range(0f,mapSize),0f,Random.Range(0f,mapSize));
			
			position.x = Mathf.Clamp(position.x,250f,750f);
			position.z = Mathf.Clamp(position.z,250f,750f);

			Transform house = Instantiate(housePrefab) as Transform;
			houseList.Add(house);

			house.position = position;
		}
		StartCoroutine("EnableHouses");
		yield break;
	}

	IEnumerator EnableHouses()
	{
		for (int i = 0; i < houseList.Count; i++)
		{

			Transform house = houseList[i];

			house.GetComponent<HousePlacement>().enabled = true;

		
				yield return new WaitForSeconds(1);
		

		}

	}		
	

}
