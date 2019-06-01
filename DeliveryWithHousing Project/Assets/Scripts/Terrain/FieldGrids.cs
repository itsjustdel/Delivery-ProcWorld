using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrids : MonoBehaviour {

    //attach to a field polygon to split up in to grids
    
    // Use this for initialization
    void Start()
    {
        //use mesh render bounds to create grid
        float xSize = GetComponent<MeshRenderer>().bounds.size.x;
        float zSize = GetComponent<MeshRenderer>().bounds.size.z;
        Vector3 startV3 = GetComponent<MeshRenderer>().bounds.center - ((zSize * .5f) * Vector3.forward) - ((xSize * .5f) * Vector3.right);

        //List<Vector3> edgePoints = GetComponent<FindEdges>().pointsOnEdge;

        int gridSize = 10;
        
        for (int i = 0; i < xSize; i+=gridSize)
        {
            for (int j = 0; j < zSize; j+= gridSize)
            {
                Vector3 p = startV3 + i * Vector3.right + j*Vector3.forward;

                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Destroy(c.GetComponent<MeshCollider>());
                c.GetComponent<MeshRenderer>().enabled = false;
                Vector3[] vertices = c.GetComponent<MeshFilter>().mesh.vertices;
                //face up the way
                //for (int q = 0; q < vertices.Length; q++)
                //vertices[q] = Quaternion.Euler(90, 0, 0)*vertices[q];
                c.transform.rotation *= Quaternion.Euler(90, 0, 0);

                //c.GetComponent<MeshFilter>().mesh.vertices = vertices;

                c.transform.position = p + gridSize*0.5f*Vector3.right + gridSize*0.5f*Vector3.forward;

                c.transform.localScale *= gridSize;
                c.transform.parent = transform;
                c.tag = "Field";

                BoxCollider bc = c.AddComponent<BoxCollider>();
                bc.isTrigger = true;

                FieldGridPlanter fgp = c.AddComponent<FieldGridPlanter>();
                fgp.enabled = false;

            }
        }

        

        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
    }
	
}
