using UnityEngine;

public class TerrainFace
{
    private Mesh _mesh;
    public int resolution;
    public Vector3 localUp;
    private Vector3 _axisA;
    private Vector3 _axisB;
    private ShapeGenerator _shapeGenerator;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        _shapeGenerator = shapeGenerator;
        _mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        
        // axisA is perpendicular to localUp
        _axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        // axisB is perpendicular to localUp and axisA
        _axisB = Vector3.Cross(localUp, _axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6]; // 6 vertices per square
        
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // create vertices
                int i = x + y * resolution; // index of vertex
                Vector2 percent = new Vector2(x, y) / (resolution - 1); // 0 to 1
                // localUp is the normal of the face
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * _axisA + (percent.y - 0.5f) * 2 * _axisB; // -1 to 1
                // normalize to get point on unit sphere
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; // 0 to 1
                vertices[i] = _shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                // vertices[i] = _shapeGenerator.CalculatePointOnPlanet(pointOnUnitCube);
                
                // create triangles
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // 2 triangles per square
                    // 1st triangle
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;
                    
                    // 2nd triangle
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    
                    // increment triIndex
                    triIndex += 6;
                }
            }
        }
        
        // clear mesh and set vertices and triangles
        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
    }
}
