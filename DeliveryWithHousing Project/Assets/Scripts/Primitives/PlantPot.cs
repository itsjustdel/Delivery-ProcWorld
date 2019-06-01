using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlantPot : MonoBehaviour {

  //  public bool tree;
 //   public bool roses;

    public float tubeHeightMin = 0.2f;
    public float tubeHeightMax = 1f;
    public float topRadiusMin = 0.5f;
    public float topRadiusMax = 2;
    public float bottomRadiusReduceMin = 0.5f;
    public float bottomRadiusReduceMax = 1.2f;
    public int tubeSidesMin = 3;
    public int tubeSidesMax = 3;
    public float potThicknessMin = 0.1f;
    public float potThicknessMax = 0.5f;
    public float potThickness;

    private List<Vector3> soilPoints = new List<Vector3>();

    public bool makeRectangle = false;

    public float topRadius1;
    public float height;

    //public float 
    // Use this for initialization
    void Start ()
    {
        //gameObject.AddComponent<MeshFilter>();

        Tube();

        //GetComponent<PottedPlantController>().enabled = true;

      //  StartCoroutine("Loop");
    }

   IEnumerator Loop()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(2);
            Tube();
        }
    }

    public static GameObject PlantPotMaker(Transform transform,float height,int nbSides,float potThickness,float bottomRadiusReduce,float topRadius1,bool addMeshCollider)
    {
        GameObject pot = new GameObject();
        pot.name = "Pot";
        pot.layer = LayerMask.NameToLayer("HouseFeature");
        pot.transform.parent = transform;
        pot.transform.position = transform.position;
        MeshFilter filter = pot.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pot.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Terracotta") as Material;
        Mesh mesh = filter.mesh;
        mesh.Clear();
        
        float topRadius2 = potThickness;

        float bottomRadius1 = topRadius1 * bottomRadiusReduce;
        float bottomRadius2 = potThickness;


        int nbVerticesCap = nbSides * 2 + 2;
        int nbVerticesSides = nbSides * 2 + 2;
        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
        int vert = 0;
        float _2pi = Mathf.PI * 2f;

        // Bottom cap
        int sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0f, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0f, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vert += 2;

            /*
            Vector3 soilPoint = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            soilPoint += transform.position;
            soilPoints.Add(soilPoint);
            */

        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vert += 2;
        }
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert < nbVerticesCap)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert < nbVerticesCap * 2)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        vert = 0;
        // Bottom cap
        sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }
        #endregion

        #region Triangles
        int nbFace = nbSides * 4;
        int nbTriangles = nbFace * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        // Bottom cap
        int i = 0;
        sideCounter = 0;
        while (sideCounter < nbSides)
        {
            int current = sideCounter * 2;
            int next = sideCounter * 2 + 2;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }

        // Top cap
        while (sideCounter < nbSides * 2)
        {
            int current = sideCounter * 2 + 2;
            int next = sideCounter * 2 + 4;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }

        // Sides (out)
        while (sideCounter < nbSides * 3)
        {
            int current = sideCounter * 2 + 4;
            int next = sideCounter * 2 + 6;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }


        // Sides (in)
        while (sideCounter < nbSides * 4)
        {
            int current = sideCounter * 2 + 6;
            int next = sideCounter * 2 + 8;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }
        #endregion

        //if we want to scale on the x, we need to spin the vertices a bit
        #region SpinVertices
        for (int j = 0; j < vertices.Length; j++)
        {
            vertices[j] = Quaternion.Euler(0f, 45f, 0f) * vertices[j];
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        ;


        /*
        //start plant building 
        Soil(potThickness, stretch);
   */

        if (addMeshCollider)
            pot.AddComponent<BoxCollider>();

        return pot;
    }


    void Tube()
    {
        GameObject pot = new GameObject();
        pot.name = "Pot";
        pot.transform.parent = transform;
        pot.transform.position = transform.position;
        MeshFilter filter = pot.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = pot.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Terracotta") as Material;
        Mesh mesh = filter.mesh;
        mesh.Clear();
        
        height = Random.Range(tubeHeightMin, tubeHeightMax);

        int nbSides = Random.Range(tubeSidesMin, tubeSidesMax);
        if (makeRectangle)
            nbSides = 4;

        potThickness = Random.Range(potThicknessMin, potThicknessMax);
        float bottomRadiusReduce = Random.Range(bottomRadiusReduceMin, bottomRadiusReduceMax);
        // Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2
        topRadius1 = Random.Range(topRadiusMin, topRadiusMax);
        float topRadius2 = potThickness;
        
        float bottomRadius1 = topRadius1 * bottomRadiusReduce;
        float bottomRadius2 = potThickness;
        

        int nbVerticesCap = nbSides * 2 + 2;
        int nbVerticesSides = nbSides * 2 + 2;
        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
        int vert = 0;
        float _2pi = Mathf.PI * 2f;

        // Bottom cap
        int sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0f, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0f, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);
            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vert += 2;

            Vector3 soilPoint = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            soilPoint += transform.position;
            soilPoints.Add(soilPoint);
            
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 + topRadius2 * .5f), height, sin * (topRadius1 + topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 + bottomRadius2 * .5f), 0, sin * (bottomRadius1 + bottomRadius2 * .5f));
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;
            float cos = Mathf.Cos(r1);
            float sin = Mathf.Sin(r1);

            vertices[vert] = new Vector3(cos * (topRadius1 - topRadius2 * .5f), height, sin * (topRadius1 - topRadius2 * .5f));
            vertices[vert + 1] = new Vector3(cos * (bottomRadius1 - bottomRadius2 * .5f), 0, sin * (bottomRadius1 - bottomRadius2 * .5f));
            vert += 2;
        }
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert < nbVerticesCap)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert < nbVerticesCap * 2)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            sideCounter = sideCounter == nbSides ? 0 : sideCounter;

            float r1 = (float)(sideCounter++) / nbSides * _2pi;

            normales[vert] = -(new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1)));
            normales[vert + 1] = normales[vert];
            vert += 2;
        }
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        vert = 0;
        // Bottom cap
        sideCounter = 0;
        while (vert < nbVerticesCap)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Top cap
        sideCounter = 0;
        while (vert < nbVerticesCap * 2)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(0f, t);
            uvs[vert++] = new Vector2(1f, t);
        }

        // Sides (out)
        sideCounter = 0;
        while (vert < nbVerticesCap * 2 + nbVerticesSides)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }

        // Sides (in)
        sideCounter = 0;
        while (vert < vertices.Length)
        {
            float t = (float)(sideCounter++) / nbSides;
            uvs[vert++] = new Vector2(t, 0f);
            uvs[vert++] = new Vector2(t, 1f);
        }
        #endregion

        #region Triangles
        int nbFace = nbSides * 4;
        int nbTriangles = nbFace * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        // Bottom cap
        int i = 0;
        sideCounter = 0;
        while (sideCounter < nbSides)
        {
            int current = sideCounter * 2;
            int next = sideCounter * 2 + 2;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }

        // Top cap
        while (sideCounter < nbSides * 2)
        {
            int current = sideCounter * 2 + 2;
            int next = sideCounter * 2 + 4;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }

        // Sides (out)
        while (sideCounter < nbSides * 3)
        {
            int current = sideCounter * 2 + 4;
            int next = sideCounter * 2 + 6;

            triangles[i++] = current;
            triangles[i++] = next;
            triangles[i++] = next + 1;

            triangles[i++] = current;
            triangles[i++] = next + 1;
            triangles[i++] = current + 1;

            sideCounter++;
        }


        // Sides (in)
        while (sideCounter < nbSides * 4)
        {
            int current = sideCounter * 2 + 6;
            int next = sideCounter * 2 + 8;

            triangles[i++] = next + 1;
            triangles[i++] = next;
            triangles[i++] = current;

            triangles[i++] = current + 1;
            triangles[i++] = next + 1;
            triangles[i++] = current;

            sideCounter++;
        }
        #endregion

        //if we want to scale on the x, we need to spin the vertices a bit
        #region SpinVertices
        for(int j = 0; j < vertices.Length; j++)
        {
            vertices[j] = Quaternion.Euler(0f, 45f, 0f)* vertices[j];
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        ;

        //stretch the pot if we want a rectangle
        Vector3 stretch = Vector3.zero;
        if(makeRectangle)
        {
            stretch = new Vector3(Random.Range(2f, 5f), 0f, 0f);
            pot.transform.localScale += stretch;
        }

        //start plant building 
        Soil(potThickness,stretch);
        /*
        if(tree)
            Tree(topRadius1, height);

        else if(roses)
        {
            PottedPlantController ppc = gameObject.GetComponent<PottedPlantController>();
           
            ppc.roses = true;
            ppc.enabled = true;
        }
        */

    }
    void Soil(float potThickness,Vector3 stretch)
    {
        GameObject soil = new GameObject();
        soil.name = "Soil";
        soil.transform.parent = transform;
        soil.transform.position = transform.position;

        Mesh mesh = new Mesh();

        Vector3 middle = FindCentralPointByAverage(soilPoints.ToArray());

        List<Vector3> soilPointsWithMiddle = new List<Vector3>();

      //  Debug.Log(soilPoints.Count);
        soilPointsWithMiddle.Add(middle - transform.position);

        foreach (Vector3 v3 in soilPoints)
            soilPointsWithMiddle.Add(v3 - transform.position);

        //if we want to scale on the x, we need to spin the vertices a bit
        #region SpinVertices
        for (int j = 0; j < soilPointsWithMiddle.Count; j++)
        {
            soilPointsWithMiddle[j] = Quaternion.Euler(0f, 45f, 0f) * soilPointsWithMiddle[j];
        }
        #endregion

        mesh.vertices = soilPointsWithMiddle.ToArray();

        List<int> tris = new List<int>();
        //create triangles using center point as the anchor for each triangle
        for(int i = 0; i < soilPointsWithMiddle.Count-1; i++)
        {
            if (i == 0)
                continue;

            tris.Add(0);
            tris.Add(i + 1);
            tris.Add(i);
            
        }

        mesh.triangles = tris.ToArray();

        MeshFilter meshFilter = soil.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = soil.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Brown") as Material;

        //move down off the edge slightly
        Vector3 newPos = soil.transform.position;
        newPos.y -= potThickness/2;
        soil.transform.position = newPos;

        //stretch 
        soil.transform.localScale += stretch;

    }
    void Tree(float radius, float height)
    {
        Trunk(radius, height);
        BushyBits(radius, height);
    }

    void Trunk(float radius,float height)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        float x = radius/2;
        float y = height/2;        
        float z = radius/2;
        Vector3 size = new Vector3(x, y, z);

        trunk.transform.localScale = size;

        trunk.transform.parent = transform;
        Vector3 pos = transform.position;
        pos.y += height;// * 0.5f;
        trunk.transform.position = pos;

        trunk.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load("Brown") as Material;
    }

    void BushyBits(float radius, float height)
    {
        int amount = Random.RandomRange(1, 4);
        GameObject ico = Resources.Load("Prefabs/Flora/IcoPrefab") as GameObject;
        
        for (int i = 0; i <= amount; i++)
        {
            if (i == 0)
                continue;

            ico.transform.localScale = Vector3.one;
            ico.transform.localScale *= radius;

            bool stretched = false;
            //sometimes stretch bush if there is only one
            if (amount == 1)
            {
                if (Random.Range(0, 2) == 0)
                {
                    Vector3 size = ico.transform.localScale;
                    size.y *= 2;
                    ico.transform.localScale = size;

                    stretched = true;
                }
            }

            Vector3 position = transform.position;
            
            position.y += (height*1.25f) + (radius*i);

            if (stretched)
                position.y += radius;

            if (i != 1)
            {                //randomise positiona bit//do not do first one
                position.x += Random.Range(-0.05f, 0.05f);
                //  position.y *= Random.Range(0.9f, 1.1f);
                position.z += Random.Range(-0.05f, 0.05f);
            }

            GameObject bushyBit = Instantiate(ico, position, Quaternion.identity) as GameObject;
            bushyBit.transform.parent = transform;
        }
        
    }

        // Update is called once per frame
        void Update () {
	
	}
    Vector3 FindCentralPointByAverage(Vector3[] mVertices)
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
