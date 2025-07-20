using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI field-of-view vision system that detects objects within a cone-shaped range,
/// based on angle, distance, and line-of-sight. Includes mesh visualization and periodic scanning.
/// </summary>
public class AI_VisionSensor : MonoBehaviour
{
    [Header("Detection Settings")]
    public float DistanceRange = 10f;

    [Range(0, 180)]
    public float angle = 30f;

    public float height = 1.0f;
    public Color MColor = Color.red; // Gizmo color

    [Tooltip("How many times per second the scan is updated.")]
    public int scanFrequency = 30;

    [Tooltip("Layers considered valid targets.")]
    public LayerMask layers;

    [Tooltip("Layers considered as obstacles (block line-of-sight).")]
    public LayerMask ObsticlesLayer;

    [Tooltip("True if any targets were found in the last scan.")]
    public bool HasTarget;

    /// <summary>
    /// List of currently visible objects. Automatically cleans null references.
    /// </summary>
    public List<GameObject> ObjectFound
    {
        get
        {
            objects.RemoveAll(obj => !obj); // Auto-clean nulls
            return objects;
        }
    }

    private List<GameObject> objects = new List<GameObject>();
    private readonly Collider[] colliders = new Collider[50]; // Reuse for performance

    private Mesh Ai_mesh;
    private int Count;
    private float ScanInterval;
    private float ScanTimer;

    private void Start()
    {
        ScanInterval = 1.0f / scanFrequency;
    }

    private void Update()
    {
        HasTarget = ObjectFound.Count > 0;

        ScanTimer -= Time.deltaTime;
        if (ScanTimer < 0)
        {
            ScanTimer += ScanInterval;
            Scan();
        }
    }

    /// <summary>
    /// Removes a specific object from the detection list.
    /// </summary>
    public void RemoveItem(GameObject item)
    {
        objects.Remove(item);
    }

    /// <summary>
    /// Checks whether a given object is within line-of-sight, angle, and height limits.
    /// </summary>
    public bool InSight(GameObject obj)
    {
        Vector3 originP = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - originP;

        // Check height bounds
        if (direction.y < -height || direction.y > height)
            return false;

        // Check field of view angle
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
            return false;

        // Check for obstructions using a linecast
        originP.y += height / 2;
        dest.y = originP.y;

        if (Physics.Linecast(originP, dest, ObsticlesLayer))
            return false;

        return true;
    }

    /// <summary>
    /// Scans the surrounding area for visible objects using a non-allocating physics call.
    /// </summary>
    private void Scan()
    {
        Count = Physics.OverlapSphereNonAlloc(
            transform.position,
            DistanceRange,
            colliders,
            layers,
            QueryTriggerInteraction.Collide
        );

        objects.Clear();
        for (int i = 0; i < Count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (InSight(obj))
            {
                objects.Add(obj);
            }
        }
    }

    /// <summary>
    /// Converts a given angle into a world-space direction vector.
    /// </summary>
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Generates a wedge mesh to visualize the AI's field of view in the scene.
    /// </summary>
    private Mesh CreateWedgeMesh()
    {
        Ai_mesh = new Mesh();

        int segments = 10;
        int numTriangle = (segments * 4) + 2 + 2;
        int numVertices = numTriangle * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangle = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0f, -angle, 0f) * Vector3.forward * DistanceRange;
        Vector3 bottomRight = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * DistanceRange;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        // Left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // Right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward * DistanceRange;
            bottomRight = Quaternion.Euler(0f, currentAngle + deltaAngle, 0f) * Vector3.forward * DistanceRange;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // Far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // Top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // Bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++)
        {
            triangle[i] = i;
        }

        Ai_mesh.vertices = vertices;
        Ai_mesh.triangles = triangle;
        Ai_mesh.RecalculateNormals();

        return Ai_mesh;
    }

    /// <summary>
    /// Called when script values are changed in the Inspector.
    /// Re-generates the wedge mesh for visualization.
    /// </summary>
    private void OnValidate()
    {
        Ai_mesh = CreateWedgeMesh();
        ScanInterval = 1.0f / scanFrequency;
    }

    /// <summary>
    /// Draws the AI vision cone using a generated mesh.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (Ai_mesh)
        {
            Gizmos.color = MColor;
            Gizmos.DrawMesh(Ai_mesh, transform.position, transform.rotation);
        }
    }
}
