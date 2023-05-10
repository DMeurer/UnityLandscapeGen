using UnityEngine;
using UnityEngine.Serialization;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    
    public bool autoUpdate = true;
    
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;
    
    private ShapeGenerator _shapeGenerator;
    
    [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
    private TerrainFace[] _terrainFaces;

    void Initialize()
    {
        // create shape generator
        _shapeGenerator = new ShapeGenerator(shapeSettings);
        // create mesh filters
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        _terrainFaces = new TerrainFace[6];
        
        // directions of faces
        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };
        
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                // create mesh filter
                GameObject meshObj = new GameObject("mesh");
                // set parent to this object
                meshObj.transform.parent = transform;
                
                // add mesh renderer
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                // set mesh filter
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                // set mesh
                meshFilters[i].sharedMesh = new Mesh();
            }
            
            // create terrain face
            _terrainFaces[i] = new TerrainFace(_shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }
    
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }
    
    public void OnShapeSettingsUpdated()
    {
        if (!autoUpdate) return;
        Initialize();
        GenerateMesh();
    }

    public void OnColorSettingsUpdated()
    {
        if (!autoUpdate) return;
        Initialize();
        GenerateColors();
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in _terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    private void GenerateColors()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColour;
        }
    }
}
