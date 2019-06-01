using UnityEngine;
using System.Collections;

public class GUIElements : MonoBehaviour {


    //http://answers.unity3d.com/questions/323195/how-can-i-have-a-static-class-i-can-access-from-an.html


    public static GUIElements GUIElement;


    //info for other scripts to grab gui elements quickly - asigned in prefab in inspector 
    
    public GameObject GUIParentObject;
    public GameObject textPanelObject;
    public GameObject backgroundPanelObject;
    public GameObject button1Object;
    public GameObject button2Object;
    public GameObject button3Object;    

    void Awake()
    {
        //create singleton

        if (GUIElement != null)
            GameObject.Destroy(GUIElement);
        else
            GUIElement = this;

       // DontDestroyOnLoad(this);
    }
}
