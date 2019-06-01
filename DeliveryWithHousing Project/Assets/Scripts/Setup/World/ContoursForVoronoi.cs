using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContoursForVoronoi : MonoBehaviour {

    //makes an array of pointswith higher density at slopes
    //the idea behinfd this is to create more options for a path finder when the terrain is rougher, so it may find a way over steeper sections by
    //winding up a hil
    public MeshGenerator mg;
    int heightStep = 10;
    int detail = 20;
    int size = 2000;
    List<Vector3> contourPounts = new List<Vector3>();
	// Use this for initialization
	void Start ()
    {   
        //
        for (int i = -size/2; i <= size/2; i+=detail) //x
        {
            float lastYHit = 0f;

            for (int j = -size/2; j <= size/2; j+=detail) //z
            {
                Vector3 from = new Vector3(i, 1000, j);
                
                
                if (j == -size/2 || j == size/2 || i == -size / 2 || i == size / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(from, Vector3.down, out hit, 2000f, LayerMask.GetMask("TerrainBase")))
                    {
                        //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      // c.transform.position = hit.point;
                      //  c.transform.localScale *= 10;
                        lastYHit = hit.point.y;
                       // Debug.Log("laastY hit FIRST " + lastYHit);

                       // contourPounts.Add(hit.point);
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(from, Vector3.down, out hit, 2000f, LayerMask.GetMask("TerrainBase")))
                    {
                        //if the terrain has a difference of ten from the last y hit, mark it
                        if(Mathf.Abs(hit.point.y - lastYHit) >= 10f)
                        {
                          //  Debug.Log(Mathf.Abs(hit.point.y - lastYHit));
                            //GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                         //  c.transform.position = hit.point;
                          //  c.transform.localScale *= 10;
                            lastYHit = hit.point.y;
                            contourPounts.Add(hit.point);
                        }

                    }
                }
            }
        }

        //now we need to zero points for mesh graph

        for (int i = 0; i < contourPounts.Count; i++)
        {
            contourPounts[i] = new Vector3(contourPounts[i].x, 0, contourPounts[i].z);
        }

        //randomise
        for (int i = 0; i < contourPounts.Count; i++)
        {

            contourPounts[i] = new Vector3(contourPounts[i].x *Random.Range(0.75f,1.25f), 0, contourPounts[i].z * Random.Range(0.75f, 1.25f));
        }

        //add border
        //pass the centre of the tjunction to the circle function
        Vector2 center = new Vector2(transform.position.x, transform.position.z);
        List<Vector3> borderPoints = DrawCirclePoints(200, 1500, center);
        foreach (Vector3 v3 in borderPoints)
            contourPounts.Add(v3);

        borderPoints = DrawCirclePoints(300, 2000, center);
        foreach (Vector3 v3 in borderPoints)
            contourPounts.Add(v3);

       // borderPoints = DrawCirclePoints(500, 3000, center);
       // foreach (Vector3 v3 in borderPoints)
           // contourPounts.Add(v3);


        mg.yardPoints = contourPounts;
        mg.enabled = true;
	}

    List<Vector3> DrawCirclePoints(int points, double radius, Vector2 center)
    {

        List<Vector3> pointsreturn = new List<Vector3>();

        float slice = 2 * Mathf.PI / points;
        for (int i = 0; i < points; i++)
        {
            float angle = slice * i;
            int newX = (int)(center.x + radius * Mathf.Cos(angle));
            int newY = (int)(center.y + radius * Mathf.Sin(angle));
            Vector2 p = new Vector2(newX, newY);

            Vector3 p3 = new Vector3(p.x, 0f, p.y);

            pointsreturn.Add(p3);
        }

        return pointsreturn;
    }
}
