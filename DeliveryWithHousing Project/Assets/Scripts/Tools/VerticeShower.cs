using UnityEngine;
using System.Collections;

public class VerticeShower : MonoBehaviour {

    void Awake ()
    {
        enabled = false;
    }
	// Use this for initialization
	void Start ()
    {
        

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GameObject vertices = new GameObject();
        vertices.transform.parent = transform;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = mesh.vertices[i];
            cube.transform.localScale *= 0.01f;
            cube.name = i.ToString();
            cube.transform.parent = vertices.transform;
        }
    }
	
	
}
