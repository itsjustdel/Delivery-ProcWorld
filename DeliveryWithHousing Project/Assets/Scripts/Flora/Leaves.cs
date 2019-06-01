using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Leaves : MonoBehaviour {

    public bool randomise;
    public int leafNumber = 4;
    public int leafDetailAmount = 12; //needs to be an even number when divided by 2
    public float radius = 5f;
    private float xStretch = 1f;
   // public float yStretch = 1f;
    public float zStretch = 1f;
    public float randomRangeForLeafSize = 0.2f;
    public bool render = false;

    

  //  private List<Vector3> points = new List<Vector3>();

    // Use this for initialization
    void Start ()
    {
        if(randomise)
            Randomise();

        BuildLeaves();
        
	}

    void Randomise()
    {
        PlantController pc = GetComponent<PlantController>();
        //randomise values
        //leafNumber
        //how many leaves are placed around stem
        if (pc.tulips)
        {
            leafNumber = Random.Range(1, 10);
        }

        {
         //   leafNumber = Random.Range(10, 11);
        }
        //radius
        //governs overall size of plant. Should be set externally by where it is being placed?
        
        if (pc.tulips)
        {
            radius = GetComponent<PlantPot>().topRadius1;
        }
        else if (pc.cabbage)
        {
            //radius = 0.2f; //set from plant controller
        }
        //leafDetailAmount
        //how many vertices are on each leaf. Probably doesn't need randomised, set to lowest possible (aesthetics vs performance) //needs to be even when diveided by 2
        //leafDetailAmount = 12;

        //xStretch
        //same as radius, not needed atm

        //zStretch
        //width of leaf. Multipler//grab pot radius and use this
        zStretch = Random.Range(0.1f, 3f);
        
    }

    void BuildLeaves()
    {
        PlantController pc = GetComponent<PlantController>();
        GameObject leaves = new GameObject();

        //set leaves to sit at pot height, if there is a pot
        Vector3 v3 = transform.position;
        if (pc.tulips)
        {
            v3.y = GetComponent<PlantPot>().height - (GetComponent<PlantPot>().potThickness / 2) + transform.position.y;
        }

        leaves.transform.position = v3;

        leaves.transform.parent = transform;
        leaves.name = "Tulip Leaves";

        for (int i = 0; i < leafNumber; i++)
        {
            GameObject leaf = new GameObject();
            leaf.name = "Leaf" + i.ToString();
            leaf.transform.parent = leaves.transform;
            leaf.transform.position = leaves.transform.position;

            List<Vector3> points = Outline(gameObject);
            points = Wrap(points);
            points = AlignPointsWithTransform(points);
            Mesh(points,leaf);
            Rotate(leaf, i);

            //points.Clear();
        }
    }

 
    public static List<Vector3> OutlineForArray(float xStretchInstance, float zStretchInstance,float radius,int leafDetailAmount)
    {
  //creates vector list for leaf mesh

        List<Vector3> points = new List<Vector3>();

        Vector3 position = Vector3.zero;

        Vector3 direction = Vector3.forward;

     //   float randomRangeForLeafSize = 0.2f;
   
       //     xStretchInstance *= Random.Range(1f - radius, 1f + radius);
       //     zStretchInstance *= xStretchInstance;     //doing before calling
        
        //int leafDetailAmount = go.GetComponent<Leaves>().leafDetailAmount;
        //create ring of points and stretch on x and z
        for (int j = 0; j < 360; j += 360 / leafDetailAmount) //j is 0 //cut code for creating a sphere. for loop only goes once
        {
            Vector3 point = position - (direction * radius);
            Vector3 pivot = position;
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(0f, j, 0f) * dir;

            Vector3 pointToAdd = pivot + dir;

            pointToAdd.x *= xStretchInstance;
            // pointToAdd.y += yStretch;
            pointToAdd.z *= zStretchInstance;

            //move mesh upwards so it rotates from its root
            pointToAdd.z += zStretchInstance * radius;


            points.Add(pointToAdd);
        }

        //add first point again to create a full loop for mesh
        points.Add(points[0]);

        return points;
    }
    public static List<Vector3> WrapForArray(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 v3 = points[i];
            v3.y += Mathf.Abs(points[i].x);

            points[i] = v3;
        }
        return points;
    }
    public static List<Mesh> MeshForArray(List<Vector3> points)
    {
        List<Mesh> meshes = new List<Mesh>();


        Mesh mesh = new Mesh();

        Vector3 middle = FindCentralPointByAverage(points.ToArray());

        List<Vector3> leafFrontPointsWithMiddle = new List<Vector3>();

        leafFrontPointsWithMiddle.Add(middle);// - transform.position);

        foreach (Vector3 v3 in points)
            leafFrontPointsWithMiddle.Add(v3);// - transform.position);

        //if we want to scale on the x, we need to spin the vertices a bit
        #region SpinVertices
        for (int j = 0; j < leafFrontPointsWithMiddle.Count; j++)
        {
            //  leafFrontPointsWithMiddle[j] = Quaternion.Euler(0f, 45f, 0f) * leafFrontPointsWithMiddle[j];
        }
        #endregion

        mesh.vertices = leafFrontPointsWithMiddle.ToArray();

        List<int> tris = new List<int>();
        //create triangles using center point as the anchor for each triangle
        for (int i = 0; i < leafFrontPointsWithMiddle.Count - 1; i++)
        {
            if (i == 0)
                continue;

            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);

        }

        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        meshes.Add(mesh);

        //create back mesh

        Mesh meshBack = new Mesh();
        meshBack.vertices = mesh.vertices;
        List<int> trisBack = new List<int>();
        for (int i = 0; i < meshBack.vertexCount - 1; i++)
        {
            if (i == 0)
                continue;

            trisBack.Add(0);
            trisBack.Add(i + 1);
            trisBack.Add(i);

        }
        meshBack.triangles = trisBack.ToArray();


        meshes.Add(meshBack);

        return meshes;
    }
    public static Quaternion RotateForArray(Vector3 firstMeshPoint, Vector3 secondMeshPoint, int totalLeaves,int child)
    {
        //leaf must not be rotated from building before we do this. This is beacseu we flatten the x valuie to work our the angle.
        //May be a better way to do this other than flattening the x coordinate to get out angle

        //get angle between 1st two mesh points //middle is 0
        Vector3 p1 = firstMeshPoint;
        Vector3 p2 = secondMeshPoint;// leaf.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[2];
        p2.x = p1.x;   

        //rock backwards
        float angle = Vector3.Angle(Vector3.up, (p2 - p1));
        //rotate on y depending on what child it is. Create a lovely corona of leaves
        float percentage = 360f / totalLeaves;
        float yRot = percentage * (child);
        yRot = Random.Range(-180f, 180f); //change 10 to zstretch/something?

        //Debug.Log(yRot);
        Quaternion rot = Quaternion.Euler(360 - angle, yRot, 0f);
        //leaf.transform.rotation = Quaternion.Euler(360 - angle, yRot, 0f);
        return rot;
    }

    public static List<Vector3> Outline(GameObject go)
    {
        PlantController pc = go.GetComponent<PlantController>();

        List<Vector3> points = new List<Vector3>();

        Vector3 position = Vector3.zero;
        
        Vector3 direction = Vector3.forward;

        //randomise muiltipliers
        float xStretchInstance = go.GetComponent<Leaves>().xStretch;
        float zStretchInstance = go.GetComponent<Leaves>().zStretch;

        float randomRangeForLeafSize = 0.2f;
        if (pc.tulips)
        {
            xStretchInstance *= Random.Range(1f - randomRangeForLeafSize, 1f + randomRangeForLeafSize);
            zStretchInstance *= Random.Range(1f - randomRangeForLeafSize, 1f + randomRangeForLeafSize);
        }
        if(pc.cabbage)
        {
            xStretchInstance *= Random.Range(1f - go.GetComponent<Leaves>().radius, 1f + go.GetComponent<Leaves>().radius);
            zStretchInstance *= xStretchInstance;
        }
        int leafDetailAmount = go.GetComponent<Leaves>().leafDetailAmount;
        //create ring of points and stretch on x and z
        for (int j = 0; j < 360; j += 360/ leafDetailAmount) //j is 0 //cut code for creating a sphere. for loop only goes once
        {
            Vector3 point = position - (direction * go.GetComponent<Leaves>().radius);
            Vector3 pivot = position;
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(0f, j, 0f) * dir;

            Vector3 pointToAdd = pivot + dir;
            
            

            pointToAdd.x *= xStretchInstance;
           // pointToAdd.y += yStretch;
            pointToAdd.z *= zStretchInstance;

            //move mesh upwards so it rotates from its root
            pointToAdd.z += zStretchInstance * go.GetComponent<Leaves>().radius;



            //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //     cube.transform.position = pointToAdd;
            
            points.Add(pointToAdd);
        }

        //add first point again to create a full loop for mesh
        points.Add(points[ 0 ]);

        return points;
    }

    List<Vector3> Wrap(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 v3 = points[i];
            v3.y += Mathf.Abs( points[i].x );

            points[i] = v3;
        }
        return points;
    }
    public List<Vector3> AlignPointsWithTransform(List<Vector3> points)
    {
        //the points need to be calculated over the zero axis.
        //Now they have been worked out we can move them all where they should be
        for (int i = 0; i < points.Count;i++)
        {
            points[i] += transform.position;
        }
        return points;
    }

    void Mesh(List<Vector3> points,GameObject leaf)
    {
        GameObject leafFront = new GameObject();
        leafFront.name = "Leaf Front";
        leafFront.transform.parent = leaf.transform;
        leafFront.transform.position = leaf.transform.position;

        Mesh mesh = new Mesh();

        Vector3 middle = FindCentralPointByAverage(points.ToArray());

        List<Vector3> leafFrontPointsWithMiddle = new List<Vector3>();
        
        leafFrontPointsWithMiddle.Add(middle - transform.position);

        foreach (Vector3 v3 in points)
            leafFrontPointsWithMiddle.Add(v3 - transform.position);

        //if we want to scale on the x, we need to spin the vertices a bit
        #region SpinVertices
        for (int j = 0; j < leafFrontPointsWithMiddle.Count; j++)
        {
          //  leafFrontPointsWithMiddle[j] = Quaternion.Euler(0f, 45f, 0f) * leafFrontPointsWithMiddle[j];
        }
        #endregion

        mesh.vertices = leafFrontPointsWithMiddle.ToArray();

        List<int> tris = new List<int>();
        //create triangles using center point as the anchor for each triangle
        for (int i = 0; i < leafFrontPointsWithMiddle.Count - 1; i++)
        {
            if (i == 0)
                continue;

            tris.Add(0);
            tris.Add(i);
            tris.Add(i + 1);        

        }

        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = leafFront.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = leafFront.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Green") as Material;
        

        if(!render)
            meshRenderer.enabled = false;

        //move down off the edge slightly
        Vector3 newPos = leafFront.transform.position;
        
        leafFront.transform.position = newPos;

        //create back
        GameObject leafBack = new GameObject();
        leafBack.name = "Leaf Back";
        leafBack.transform.parent = leaf.transform;
        leafBack.transform.position = leaf.transform.position;

        MeshFilter meshFilterBack = leafBack.AddComponent<MeshFilter>();        

        MeshRenderer meshRendererBack = leafBack.AddComponent<MeshRenderer>();
        meshRendererBack.sharedMaterial = Resources.Load("Green") as Material;

        if (!render)
            meshRendererBack.enabled = false;




        Mesh meshBack = new Mesh();
        meshBack.vertices = mesh.vertices;
        List<int> trisBack = new List<int>();
        for (int i = 0; i < meshBack.vertexCount -1 ; i++)
        {
            if (i == 0)
                continue;

            trisBack.Add(0);
            trisBack.Add(i+1);
            trisBack.Add(i);

        }



        meshBack.triangles = trisBack.ToArray();
        meshFilterBack.mesh = meshBack;


    }

    void Rotate(GameObject leaf,int child)
    {
        //leaf must not be rotated from building before we do this. This is beacseu we flatten the x valuie to work our the angle.
        //May be a better way to do this other than flattening the x coordinate to get out angle

        //get angle between 1st two mesh points //middle is 0
        Vector3 p1 = leaf.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[1];
        Vector3 p2 = leaf.transform.GetChild(0).GetComponent<MeshFilter>().mesh.vertices[2];
        p2.x = p1.x;
/*
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = p1;
        cube2.transform.position = p2;
        */

        //rock backwards
        float angle = Vector3.Angle(Vector3.up, (p2 - p1));
        //rotate on y depending on what child it is. Create a lovely corona of leaves
        float percentage = 360f / leafNumber;
        float yRot = percentage * (child);
        yRot += Random.Range(-10f, 10f); //change 10 to zstretch/something?

        leaf.transform.rotation = Quaternion.Euler(360 - angle, yRot, 0f);

    }

    public static Vector3 FindCentralPointByAverage(Vector3[] mVertices)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        for (int i = 0; i < mVertices.Length; i++)
        {
            x += mVertices[i].x;
            y += mVertices[i].y;
            z += mVertices[i].z;
        }

        x = x / mVertices.Length;
        y = y / mVertices.Length;
        z = z / mVertices.Length;

        Vector3 centre = new Vector3(x, y, z);

        return centre;
    }
}
