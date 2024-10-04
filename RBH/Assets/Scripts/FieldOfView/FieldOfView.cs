using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    /// <summary>
    /// There is one vertex per ray and we always have 1 vertex at the origin, 
    /// and the shape of the mesh must consist of at least one triangle, 
    /// which in turn consists of 3 vertices. 
    /// So we need at least two rays.
    /// </summary>
    private const int MinNumOfRays = 2;

    [SerializeField]
    [Tooltip("It will hold generated mesh")]
    private MeshFilter meshFilter;

    [SerializeField]
    [Tooltip("Must be greater than zero")]
    private float fieldOfViewAngle = 90.0f;

    [SerializeField]
    [Tooltip("Must be greater than zero")]
    private float viewDistance = 5.0f;

    [SerializeField]
    [Tooltip("Additional rays used to generate the mesh. The higher the value, the more detailed the mesh, but also slower generation!")]
    private int AdditionalRayCount = 2;

    [SerializeField]
    private LayerMask FieldOfViewCollisionLayer;

    private FieldOfViewMeshBuilder meshBuilder;

    private int RayCount => MinNumOfRays + AdditionalRayCount;

    void Awake()
    {
        AssertDesignerFileds();

        var mesh = new Mesh();

        meshFilter.mesh = mesh;

        // one vertex per ray + origin vertex 
        var numberOfVertices = RayCount + 1;

        // every triangle has 3 indexes,
        // first is always zero,
        // second is previously added vertex index
        // and third is currently added vertex index
        var numberOfTrianglesIndexes = 3 * (RayCount - 1);

        meshBuilder = new(mesh, numberOfVertices, numberOfTrianglesIndexes);
    }

    void LateUpdate()
    {
        // Late update because player movement and mouse aim must run before us

        meshBuilder.Reset(transform);
        meshBuilder.AddVertex(Vector3.zero);

        foreach (var rayDirection in GetRayDirections())
        {
            var hit = Physics2D.Raycast(transform.position, rayDirection, viewDistance, FieldOfViewCollisionLayer);

            if (hit.collider == null)
            {
                meshBuilder.AddVertex(rayDirection * viewDistance);
                continue;
            }

            meshBuilder.AddVertex((Vector3)hit.point - transform.position);
        }

        Debug.Assert(meshBuilder.Build());
    }

    private IEnumerable<Vector3> GetRayDirections()
    {
        var initialAngle = GetAngleDegreesFromVector(transform.up);
        initialAngle += fieldOfViewAngle / 2.0f;

        var angleIncrease = fieldOfViewAngle / (RayCount - 1);

        var directions = Enumerable
            .Range(0, RayCount)
            .Select(i => GetVectorFromAngleDegrees(initialAngle - i * angleIncrease));

        return directions;
    }

    private float GetAngleDegreesFromVector(Vector3 vector)
    {
        vector.Normalize();

        // Something, something, trigonometry I guess
        var angle = math.atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360.0f;
        }

        return angle;
    }

    private Vector3 GetVectorFromAngleDegrees(float angle)
    {
        var angleRadians = math.radians(angle);

        // Trigonometry again...
        return new Vector3(math.cos(angleRadians), math.sin(angleRadians));
    }

    private void AssertDesignerFileds()
    {
        Debug.Assert(fieldOfViewAngle > 0, $"{nameof(fieldOfViewAngle)} must be greater than zero");
        Debug.Assert(viewDistance > 0, $"{nameof(viewDistance)} must be greater than zero");
    }
}
