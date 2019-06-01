using UnityEngine;
using System.Collections;

public class BuildingsAlongCurve : MonoBehaviour {

	public int houseAmt = 10;
	public Transform house;
	public BRoadMesher bRoadMesher;
	private Mesh aRoadMesh;


	// Use this for initialization
	void Start () {	


		StartCoroutine("PlaceHouses");


	}

	/*
	 * 
	 * ///TODO find broad mesher in hierarchy
	IEnumerator PlaceHouses ()
	{

		for (int i = 0; i < houseAmt; i++)
		{
			aRoadMesh = bRoadMesher.targetRoadMesh;
			
			int random = Random.Range(20,aRoadMesh.vertexCount-20);
			
			Vector3 middle = Vector3.Lerp(aRoadMesh.vertices[random],aRoadMesh.vertices[random+1],0.5f);
			
			Vector3 direction = aRoadMesh.vertices[random] - aRoadMesh.vertices[random +1];
			direction.Normalize();
			
			float randomNumber2 = Random.Range(10f,50f);
			direction*=randomNumber2;
			
			Vector3 position = middle + direction;
			
			Transform item = Instantiate(house) as Transform;
			item.position = position;
			item.name = "Test House";
			yield return new WaitForSeconds(1);
		}
	
	}
	*/
}
