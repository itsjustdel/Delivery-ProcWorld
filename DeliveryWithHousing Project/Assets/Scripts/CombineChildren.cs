using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour {
	public bool receiveShadows = true;
	public bool castShadows = true;
    public bool disableColliders = false;
    public bool addAutoWeld = false;
    public bool addMeshCollider = false;
    public bool tagAsField = false;
    public bool addFindEdges = false;
    public bool renderer = false;
    public bool houseCell = false;
    public bool postOffice = false;
    public bool gardenCentre = false;
    public bool fieldCell = false;
    public bool ignoreDisabledRenderers = false;
    public bool reAlignCell;
    public bool addLod = false;
    
    public int LodLevels = 4;//always

	void Start()
	{
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (var meshRenderer in meshRenderers)
		{
			foreach (var material in meshRenderer.sharedMaterials)
				if (material != null && !combines.ContainsKey(material))
					combines.Add(material, new List<CombineInstance>());
		}
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		foreach(var filter in meshFilters)
		{
			if (filter.sharedMesh == null)
				continue;

            if (ignoreDisabledRenderers)
            {
                if(filter.GetComponent<MeshRenderer>() != null)
                    if (filter.GetComponent<MeshRenderer>().enabled == false)
                        continue;
            }

			CombineInstance ci = new CombineInstance();
			ci.mesh = filter.sharedMesh;
            
			ci.transform = myTransform * filter.transform.localToWorldMatrix;
            if (filter.GetComponent<MeshRenderer>() != null)
                combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
            if (filter.GetComponent<MeshRenderer>() != null)
                if (!renderer)
			        filter.GetComponent<Renderer>().enabled = false;

            //if the public bool is true, disable the box colliders
            if (disableColliders)
                filter.GetComponent<BoxCollider>().enabled = false;
        }
		
		foreach(Material m in combines.Keys)
		{
			var go = new GameObject("Combined mesh");

            if (tagAsField)
                go.tag = "Field";
            else
			    go.tag = "CombinedMesh";

			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			
			var filter = go.AddComponent<MeshFilter>();
			filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
			
			var renderer = go.AddComponent<MeshRenderer>();
			renderer.material = m;


            StartCoroutine("AddToGo", go);
		
        }
		//this.gameObject.AddComponent<MeshControl>();
	}

    IEnumerator AddToGo(GameObject go)
    {
        yield return new WaitForEndOfFrame();

        var renderer = go.GetComponent<MeshRenderer>();

        if (!receiveShadows)
            renderer.receiveShadows = false;

        if (!castShadows)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        if (addAutoWeld)
        {
            AutoWeld aw = go.AddComponent<AutoWeld>();
            aw.addFindEdges = true;
        }

        if (addFindEdges)
        {
            FindEdges findEdges = go.AddComponent<FindEdges>();
            findEdges.addBushesForCell = true;

        }

        //realign before adding mesh collider
        if (reAlignCell)
        {
            Realign();
        }

        if (addMeshCollider)
            go.AddComponent<MeshCollider>();

        if (houseCell)
        {
            go.tag = "HouseCell";
           go.layer = 28;
          //  go.AddComponent<ReAlign>();
            AutoWeld autoWeld = go.AddComponent<AutoWeld>();
            FindEdges fe = go.AddComponent<FindEdges>();
 //           fe.houseCell = true;
            yield return new WaitForEndOfFrame();
            go.AddComponent<MeshCollider>();//not applying properly? -- doing at the start of stretch quads/houseplacement

            yield return new WaitForEndOfFrame();
            
            HouseV2 hV2 = go.AddComponent<HouseV2>();
            hV2.enabled = false;
            //go.AddComponent<HouseQuests>();
            //test -- performance issues
           // go.AddComponent<GridInsidePolygon>(); --adds grass material atm
        }

        if (postOffice)
        {
            go.tag = "PostOffice";
            go.layer = 28;
            //  go.AddComponent<ReAlign>();
            AutoWeld autoWeld = go.AddComponent<AutoWeld>();
            FindEdges fe = go.AddComponent<FindEdges>();
            //           fe.houseCell = true;
            yield return new WaitForEndOfFrame();
            go.AddComponent<MeshCollider>();//not applying properly? -- doing at the start of stretch quads/houseplacement

            yield return new WaitForEndOfFrame();
            //add script whihc creates mail
            go.AddComponent<PostOffice>();
            //script which describes

            //test -- performance issues
            // go.AddComponent<GridInsidePolygon>(); --adds grass material atm
        }

        if (fieldCell)
        {
            AutoWeld autoWeld = go.AddComponent<AutoWeld>();
            go.tag = "Field";

            yield return new WaitForEndOfFrame();
            go.AddComponent<MeshCollider>();

            //this could be added by van trigger
            go.AddComponent<FindEdges>();

            go.AddComponent<FieldManager>();

            //splits field up in to grids so raycasting isnt as intensive for planting crops
            go.AddComponent<FieldGrids>().enabled = false;

            //Field layer
            go.layer = 12;
            
        }

        if(gardenCentre)
        {
            go.layer = 28;
            go.AddComponent<GardenCentre>();

        }
        
        if(addLod)
        {

            // Programmatically create a LOD group and add LOD levels.
            // Create a GUI that allows for forcing a specific LOD level.
            
            go.name = "LOD Parent";


            //make a list of these meshes 
            List<MeshFilter> meshFilters = new List<MeshFilter>();
            //first on is rendering object/filter
            meshFilters.Add(go.GetComponent<MeshFilter>());

            //save high poly in object - do we really need objects? I guess it lets me check meshes in inspector
            GameObject lod0 = new GameObject();
            lod0.transform.position = go.transform.position;
            lod0.transform.parent = go.transform;
            lod0.name = "LOD0";
            MeshFilter mf0 = lod0.AddComponent<MeshFilter>();
            mf0.mesh = (Mesh)Instantiate(go.GetComponent<MeshFilter>().mesh);
            //add high poly combined mesh
            meshFilters.Add(mf0);

           
            if (go.transform.parent.name == ("Interior Parent"))
            {
                //no mesh, we will hide at first LOD level
                Mesh medPolyMesh = new Mesh();
                GameObject lod1 = new GameObject();
                lod1.transform.position = go.transform.position;
                lod1.transform.parent = go.transform;
                lod1.name = "LOD1";
                MeshFilter mf1 = lod1.AddComponent<MeshFilter>();
                mf1.mesh = medPolyMesh;
                //add low poly welded mesh
                meshFilters.Add(mf1);

                //no mesh atm - find way to decimate efficiently
                Mesh lowPolyMesh = new Mesh();
                GameObject lod2 = new GameObject();
                lod2.transform.position = go.transform.position;
                lod2.transform.parent = go.transform;
                lod2.name = "LOD2";
                MeshFilter mf2 = lod2.AddComponent<MeshFilter>();
                mf2.mesh = lowPolyMesh;

                //add low poly welded mesh
                meshFilters.Add(mf1);
            }
            if (go.transform.parent.name == ("Exterior Parent"))
            {

                GameObject lod1 = new GameObject();
                lod1.transform.position = go.transform.position;
                lod1.transform.parent = go.transform;
                lod1.name = "LOD1";
                MeshFilter mf1 = lod1.AddComponent<MeshFilter>();
                mf1.mesh = meshFilters[meshFilters.Count-1].mesh;
                //keep mesh the sma eon level2
                meshFilters.Add(mf1);

                //still on high mesh
                GameObject lod2 = new GameObject();
                lod2.transform.position = go.transform.position;
                lod2.transform.parent = go.transform;
                lod2.name = "LOD2";
                MeshFilter mf2 = lod2.AddComponent<MeshFilter>();
                mf2.mesh = meshFilters[meshFilters.Count - 1].mesh;
                meshFilters.Add(mf2);
            }
            if (go.transform.parent.name == ("Garden Tree Meshes"))
            {
                //keep same mesh
                GameObject lod1 = new GameObject();
                lod1.transform.position = go.transform.position;
                lod1.transform.parent = go.transform;
                lod1.name = "LOD1";
                MeshFilter mf1 = lod1.AddComponent<MeshFilter>();
                mf1.mesh = meshFilters[meshFilters.Count - 1].mesh;
                meshFilters.Add(mf1);

                //no mesh atm - find way to decimate efficiently
                Mesh lowPolyMesh = new Mesh();
                GameObject lod2 = new GameObject();
                lod2.transform.position = go.transform.position;
                lod2.transform.parent = go.transform;
                lod2.name = "LOD2";
                MeshFilter mf2 = lod2.AddComponent<MeshFilter>();
                mf2.mesh = lowPolyMesh;

                //add low poly welded mesh
                meshFilters.Add(mf1);
            }
            
            //add empty for all

            //no mesh - vanish on last lod
            Mesh noMesh = new Mesh();
            GameObject lod3 = new GameObject();
            lod3.transform.position = go.transform.position;
            lod3.transform.parent = go.transform;
            lod3.name = "LOD3";
            MeshFilter mf3 = lod3.AddComponent<MeshFilter>();
            mf3.mesh = noMesh;

            //add empty
            meshFilters.Add(mf3);
            





            //now add this list and the meshfilter to LODswitcher list - this will control when to switch between them
            LODSwitcher lodSwitcher = GameObject.FindWithTag("Code").GetComponent<LODSwitcher>();
            lodSwitcher.meshFilters.Add(meshFilters);
            
            


            //mf1.mesh = mesh1;
            //mf1.mesh = go.transform.GetComponent<MeshFilter>().mesh;

            //Renderer[] renderers = new Renderer[] { go.GetComponent<MeshRenderer>()};
            // Add 2 LOD levels
            //LOD[] lods = new LOD[2];
            //first level
            //lods[0] = new LOD(1.0f / 2, renderers ) ;
            //second
            //renderers = new Renderer[] { mr1};
            //lods[1] = new LOD(1.0f/3, renderers);//50%?

            //group.SetLODs(lods);
            //group.RecalculateBounds();
        }

        yield break;
    }

    void Realign()
    {

        //makes the transform position the centre of the mesh and moves the mesh vertices so the stay the same in world space
        Mesh mesh = transform.Find("Combined mesh").GetComponent<MeshFilter>().mesh;

        //find the Y offset


        transform.position = mesh.bounds.center;

        Vector3[] verts = mesh.vertices;
        List<Vector3> vertsList = new List<Vector3>();

        for (int i = 0; i < verts.Length; i++)
        {
            vertsList.Add(verts[i] - transform.position);
        }



        mesh.vertices = vertsList.ToArray();



    }
}