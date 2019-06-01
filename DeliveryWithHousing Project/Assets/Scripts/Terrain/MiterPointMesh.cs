using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MiterPointMesh : MonoBehaviour {

    private List<Vector3> pointsList = new List<Vector3>();
    // Use this for initialization
    void Start ()
    {
       
        List<Vector3> lastPoints = GetComponent<FindEdges>().pointsOnEdge;
        for (int i = 0; i < 6; i ++)
        {
            List<Vector3> tempList = new List<Vector3>();
            //get a list of intersection points at depth i
            tempList = IntersectionPoints(1,lastPoints);

            foreach (Vector3 v3 in tempList)
                pointsList.Add(v3);

            lastPoints = tempList;

        }


    
	}

    void OnDrawGizmos()
    {
        //foreach (List<Vector3> list in pointsList)
            foreach (Vector3 v3 in pointsList)
            {
                Gizmos.DrawSphere(v3, 0.2f);
            }
    }

    //get edge points

    //create miter points every step towards zero
    List<Vector3> IntersectionPoints(int step,List<Vector3> pointsOnEdge)
    {
        //List<Vector3> pointsOnEdge = GetComponent<FindEdges>().pointsOnEdge;
        List<Vector3> points = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>();
        //create vectors inside polygon
        for (int i = 0; i < pointsOnEdge.Count - 1; i++)
        {

          
            Vector3 p0 = pointsOnEdge[i];
            //points on edge is [1] here to create a vector to intersect with from the last point to the second point
            //this gives us the final intersect point at the first vector
            Vector3 p1 = pointsOnEdge[1];

            if (i != pointsOnEdge.Count - 1)
                p1 = pointsOnEdge[i + 1];


            Vector3 dir = p1 - p0;

            //always builds clockwise so we only need to rotate right(to the inside of the polygon)
            Vector3 normal = (Quaternion.Euler(0f, 90f, 0f) * dir);
            normal.Normalize();
            normal *= step;

            //move points inside
            p0 += normal;
            p1 += normal;

            //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     cube.transform.position = p0;

            //    GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube1.transform.position = p1;


            points.Add(p0);
            directions.Add(dir);


        }

        //get intersection points

        List<Vector3> intersectionPoints = new List<Vector3>();
        //check for intersections and add to list
        for (int i = 0; i < points.Count; i++)
        {
            //miss the last point
            if (i < points.Count - 1)
            {
                Vector3 closestP1;
                Vector3 closestP2;
                BushesForCell.ClosestPointsOnTwoLines(out closestP1, out closestP2, points[i], directions[i], points[i + 1], directions[i + 1]);

                //we only need to use the second
                intersectionPoints.Add(closestP1);

                //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //  cube.transform.position = closestP1;
            }
        }

        return intersectionPoints;
    }
    //add vertcies and triangles

    //make mesh
}
