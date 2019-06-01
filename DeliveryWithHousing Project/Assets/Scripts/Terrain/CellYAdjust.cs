using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellYAdjust : MonoBehaviour {


	public float rayYAmount = 1000f;
	private List<Vector3> templist = new List<Vector3>();
	private MeshFilter meshfilter;
	private Mesh mesh;
	//these booleans allow this script to be used in slightly different occasions
//	public bool saveOriginalMesh;
//	public bool activatePolygonTester;
    public bool dontCheckForRoad = false;
    public bool reportToSubdivideOnFinish = false;

    public bool addAutoWeld = false;
	void Awake()
	{

	}
	void Start () {	

		meshfilter = GetComponent<MeshFilter>();
		mesh = meshfilter.mesh;
		foreach ( Vector3 v3 in mesh.vertices)
			templist.Add(v3);

        //		if(saveOriginalMesh)
        //		{
        //			//Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        //			gameObject.GetComponent<PolygonTester>().originalMesh = mesh;
        //		}

        if (!dontCheckForRoad)
            StartCoroutine("AdjustCellHeightForRoad");
        else
            StartCoroutine("AdjustHeightForField");

	}

    IEnumerator AdjustCellHeightForRoad()
    {


        List<Vector3> tempList = new List<Vector3>();

        RaycastHit hit;

        LayerMask myLayerMask = LayerMask.GetMask("Road");
        LayerMask myLayerMask2 = LayerMask.GetMask("RoadSkirt");
        LayerMask myLayerMask3 = LayerMask.GetMask("TerrainBase");


        ///create a temporary list of themesh vertice positions
        /// if we dont do this Garbage Collection goes through the roof as c# and unity
        /// create a copy of the mesh for each i in mesh.vertexCount
        /// see http://gamasutra.com/blogs/RobertZubek/20150504/242572/C_memory_and_performance_tips_for_Unity.php#comment267206
        /// 
        /// The performance limit is now on themesh collider being injected in to the engine. (PhysX) 
        List<Vector3> templist = new List<Vector3>();
        foreach (Vector3 v3 in mesh.vertices)
            templist.Add(v3);

        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //	meshTempList.Add(mesh.vertices[i]);
        //}

        for (int i = 0; i < templist.Count; i++)
        {

            //road
            //if (Physics.SphereCast(templist[i] + transform.position + (Vector3.up* rayYAmount),0.25f, -Vector3.up, out hit,rayYAmount*2,myLayerMask))//21/12/17
            if (Physics.Raycast(templist[i] + transform.position + (Vector3.up * rayYAmount), -Vector3.up, out hit, rayYAmount * 2, myLayerMask))
            {
                //sit the vertic just under the road
                tempList.Add(hit.point - Vector3.up * 0.1f);// + Vector3.down*10);		//21/12/17
                //tempList.Add(hit.point);


            }

            //roadskirt
            else if (Physics.Raycast(templist[i] + transform.position + (Vector3.up * rayYAmount), -Vector3.up, out hit, rayYAmount * 2, myLayerMask2))
            {

                tempList.Add(hit.point);// + Vector3.down*10);

                //       GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //       cube.transform.position = hit.point;
                //       cube.transform.localScale *= 0.1f;

            }



            //terrainbase	
            else if (Physics.Raycast(templist[i] + transform.position + (Vector3.up * rayYAmount), -Vector3.up, out hit, rayYAmount * 2, myLayerMask3))
            {
                tempList.Add(hit.point);
            }

            else
            {
                tempList.Add(mesh.vertices[i]);
                //	Debug.Log("missed - will be a hole in the voronoi nesh");
            }



            /* *****Code below works but gets warped at junctions? -is possibly quicker than above code
            LayerMask all = LayerMask.GetMask("Road", "RoadSkirt", "TerrainBase");
            RaycastHit[] hits = Physics.RaycastAll(templist[i] + transform.position + (Vector3.up * rayYAmount), Vector3.down, rayYAmount * 2, all);

            bool foundRoad = false;
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].transform.tag == "Road")
                {
                    tempList.Add(hits[j].point - Vector3.up * 0.1f);
                    foundRoad = true;
                }
            }

            //if did not find a road above, look for verge
            bool foundVerge = false;
            if (!foundRoad)
            { 
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].transform.tag == "TerrainSecondLayer")
                    {
                        tempList.Add(hits[j].point);
                        foundVerge = true;
                    }
                }
             }

            //if we find neither road or verge, look for terrain base
            bool foundTerrainBase = false;
            if (!foundVerge && !foundRoad)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    //the base mesh
                    if (hits[j].transform.tag == "TerrainBase")
                    {
                        tempList.Add(hits[j].point);
                        foundTerrainBase = true;
                    }
                }
            }

            if(!foundRoad && !foundVerge && !foundTerrainBase)
                tempList.Add(mesh.vertices[i]);
            
    */

            //System.GC.Collect();

            if (i % 50 == 0)
            {

                yield return new WaitForEndOfFrame();
            }

        }

        mesh.vertices = tempList.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        mesh.name = "Y Altered Mesh";


        meshfilter.mesh = mesh;

        if (GetComponent<MeshCollider>() != null)
        { 
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.enabled = true;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
        /*	
                if(activatePolygonTester)
                {
                    PolygonTester pt = gameObject.AddComponent<PolygonTester>();
                    pt.enabled = true;
                }
        */
        //once finished, remove script from gameobject;
        //Destroy(this);

        //start a chain reaction of finished coroutine reports
        if (reportToSubdivideOnFinish)
            GetComponent<SubdivideMesh>().reportToJunctionCells();

        if (gameObject.GetComponent<AddToHitRoadCell>() != null)
            gameObject.GetComponent<AddToHitRoadCell>().working = false;

        if (addAutoWeld)
            gameObject.AddComponent<AutoWeld>();


		yield break;
	}

    IEnumerator AdjustHeightForField()
    {

        List<Vector3> tempList = new List<Vector3>();

        RaycastHit hit;
        
        LayerMask myLayerMask = LayerMask.GetMask("TerrainBase");

        List<Vector3> templist = new List<Vector3>();
        foreach (Vector3 v3 in mesh.vertices)
            templist.Add(v3);

        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //	meshTempList.Add(mesh.vertices[i]);
        //}

        for (int i = 0; i < templist.Count; i++)
        {
            if (Physics.Raycast(templist[i] + transform.position + (Vector3.up * rayYAmount), -Vector3.up, out hit, rayYAmount * 2, myLayerMask))
            {
                tempList.Add(hit.point);
            }
 
            else
            {
                tempList.Add(mesh.vertices[i]);
             //   Debug.Log("missed - will be a hole in the voronoi nesh");
            }

            //System.GC.Collect();

            if (i % 50 == 0)
            {
                yield return new WaitForEndOfFrame();
            }

        }

        mesh.vertices = tempList.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        mesh.name = "Y Altered Mesh For Field";

        meshfilter.mesh = mesh;
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

       
        //once finished, remove script from gameobject;
        Destroy(this);

        yield break;
    }

    public static Mesh AdjustYForLayer(Transform parent, Mesh mesh,string layer,float rayYAmount)
    {
        Vector3[] vertices = mesh.vertices;
        LayerMask myLayerMask = LayerMask.GetMask(layer);
        RaycastHit hit;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Physics.Raycast(vertices[i] + (Vector3.up * rayYAmount) + parent.position, -Vector3.up, out hit, rayYAmount * 2, myLayerMask))
            {
                vertices[i] = hit.point - parent.position;

           
            }

            else
            {
                //leave as is
               
            }
        }

        mesh.vertices = vertices;

        return mesh;
    } 

}
