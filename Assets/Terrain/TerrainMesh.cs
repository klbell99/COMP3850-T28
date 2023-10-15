using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    public Terrain terrain;
    public GameObject copyToObject;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = copyToObject.GetComponent<MeshFilter>(); // get the mesh filter
        Mesh m = mf.mesh; // get the mesh from the mesh filter

        List<Vector3> newVerts = new List<Vector3>(); // create a list for the verticies for the mesh

        foreach (var vert in m.vertices) // for each vertex in the terrain
        {
            Vector3 wPos = copyToObject.transform.localToWorldMatrix * vert; // grab the vertex in the world space
            Vector3 newVert = vert;
            newVert.y = terrain.SampleHeight(wPos); // set the hight of the mesh vertex to the height of the terrain vertex
            newVerts.Add(newVert); // add the vertex to the list for the mesh
        }
        m.SetVertices(newVerts); // apply the vertices from the list

        m.RecalculateNormals();
        m.RecalculateTangents();
        m.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
