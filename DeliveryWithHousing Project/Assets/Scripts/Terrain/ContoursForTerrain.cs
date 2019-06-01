using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContoursForTerrain : MonoBehaviour {

    //creates contours
    List<Vector3> points = new List<Vector3>();
    public int height =400;
    public int density = 20;

    public MeshGenerator meshGenerator;
    
    // Use this for initialization
    void Start()
    {
        Border();
        StartCoroutine("Raycast"); //also starts mesh generator
    }

    void Border()
    {

    }

    IEnumerator Raycast()
    {

        StartCoroutine("ShootRight");
        StartCoroutine("ShootLeft");
        StartCoroutine("ShootUp");
        StartCoroutine("ShootDown");

        foreach (Vector3 point in points)
        {
            /*
         GameObject cube2= GameObject.CreatePrimitive(PrimitiveType.Cube);
         cube2.transform.position = point;
            cube2.transform.localScale *= density;
            Destroy(cube2.GetComponent<BoxCollider>());
            */
            meshGenerator.yardPoints.Add(point);
        }

        yield return new WaitForEndOfFrame();

        meshGenerator.enabled = true;

        yield break;
    }

    IEnumerator ShootRight()
    {

        //Grab mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3 corner = mesh.vertices[0];

        corner -= Vector3.up * height * 0.5f;

        for (int i = 0; i < 5000; i += density) // 5000 is width i+= is grid size
        {
            //go upwards in steps too, only go up as much as the mesh is wide. 
            //This means the world should no be taller, than it is wide
            for (int j = 0; j < height; j += density)//height
            {
                Vector3 position = corner + (Vector3.forward * i) + (Vector3.up * j) + transform.position;


                RaycastHit[] hits = Physics.RaycastAll(position, Vector3.right, 5000f, LayerMask.GetMask("TerrainBase"));

                foreach (RaycastHit hit in hits)

                {
                    points.Add(hit.point);
                }
                //   yield return new WaitForEndOfFrame();
            }
        }

        yield break;
    }

    IEnumerator ShootLeft()
    {

        //Grab mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3 corner = mesh.vertices[mesh.vertexCount-1];

        corner -= Vector3.up * height * 0.5f;

        for (int i = 0; i < 5000; i += density) // 5000 is width i+= is grid size
        {
            //go upwards in steps too, only go up as much as the mesh is wide. 
            //This means the world should no be taller, than it is wide
            for (int j = 0; j < height; j += density)//height
            {
                Vector3 position = corner + (Vector3.back * i) + (Vector3.up * j) + transform.position;

                RaycastHit[] hits = Physics.RaycastAll(position, Vector3.left, 5000f, LayerMask.GetMask("TerrainBase"));

                foreach (RaycastHit hit in hits)

                {
                    points.Add(hit.point);
                }
                //   yield return new WaitForEndOfFrame();
            }
        }
        
        yield break;
    }

    IEnumerator ShootUp()
    {

        //Grab mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3 corner = mesh.vertices[0];
        corner -= Vector3.up * height * 0.5f;

        for (int i = 0; i < 5000; i += density) // 5000 is width i+= is grid size
        {
            //go upwards in steps too, only go up as much as the mesh is wide. 
            //This means the world should no be taller, than it is wide
            for (int j = 0; j < height; j += density)//height
            {
                Vector3 position = corner + (Vector3.right * i) + (Vector3.up * j) + transform.position;


                RaycastHit[] hits = Physics.RaycastAll(position, Vector3.forward, 5000f, LayerMask.GetMask("TerrainBase"));

                foreach (RaycastHit hit in hits)

                {
                    points.Add(hit.point);
                }
                //   yield return new WaitForEndOfFrame();
            }
        }

        yield break;
    }


    IEnumerator ShootDown()
    {

        //Grab mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3 corner = mesh.vertices[mesh.vertexCount-1];
        corner -= Vector3.up * height * 0.5f;

        for (int i = 0; i < 5000; i += density) // 5000 is width i+= is grid size
        {
            //go upwards in steps too, only go up as much as the mesh is wide. 
            //This means the world should no be taller, than it is wide
            for (int j = 0; j < height; j += density)//height
            {
                Vector3 position = corner + (Vector3.left * i) + (Vector3.up * j) + transform.position;


                RaycastHit[] hits = Physics.RaycastAll(position, Vector3.back, 5000f, LayerMask.GetMask("TerrainBase"));

                foreach (RaycastHit hit in hits)

                {
                    points.Add(hit.point);
                }
                //   yield return new WaitForEndOfFrame();
            }
        }

        yield break;
    }


}
