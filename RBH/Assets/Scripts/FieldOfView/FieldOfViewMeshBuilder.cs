using System;
using UnityEngine;

public class FieldOfViewMeshBuilder
{
    private const int VerticesInitilaLenght = 50;
    private const int TrianglesVerticesIndexesInitilaLenght = 50;

    private readonly Mesh _mesh;
    private readonly Transform _worldOrigin;

    private Vector3[] _vertices = new Vector3[VerticesInitilaLenght];
    private int[] _trianglesVerticesIndexes = new int[TrianglesVerticesIndexesInitilaLenght];

    private int _nextVertex = 0;
    private int _nextTriangeIndex = 0;

    public FieldOfViewMeshBuilder(Mesh mesh, Transform worldOrigin)
    {
        _mesh = mesh;
        _mesh.MarkDynamic();

        _worldOrigin = worldOrigin;
    }

    public void Reset()
    {
        _nextVertex = 0;
        _nextTriangeIndex = 0;
    }

    public void AddVertex(Vector3 vertexPosition)
    {
        _vertices[_nextVertex] = vertexPosition;

        IncreaseVertexIndexAndResize();
    }

    public void AddVertexAndTriangleIndexes(Vector3 vertexPosition)
    {
        _vertices[_nextVertex] = vertexPosition;

        AddTrianglesVerticesIndexes();

        IncreaseVertexIndexAndResize();
    }

    public bool Build()
    {
        if (_nextTriangeIndex < 3)
        {
            return false;
        }

        // I don't know why, but I have to do it, otherwise the mesh rotates twice
        _worldOrigin.InverseTransformVectors(_vertices.AsSpan(0, _nextVertex));

        _mesh.Clear();

        _mesh.SetVertices(_vertices, 0, _nextVertex);
        _mesh.SetTriangles(_trianglesVerticesIndexes, 0, _nextTriangeIndex, 0);

        _mesh.MarkModified();

        return true;
    }

    private void AddTrianglesVerticesIndexes()
    {
        if (_nextVertex < 2)
        {
            return;
        }

        // The order is important because it determines the direction in which the mesh is rendered
        _trianglesVerticesIndexes[_nextTriangeIndex] = 0;
        _trianglesVerticesIndexes[_nextTriangeIndex + 1] = _nextVertex;
        _trianglesVerticesIndexes[_nextTriangeIndex + 2] = _nextVertex - 1;

        IncreaseTriangleVertexIndexesAndResize();
    }

    private void IncreaseVertexIndexAndResize()
    {
        _nextVertex++;

        if (_vertices.Length <= _nextVertex)
        {
            Array.Resize(ref _vertices, _vertices.Length * 2);
        }
    }

    private void IncreaseTriangleVertexIndexesAndResize()
    {
        _nextTriangeIndex += 3;

        if (_trianglesVerticesIndexes.Length <= _nextTriangeIndex + 2)
        {
            Array.Resize(ref _trianglesVerticesIndexes, _trianglesVerticesIndexes.Length * 2);
        }
    }
}
