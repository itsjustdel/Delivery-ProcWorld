using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointInPolygon : MonoBehaviour {
    /// <summary>
    /// Determines if the given point is inside the polygon
    /// </summary>
    /// <param name="polygon">the vertices of polygon</param>
    /// <param name="testPoint">the given point</param>
    /// <returns>true if the point is inside the polygon; otherwise, false</returns>
    public static bool IsPointInPolygon4(Vector3[] polygon, Vector3 testPoint)
    {
        //convert to v2
        testPoint = new Vector3(testPoint.x, testPoint.z, 0f);

        bool result = false;
        int j = polygon.Length - 1;
        for (int i = 0; i < polygon.Length; i++)
        {
            //convert to v2
            polygon[i] = new Vector3(polygon[i].x, polygon[i].z, 0f);

            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
            {
                if (polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[j].y - polygon[i].y) * (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {

      

        int polygonLength = polygon.Length, i = 0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength - 1];
        endX = endPoint.x;
        endY = endPoint.y;
        while (i < polygonLength)
        {
            startX = endX; startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x; endY = endPoint.y;
            //
            inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                      && /* if so, test if it is under the segment */
                      ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
        }
        return inside;
    }
}
