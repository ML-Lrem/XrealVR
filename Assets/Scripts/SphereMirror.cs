using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMirror: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2[] vec2UVs = transform.GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < vec2UVs.Length; i++)
        {
            vec2UVs[i] = new Vector2(1.0f - vec2UVs[i].x, vec2UVs[i].y);
        }
        transform.GetComponent<MeshFilter>().mesh.uv = vec2UVs;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
