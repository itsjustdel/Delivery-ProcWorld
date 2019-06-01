using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SubdivideMesh : MonoBehaviour {
    public bool blockYAdjust = false;
    public bool uniqueVertices = false;
    
    //	level should be a number that's made of 2a * 3b where a and b are whole numbers between 0 and +inv
    // 	[2,3,4,6,8,9,12,16,18,24,27,32,36,48,64, ...]
    public bool finished = false;

    public int amount = 8;
    public bool doYAdjust = true;
	void Awake()
	{
		//enabled = false;
	}


	void Start () {

		MeshFilter MF = gameObject.GetComponent<MeshFilter>();
		Mesh mesh = MF.mesh;

        //create instance of Mesh helper class
        MeshHelper meshHelper = new MeshHelper();

        //send this object's mesh, how many subdivisions, and this gameObject to subdivision script
		StartCoroutine(meshHelper.Subdivide(mesh,amount,gameObject,doYAdjust)); //96 max  // divides a single quad into 6x6 quads // normally 32
		MF.mesh = mesh;

        

        if (uniqueVertices)
            mesh = UniqueVertices(mesh);


	//	StartCoroutine("EnableYAdjust"); //now called from end of subdivision script
    //this is due to the subdivision script yield from frame to smooth bottlenecks


	}

    Mesh UniqueVertices(Mesh mesh)
    {

        //Process the triangles
        Vector3[] oldVerts = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = new Vector3[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.name = "Unique Verts";

        return mesh;
    }

    public IEnumerator WaitForMeshThenRandomise(GameObject subdividedGo, float randomScale, bool yOnly)
    {
        Mesh subMesh = subdividedGo.GetComponent<MeshFilter>().mesh;

        if (subMesh.vertexCount == 0)
        {
            //Debug.Log("SBMC " + subMesh.vertexCount);
            //subdivide takes a few frames
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();//lovely
            //            yield return new WaitForSeconds(1);
            StartCoroutine(WaitForMeshThenRandomise(subdividedGo, randomScale, yOnly));

        }
        else
        {
            //  Debug.Log((subMesh.vertexCount) + " IN");
            subMesh = subdividedGo.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = subMesh.vertices;
            for (int i = 0; i < subMesh.vertexCount; i++)
            {
                Vector3 random = new Vector3(Random.Range(-randomScale * Random.value * 5, randomScale * Random.value * 5),
                                             Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                             Random.Range(-randomScale * Random.value, randomScale * Random.value));

                if (yOnly)
                {
                    random = new Vector3(0f, Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                                 0f);
                }
                vertices[i] += random;
                //			verticeList.Add( mesh.vertices[i+1] + random);
                //			verticeList.Add( mesh.vertices[i+2] + random);
                //		lst[i] = mesh.vertices[i] + random;


            }
            //textMesh(mesh.vertices);
            subMesh.vertices = vertices;
            //	mesh.vertices = lst.ToArray();

            subMesh.RecalculateNormals();

            subdividedGo.GetComponent<MeshFilter>().mesh = subMesh;
        }


    }
	
	public IEnumerator EnableYAdjust()
	{
        if (blockYAdjust)
            yield break;

        yield return new WaitForEndOfFrame();
		CellYAdjust cya =  gameObject.AddComponent<CellYAdjust>();
        cya.reportToSubdivideOnFinish = true;
//		cya.enabled = false;
//		gameObject.GetComponent<CellHasBeenBuilt>().hasBeenBuilt = true;
//		cya.saveOriginalMesh = false;
//		cya.activatePolygonTester = false;
//		cya.rayYAmount = 10f;
		//add trigger
//		BoxCollider box = gameObject.AddComponent<BoxCollider>();
//		box.isTrigger = true;
	}

    public void reportToJunctionCells() 
    {
        GameObject.FindGameObjectWithTag("VoronoiMesh").GetComponent<JunctionCells>().working = false;
    }

    public static Mesh SubdivideUsingOuterPointsOld(Mesh meshIn)
    {
        //Splits mesh up using outer points to govern pattern

        Mesh meshOut = new Mesh();

        //get outer points
        List<int> outerIndices = FindEdges.EdgeVertices(meshIn,0.0001f);
        //make loop
        outerIndices.Add(outerIndices[0]);
        
        List<Vector3> verticesOut = new List<Vector3>();
        List<int> triangles = new List<int>();
        //float distanceToCentre = 3f;
        int counter = 0;
        while (counter < 2)
        {
            for (int i = 0; i < outerIndices.Count - 1; i++)
            {
                //this welds vertices together, but not on first ring(outer)
                if (counter > 0)
                {
                    float distanceToNext = Vector3.Distance(meshIn.vertices[outerIndices[i]], meshIn.vertices[outerIndices[i + 1]]);
                    float threshold = 0.1f;
                    if (distanceToNext < threshold)
                    {
                        continue;
                    }
                }
                Vector3 toCentre = (meshIn.bounds.center - meshIn.vertices[outerIndices[i]]).normalized;
                Vector3 toCentreForNext = (meshIn.bounds.center - meshIn.vertices[outerIndices[i+1]]).normalized;

                Vector3 outerPos = meshIn.vertices[outerIndices[i]] + toCentre * counter;
                Vector3 outerPosNext = meshIn.vertices[outerIndices[i+1]] + toCentreForNext * counter;
                Vector3 innerPos = meshIn.vertices[outerIndices[i]] + toCentre * (counter+1);
                Vector3 innerNext = meshIn.vertices[outerIndices[i+1]] + toCentreForNext * (counter + 1);

                verticesOut.Add(outerPos);//0
                triangles.Add(verticesOut.Count - 1);
                verticesOut.Add(outerPosNext);//1
                triangles.Add(verticesOut.Count - 1);
                verticesOut.Add(innerPos);//2        
                triangles.Add(verticesOut.Count - 1);

                verticesOut.Add(outerPosNext);//1    
                triangles.Add(verticesOut.Count - 1);
                verticesOut.Add(innerNext);//3
                triangles.Add(verticesOut.Count - 1);
                verticesOut.Add(innerPos);//2
                triangles.Add(verticesOut.Count - 1);

                //tris for these
                //triangles.Add(verticesOut.Count - 3);
                //triangles.Add(verticesOut.Count - 1);
                //triangles.Add(verticesOut.Count + 2);
                //triangles.Add(verticesOut.Count + 1);
                //triangles.Add(verticesOut.Count + 3);
                //triangles.Add(verticesOut.Count + 2);
            }

            counter++;
        }

        foreach(Vector3 v3 in verticesOut)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.position = v3;
            c.transform.localScale *= 0.1f;
            
        }
        meshOut.vertices = verticesOut.ToArray();
        meshOut.triangles = triangles.ToArray();
        meshOut.RecalculateBounds();
        meshOut.RecalculateNormals();
        //create even gap between points
        //on first loop (most outer), don't skip if too close to next point

        return meshOut;
    }

    public static Mesh SubdivideUsingOuterPoints(Mesh meshIn,float stepSize)
    {
        //Splits mesh up using outer points to govern pattern

        Mesh meshOut = new Mesh();

        //get outer points
        List<int> outerIndices = FindEdges.EdgeVertices(meshIn, 0.0001f);
        //make loop
        //outerIndices.Add(outerIndices[0]);
        //outerIndices.Add(outerIndices[1]);
        //make orignal last item, the firsdt as well
        //outerIndices.Insert(0, outerIndices[outerIndices.Count - 3]);

        


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<Vector3> verticesTemp = new List<Vector3>();
        List<int> trianglesTemp = new List<int>();

        CreateSkirt(out vertices, out triangles, meshIn, outerIndices, stepSize);

        if (vertices != null)
        {
            //if skirt manged to build

            meshOut.vertices = vertices.ToArray();
            meshOut.triangles = triangles.ToArray();
            meshOut.RecalculateBounds();
            meshOut.RecalculateNormals();
            return meshOut;
        }
        else
        {
            meshOut = null;
            return meshOut;
        }
        
    }
    public static void CreateRingFromIndices(out List<Vector3> verticesOut,out List<int> trianglesOut, Mesh meshIn,List<int> outerIndices,float stepSize)
    {
        //create a mesh ring and returns the inner points

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<List<Vector3>> innerPointsByEdge = new List<List<Vector3>>();
        List<List<Vector3>> innerPointsByEdgeReversed = new List<List<Vector3>>();
        for (int i = 1; i < outerIndices.Count - 2; i++) //1 and -2 as we bookended the loop so we can check boywond and befroe it
        {

          

            Vector3 start = Vector3.Lerp(meshIn.vertices[outerIndices[i + 1]], meshIn.vertices[outerIndices[i]], 0.5f);



            Vector3 toCentreFromStart = (meshIn.bounds.center - start).normalized;
            Vector3 toCentreFromFirst = (meshIn.bounds.center - meshIn.vertices[outerIndices[i]]).normalized;
            Vector3 toCentreForNext = (meshIn.bounds.center - meshIn.vertices[outerIndices[i + 1]]).normalized;
            Vector3 toCentreFromMiddleStart = (meshIn.bounds.center - start).normalized;

            Vector3 innerBorderPoint = meshIn.vertices[outerIndices[i]] + toCentreFromFirst * stepSize;
            Vector3 innerBorderPointNext = meshIn.vertices[outerIndices[i + 1]] + toCentreForNext * stepSize;

            Vector3 innerStart = Vector3.Lerp(innerBorderPoint, innerBorderPointNext, 0.5f);
            for (int j = 0; j < 2; j++)//star t centre and work each way
            {//this welds vertices together, but not on first ring(outer)


                Vector3 end = meshIn.vertices[outerIndices[i + 1]];
                if (j == 1)
                    end = meshIn.vertices[outerIndices[i]];

                float distanceToNext = Vector3.Distance(start, end);

                Vector3 toCentreForEnd = (meshIn.bounds.center - end).normalized;
               
                Vector3 innerNextVertice = end + (toCentreForEnd * stepSize);

                    /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 2;
                c.transform.position = start;
                c.name = "start";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 2;
                c.transform.position = innerStart;
                c.name = "inner start";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;                

                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 4;
                c.transform.position = innerBorderPoint;
                c.name = "inner bp 1";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
                c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 4;
                c.transform.position = innerBorderPointNext;
                c.name = "inner bp2";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
                */

                Vector3 dirToNext = (end - start).normalized;
                Vector3 dirToNextInner = (innerNextVertice - innerStart).normalized;
                float distanceToNextInner = Vector3.Distance(innerStart, innerNextVertice);


                List<Vector3> insidePoints = new List<Vector3>();
                for (float k = 0; k < distanceToNextInner - stepSize; k += stepSize)
                {

                    //outer
                    Vector3 outerPos = start + (dirToNext * k);
                    Vector3 outerPosNext = start + (dirToNext) * (k + stepSize);//two steps
                    Vector3 toCentreFromOuter1 = (meshIn.bounds.center - outerPos).normalized;
                    Vector3 toCentreFromOuter2 = (meshIn.bounds.center - outerPosNext).normalized;

                    //now to create a straight inside edge we need to create a line between the two inner corners of this side and test for line intersection

                    Vector3 intersect1;
                    LineLineIntersection(out intersect1, innerStart, dirToNextInner, outerPos, toCentreFromOuter1);
                    Vector3 intersect2;
                    LineLineIntersection(out intersect2, innerStart, dirToNextInner, outerPosNext, toCentreFromOuter2);

                    insidePoints.Add(intersect1);
                    insidePoints.Add(intersect2);

                    /*
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = outerPos;
                    c.name = "Outer";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = outerPosNext;
                    c.name = "Outer next";


                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = intersect1;
                    c.name = "1";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize/2;
                    c.transform.position = intersect2;
                    c.name = "2";
                    */

                    if (j == 0)
                    {
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outerPosNext);//1    
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2
                        triangles.Add(vertices.Count - 1);
                    }
                    else if (j == 1)
                    {
                        //reverse
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(intersect1);//2
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1    
                        triangles.Add(vertices.Count - 1);
                    }

                    //check if last step
                    if (k + stepSize >= distanceToNextInner - stepSize)
                    {

                        outerPos = outerPosNext;
                        outerPosNext = end;



                        toCentreFromOuter1 = (meshIn.bounds.center - outerPos).normalized;
                        toCentreFromOuter2 = (meshIn.bounds.center - outerPosNext).normalized;

                        //now to create a straight inside edge we need to create a line between the tow inner corners of this side and test for line intersection


                        LineLineIntersection(out intersect1, innerStart, dirToNextInner, outerPos, toCentreFromOuter1);
                        LineLineIntersection(out intersect2, innerStart, dirToNextInner, outerPosNext, toCentreFromOuter2);
                        insidePoints.Add(intersect1);
                        insidePoints.Add(intersect2);
                        /*
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = outerPos;
                        c.name = "Outer last";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = outerPosNext;
                        c.name = "Outer next Last";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = intersect1;
                        c.name = "1";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = intersect2;
                        c.name = "2";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        */
                        if (j == 0)
                        {
                            vertices.Add(outerPos);
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect1);//2        
                            triangles.Add(vertices.Count - 1);

                            vertices.Add(outerPosNext);//1    
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect2);//3
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect1);//2
                            triangles.Add(vertices.Count - 1);
                        }
                        else if (j == 1)
                        {
                            //reverse
                            vertices.Add(intersect1);//2        
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPos);
                            triangles.Add(vertices.Count - 1);

                            vertices.Add(intersect1);//2
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect2);//3
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1    
                            triangles.Add(vertices.Count - 1);
                        }
                    }
                }

                if (distanceToNext < stepSize)
                {
                    //only need to do once
                    if (j == 0)
                    {
                        Vector3 outerPos = meshIn.vertices[outerIndices[i]];
                        Vector3 outerPosNext = meshIn.vertices[outerIndices[i + 1]];

                        //get next vertice, and point vector back
                        //get half way point between next inside border point and nextnext border point
                        Vector3 innerBorderPointNextNext = meshIn.vertices[outerIndices[i + 2]] + (meshIn.bounds.center - meshIn.vertices[outerIndices[i + 2]]).normalized * stepSize;
                        Vector3 halfWayToNextNext = Vector3.Lerp(innerBorderPointNext, innerBorderPointNextNext, 0.5f);
                        Vector3 directionBackFromHalfWayToNext = (innerBorderPointNext - halfWayToNextNext).normalized;

                        //get previous
                        Vector3 innerBorderPointPrevious = meshIn.vertices[outerIndices[i - 1]] + (meshIn.bounds.center - meshIn.vertices[outerIndices[i - 1]]).normalized * stepSize;
                        Vector3 halfWayToPrevious = Vector3.Lerp(innerBorderPoint, innerBorderPointPrevious, 0.5f);
                        Vector3 directionFromPreviousToCurrent = (innerBorderPoint - halfWayToPrevious).normalized;

                        //zero ys
                        float y = meshIn.bounds.center.y;
                        Vector3 p1 = halfWayToNextNext;
                        p1.y = y;
                        Vector3 d1 = directionBackFromHalfWayToNext;
                        d1.y = 0;//direction of y is zero
                        Vector3 p2 = halfWayToPrevious;
                        p2.y = y;
                        Vector3 d2 = directionFromPreviousToCurrent;
                        d2.y = 0;
                        //directionBackFromNextNext.y = 0;
                        // innerPrevious.y = y;
                        // directionFromPrevious.y = 0;

                        Vector3 intersect1;
                        LineLineIntersection(out intersect1, p1, d1, p2, d2);

                        insidePoints.Add(innerBorderPoint);
                        insidePoints.Add(intersect1);

                        insidePoints.Add(intersect1);
                        insidePoints.Add(innerBorderPointNext);

                        /*
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = innerBorderPointPrevious;
                        c.name = "inner bp previous";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = halfWayToPrevious;
                        c.name = "halfway to previous";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 4;
                        c.transform.position = halfWayToPrevious + directionFromPreviousToCurrent* 0.2f;
                        c.name = "direction back from halfway to previous";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = innerBorderPointNextNext;
                        c.name = "inner bp to next next";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = halfWayToNextNext;
                        c.name = "halfway to next";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 4;
                        c.transform.position = halfWayToNextNext+ directionBackFromHalfWayToNext* 0.2f;
                        c.name = "direction back from halfway to next";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c.transform.localScale *= 0.5f;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 4;
                        c.transform.position = intersect1;
                        c.name = "Intersect";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
                        c.transform.localScale *= 0.5f;
                        
    */



                        //make triangles, we need three to cover the corner, making one inside sharp corner
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(innerBorderPoint);//1
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outerPosNext);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(innerBorderPointNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);
                    }
                }

                if(j==0)
                    innerPointsByEdge.Add(insidePoints);
                else if(j==1)
                    innerPointsByEdgeReversed.Add(insidePoints);
            }
        }


        //organise lists in to edges, at the moment they are split in half in tow two lists, one reversed, one not
        List<List<List<Vector3>>> bothLists = new List<List<List<Vector3>>>();
        bothLists.Add(innerPointsByEdge);
        bothLists.Add(innerPointsByEdgeReversed);

        verticesOut = vertices;
        trianglesOut = triangles;

        return;

        //second inner ring
        for (int a = 0; a < bothLists.Count; a++)
        {
            for (int i = 0; i < bothLists[a].Count; i++)
            {
                for (int j = 0; j < bothLists[a][i].Count - 1; j += 2)
                {

                    Vector3 p1 = bothLists[a][i][j];
                    Vector3 p2 = bothLists[a][i][j+1];
                    Vector3 p3 = p1 + (meshIn.bounds.center - p1).normalized * stepSize;
                    Vector3 p4 = p2 + (meshIn.bounds.center - p2).normalized * stepSize;

                    // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // c.transform.position = bothLists[a][i][j];
                    // c.transform.transform.localScale *= 0.1f;

                    // c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //  c.transform.position = bothLists[a][i][j + 1];
                    // c.transform.transform.localScale *= 0.1f;

                    if (a == 0)
                    {
                        vertices.Add(p1);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p4);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);
                    }
                    else if (a == 1)
                    {
                        //reverse
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);                        
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p1);
                        triangles.Add(vertices.Count - 1);


                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p4);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                    }

                }
            }
        }

        verticesOut = vertices;
        trianglesOut = triangles;
    }//nope
    public static void CreateSkirtFromIndices(out List<Vector3> verticesOut, out List<int> trianglesOut, Mesh meshIn, List<int> outerIndices, float stepSize)
    {
        //create a mesh ring and returns the inner points

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        List<List<Vector3>> innerPointsByEdge = new List<List<Vector3>>();
        List<List<Vector3>> innerPointsByEdgeReversed = new List<List<Vector3>>();
        for (int i = 1; i < outerIndices.Count - 2; i++) //1 and -2 as we bookended the loop so we can check boywond and befroe it
        {

            if (Vector3.Distance(meshIn.vertices[outerIndices[i + 1]], meshIn.vertices[outerIndices[i]]) > 4)
                continue;

            Vector3 start = Vector3.Lerp(meshIn.vertices[outerIndices[i + 1]], meshIn.vertices[outerIndices[i]], 0.5f);



            Vector3 toCentreFromStart = (meshIn.bounds.center - start).normalized;
            Vector3 toCentreFromFirst = (meshIn.bounds.center - meshIn.vertices[outerIndices[i]]).normalized;
            Vector3 toCentreForNext = (meshIn.bounds.center - meshIn.vertices[outerIndices[i + 1]]).normalized;
            Vector3 toCentreFromMiddleStart = (meshIn.bounds.center - start).normalized;

            Vector3 innerBorderPoint = meshIn.vertices[outerIndices[i]] + toCentreFromFirst * stepSize;
            Vector3 innerBorderPointNext = meshIn.vertices[outerIndices[i + 1]] + toCentreForNext * stepSize;

            Vector3 innerStart = Vector3.Lerp(innerBorderPoint, innerBorderPointNext, 0.5f);
            for (int j = 0; j < 2; j++)//star t centre and work each way
            {//this welds vertices together, but not on first ring(outer)


                Vector3 end = meshIn.vertices[outerIndices[i + 1]];
                if (j == 1)
                    end = meshIn.vertices[outerIndices[i]];

                float distanceToNext = Vector3.Distance(start, end);

                Vector3 toCentreForEnd = (meshIn.bounds.center - end).normalized;

                Vector3 innerNextVertice = end + (toCentreForEnd * stepSize);

                /*
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale *= stepSize / 2;
            c.transform.position = start;
            c.name = "start";
            c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale *= stepSize / 2;
            c.transform.position = innerStart;
            c.name = "inner start";
            c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;                

            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale *= stepSize / 4;
            c.transform.position = innerBorderPoint;
            c.name = "inner bp 1";
            c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
            c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale *= stepSize / 4;
            c.transform.position = innerBorderPointNext;
            c.name = "inner bp2";
            c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
            */

                Vector3 dirToNext = (end - start).normalized;
                Vector3 dirToNextInner = (innerNextVertice - innerStart).normalized;
                float distanceToNextInner = Vector3.Distance(innerStart, innerNextVertice);


                List<Vector3> insidePoints = new List<Vector3>();
                for (float k = 0; k < distanceToNextInner - stepSize; k += stepSize)
                {

                    //outer
                    Vector3 outerPos = start + (dirToNext * k);
                    Vector3 outerPosNext = start + (dirToNext) * (k + stepSize);//two steps
                    Vector3 toCentreFromOuter1 = (meshIn.bounds.center - outerPos).normalized;
                    Vector3 toCentreFromOuter2 = (meshIn.bounds.center - outerPosNext).normalized;

                    //now to create a straight inside edge we need to create a line between the two inner corners of this side and test for line intersection

                    Vector3 intersect1;
                    LineLineIntersection(out intersect1, innerStart, dirToNextInner, outerPos, toCentreFromOuter1);
                    Vector3 intersect2;
                    LineLineIntersection(out intersect2, innerStart, dirToNextInner, outerPosNext, toCentreFromOuter2);

                    insidePoints.Add(intersect1);
                    insidePoints.Add(intersect2);

                    /*
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = outerPos;
                    c.name = "Outer";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = outerPosNext;
                    c.name = "Outer next";


                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = intersect1;
                    c.name = "1";

                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize/2;
                    c.transform.position = intersect2;
                    c.name = "2";
                    */

                    if (j == 0)
                    {
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outerPosNext);//1    
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect1);//2
                        triangles.Add(vertices.Count - 1);
                    }
                    else if (j == 1)
                    {
                        //reverse
                        vertices.Add(intersect1);//2        
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(intersect1);//2
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1    
                        triangles.Add(vertices.Count - 1);
                    }

                    //check if last step
                    if (k + stepSize >= distanceToNextInner - stepSize)
                    {

                        outerPos = outerPosNext;
                        outerPosNext = end;

                        toCentreFromOuter1 = (meshIn.bounds.center - outerPos).normalized;
                        toCentreFromOuter2 = (meshIn.bounds.center - outerPosNext).normalized;

                        //now to create a straight inside edge we need to create a line between the tow inner corners of this side and test for line intersection

                        Vector3 outerPosNextNext = meshIn.vertices[outerIndices[i + 2]];
                        LineLineIntersection(out intersect1, innerStart, dirToNextInner, outerPos, toCentreFromOuter1);
                        LineLineIntersection(out intersect2, innerStart, dirToNextInner, outerPosNext, toCentreFromOuter2);
                        insidePoints.Add(intersect1);
                        insidePoints.Add(intersect2);
                        /*
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = outerPos;
                        c.name = "Outer last";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = outerPosNext;
                        c.name = "Outer next Last";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = outerPosNextNext;
                        c.name = "Outer next next";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = intersect1;
                        c.name = "1";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = intersect2;
                        c.name = "2";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        */
                        if (j == 0)
                        {
                            vertices.Add(outerPos);
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect1);//2        
                            triangles.Add(vertices.Count - 1);

                            vertices.Add(outerPosNext);//1    
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect2);//3
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect1);//2
                            triangles.Add(vertices.Count - 1);
                        }
                        else if (j == 1)
                        {
                            //reverse
                            vertices.Add(intersect1);//2        
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPos);
                            triangles.Add(vertices.Count - 1);

                            vertices.Add(intersect1);//2
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect2);//3
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);//1    
                            triangles.Add(vertices.Count - 1);
                        }

                        //last triangle to take it up to edge of long edge*****no working
                        
                        float y = meshIn.bounds.center.y;
                        Vector3 p1 = intersect2;
                        p1.y = y;
                        Vector3 d1 = (intersect2 - intersect1).normalized;
                        d1.y = 0;//direction of y is zero
                        Vector3 p2 = outerPosNext;
                        p2.y = y;
                        Vector3 d2 = (outerPosNextNext - outerPosNext).normalized;
                        d2.y = 0;
                        //directionBackFromNextNext.y = 0;
                        // innerPrevious.y = y;
                        // directionFromPrevious.y = 0;

                        Vector3 intersect3;
                        LineLineIntersection(out intersect3, p1, d1, p2, d2);
                        
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = p1;
                        c.name = "p1";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;

                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = p2;
                        c.name = "p2";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;

                        /*
                        if (j == 0)
                        {
                            vertices.Add(intersect3);//2        
                            triangles.Add(vertices.Count - 1);                            
                            vertices.Add(intersect2);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outerPosNext);
                            triangles.Add(vertices.Count - 1);


                        }
                        else if (j == 1)
                        {

                            vertices.Add(outerPosNext);
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(intersect2);//1
                            triangles.Add(vertices.Count - 1);                            
                            vertices.Add(intersect3);//2        
                            triangles.Add(vertices.Count - 1);
                        }
                        */
                        //**end of no working
                    }
                }

                if (distanceToNext < stepSize)
                {
                    //only need to do once
                    if (j == 0)
                    {
                        Vector3 outerPos = meshIn.vertices[outerIndices[i]];
                        Vector3 outerPosNext = meshIn.vertices[outerIndices[i + 1]];

                        //get next vertice, and point vector back
                        //get half way point between next inside border point and nextnext border point
                        Vector3 innerBorderPointNextNext = meshIn.vertices[outerIndices[i + 2]] + (meshIn.bounds.center - meshIn.vertices[outerIndices[i + 2]]).normalized * stepSize;
                        Vector3 halfWayToNextNext = Vector3.Lerp(innerBorderPointNext, innerBorderPointNextNext, 0.5f);
                        Vector3 directionBackFromHalfWayToNext = (innerBorderPointNext - halfWayToNextNext).normalized;

                        //get previous
                        Vector3 innerBorderPointPrevious = meshIn.vertices[outerIndices[i - 1]] + (meshIn.bounds.center - meshIn.vertices[outerIndices[i - 1]]).normalized * stepSize;
                        Vector3 halfWayToPrevious = Vector3.Lerp(innerBorderPoint, innerBorderPointPrevious, 0.5f);
                        Vector3 directionFromPreviousToCurrent = (innerBorderPoint - halfWayToPrevious).normalized;

                        //zero ys
                        float y = meshIn.bounds.center.y;
                        Vector3 p1 = halfWayToNextNext;
                        p1.y = y;
                        Vector3 d1 = directionBackFromHalfWayToNext;
                        d1.y = 0;//direction of y is zero
                        Vector3 p2 = halfWayToPrevious;
                        p2.y = y;
                        Vector3 d2 = directionFromPreviousToCurrent;
                        d2.y = 0;
                        //directionBackFromNextNext.y = 0;
                        // innerPrevious.y = y;
                        // directionFromPrevious.y = 0;

                        Vector3 intersect1;
                        LineLineIntersection(out intersect1, p1, d1, p2, d2);

                        insidePoints.Add(innerBorderPoint);
                        insidePoints.Add(intersect1);

                        Vector3 outerPosPrev = meshIn.vertices[outerIndices[i - 1]];

                        //get outer mesh point on the long edge next othis corner
                        p1 = outerPos;
                        p1.y = y;
                        d1 = (outerPosPrev - outerPos).normalized;
                        d1.y = 0;//direction of y is zero
                        p2 = intersect1;
                        p2.y = y;
                        d2 = directionBackFromHalfWayToNext;
                        d2.y = 0;

                        Vector3 intersect2;
                        LineLineIntersection(out intersect2, p1, d1, p2, d2);
                        insidePoints.Add(intersect1);
                        insidePoints.Add(intersect2);

                        /*
                      GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = outerPos;
                      c.name = "outerpos";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c=  GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = outerPosPrev;
                      c.name = "outerposprev";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = intersect1;
                      c.name = "intersect1";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = intersect1 + directionBackFromHalfWayToNext;
                      c.name = "intersect 1 + dir";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = intersect2;
                      c.name = "intersect2";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                      c.transform.localScale *= 0.5f;




                      GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = innerBorderPointPrevious;
                      c.name = "inner bp previous";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = halfWayToPrevious;
                      c.name = "halfway to previous";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 4;
                      c.transform.position = halfWayToPrevious + directionFromPreviousToCurrent* 0.2f;
                      c.name = "direction back from halfway to previous";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = innerBorderPointNextNext;
                      c.name = "inner bp to next next";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 2;
                      c.transform.position = halfWayToNextNext;
                      c.name = "halfway to next";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 4;
                      c.transform.position = halfWayToNextNext+ directionBackFromHalfWayToNext* 0.2f;
                      c.name = "direction back from halfway to next";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                      c.transform.localScale *= 0.5f;

                      c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                      c.transform.localScale *= stepSize / 4;
                      c.transform.position = intersect1;
                      c.name = "Intersect";
                      c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Orange") as Material;
                      c.transform.localScale *= 0.5f;
                      */




                        //make triangles, we need two to conver corner, building to edge of cell
                        
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outerPosNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(innerBorderPointNext);//2        
                        triangles.Add(vertices.Count - 1);
                        
                        vertices.Add(outerPos);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(innerBorderPointNext);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(intersect2);//2        
                        triangles.Add(vertices.Count - 1);
                        
                    }
                }

                if (j == 0)
                    innerPointsByEdge.Add(insidePoints);
                else if (j == 1)
                    innerPointsByEdgeReversed.Add(insidePoints);
            }
        }


        //organise lists in to edges, at the moment they are split in half in tow two lists, one reversed, one not
        List<List<List<Vector3>>> bothLists = new List<List<List<Vector3>>>();
        bothLists.Add(innerPointsByEdge);
        bothLists.Add(innerPointsByEdgeReversed);

        verticesOut = vertices;
        trianglesOut = triangles;

        return;

        //second inner ring
        for (int a = 0; a < bothLists.Count; a++)
        {
            for (int i = 0; i < bothLists[a].Count; i++)
            {
                for (int j = 0; j < bothLists[a][i].Count - 1; j += 2)
                {

                    Vector3 p1 = bothLists[a][i][j];
                    Vector3 p2 = bothLists[a][i][j + 1];
                    Vector3 p3 = p1 + (meshIn.bounds.center - p1).normalized * stepSize;
                    Vector3 p4 = p2 + (meshIn.bounds.center - p2).normalized * stepSize;

                    // GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    // c.transform.position = bothLists[a][i][j];
                    // c.transform.transform.localScale *= 0.1f;

                    // c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //  c.transform.position = bothLists[a][i][j + 1];
                    // c.transform.transform.localScale *= 0.1f;

                    if (a == 0)
                    {
                        vertices.Add(p1);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p4);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);
                    }
                    else if (a == 1)
                    {
                        //reverse
                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p1);
                        triangles.Add(vertices.Count - 1);


                        vertices.Add(p3);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p4);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(p2);
                        triangles.Add(vertices.Count - 1);
                    }

                }
            }
        }

        verticesOut = vertices;
        trianglesOut = triangles;
    }//nope

    public static void CreateSkirt(out List<Vector3> verticesOut, out List<int> trianglesOut, Mesh meshIn, List<int> outerIndices, float stepSize)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //find two groups of points, one each side of the road
        
        //go through edges until we find one hat hits road 
        
        //make a loop so we can test the last point to the first
        outerIndices.Add(outerIndices[0]);
        
        
        List<List<int>> cellEdges = new List<List<int>>();
        
        for (int i = 0; i < outerIndices.Count-1; i++)
        {
            /*
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.localScale *= stepSize / 4;
            s.transform.position = meshIn.vertices[outerIndices[i]];
            s.name = "indice " + i.ToString();
            */

            Vector3 mid = Vector3.Lerp(meshIn.vertices[outerIndices[i]], meshIn.vertices[outerIndices[i+1]], 0.5f);
            RaycastHit hit;
            if (Physics.Raycast(mid + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
            { 
              /*  
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 2;
                c.transform.position = hit.point;
                c.name = "road";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                */
                //find next road
                List<int> cellEdge = new List<int>();
                //add this road edge - we need it's direction
                cellEdge.Add(i);
                cellEdge.Add(i + 1);
                
                for (int j = 0; j < outerIndices.Count -1 ; j++)
                {

                    int indexToUse = j;
                    indexToUse += i+1;
                    if (indexToUse >= outerIndices.Count - 1)
                        indexToUse -= outerIndices.Count-1;

                  /*  
                    c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.localScale *= stepSize / 2;
                    c.transform.position = meshIn.vertices[outerIndices[indexToUse]];
                    c.name = "cell edge";
                    c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                    */
                    mid = Vector3.Lerp(meshIn.vertices[outerIndices[indexToUse]], meshIn.vertices[outerIndices[indexToUse + 1]], 0.5f);
                    if (!Physics.Raycast(mid + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("Road")))
                    {
                        /*
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = meshIn.vertices[outerIndices[indexToUse]];
                        c.name = "cell edge No Road";
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                       */
                        //add to list of cell edge
                        cellEdge.Add(indexToUse);
                        cellEdge.Add(indexToUse+1);
                    }
                    else
                    {

                        //if it hits road,finish this list and send list to lists list
                        cellEdges.Add(cellEdge);
                        
                        
                        //jump from loop
                        break;
                    }
                }
            }
            else
            {
                /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 2;
               c.transform.position = mid;
                c.name = "edge";
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                */
               // cellEdge.Add(i);
            }
        }

       // Debug.Log(cellEdges.Count);
        //check for non uniform cell
        if(cellEdges.Count != 2)
        {
            //return null // we will use anothe subdivision method for this non unifrom cell
            verticesOut = null;
            trianglesOut = null;
            return;
        }
        //another non uniform check,
        foreach(List<int> cellEdge in cellEdges)
        {
            //each "cellEdge" must contain 4 entries - 2 for the road vector, start end, and anothe 2 for the edge after the road
            if (cellEdge.Count < 4)
            {
                verticesOut = null;
                trianglesOut = null;
                return;
            }
        }

        for (int i = 0; i < cellEdges.Count; i++)
        {
            Vector3 longEdgeDir1 = (meshIn.vertices[outerIndices[cellEdges[i][0]]] - meshIn.vertices[outerIndices[cellEdges[i][1]]]).normalized;
            //get other road edge saved in other list
            int otherIndex = 0;
            if (i == 0)
                otherIndex = 1;
            //note, direction is reversed
            Vector3 longEdgeDir2 = (meshIn.vertices[outerIndices[cellEdges[otherIndex][1]]] - meshIn.vertices[outerIndices[cellEdges[otherIndex][0]]]).normalized;

            float totalDistanceOfEdge = Vector3.Distance(meshIn.vertices[outerIndices[cellEdges[i][2]]], meshIn.vertices[outerIndices[cellEdges[i][cellEdges[i].Count - 1]]]);
            for (int j = 0; j < cellEdges[i].Count-1; j++)
            {
                //saved road edge as first two points
                if (j < 2)
                    continue;
                /*
                GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                c.transform.localScale *= stepSize / 2;
                c.transform.position = meshIn.vertices[outerIndices[cellEdges[i][j]]];
                c.name = i.ToString();
                c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                */
                //if(j < 2)
                //    c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                Vector3 p1 = meshIn.vertices[outerIndices[cellEdges[i][j]]];
                Vector3 p2 = meshIn.vertices[outerIndices[cellEdges[i][j + 1]]];
                float distanceToNext = Vector3.Distance(p1, p2);             
                Vector3 dirToNext = (p2 - p1).normalized;

                for (float k = 0; k < distanceToNext - stepSize; k += stepSize)
                {
                    float distanceToRoad = 1.5f; //slighlty more than we need to stop empty spaces
                    for (float x = 0; x < distanceToRoad; x += stepSize)
                    {
                        //step along the line
                        Vector3 temp1 = p1 + (dirToNext * k);
                        Vector3 temp2 = p1 + (dirToNext * (k + stepSize));

                        //lerp direction between the two outer/long directions - I think of an rpm needle goin from 0 to 100
                        int pointNumber = cellEdges[i].Count - 2;//take two off for long edge
                        //get lerp amount based on the percentage at which this point is from point1 of outside cell points to last - p[2] to p[last]
                        float distanceTo1 = Vector3.Distance(temp1, meshIn.vertices[outerIndices[cellEdges[i][2]]]);
                        //now to other long edge
                        float distanceTo2 = Vector3.Distance(temp2, meshIn.vertices[outerIndices[cellEdges[i][2]]]);

                        //get percentages
                        float difference = totalDistanceOfEdge - distanceTo1;
                        float difference2 = totalDistanceOfEdge - distanceTo2;
                        difference = 1f - (difference / totalDistanceOfEdge);
                        difference2 = 1f - (difference2 / totalDistanceOfEdge);

                        Vector3 dir = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference);
                        Vector3 dir2 = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference2);

                        Vector3 inside1 = temp1 + dir * (x+stepSize);
                        Vector3 inside2 = temp2 + dir2 * (x+stepSize);
                        //now work out step for outer position.(temp is only along outer cell edge)
                        Vector3 outside1 = temp1 + dir * x;
                        Vector3 outside2 = temp2 + dir2 * x;
                        /*
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = temp1;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = temp2;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = inside1;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = inside2;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        */
                        vertices.Add(outside1);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outside2);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside1);//2        
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outside2);//2
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside1);//1    
                        triangles.Add(vertices.Count - 1);

                        //catch last
                        if (k + stepSize >= distanceToNext - stepSize)
                        {
                            //fill last gap
                            temp1 = p1 + (dirToNext * (k + stepSize));
                            temp2 = p2;

                            //lerp direction between the two outer/long directions - I think of an rpm needle goin from 0 to 100
                            pointNumber = cellEdges[i].Count - 2;//take two off for long edge
                                                                 //get lerp amount based on the percentage at which this point is from point1 of outside cell points to last - p[2] to p[last]
                            distanceTo1 = Vector3.Distance(temp1, meshIn.vertices[outerIndices[cellEdges[i][2]]]);
                            //now to other long edge
                            distanceTo2 = Vector3.Distance(temp2, meshIn.vertices[outerIndices[cellEdges[i][2]]]);

                            //get percentages
                            difference = totalDistanceOfEdge - distanceTo1;
                            difference2 = totalDistanceOfEdge - distanceTo2;
                            difference = 1f - (difference / totalDistanceOfEdge);
                            difference2 = 1f - (difference2 / totalDistanceOfEdge);

                            dir = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference);
                            dir2 = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference2);

                            inside1 = temp1 + dir * (x + stepSize);
                            inside2 = temp2 + dir2 * (x + stepSize);
                            //now work out step for outer position.(temp is only along outer cell edge)
                            outside1 = temp1 + dir * x;
                            outside2 = temp2 + dir2 * x;

                            vertices.Add(outside1);
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(outside2);//1
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(inside1);//2        
                            triangles.Add(vertices.Count - 1);

                            vertices.Add(outside2);//2
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(inside2);//3
                            triangles.Add(vertices.Count - 1);
                            vertices.Add(inside1);//1    
                            triangles.Add(vertices.Count - 1);
                        }

                    }
                }
                //catch if only a small gap,smaller than stepsize
                if (distanceToNext < stepSize)
                {
                    for (float x = 0; x < 1; x += stepSize)
                    {
                        p1 = meshIn.vertices[outerIndices[cellEdges[i][j]]];
                        p2 = meshIn.vertices[outerIndices[cellEdges[i][j + 1]]];
                        distanceToNext = Vector3.Distance(p1, p2);
                        dirToNext = (p2 - p1).normalized;


                        //lerp direction between the two outer/long directions - I think of an rpm needle goin from 0 to 100
                        int pointNumber = cellEdges[i].Count - 2;//take two off for long edge
                                                                 //get lerp amount based on the percentage at which this point is from point1 of outside cell points to last - p[2] to p[last]
                        float distanceTo1 = Vector3.Distance(p1, meshIn.vertices[outerIndices[cellEdges[i][2]]]);
                        //now to other long edge
                        float distanceTo2 = Vector3.Distance(p2, meshIn.vertices[outerIndices[cellEdges[i][2]]]);

                        //get percentages
                        float difference = totalDistanceOfEdge - distanceTo1;
                        float difference2 = totalDistanceOfEdge - distanceTo2;
                        difference = 1f - (difference / totalDistanceOfEdge);
                        difference2 = 1f - (difference2 / totalDistanceOfEdge);

                        Vector3 dir = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference);
                        Vector3 dir2 = Vector3.Lerp(longEdgeDir1, longEdgeDir2, difference2);

                        Vector3 inside1 = p1 + dir * (x + stepSize);
                        Vector3 inside2 = p2 + dir2 * (x + stepSize);
                        //now work out step for outer position.(temp is only along outer cell edge)
                        Vector3 outside1 = p1 + dir * x;
                        Vector3 outside2 = p2 + dir2 * x;
                        /*
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = temp1;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = temp2;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = inside1;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Blue") as Material;
                        c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        c.transform.localScale *= stepSize / 2;
                        c.transform.position = inside2;
                        c.name = k.ToString();
                        c.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Red") as Material;
                        */
                        vertices.Add(outside1);
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(outside2);//1
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside1);//2        
                        triangles.Add(vertices.Count - 1);

                        vertices.Add(outside2);//2
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside2);//3
                        triangles.Add(vertices.Count - 1);
                        vertices.Add(inside1);//1    
                        triangles.Add(vertices.Count - 1);
                    }
                }
            }

           
        }
        
        verticesOut = vertices;
        trianglesOut = triangles;
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

}
