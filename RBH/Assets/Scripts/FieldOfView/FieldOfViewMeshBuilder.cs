using UnityEngine;

public class FieldOfViewMeshBuilder
{
    private readonly Mesh _mesh;
    private readonly Vector3[] _vertices;
    private readonly int[] _trianglesVerticesIndexes;

    private Transform _worldOrigin;
    private int _currentVertex = 0;
    private int _currentTriangeIndex = 0;

    public FieldOfViewMeshBuilder(Mesh mesh, int numberOfVertices, int numberOfTriangleIndexes)
    {
        _mesh = mesh;
        _mesh.MarkDynamic();

        _vertices = new Vector3[numberOfVertices];
        _trianglesVerticesIndexes = new int[numberOfTriangleIndexes];
    }

    public void Reset(Transform worldOrigin)
    {
        _worldOrigin = worldOrigin;
        _currentVertex = 0;
        _currentTriangeIndex = 0;
    }

    public void AddVertex(Vector3 vertexPosition)
    {
        _vertices[_currentVertex] = vertexPosition;

        if (_currentVertex >= 2)
        {
            _trianglesVerticesIndexes[_currentTriangeIndex] = 0;
            _trianglesVerticesIndexes[_currentTriangeIndex + 1] = _currentVertex - 1;
            _trianglesVerticesIndexes[_currentTriangeIndex + 2] = _currentVertex;

            _currentTriangeIndex += 3;
        }

        _currentVertex++;
    }

    public bool Build()
    {
        if (_currentTriangeIndex < 3)
        {
            return false;
        }

        // I don't know why, but I have to do it, otherwise the mesh rotates twice.
        _worldOrigin.InverseTransformVectors(_vertices);

        _mesh.vertices = _vertices;
        _mesh.triangles = _trianglesVerticesIndexes;

        _mesh.MarkModified();

        return true;
    }
}
