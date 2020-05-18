using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Pathfind grid")]
    [SerializeField]
    private PathfindGrid _grid = null;

    [Header("Map parameters")]
    [SerializeField]
    private int _mapWidth = 2;
    [SerializeField]
    private int _mapHeight = 2;
    [SerializeField]
    private float _yPosition = -0.5f;
    [SerializeField]
    private float _waterDepth = 0.3f;

    [SerializeField]
    private int _seed = 0;
    [SerializeField]
    private Vector2 _offset = Vector2.zero;

    [SerializeField]
    private float _noiseScale = 1;

    [SerializeField]
    private int _octaves = 1;
    [SerializeField] [Range(0, 1)]
    private float _persistance = 0.5f;
    [SerializeField]
    private float _lacunarity = 2f;

    [Header("Map generator display")]
    public bool AutoUpdate = true;

    [SerializeField]
    private MapDisplay _mapDisplay = null;

    [SerializeField]
    private bool _generateObstacles = false;

    [SerializeField]
    private TerrainType[] _regions = null;
    private float[] _relativeRegionHeights;
    private float[] _defaultRegionHeights;

    public float DefaultHeight { get; private set; }

    private void Awake()
    {
        DefaultHeight = 1f;
        _seed = Random.Range(0, 99999999);
        GenerateMap();
        SetRelativeRegionHeights();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

        Color[] colormap = SetColorMap(noiseMap);

        if (_waterDepth > DefaultHeight)
            DefaultHeight += _waterDepth;

        // Display
        var meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, _regions[0].height, _yPosition, _waterDepth, DefaultHeight);
        _mapDisplay.DrawMesh(meshData, TextureGenerator.TextureFromColormap(colormap, _mapWidth, _mapHeight));
        if (_generateObstacles)
            ObstacleGenerator.GenerateObstacleMesh(meshData);

        if (_grid != null)
            _grid.StartPathfindGrid();
    }

    private Color[] SetColorMap(float[,] noiseMap)
    {
        Color[] colormap = new Color[_mapWidth * _mapHeight];
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                float lowerHeight = 0;
                for (int i = 0; i < _regions.Length; i++)
                {
                    TerrainType region = _regions[i];

                    if (currentHeight <= region.height)
                    {
                        if (i > 0)
                            lowerHeight = _regions[i - 1].height;

                        float t = Mathf.InverseLerp(lowerHeight, region.height, currentHeight);
                        Color regionColor = Color.Lerp(region.startColor, region.endColor, t);
                        colormap[y * _mapWidth + x] = regionColor;
                        break;
                    }
                }
            }
        }
        return colormap;
    }

    private void OnValidate()
    {
        if (_mapHeight < 1)
            _mapHeight = 1;
        if (_mapWidth < 1)
            _mapWidth = 1;
        if (_lacunarity < 1)
            _lacunarity = 1;
        if (_octaves < 0)
            _octaves = 0;
    }

    private void SetRelativeRegionHeights()
    {
        _relativeRegionHeights = new float[_regions.Length];
        _defaultRegionHeights = new float[_regions.Length];
        for (int i = 0; i < _regions.Length; i++)
        {
            _relativeRegionHeights[i] = (_regions[i].height - _regions[0].height) / (_regions[_regions.Length - 1].height - _regions[0].height);
            _defaultRegionHeights[i] = _regions[i].height;
        }
    }

    public void SetWidth(float width)
    {
        _mapWidth = (int) width;
        GenerateMap();
    }

    public void SetHeight(float height)
    {
        _mapHeight = (int) height;
        GenerateMap();
    }

    public void SetWaterDepth(float depth)
    {
        _waterDepth = depth;
        GenerateMap();
    }

    public void SetSeed(string seed)
    {
        _seed = int.Parse(seed);
        GenerateMap();
    }

    public void SetScale(float scale)
    {
        _noiseScale = scale;
        GenerateMap();
    }

    public void SetComplexity(float complexity)
    {
        _octaves = (int) complexity;
        GenerateMap();
    }

    public void SetWaterLevel(float waterHeight)
    {
        float referenceHeightDiff = 1f - waterHeight;
        for (int i = 0; i < _regions.Length; i++)
        {
            _regions[i].height = waterHeight + (1f - waterHeight) * _relativeRegionHeights[i];
            GenerateMap();
        }
    }

    public void SetXOffset(float xOffset)
    {
        _offset += Vector2.right * (xOffset - _offset.x);
        GenerateMap();
    }

    public void SetYOffset(float yOffset)
    {
        _offset += Vector2.up * (yOffset - _offset.y);
        GenerateMap();
    }

    public int GetSeed()
    {
        return _seed;
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color startColor;
    public Color endColor;
}
