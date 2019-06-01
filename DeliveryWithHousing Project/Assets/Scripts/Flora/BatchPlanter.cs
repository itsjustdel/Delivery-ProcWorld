using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchPlanter : MonoBehaviour {

    //is atttached to an object in a field which has children at positions. This script plant given plant at hese position then combines them in to a ncie mesh to reduce draw calls

    private BranchArray branchArray;

    private void Awake()
    {
        //player will start this with trigger
        enabled = false;
    }

    // Use this for initialization
    public void Start () {


        

        
        GameObject cropParent = gameObject;
        MeshRenderer meshRenderer = cropParent.AddComponent<MeshRenderer>();

        //grab compnent which has the look up list for the generated meshes of cabbages/trees etc
        branchArray = GameObject.FindWithTag("Code").GetComponent<BranchArray>();

        //this only happens whenn testing and recomopiling whilst unity is playing
        if (branchArray.cabbageHeads.Count == 0)
            branchArray.Cabbages();

        Material[] mudArray = GameObject.FindGameObjectWithTag("Materials").GetComponent<Materials>().mudMaterials;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mudArray[Random.Range(0, mudArray.Length)];

        Material[] m = new Material[]
        {
           Resources.Load("Green") as Material,
           Resources.Load("Yellow") as Material
        };
        
        cropParent.transform.parent = transform;
        cropParent.transform.position = transform.position;
       

        MeshFilter meshFilter = cropParent.AddComponent<MeshFilter>();
        
        meshRenderer.sharedMaterials = m;

        Mesh parentMesh = new Mesh();
        List<Vector3> parentVertices = new List<Vector3>();
        List<int> parentTriangles = new List<int>();
        List<int> headTriangles = new List<int>();

        //run throught children, create a plant for each one
        for (int i = 0; i < transform.childCount; i++)
        {
            
           

            /*
            if (parentTriangles.Count > 4000 || i == transform.childCount - 1)// && i != 0)//i % batchSize == 0 //4000 is "sweet spot"
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
            */
            //create a mesh of one flower
            Mesh mesh = CreateCabbageLeaves(transform.GetChild(i).transform.position, cropParent);

            //add this flower's leaves mesh a points to parent

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            for (int t = 0; t < triangles.Length; t++)
            {
                parentTriangles.Add(parentVertices.Count + triangles[t]);
            }

            foreach (Vector3 v3 in vertices)
                parentVertices.Add((v3) + transform.GetChild(i).transform.localPosition);

            //add a head

            //add head to vertices

            Mesh headTempMesh = branchArray.cabbageHeads[Random.Range(0, branchArray.cabbageHeads.Count)];
            int[] headTempTriangles = headTempMesh.triangles;
            Vector3[] headTempVertices = headTempMesh.vertices;

            for (int h = 0; h < headTempTriangles.Length; h++)
            {
                headTriangles.Add(parentVertices.Count + headTempTriangles[h]);
            }

            foreach (Vector3 v3 in headTempVertices)
                parentVertices.Add(v3 + transform.GetChild(i).transform.localPosition);


           

        }
        //create and add mesh
        parentMesh.vertices = parentVertices.ToArray();
        //parentMesh.triangles = parentTriangles.ToArray();
        parentMesh.subMeshCount = 2;
        parentMesh.SetTriangles(parentTriangles, 0);
        parentMesh.SetTriangles(headTriangles, 1);

        parentMesh.RecalculateBounds();
        parentMesh.RecalculateNormals();
        meshFilter.mesh = parentMesh;

        

          //add to LOD switcher

        LODSwitcher lodSwitcher = GameObject.FindWithTag("Code").GetComponent<LODSwitcher>();
        List<MeshFilter> filters = new List<MeshFilter>();
        //add meshfilter that will be used to diplsay mesh
        filters.Add(meshFilter);
          
        //now create objects to hold emshes for diff levels of detail
        //LOD 0
        
        GameObject lod0= new GameObject();
        lod0.name = "LOD0";
        lod0.transform.parent = cropParent.transform;
        lod0.transform.position = transform.position;
        MeshFilter mf0 = lod0.AddComponent<MeshFilter>();
        mf0.mesh = parentMesh;
        filters.Add(mf0);
        //LOD 1
        GameObject lod1 = new GameObject();
        lod1.transform.parent = gameObject.transform;
        lod1.transform.position = transform.position;
        lod1.name = "LOD1";        
        Mesh noMesh = new Mesh();        
        MeshFilter mf1 = lod1.AddComponent<MeshFilter>();
        mf1.mesh = noMesh;
        filters.Add(mf1);

        GameObject lod2 = new GameObject();        
        lod2.transform.position = transform.position;
        lod2.transform.parent = transform;
        lod2.name = "LOD2";
        MeshFilter mf2 = lod2.AddComponent<MeshFilter>();
        noMesh = new Mesh();
        mf2.mesh = noMesh;
        filters.Add(mf2);

        GameObject lod3 = new GameObject();
        lod3.transform.position = transform.position;
        lod3.transform.parent = transform;
        lod3.name = "LOD3";
        MeshFilter mf3 = lod3.AddComponent<MeshFilter>();
        noMesh = new Mesh();
        mf3.mesh = noMesh;
        filters.Add(mf3);

        //add to lod
        lodSwitcher.meshFilters.Add(filters);

        //tell building pipeline we are finished -this is being called enabled and reported to buildlist by field grid planter atm
        //GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
    }


    Mesh CreateCabbageLeaves(Vector3 plantPosition, GameObject parent)
    {
        int leafNumber = Random.Range(4, 8);

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int random = Random.Range(0, branchArray.cabbageLeaves.Count);
        List<Mesh> meshes = branchArray.cabbageLeaves[random];

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
            for (int v = 0; v < tempVertices.Length; v++)
            {
                tempVertices[v] = rot * tempVertices[v];


                //add to main mesh list
                vertices.Add(tempVertices[v]);

            }


            // plant.transform.GetChild(0).rotation = rot; //leafParent.transform.rotation = rot;
        }


        mesh.SetVertices(vertices);
        //mesh.subMeshCount = 1;        
        mesh.triangles = triangles.ToArray();


        return mesh;
    }

}
