using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int _xSize = 20;
    [SerializeField] private int _zSize = 20;

    [SerializeField] private float _scale = 5;
    [SerializeField] private float _height = 5;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _mesh = new Mesh();
        _meshFilter.mesh = _mesh;

        GenerateMesh();
    }

    private void Update()
    {
        UpdateVertices();
        UpdateMesh();
    }

    private void GenerateMesh()
    {
        _vertices = new Vector3[(_xSize + 1) * (_zSize + 1)];

        UpdateVertices();

        _triangles = new int[_xSize * _zSize * 6];

        var vert = 0;
        var tris = 0;
        for (var z = 0; z < _zSize; z++)
        {
            for (var x = 0; x < _xSize; x++)
            {
                _triangles[tris + 0] = vert + 0;
                _triangles[tris + 1] = vert + _xSize + 1;
                _triangles[tris + 2] = vert + 1;
                _triangles[tris + 3] = vert + 1;
                _triangles[tris + 4] = vert + _xSize + 1;
                _triangles[tris + 5] = vert + _xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }
    }

    private void UpdateVertices()
    {
        float resolutionX = 1f / _xSize;
        float resolutionZ = 1f / _zSize;
        
        for (int i = 0, z = 0; z <= _zSize; z++)
        {
            for (var x = 0; x <= _xSize; x++, i++)
            {
                float y = Mathf.PerlinNoise(x * resolutionX * _scale, z * resolutionZ * _scale) * _height;
                _vertices[i] = new Vector3(x, y, z);
            }
        }
    }

    private void UpdateMesh()
    {
        _mesh.Clear();

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
    }
}