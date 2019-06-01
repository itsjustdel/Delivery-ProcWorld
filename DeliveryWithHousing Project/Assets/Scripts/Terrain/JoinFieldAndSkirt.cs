using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class JoinFieldAndSkirt : MonoBehaviour
{



    [HideInInspector]
    public List<Vector3> tri = new List<Vector3>();
    // Use this for initialization
    void Start()
    {


        StartCoroutine("CreatePolygonList");
        // PolygonFromList();
        //TestPolygon();
    }
  

    IEnumerator CreatePolygonList()
    {
        List<EdgeHelpers.Edge> outline = GetComponent<FindEdges>().boundaryPath;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < outline.Count; i++)
        {
            csTriangulation.Triangulation meshTriangle = new csTriangulation.Triangulation();
            //add this to field object
            List<Vector3> v3List = new List<Vector3>();
            List<int> intList = new List<int>();

            Vector3 p1 = vertices[outline[i].v1];
            Vector3 p2 = vertices[outline[i].v2];


            //lerp between points
            float distance = Vector3.Distance(p1, p2);

            for (float j = 0; j <= distance; j++)
            {
                //yield return new WaitForEndOfFrame();

                Vector3 direction = (p2 - p1).normalized;
                direction *= j;

                RaycastHit hit;


                if (!Physics.SphereCast(p1 + direction + (Vector3.up * 5), 2f, Vector3.down, out hit, 10f, LayerMask.GetMask("RoadSkirt")))
                {
                    Debug.Log("didnt hit");
                    //skip this outline edge
                    i++;
                    continue;
                }
                else
                { 

                    MeshCollider meshCollider = hit.collider as MeshCollider;

                    Mesh hitMesh = meshCollider.sharedMesh;
                    Vector3[] hitVertices = hitMesh.vertices;
                    int[] triangles = hitMesh.triangles;


                    Vector3 hit0 = hitVertices[triangles[hit.triangleIndex * 3 + 0]];
                    Vector3 hit1 = hitVertices[triangles[hit.triangleIndex * 3 + 1]];
                    Vector3 hit2 = hitVertices[triangles[hit.triangleIndex * 3 + 2]];

                    List<Vector3> hitList = new List<Vector3>();
                    hitList.Add(hit0);
                    hitList.Add(hit1);
                    hitList.Add(hit2);

                    float distanceToHit = Mathf.Infinity;
                    Vector3 closest = Vector3.zero;
                    for (int k = 0; k < hitList.Count; k++)
                    {
                        float temp = Vector3.Distance(hit.point, hitList[k]);

                        if (temp < distanceToHit)
                        {
                            closest = hitList[k];
                            distanceToHit = temp;
                        }
                    }


                    //add the point on the outline we are using
                    Vector3 thisPoint = new Vector3(p1.x, p1.y, p1.z) + direction;
            //        if (!v3List.Contains(thisPoint))
            //        {
                        v3List.Add(thisPoint);
                        intList.Add(1);

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = thisPoint;
                        cube.transform.name = i.ToString();
                        cube.transform.localScale *= 0.2f;
             //       }

                    //now we have closet vertice to our point, add to a list to make a polygon from
                    //polygon is 2d, we can raycast quickly to get y point again
                    Vector3 listPoint = new Vector3(closest.x, closest.y, closest.z);

               //     if (!v3List.Contains(listPoint))
               //     {
                        v3List.Add(listPoint);
                        intList.Add(0);

                        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube2.transform.position = listPoint;
                        cube2.transform.name = i.ToString();
                        cube2.transform.localScale *= 0.1f;
                 //   }

                    

           /*        

                    //add the last point too
                    if (j == distance)
                    {
                        //v3List.Add(p1);
                        v3List.Add(p2);
                        // intList.Add(1);
                        intList.Add(1);
                    }
            */
                    //    meshTriangle.Add(new Vector2(p1.x, p1.z), 1);
                }
            }

            

            for (int k = 0; k < v3List.Count; k++)
            {
                meshTriangle.Add(v3List[k], intList[k]);

            }

        //    if (v3List.Count > 0)
        //        StartCoroutine(meshTriangle.Calculate(this));  //this has changed to cellandverge script type


            // meshTriangle.Clear();
            yield return new WaitForEndOfFrame();
        }
       
        yield break;
    }

    void PolygonFromList()
    {
        /*
        Vector3[] vertices2D = v3List.ToArray();

        // Use the triangulator to get indices for creating triangles

        int[] indices = new int[0];

        indices = indices.Reverse().ToArray();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {

            vertices[i] = new Vector3(vertices2D[i].x, 0f, vertices2D[i].y);
            //raycast for the y co ordinate
            RaycastHit hit;
            if (Physics.Raycast(vertices[i] + (Vector3.up * 500), Vector3.down, out hit, 1000f, LayerMask.GetMask("TerrainBase")))
            {
                vertices[i].y = hit.point.y;

             //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
             //   cube.transform.position = hit.point;
             //   cube.name = i.ToString();
            }
            else
            {
                Debug.Log("Raycast for Y hit on road skirt join did not hit");
            }
        }

    


        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;

        GameObject go = new GameObject();
        go.transform.parent = transform;
        go.name = "Join";

        go.gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = go.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;

        */

    }

    void Update()
    {
      //  Debug.Log(tri.Length);
        for (int i = 0; i < tri.Count-2; i += 3)
        {

            Debug.DrawLine(tri[i], tri[i + 1], Color.red);
            Debug.DrawLine(tri[i], tri[i + 2], Color.red);
            Debug.DrawLine(tri[i + 1], tri[i + 2], Color.red);

        }

    }
}