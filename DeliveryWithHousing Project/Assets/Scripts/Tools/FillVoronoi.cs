using UnityEngine;
using System.Collections;

public class FillVoronoi : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

        MeshGenerator mg = GetComponent<MeshGenerator>();

        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        for(int i = 0; i < vertices.Length; i++)
        {

            vertices[i] = Quaternion.Euler(90f, 0f, 0f) * vertices[i];

            vertices[i] *= 5f;
            mg.yardPoints.Add(vertices[i]);

        }

        mg.yardPoints.Add(Vector3.right);
        mg.yardPoints.Add(Vector3.left);

        GetComponent<MeshFilter>().mesh.vertices = vertices;

        
        

    }

}
