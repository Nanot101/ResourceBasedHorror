using System.Collections.Generic;
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

    private const int FrontViewRays = 200 + MinNumOfRays;
    private const int AroundViewRays = 250 + MinNumOfRays;

    [SerializeField]
    [Tooltip("It will hold generated mesh")]
    private MeshFilter meshFilter;

    [SerializeField]
    private LayerMask fieldOfViewCollisionLayer;

    private readonly object lockObject = new();
    private FieldOfViewMeshBuilder meshBuilder;

    private FieldOfViewDistanceSettings currentSettings;
    private FieldOfViewDistanceSettings nextSettings;

    private int FrontRayCount => FrontViewRays + currentSettings.AdditionalFrontRayCount;
    private int AroundRayCount => AroundViewRays + currentSettings.AdditionalAroundRayCount;

    private void Awake()
    {
        AssertDesignerFileds();

        var mesh = new Mesh();
        meshFilter.mesh = mesh;

        meshBuilder = new FieldOfViewMeshBuilder(mesh, transform);
    }

    private void LateUpdate()
    {
        // Late update because player movement and mouse aim must run before us

        SetCurrentSettings();

        if (currentSettings == null)
        {
            return;
        }

        meshBuilder.Reset();
        meshBuilder.AddVertex(Vector3.zero);

        AddFronView();
        TryAddAroundView();

        var buildSuccessful = meshBuilder.Build();

        Debug.Assert(buildSuccessful);
    }

    public void SetNextSettings(FieldOfViewDistanceSettings settings)
    {
        lock (lockObject)
        {
            nextSettings = settings;
        }
    }

    private void SetCurrentSettings()
    {
        lock (lockObject)
        {
            currentSettings = nextSettings;
        }
    }

    private void AddFronView()
    {
        var frontVertices = FieldOfViewVerticesProvider.GetFrontVertices(
            FrontRayCount,
            currentSettings.FrontViewDistance,
            currentSettings.FrontViewAngle,
            transform,
            fieldOfViewCollisionLayer);

        BuildView(frontVertices);
    }

    private void TryAddAroundView()
    {
        if (currentSettings.AroundViewDistance <= 0.0f)
        {
            return;
        }

        AddAroundView();
    }

    private void AddAroundView()
    {
        var frontVertices = FieldOfViewVerticesProvider.GetAroundVertices(
            AroundRayCount,
            currentSettings.AroundViewDistance,
            currentSettings.FrontViewAngle,
            transform,
            fieldOfViewCollisionLayer);

        BuildView(frontVertices);
    }

    private void BuildView(IReadOnlyList<Vector3> frontVertices)
    {
        meshBuilder.AddVertex(frontVertices[0]);

        for (var i = 1; i < frontVertices.Count; i++)
        {
            meshBuilder.AddVertexAndTriangleIndexes(frontVertices[i]);
        }
    }

    private void AssertDesignerFileds()
    {
        Debug.Assert(meshFilter != null, $"{nameof(meshFilter)} is required");
    }
}
