﻿using UnityEngine;
using System.Collections;

public class PerlinNoisePlane : MonoBehaviour
{
    public float power = 3.0f;
    public float scale = 1.0f;
    public Vector2 v2SampleStart = new Vector2(0f, 0f);

    void Start()
    {
        v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        MakeSomeNoise();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            v2SampleStart = new Vector2(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
            MakeSomeNoise();
            
        }
    }

    void MakeSomeNoise()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Vector3[] vertices = mf.mesh.vertices;
       // Debug.Log(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            float xCoord = v2SampleStart.x + vertices[i].x * scale;
            float yCoord = v2SampleStart.y + vertices[i].z * scale;
            vertices[i].y += (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * power;
          //  Debug.Log(vertices[i].y);
        }
        mf.mesh.vertices = vertices;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateNormals();

         Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }
}
