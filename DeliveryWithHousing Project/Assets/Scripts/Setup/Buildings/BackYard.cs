using UnityEngine;
using System.Collections;


public class BackYard : MonoBehaviour {

	//Plot points form the side of the cube back

	private int yardDepth = 20;
	public Transform cubePrefab;
	public Transform bushPrefab;
	public PolygonTester pT;
	public MeshGenerator mG;
	// Use this for initialization
	void Start () {
		StartCoroutine("TJunctionVoronoi");

	}

	IEnumerator TJunctionVoronoi()
	{
		yield return new WaitForSeconds(1);
		//add points of back yards to tJunction's voronoi mesh generator
		HouseVariables hV = transform.Find("Brick House").GetComponent<HouseVariables>();
		mG = transform.parent.GetComponent<MeshGenerator>();
		Quaternion lookRot = hV.body.lookRot;

		Vector3 middle = transform.position;

		Vector3 rightSide = transform.position;// + 
		rightSide += (lookRot*Vector3.right)*5;
		Vector3 leftSide = transform.position;// + ((lookRot*Vector3.back)*yardDepth);
		leftSide += (lookRot*Vector3.left)*5;

		middle.y = 0;
		rightSide.y = 0;
		leftSide.y =0;

		mG.yardPoints.Add(middle);
	//	mG.yardPoints.Add(rightSide);
	//	mG.yardPoints.Add(leftSide);

		middle += (lookRot*Vector3.back)*yardDepth;
		rightSide += (lookRot*Vector3.back)*yardDepth;
		leftSide += (lookRot*Vector3.back)*yardDepth;

		middle.y = 0;
		rightSide.y = 0;
		leftSide.y =0;

		mG.yardPoints.Add(middle);
	//	mG.yardPoints.Add(rightSide);
	//	mG.yardPoints.Add(leftSide);

		//add circle border to "close" edges so vornoi mesh will work
	

		yield break;


	}

	IEnumerator Bushes()
	{
		HouseVariables hV = transform.Find("Brick House").GetComponent<HouseVariables>();
		Quaternion lookRot = hV.body.lookRot;
		for (int i = 0; i < yardDepth; i++)
		{

			Vector3 rightSide = transform.position + ((lookRot*Vector3.back)*i);
			rightSide += (lookRot*Vector3.right)*5;

			Vector3 leftSide = transform.position + ((lookRot*Vector3.back)*i);
			leftSide += (lookRot*Vector3.left)*5;

			//Place bushes at target points
			Transform bushR =  Instantiate(bushPrefab,rightSide,Quaternion.identity) as Transform;
			bushR.parent = this.transform;
			bushR.GetComponent<PlaceFlora>().enabled = true;
			Transform bushL =  Instantiate(bushPrefab,leftSide,Quaternion.identity) as Transform;
			bushL.parent = this.transform;
			bushL.GetComponent<PlaceFlora>().enabled = true;

			//add points to voronoi mesh generator
			rightSide.y = 0;//transform.position.y;
			leftSide.y= 0;//transform.position.y;

			mG.yardPoints.Add(rightSide);
			mG.yardPoints.Add(leftSide);

			//add extras for voronoi edges
			rightSide += (lookRot*Vector3.right)*5;
			rightSide.y = 0;
			leftSide += (lookRot*Vector3.left)*5;
			leftSide.y= 0;
			mG.yardPoints.Add(rightSide);
			mG.yardPoints.Add(leftSide);

		}
		//pT.enabled = true;

		int yardWidth = 10;
		for (int i = 0 ; i < yardWidth; i++)
		{

			Vector3 pos = transform.position + (lookRot*Vector3.back*yardDepth);
			pos -= (lookRot*Vector3.right) * yardWidth*0.5f;
			pos -= (lookRot*Vector3.left)*i;

			Transform bush =  Instantiate(bushPrefab,pos,Quaternion.identity) as Transform;
			bush.parent = this.transform;
			bush.GetComponent<PlaceFlora>().enabled = true;

			Vector3 posAtHouse = transform.position;
			posAtHouse -= (lookRot*Vector3.right) * yardWidth*0.5f;
			posAtHouse -= (lookRot*Vector3.left)*i;

			pos.y = 0;//transform.position.y;
			posAtHouse.y = 0;//transform.position.y;

		//	mG.yardPoints.Add(pos);
		//	mG.yardPoints.Add(posAtHouse);


			pos += lookRot*Vector3.back*yardDepth;
			pos.y = 0;

			posAtHouse -= lookRot*Vector3.back*yardDepth;
			pos.y = 0;

		//	mG.yardPoints.Add(pos);
		//	mG.yardPoints.Add(posAtHouse);
		}



		yield break;
	}

}
