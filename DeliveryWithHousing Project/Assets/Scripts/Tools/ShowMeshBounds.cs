using UnityEngine;
using System.Collections;

public class ShowMeshBounds : MonoBehaviour
{
    private Transform m_MeshBounds;
    private Transform m_RendererBounds;
    private Mesh m_Mesh;
    private Renderer m_Renderer;
    void Start()
    {
        Material myMat1 = new Material(Shader.Find("Transparent/Diffuse"));
        myMat1.color = new Color(1, 0, 0, 0.2f);
        Material myMat2 = new Material(myMat1);
        myMat2.color = new Color(0, 1, 0, 0.2f);

        m_MeshBounds = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        m_MeshBounds.parent = transform;
        m_MeshBounds.localPosition = Vector3.zero;
        m_MeshBounds.localEulerAngles = Vector3.zero;
        m_RendererBounds = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        m_RendererBounds.localEulerAngles = Vector3.zero;

        m_MeshBounds.gameObject.GetComponent<MeshRenderer>().material = myMat1;
        m_RendererBounds.gameObject.GetComponent<MeshRenderer>().material = myMat2;

        MeshFilter MF = GetComponent<MeshFilter>();
        if (MF != null)
            m_Mesh = MF.mesh;
        SkinnedMeshRenderer SMR = GetComponent<SkinnedMeshRenderer>();
        if (SMR != null)
            m_Mesh = SMR.sharedMesh;
        m_Renderer = GetComponent<Renderer>();
    }

    void LateUpdate()
    {
        Bounds meshB = m_Mesh.bounds;
        Bounds rendererB = m_Renderer.bounds;
        m_MeshBounds.localPosition = meshB.center;
        m_MeshBounds.localScale = meshB.size;

        m_RendererBounds.position = rendererB.center;
        m_RendererBounds.localScale = rendererB.size;

    }
}