using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionCells : MonoBehaviour {

    /// <summary>
    /// Receives Vector3's from ringroadlist class and attaches necessary scripts to build the mesh around roads
    /// </summary>
    /// 

    /*
    junction cell.
    subdivide
    check for smooth verge - child them
    combine children
    autoweld at 1/0.5
    subdivide
    viola

    */
    //list populated from RingRoadList line.536
    public List<Vector3> points = new List<Vector3>();

    public GameObject terrainCell;
    public GameObject copy;
    public bool working = false;
    public List<GameObject> cells = new List<GameObject>();
    public List<List<GameObject>> corners = new List<List<GameObject>>();
    public bool addCells;
    public bool moveVertices;
    // Use this for initialization
    void Start ()
    {
        //populates Cells list with cells around junction
        AddOutlyingCells();

        GroupCellsOnEachCornerOfRoad();
     //   CombineCorners();
    }
    void AddOutlyingCells()
    {
        //checks vertices in all cells surrounding the junction. If too close or overlapping the junction mesh, ove the vertices back away to 
        //leave space for the joining stitch script



        //raycast from centre of junction in a sphere, gathering a list of cells to work on

        //for each junction
        for (int j = 0; j < points.Count; j++)
        {
            Vector3 position = points[j];

            for (int i = 0; i < 360; i += 10)
            {
                Vector3 point = position - (Vector3.left * 15);//radius is 15 on junction trigger
                Vector3 pivot = position;
                Vector3 dir = point - pivot;
                dir = Quaternion.Euler(0f, i, 0f) * dir;


                Vector3 anchor = pivot + dir;

                //raycast looking for terrain cell layer. Add hit cell to list
                RaycastHit hit;
                if (Physics.Raycast(anchor + (Vector3.up * 10), Vector3.down, out hit, 20f, LayerMask.GetMask("TerrainCell")))
                {
                    //add to list if hit
                    if (!cells.Contains(hit.collider.gameObject))
                    { 
                        cells.Add(hit.collider.gameObject);
                     //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                     //   cube.transform.position = hit.point;
                    }
                }
                else
                    Debug.Log("Did not hit - junction outlying cells");
            }
        }
    }
    void GroupCellsOnEachCornerOfRoad()
    {
        //divides the T Junction in to individual Chunks

        int start = 0;
        //look for start cell. The cell after one which has hit a road
        for (int i = 0; i < cells.Count;)
        {
            //assumes there are cells that have hit the road
            if (cells[i].GetComponent<PolygonTester>().hitRoad == true)
            {
                //great. We have the start
                start = i;

         //       GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //       cube.transform.position = cells[i].GetComponent<MeshFilter>().mesh.vertices[0];
         //       cube.name = i.ToString();

                break;
            }
            else if (cells[i].GetComponent<PolygonTester>().hitRoad == false)
            {
                //keep looking for start
                i++;
            }
        }
        
        //the cells list to add to above list // declared up top - corners
        List<GameObject> corner = new List<GameObject>();

        //start from the cell after the one which hit the road
        for(int i = 0; i < cells.Count; i++)
        {
            if(cells[i].GetComponent<PolygonTester>().hitRoad == false)
            {
                corner.Add(cells[i].gameObject);
                cells[i].gameObject.tag = "JunctionCell";
            }

            if (cells[i].GetComponent<PolygonTester>().hitRoad == true)
            {
                //we have found the end of our corner, add the list to the list of lists ;)
                corners.Add(corner);
                

                //we can clear the corner list and continue on our search 
                corner.Clear();
            }
        }

        //now do the first section of cells we missed by starting half way through
        /*
        //go up until the cell that found the start- this will be hitRoad = true, thus forcing the list to be added
        for(int i = 0; i <= start; i++)
        {
            if (cells[i].GetComponent<PolygonTester>().hitRoad == false)
            {
                corner.Add(cells[i].gameObject);
                cells[i].gameObject.tag = "JunctionCell";
            }

            if (cells[i].GetComponent<PolygonTester>().hitRoad == true)
            {
                //we have found the end of our corner, add the list to the list of lists ;)
                corners.Add(corner);

                //we can clear the corner list and continue on our search 
              //  corner.Clear();
            }
        }
        */
    }
    void CombineCorners()
    {
        Debug.Log(corners.Count);
        //each list
        for (int i = 0; i < corners.Count; i++)
        {
            List<GameObject> corner = corners[i];
            //each gameobject in list
           // Debug.Log(corners[i].Count);
            Debug.Log(corner.Count);
            for (int j = 0; j < corner.Count; j++)
            {
              //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
              //  cube.transform.position = corner[j].GetComponent<MeshFilter>().mesh.vertices[0];
              //  cube.name = i.ToString();
            }
        }
    }

    //below are used to smoosh the junction together. Not using this
    //loop coroutine used to gather all junction cells, subdivide and autowel it all together -- never got it to work. Too many complexities for it to work everytime smoothly
    //StartCoroutine("Loop"); 
    IEnumerator Loop()
    {

        for (int i = 0; i < points.Count; i++)
        {
            //find cell at junction
            terrainCell = Raycast(points[i]);

            //look for smooth verges and child
            FindSmoothVerges();

            //make materials the same for all meshes
            MaterialMatch();

            //add subdivide
            Subdivide(terrainCell,16);
            working = true;          

            //wait for subdivide to complete // Once subdivide is finished, it flags this finsihed e.g working = false;
            while (working)
                yield return new WaitForEndOfFrame();

            //add auto weld
            AutoWeld(terrainCell,0.05f);

            //add combine meshes
            CombineMeshes();

            //wait for unity to update
            yield return new WaitForEndOfFrame();

            //add an autoweld to the combined emsh to mooth out any overlappin edges
            GameObject combined = terrainCell.transform.Find("Combined mesh").gameObject;
            AutoWeld(combined, 0.05f);

           

        }

        yield return new WaitForEndOfFrame();

        AddOutlyingCells();

        MoveVertices();

        yield break;
    }	
    GameObject Raycast(Vector3 position)
    {
        //look only for terrain Cell
        GameObject terrainCell = null;
        LayerMask lm = LayerMask.GetMask("TerrainCell");
        RaycastHit hit;
        if (Physics.Raycast(position + (Vector3.up * 10), Vector3.down, out hit,20f, lm))
        {
            terrainCell = hit.collider.gameObject;
        }
        else
            Debug.Log("JunctionCell failed to find cell");

        return terrainCell;
    }

    void MoveVertices()
    {
        //for every vertices in the mesh in the cells list, spherecast looking for a verge

        for(int i = 0; i < cells.Count; i++)
        {
            Mesh mesh = cells[i].GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;

            bool updateMesh = false;
            

            for (int j = 0; j < vertices.Length; j++)
            {
                RaycastHit[] hits = Physics.SphereCastAll(vertices[j] + (Vector3.up * 10), 0.1f, Vector3.down*20);

                foreach (RaycastHit hit in hits)
                {
                    //only looking for the road skirt
                    if (hit.collider.gameObject.layer == 10)
                    {
                        //we are too close to the junction, move this vertice back 1 unit (a normalised vector)
                        Vector3 toCentre = (vertices[0] - vertices[j]).normalized;
                        Vector3 newPos = vertices[j] + toCentre;

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = vertices[j];
                        cube.name = "OLD POS";

                        vertices[j] = newPos;

                        updateMesh = true;
                        
                    }
                }
            }

            if (updateMesh)
            {
                //asign temp vertices back to the mesh
                mesh.vertices = vertices;
                cells[i].GetComponent<MeshCollider>().enabled = false;
                cells[i].GetComponent<MeshCollider>().sharedMesh = mesh;
                cells[i].GetComponent<MeshCollider>().enabled = true;
            }

        }
        //if found, move this vertice back towards the centre of its cell
    }
    void CreateCopy()
    {
        //create copy of cell and child it to itself
        copy = Instantiate(terrainCell);
        copy.transform.parent = terrainCell.transform;
        copy.name = "Copy";
    }
    void FindSmoothVerges()
    {
        //spherecast from middle point of mesh
        Vector3 centre = terrainCell.GetComponent<MeshFilter>().mesh.vertices[0];

        RaycastHit[] hits = Physics.SphereCastAll(centre + (Vector3.up * 20),15f,Vector3.down,40f);
        
        foreach(RaycastHit hit in hits)
        {
            //filter out hits that are not on the smooth verge layer - Can't get spherecastAll layermask working
            if (hit.collider.gameObject.layer != 21)
                continue;

            //for every smoothe verge hit, make the terrain/junction cell we are casting from their parent
            hit.collider.transform.parent = terrainCell.transform;

        }

    }
    void MaterialMatch()
    {
        //for all children of terrainCell, make material match so combine meshes script joins them properly
        MeshRenderer[] meshRenderers = terrainCell.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in meshRenderers)
        {
            mr.sharedMaterial = terrainCell.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
    void Subdivide(GameObject terrainCell,int amount)
    {
        SubdivideMesh sm = terrainCell.AddComponent<SubdivideMesh>();
        sm.amount = amount;
    }
    void CombineMeshes()
    {
        CombineChildren cc = terrainCell.AddComponent<CombineChildren>();
        cc.addMeshCollider = true;
    }
    void AutoWeld(GameObject go, float threshold)
    {
        //add auto weld to combinedmesh gameobject
        AutoWeld autoWeld = go.AddComponent<AutoWeld>();
        autoWeld.threshold = threshold;
    }
}
