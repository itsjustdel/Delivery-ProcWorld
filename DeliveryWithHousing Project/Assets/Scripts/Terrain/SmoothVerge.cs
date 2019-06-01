using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SmoothVerge : MonoBehaviour
{
    private BezierSpline spline;
    private int closestInt;
    private float stepSize;
    // Use this for initialization
    void Start()
    {

        CreateMesh();
        DisableParentCell();
       // Join();
    }


    void CreateMesh()
    {

        Vector3 centralPoint = gameObject.GetComponent<MeshFilter>().mesh.vertices[0];


        //find point on road curve closest to central point

        RaycastHit hit;
        LayerMask lM = LayerMask.GetMask("Road");
        GameObject road = null;
        //raycast for road
        if (!Physics.Raycast(centralPoint + Vector3.up*100, Vector3.down, out hit, 200f, lM))
        {

            StartCoroutine("BuildList");
            Debug.Log("smooth verge missed");

            return;
        }


        road = hit.collider.gameObject;



        if (road == null)
        {
            Debug.Log("road null");
        }
        //get curve from road gameObject

        spline = road.transform.parent.transform.GetComponent<RingRoadMesh>().updatedSpline;

        if (spline == null)
            Debug.Log("null");

        //find point nearest central point on spline  

        float distance = Mathf.Infinity;
        Vector3 closestV3 = Vector3.zero;
        //closestInt = 0;
        float frequency = road.transform.parent.transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount;

        stepSize = 1f / frequency;
        for (int i = 0; i < frequency; i++)
        {

            Vector3 point = spline.GetPoint(i * stepSize);

            float temp = Vector3.Distance(centralPoint, point);
            if (temp < distance)
            {
                distance = temp;
                closestV3 = point;
                closestInt = i;
            }
        }

        int ySize = 0;

        List<Vector3> points = new List<Vector3>();

        int width = 3;
        //from the closest point on the curve, move back and run through a small section
        for (float i = closestInt - 5; i < closestInt + 5; i += 0.5f)
        {

            Vector3 point = spline.GetPoint(i * stepSize);

            Vector3 direction = spline.GetDirection(i * stepSize);
            //direction.Normalize();

            Vector3 rot1 = Quaternion.Euler(0f, 90f, 0f) * direction;

            Vector3 offToTheSide = point - (rot1 * width);

            List<Vector3> temp = new List<Vector3>();

            for (float j = 0; j <= width * 2; j += 0.5f)
            {
                Vector3 pos = offToTheSide + (rot1 * j);

                RaycastHit[] hits = (Physics.SphereCastAll(pos + (Vector3.up * 5), 0.1f, Vector3.down, 100f, LayerMask.GetMask("TerrainCell")));

                foreach (RaycastHit hit2 in hits)
                {
                    if (hit2.collider.transform == transform)
                    {

                        if (!points.Contains(pos))
                            temp.Add(pos);
                    }

                }
            }

            //if list has not been cleared by a missed raycast, enter in to main points list//doesnt happen like this anymore
            foreach (Vector3 v3 in temp)
            {
                points.Add(v3);

           //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
           //     cube.transform.position = v3;
           //     cube.transform.localScale *= 0.1f;
           //     cube.GetComponent<BoxCollider>().enabled = false;
            }
        }


        int acrossRoad = width * 2 * 2; //amount of hits - 1
        int alongRoad = 10; //amount of hits - 1


        int xSize = acrossRoad;
        ySize = alongRoad;

        int[] triangles = new int[(xSize * ySize) * 6];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        StartCoroutine("BuildList");

        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        //mesh.normals = normals;
        mesh.triangles = triangles;


        mesh.RecalculateNormals();
        ;

        GameObject verge = new GameObject();
        verge.transform.parent = this.gameObject.transform;
        verge.name = "Join";

       

        verge.layer = 21;
        MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
        meshFilterInstance.mesh = mesh;
        MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
        // meshRenderer.enabled = false;
        meshRenderer.sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
         MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();
        meshCollider.enabled = false;
        //  meshCollider.sharedMesh = mesh;

        //add cell Y adjust

        verge.AddComponent<CellYAdjust>();


        return;

        //below is added when fields are created

        Vector3 pointForField = spline.GetPoint(closestInt * stepSize);


        Vector3 directionForField = spline.GetDirection(closestInt * stepSize);
        directionForField.Normalize();

        Vector3 rot1ForField = Quaternion.Euler(0f, 90f, 0f) * directionForField;
        Vector3 rot2ForField = Quaternion.Euler(0f, -90f, 0f) * directionForField;

        rot1ForField *= 4;
        rot2ForField *= 4;

        Vector3 pos1 = pointForField + rot1ForField;
        Vector3 pos2 = pointForField + rot2ForField;

       // RaycastHit hit;
        if (Physics.Raycast(pos1 + (Vector3.up * 100), Vector3.down, out hit, 200f, LayerMask.GetMask("TerrainCell")))
        {
            JoinCellAndVerge jcav = hit.collider.gameObject.AddComponent<JoinCellAndVerge>();
            jcav.enabled = true;
            //add to build list
      //      GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(hit.collider.gameObject);

      //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       //     cube.transform.position = hit.point;
        }

        if (Physics.Raycast(pos2 + (Vector3.up * 100), Vector3.down, out hit, 200f, LayerMask.GetMask("TerrainCell")))
        {

            JoinCellAndVerge jcav = hit.collider.gameObject.AddComponent<JoinCellAndVerge>();
            jcav.enabled = true;

            //add to build list
        //    GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(hit.collider.gameObject);


        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = hit.point;
        }

       

        
        //raycast for Field
        //Debug.Log(closestInt);
        

        //start coroutine to tell buildlist to continue
       
    }

    void DisableParentCell()
    {
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    public void Join()
    {
        Vector3 pointForField = spline.GetPoint(closestInt * stepSize);


        Vector3 directionForField = spline.GetDirection(closestInt * stepSize);
        directionForField.Normalize();

        Vector3 rot1ForField = Quaternion.Euler(0f, 90f, 0f) * directionForField;
        Vector3 rot2ForField = Quaternion.Euler(0f, -90f, 0f) * directionForField;

        rot1ForField *= 4;
        rot2ForField *= 4;

        Vector3 pos1 = pointForField + rot1ForField;
        Vector3 pos2 = pointForField + rot2ForField;

        RaycastHit hit;
        if (Physics.Raycast(pos1 + (Vector3.up * 100), Vector3.down, out hit, 200f, LayerMask.GetMask("TerrainCell")))
        {
            JoinCellAndVerge jcav =  hit.collider.gameObject.AddComponent<JoinCellAndVerge>();
            jcav.enabled = true;
            //add to build list
      //      GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(hit.collider.gameObject);

                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
               cube.transform.position = hit.point;
        }

        if (Physics.Raycast(pos2 + (Vector3.up * 100), Vector3.down, out hit, 200f, LayerMask.GetMask("TerrainCell")))
        {

            JoinCellAndVerge jcav = hit.collider.gameObject.AddComponent<JoinCellAndVerge>();
            jcav.enabled = true;

            //add to build list
            GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(hit.collider.gameObject);


            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
             cube.transform.position = hit.point;
        }
    }
    IEnumerator BuildList()
    {
        //yield return new WaitForEndOfFrame();
        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
        //add this game object again to the list so it can go back round andd the joining verge
        GameObject.FindWithTag("Code").GetComponent<BuildList>().objects.Add(gameObject);
        yield break;
    }
}
