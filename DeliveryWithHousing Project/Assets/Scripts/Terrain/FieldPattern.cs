using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FieldPattern : MonoBehaviour {
    private List<Vector3> points = new List<Vector3>();
  
    //front and back meshes for leaf-- need so normals look correct
    private List<List<Mesh>> leaves = new List<List<Mesh>>();
    private List<Mesh> heads = new List<Mesh>();
    public bool planted = false;
    public bool plantTest = false;
    List<Vector3> forGizmo = new List<Vector3>();

    public BuildList buildList;
    float gap = 2f;
    // Use this for initialization
    void Awake()
    {
        buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();

        //enabled by build list 
        enabled = false;
        

    }
    
    void OnDrawGizmo()
    {
        foreach (Vector3 v3 in forGizmo)
        {
            Gizmos.DrawSphere(v3, 0.2f);
        }
    }
    void Start ()
    {
        //  StartCoroutine("Lines");

        if (GetComponent<FieldManager>().fieldUse == "Cabbages")
             StartCoroutine("LinesForCabbages");//this triggers lines from arrays in batches, to buildingFinished()- Area Trigger will create the meshes
            //GridInsidePoly();


        if (GetComponent<FieldManager>().fieldUse == "Trees")
        {
            StartCoroutine("PlantTrees");
        }

        //   fieldType = "Cabbages";
    }
    public void StartPlanting()
    {
        StartCoroutine("PlantFromArray");//called from activate objects
        planted = true;
    }

    IEnumerator Arrays()
    {
        for (int i = 0; i < 100; i++)
        {
            CreateLeafArray();

        }

        for(int i = 0; i < 10; i++)
        {
            CreateHeadArray();
        }

        
        StartCoroutine("LinesForCabbages"); 

        yield break;
    }

    IEnumerator PlantTrees()
    {
        
        //trees are created from pre built arrays of branches
        BranchArray branchArray = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();

        int numberOfTrees = 10;

        //raycast for field, create random v3 with mesh bounds and check it hits this field

        //size from middle to edge
        float limitX = GetComponent<MeshFilter>().mesh.bounds.extents.x*0.5f;
        float limitZ = GetComponent<MeshFilter>().mesh.bounds.extents.z*0.5f;

        int treesPlanted = 0;
        for (int i = 0; i < numberOfTrees; i++)        {
            if (treesPlanted >= 10)
                continue;

            //random position inside bounds, moved up for raycast to shoot down
            Vector3 random = new Vector3(Random.Range(-limitX, limitX), 100f, Random.Range(-limitZ, limitZ));            
            random += GetComponent<MeshFilter>().mesh.bounds.center;
            //check if this position hits the field
            RaycastHit hit;
            if (Physics.Raycast(random, Vector3.down, out hit, 200f))
            {
                if (hit.transform == transform)
                {
                    
                    //StartCoroutine(branchArray.MakeGardenTree(transform.gameObject, hit.point));//entering in to build list instead

                    //make mound for tree
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = hit.point;
                    //squish the sphere. This could be dont using a custom mesh to save tris
                    float moundSize = Random.Range(1f, 3f);
                    sphere.transform.localScale += new Vector3(moundSize, 0f, moundSize);

                    sphere.AddComponent<IcoMesh>();
                    //make same as field colour
                    sphere.GetComponent<MeshRenderer>().sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
                    sphere.name = "Tree Mound";
                    sphere.transform.parent = hit.transform;

                    QuickTree qT = sphere.AddComponent<QuickTree>();
                    qT.enabled = false;

                    buildList.components.Add(qT);


                    //yield return new WaitForEndOfFrame();
                    //yield return new WaitForSeconds(1);

                    //      Debug.Log("planted");
                    treesPlanted++;
                }
            }
        }


        buildList.BuildingFinished();
        yield break;

    }
    void CreateLeafArray()
    {
        //use leaves class to creat a leaf for us

        //if cabbage

        //create vector3 for vertices
        float variance = 0.1f;
        float standardSize = 0.4f;
        float radius = Random.Range(standardSize - variance, standardSize + variance);

        float xStretch = 1f* Random.Range(1f - radius, 1f + radius);

        radius = Random.Range(standardSize - variance, standardSize + variance);//too much?

        float zStretch = 1f*  Random.Range(1f - radius, 1f + radius);

        int leafDetail = 8;//needs to be an even number when divided by 2

        List<Vector3> points = Leaves.OutlineForArray(xStretch,zStretch,radius,leafDetail);
        //bends leaves (?)
        points = Leaves.WrapForArray(points);

        //create front and back mesh for leaf
        List<Mesh> meshes = Leaves.MeshForArray(points);

        //add this leaf to the leaf List
        leaves.Add(meshes);

    }
    void CreateHeadArray()
    {
        //how to decide how randomised? //join up with radius on leaf? //
        
        float variance = 0.05f;
        float standardSize = 0.2f;
        float headSize = Random.Range(standardSize-variance,standardSize+variance);

        Mesh mesh = new Mesh();
        Mesh meshPrefab = Resources.Load("Prefabs/Flora/IcoMesh") as Mesh;
        
        //make a copy of vertices
        Vector3[] vertices = meshPrefab.vertices;

        //randomise
        float randomScale = headSize * 0.5f; ;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 random = new Vector3(Random.Range(-randomScale * Random.value * 5, randomScale * Random.value * 5),
                                         Random.Range(-randomScale * Random.value, randomScale * Random.value),
                                         Random.Range(-randomScale * Random.value, randomScale * Random.value));
            vertices[i] += random;     
        }  

        //rescale prefab mesh and moveup slightly
        for (int i = 0; i < vertices.Length; i++)
        {
            
            vertices[i] *= headSize;
            vertices[i].y += headSize*1.5f;
        }



        


        mesh.vertices = vertices;
        mesh.triangles = meshPrefab.triangles;
        mesh.RecalculateNormals();

        //add to list for placing later
        heads.Add(mesh);        
    }

    Mesh CreateCabbageLeaves(Vector3 plantPosition, GameObject parent)
    {
        int leafNumber = Random.Range(4,8);
       
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int random = Random.Range(0, leaves.Count);
        List<Mesh> meshes = leaves[random];

        for (int p = 0; p < leafNumber; p++)
        {
          
            
            Vector3[] tempVertices = meshes[0].vertices;    //0 for front of mesh
            int[] tempTriangles = meshes[0].triangles;
            
            //0 is top mesh. still support there for underside mesh if we use [1]     
     
            //work how much to spin each leaf
            Vector3 p1 = tempVertices[1]; //leafParent.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[1];
            Vector3 p2 = tempVertices[2];   //same as above
            Quaternion rot = Leaves.RotateForArray(p1, p2, leafNumber, p);


            //add tris
            for (int t = 0; t < tempTriangles.Length; t++)
            {
                triangles.Add(tempTriangles[t] + vertices.Count);
            }

            //spin vertices
            for (int v=0; v< tempVertices.Length; v++)
            {
                tempVertices[v] = rot * tempVertices[v];


                //add to main mesh list
                vertices.Add( tempVertices[v]);

            }
            

            // plant.transform.GetChild(0).rotation = rot; //leafParent.transform.rotation = rot;
        }

        
        mesh.SetVertices(vertices);
        //mesh.subMeshCount = 1;        
        mesh.triangles = triangles.ToArray();
 

        return mesh;
    }

    
    
    void GridInsidePoly()
    {
        //test a grid from bounds.extents and clip pints that land outside field's polyline(edge)
        //create 2d array from bounds
        List<Vector2> grid = new List<Vector2>();
        float xSize= GetComponent<MeshRenderer>().bounds.size.x;
        float zSize = GetComponent<MeshRenderer>().bounds.size.z;
        Vector3 startV3 = GetComponent<MeshRenderer>().bounds.center - ((zSize * .5f) * Vector3.forward);

        List<Vector3> edgePoints = GetComponent<FindEdges>().pointsOnEdge;
        for (int i = 0; i < zSize; i++)
        {
            
            //run forwadr through grid and shoot right, use closest point on two lines function to find intersects
            Vector3 p = startV3 + (i * Vector3.forward);
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            a.transform.position = p;
            a.name = "P";
            for (int j = 0; j < edgePoints.Count -1; j++)
            {
                Vector3 cp1 = Vector3.zero;
                Vector3 cp2 = Vector3.zero;
                
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.position = edgePoints[j];
                s.name = "edge point0";
                s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.position = edgePoints[j+1];
                s.name = "edge point1";

                ClosestPointsOnTwoLines(out cp1, out cp2, p, p + Vector3.right, edgePoints[j], edgePoints[j + 1] - edgePoints[j]);

                if(cp1 != Vector3.zero)
                {
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = cp1;
                    c.name = "cp1";
                }
                if (cp2 != Vector3.zero)
                {
                    GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.position = cp2;
                    c.name = "cp2";
                }
                
            }

        }
        
    }
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

        else
        {
            return false;
        }
    }

    static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {
        int j = polyPoints.Length - 1;
        bool inside = false;
        for (int i = 0; i < polyPoints.Length; j = i++)
        {
            if (((polyPoints[i].y <= p.y && p.y < polyPoints[j].y) || (polyPoints[j].y <= p.y && p.y < polyPoints[i].y)) &&
               (p.x < (polyPoints[j].x - polyPoints[i].x) * (p.y - polyPoints[i].y) / (polyPoints[j].y - polyPoints[i].y) + polyPoints[i].x))
                inside = !inside;
        }
        return inside;
    }
    IEnumerator LinesForCabbages()
    {

        //change this filed's layer to Field Test. This will be th eonly object in the scene with this layer
        int initialLayer = gameObject.layer;
        gameObject.layer = 29;

        //raycast in parallel lines, move mesh points hit upwards to create bumps

        Vector3 startPoint = GetComponent<MeshRenderer>().bounds.center;// - GetComponent<MeshRenderer>().bounds.extents;
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = startPoint;
        Vector3 topCorner = GetComponent<MeshRenderer>().bounds.max;
        Vector3 bottomCorner = GetComponent<MeshRenderer>().bounds.min;

        //grab edge points
        List<Vector3> edgePointsOriginal = GetComponent<FindEdges>().pointsOnEdge;
        //create list of evenly spaced border points
        List<Vector3> edgePoints = new List<Vector3>();
        
        for (int i = 0; i < edgePointsOriginal.Count-1; i++)
        {
            Vector3 dir = (edgePointsOriginal[i + 1] - edgePointsOriginal[i]).normalized;
            float distance = Vector3.Distance(edgePointsOriginal[i + 1], edgePointsOriginal[i]);

            for(float j = 0; j < distance; j+=1f)
            {
                Vector3 p = edgePointsOriginal[i] + dir * j;
                edgePoints.Add(p);
                forGizmo.Add(p);
            }
        }



            //   float size = Vector3.Distance(startPoint, topCorner);
            float size = Vector3.Distance(bottomCorner, topCorner);// 200; //could do while inifite loop. safer this way


        for (int d = 0; d < 4; d++)
        {
            Vector3 dir = Vector3.zero;
            Vector3 sideDir = Vector3.zero;
            if (d == 0)
            {
                dir = Vector3.forward;
                sideDir = Quaternion.Euler(0, 90, 0) * dir;
            }
            else if (d == 1)
            {
                dir = Vector3.forward;
                sideDir = Quaternion.Euler(0, -90, 0) * dir;
            }
            else if (d == 2)
            {
                dir = Vector3.back;
                sideDir = Quaternion.Euler(0, 90, 0) * dir;
            }
            else if (d == 3)
            {
                dir = Vector3.back;
                sideDir = Quaternion.Euler(0, -90, 0) * dir;
            }

            // Vector3 sideDir = Quaternion.Euler(0, 90, 0) * dir;
            int startIndexI = 0;

            //stop overlaps by starting a little later
            if (d == 2 || d == 3)
                startIndexI++;

            //  int hitsPrevious = 0;
            int bottleCount = 0;
            for (int i = startIndexI; i < size; i+=2)
            {
                bottleCount++;
                //bottleneck protection
                if (bottleCount >= 10)
                {
                    yield return new WaitForEndOfFrame();
                    bottleCount = 0;
                }
                
                List<Vector3> thisRow = new List<Vector3>();

                    float startIndexJ = 0;
                //stop overlaps by starting a little later  //working?
                if (d == 1 || d == 3)
                    startIndexJ+=1f;
                bool startedRow = false;
                bool overShot = false;
                bool addedLastChunk = false;
                Vector3 previousHitPoint = Vector3.zero;
                List<Vector3> provisionalPoints = new List<Vector3>();

                for (float j = startIndexJ; j < size;)// j += 1f)
                {
                    //cast a set of rays in a circle downwards, better performance than spherecast, and is all we need for this task
                    //looking for any other cell that isnt ours. Only adding points if only hitting our own cell

                    bool add = false;
                    
                    Vector3 hitPoint = Vector3.zero;
                    List<RaycastHit> hits = new List<RaycastHit>();
                    for (float r = 0f; r < 360f; r += 360f)
                    {
                        Vector3 direction = Quaternion.Euler(0f, r, 0f) * Vector3.left;
                        Vector3 shootFrom = startPoint + (dir * i) + (sideDir * j) + direction;

                        
                        RaycastHit hit;
                        if (Physics.Raycast(shootFrom + Vector3.up * 10, Vector3.down, out hit, 20f, LayerMask.GetMask("FieldTest")))
                        {


                            hitPoint = hit.point;

                            

                            add = true;
                            /*
                            //if any hit hits another field, do not add hit
                            if (hit.transform.tag == "Field" && hit.transform != transform)
                            {

                                add = false;
                            }

                            //if it hits a terrain cell( the cells which over lap the road, do not add
                            if (hit.transform.tag == "Cell")
                            {
                                add = false;
                            }
                            */
                        }
                       
                        


                    }

                    if (add)
                    {                        
                        startedRow = true;

                        if (!overShot)
                        {
                            //we have not overshot

                            //dont do first one since we add points post second ray
                            if (previousHitPoint == Vector3.zero)
                            {
                                previousHitPoint = hitPoint;
                            //    yield return new WaitForEndOfFrame();
                            //    if (j == 2)
                             //       Debug.Log("previous hit point not being assigned");
                                continue;
                            }

                            //add the list filled from the previous jump//
                            foreach (Vector3 v3 in provisionalPoints)
                                points.Add(v3);

                            //clear list
                            provisionalPoints = new List<Vector3>();

                            //if we have not overshot,m we ahve succesfully jumped 10 steps in the field. Lerp these points in.
                            //This is so we save on 10 raycasts

                          //  float start = 0f;
                            for (float k = 0; k <= 10 ; k++)
                            {

                                //Vector3 projectedPosition = startPoint + (dir * i) + (sideDir * (j + k));
                                //lerp works form 0 to 1
                                float lerpAmount = k / 10f;
                                Vector3 projectedPosition = Vector3.Lerp(previousHitPoint, hitPoint, lerpAmount);

                                bool tooClose = false;
                                //check of point is too close to an edge point
                                foreach(Vector3 v3 in edgePoints)
                                {
                                    if (Vector3.Distance(projectedPosition, v3) < gap)
                                        tooClose = true;
                                }

                                if (!tooClose)
                                {
                                    //randomise slightly
                                    projectedPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));
                                    provisionalPoints.Add(projectedPosition);
                                }
                            }
                            //push it on a bit
                            j += 10f;
                           // Debug.Log(j);
                           // yield return new WaitForEndOfFrame();
                        }
                        else if(overShot)
                        {

                            if (!addedLastChunk)
                            {
                               
                                //addedLastChunk = true;
                            }

                            //add points on the way to the field edge

                            Vector3 point = startPoint + (dir * i) + (sideDir * j);
                            point = hitPoint;


                            bool tooClose = false;
                            //check of point is too close to an edge point
                            foreach (Vector3 v3 in edgePoints)
                            {
                                if (Vector3.Distance(point, v3) < gap)
                                    tooClose = true;
                            }

                            if (!tooClose)
                            {
                                //randomise slightly
                                point += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));

                                points.Add(point);
                            }
                            
                            //edge our way to the edge 
                            j++;
                           // yield return new WaitForEndOfFrame();
                        }
                        //   hitsPrevious++;
                    }
                    else if (!add)
                    {
                        //if have not started row yet, keep looking
                        if (!startedRow)
                        {
                            /*
                            //if we are on the last point in the row, we ahve found the end of the field, jump out of current direction
                            if (j >= size)
                            {
                                //force stop this whole row
                                i = (int)size;

                                continue;
                            }
                          *///  else if (j < size)
                          //  {
                                j++;
                                
                          //  }
                        }
                        else if (startedRow)
                        {
                            //we have shot past the edge of the field//reverse
                            //addd our last chunk of points
                            //add the list filled from the previous jump
                            foreach (Vector3 p in provisionalPoints)
                            {
                                Vector3 point = p;

                                bool tooClose = false;
                                //check of point is too close to an edge point
                                foreach (Vector3 v3 in edgePoints)
                                {
                                    if (Vector3.Distance(point, v3) < gap)
                                        tooClose = true;
                                }

                                if (!tooClose)
                                {
                                    //randomise slightly
                                    point += new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));

                                    points.Add(point);
                                }
                            }



                            //clear list
                            provisionalPoints = new List<Vector3>();

                            //move 11 back,  and baby step, j++
                            if (!overShot)
                            {
                                overShot = true;

                                j -= 11f; //10f +1
                               // yield return new WaitForEndOfFrame();
                            }
                            //if we have already overshot and baby stepped back to the edge of the field
                            else if (overShot)
                            {
                                //we have found the end. stop, move on to next row.
                                break;
                            }
                        }
                    }
                    //used to lerp on next hit
                    previousHitPoint = hitPoint;
                }
            }
        }

        // BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        // buildList.BuildingFinished();

        //reset layer now we have finished. We can do this because we only build one field at a time in the scene
        gameObject.layer = initialLayer;

    

        StartCoroutine("PlantFromArrayInBatches");

        yield break;

    }

    IEnumerator PlantFromArray()
    {
        //plants the whole field with grouped meshes(pre - combining them)

    
        //Runs through each point given by raycasting. Batches meshes, and adds each plant's vertices and triangles directly in to large mesh
        //creates gameobjects for these meshes
        Material[] mudArray = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().mudMaterials;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mudArray[Random.Range(0, mudArray.Length)];

        Material[] m = new Material[]
        {
           Resources.Load("Green") as Material,
           Resources.Load("Yellow") as Material
        };

        //create first parent object
        GameObject cropParent = new GameObject();
        cropParent.name = "Crop Parent";
        cropParent.transform.parent = transform;
        cropParent.transform.position = transform.position;
        CombineChildren cc = cropParent.AddComponent<CombineChildren>();
        cc.enabled = false;

        MeshFilter meshFilter = cropParent.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cropParent.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = m;
        
        Mesh parentMesh = new Mesh();
        List<Vector3> parentVertices = new List<Vector3>();
        List<int> parentTriangles = new List<int>();      
        List<int> headTriangles = new List<int>();
                
        for (int i = 0; i < points.Count; i++)
        {
            //bottlenck
            // if (i % batchSize == 0)
            //     yield return new WaitForEndOfFrame();
            if (i % 100 == 0) 
                yield return new WaitForEndOfFrame();
        
            //every x, create a new parent
            if (parentTriangles.Count > 4000|| i == points.Count -1)// && i != 0)//i % batchSize == 0 //4000 is "sweet spot"
            {
           //     yield return new WaitForEndOfFrame();
                //create and add mesh
                parentMesh.vertices = parentVertices.ToArray();
                //parentMesh.triangles = parentTriangles.ToArray();
                parentMesh.subMeshCount = 2;
                parentMesh.SetTriangles(parentTriangles, 0);
                parentMesh.SetTriangles(headTriangles, 1);

                parentMesh.RecalculateBounds();
                parentMesh.RecalculateNormals();
                meshFilter.mesh = parentMesh;

                //create new gameobject
                //    yield return new WaitForEndOfFrame();
                cropParent = new GameObject();
                cropParent.name = "Crop Parent";
                cropParent.transform.parent = transform;
                cropParent.transform.position = transform.position;
                meshFilter = cropParent.AddComponent<MeshFilter>();
                meshRenderer = cropParent.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterials = m;
                cc = cropParent.AddComponent<CombineChildren>();
                cc.enabled = false;

                //MeshRenderer meshRenderer = plant.AddComponent<MeshRenderer>();
                //meshRenderer.sharedMaterials = m;
                //    meshRenderer.enabled = false;

                parentMesh = new Mesh();
                parentVertices = new List<Vector3>();
                parentTriangles = new List<int>();
                headTriangles = new List<int>();
            }
                   
            //create a mesh of one flower
            Mesh mesh = CreateCabbageLeaves(points[i],cropParent);

            //add this flower's leaves mesh a points to parent

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            for(int t =0; t < triangles.Length; t++)
            {
                parentTriangles.Add(parentVertices.Count + triangles[t]);
            }

            foreach (Vector3 v3 in vertices)
                parentVertices.Add(( v3 + points[i]) );

            //add a head

            //add head to vertices
                
            Mesh headTempMesh = heads[Random.Range(0, heads.Count)];
            int[] headTempTriangles = headTempMesh.triangles;
            Vector3[] headTempVertices = headTempMesh.vertices;

            for (int h = 0; h < headTempTriangles.Length; h++)
            {
                headTriangles.Add(parentVertices.Count + headTempTriangles[h]);
            }

            foreach (Vector3 v3 in headTempVertices)
                parentVertices.Add(( v3 + points[i]) );
           
        }

        BuildList buildList = GameObject.FindGameObjectWithTag("Code").GetComponent<BuildList>();
        buildList.BuildingFinished();
        //StartCoroutine("CombineMeshes");
        yield break;
    }
    IEnumerator PlantFromArrayInBatches()
    {
        int batchSize = 10;
        int batchCount = 0;

        //make a game oject with a collider and batch script - when the player triggers this object, it will plant the plants
        GameObject batchParent = new GameObject();
        batchParent.transform.position = points[0 + batchSize/2];
        batchParent.name = "Batch Parent";
        //for trigger to activate
        batchParent.tag = "Flora";

        batchParent.AddComponent<BatchPlanter>();//enabled is false on Awake()
        BoxCollider bc= batchParent.AddComponent<BoxCollider>();
        bc.isTrigger = true;

        for (int i = 0; i < points.Count; i++)
        {
            if (i < points.Count - 1)
            {
                //check if we start a new row
                float distanceToNext = Vector3.Distance(points[i], points[i + 1]);

                if (batchCount == batchSize || distanceToNext > 2)
                {
                    //make new parent
                    batchParent = new GameObject();

                    //plant in centre of batch unless we are at the end of the points array
                    int middleOfBatch = batchSize / 2;                    
                    //check if we near the end
                    if (i + middleOfBatch >= points.Count)
                        //middle of what's left
                        middleOfBatch =(int)((points.Count - i)/2);

                    batchParent.transform.position = points[i + middleOfBatch];
                    batchParent.name = "Batch Parent";
                    batchParent.tag = "Flora";
                    batchParent.AddComponent<BatchPlanter>();
                    bc = batchParent.AddComponent<BoxCollider>();
                    bc.isTrigger = true;

                    batchCount = 0;
                }
            }

            //place individual game objects 
            GameObject plant = new GameObject();// GameObject.CreatePrimitive(PrimitiveType.Cube);/
            plant.name = "Plant Single";
            plant.transform.position = points[i];
            plant.transform.parent = batchParent.transform;
            

            batchCount++;

            if (i % 500 == 0)
                yield return new WaitForEndOfFrame();
        }


        //tell building pipeline we are finished
        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
        yield break;
    }

    IEnumerator Bushes()
    {
         
        BranchArray branchArray = GameObject.FindGameObjectWithTag("Code").GetComponent<BranchArray>();
        foreach (Vector3 v3 in points)
        {
            RaycastHit hit;
            if (Physics.Raycast(v3 + Vector3.up * 5, Vector3.down, out hit, 10f, LayerMask.GetMask("Default")))
            {

               // branchArray.MakeShrub(transform.gameObject, hit.point);
                yield return new WaitForEndOfFrame();
            }
            else
                Debug.Log("field pattern missed ray");
        }
    }

    IEnumerator Plant()
    {

        //    if(fieldType == "Cabbages")
        //    {

     

        //batch in to parent objects so we can use combine meshes script
        GameObject cropParent = new GameObject();
        cropParent.transform.parent = transform;
        cropParent.transform.position = transform.position;
        CombineChildren cc = cropParent.AddComponent<CombineChildren>();
        cc.enabled = false;

        for (int i = 0; i < points.Count; i++)
        {
            if (i % 100 == 0 && i != 0)
            {
                yield return new WaitForEndOfFrame();
            }


                //every x, create a new parent
                if (i % 200 == 0 && i != 0)
            {
            //    yield return new WaitForEndOfFrame();
                cropParent = new GameObject();
                cropParent.transform.parent = transform;
                cropParent.transform.position = transform.position;
                cc = cropParent.AddComponent<CombineChildren>();
                cc.enabled = false;
            }

            RaycastHit hit;
            //raycast for actual soil height
            if (Physics.Raycast(points[i] + Vector3.up * 2, Vector3.down, out hit, 4f))
            {
             //   GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
             //   cube.transform.position = hit.point;

                GameObject cabbage = new GameObject();
                cabbage.transform.parent = cropParent.transform;
                cabbage.transform.position = hit.point;
                PlantController pc = cabbage.AddComponent<PlantController>();
                pc.intendedPosition = hit.point;
                pc.cabbage = true;
            }
            else
                Debug.Log("No hit on field ray");



           //
         }

        StartCoroutine("CombineMeshes");
     //   }
        yield break;
    }

    IEnumerator CombineMeshes()
    {

        yield return new WaitForEndOfFrame();
      
        CombineChildren[] ccs = transform.GetComponentsInChildren<CombineChildren>();

      //  Debug.Log(ccs.Length);
        for(int i =0; i < ccs.Length; i++)
        {
          //  ccs[i].enabled = true;
            
            //if(i%10==0)
                yield return new WaitForEndOfFrame();

        }

        yield return new WaitForEndOfFrame();
        //destroy gameobjects

        for (int i = 0; i < ccs.Length; i++)
        {
            //for each child in parent

            for(int j = 0; j < ccs[i].transform.childCount; j++)
            {
               // if (ccs[i].transform.GetChild(j).name != "Combined mesh") 
                //    Destroy(ccs[i].transform.GetChild(j).gameObject);
            }

            //if(i%10==0)
            yield return new WaitForEndOfFrame();

        }

        yield break;
    }

    void Update()
    {
        if (plantTest)
        {
            plantTest = false;
            StartPlanting();
        }
    }
	
}
