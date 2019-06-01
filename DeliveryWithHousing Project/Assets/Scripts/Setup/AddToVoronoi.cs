using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class AddToVoronoi : MonoBehaviour {

	//How many mesh points to skip when adding to mesh Generator

	public int pointDensityRatio = 2;
	public int roadSpread = 5;
	MeshGenerator mG ;
	GameObject[] roadMeshArray;
	GameObject[] buildingsArray;
	public List<MeshAndVertice> meshAndVerticeList = new List<MeshAndVertice>();
	public List<Vector3> pointsForVoronoi = new List<Vector3>();
	//Gathers all the points to put in to voronoi mesh generator
	public void Start () {

		mG = gameObject.GetComponent<MeshGenerator>();

        

	//	roadMeshArray = GameObject.FindGameObjectsWithTag("Road");
	//	buildingsArray = GameObject.FindGameObjectsWithTag("Building");
	


	//	StartCoroutine("AddBuildingPoints");
        //daisy chaining
	StartCoroutine("AddBorderToVoronoi");/////////not in chain
	//	StartCoroutine("AddCellPoints");
	//	StartCoroutine("AddRoadPoints");//////////not in chain
	//	StartCoroutine("AddPointsForVoronoiList");
	//	StartCoroutine("EnableMG");

	//	
	}

	IEnumerator EnableMG()
	{
        // yield return new WaitForSeconds(10);

        gameObject.GetComponent<MeshGenerator>().Start();

        yield break;
	}

	IEnumerator AddPointsForVoronoiList()
	{
		foreach (Vector3 v3 in pointsForVoronoi)
		{
			mG.yardPoints.Add(new Vector3(v3.x,0f,v3.z));
		}
        mG.yardPoints =  mG.yardPoints.Distinct().ToList();

        StartCoroutine("EnableMG");
        yield break;
	}
	IEnumerator AddRoadPoints()
	{
		Transform house = Resources.Load("Cube", typeof (Transform)) as Transform;
		//insert points to the mesh generator's list(array)
		//avoid points which have been inserted in to the mesh and vertice list
		//this list is populated by the place building script(s) 
		Debug.Log(meshAndVerticeList.Count);
		//run through  Road GameObject array
		for (int g = 0; g < meshAndVerticeList.Count;g++)
		{
			Mesh mesh = meshAndVerticeList[g].mesh;
			GameObject cell = meshAndVerticeList[g].cell;
			//grab meshFilter component from the GameObject in question


				for (int i = 0; i < mesh.vertexCount; i+=pointDensityRatio)
				{

/*					bool correctMesh = false;

					//if the mesh equals the mesh in the meshAndverticeList
					if( cell == meshAndVerticeList[j].cell )
					{

						Debug.Log("correct mesh");
						correctMesh = true;
					}
*/
					//if i is not near any indice in the list
					//checks to see if i is not in range
					bool inRange = false;

					int range = 50;
					int lowerRange = meshAndVerticeList[g].ind - range;
					int higherRange = meshAndVerticeList[g].ind + range;
					int target = i - (range/2);								//TODO sort index to make loop?
					if( target >=lowerRange && i <= higherRange )
					{
						inRange = true;
						Debug.Log("in range");
					}


					if (inRange)// && correctMesh)
					{
							//Add to list
							//find center of road
							Vector3 pos = Vector3.Lerp( mesh.vertices[i], mesh.vertices[i+1],0.5f);
							pos += cell.transform.position;
							pos.y = 0;
							mG.yardPoints.Add(pos);
				
								Debug.Log("inside");
							//add points off to the side of the road
							Vector3 dirL =  mesh.vertices[i] - mesh.vertices[i+1];
							dirL *= roadSpread;
							Vector3 posL = dirL + pos; 
							mG.yardPoints.Add(posL);

							Vector3 dirR =  mesh.vertices[i+1] - mesh.vertices[i];
							dirR *=roadSpread;
							Vector3 posR = dirR + pos; 
							mG.yardPoints.Add(posR);//zerooooooo


					//	Transform item = Instantiate(house) as Transform;
					//	item.name = "Test Cube";
					//	item.position = pos;
						//item.parent = this.transform;
					}	

						
			}
			yield return new WaitForFixedUpdate();

		}

        yield break;
	}


	IEnumerator AddCellPoints()
	{
		List<Vector3> cellList = new List<Vector3>();

		GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
		foreach (GameObject go in cells)
		{
			Mesh mesh = go.GetComponent<MeshFilter>().mesh;
			foreach (Vector3 v3 in mesh.vertices)
			cellList.Add(v3 + go.transform.position);		
		}
	


		cells = GameObject.FindGameObjectsWithTag("Countryside");
		foreach (GameObject go in cells)
		{
			Mesh mesh = go.GetComponent<MeshFilter>().mesh;
			foreach (Vector3 v3 in mesh.vertices)
				cellList.Add(v3 + go.transform.position);		
		}

		cellList = cellList.Distinct().ToList();
		
		foreach (Vector3 v3 in cellList)
			mG.yardPoints.Add(v3);

        StartCoroutine("AddBorderToVoronoi");
        //StartCoroutine("EnableMG");
        //StartCoroutine("AddPointsForVoronoiList");
        yield break;
	}

	IEnumerator AddBuildingPoints()
	{
		//now add building points
		foreach (GameObject go in buildingsArray)
		{
			Vector3 pos = go.transform.position;
			pos.y = 0f;
			mG.yardPoints.Add(pos);
		}

        StartCoroutine("AddCellPoints");
        //StartCoroutine("AddBorderToVoronoi");   //only one of these two routines should be chosen
        yield break;
	}

	IEnumerator AddBorderToVoronoi()
	{
		//pass the centre of the tjunction to the circle function
		Vector2 center = new Vector2(transform.position.x,transform.position.z);
		
		for (int i = 1500; i < 2000; i+=50)
		{
		DrawCirclePoints(10,i,center);
		
		}
         StartCoroutine("AddPointsForVoronoiList");
        //StartCoroutine("EnableMG");
        yield break;
	}
	
	void DrawCirclePoints(int points, double radius, Vector2 center)
	{
		
		MeshGenerator mG = transform.GetComponent<MeshGenerator>();
		
		float slice = 2 * Mathf.PI / points;
		for (int i = 0; i < points; i++)
		{
			float angle = slice * i;
			int newX = (int)(center.x + radius * Mathf.Cos(angle));
			int newY = (int)(center.y + radius * Mathf.Sin(angle));
			Vector2 p = new Vector2(newX, newY);
			
			Vector3 p3 = new Vector3(p.x,0f,p.y);
			
			mG.yardPoints.Add(p3);
		}
	}

	public class MeshAndVertice
	{
		public GameObject cell;
		public Mesh mesh;
		public int ind;
	}
}
