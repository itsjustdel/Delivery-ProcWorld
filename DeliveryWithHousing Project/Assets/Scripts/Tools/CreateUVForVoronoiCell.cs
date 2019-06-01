using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateUVForVoronoiCell : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log(mesh.uv.Length);

        List<Vector2> uvs = new List<Vector2>();
        for(int i = 0; i < mesh.vertexCount; i++)
        {
            Vector2 uv = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
            uvs.Add(uv);
        }

        mesh.uv = uvs.ToArray();


        Debug.Log(mesh.uv.Length);
    }


}