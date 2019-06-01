using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ExtrudeCell : MonoBehaviour {

    public float depth = 0.2f;
    public float yAmount;
    // Use this for initialization
    void Start()
    {


        Realign();


        GetComponent<MeshFilter>().mesh =  Extrude(GetComponent<MeshFilter>().mesh,depth);

        Rotate();

      //  FixMeshCollider();

     //   Combine();
        
    }

    void Realign()
    {
        
        //makes the transform position the centre of the mesh and moves the mesh vertices so the stay the same in world space
        Mesh mesh = GetComponent<MeshFilter>().mesh;

       
        transform.position = mesh.vertices[0];

        Vector3[] verts = mesh.vertices;
        List<Vector3> vertsList = new List<Vector3>();

        for(int i = 0; i < verts.Length; i++)
        {
            Vector3 point = verts[i] - transform.position;
        //    point.y = 0;
            vertsList.Add(point);
        }

        

        mesh.vertices = vertsList.ToArray();

       

    }

    public static Mesh Extrude(Mesh mesh,float depth)
    { 

        Vector3[] verts = mesh.vertices;        
        List<Vector3> vertsList = new List<Vector3>();
        List<int> trisList = new List<int>();

        for (int i = 0; i < verts.Length-1 ; i++)
        {
            if (i == 0)
                continue;

            vertsList.Add(verts[i]);
            vertsList.Add(verts[i + 1]);
            vertsList.Add(verts[i + 1] + (Vector3.down*depth));
            vertsList.Add(verts[i] + (Vector3.down * depth));
        }

        //add joining link/ last one
        vertsList.Add(verts[verts.Length-1]);
        vertsList.Add(verts[1]);
        vertsList.Add(verts[1] + (Vector3.down * depth));
        vertsList.Add(verts[verts.Length - 1] + (Vector3.down * depth));


        for (int i = 0; i < vertsList.Count - 2; i+=2)
        {
            
            trisList.Add(i + 0);
            trisList.Add(i + 2);
            trisList.Add(i + 1);

            trisList.Add(i + 0);
            trisList.Add(i + 3);
            trisList.Add(i + 2);

        }

        foreach (Vector3 v3 in vertsList)
        {
         //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         //   cube.transform.position = v3;
         //   cube.transform.localScale *= 0.1f;
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertsList.ToArray();
        newMesh.triangles = trisList.ToArray();

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        
        return newMesh;
	}
	
	void Rotate()
    {
        transform.rotation *= Quaternion.Euler(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
    }
    void FixMeshCollider()
    {

    }
    void Combine()
    {
        //if not already added combine, do it now
        if (transform.parent.GetComponent<CombineChildren>() == null)
            transform.parent.gameObject.AddComponent<CombineChildren>();

    }
}
