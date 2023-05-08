using System;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private Mesh _mesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    public int xSize = 50;
    public int zSize = 50;
    private long _meshArea = 0;
    public int meshResolution = 4;
    private int _meshResolutionPrevious;
    public float perlinZoomLarge = 0.05f;
    public float terrainHeight = 15f;
    private float _terrainHeightPrevious;
    public float perlinZoomDetail = 0.2f;
    public float detailWeight = 2f;
    private float _detailWeightPrevious;
    private float _perlinZoomPrevious;

    private bool _changes;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    // Update is called once per frame
    void Update()
    {
        _changes = CheckChanges();
        CreateShape();
        UpdateMesh();

        UpdatePreviousVars();
    }

    private bool CheckChanges()
    {
        if (_meshArea != xSize * zSize) return true;
        if (meshResolution != _meshResolutionPrevious) return true;

        // compare floats with some tolerance to 
        if (Math.Abs((perlinZoomLarge + perlinZoomDetail) - _perlinZoomPrevious) > 0.01f) return true;
        if (Math.Abs(terrainHeight - _terrainHeightPrevious) > 0.01f) return true;
        if (Math.Abs(detailWeight - _perlinZoomPrevious) > 0.01f) return true;

        return false;
    }

    private void UpdatePreviousVars()
    {
        _meshArea = xSize * zSize;
        _meshResolutionPrevious = meshResolution;
        _perlinZoomPrevious = perlinZoomLarge + perlinZoomDetail;
        _terrainHeightPrevious = terrainHeight;
    }

    private void CreateShape()
    {
        if (!_changes) return;

        if (meshResolution == 0) meshResolution = 1;

        _vertices = new Vector3[(xSize * meshResolution + 1) * (zSize * meshResolution + 1)];

        for (int i = 0, z = 0; z <= zSize * meshResolution; z++)
        {
            for (int x = 0; x <= xSize * meshResolution; x++)
            {
                // generate point height using Perlin noise
                float terrainLarge = Mathf.PerlinNoise(x / (float)meshResolution * perlinZoomLarge,
                    z / (float)meshResolution * perlinZoomLarge) * terrainHeight;
                float terrainDetail = Mathf.PerlinNoise(x / (float)meshResolution * perlinZoomDetail,
                    z / (float)meshResolution * perlinZoomDetail) * detailWeight;
                float y = terrainLarge + terrainDetail;
                
                // ann new point to the terrain
                _vertices[i] = new Vector3(x / (float)meshResolution, y, z / (float)meshResolution);
                i++;
            }
        }


        _triangles = new int[((long)xSize * meshResolution) * ((long)zSize * meshResolution) * 6 + 6];
        
        int vert = 0;
        int tris = 0;
        
        for (int z = 0; z < zSize * meshResolution; z++)
        {
            for (int x = 0; x < xSize * meshResolution; x++)
            {
                /* create first triangle
                     *---*
                     | /
                     *   *
                 */
                _triangles[tris + 0] = vert + 0;
                _triangles[tris + 1] = vert + xSize * meshResolution + 1;
                _triangles[tris + 2] = vert + 1;
                /* create second triangle
                     *   *
                       / |
                     *---*
                 */
                _triangles[tris + 3] = vert + 1;
                _triangles[tris + 4] = vert + xSize * meshResolution + 1;
                _triangles[tris + 5] = vert + xSize * meshResolution + 2;
        
                /*
                 * Result:
                 *      *---*
                 *      | / |
                 *      *---*
                 */
        
                vert++;
                tris += 6;
            }
        
            // skip the triangle that goes form one row to another
            vert++;
        }
    }

    private void UpdateMesh()
    {
        if (!_changes) return;

        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }

    /*
     // debug point creation
    private void OnDrawGizmos()
    {
        for (int i = 0; i < _vertices.Length; i=i+10000)
        {
            Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
    }
    */
}