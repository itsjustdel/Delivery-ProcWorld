using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FenceAroundCell : MonoBehaviour {

    public List<Vector3> pointsOnEdge;
    public List<Vector3> fencePoints = new List<Vector3>();
   public List<Vector3> intersectionPoints = new List<Vector3>();
    public List<Vector3> finalPoints = new List<Vector3>();
    public float postFrequency = 2f;
    public float postHeight = 4f;    
    public float postWidth = 0.2f;

    public float strutNumber = 1f;
    public float strutHeight = 0.2f;    
    public float strutWidth = 0.1f;
    public bool houseCell;
    public float pathSize;

    public bool forceStop;
    // Use this for initialization


    void Start ()
    {
       

        //get path sizes
        if (gameObject.tag == "HouseCell")
            pathSize = GetComponent<HouseCellInfo>().fenceIndentation;

        if (gameObject.tag == "Field")
        {
            pathSize = GetComponent<FieldManager>().fenceIndentation;
        }


        //Find edges script has already found our path
        //pointsOnEdge = transform.parent.GetComponent<HouseCellInfo>().mVertices;          //if for junction - defunct?
        pointsOnEdge = GetComponent<FindEdges>().pointsOnEdge;
        RandomiseValues();


        IntersectionPoints();

        CreateGap();

        if (!forceStop)
        {
            PostPoints();

            StartCoroutine("FenceFromList");
        }
     //   AddGardenFeature();
       
	}

    void RandomiseValues()
    {
        postFrequency = Random.Range(1f, 3f);
        postHeight = Random.Range(0.2f, 1f);
        postWidth = postHeight / 4;//Random.Range(0.1f, 0.5f);

        strutNumber = Random.Range(1, 5);


        //strutHeight = (postHeight) / (strutNumber);
        //strutHeight *= 0.5f;
        strutHeight = postWidth/2;
        strutWidth = postWidth / 2;
    }
    void IntersectionPoints()
    {
        //uses pointOnEdge
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


            Vector3 dir = p1 - p0;

            //always builds clockwise so we only need to rotate right(to the inside of the polygon)
            Vector3 normal = (Quaternion.Euler(0f, 90f, 0f) * dir);
            normal.Normalize();
            normal *= pathSize;

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

        foreach (Vector3 v3 in intersectionPoints)
        {
          //       GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
          //    cube2.name = "intersect";
           //  cube2.transform.position = v3;
        }

        return;

        

    }
    void CreateGap()
    {
        //uses intersection points
        //populates fence points

        //only look for gap if a house cell atm
        if (gameObject.tag == "Field")
        {
            foreach (Vector3 v3 in intersectionPoints)
            {
                fencePoints.Add(v3);
            }
            //bugfinding
            //forceStop = true;
            Debug.Log("Field Fence returned");
            return;
        }
        //otherwise, this is a house cell, find gap for path

        //go through points on edge and look for gate trigger block. If we hit this, start from here.
        //move through list from this start point unitl we hit the gate trigger again. This is the end point
        Vector3 lastPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        bool firstPostAdded = false;
        bool lastPointAdded = false;
        bool firstPointAdded = false;
        int firstPost = 0;
        //look for start
        for (int i = 0; i < intersectionPoints.Count ; i++)
        {
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = pointsOnEdge[i];
            //cube.transform.localScale *= 0.1f;


            //work our way to the next corner point
            float distance = 0f;
            if(i == intersectionPoints.Count-1)
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
                Vector3 position = intersectionPoints[i] + (directionToNextPost * h);// + transform.position;
                                                                                     //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                                                                     //   cube.transform.position = position;
                                                                                     //   cube.transform.localScale *= 0.1f;
                                                                                     // Vector3 nextPosition = intersectionPoints[i] + (directionToNextPost * (h + postFrequency)); //h + 1
                RaycastHit hit;
                if (Physics.Raycast(position + (Vector3.up * 5), Vector3.down,out hit, 10f, LayerMask.GetMask("HouseFeature"), QueryTriggerInteraction.Collide))
                {
                    if (hit.transform.name == "Fence Trigger")
                    {
                        //the first time this hits, make this our last point
                        if (!lastPointAdded)
                        {
                            lastPoint = position; //previous pos?
                            lastPointAdded = true;
                        }
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
            Debug.Log("Fence failed");


            //report to build list
            BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
            buildList.BuildingFinished();


            return;
        }
        /*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = firstPoint;
        cube.transform.localScale *= 0.1f;
        cube.name = "first";
        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube2.transform.position = lastPoint;
        cube2.transform.localScale *= 0.1f;
        cube2.name = "last";
        */

        //create ordered list

        fencePoints.Add(firstPoint + transform.position);
        for (int i = firstPost; i < intersectionPoints.Count; i++)
        {
            //skip first one, we have replaced start point with first Point
            if (i == firstPost)
                continue;

            fencePoints.Add(intersectionPoints[i] + transform.position);
        }

        float lastToFirstDistance = Vector3.Distance(lastPoint, firstPoint);

        for (int i = 0; i <= firstPost; i++)
        {
            //if point is furthr away than last post is to first post, add
            float thisToFirstDistance = Vector3.Distance(intersectionPoints[i], firstPoint);
            
            if(thisToFirstDistance > lastToFirstDistance)
                fencePoints.Add(intersectionPoints[i] + transform.position);
        }
            fencePoints.Add(lastPoint + transform.position);

        
        for (int i= 0; i < fencePoints.Count;i++)
        {
        //  GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //  cube3.transform.position = fencePoints[i];
        //    cube3.transform.localScale *= 0.1f;
            
        }
        
    }
    void PostPoints()
    {
        //lerping
        
        //uses fence points
        //populates final points

        //place between intersection points
        for (int i = 0; i < fencePoints.Count - 1; i++)
        {
            Vector3 p0 = fencePoints[i];
            Vector3 p1 = fencePoints[i + 1];

            Vector3 dir = (p1 - p0).normalized;

            float distance = Vector3.Distance(p0, p1);

            for (int j = 0; j < distance; j++)
            {
                Vector3 pos = p0 + (j * dir);

                //  GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube1.transform.position = pos;
               // cube1.transform.localScale *= 0.1f;
                //add to a list for building
                finalPoints.Add(pos);

            }
        }

        //last post
        finalPoints.Add(fencePoints[fencePoints.Count - 1]);

   
        //completing the loop
        if (gameObject.tag == "Field")
        {
            Vector3 last = fencePoints[fencePoints.Count - 1];
            Vector3 first = fencePoints[0];
            float distanceLast = Vector3.Distance(first, last);
            Vector3 dirLast = (first - last).normalized;

            for (int j = 0; j < distanceLast; j++)
            {
                Vector3 pos = last + (j * dirLast);

                //   GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    cube1.transform.position = pos;

                finalPoints.Add(pos);
            }
        }

        for (int i = 0; i < finalPoints.Count; i++)
        {
           //   GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
           //   cube3.transform.position = finalPoints[i];
           //     cube3.transform.localScale *= 0.1f;

        }

    }
  
    IEnumerator FenceFromList()
    {
        //uses finalPoints


      //  if (!houseCell)
      //      fencePoints = pointsOnEdge;
        //load a cube so we can use its vertices as a mesh template to create the posts for the fence
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh cubeMesh = cube.GetComponent<MeshFilter>().mesh;       

        List<Vector3> fenceVertices = new List<Vector3>();
        List<int> fenceTriangles = new List<int>();
        //manipulate cube mesh to make it in to a post 
       
       Material material = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().fenceMaterials[Random.Range(0, 4)];

        // fencePoints = pointsOnEdge;
        //each corner point

        //to -2 so it builds to last post and stops
        for (int i = 0; i < finalPoints.Count-1; i++)
        {
           // yield return new WaitForEndOfFrame();
            //if (i != 0 && i % 5 == 0)
            if(fenceTriangles.Count > 4000)
            {
                Mesh fenceMeshPart = new Mesh();
                fenceMeshPart.vertices = fenceVertices.ToArray();
                fenceMeshPart.triangles = fenceTriangles.ToArray();
                fenceMeshPart.RecalculateBounds();
                fenceMeshPart.RecalculateNormals();

                GameObject fencePart = new GameObject();
                fencePart.name = "Fence";
                fencePart.transform.parent = transform;
                MeshFilter meshFilterPart = fencePart.AddComponent<MeshFilter>();
                meshFilterPart.mesh = fenceMeshPart;
                MeshRenderer meshRendererPart = fencePart.AddComponent<MeshRenderer>();
                meshRendererPart.sharedMaterial = material;

                fenceVertices.Clear();
                fenceTriangles.Clear();

                yield return new WaitForEndOfFrame();

            }
            //work our way to the next corner point
            float distance = Vector3.Distance(finalPoints[i], finalPoints[i + 1]);
         
            Vector3 directionToNextPost = finalPoints[i+1] - finalPoints[i];
            //if distance is more than one, make it a unit vector(e.g 1 length)
            if(directionToNextPost.magnitude > 1)
                directionToNextPost.Normalize();

            for (float h = 0; h < distance ; h+=postFrequency) //minus postfrequency half to stop posts building near to the end // - postFrequency/2
            {
              //  yield return new WaitForEndOfFrame();
                Vector3 position = finalPoints[i] + (directionToNextPost * h);
        
                //Vertical
                #region vertical

                List<Vector3> tempVertsVertical = new List<Vector3>();
                
                //create temporary list
                foreach (Vector3 v in cubeMesh.vertices)
                {                    
                    tempVertsVertical.Add(v);
                }

                for (int y = 0; y < tempVertsVertical.Count; y++)
                {
                    Vector3 temp = tempVertsVertical[y];
                    //make pivot point the bottom of the cube//cube is 1 unit in size
                    temp.y += 0.5f;
                    //stretch y
                    temp.y *= postHeight;
                    //shrink x and z
                    temp.x *= postWidth;
                    temp.z *= postWidth;

                    //rotate to face inwards
                    Quaternion rot = Quaternion.LookRotation(directionToNextPost);
                    rot *= Quaternion.Euler(0, 90f, 0);

                    temp = rot * temp;

                    tempVertsVertical[y] = temp;
                }
                //add vertices
                for (int k = 0; k < tempVertsVertical.Count; k++)
                {
                    fenceVertices.Add(position + tempVertsVertical[k]);
                }

              
                #endregion
               
                //Horizontal

                
                #region horizontal
                List<Vector3> tempVertsHorizontal = new List<Vector3>();
                foreach (Vector3 v in cubeMesh.vertices)
                {                  
                    tempVertsHorizontal.Add(v);
                }
                Vector3 nextPosition = finalPoints[i] + (directionToNextPost * (h + postFrequency)); //h + 1
                //find central point between this and next point
                Vector3 centre = Vector3.Lerp(position, nextPosition, 0.5f);
                              

                //GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube2.transform.position = centre;

                //manipulate cube mesh to make it in to horizontal stretches
                for (int x = 0; x < tempVertsHorizontal.Count; x++)
                {
                    Vector3 temp = tempVertsHorizontal[x];

                    //stretch the x to meet post at each side
                    //if not the last one
                    if(h < distance - (postFrequency))
                        temp.x *= postFrequency;
                    //if last one
                    else
                    {
                        //find distance to last post and centre 
                        centre = Vector3.Lerp(position, finalPoints[i + 1],0.5f);
                        float distanceToLast = Vector3.Distance(centre, finalPoints[i+1]);
                        //multiply by doubel since we only found distance from the center, not the full length
                        temp.x *= distanceToLast * 2;

                     //   GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                     //   cube2.transform.position = nextPosition;
                    }

                    //stretch y
                    temp.y *= strutHeight;
                    temp.z *= strutWidth;                   

                    //rotate to face inwards
                    Quaternion rot = Quaternion.LookRotation(directionToNextPost);
                    rot *= Quaternion.Euler(0, 90f, 0);
                    temp = rot * temp;
                    //assing
                    tempVertsHorizontal[x] = temp;
                }

                for (int s = 0; s < strutNumber; s++)
                {
                    //add vertices
                    for (int k = 0; k < tempVertsHorizontal.Count; k++)
                    { 
                        Vector3 pos = centre + tempVertsHorizontal[k];
                        float heightAdjustment = (postHeight / (strutNumber + 1));// * postWidth;
                        
                        pos.y += heightAdjustment * (s+1);
                        //if only one strut, move towards top of post//aesthetics
                        if (strutNumber == 1)
                            pos.y += postHeight / 3;

                        fenceVertices.Add(pos);
                    }
                  
                }

                #endregion
            
            }
           
            //add triangles
            int tempVerts = cubeMesh.vertexCount;

            for (int j = 0; j < fenceVertices.Count / tempVerts; j++)
            {
                //for each cube template
                for (int k = 0; k < cubeMesh.triangles.Length; k++)
                {
                    fenceTriangles.Add(cubeMesh.triangles[k] + (tempVerts * j));
                }
            }
        }

        //add final post
        #region Last
        //if house cell, leave the gap
      
        Vector3 positionLast = finalPoints[finalPoints.Count - 1];
        Vector3 directionToFirstPost = finalPoints[0] - finalPoints[finalPoints.Count - 1];
        List<Vector3> tempVertsVerticalLast = new List<Vector3>();

        //create temporary list
        foreach (Vector3 v in cubeMesh.vertices)
        {
            tempVertsVerticalLast.Add(v);
        }

        for (int y = 0; y < tempVertsVerticalLast.Count; y++)
        {
            Vector3 temp = tempVertsVerticalLast[y];
            //make pivot point the bottom of the cube//cube is 1 unit in size
            temp.y += 0.5f;
            //stretch y
            temp.y *= postHeight;
            //shrink x and z
            temp.x *= postWidth;
            temp.z *= postWidth;

            //rotate to face inwards
            Quaternion rot = Quaternion.LookRotation(directionToFirstPost);
            rot *= Quaternion.Euler(0, 90f, 0);

            temp = rot * temp;

            tempVertsVerticalLast[y] = temp;
        }
        //add vertices
        for (int k = 0; k < tempVertsVerticalLast.Count; k++)
        {
            fenceVertices.Add(positionLast + tempVertsVerticalLast[k]);
        }

        //add triangles
        for (int j = 0; j < fenceVertices.Count / tempVertsVerticalLast.Count; j++)
        {
            //for each cube template
            for (int k = 0; k < cubeMesh.triangles.Length; k++)
            {
                fenceTriangles.Add(cubeMesh.triangles[k] + (tempVertsVerticalLast.Count * j));
            }
        }
        
        #endregion

        Mesh fenceMesh = new Mesh();
        fenceMesh.vertices = fenceVertices.ToArray();
        fenceMesh.triangles = fenceTriangles.ToArray();
        fenceMesh.RecalculateBounds();        
        fenceMesh.RecalculateNormals();

        GameObject fence = new GameObject();
        fence.name = "Fence";
        fence.transform.parent = transform;
        MeshFilter meshFilter = fence.AddComponent<MeshFilter>();
        meshFilter.mesh = fenceMesh;
        MeshRenderer meshRenderer = fence.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        fenceVertices.Clear();
        fenceTriangles.Clear();

        //report to build list
        BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        buildList.BuildingFinished();


        Destroy(cube);
        yield break;
    }


    void AddGardenFeature()
    {

        //reporting finishd here atm
     ////   BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
    //    buildList.BuildingFinished();


        GardenFeature gf = gameObject.AddComponent<GardenFeature>();

        

        /*
        //find the individual cells which make up this "House/Junction" cell

        List<GameObject> cells = new List<GameObject>();

        GameObject parent = transform.parent.gameObject;
        int childCount = parent.transform.childCount;

        //only add uncombined cells
        for(int i = 0; i < childCount; i++)
        {
            if(parent.transform.GetChild(i).name=="Cell")
                cells.Add(parent.transform.GetChild(i).gameObject);
        }

        foreach (GameObject cell in cells)
        {
            GardenFeature gf = cell.AddComponent<GardenFeature>();
            gf.cone = true;
        }
        */
    }

   
}
