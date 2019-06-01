using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flowers : MonoBehaviour {

    public float maxWidth = 10f;
    public float randomScale = 0.05f;
    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GameObject prefab = Resources.Load("Prefabs/Flora/IcoPrefab") as GameObject;
        prefab.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
        for (int i = 0; i < mesh.vertexCount; i+=2)
        {
            
            GameObject cube = Instantiate(prefab);

            Vector3 random = new Vector3(Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                         Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                         Random.Range(-randomScale * Random.value, randomScale * Random.value));

            cube.transform.position = mesh.vertices[i] + random; ;
            Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);
            cube.transform.localScale = scale;

            Quaternion rotation = cube.transform.rotation;
            rotation *= Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(-180f, 180f), Random.Range(-30f, 30f));
            cube.transform.rotation = rotation;

            cube.transform.parent = transform;
        }

    }      
	
    void BaseForTable()
    {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 360; i += 10)
                {

                    Vector3 point = transform.position - (Vector3.left * (maxWidth - j)) + (j * Vector3.up);//radius is 15 on junction trigger
                    Vector3 pivot = transform.position;
                    Vector3 dir = point - pivot;
                    dir = Quaternion.Euler(0f, i, 0f) * dir;
                    Vector3 anchor = pivot + dir;
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = anchor + transform.position;
                    
                }
            }
     }
    

	// Update is called once per frame
	void Update () {
	
	}
}
