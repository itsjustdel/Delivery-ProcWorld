using UnityEngine;
using System.Collections.Generic;
using System.Collections;
//using System.Linq;
public class PolygonTester : MonoBehaviour {

    //checks to see what this polygon needs to do, if it hits a road, add verge scripts
    //if its ina  junction...

	public Mesh originalMesh;
    public bool hitRoad = false;
    public bool inJunction;
//	public GameObject bush;

	void Awake()
	{
	//	enabled = false;
	}

	void Start()
	{
	//TODO relabel this stuff

		//save the original mesh so the Place Bushes script etc, have the original edges to palce around,
		//after the cell gets subdivided, we lose the simple vertice setup which makes it easy to instantiate around the edges



		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		originalMesh = mesh;

        //CheckCellIsFacingUpwards(mesh); //not working

		StartCoroutine("RaycastCheck"); //this checks for a raod with thepolygona nd fires thecell Y Adjust script up

	}

    void CheckCellIsFacingUpwards(Mesh mesh)
    {

        Vector3 position = mesh.vertices[0] + Vector3.up;
        RaycastHit hit;
        if(!Physics.Raycast(position,Vector3.down,out hit,2f,LayerMask.GetMask("TerrainCell")))
        {
            //cell is upside down
            //flip triangles
            List<int> tris = new List<int>();

            foreach (int i in mesh.triangles)
                tris.Add(i);

            
            tris.Reverse();

            mesh.triangles = tris.ToArray();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.name = "UpwardsCell";

        }
    }


    IEnumerator RaycastCheck()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;

        //copying this to a seperate cuts down on Garbage Collection//optimisation
        Vector3[] tempVertices = mesh.vertices;

        /*
            //Removed for now as this stop fields stopping at junctions*******
        //check for junction
        RaycastHit hitJunction;
        
        if(Physics.Raycast(tempVertices[0] + (Vector3.up *1000),Vector3.down,out hitJunction, 2000f,LayerMask.GetMask("Junction"),QueryTriggerInteraction.Collide))
        {
            //Debug.Log("Junction Hit");
            inJunction = true;

        //    yield break;
        }
        
        */
   //     List<Vector3> list = new List<Vector3>();
        //start from 1 to miss out first vertice, the first vertice is alwayes the centre of the polygon
        for (int i = 0; i < mesh.vertexCount - 1; i++)
        {
            float distance = 0f;
            if (i != 0)
            {
                distance = Vector3.Distance(tempVertices[i], tempVertices[i + 1]);
            }
            else if (i == 0)
            {
                distance = Vector3.Distance(tempVertices[i + 1], tempVertices[mesh.vertexCount - 1]);
            }
            //Debug.Log(distance);
            
           // List<Vector3> tempList = new List<Vector3>();
            for (float j = 0; j < distance; j++)
            {

                if (i == 0)
                {
                    Vector3 dir = tempVertices[i + 1] - tempVertices[mesh.vertexCount - 1];
                    dir.Normalize();
                    Vector3 pos = tempVertices[mesh.vertexCount - 1] + (dir * j);

                    //check for raycast
                    RaycastHit hit;
                    LayerMask lM = LayerMask.GetMask("Road");


                    //if it doesnt hit a road, add
                    if (!Physics.Raycast(pos + Vector3.up * 1000, Vector3.down, out hit, 2000f, lM))
                    {
                      //  tempList.Add(pos);
                    }
                    else
                    {
                      //  tempList.Clear();
                        hitRoad = true;
                    }

                }

                else if (i != 0)
                {

                    Vector3 dir = tempVertices[i + 1] - tempVertices[i];
                    dir.Normalize();
                    Vector3 pos = tempVertices[i] + (dir * j);


                    //check for raycast
                    RaycastHit hit;
                    LayerMask lM = LayerMask.GetMask("Road");


                    //if it doesnt hit a road, add
                    if (!Physics.Raycast(pos + Vector3.up * 1000, Vector3.down, out hit, 2000f, lM))
                    {
                       // tempList.Add(pos);
                        //	continue;
                    }
                    //	if (Physics.Raycast(pos + Vector3.up*1000,Vector3.down, out hit,2000f,lM))
                    else
                    {
                     //   tempList.Clear();
                        hitRoad = true;


                    }
                }

                if (!hitRoad)
                {
                    //Clear the list and start on new edge
                 //   for (int l = 0; l < tempList.Count; l++)
                    //    list.Add(tempList[l]);
                }

                //yield return new WaitForFixedUpdate();

            }
        }

        if(hitRoad && inJunction)
        {
        //    Destroy(gameObject.GetComponent<CellYAdjust>());
        //    SubdivideMesh sdm = gameObject.AddComponent<SubdivideMesh>();
        //    sdm.enabled = true;

            //set material 

        //    GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Path", typeof(Material)) as Material;
            
            //add to build list
            //  GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(gameObject);


        }
        //
        if (hitRoad)
        {
            //   GetComponent<MeshRenderer>().enabled = false;
            //   GetComponent<MeshCollider>().enabled = false;
            GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Green") as Material;
            gameObject.AddComponent<AddToHitRoadCell>();
        }

        yield break;
    }
        

        public void EnableScripts()
        { 
			//called from activateObjects when van trigger hits cell
			if(hitRoad && inJunction)
			{
				//enable subdivision script 
				SubdivideMesh sdm = gameObject.GetComponent<SubdivideMesh>();
				sdm.enabled = true;
				Destroy(gameObject.GetComponent<CellYAdjust>());
            //GameObject.Find("VoronoiMesh").GetComponent<BuildController>().gOList.Add(this.gameObject);

            Debug.Log("call to enable scripts");

			}
            else if(!hitRoad)
            {
            Debug.Log("eh");
            }
			//tempList.Clear();

		}
    
	}

