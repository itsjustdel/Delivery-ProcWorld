using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class Verge : MonoBehaviour {

	public bool forRingRoad;
	public LayerMask myLayerMask;
	public Transform cubePrefab;
	private BezierSpline bezierL;
	private BezierSpline bezierR;
	private MeshFilter meshFilter;
	private SplineMesh splineMesh;
	private BRoadMesher bRoadMesher;
	private RingRoadMesh ringRoadMesh;
	public float frequency = 5000;
	private GameObject plane;
	public bool doCubes = false;
	public int xSize = 5000;
	public int ySize = 6;
	public float yScaler = 0.5f;
	public List<Vector3> lastRowListLeft = new List<Vector3>();
	public List<Vector3> lastRowListRight = new List<Vector3>();
    public CellManager cellManager;
//	public List<int> junctionIndexList;

	public bool finished;

//	public int zSize = 100;
	// Use this for initialization
	void Awake(){
		//enabled =false;
	}

	void Start () {
        cellManager = GameObject.Find("WorldPlan").GetComponent<CellManager>();
		myLayerMask = LayerMask.GetMask("TerrainBase");
   

        CreateCurves();


		//bezierL = splineMesh.newBezierL;
		//bezierR = splineMesh.newBezierR;
	//	lastRowListLeft = new List<Vector3>();
	//	lastRowListRight = new List<Vector3>();


		StartCoroutine(BuildMesh(bezierR,"right"));
		StartCoroutine(BuildMesh(bezierL,"left"));

		//turn off meshCollider on terrain now we are finsihed with it
		//GameObject.FindGameObjectWithTag("TerrainBase").GetComponent<MeshCollider>().enabled = false;


	}

	
	void Update()
	{
		if (doCubes == true) {StartCoroutine("Cubes");doCubes = false;}
	}

    void CreateCurves()
    {
        //BezierSpline updatedSpline = GetComponent<RingRoadMesh>().updatedSpline;
        BezierSpline updatedSpline = GetComponent<RingRoadMesh>().spline;
        float frequency = GetComponent<RingRoadMesh>().frequency;
        float stepSize = 1f / frequency;
        float roadWidth = GetComponent<RingRoadMesh>().roadWidth;

        List<Vector3> splineLList = new List<Vector3>();
        List<BezierControlPointMode> splineLControlPointsList = new List<BezierControlPointMode>();

        List<Vector3> splineRList = new List<Vector3>();
        List<BezierControlPointMode> splineRControlPointsList = new List<BezierControlPointMode>();

        for (int i = 0; i <= frequency; i++)
        {
            //use the function "GetPoint" in the bezier class
            Vector3 position = updatedSpline.GetPoint((i) * stepSize);
           
            //use the function "GetDirection" - this gives us the normalized velocity at the given point
            Vector3 direction = updatedSpline.GetDirection((i) * stepSize);
            
            Vector3 rot1 = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rot2 = Quaternion.Euler(0, 90, 0) * direction;

            
            //create directions to add points each side of the curve for mesh
            rot1 *= roadWidth;
            rot2 *= roadWidth;

            Vector3 offset1 = position + rot1;
            Vector3 offset2 = position + rot2;

            //these lists will hold info for the verge script to use	

            //move thee points towards the centre of the road slightly to ensure there ar no gaps in the mesh
            Vector3 modified1 = Vector3.Lerp(position, offset1, .9f); //change to 0.9  to move to the inside of the road slightly		
            splineLList.Add(modified1 - transform.position);
            splineLControlPointsList.Add(BezierControlPointMode.Aligned);

            Vector3 modified2 = Vector3.Lerp(position, offset2, .9f);
            splineRList.Add(modified2 - transform.position);
            splineRControlPointsList.Add(BezierControlPointMode.Aligned);
        }

        //Instantiate Curves
        bezierL = gameObject.AddComponent<BezierSpline>();
        bezierL.points = splineLList.ToArray();
        bezierL.modes = splineLControlPointsList.ToArray();


        bezierR = gameObject.AddComponent<BezierSpline>();
        bezierR.points = splineRList.ToArray();
        bezierR.modes = splineRControlPointsList.ToArray();
    }

	IEnumerator BuildMesh(BezierSpline bezier, string side) 
	{
	    //use ringroadmesh frequency
        
        //frequency = gameObject.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount;

        //frequency = gameObject.transform.GetComponent<RingRoadList>().ringRoadPoints.Count * 200; //took out voronoi road builder, asigning directly
                                                   //meshFilter = plane.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
		List<Vector3> tempList = new List<Vector3>();


//		Mesh meshTerrain = GameObject.FindGameObjectWithTag("TerrainBase").GetComponent<MeshFilter>().mesh;	
		Vector3 yAdd = new Vector3(0f,500f,0f);


        xSize = (int)frequency;

		for (int r = 0; r <=  ySize ;r++)//how deep each section is
		{
          //  yield return new WaitForEndOfFrame();

			for (float i = 0; i <= xSize;i++)//how long each section is// Change += to change the density level
			{
                if (i % 1000 == 0  && i != 0)
                    yield return new WaitForEndOfFrame();

				Vector3 position = bezier.GetPoint((i)/(frequency-1));
				Vector3 direction = bezier.GetDirection (i/(frequency-1));
//				float random = Random.Range(0.5f,1.5f);
				Vector3 pAndR = Vector3.zero;
				Vector3 rot1 = Vector3.zero;
				//Check what side of the road it has been passed and adjust directional vector accordingly
				if (side == "left")
				{
					rot1 = Quaternion.Euler (0, -90, 0) * (direction*r*yScaler);
					//rot1 = Vector3.right*r;
				} 
				else if (side == "right")
				{
					rot1 = Quaternion.Euler (0, 90, 0) * (direction*r*yScaler);
					//rot1 = Vector3.left*r;
				}

				// Ignore first row of mesh, as this is attached to the kerbside, move all other mesh heights to match terrain//
				if (r!=0)
				{
					RaycastHit hit;
                    //look for road, if it hits a road, we are at a junction, hide verge under road

                    if (Physics.Raycast(position + rot1 + (Vector3.up*100) - transform.position, -Vector3.up, out hit, 1000.0F, LayerMask.GetMask("Road")))
                    {
                        tempList.Add(position + rot1 - Vector3.up);

                        //stop the road here
                       // xSize = (int)i;
                    }

                    else if (Physics.Raycast(position+rot1+yAdd-transform.position, -Vector3.up, out hit, 1000.0F, myLayerMask))
					{					
						pAndR = position+rot1 - transform.position;
						Vector3 target = new Vector3(pAndR.x,hit.point.y,pAndR.z);
						tempList.Add(target);
						//tempList.Add(position+rot1);
					}
					else
					{
						tempList.Add (position+transform.position);
						//Debug.Log("didn't hit");
					}
				}
				else if (r==0)				
				{

                    RaycastHit hit;
                    //look for road, if it hits a road, we are at a junction, hide verge under road

                    if (Physics.Raycast(position + rot1 + yAdd - transform.position, -Vector3.up, out hit, 1000.0F, LayerMask.GetMask("Road")))
                    {
                        //if it hits a road that isnt its own road, hide underneath (at a junction)
                        Transform ownTransform = transform.Find("RingRoad").transform;
                        Transform hitTransform = hit.transform;

                        if (ownTransform != hitTransform)
                            tempList.Add(hit.point - Vector3.up);
                        else
                            tempList.Add(position + rot1 - transform.position);

                    }
                    else
                        tempList.Add(position+rot1 - transform.position);   //could spherecast and attach to nearest road mesh?
				}


                
				//creates a list the place along curve script uses

				if (r == ySize)
				{
					if (side == "left")
					{
						lastRowListLeft.Add(position+rot1- transform.position);
						//Debug.Log("adding left");
					}
					if (side == "right")
					{
						lastRowListRight.Add(position+rot1 - transform.position);
						//Debug.Log("adding right");
					}
				
				}
                
				//Debug.Log (xSize*ySize);
			}

		}


		int[] triangles = new int[(xSize * ySize) * 6];

		if (side == "left")
		{
			for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}
			}
		}

		if (side == "right")
		{
			for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
				for (int x = 0; x < xSize; x++, ti += 6, vi++) {
					triangles[ti] = vi;
					triangles[ti + 4] = triangles[ti + 1] = vi + 1;
					triangles[ti + 3] = triangles[ti + 2] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}
			}
		}

//		Debug.Log(tempList.Count);
//		Debug.Log(triangles.Length);
		mesh.vertices = tempList.ToArray();
		//mesh.normals = normals;
		mesh.triangles = triangles;


		mesh.RecalculateNormals();
		;

		GameObject verge = new GameObject();
		verge.transform.parent = this.gameObject.transform;
		verge.name = "Verge";
		verge.tag = "TerrainSecondLayer";
		verge.layer = 10;
		MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
		meshFilterInstance.mesh = mesh;
		MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        meshRenderer.sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
		MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;

		finished =true;

        //tell cell manager we have finished this verge
        cellManager.vergesBuilt++;
		yield break;
	}

	IEnumerator Cubes()
	{

		meshFilter = gameObject.GetComponent<MeshFilter>();
		for (int i = 0; i <meshFilter.sharedMesh.vertexCount;i++)
		{

			Mesh mesh = meshFilter.sharedMesh;
			Transform cube = Instantiate(cubePrefab) as Transform;
			cube.position = mesh.vertices[i];
			yield return new WaitForFixedUpdate();
		}
	}



}
