using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class BRoadMesher : ProcBase {
	private List<Vector3> vertices = new List<Vector3>();

	//public List<Vector3> path = new List<Vector3>();
//	public List<Vector3> pathShortened = new List<Vector3>();
	public List<Vector3> pathFull = new List<Vector3>();
	private List<int> indices = new List<int>();
	public float frequency = 10f;
	public float junctionFrequency = 10f;
	public float bRoadWidth = 3f;
	public float raiseAmt = 1f;
	public BezierSpline bezier;
	public Vector3 p1,p2;
	public Transform cube;
	public BRoadPlotter bR;
	public List<Vector3> targetRoadPoints;
	public int closestInd;
	public int startInd;
	public int junctionWidth = 10;
	public LayerMask myLayerMask;
	//other scripts

	public Verge verge;
	public BezierSpline splineL;
	public BezierSpline splineR;

	//Declare mesh vars so they can be used by each function. Could make the fucntions pass these if you want to streamline
//	public Mesh targetRoadMesh;
	private Mesh junctionMesh;
	private Mesh bRoadMesh;

	private MeshCollider meshCollider;
	private	MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	public bool mainRoad;

	//private MeshCollider meshCollider2;
//	private	MeshFilter meshFilter2;
//	private MeshRenderer meshRenderer2;



	// Use this for initialization
	void Start () {

//		pathFull.RemoveAt(0);
//		pathFull.RemoveAt(1);
		//if(mainRoad)
		//	targetRoadMesh = transform.parent.FindChild("RingRoad").GetComponent<MeshFilter>().sharedMesh;
		
		//if(!mainRoad)
		//	targetRoadMesh = transform.parent.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;


		AddJunctionToList();
	}
	/// <summary>
	/// Scans the A road mesh and returns the closest point from the end of the B road
	/// </summary>
	void FindClosestPointInMeshByDistance(int targetIndex)
	{
		Mesh targetRoadMesh;
	//	if(mainRoad)
			targetRoadMesh = transform.parent.Find("RingRoad").GetComponent<MeshFilter>().sharedMesh;

	//	if(!mainRoad)
	//		targetRoadMesh = transform.parent.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;
	

		////////////Add the junction to the start of the array for bezier spline
		
		float minDistanceSqr = Mathf.Infinity;
		//could search the A Road Plotter's array instead
		for (int i = 0; i < targetRoadMesh.vertices.Length; i++)
		{
			//Vector3 midPoint = Vector3.Lerp (vertices[0],vertices[1],0.5f);
//			Debug.Log(pathFull.Count);
//			Debug.Log(targetIndex);
			Vector3 midPoint = pathFull[targetIndex];
			
			Vector3 diff = midPoint-targetRoadMesh.vertices[i];
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
				//closestPoint = targetRoadMesh.vertices[i];
				
				closestInd = i ;
				
			}			
		}

		//gameObject.GetComponent<TJunctionBuildings>().closestInd = closestInd;
		//script finds this iteslf now


	//	Transform item = Instantiate(cube) as Transform;
	//	item.name = "closestInd";
	//	item.position = targetRoadMesh.vertices[closestInd];
	}

	public static int FindClosestPoint(int targetIndex, Mesh mesh, List<Vector3> list)
	{
	
		int closest = 0;
		float minDistanceSqr = Mathf.Infinity;
		//could search the A Road Plotter's array instead
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
		
			Vector3 midPoint = list[targetIndex];
			
			Vector3 diff = midPoint-mesh.vertices[i];
			float distSqr = diff.sqrMagnitude;
			
			if (distSqr < minDistanceSqr)
			{
				minDistanceSqr = distSqr;
								
				closest = i ;
				
			}			
		}
	
			return closest;
	
	}
	/// <summary>
	/// Finds the closest point in mesh by ray cast.
	/// </summary>
	// 


	/// <summary>
	/// Adds the junction to list which creates the Road mesh.
	/// </summary>
	void AddJunctionToList(){
		bezier = gameObject.GetComponent<BezierSpline> ();
		frequency = bR.Path.Count*10;	
		//targetRoadMesh= GameObject.Find("ARoad").GetComponent<MeshFilter>().sharedMesh; //old

		Mesh targetRoadMesh;
		//if (mainRoad)
			targetRoadMesh = transform.parent.Find("RingRoad").GetComponent<MeshFilter>().sharedMesh;
		//if(!mainRoad)
		//	targetRoadMesh = targetRoadMesh = transform.parent.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;

		FindClosestPointInMeshByDistance(1);
		//use the static function to work out the mesh index for the start of the road

		List<Vector3> pathWithJunction = new List<Vector3>();
		//Add the point at which the Broad ends
		Vector3 middle = Vector3.Lerp(targetRoadMesh.vertices[closestInd+junctionWidth],
		                              targetRoadMesh.vertices[closestInd+junctionWidth+1],
		                              0.5f);

		//add startpoint
		pathWithJunction.Add(middle);
		//now add control point
		pathWithJunction.Add(targetRoadMesh.vertices[closestInd]);
		//now add the rest of the curve from the plotter script
		
		for (int i = 0; i< pathFull.Count ; i++)
		{
			pathWithJunction.Add(pathFull[i]);
		}

		//now add the "start" of the road on to the list

	 if (!mainRoad)
		{
			//if not the main road, switch target to Broad mesh. Main road connects ring road to ring road.
			//B road connects ring road to B Road
			targetRoadMesh = transform.parent.Find("Aroad2(Clone)").Find("BRoad").GetComponent<MeshFilter>().sharedMesh;
		}
		//use the static function to work out the mesh index for the start of the road (list inverted)
		startInd = FindClosestPoint(pathFull.Count-1,targetRoadMesh,pathFull);		
		
	
		//Add the point at which the Broad starts

		int ind = startInd + junctionWidth;
		int ind2 = startInd + junctionWidth+1;
		if ((startInd + junctionWidth) >= targetRoadMesh.vertexCount)
		{
			ind = targetRoadMesh.vertexCount-1;
			ind2 = targetRoadMesh.vertexCount-2;
			Debug.Log("altering targetroadMesh indice number");
		}
		Vector3 middleStart = Vector3.Lerp(targetRoadMesh.vertices[ind],
		                              targetRoadMesh.vertices[ind2],
		                              0.5f);
		

			pathWithJunction.Add(targetRoadMesh.vertices[startInd]);
			//finally add the "start" - the list is inverted for what we need it for
			pathWithJunction.Add(middleStart);
		

		// Sort out the control points for the bezier which has already been filled up from the road plotter script
		
		List<BezierControlPointMode> controlPointList = new List<BezierControlPointMode>();
		
		for (int i = 0; i<  pathWithJunction.Count; i++)
		{
			controlPointList.Add(BezierControlPointMode.Aligned);
		}
		
		bezier.points = pathWithJunction.ToArray();
		bezier.modes = controlPointList.ToArray();
		
		//StartCoroutine ("MeshRoad");
		meshItUpOneSegment();
	}

	void meshItUpOneSegment()
	{
		GameObject child = new GameObject ();
		child.name = "BRoad";
		child.transform.position = Vector3.zero;//change in curve Injector too?
		child.transform.parent = this.gameObject.transform;
		child.layer = 9;
		child.tag = "Road";
		
		//variables for verge
		splineL = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineLList = new List<Vector3>();
		List<BezierControlPointMode> splineLControlPointsList = new List<BezierControlPointMode>();
		
		splineR = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineRList = new List<Vector3>();
		List<BezierControlPointMode> splineRControlPointsList = new List<BezierControlPointMode>();

		BezierSpline splineLowDetail = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineLowDetailList = new List<Vector3>();
		List<BezierControlPointMode> splineLowDetailControlPointsList = new List<BezierControlPointMode>();
		
		List<Vector3> vertices = new List<Vector3>();		
		List<int> indices = new List<int>();
		
		float stepSize = frequency;		
		stepSize =1f/(frequency-1);
		
		for (float f = 0; f <= (frequency); f+=100) { //normall f++  
			
			Vector3 position = bezier.GetPoint((f) * stepSize );
			//Vector3 position = spline.GetPoint(f/frequency);
			Vector3 direction = bezier.GetDirection((f)*stepSize );
			//Debug.Log(direction);
			
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=bRoadWidth;
			rot2*=bRoadWidth;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;
			
			splineLowDetailList.Add(position - transform.position);
			splineLowDetailControlPointsList.Add(BezierControlPointMode.Aligned);

/*			vertices.Add(offset1 - transform.position);
			vertices.Add(offset2- transform.position);
			
			//while we are at it, add points to spline so that the verge can use them						
in			splineLList.Add(offset1 - transform.position);
normally	splineLControlPointsList.Add(BezierControlPointMode.Mirrored);
			
			splineRList.Add(offset2 - transform.position);
			splineRControlPointsList.Add(BezierControlPointMode.Mirrored);
*/			
		}

		splineLowDetail.points = splineLowDetailList.ToArray();
		splineLowDetail.modes =  splineLowDetailControlPointsList.ToArray();
		//now use this lower detailed curve to add points to mesh

		for (float f = 0; f <=frequency; f++)
		{
			Vector3 position = splineLowDetail.GetPoint((f) * stepSize );
			//Vector3 position = spline.GetPoint(f/frequency);
			Vector3 direction = splineLowDetail.GetDirection((f)*stepSize );
			//Debug.Log(direction);
			
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=bRoadWidth;
			rot2*=bRoadWidth;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;

			vertices.Add(offset1 - transform.position);
			vertices.Add(offset2- transform.position);
			
			//while we are at it, add points to spline so that the verge can use them						
			splineLList.Add(offset1 - transform.position);
			splineLControlPointsList.Add(BezierControlPointMode.Mirrored);
			
			splineRList.Add(offset2 - transform.position);
			splineRControlPointsList.Add(BezierControlPointMode.Mirrored);
		
		
		}
		
		//put the templist in to the spline Class
		
		splineL.points = splineLList.ToArray();
		splineL.modes = splineLControlPointsList.ToArray();
		
		splineR.points = splineRList.ToArray();
		splineR.modes = splineLControlPointsList.ToArray();
		
		//Now back to the road mesh
		
		Mesh mesh = new Mesh();	
		mesh.vertices = vertices.ToArray();
		
		for ( int i = 0; i < vertices.Count -2; i+=2)// less than double of for loop above
		{
			indices.Add(i+2);
			indices.Add(i+1);
			indices.Add(i);	
			indices.Add(i+3);
			indices.Add(i+1);
			indices.Add(i+2);
		}
		
		mesh.triangles = indices.ToArray();
		

		
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();		
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
		mesh.RecalculateNormals();
		meshFilter.sharedMesh = mesh;
		
		Material newMat = Resources.Load("Brown",typeof(Material)) as Material;		
		meshRenderer.material = newMat;
	
		StartCoroutine("JunctionFromCurves");
		StartCoroutine("JunctionFromCurves2");
	}


	/// <summary>
	/// //Create curve from centre of A Road to centre of the end of the B Road
	// with a control points (cloesetInd)
	/// </summary>
	IEnumerator JunctionFromCurves()
	{
		//yield return new WaitForFixedUpdate();
		//create new game object from junction
		Mesh targetRoadMesh;
		//if(mainRoad)
			targetRoadMesh = transform.parent.Find("RingRoad").GetComponent<MeshFilter>().sharedMesh;
		
	//	if(!mainRoad)
	//		targetRoadMesh = transform.parent.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;
		

		junctionFrequency = (bR.Path.Count - (1/bR.Path.Count)) * 100f;


		GameObject junction = new GameObject();
		junction.tag = "Road";
		junction.name = "Junction Section";
		junction.layer = 9;

		BezierSpline junctionSpline = junction.AddComponent<BezierSpline>();
		List<Vector3> tempList = new List<Vector3>();
		List<BezierControlPointMode> controlPointList = new List<BezierControlPointMode>();

	
		//Add the end of BRoad Plotter
		tempList.Add(pathFull[1]);
		tempList.Add(pathFull[0]);
		//Add the control point
		//tempList.Add(targetRoadMesh.vertices[closestInd]);
		//Add a second because bezier spline needs four points to make one curve
		tempList.Add(targetRoadMesh.vertices[closestInd]);


		//Add the point on the A Road at which the junction starts to "flow" from

		Vector3 middle = Vector3.Lerp(targetRoadMesh.vertices[closestInd-junctionWidth],
		                              targetRoadMesh.vertices[closestInd-junctionWidth+1],
		                              0.5f);
		tempList.Add(middle);

		//Add tempList Vector3s to junctionSpline

		junctionSpline.points = tempList.ToArray();

		//Set control points

		for (int i = 0; i<  tempList.Count; i++)
		{
			controlPointList.Add(BezierControlPointMode.Aligned);
		}

		//add control points to the junctionSpline
		junctionSpline.modes = controlPointList.ToArray();

		//Place Cubes along junctionSpline

		float stepSize = junctionFrequency;
		stepSize =1f/(junctionFrequency-1);



		//Mesh junctionSpline

		Mesh mesh = new Mesh();

		//Let's just re use the tempList from before
		tempList.Clear();

		//make the first two pints on the mesh equal to the last two on the B Road

	//	tempList.Add(bRoadMesh.vertices[1]);
		//tempList.Add(bRoadMesh.vertices[0]);

		//Iterate through curve and add points to tempList
		for (int i = 0; i < junctionFrequency; i++)
		{
			Vector3 position = junctionSpline.GetPoint(i * stepSize);
			Vector3 direction = junctionSpline.GetDirection(i * stepSize );
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=2f;
			rot2*=2f;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;

			//Cast a ray to see what is underneath point

			RaycastHit hit;

			LayerMask lm = LayerMask.GetMask("Road");
			if (Physics.Raycast(offset1, -Vector3.up, out hit, 100.0F,lm))
			{
				if (hit.collider.tag == "Road")
				{
					offset1 = hit.point;

				//Debug.Log ("hit road");
		//		Transform item1 = Instantiate(cube) as Transform;
		//		item1.position = hit.point;
		//			item1.name="road hit";
				}
			}

		
			if (Physics.Raycast(offset2, -Vector3.up, out hit, 100.0F,lm))
			{
				if (hit.collider.tag == "Road")
				{
					offset2 = hit.point;
		//	Transform item2 = Instantiate(cube) as Transform;
		//		item2.position = hit.point;
		//			item2.name="road hit2";
				}
			}


			tempList.Add(offset1);
			tempList.Add(offset2);

		}

	
		//add the tempList of vertices to the mesh

		mesh.vertices = tempList.ToArray();

		//Now assign the triangles to the vertices
		List<int> indices = new List<int>();

		for ( int i = 0; i < tempList.Count -2; i+=2)// 
		{
			indices.Add(i+2);
			indices.Add(i+1);
			indices.Add(i);	
			indices.Add(i+3);
			indices.Add(i+1);
			indices.Add(i+2);
		}

		mesh.triangles = indices.ToArray();
		mesh.RecalculateNormals();
		;
		//add components to the junction gameobject


		MeshFilter meshFilter = junction.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		MeshCollider meshCollider = junction.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;

		MeshRenderer meshRenderer = junction.AddComponent<MeshRenderer>();
		meshRenderer.material = Resources.Load("Brown",typeof(Material)) as Material;	

		junction.transform.parent = this.transform;

	//	StartCoroutine("ReScanBRoad");
		yield break;

	}

	IEnumerator JunctionFromCurves2()
	{
		//yield return new WaitForFixedUpdate();
		//create new game object from junction
		
		Mesh targetRoadMesh;
		//if(mainRoad)
			targetRoadMesh = transform.parent.Find("RingRoad").GetComponent<MeshFilter>().sharedMesh;
		
		if(!mainRoad)
			targetRoadMesh = transform.parent.Find("Aroad2(Clone)").Find("BRoad").GetComponent<MeshFilter>().sharedMesh;
		

		junctionFrequency = (bR.Path.Count - (1/bR.Path.Count)) * 100f;
		
		
		GameObject junction = new GameObject();
		junction.tag = "Road";
		junction.name = "Junction Section2";
		junction.layer = 9;
		
		BezierSpline junctionSpline = junction.AddComponent<BezierSpline>();
		List<Vector3> tempList = new List<Vector3>();
		List<BezierControlPointMode> controlPointList = new List<BezierControlPointMode>();
		
		//add the middle of end of the bRoad
		
		//Vector3 middle = Vector3.Lerp(bRoadMesh.vertices[0],
		//                              bRoadMesh.vertices[1],
		//                              0.5f);
		//Add the end of the mesh
		//tempList.Add(middle);
		
		
		//Add the end of BRoad Plotter
		tempList.Add(pathFull[pathFull.Count-2]);
		tempList.Add(pathFull[pathFull.Count-1]);
		//Add the control point
		//tempList.Add(targetRoadMesh.vertices[closestInd]);
		//Add a second because bezier spline needs four points to make one curve

//		if(mainRoad)
//			targetRoadMesh = transform.parent.FindChild("RingRoad").GetComponent<MeshFilter>().sharedMesh;
		
//		if(!mainRoad)
//			targetRoadMesh = transform.parent.FindChild("Aroad2(Clone)").FindChild("BRoad").GetComponent<MeshFilter>().sharedMesh;

		tempList.Add(targetRoadMesh.vertices[startInd]);
		
		
		//Add the point on the A Road at which the junction starts to "flow" from
		
		Vector3 middle = Vector3.Lerp(targetRoadMesh.vertices[startInd-junctionWidth],
		                              targetRoadMesh.vertices[startInd-junctionWidth+1],
		                              0.5f);
		tempList.Add(middle);
		
		//Add tempList Vector3s to junctionSpline
		
		junctionSpline.points = tempList.ToArray();
		
		//Set control points
		
		for (int i = 0; i<  tempList.Count; i++)
		{
			controlPointList.Add(BezierControlPointMode.Aligned);
		}
		
		//add control points to the junctionSpline
		junctionSpline.modes = controlPointList.ToArray();
		
		//Place Cubes along junctionSpline
		
		float stepSize = junctionFrequency;
		stepSize =1f/(junctionFrequency-1);
		
		
		
		//Mesh junctionSpline
		
		Mesh mesh = new Mesh();
		
		//Let's just re use the tempList from before
		tempList.Clear();
		
		//make the first two pints on the mesh equal to the last two on the B Road
		
		//	tempList.Add(bRoadMesh.vertices[1]);
		//tempList.Add(bRoadMesh.vertices[0]);
		
		//Iterate through curve and add points to tempList
		for (int i = 0; i < junctionFrequency; i++)
		{
			Vector3 position = junctionSpline.GetPoint(i * stepSize);
			Vector3 direction = junctionSpline.GetDirection(i * stepSize );
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=2f;
			rot2*=2f;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;
			
			//Cast a ray to see what is underneath point
			
			RaycastHit hit;
			
			LayerMask lm = LayerMask.GetMask("Road");
			if (Physics.Raycast(offset1, -Vector3.up, out hit, 100.0F,lm))
			{
				if (hit.collider.tag == "Road")
				{
					offset1 = hit.point;
					
					//Debug.Log ("hit road");
					//		Transform item1 = Instantiate(cube) as Transform;
					//		item1.position = hit.point;
					//			item1.name="road hit";
				}
			}
			
			
			if (Physics.Raycast(offset2, -Vector3.up, out hit, 100.0F,lm))
			{
				if (hit.collider.tag == "Road")
				{
					offset2 = hit.point;
					//	Transform item2 = Instantiate(cube) as Transform;
					//		item2.position = hit.point;
					//			item2.name="road hit2";
				}
			}
			
			
			tempList.Add(offset1);
			tempList.Add(offset2);
			
		}
		
		
		//add the tempList of vertices to the mesh
		
		mesh.vertices = tempList.ToArray();
		
		//Now assign the triangles to the vertices
		List<int> indices = new List<int>();
		
		for ( int i = 0; i < tempList.Count -2; i+=2)// 
		{
			indices.Add(i+2);
			indices.Add(i+1);
			indices.Add(i);	
			indices.Add(i+3);
			indices.Add(i+1);
			indices.Add(i+2);
		}
		
		mesh.triangles = indices.ToArray();
		mesh.RecalculateNormals();
		;
		//add components to the junction gameobject
		
		
		MeshFilter meshFilter = junction.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		
		MeshCollider meshCollider = junction.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
		
		MeshRenderer meshRenderer = junction.AddComponent<MeshRenderer>();
		meshRenderer.material = Resources.Load("Brown",typeof(Material)) as Material;	
		
		junction.transform.parent = this.transform;
		
		//	StartCoroutine("ReScanBRoad");
		yield break;
		
	}

	IEnumerator ReScanBRoad()
	{
		//Raycast down form original BRoad looking for junction and setting the B Road to the Junction'sY coordinate
		yield return new WaitForSeconds(1);

		List<Vector3> vertices = gameObject.transform.Find("BRoad").GetComponent<MeshFilter>().mesh.vertices.ToList();
		Mesh mesh = gameObject.transform.Find("BRoad").GetComponent<MeshFilter>().mesh;
		int vCount = gameObject.transform.Find("BRoad").GetComponent<MeshFilter>().mesh.vertexCount;

		for (int i = 0; i < vCount; i++)
		{
			RaycastHit hit;
			LayerMask lm = LayerMask.GetMask("Road");
			if (Physics.Raycast(mesh.vertices[i], -Vector3.up, out hit, 100.0F,lm))
			{

					vertices[i] = hit.point;
				//	Transform item = Instantiate(cube) as Transform;
				//	item.position = hit.point;
				//	item.name = "JunctionHit";


			}

		}

		gameObject.transform.Find("BRoad").GetComponent<MeshFilter>().mesh.vertices = vertices.ToArray();
		gameObject.transform.Find("BRoad").GetComponent<MeshCollider>().sharedMesh =
			gameObject.transform.Find("BRoad").GetComponent<MeshFilter>().mesh;
		
		yield break;
	}


	void moveTerrain(Vector3 position)
	{

		Mesh meshTerrain = GameObject.FindGameObjectWithTag("TerrainBase").GetComponent<MeshFilter>().mesh;
		Vector3[] verticesTerrain = meshTerrain.vertices;
		int[] triangles = meshTerrain.triangles;
		Vector3 yAdd = new Vector3(0f,200f,0f);
		RaycastHit hit;

		if (Physics.Raycast(position+yAdd - transform.position, -Vector3.up, out hit, 500.0F,9))//shoots ray down the way from above ignoring road layer (9)
		{			
			
			//Deform Terrain under where road is placed// put this in coroutine
			
			MeshCollider meshColliderTerrain = hit.collider as MeshCollider;
			if (meshColliderTerrain == null || meshColliderTerrain.sharedMesh == null)
				return;
			
			Vector3 p0 = verticesTerrain[triangles[hit.triangleIndex * 3 + 0]];
			Vector3 p1 = verticesTerrain[triangles[hit.triangleIndex * 3 + 1]];
			Vector3 p2 = verticesTerrain[triangles[hit.triangleIndex * 3 + 2]];
			
			
			p0.y = position.y - raiseAmt - transform.position.y;
			p1.y = position.y -raiseAmt- transform.position.y;  ///this works -- to improve, find closest surround ing points and smooth
			p2.y = position.y-raiseAmt - transform.position.y;
			Debug.Log("working");
			verticesTerrain[triangles[hit.triangleIndex * 3 + 0]] = p0;
			verticesTerrain[triangles[hit.triangleIndex * 3 + 1]] = p1;
			verticesTerrain[triangles[hit.triangleIndex * 3 + 2]] = p2;
			
			
		}
	//	Transform terrainCube = Instantiate(cube) as Transform;
	//	terrainCube.position= hit.point;
	//	meshTerrain.vertices = verticesTerrain.ToArray(); //find a better place to put this. Calls every pint being scanned
	
	}

}
