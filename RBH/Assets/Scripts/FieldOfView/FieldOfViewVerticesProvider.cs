using System.Collections.Generic;
using UnityEngine;

public static class FieldOfViewVerticesProvider
{
    private const int InitialListCapacity = 50;

    private static readonly List<Vector3> _frontVertices = new(InitialListCapacity);
    private static readonly List<Vector3> _aroundVertices = new(InitialListCapacity);

    public static IReadOnlyList<Vector3> GetFrontVertices(
        int verticesCount,
        float distance,
        float frontAngle,
        Transform origin,
        LayerMask collisionLayer)
    {
        _frontVertices.Clear();

        var initialAngle = frontAngle / -2.0f;
        float angleIncrease = CalculateAngleIncrease(verticesCount, frontAngle);

        SetVertices(_frontVertices, verticesCount, distance, initialAngle, angleIncrease, origin, collisionLayer);

        return _frontVertices;
    }

    public static IReadOnlyList<Vector3> GetAroundVertices(
        int verticesCount,
        float distance,
        float frontAngle,
        Transform origin,
        LayerMask collisionLayer)
    {
        _aroundVertices.Clear();

        var angleIncrease = CalculateAngleIncrease(verticesCount, 360.0f - frontAngle);
        var initialAngle = frontAngle / 2.0f;

        SetVertices(_aroundVertices, verticesCount, distance, initialAngle, angleIncrease, origin, collisionLayer);

        return _aroundVertices;
    }

    private static float CalculateAngleIncrease(int verticesCount, float frontAngle)
    {
        return frontAngle / (verticesCount - 1);
    }

    private static void SetVertices(
        List<Vector3> vertices,
        int verticesCount,
        float distance,
        float initialAngle,
        float angleIncrease,
        Transform origin,
        LayerMask collisionLayer)
    {
        var angle = initialAngle;

        for (var i = 0; i < verticesCount; i++)
        {
            var direction = Quaternion.Euler(0.0f, 0.0f, angle) * origin.up;

            var hit = Physics2D.Raycast(origin.position, direction, distance, collisionLayer);

            AddVertexFromHit(vertices, distance, origin, direction, hit);

            angle += angleIncrease;
        }
    }

    private static void AddVertexFromHit(
        List<Vector3> vertices,
        float distance,
        Transform origin,
        Vector3 direction,
        RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            vertices.Add((Vector3)hit.point - origin.position);
            return;
        }

        vertices.Add(direction * distance);
    }
}
