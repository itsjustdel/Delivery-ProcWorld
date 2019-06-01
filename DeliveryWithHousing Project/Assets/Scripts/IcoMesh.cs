using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class IcoMesh : MonoBehaviour {
	//public Transform cubePrefab;
	public Mesh mesh;
	public TextMesh tmPrefab;
	public float randomScale = 0.05f;
    public bool yOnly;

	// Use this for initialization
	void Start () {
		StartCoroutine("MoveMesh");

        
	}
	
	// Update is called once per frame


	IEnumerator MoveMesh()
	{
		mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
		for(int i = 0; i < mesh.vertexCount; i++)
		{
            	Vector3 random = new Vector3(Random.Range(-randomScale*Random.value*5,randomScale*Random.value*5),
            	                             Random.Range(-randomScale*Random.value,randomScale*Random.value),
            	                             Random.Range(-randomScale*Random.value,randomScale*Random.value));

            if (yOnly)
            {
                random = new Vector3(0f, Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                             0f);
            }
            vertices[i] += random;
//			verticeList.Add( mesh.vertices[i+1] + random);
//			verticeList.Add( mesh.vertices[i+2] + random);
	//		lst[i] = mesh.vertices[i] + random;
		

		}
        //textMesh(mesh.vertices);
        mesh.vertices = vertices;
        //	mesh.vertices = lst.ToArray();

        mesh.RecalculateNormals();
		//Once completed, enable CombineMesh on parent object

//		CombineChildren combine = transform.parent.GetComponent<CombineChildren>();

//		if (combine.enabled == false) 
//			combine.enabled = true;


		yield break;
	}

	void textMesh(Vector3[] vertices)
	{
		int i = 0;
		foreach( Vector3 pos in vertices )
		{
			TextMesh tm = GameObject.Instantiate( tmPrefab, pos+transform.position, Quaternion.identity ) as TextMesh;
			tm.name = (i).ToString();
			tm.text = (i++).ToString();
			tm.transform.parent = transform;
		}
	}
}
