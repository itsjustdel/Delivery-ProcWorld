using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DualGraph2d;
using System;
using System.Linq;

[RequireComponent (typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour {

	//public BuildController buildController;
	public Material[] materials;
	public Transform cubePrefab;
	public BezierSpline roadCurve;
	//public BezierSpline roadCurveL;
	//public BezierSpline roadCurveR;
	//public float roadCurveFrequency;
	//public float roadSpread = 3f;
	//public GridPlayer gridPlayer;
    private List<Vector3> roadList = new List<Vector3>();
    public Vector3 volume;//= new Vector3(20.0f,0.0f,20.0f);
	public float rootTriMultiplier=1.0f;
	public int cellNumber= 20;
    public bool useSortedGeneration;//=true;
	public bool drawCells=false;
	public bool drawDeluany=false;
	public bool drawRoots=false;
	public bool drawVoronoi=false;
	public bool drawGhostEdges=false;
	public bool drawPartialGhostEdge=false;
	public bool drawCircumspheres=false;
	public Color sphereColor= Color.cyan;
	

	public DualGraph dualGraph;
//	private float totalTime;
	private float computeTime;
	private Mesh graphMesh;

	public bool addRoad;
	public bool fillWithRandom;
	public bool fillWithPoints;
	public bool addToBuildController;
    public bool worldPlan;
    public bool addMeshCollider;
    public bool addCellYAdjust;
	public bool addPolygonTester;
	public bool addSubdivideMesh;
	public bool addPathFinder;
	//public bool forHex;
    public bool gardenFeature;
    public bool shrinkCells;
    public bool extrudeCells;
    public bool combineCells;
    
    public Vector3 moveToAfterBuilding;
    

    public bool junctions;
    public bool fields;
    public bool voronoiWaypoints;
    public bool ringRaycast;
    public bool renderCells;
    public bool joinCellAndVerge;
    public bool test;
    public bool voronoiRoadLayout;
    public bool combinePointsWithinThreshold;
    public float threshold = 2f;

	public List<Vector3> yardPoints = new List<Vector3>();
 //   public List<GameObject> cellsList = new List<GameObject>();
   // public List<Mesh> meshList = new List<Mesh>();
    public List<Vector3[]> meshVerts = new List<Vector3[]>();
    public List<int[]> meshTris = new List<int[]>();
	public void Start () {

     
        //Go get points from Road Curve

		//make sure yardpoints y points are zerod

		for (int i = 0; i < yardPoints.Count; i++)
		{
			Vector3 temp = yardPoints[i];
			temp = new Vector3(yardPoints[i].x,0f,yardPoints[i].z);
			yardPoints[i] = temp;

		}

        if(!test)
            materials = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().myMaterials;

		dualGraph = new DualGraph(volume);
		
		if (cellNumber<1)
			cellNumber=1;

		if (fillWithPoints)
			cellNumber = yardPoints.Count;

        if (combinePointsWithinThreshold)
        {
          //  Debug.Log("yard points before = " + yardPoints.Count);
            Vector3[] tempArray = RemovePointsWithinThreshold.Clean(yardPoints.ToArray(), 100, threshold);
            yardPoints = new List<Vector3>();
            foreach (Vector3 v3 in tempArray)
                yardPoints.Add(v3);

         //   Debug.Log("yard points after = " + yardPoints.Count);
        }

        StartCoroutine("GeneratePoints");
		
		
		
	//	float meshTime= Time.realtimeSinceStartup;
	//	computeTime= meshTime- computeTime;

        //GenerateMesh();
        //StartCoroutine("GenerateMesh"); //need to create bools for scripts finishing, daisy chain instead od wait for frame end

        /*
		meshTime= Time.realtimeSinceStartup- meshTime;
		totalTime= Time.realtimeSinceStartup-totalTime;
		Debug.Log("generation time: "+ (totalTime-computeTime-meshTime) +"; Graph computation time: "+ computeTime +"; Mesh generation time: "+ meshTime +"; total time: "+ totalTime);
		Debug.Log("Cells:"+dualGraph.cells.Count+"; Spheres: "+dualGraph.spheres.Count+"; volume: "+DualGraph.volume);
        */

        //add colliders/and setup scripts to each cell
        
        //starts the road planning phase if bool is ticked
       


        //		if(forHex)
        //		{
        //			
        //			StartCoroutine("BuildHexGrid");	//triggering fomr worldbuildoreder now
        //		}

        

	}

    IEnumerator GeneratePoints()
    {

        Vector3[] points = new Vector3[cellNumber];

        if (useSortedGeneration)
            GenSortedRandCells(ref points);
        else
            GenRandCells(ref points);


        dualGraph.DefineCells(points, rootTriMultiplier);
    //    computeTime = Time.realtimeSinceStartup;

        if (useSortedGeneration)
            dualGraph.ComputeForAllSortedCells();
        else
            dualGraph.ComputeForAllCells();

        yield return new WaitForEndOfFrame();

        StartCoroutine("GenerateMesh");

        yield break;
    }

    IEnumerator AddToCells()
    {

        for(int i = 0; i < meshVerts.Count; i++)
        {
            //create a game object for each cell in the mesh list
            GameObject cell = new GameObject();
            cell.transform.parent = this.gameObject.transform;            
            cell.name = "Cell";
            cell.tag = "Cell";
            if (worldPlan)
                cell.layer = 8;
            else
                cell.layer = 13;

            //create a mesh from the already populated lists
            Mesh mesh = new Mesh();
            mesh.vertices = meshVerts[i];
            mesh.triangles = meshTris[i];

            MeshFilter meshFilter = cell.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            if (addMeshCollider)
            {
                MeshCollider meshCollider = cell.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }

            MeshRenderer meshRenderer = cell.AddComponent<MeshRenderer>();
            if(!test)
            meshRenderer.enabled = false;

            meshRenderer.sharedMaterial = Resources.Load("Cement") as Material;


            if (!gardenFeature)
            {              

                if (renderCells)
                    meshRenderer.enabled = true;
                if (!test)
                {
                    int r = UnityEngine.Random.Range(0, materials.Length - 1);
                    meshRenderer.sharedMaterial = materials[r];
                }
            }
          
            //bottleneck protection, build a 100 at a time
            if (i != 0 && i % 100 == 0)
                yield return new WaitForEndOfFrame();

            

            if (addCellYAdjust)
            {
                CellYAdjust cya = cell.AddComponent<CellYAdjust>();          

            }
            if (addPolygonTester)
                cell.AddComponent<PolygonTester>();

            if (addSubdivideMesh)
                cell.AddComponent<SubdivideMesh>();

            if (joinCellAndVerge)
                cell.AddComponent<JoinCellAndVerge>();

            if (shrinkCells)
                cell.AddComponent<ShrinkCells>();

            if (extrudeCells)
            {
                ExtrudeCell ex = cell.AddComponent<ExtrudeCell>();    
            }

            //simple flag script
            // cell.AddComponent<CellHasBeenBuilt>(); //unused?


        }


        if (extrudeCells)
        {
            yield return new WaitForEndOfFrame();
            CombineChildren cc = gameObject.AddComponent<CombineChildren>();

            transform.position = moveToAfterBuilding;
        }
        if(combineCells)
        {
            CombineChildren cc = gameObject.AddComponent<CombineChildren>();
            cc.addAutoWeld = true;
          
            yield return new WaitForEndOfFrame();
            gameObject.GetComponent<VoronoiChunk>().JoinBorder(); //doing a while/wait loop in VoronoiChunk now --not working
        }

        if (ringRaycast)
            StartCoroutine("RingRayCast");

        if (addPathFinder)
            StartCoroutine("ActivatePathFinder");

        if (junctions)
            StartCoroutine("Junctions");

        if (fields)
            StartCoroutine("ActivateFields");

        if (voronoiWaypoints)
            StartCoroutine("VoronoiWaypoints");

        if (voronoiRoadLayout)
            GetComponent<VoronoiRoadBuilder>().enabled = true;

        yield break;
    }

    IEnumerator RingRayCast()
    {
        yield return new WaitForEndOfFrame();
            GetComponent<RingRaycast>().Start();
        yield break;
    }

    IEnumerator Junctions()
    {
        yield return new WaitForEndOfFrame();

        //cells added to this list from ringroad list. raycast around end of road

        CellManager cm = GameObject.FindGameObjectWithTag("WorldPlan").GetComponent<CellManager>();

        List<JunctionArea> jas = cm.junctions;

        foreach(JunctionArea ja in jas)
        {
            ja.enabled = true;
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
   
    IEnumerator VoronoiWaypoints()
    {
        yield return new WaitForEndOfFrame();
        gameObject.AddComponent<VoronoiWaypoints>().enabled = true;
        
        yield break;
    }

    IEnumerator BuildHexGrid()
	{
		GameObject hexGrid = GameObject.Find("HexGrid");
		hexGrid.SetActive(true);

		yield return new WaitForEndOfFrame();
		yield break;
	}
	IEnumerator ActivatePathFinder()
	{
		yield return new WaitForEndOfFrame();
		gameObject.GetComponent<Pathfinder>().enabled = true;
	}
    IEnumerator ActivateFields()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        GetComponent<Fields>().Start();
    }
    /// <summary>
    /// Generates random cells.
    /// </summary>
    /// <param name="p">P.</param>
    /// <summary>
    /// Generates random cells.
    /// </summary>
    /// <param name="p">P.</param>
    private void GenRandCells(ref Vector3[] p){		

		List<Vector3> tempList = new List<Vector3>();

	/*	for (int i = 0; i <gridPlayer.Path.Count;i++)
		{
			Vector3 position = gridPlayer.Path[i] - transform.position;
			position.y = 0f;
			tempList.Add(position);

		}

		for(int i=0; i<p.Length - gridPlayer.Path.Count; i++){

			Vector3 position = new Vector3(UnityEngine.Random.Range(-volume.x,volume.x),0.0f,UnityEngine.Random.Range(-volume.z,volume.z));
			tempList.Add(position);
			//p[i]= new Vector3(Random.Range(-volume.x,volume.x),0.0f,Random.Range(-volume.z,volume.z));
		}

		p = tempList.ToArray();

*/
		if(fillWithPoints)
		{
			//cellNumber = yardPoints.Count;
			
			for(int i = 0; i < yardPoints.Count; i++)
			{					
				tempList.Add(yardPoints[i]);
			}

			p = tempList.ToArray();
		}
	}

	/// <summary>
	/// Generates random cells, sorted by x value.
	/// </summary>
	/// <param name="points">Points.</param>
	//Note about sorting: using a sorted list requires the x values to always be different
	private void GenSortedRandCells(ref Vector3[] points){
		SortedList<float, Vector3> p= new SortedList<float,Vector3>();



		//for (int i = 0; i <gridPlayer.Path.Count;i++)
		//{

		if (addRoad){

			//adds the roadlist to the mesh generator
			for (int i = 0; i <roadList.Count;i++)
			{
				//Vector3 position = gridPlayer.Path[i] - transform.position;
				Vector3 position = roadList[i];//has already been adjusted for transform position
				position.y = 0f;

				try{
				p.Add(position.x,position);

				}
				catch(System.ArgumentException){
					//Debug.Log("sort conflict");

					float newX = position.x + UnityEngine.Random.Range(-0.1f,0.1f);
					while (newX == position.x){
						newX = position.x + UnityEngine.Random.Range(-0.1f,0.1f);
						Debug.Log("in while loop for Voronoi points");
					}
					position = new Vector3(newX,0f,position.z);


					p.Add(position.x,position);

				}						
			}
		}

		//adds random values for the rest
		if(fillWithRandom)
			{
			for(int i=0; i<cellNumber - roadList.Count; i++){
				Vector3 v = new Vector3(UnityEngine.Random.Range(-volume.x,volume.x),0.0f,UnityEngine.Random.Range(-volume.z,volume.z));
				try{
					p.Add(v.x, v);

				
				}
				catch(System.ArgumentException){
					i--;
					//Debug.Log("sort conflict");
				}
			}
			p.Values.CopyTo(points,0);
		}

		if(fillWithPoints)
		{
			//cellNumber = yardPoints.Count;

			for(int i = 0; i < yardPoints.Count; i++)
			{
				try{
					p.Add(yardPoints[i].x,yardPoints[i]);
				}
				catch(System.ArgumentException)
				{

					Array.Resize(ref points,points.Length-1);
					cellNumber-=1;
				}
			}
			p.Values.CopyTo(points,0);
		}
	}
	/// <summary>
	/// Generates the mesh.
	/// </summary>
	IEnumerator GenerateMesh()
    {
    //    Debug.Log("prepare cells for mesh start");
        dualGraph.PrepareCellsForMesh();
        //yield return new WaitForEndOfFrame();
     //   Debug.Log("prepare cells for mesh end");
		if (graphMesh==null){
			graphMesh= new Mesh();
			graphMesh.name= "Graph Mesh";
		}
		else{
			//For the love of god, why are you calling this twice?!?!
			graphMesh.Clear();
		}

	//	List<Vector3> vert= new List<Vector3>();
	//	List<Vector2> uvs= new List<Vector2>();
	//	List<int> tris= new List<int>();
	//	int vertCount=0;

	//	foreach(Cell c in dualGraph.cells)
     //   {
        for(int i = 0; i < dualGraph.cells.Count; i++)
        {
            //bottleneck protection
           // if(i!=0 && i % 100 == 0)
            //    yield return new WaitForEndOfFrame();


            List<Vector3> vert= new List<Vector3>();
			List<Vector2> uvs= new List<Vector2>();
			List<int> tris= new List<int>();
			int vertCount=0;
			if(!dualGraph.cells[i].root && !dualGraph.cells[i].IsOpenEdge()){						//use only interior until faux edges are added
				if(dualGraph.cells[i].IsOpenEdge()){
					Debug.Log("open edge");
				}

				foreach(Vector3 v in dualGraph.cells[i].mesh.verts){
                    //Debug.Log("in verts");
					vert.Add(v);
				}
				foreach(Vector2 v in dualGraph.cells[i].mesh.uv){
					uvs.Add(v);
                   // Debug.Log("in uv");
                }

				for(int j = 2; j < dualGraph.cells[i].mesh.verts.Length; j++){
					tris.Add(vertCount);
					tris.Add(vertCount + j - 1);
					tris.Add(vertCount + j);
				}

				//finishing the loop
				tris.Add(vertCount);
				tris.Add(vertCount+ dualGraph.cells[i].mesh.verts.Length-1);
				tris.Add(vertCount+1);

				vertCount=vert.Count;
			}
			//Check for empty meshes and skip
			if (vert.Count == 0) continue;

			///Export to individual GameObject
		//	GameObject cell = new GameObject();

            
            //add mesh info to lists to crate mesh in a coroutine and drip feed in to unity
            //Mesh mesh = new Mesh();
		//	mesh.vertices = vert.ToArray();
        //    mesh.triangles = tris.ToArray();
          
            meshVerts.Add(vert.ToArray());
            meshTris.Add(tris.ToArray());       
    
            
		}

        StartCoroutine("AddToCells");

        yield break;
        
    }

	void OnDrawGizmos(){
		if(dualGraph!=null){
			if(drawCells){
				foreach(Cell c in dualGraph.cells){
					if(c.root){
						Gizmos.color=Color.red;
					}
					else{
						Gizmos.color= Color.blue;
					}
					Gizmos.DrawCube(c.point,Vector3.one);
				}
			}
			
			if (drawDeluany){
				foreach(Cell c in dualGraph.cells){
					foreach(VoronoiEdge e in c.edges){
						if (e.cellPair.root || c.root){
							if (drawRoots){
								Gizmos.color= Color.gray;
								Gizmos.DrawLine(c.point, e.cellPair.point);
							}
						}
						else{
							Gizmos.color= Color.green;
							Gizmos.DrawLine(c.point, e.cellPair.point);
						}
					}
				}
			}
			if (drawVoronoi){
				foreach(Cell c in dualGraph.cells){
					foreach (VoronoiEdge e in c.edges){
						if(e.isConnected){
							Gizmos.color=Color.black;
							if(e.ghostStatus==Ghosting.none){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
							else if(drawGhostEdges){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
							else if(drawPartialGhostEdge&& e.ghostStatus== Ghosting.partial){
								Gizmos.DrawLine(e.Sphere.Circumcenter, e.SpherePair.Circumcenter);
							}
						}
					}
				}
			}
			if (drawCircumspheres){
				Gizmos.color= sphereColor;
				foreach(Circumcircle c in dualGraph.spheres){
					Gizmos.DrawSphere(c.Circumcenter, c.circumradius);
				}
			}
			
		}
	}
}
