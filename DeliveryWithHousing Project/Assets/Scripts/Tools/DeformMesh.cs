using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DeformMesh : MonoBehaviour
{
    Camera cam;
    public bool mouse;
    public bool van;
    private WheelCollider wc;
    private float limit = 1f;
    void Start()
    {
        cam = GetComponent<Camera>();
        wc.GetComponent<WheelCollider>();
    }


    void FixedUpdate()
    {
        if (mouse)
            Mouse();

        if (van)
            Van();

    }
    void Van()
    {


        
        if( Physics.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Road")))
        {
            //ignore road just now
            //if used perhaps, give different terrai types different sink limits
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down,1f,LayerMask.GetMask("TerrainCell")); //0.15 is wheel width
       // RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.15f,Vector3.down, 1f, LayerMask.GetMask("TerrainCell")); //0.15 is wheel width
                                                                                                                        //       Debug.Log(hits.Length);
        foreach (RaycastHit hit in hits)
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return;
            
            DeformController deformController = hit.transform.GetComponent<DeformController>();
            //deformController.displayCorrectTriangle(hit.point,true);  //working GC probs
            deformController.MoveTriangle(deformController.gameObject, hit.point, true);
            deformController.hitByWheel = true;
   
        }
    }
    void Mouse()
    {
        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = hit.transform.GetComponent<MeshFilter>().mesh;
        Mesh originalMesh = hit.transform.GetComponent<OriginalMeshInfo>().originalMesh;

        //make temp
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        int triHit = triangles[hit.triangleIndex * 3 + 0];
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]]; //+ hit.transform.position;
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]]; //+ hit.transform.position;
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]]; //+ hit.transform.position;

//      p0 = hitTransform.TransformPoint(p0);
 //     p1 = hitTransform.TransformPoint(p1);
//      p2 = hitTransform.TransformPoint(p2);
     //   Debug.DrawLine(p0, p1);
    //    Debug.DrawLine(p1, p2);
    //    Debug.DrawLine(p2, p0);
        
            p0.y -= 0.1f;
            p1.y -= 0.1f;
            p2.y -= 0.1f;




  //      p0.x = originalMesh.vertices[triangles[triHit]].x;// Mathf.Clamp(p0.x, originalVertices[triHit].x - limit, originalVertices[triHit].x + limit);
        p0.y = Mathf.Clamp(p0.y, originalMesh.vertices[triangles[triHit]].y - limit, originalMesh.vertices[triangles[triHit]].y + limit);
    //    p0.z = originalMesh.vertices[triangles[triHit]].z;
        //    p0.z = Mathf.Clamp(p0.z, originalVertices[triHit].z - limit, originalVertices[triHit].z + limit);


        //    p1.x = Mathf.Clamp(p1.x, originalVertices[triHit + 1].x - limit, originalVertices[triHit + 1].x + limit);
//        p1.x = originalMesh.vertices[triangles[triHit+  1]].x;
        p1.y = Mathf.Clamp(p1.y, originalMesh.vertices[triangles[triHit +1 ]].y - limit, originalMesh.vertices[triangles[triHit +1 ]].y + limit);
        //    p1.z = Mathf.Clamp(p1.z, originalVertices[triHit + 1].z - limit, originalVertices[triHit + 1].z + limit);
        //p1.z = originalMesh.vertices[triangles[triHit + 1]].z;

        //p2.x = originalMesh.vertices[triangles[triHit + 2]].x;
        //    p2.x = Mathf.Clamp(p2.x, originalVertices[triHit + 2].x - limit, originalVertices[triHit + 2].x + limit);
        p2.y = Mathf.Clamp(p2.y, originalMesh.vertices[triangles[triHit + 2]].y - limit, originalMesh.vertices[triangles[triHit + 2]].y + limit);
        //     p2.z = Mathf.Clamp(p2.z, originalVertices[triHit + 2].z - limit, originalVertices[triHit + 2].z + limit);
        //p2.z = originalMesh.vertices[triangles[triHit + 2]].z;

        DeformController deformController = hit.transform.GetComponent<DeformController>();
        deformController.deformedVertices[triangles[hit.triangleIndex * 3 + 0]] = p0;
        deformController.deformedVertices[triangles[hit.triangleIndex * 3 + 1]] = p1;
        deformController.deformedVertices[triangles[hit.triangleIndex * 3 + 2]] = p2;
    }
}


