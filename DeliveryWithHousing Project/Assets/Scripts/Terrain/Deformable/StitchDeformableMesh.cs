using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StitchDeformableMesh : MonoBehaviour {

    //raycasts and finds vertices which share the same space


    // Use this for initialization
    void Start()
    {
        V3();
    }

    void V3()
    {
        //we cant trust raycast. find closest vertice to each edge vertice. partner them

        //find surrounding cells
        List<int> edgePoints = GetComponent<FindEdges>().edgeVertices;
        List<Transform> transforms = new List<Transform>();
     
        //Debug.Log(transforms.Count);
        //for each transform, run through this transform's edge vertices and find the nearest one for each. 
        Mesh thisMesh = GetComponent<MeshFilter>().mesh;

       ////////cache verts
        for (int i = 0; i < edgePoints.Count; i++)
        {
        RaycastHit[] hits = Physics.RaycastAll(thisMesh.vertices[edgePoints[i]] + Vector3.up + transform.position, Vector3.down, 2f, LayerMask.GetMask("TerrainCell"));

        for (int h = 0; h < hits.Length; h++)
        {
            if (hits[h].transform != transform)
            {
                if (hits[h].transform.GetComponent<DeformController>() != null)
                {

                    //find closest edge point on mesh hit
                    Mesh hitMesh = hits[h].transform.GetComponent<MeshFilter>().mesh;
                    // Vector3[] hitVertices = t.GetComponent<MeshFilter>().mesh.vertices;
                    List<int> hitVertices = hits[h].transform.GetComponent<FindEdges>().edgeVertices;
                    // Debug.Log(edgePoints.Count);
                    Debug.Log(hitVertices.Count + " hit vertices");

                    float distanceToHit = Mathf.Infinity;
                    int closest = 0;

                    for (int j = 0; j < hitVertices.Count; j++)
                    {

                        float temp = Vector3.Distance(thisMesh.vertices[edgePoints[i]] + transform.position, hitMesh.vertices[hitVertices[j]] + hits[h].transform.position);//temp this/cache

                        if (temp < distanceToHit)
                        {
                            closest = j;
                            distanceToHit = temp;
                        }
                    }

                    //add partner info
                    DeformController.MeshAndIndex info = new DeformController.MeshAndIndex();
                    // info.thisMesh = mesh;
                    info.thisVerticeNumber = edgePoints[i];
                    info.otherTransform = hits[h].transform;
                    info.otherVerticeNumber = hitVertices[closest];

                    GetComponent<DeformController>().partners.Add(info);

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = thisMesh.vertices[info.thisVerticeNumber];
                    cube.transform.localScale *= 0.2f;
                    cube.transform.parent = transform;
                    cube.name = "This";


                    GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube2.transform.position = hitMesh.vertices[info.otherVerticeNumber];
                        cube2.transform.localScale *= 0.2f;
                    cube2.transform.parent = hits[h].transform;
                    cube2.name = "Other";

                        
                }
            }
        }
        }
    
                

    }
    

    void V2()
    {
        //raycast on each edge point 
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //    List<int> edgeVertices = GetComponent<FindEdges>().edgeVertices;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            RaycastHit[] hits = Physics.RaycastAll(vertices[i] + Vector3.up + transform.position, Vector3.down, 2f, LayerMask.GetMask("TerrainCell"));            

       //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       //     cube.transform.position = mesh.vertices[edgeVertices[i]] + Vector3.up + transform.position;
       //     cube.transform.localScale *= 0.2f;
       //     cube.transform.parent = transform;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform != transform)
                {
                    //find closest edge point on mesh hit
                    Mesh hitMesh = hit.transform.GetComponent<MeshFilter>().mesh;
                    Vector3[] hitVertices = hit.transform.GetComponent<MeshFilter>().mesh.vertices;
                    //         List<int> hitEdgeVertices = hit.transform.GetComponent<FindEdges>().edgeVertices;

                    float distanceToHit = Mathf.Infinity;
                    int closest = 0;

                    for (int j = 0; j < hitVertices.Length; j++)
                    {

                        float temp = Vector3.Distance(vertices[i] + transform.position, hitVertices[j] + hit.transform.position);

                        if (temp < distanceToHit)
                        {
                            closest = j;
                            distanceToHit = temp;
                        }
                    }

                    /*
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = vertices[i] + transform.position;//  + hit.transform.position;// hitMesh.vertices[hitEdgeVertices[0]];
                    cube.transform.localScale *= 0.2f;
                    cube.transform.parent = hit.transform;
                    cube.name = "This";


                    GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube2.transform.position = hitVertices[closest] + hit.transform.position;//  + hit.transform.position;// hitMesh.vertices[hitEdgeVertices[0]];
                        cube2.transform.localScale *= 0.2f;
                         cube2.transform.parent = hit.transform;
                    cube2.name = "Other";
                    */

                    //add partner info
                    DeformController.MeshAndIndex info = new DeformController.MeshAndIndex();
                    // info.thisMesh = mesh;
                    info.thisVerticeNumber = i;
                    info.otherTransform = hit.transform;
                    info.otherVerticeNumber = closest;

                    GetComponent<DeformController>().partners.Add(info);
                }
            }
        }
    }

    void V1()
    {
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            List<int> edgeVertices = GetComponent<FindEdges>().edgeVertices;

            for (int i = 0; i < edgeVertices.Count; i++)
            {

                //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //      cube.transform.position =  pointsOnEdge[i] + Vector3.up + transform.position;

                RaycastHit[] hits = Physics.SphereCastAll(mesh.vertices[edgeVertices[i]] + Vector3.up + transform.position, 0.1f, Vector3.down, 2f, LayerMask.GetMask("TerrainCell"));

                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform != transform)
                    {
                        //      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //       cube.transform.position = hit.point;
                        //          cube.transform.localScale *= 0.2f;
                        //     cube.transform.parent = hit.transform;

                        //add the nearest vertic from the triangle we hit
                        MeshCollider meshCollider = hit.collider as MeshCollider;

                        Mesh hitMesh = meshCollider.sharedMesh;
                        Vector3[] hitVertices = hitMesh.vertices;
                        int[] triangles = hitMesh.triangles;

                        Vector3 hit0 = hitVertices[triangles[hit.triangleIndex * 3 + 0]];// + transform.position;
                        Vector3 hit1 = hitVertices[triangles[hit.triangleIndex * 3 + 1]];// + transform.position;
                        Vector3 hit2 = hitVertices[triangles[hit.triangleIndex * 3 + 2]];// + transform.position;

                        List<Vector3> hitList = new List<Vector3>();
                        hitList.Add(hit0 + hit.transform.position);
                        hitList.Add(hit1 + hit.transform.position);
                        hitList.Add(hit2 + hit.transform.position);

                        float distanceToHit = Mathf.Infinity;
                        int closest = 0;
                        for (int k = 0; k < hitList.Count; k++)
                        {
                            //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //   cube.transform.position = hitList[k];
                            //        cube.transform.localScale *= 0.2f;
                            //   cube.transform.parent = hit.transform;

                            float temp = Vector3.Distance(mesh.vertices[edgeVertices[i]], hitList[k]);

                            if (temp < distanceToHit)
                            {
                                closest = k;
                                distanceToHit = temp;
                            }
                        }

                        //trying to get vertice number of other emsh to stitc to this vertice we ray from
                        //closest is the vertic number for pointsonedge[i]

                        //add closest to a list of foreign vertices(with gameobject it belongs to?)
                        //use edgevertices list to create link


                        //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //    cube.transform.position = hitVertices[triangles[(hit.triangleIndex * 3) + closest]] + hit.transform.position + transform.position;
                        //    cube.transform.localScale *= 0.2f;
                        //     cube.transform.parent = hit.transform;

                        //create info to give deform controller a look up table to check whether it needs to adjust partners too
                        DeformController.MeshAndIndex info = new DeformController.MeshAndIndex();
                        // info.thisMesh = mesh;
                        info.thisVerticeNumber = edgeVertices[i];
                        info.otherTransform = hit.transform;
                        info.otherVerticeNumber = triangles[(hit.triangleIndex * 3) + closest];


                        GetComponent<DeformController>().partners.Add(info);

                    }
                }
            }
        }
    }

