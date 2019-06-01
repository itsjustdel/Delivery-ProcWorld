using UnityEngine;
using System.Collections;

public class MaterialSetup : MonoBehaviour {

    public Material[] flowerMaterials;
    public Material[] leavesMaterials;

    // Use this for initialization
    void Start ()
    {
        flowerMaterials = Resources.LoadAll<Material>("FlowerColours/Flowers");
        leavesMaterials = Resources.LoadAll<Material>("FlowerColours/Leaves");
    }
	
}
