using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class SplineMesh : MonoBehaviour {
	private float initialCurveDensity;
	public float initialCurveDensityMultiplier = 1f;
	public float curveSmoothingAmount = 50f;
	public float roadTotalWidth =3;
	public float roadMeshSegmentWidth=1;
	public BezierSpline spline;
	public BezierSpline splineL;
	//public BezierSpline splineL2;
	public BezierSpline splineR;
	//public BezierSpline splineR2;
	public BezierSpline splineM;
	//public BezierSpline splineM2;
	public BezierSpline newBezierL;
	public BezierSpline newBezierR;
	public int frequency;
	public GridPlayer gp;
	public bool lookForward;
	public float raiseAmt = 0.5f;
	public Transform[] items;
	public Transform[] items2;

	public Transform cube;
	//private List<Vector3> normals = new List<Vector3>();


	void Start() {


		meshItUpOneSegment();


	}

	void meshItUpOneSegment()
	{
		frequency = gp.Path.Count*10;


//		splineM = gameObject.AddComponent<BezierSpline>();
//		List<Vector3> splineMList = new List<Vector3>();
//		List<BezierControlPointMode> splineMControlPointsList = new List<BezierControlPointMode>();
		//variables for verge
		splineL = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineLList = new List<Vector3>();
		List<BezierControlPointMode> splineLControlPointsList = new List<BezierControlPointMode>();

		splineR = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineRList = new List<Vector3>();
		List<BezierControlPointMode> splineRControlPointsList = new List<BezierControlPointMode>();

		List<Vector3> vertices = new List<Vector3>();		
		List<int> indices = new List<int>();

		float stepSize = frequency;		
		stepSize =1f/(frequency-1);

		for (float f = 0; f <= (frequency); f++) { 
			
			Vector3 position = spline.GetPoint((f) * stepSize );
			//Vector3 position = spline.GetPoint(f/frequency);
			Vector3 direction = spline.GetDirection((f)*stepSize );
			//Debug.Log(direction);

			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=roadTotalWidth;
			rot2*=roadTotalWidth;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;
		
			
			vertices.Add(offset1);
			vertices.Add(offset2);

			//insert in to new spine
//			splineMList.Add(position);
//			splineMControlPointsList.Add(BezierControlPointMode.Mirrored);

			//while we are at it, add points to spline so that the verge can use them						
			splineLList.Add(offset1 - transform.position);
			splineLControlPointsList.Add(BezierControlPointMode.Mirrored);
			
			splineRList.Add(offset2 - transform.position);
			splineRControlPointsList.Add(BezierControlPointMode.Mirrored);
			
		}
		//put the templist in to the spline Class

//		splineM.points = splineMList.ToArray();
//		splineM.modes = splineMControlPointsList.ToArray();
		
		splineL.points = splineLList.ToArray();
		splineL.modes = splineLControlPointsList.ToArray();
		
		splineR.points = splineRList.ToArray();
		splineR.modes = splineLControlPointsList.ToArray();

	//	frequency = gp.Path.Count*100;
//		for (float f = 0; f <= (frequency); f++) { 
//			
//			Vector3 position = splineM.GetPoint((f) * stepSize );
//			//Vector3 position = spline.GetPoint(f/frequency);
//			Vector3 direction = splineM.GetDirection((f)*stepSize );
//			//Debug.Log(direction);
//			
//			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
//			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
//			rot1*=roadTotalWidth;
//			rot2*=roadTotalWidth;
//			Vector3 offset1 = rot1+position;
//			Vector3 offset2 = rot2+position;
//			
			
		//		vertices.Add(offset1);
		//		vertices.Add(offset2);			
		
			
//		}





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

		GameObject child = new GameObject();
		child.name = "ARoad";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		child.layer = 9;
		child.tag = "Road";
		
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





	}

	void meshItUpTwoSegment()
	{
		//how detailed the initial curve we set is

		//multipier is how many sections the road will be split in to
		frequency = gp.Path.Count;
		initialCurveDensity = frequency*initialCurveDensityMultiplier;
		//for (float k = 0; k < frequency ; k+=(frequency/10))
		//{
		int k = 0;


		List<Vector3> vertices = new List<Vector3>();		
		List<int> indices = new List<int>();				


		splineL = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineLList = new List<Vector3>();
		List<BezierControlPointMode> splineLControlPointsList = new List<BezierControlPointMode>();
		splineR = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineRList = new List<Vector3>();
		List<BezierControlPointMode> splineRControlPointsList = new List<BezierControlPointMode>();
		splineM = gameObject.AddComponent<BezierSpline>();
		List<Vector3> splineMList = new List<Vector3>();
		List<BezierControlPointMode> splineMControlPointsList = new List<BezierControlPointMode>();
	
		
		if (frequency <= 2){// || items == null || items.Length == 0) {
			return;
		}
		float stepSize = frequency;// * items.Length;
	
		
		stepSize =1f/(frequency-1);

	 	
		//Gets curve from RoadPlotter and creates two new curves each side of the it
		//Creates two curves from one
		for (float f = 0; f <= (frequency); f++) { 

			Vector3 position = spline.GetPoint((f+k) * stepSize );
			//Vector3 position = spline.GetPoint(f/frequency);
			Vector3 direction = spline.GetDirection((f+k)*stepSize );
			Debug.Log(direction);
/*			//Vector3 direction = spline.GetVelocity((f+k)*stepSize);
			Vector3 prevDir = spline.GetVelocity((f+k-10)*stepSize);
			Vector3 nextDir = spline.GetVelocity((f+k+10)*stepSize);
			Vector3 prevDir2 = spline.GetVelocity((f+k-20)*stepSize);
			Vector3 nextDir2 = spline.GetVelocity((f+k+20)*stepSize);
			Vector3 prevDir3 = spline.GetVelocity((f+k-30)*stepSize);
			Vector3 nextDir3 = spline.GetVelocity((f+k+30)*stepSize);
			direction += nextDir+prevDir + nextDir2+prevDir3 +nextDir3+prevDir3; 
*///			direction *=0.01f;
			Vector3 rot1 = Quaternion.Euler(0,-90,0) * direction; 
			Vector3 rot2 = Quaternion.Euler(0,90,0) * direction; 
			rot1*=roadTotalWidth;
			rot2*=roadTotalWidth;
			Vector3 offset1 = rot1+position;
			Vector3 offset2 = rot2+position;



	//		Transform item = Instantiate(cube) as Transform;
	//		item.position = offset1 - transform.position;
	//		item.name = "splineMesh Cube" + f;
		
			splineMList.Add(position - transform.position);
			splineMControlPointsList.Add(BezierControlPointMode.Aligned);

			splineLList.Add(offset1 - transform.position);
			splineLControlPointsList.Add(BezierControlPointMode.Aligned);

			splineRList.Add(offset2 - transform.position);
			splineRControlPointsList.Add(BezierControlPointMode.Aligned);
			
		}
			
		//set the temp list we just filled up to the instantiated beziers
		//also adds the corresponding control point modes to be referenced by the curve
		splineM.points = splineMList.ToArray();
		splineM.modes = splineMControlPointsList.ToArray();

		splineL.points = splineLList.ToArray();
		splineL.modes = splineLControlPointsList.ToArray();

		splineR.points = splineRList.ToArray();
		splineR.modes = splineLControlPointsList.ToArray();

	
		//iterate through bezier and add points to mesh vertice temp List "vertices"
		//Runs through the two new curves and creates a list for the next "smooth" curve
		List<Vector3> newCurveR = new List<Vector3>();
		List<Vector3> newCurveM = new List<Vector3>();
		List<BezierControlPointMode> newCurveRControlPoints = new List<BezierControlPointMode>();
		List<BezierControlPointMode> newCurveMControlPoints = new List<BezierControlPointMode>();
		for (float i = 0; i <= frequency; i+=frequency/initialCurveDensity)  

			//perhaps the extra bit of mesh at the end is due to the array finishing on a control point?
			//add in fours?
		{
			Vector3 position = splineM.GetPoint(i * stepSize );
			//vertices.Add(position);										//testing//put straight in to mesh
			Vector3 position2 = splineR.GetPoint(i * stepSize );
			//vertices.Add(position2);									//testing

			//create new central curve
			newCurveM.Add(position);
			newCurveMControlPoints.Add(BezierControlPointMode.Aligned);

			//create new "left" curve
			newCurveR.Add(position2);

			newCurveRControlPoints.Add(BezierControlPointMode.Aligned);
		//	Transform item = Instantiate(cube) as Transform;
		//	item.position = position + transform.position;
		//	item.name = "vertices";
		}



		newBezierR = gameObject.AddComponent<BezierSpline>();
		newBezierR.points = newCurveR.ToArray();
		newBezierR.modes = newCurveRControlPoints.ToArray();
		BezierSpline newBezierM = gameObject.AddComponent<BezierSpline>();
		newBezierM.points = newCurveM.ToArray();
		newBezierM.modes = newCurveMControlPoints.ToArray();


		float t = frequency/initialCurveDensity;
		for (float i = 0; i <= frequency; i+=(t*(1/curveSmoothingAmount)))
		{

			Vector3 position = newBezierM.GetPoint(i * stepSize);
			Vector3 position2 = newBezierR.GetPoint(i * stepSize);
		//		Transform item = Instantiate(cube) as Transform;
		//		item.position = position2 + transform.position;
		//	Transform item2 = Instantiate(cube) as Transform;
		//	item2.position = position + transform.position;

			vertices.Add(position);
			vertices.Add(position2);
		}


		
		GameObject child = new GameObject();
		child.name = "ARoadR";
		child.transform.position = this.gameObject.transform.position;
		child.transform.parent = this.gameObject.transform;
		child.layer = 9;
		child.tag = "Road";
		
		MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
		MeshFilter meshFilter = child.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
		//		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		meshCollider.sharedMesh = mesh;
		mesh.RecalculateNormals();
		meshFilter.sharedMesh = mesh;
		
		Material newMat = Resources.Load("Brown",typeof(Material)) as Material;		
		meshRenderer.material = newMat;

		//end of for loop for cutting road up goes here


////////////////////////////////////////////////
		/// other side of road
		/// 
		List<Vector3> vertices2 = new List<Vector3>();		
		List<int> indices2 = new List<int>();		

		//iterate through bezier and add points to mesh vertice temp List "vertices"
		//Runs through the two new curves and creates a list for the next "smooth" curve
		List<Vector3> newCurveL = new List<Vector3>();

		List<BezierControlPointMode> newCurveLControlPoints = new List<BezierControlPointMode>();
	
		for (float i = 0; i <= frequency; i+=frequency/initialCurveDensity)  
		
		{
//			Vector3 position = splineM.GetPoint(i * stepSize );

			Vector3 position2 = splineL.GetPoint(i * stepSize );

			//create new "left" curve
			newCurveL.Add(position2);
			
			newCurveLControlPoints.Add(BezierControlPointMode.Aligned);
		
		}	
		
		
		newBezierL = gameObject.AddComponent<BezierSpline>();
		newBezierL.points = newCurveL.ToArray();
		newBezierL.modes = newCurveLControlPoints.ToArray();	
	
		for (float i = 0; i <= frequency; i+=(t*(1/curveSmoothingAmount)))
		{
			Vector3 position = newBezierM.GetPoint(i * stepSize);
			Vector3 position2 = newBezierL.GetPoint(i * stepSize);

			//Add to temp list vertices2 in inverted order to make triangles face up
			vertices2.Add(position2);
			vertices2.Add(position);
		}
		
		
		
		GameObject child2 = new GameObject();
		child2.name = "ARoadL";
		child2.transform.position = this.gameObject.transform.position;
		child2.transform.parent = this.gameObject.transform;
		child2.layer = 9;
		child2.tag = "Road";
		
		MeshCollider meshCollider2 = child2.gameObject.AddComponent<MeshCollider>();
		MeshFilter meshFilter2 = child2.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer2 = child2.gameObject.AddComponent<MeshRenderer>();
		meshRenderer2.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		Mesh mesh2 = new Mesh();	
		mesh2.vertices = vertices2.ToArray();
		
		for ( int i = 0; i < vertices2.Count -2; i+=2)// less than double of for loop above
		{
			indices2.Add(i+2);
			indices2.Add(i+1);
			indices2.Add(i);	
			indices2.Add(i+3);
			indices2.Add(i+1);
			indices2.Add(i+2);
		}
		
		mesh2.triangles = indices2.ToArray();

		mesh2.RecalculateBounds();
		meshCollider2.sharedMesh = mesh2;
		mesh2.RecalculateNormals();
		meshFilter2.sharedMesh = mesh2;
	
		meshRenderer2.material = newMat;


	}

	IEnumerator Cubes(Mesh mesh)
	{
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			
			Transform item = Instantiate(cube) as Transform;
			item.position = mesh.vertices[i];
			yield return new WaitForFixedUpdate();
			
		}
	}
}