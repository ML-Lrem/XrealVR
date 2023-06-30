using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSeting : MonoBehaviour
{
    public int horizontalSegments = 50;
    public int verticalSegments = 50;
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;

            if (mesh != null)
            {
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                int numVertices = (horizontalSegments + 1) * (verticalSegments + 1);
                Vector3[] vertices = new Vector3[numVertices];
                Vector2[] uv = new Vector2[numVertices];
                int[] triangles = new int[(horizontalSegments * verticalSegments * 6)];

                int vertexIndex = 0;
                int triangleIndex = 0;

                for (int i = 0; i <= verticalSegments; i++)
                {
                    float v = (float)i / verticalSegments;

                    for (int j = 0; j <= horizontalSegments; j++)
                    {
                        float u = (float)j / horizontalSegments;

                        float theta = u * Mathf.PI * 2.0f;
                        float phi = (1.0f - v) * Mathf.PI;

                        float x = Mathf.Sin(theta) * Mathf.Sin(phi);
                        float y = Mathf.Cos(phi);
                        float z = Mathf.Cos(theta) * Mathf.Sin(phi);

                        vertices[vertexIndex] = new Vector3(x, y, z);
                        uv[vertexIndex] = new Vector2(u, v);

                        if (j < horizontalSegments && i < verticalSegments)
                        {
                            int index0 = vertexIndex;
                            int index1 = vertexIndex + 1;
                            int index2 = vertexIndex + horizontalSegments + 1;
                            int index3 = vertexIndex + horizontalSegments + 2;

                            triangles[triangleIndex++] = index0;
                            triangles[triangleIndex++] = index1;
                            triangles[triangleIndex++] = index2;

                            triangles[triangleIndex++] = index1;
                            triangles[triangleIndex++] = index3;
                            triangles[triangleIndex++] = index2;
                        }

                        vertexIndex++;
                    }
                }

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
        }
    } 

    // Update is called once per frame
    void Update()
    {
        
    }
}
