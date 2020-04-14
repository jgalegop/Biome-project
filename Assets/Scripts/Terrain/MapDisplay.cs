using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField]
    private Material _terrainMaterial = null;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public void DrawMesh(MeshGenerator.MeshData meshData, Texture2D texture)
    {
        GameObject terrainMesh = GameObject.Find("TerrainMesh");
        if (terrainMesh == null)
        {
            terrainMesh = new GameObject("TerrainMesh");
            terrainMesh.AddComponent<MeshFilter>();
            terrainMesh.AddComponent<MeshRenderer>();
        }

        _meshFilter = terrainMesh.GetComponent<MeshFilter>();
        _meshRenderer = terrainMesh.GetComponent<MeshRenderer>();

        _meshFilter.sharedMesh = meshData.CreateMesh();
        _meshRenderer.sharedMaterial = _terrainMaterial;
        _meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
