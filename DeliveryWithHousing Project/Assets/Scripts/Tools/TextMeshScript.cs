using UnityEngine;
using System.Collections;

public class TextMeshScript : MonoBehaviour {
	public TextMesh tmPrefab;

    void Awake()
    {
        enabled = false;
    }
    void Start()
    {
        textMesh();
    }

void textMesh()
{
    Vector3[] vertices = gameObject.GetComponent<MeshFilter>().mesh.vertices;
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