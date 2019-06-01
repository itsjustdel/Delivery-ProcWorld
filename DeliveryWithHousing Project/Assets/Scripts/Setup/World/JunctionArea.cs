using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JunctionArea : MonoBehaviour {

    //splits junction up in to groups around road

    public Vector3 start;
    public Vector3 end;
 //   public List<Vector3> points = new List<Vector3>();
    public List<GameObject> cells = new List<GameObject>();
    public List<GameObject> junctionPieces = new List<GameObject>();
    public Material junctionMaterial;
    void Awake()
    {
        enabled = false;
    }

    // Use this for initialization
    void Start ()
    {        

        junctionMaterial = Resources.Load("Path", typeof(Material)) as Material;

        start = GetComponent<HousesForVoronoi>().start;
        end = GetComponent<HousesForVoronoi>().end;
       
        cells = OutlyingCells(start);
        GroupCellsOnEachCornerOfRoad();

        cells.Clear();

        cells = OutlyingCells(end);
        GroupCellsOnEachCornerOfRoad();

        CombineCells();
        StartCoroutine("AutoWeldCells");
    }

    List<Vector3> MakeList()
    {
        List<Vector3> list = new List<Vector3>();
        

        list.Add(start);
        list.Add(end);

        return list;
    }

    List<GameObject> OutlyingCells(Vector3 position)
    {
        List<GameObject> list = new List<GameObject>();

        //checks vertices in all cells surrounding the junction. If too close or overlapping the junction mesh, ove the vertices back away to 
        //leave space for the joining stitch script

        //raycast from centre of junction in a sphere, gathering a list of cells to work on

        //for each junction //start and end
       // for (int j = 0; j < points.Count; j++)
       // {           

            //Vector3 position = points[j];


        //move round in a circle
        for (int i = 0; i < 360; i += 5)
        {
            //probe outwards to gather outside cells too
          
               
                Vector3 point = position - (Vector3.left * (15));//radius is 15 on junction trigger
                Vector3 pivot = position;
                Vector3 dir = point - pivot;
                dir = Quaternion.Euler(0f, i, 0f) * dir;
            

                Vector3 anchor = pivot + (dir);

                //raycast looking for terrain cell layer. Add hit cell to list
                RaycastHit hit;
                if (Physics.SphereCast(anchor + (Vector3.up * 10),5f, Vector3.down, out hit, 20f, LayerMask.GetMask("TerrainCell")))
                {

                //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube.transform.position = hit.point;
                    //add to list if hit
                    if (!list.Contains(hit.collider.gameObject) && hit.transform.parent.name != "WorldPlan")
                    {
                        list.Add(hit.collider.gameObject);

                        //disable this cell's collider. We will add a collider to the larger cell this one is part of
                        //don't do if overlapping road
                        if (!hit.collider.gameObject.GetComponent<PolygonTester>().hitRoad)
                            hit.collider.gameObject.GetComponent<MeshCollider>().enabled = false;

                        //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //   cube.transform.position = hit.point;
                    }
                }
            
              //  else
               //     Debug.Log("Did not hit - junction outlying cells");
        //    }
        }
        return list;
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

                   //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                   //    cube.transform.position = cells[i].GetComponent<MeshFilter>().mesh.vertices[0];
                    //   cube.name = "start";

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
        bool clearList = false;
        //start from the cell after the one which hit the road
        for (int i = start +1; i < cells.Count; i++)
        {
            if (clearList)
            {
                corner.Clear();
                clearList = false;
            }
            if (cells[i].GetComponent<PolygonTester>().hitRoad == false)
            {
                corner.Add(cells[i].gameObject);
                cells[i].gameObject.tag = "HouseCell";
               
            }

            if (cells[i].GetComponent<PolygonTester>().hitRoad == true)
            {

                //this cell wont have a subdivide on it because it is in the junction area, so add one now
            //    cells[i].AddComponent<SubdivideMesh>();

                GameObject junction = new GameObject();
                junction.name = "House On Corner";
                junction.tag = "HouseCell";
                junction.transform.parent = this.transform;
                junctionPieces.Add(junction);

                //add class to hold any onformation we need about this corner on the junction
                junction.AddComponent<HouseCellInfo>();

                //child found cells to a gameobject
                foreach (GameObject go in corner)
                {
                //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube.transform.position = go.GetComponent<MeshFilter>().mesh.vertices[0];
                //    cube.name = i.ToString();                    

                    go.transform.parent = junction.transform;
                    go.GetComponent<MeshRenderer>().sharedMaterial = junctionMaterial;
                }
                //we can clear the corner list and continue on our search //do on next iteration, if we clear() here, it will add an empty list - go figure.(even though add is called before clear)
                clearList = true;
                
            }
        }

        //now do the first section of cells we missed by starting half way through
        
        //go up until the cell that found the start- this will be hitRoad = true, thus forcing the list to be added
        for (int i = 0; i <= start; i++)
        {
            if (clearList)
            {
                corner.Clear();
                clearList = false;
            }

            if (cells[i].GetComponent<PolygonTester>().hitRoad == false)
            {
                corner.Add(cells[i].gameObject);
                cells[i].gameObject.tag = "HouseCell";
            }

            if (cells[i].GetComponent<PolygonTester>().hitRoad == true)
            {
                //this cell wont have a subdivide on it because it is in the junction area, so add one now
              //  cells[i].AddComponent<SubdivideMesh>();

                GameObject junction = new GameObject();
                junction.name = "House On Corner";
                junction.tag = "HouseCell";
                junction.transform.parent = this.transform;
                junctionPieces.Add(junction);
                //add class to hold any onformation we need about this corner on the junction
                junction.AddComponent<HouseCellInfo>();

                //child found cells to a gameobject
                foreach (GameObject go in corner)
                {
                 //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                 //   cube.transform.position = go.GetComponent<MeshFilter>().mesh.vertices[0];
                 //   cube.name = i.ToString();

                    go.transform.parent = junction.transform;
                    go.GetComponent<MeshRenderer>().sharedMaterial = junctionMaterial;
                }
                //we can clear the corner list and continue on our search //do on next iteration, if we clear() here, it will add an empty list - go figure.(even though add is called before clear)
                clearList = true;
            }
        }    
   
    }

    void CombineCells()
    {   
        //run through junction pieces list and add combine script

        for(int i = 0; i < junctionPieces.Count; i++)
        {
            CombineMeshes(junctionPieces[i]);
        }
    }
    IEnumerator AutoWeldCells()
    {
        //wait for unity to instantiate combined mesh form above
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < junctionPieces.Count; i++)
        {
         //   AutoWeld(junctionPieces[i].transform.FindChild("Combined mesh").gameObject, 0.01f);
         //   junctionPieces[i].transform.FindChild("Combined mesh").gameObject.layer = 23;
            //junctionPieces[i].transform.FindChild("Combined mesh").name = "Junction Corner";
          //  junctionPieces[i].transform.FindChild("Combined mesh").tag = "JunctionCell";
        }

        yield return new WaitForEndOfFrame();        

        StartCoroutine("EnableJunctionInfos");

        yield break;
    }
    
    IEnumerator EnableJunctionInfos()
    {
        for (int i = 0; i < junctionPieces.Count; i++)
        {

            junctionPieces[i].GetComponent<HouseCellInfo>().enabled = true;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine("EnableStretchQuads");
    }

    IEnumerator EnableStretchQuads()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < junctionPieces.Count; i++)
        {
          //  junctionPieces[i].transform.FindChild("ProcHouse(Clone)").FindChild("Meshes").GetComponent<StretchQuads>().enabled = true;
            yield return new WaitForEndOfFrame();
        }
            yield break;
    }

    void CombineMeshes(GameObject go)
    {
        CombineChildren cc = go.AddComponent<CombineChildren>();

        cc.reAlignCell = true;
        cc.addMeshCollider = true;
        cc.addAutoWeld = true;
        cc.houseCell = true;
        
    }

    void AutoWeld(GameObject go, float threshold)
    {
        //add auto weld to combinedmesh gameobject
        AutoWeld autoWeld = go.AddComponent<AutoWeld>();
        autoWeld.threshold = threshold;
    }

}
