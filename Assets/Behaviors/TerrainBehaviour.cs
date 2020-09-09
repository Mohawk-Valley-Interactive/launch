using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainBehaviour : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] verts;
    private int[] tris;

    void Start()
    {
        mesh = new Mesh();

        verts = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, -0.5f), //LBN0
            new Vector3(0.0f, 1.0f, -0.5f), //LTN1
            new Vector3(1.0f, 1.0f, -0.5f), //RTN2
            new Vector3(1.0f, 0.0f, -0.5f), //RBN3
            new Vector3(0.0f, 0.0f, 0.5f), //LBF4
            new Vector3(0.0f, 1.0f, 0.5f), //LTF5
            new Vector3(1.0f, 1.0f, 0.5f), //RTF6
            new Vector3(1.0f, 0.0f, 0.5f)  //RBF7
        };

        tris = new int[]
        {
            0, 7, 4, // Bottom
            0, 3, 7, 
            1, 5, 2, // Top
            2, 5, 6,
            0, 1, 3, // Front
            3, 1, 2,
            4, 6, 5, // Back
            4, 7, 6,
            5, 1, 4, // Left
            0, 4, 1,
            7, 3, 2, // Right
            7, 2, 6
        };

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

	void OnCollisionEnter(Collision collision)
	{
        Debug.Log("!!!");
	}
}
