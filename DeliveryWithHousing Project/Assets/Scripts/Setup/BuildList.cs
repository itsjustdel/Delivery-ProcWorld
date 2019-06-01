using UnityEngine;

using System.Collections;
using System.Collections.Generic;




/// <summary>
/// Receives orders to build and exectutes them one at a time
/// </summary>
public class BuildList : MonoBehaviour {

    public bool building = false;
    public List<GameObject> objects = new List<GameObject>();
    public List<Component> components = new List<Component>();
    
    private float updateinterval = 1F;
    private int frames = 0;
    private float timeleft = 1F;
    private int FPS = 60;

    public int buildObjectCount = 0;
    public int prev;
    public int completed = 0;
    //public bool finishedList = true;
    public int currentlyBuilding = 0;
    public int maxBuildAmount = 10;
    public bool run = true;
    // Use this for initialization
    void Start () {

       // enabled = false;
        // BuildObjectList();
        StartCoroutine("UpdateCoroutine");
    }

    public float deltaTime = 0.0f;
    public float fps;
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

         if (components.Count > 0 && building == false)
            {
                CallToBuild();
                building = true;
            }
    }

    IEnumerator UpdateCoroutine()
    {

        if (fps > 30)
        {

            if (components.Count > 0 && building == false)
            {
                CallToBuild();
                building = true;
            }
        }

        if (run)
        {
            yield return new WaitForSeconds(0.01f);
            
            StartCoroutine("UpdateCoroutine");
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);

        rect = new Rect(0, 20, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        int count = components.Count;
        text = "Build Count " + count;
        //text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, count);
        GUI.Label(rect, text, style);
    }

    public void CallToBuild()
    {
        //build field crops first
        if(components[0] is FieldPattern)
        {
            components[0].gameObject.GetComponent<FieldPattern>().enabled = true;
        }
        else if(components[0] is FenceAroundCell)
        {
            components[0].gameObject.GetComponent<FenceAroundCell>().enabled = true;
        }
        else if (components[0] is BushesForCell)
        {
            components[0].gameObject.GetComponent<BushesForCell>().enabled = true;
        }
        else if(components[0] is HouseV2)
        {
            components[0].gameObject.GetComponent<HouseV2>().enabled = true;
        }
        else if (components[0] is QuickTree)
        {
            components[0].gameObject.GetComponent<QuickTree>().BuildTree();
        }
        else if (components[0] is BatchPlanter)
        {
            components[0].gameObject.GetComponent<BatchPlanter>().enabled = true;
        }
        else if (components[0] is FieldGrids)
        {
            components[0].gameObject.GetComponent<FieldGrids>().enabled = true;
        }
        else if (components[0] is FieldGridPlanter)
        {
            components[0].gameObject.GetComponent<FieldGridPlanter>().enabled = true;
        }
        
    }

    public void BuildingFinished()
    {

        building = false;
        // buildObjectList.RemoveAt(0);       
       // if (components.Count > 0)//if something is returning, there must be an object in the list
            components.RemoveAt(0);

        //StartCoroutine("Wait");
    
    }

    public void BuildingFinished(bool doNotWaitForFrame,bool destroy)
    {
        building = false;
        GameObject toDestroy = components[0].gameObject;
        // buildObjectList.RemoveAt(0);       
        if (components.Count > 0)
            components.RemoveAt(0);

        if(destroy)
        {
            Destroy(toDestroy);
        }
    }
    IEnumerator Wait()
    {
        //yield return new WaitForEndOfFrame();

        building = false;
        // buildObjectList.RemoveAt(0);       
        if(components.Count>0)
            components.RemoveAt(0);
        yield break;
        //yield return new WaitForEndOfFrame();
    }

}
