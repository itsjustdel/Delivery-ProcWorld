using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BorderTools : MonoBehaviour {

    //functions to create miter points

    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else {
            return false;
        }
    }
    public static List<Vector3> IntersectionPoints(List<Vector3> pointsOnEdge,float pathSize)
    {
        //list we will return
        List<Vector3> intersectionPoints = new List<Vector3>();
        //poluations intersectionPoints

        List<Vector3> points = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>();
        //create vectors inside polygon
        for (int i = 0; i < pointsOnEdge.Count; i++)
        {

            Vector3 p0 = pointsOnEdge[i];
            //points on edge is [1] here to create a vector to intersect with from the last point to the second point
            //this gives us the final intersect point at the first vector
            Vector3 p1 = pointsOnEdge[1];

            if (i != pointsOnEdge.Count - 1)
                p1 = pointsOnEdge[i + 1];


            Vector3 dir = (p1 - p0).normalized;

            //always builds clockwise so we only need to rotate right(to the inside of the polygon)
            Vector3 normal = (Quaternion.Euler(0f, 90f, 0f) * dir);
            
            normal *= pathSize;

            //move points inside
            p0 += normal;
            p1 += normal;

            points.Add(p0);
            directions.Add(dir);

          //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //  cube.transform.position = p0;
        }

        //get intersection points

        intersectionPoints = new List<Vector3>();
        //check for intersections and add to list
        for (int i = 0; i < points.Count; i++)
        {
            //miss the last point
            if (i < points.Count - 1)
            {
                Vector3 closestP1;
                Vector3 closestP2;
                if (BushesForCell.ClosestPointsOnTwoLines(out closestP1, out closestP2, points[i], directions[i], points[i + 1], directions[i + 1]))
                {

                    //we only need to use the second

                    if (closestP1 != Vector3.zero)
                        intersectionPoints.Add(closestP1);
                }

            }
            //last point has been duplicated by 1st on find edges so we do not need to reverse any directional vectors

        }

        return intersectionPoints;



    }

    //looks for anything with layer "HouseFeature" and create a gap.Only support for 1 gap
    public static List<Vector3> CreateGap(List<Vector3> intersectionPoints)
    {
        
        List<Vector3> fencePoints = new List<Vector3>();
        //go through points on edge and look for gate trigger block. If we hit this, start from here.
        //move through list from this start point unitl we hit the gate trigger again. This is the end point
        Vector3 lastPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        bool firstPostAdded = false;
        bool lastPointAdded = false;
        bool firstPointAdded = false;
        int firstPost = 0;
        //look for start
        for (int i = 0; i < intersectionPoints.Count; i++)
        {
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = pointsOnEdge[i];
            //cube.transform.localScale *= 0.1f;


            //work our way to the next corner point
            float distance = 0f;
            if (i == intersectionPoints.Count - 1)
            {
                //if last one, measure to the first one to complete the loop
                distance = Vector3.Distance(intersectionPoints[i], intersectionPoints[0]);
            }
            else
                distance = Vector3.Distance(intersectionPoints[i], intersectionPoints[i + 1]);


            Vector3 directionToNextPost = Vector3.zero;
            if (i == intersectionPoints.Count - 1)
            {
                //if last one, make direction to the first point
                directionToNextPost = intersectionPoints[0] - intersectionPoints[i];
            }
            else
                directionToNextPost = intersectionPoints[i + 1] - intersectionPoints[i];

            //if distance is more than one, make it a unit vector(e.g 1 length)
            if (directionToNextPost.magnitude > 1)
                directionToNextPost.Normalize();

            for (float h = 0; h < distance; h += 0.1f) //minus postfrequency half to stop posts building near to the end // - postFrequency/2
            {
                Vector3 position = intersectionPoints[i] + (directionToNextPost * h);

                if (Physics.Raycast(position + (Vector3.up * 5), Vector3.down, 10f, LayerMask.GetMask("HouseFeature"), QueryTriggerInteraction.Collide)) 
                {
                    //the first time this hits, make this our last point
                    if (!lastPointAdded)
                    {
                        lastPoint = position; //previous pos?
                        lastPointAdded = true;
                    }
                }
                else
                //as soon as it doesnt hit house feature trigger block
                {
                    if (lastPointAdded)
                    {
                        if (!firstPointAdded)
                        {
                            firstPoint = position;
                            firstPointAdded = true;
                        }
                    }
                }
            }

            if (firstPointAdded && lastPointAdded && !firstPostAdded)
            {
                firstPost = i;
                firstPostAdded = true;
            }
            //we have what we were looking for
            //   break;
        }

        //find last point's proper position


        if (firstPoint == Vector3.zero || lastPoint == Vector3.zero)
        {
            Debug.Log("static points failed");
            return null;
        }
          
        //create ordered list

        fencePoints.Add(firstPoint); //used to use transform.position. If needed pass transform to static

        for (int i = firstPost; i < intersectionPoints.Count; i++)
        {
            //skip first one, we have replaced start point with first Point
            if (i == firstPost)
                continue;

            fencePoints.Add(intersectionPoints[i]);
        }

        float lastToFirstDistance = Vector3.Distance(lastPoint, firstPoint);

        for (int i = 0; i <= firstPost; i++)
        {
            //if point is furthr away than last post is to first post, add
            float thisToFirstDistance = Vector3.Distance(intersectionPoints[i], firstPoint);

            if (thisToFirstDistance > lastToFirstDistance)
                fencePoints.Add(intersectionPoints[i]);
        }

        fencePoints.Add(lastPoint);

        return fencePoints;
    }

    public static List<Vector3> LerpedPoints(List<Vector3> fencePoints, bool completeLoop, float gap)
    {
        //lerping
        List<Vector3> finalPoints = new List<Vector3>();
        //uses fence points
        //populates final points

        //place between intersection points
        for (int i = 0; i < fencePoints.Count - 1; i++)
        {
            Vector3 p0 = fencePoints[i];
            Vector3 p1 = fencePoints[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (float j = 0; j < distance; j+= gap)
            {
                Vector3 pos = p0 + (j * dir);
               
                finalPoints.Add(pos);

            }
        }

        //last post
        finalPoints.Add(fencePoints[fencePoints.Count - 1]);


        //completing the loop
        if (completeLoop)
        {

            Vector3 last = fencePoints[fencePoints.Count - 1];
            Vector3 first = fencePoints[0];
            float distanceLast = Vector3.Distance(first, last);
            Vector3 dirLast = (first - last).normalized;

            for (float j = 0; j < distanceLast; j+=gap)
            {
                Vector3 pos = last + (j * dirLast);
                finalPoints.Add(pos);
                
            }
        }

        return finalPoints;
    }

    public static  List<List<Vector3>> featurePositions(GameObject go, bool onlyAlongRoad,float featureSize,float gap,bool completeLoop)
    {
        List<Vector3> edges = go.GetComponent<FindEdges>().pointsOnEdge;
        //create border/miter points
        List<Vector3> intersectionPoints = BorderTools.IntersectionPoints(edges,featureSize);
        //create gap?

        //lerp


        List<Vector3> pointsTemp = new List<Vector3>();

     

        if (onlyAlongRoad)
        {
            for (int i = 0; i < intersectionPoints.Count; i++)
            {//spherecast for road

                RaycastHit hit;
                if (Physics.SphereCast(intersectionPoints[i] + Vector3.up * 10, featureSize, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
                {
                    //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //    cube.transform.position = intersectionPoints[i];
                    //    cube.transform.localScale *= 0.1f;
                    //    cube.transform.parent = transform;

                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Road"))
                    {   //we are near the raod side, add flower bed
                        pointsTemp.Add(intersectionPoints[i]);
                    }

                    // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // cube.transform.position = v3;
                }

            }
        }
        else
        {
            //add points all the way round
            pointsTemp = intersectionPoints;
        }
        //now we have a list of points along edge of road

        //lerp these points

        //complete loop if flower beds are not only along road
       // bool completeLoop = !onlyAlongRoad;

        List<Vector3> lerpedPoints = BorderTools.LerpedPoints(pointsTemp, completeLoop, gap);//do not complete loop - false

        List<List<Vector3>> allPoints = new List<List<Vector3>>();
        //reset temp list. We are going to populate this and add to a list of lists for each mud section
        pointsTemp = new List<Vector3>();
        //now check for house features and skip over. Create a list for each section

        // Debug.Log(lerpedPoints.Count);
        for (int i = 0; i < lerpedPoints.Count; i++)
        {

            //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.transform.position = lerpedPoints[i];
            //    cube.transform.localScale *= 0.5f;
            //    cube.transform.parent = transform;
            RaycastHit hit;
            if (Physics.Raycast(lerpedPoints[i] + Vector3.up * 5, Vector3.down, out hit, 10f, LayerMask.GetMask("HouseFeature")))
            {
                //if we hit a house feature, we can't use this point                
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("HouseFeature"))
                {
                    //we have hit a path, skip, and create new list if a list has been populated
                    if (pointsTemp.Count != 0)
                    {
                        //pointsTemp.Add(intersectionPoints[i]);
                        allPoints.Add(pointsTemp);

                        pointsTemp = new List<Vector3>();
                    }

                }
            }
            else
            {
                //this point is clear of any paths etc - add it
                pointsTemp.Add(lerpedPoints[i]);

            }
        }
        //add remaining points to list
        allPoints.Add(pointsTemp);

        return allPoints;

    }
}
