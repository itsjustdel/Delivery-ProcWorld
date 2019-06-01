using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BranchArray : MonoBehaviour {

    public GameObject materials;
    public Material[] greens;
    
    public List<Mesh> branches = new List<Mesh>();//shrub
    public List<Mesh> gardenTreeBranches = new List<Mesh>();
    public List<List<Mesh>> leaves= new List<List<Mesh>>();
    public List<List<Mesh>> gardenTreeLeaves = new List<List<Mesh>>();
    public List<Vector3> roses = new List<Vector3>();

    //cabbages
    public List<List<Mesh>> cabbageLeaves = new List<List<Mesh>>();
    public List<Mesh> cabbageHeads = new List<Mesh>();


    public bool makeShrub;
    public int stemsPerShrub = 20;
    public int stemsPerTree = 5;
    public int stemPrefabAmount = 100;
    public int cabbages = 100;
    
	// Use this for initialization
	void Start ()
    {
        //grab material array
        greens = materials.GetComponent<Materials>().myMaterials;
        
        GameObject shrubPrefabs = new GameObject();
        shrubPrefabs.transform.parent = transform;

        //build arrays for trees and shrubs
	    for(int i = 0; i < stemPrefabAmount; i++)
        {
            PlantController pc = shrubPrefabs.AddComponent<PlantController>();
            pc.shrub = true;
        }

       for(int i = 0; i < stemPrefabAmount; i++)
        {
            PlantController pc = shrubPrefabs.AddComponent<PlantController>();
            pc.gardenTree = true;
        }

        Cabbages();
    }

    public void Cabbages()
    {
        for (int i = 0; i < cabbages; i++)
        {
            CreateCabbageLeafArray();

        }

        //cabbbage head's are hard going, we do we need that many? ratio of 10:1 atm
        for (int i = 0; i < cabbages/10; i++)
        {
            CreateCabbageHeadArray();
        }
    }
    void CreateCabbageLeafArray()
    {
        //use leaves class to creat a leaf for us

        //if cabbage

        //create vector3 for vertices
        float variance = 0.1f;
        float standardSize = 0.4f;
        float radius = Random.Range(standardSize - variance, standardSize + variance);

        float xStretch = 1f * Random.Range(1f - radius, 1f + radius);

        radius = Random.Range(standardSize - variance, standardSize + variance);//too much?

        float zStretch = 1f * Random.Range(1f - radius, 1f + radius);

        int leafDetail = 8;//needs to be an even number when divided by 2

        List<Vector3> points = Leaves.OutlineForArray(xStretch, zStretch, radius, leafDetail);
        //bends leaves (?)
        points = Leaves.WrapForArray(points);

        //create front and back mesh for leaf
        List<Mesh> meshes = Leaves.MeshForArray(points);

        //add this leaf to the leaf List
        cabbageLeaves.Add(meshes);

    }
    void CreateCabbageHeadArray()
    {
        //how to decide how randomised? //join up with radius on leaf? //

        float variance = 0.05f;
        float standardSize = 0.2f;
        float headSize = Random.Range(standardSize - variance, standardSize + variance);

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
            vertices[i].y += headSize * 1.5f;
        }






        mesh.vertices = vertices;
        mesh.triangles = meshPrefab.triangles;
        mesh.RecalculateNormals();

        //add to list for placing later
        cabbageHeads.Add(mesh);
    }

    // Update is called once per frame
    void Update () {

        if (makeShrub)
        {
            makeShrub = false;
            for (int s = 0; s < 5; s++)
            {
                MakeShrub(gameObject,Vector3.right *s*5,false);
            }
            
        }
	}

    public void MakeShrub(GameObject parent, Vector3 position,bool batch)
    {

        if (branches.Count < stemsPerShrub)
        {
            Debug.Log("Not enough stem prefabs");
        }

        GameObject shrub = new GameObject();
        shrub.name = "Shrub Meshes";
        shrub.transform.parent = parent.transform;
        shrub.transform.position = position;        
        
        for (int i = 0; i < stemsPerShrub; i++)
        {
            if (branches.Count == 0)
            {
               // Debug.Log("retunrding from branches == 0");
                return;
            }

            Quaternion randomRot = Random.rotation;
            randomRot = Quaternion.Euler(0f, randomRot.y, 0f);

            GameObject stem = new GameObject();
            stem.name = "stem";
            stem.transform.parent = shrub.transform;
            stem.transform.position = shrub.transform.position;
            stem.transform.rotation = randomRot;
            MeshRenderer meshRenderer = stem.AddComponent<MeshRenderer>();
            
            meshRenderer.sharedMaterial = Resources.Load("Brown") as Material;
            MeshFilter meshFilter = stem.AddComponent<MeshFilter>();

            int random = Random.Range(0, stemPrefabAmount);
            //meshFilter.mesh = branches[random];

            List<Mesh> leavesMeshes = leaves[random];
            //Debug.Log("Leaves mesh count = " + leavesMeshes.Count);

            foreach (Mesh lMesh in leavesMeshes)
            {
                GameObject leavesObj = new GameObject();
                leavesObj.transform.parent = shrub.transform;
                leavesObj.transform.position = shrub.transform.position;
                leavesObj.transform.rotation = randomRot;
                leavesObj.name = "Leaves";
                MeshRenderer meshRendererLeaves = leavesObj.AddComponent<MeshRenderer>();
                Material mat = greens[Random.Range(0, greens.Length)];
                meshRendererLeaves.sharedMaterial = mat;
                MeshFilter meshFilterLeaves = leavesObj.AddComponent<MeshFilter>();

                meshFilterLeaves.sharedMesh = lMesh;
            }
        }

        shrub.transform.localScale *= Random.Range(0.05f, 0.1f); //meshes are created rather large

       
            CombineChildren cc = shrub.AddComponent<CombineChildren>();
        //cc.receiveShadows = false;
        //cc.castShadows = false;
        if (batch)
        {
            StartCoroutine("Batch", shrub);
        }
    }

    public IEnumerator MakeGardenTree(GameObject parent, Vector3 position)
    {
       // Debug.Log("Starting tree");
        if (branches.Count < stemsPerTree)
        {
            Debug.Log("Not enough stem prefabs");
        }

        GameObject shrub = new GameObject();
        shrub.name = "Garden Tree Meshes";
        shrub.transform.parent = parent.transform;
        shrub.transform.position = position;
        
        for (int i = 0; i < stemsPerTree; i++)
        {
            if (branches.Count == 0)
                yield break;

            Quaternion randomRot = Random.rotation;
            randomRot = Quaternion.Euler(0f, randomRot.y, 0f);

            GameObject stem = new GameObject();
            stem.name = "stem";
            stem.transform.parent = shrub.transform;
            stem.transform.position = shrub.transform.position;
            stem.transform.rotation = randomRot;
            MeshRenderer meshRenderer = stem.AddComponent<MeshRenderer>();
            meshRenderer.enabled = false;

            meshRenderer.sharedMaterial = Resources.Load("Brown") as Material;
            MeshFilter meshFilter = stem.AddComponent<MeshFilter>();

            int random = Random.Range(0, stemPrefabAmount);
            meshFilter.mesh = gardenTreeBranches[random];


            

            List<Mesh> leavesList = gardenTreeLeaves[random];
            foreach (Mesh leavesMesh in leavesList)
            {
                GameObject leavesObj = new GameObject();
                leavesObj.transform.parent = shrub.transform;
                leavesObj.transform.position = shrub.transform.position;
                leavesObj.transform.rotation = randomRot;
                leavesObj.name = "Leaves";
                MeshRenderer meshRendererLeaves = leavesObj.AddComponent<MeshRenderer>();
                meshRendererLeaves.enabled = false;

                /////////********************///////////// unique mats?nooo
                Material mat = greens[Random.Range(0, greens.Length)];
                meshRendererLeaves.sharedMaterial = mat;
                MeshFilter meshFilterLeaves = leavesObj.AddComponent<MeshFilter>();
                meshFilterLeaves.sharedMesh = leavesMesh;
            }
            yield return new WaitForEndOfFrame();
        }

        
        CombineChildren cc = shrub.AddComponent<CombineChildren>();
        cc.receiveShadows = false;
        cc.castShadows = false;
        cc.addLod = true;

        //tell main build list we have finsihed this tree
        GameObject.FindWithTag("Code").GetComponent<BuildList>().BuildingFinished();
        yield break;
     //   StartCoroutine("Batch", shrub);

    }

    IEnumerator Batch(GameObject shrub)//shrub Meshes
    {
        //we need to wait for combine children to instantiate its meshes
        yield return new WaitForEndOfFrame();

        //check if all the shrubs in this batch have been completed
        GameObject batched = shrub.transform.parent.parent.gameObject;
        int childCount = shrub.transform.parent.parent.childCount;
       // Debug.Log(childCount);
        int built = 0;
        for(int i = 0; i < childCount; i++)
        {
            if (batched.transform.GetChild(i).childCount > 0)
                built++;
        }

        if (built == childCount)
        {
            //dont add more than one. can happen if frame lags
            if (batched.GetComponent<CombineChildren>() == null)
            {
                CombineChildren cc = batched.AddComponent<CombineChildren>();
                cc.ignoreDisabledRenderers = true;
            }
            //Debug.Log("built equals count");
        }

        yield break;
    }
    

    IEnumerator AddLeaves(List<Vector3> leavesPositions,GameObject parent)
    {
        // yield return new WaitForEndOfFrame();
        /*
        foreach (Vector3 v3 in leavesPositions)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = v3;
        }
        */
        //Debug.Log(leavesPositions.Count);
        //only using this for shrub at the moment- keeping framework for other plants in case..
        bool shrub = true;
        bool bonzai = false;
        bool roses = false;
        GameObject leaves = new GameObject();
        leaves.transform.parent = parent.transform;
        Vector3 pos = leaves.transform.position;
        //pos.y += (GetComponent<PlantPot>().height) - (GetComponent<PlantPot>().potThickness / 2);
        leaves.transform.localPosition = pos;
        leaves.name = "Leaves";

        //   GameObject leavesInstance = Instantiate(leafPrefab);

        Mesh newMesh = new Mesh();
        newMesh.name = "Leaves As One";

        Mesh leavesPrefabMesh = Resources.Load("Prefabs/Flora/LeafCombinedMeshCentral") as Mesh;
        List<Vector3> leavesVerts = new List<Vector3>();
        List<int> leavesTris = new List<int>();

        //create tempLists

        List<Vector3> tempVerts = new List<Vector3>();
        foreach (Vector3 v in leavesPrefabMesh.vertices)
            tempVerts.Add(v);

        List<int> tempInts = new List<int>();

        foreach (int i in leavesPrefabMesh.triangles)
            tempInts.Add(i);


        //add vertices from prefab to new mesh
        for (int i = 0; i < leavesPositions.Count; i++)  //minus count number stops leaves growing at bottom
        {
            //rotate (if)

            Quaternion q = Quaternion.identity;
            if (roses || bonzai)
            {
                q = Random.rotation; //Quaternion.Euler(0, Random.Range(80f, 100f), 0);//
            }

            if (shrub)
                q = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(0f, 360f), Random.Range(-10f, 10f));//

            List<Vector3> alteredVerts = new List<Vector3>();
            for (int j = 0; j < tempVerts.Count; j++)
            {
                Vector3 v = tempVerts[j];
                //scale
                if (bonzai)
                    v *= 0.25f; //segment length?pot size?
                //rotate
                v = q * v;
                alteredVerts.Add(v);
            }

            for (int j = 0; j < alteredVerts.Count; j++)
            {
                Vector3 position = alteredVerts[j] + leavesPositions[i];
                leavesVerts.Add(position);
            }
        }

        //add triangles

        //run through verts and add triangles for each prefab/combined mesh
        for (int i = 0; i < leavesPositions.Count; i++)
        {
            //foreach prefab mesh
            for (int j = 0; j < tempInts.Count; j++)
            {
                leavesTris.Add(tempInts[j] + (tempVerts.Count * i));
            }
        }


        //create mesh
        newMesh.vertices = leavesVerts.ToArray();
        newMesh.triangles = leavesTris.ToArray();
        newMesh.RecalculateNormals();
        MeshFilter meshFilter = leaves.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = newMesh;
        MeshRenderer meshRenderer = leaves.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Leaf") as Material;


        //AddRosesMaster();
        //Scale();
        // gameObject.AddComponent<CombineChildren>();

        yield break;
    }
    IEnumerator AddLeavesV2(List<Vector3> leavesPositions, GameObject parent)//cubes
    {
        // yield return new WaitForEndOfFrame();
        /*
        foreach (Vector3 v3 in leavesPositions)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = v3;
        }
        */
        //Debug.Log(leavesPositions.Count);
        //only using this for shrub at the moment- keeping framework for other plants in case..
        bool shrub = true;
        bool bonzai = false;
        bool roses = false;
        GameObject leaves = new GameObject();
        leaves.transform.parent = parent.transform;
        Vector3 pos = leaves.transform.position;
        //pos.y += (GetComponent<PlantPot>().height) - (GetComponent<PlantPot>().potThickness / 2);
        leaves.transform.localPosition = pos;
        leaves.name = "Leaves";

        //   GameObject leavesInstance = Instantiate(leafPrefab);

        Mesh newMesh = new Mesh();
        newMesh.name = "Leaves As One";

        GameObject leavesParent = Resources.Load("Prefabs/Flora/LeavesParent") as GameObject;
        //create tempLists
        List<Vector3> leavesVerts = new List<Vector3>();
        List<int> leavesTris = new List<int>();
        for (int x = 0; x < leavesParent.transform.childCount; x++)
        {

            //grab each cube in prefab
            Mesh leavesPrefabMesh = leavesParent.transform.GetChild(x).GetComponent<MeshFilter>().mesh;

           


            List<Vector3> tempVerts = new List<Vector3>();
            foreach (Vector3 v in leavesPrefabMesh.vertices)
                tempVerts.Add(v);

            List<int> tempInts = new List<int>();

            foreach (int i in leavesPrefabMesh.triangles)
                tempInts.Add(i);


            //add vertices from prefab to new mesh
            for (int i = 0; i < leavesPositions.Count; i++)  //minus count number stops leaves growing at bottom
            {
                //rotate (if)

                Quaternion q = Quaternion.identity;
                if (roses || bonzai)
                {
                    q = Random.rotation; //Quaternion.Euler(0, Random.Range(80f, 100f), 0);//
                }

                if (shrub)
                    q = Quaternion.Euler(Random.Range(-10f, 10f), Random.Range(0f, 360f), Random.Range(-10f, 10f));//

                List<Vector3> alteredVerts = new List<Vector3>();
                for (int j = 0; j < tempVerts.Count; j++)
                {
                    Vector3 v = tempVerts[j];
                    //scale
                    if (bonzai)
                        v *= 0.25f; //segment length?pot size?
                                    //rotate
                    v = q * v;
                    alteredVerts.Add(v);
                }

                for (int j = 0; j < alteredVerts.Count; j++)
                {
                    Vector3 position = alteredVerts[j] + leavesPositions[i];
                    leavesVerts.Add(position);
                }
            }

            //add triangles

            //run through verts and add triangles for each prefab/combined mesh
            for (int i = 0; i < leavesPositions.Count; i++)
            {
                //foreach prefab mesh
                for (int j = 0; j < tempInts.Count; j++)
                {
                    leavesTris.Add(tempInts[j] + (tempVerts.Count * i));
                }
            }
        }

        //create mesh
        newMesh.vertices = leavesVerts.ToArray();
        newMesh.triangles = leavesTris.ToArray();
        newMesh.RecalculateNormals();
        MeshFilter meshFilter = leaves.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = newMesh;
        MeshRenderer meshRenderer = leaves.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = Resources.Load("Leaf") as Material;


        //AddRosesMaster();
        //Scale();
        // gameObject.AddComponent<CombineChildren>();

        yield break;
    }
}
