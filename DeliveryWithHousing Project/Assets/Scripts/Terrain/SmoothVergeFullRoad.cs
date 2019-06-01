using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothVergeFullRoad : MonoBehaviour {

    /// <summary>
    /// Attatches to each road. Builds A verge and joins the fields/cells to this verge along the full road
    /// </summary>

	// Use this for initialization
	void Start ()
    {
        StartCoroutine("Verge");
	}
	
	
    IEnumerator Verge ()
    {
        BezierSpline spline = transform.GetComponent<RingRoadMesh>().updatedSpline;
        float frequency = transform.Find("RingRoad").GetComponent<MeshFilter>().mesh.vertexCount;
        float stepSize = 1f / frequency;
       
        int width = 3;
        int spread = 50;


        for (float i = 0; i < frequency; i+=spread)
        {
            List<Vector3> points = new List<Vector3>();

            for (float j = 0; j <= spread; j+=0.5f)
            {
                Vector3 point = spline.GetPoint((i + j) * stepSize);
             //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.transform.position = point;
              //  cube.name = "i " + i.ToString() + "j " + j.ToString();

               // continue;
                    

                Vector3 direction = spline.GetDirection((i+ j) * stepSize);
                //direction.Normalize();

                Vector3 rot1 = Quaternion.Euler(0f, 90f, 0f) * direction;

                Vector3 offToTheSide = point - (rot1 * width);

                List<Vector3> temp = new List<Vector3>();

                for (float k = 0; k <= width * 2; k +=0.25f)
                {
                    Vector3 pos = offToTheSide + (rot1 * k);

                    points.Add(pos);


                }
            }



            int acrossRoad = width * 2 * 4;//*4 because loop is +=0.25f
            int alongRoad = spread * 2;


            int xSize = acrossRoad;
            int ySize = alongRoad;

            int[] triangles = new int[(xSize * ySize) * 6];

            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }
            }



            Mesh mesh = new Mesh();
            mesh.vertices = points.ToArray();
            //mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            ;

            GameObject verge = new GameObject();
            verge.transform.parent = transform;
            verge.name = "SmoothVerge";

            verge.layer = 21;
            MeshFilter meshFilterInstance = verge.gameObject.AddComponent<MeshFilter>();
            meshFilterInstance.mesh = mesh;
            MeshRenderer meshRenderer = verge.gameObject.AddComponent<MeshRenderer>();
            // meshRenderer.enabled = false;
            meshRenderer.sharedMaterial = Resources.Load("Green", typeof(Material)) as Material;
            MeshCollider meshCollider = verge.gameObject.AddComponent<MeshCollider>();

            //add cell Y adjust

            verge.AddComponent<CellYAdjust>();


            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
