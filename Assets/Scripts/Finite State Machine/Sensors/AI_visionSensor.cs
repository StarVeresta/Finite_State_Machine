using System.Collections.Generic;
using UnityEngine;

public class AI_VisionSensor : MonoBehaviour
{
    public float DistanceRange = 10f;
    [Range(0,180)]
    public float angle = 30f;
    public float height = 1.0f;
    public Color MColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask ObsticlesLayer;
    public bool HasTarget;

    public List<GameObject> ObjectFound
    {
        get
        {
            objects.RemoveAll(obj => !obj);
            return objects;
        }
    }

    private List<GameObject> objects = new List<GameObject>();
    private readonly Collider[] colliders = new Collider[50];
    private Mesh Ai_mesh;
    private int Count;
    private float ScanInterval;
    private float ScanTimer;

    void Start()
    {
        ScanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = ObjectFound.Count > 0 ? true : false;

        ScanTimer -= Time.deltaTime;
        if (ScanTimer < 0)
        {
            ScanTimer += ScanInterval;
            Scan();
        }
    }

    public void RemoveItem(GameObject item)
    {
        objects.Remove(item);
    }

    public bool InSight(GameObject obj)
    {
        Vector3 originP = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - originP;
        if (direction.y < -height || direction.y > height)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        originP.y += height / 2;
        dest.y = originP.y;
        if (Physics.Linecast(originP, dest, ObsticlesLayer))
        {
            return false;
        }

        return true;
    }

    private void Scan()
    {
        Count = Physics.OverlapSphereNonAlloc(transform.position, DistanceRange, colliders, layers, QueryTriggerInteraction.Collide);

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

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    Mesh CreateWedgeMesh()
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

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
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

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom
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

    private void OnValidate()
    {
        Ai_mesh = CreateWedgeMesh();
        ScanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (Ai_mesh)
        {
            Gizmos.color = MColor;
            Gizmos.DrawMesh(Ai_mesh, transform.position, transform.rotation);
        }
    }

}

