using System.Collections.Generic;
using UnityEngine;

public class AI_audioSensor : MonoBehaviour
{
    public Color MColor = Color.red;
    public int scanFrequency = 30;
    public float HearingDistance = 10f;
    public LayerMask excludeLayerMask;  // Optional, in case you want to exclude certain layers
    public bool SoundDetected;

    private Transform AudioSourcePosition;
    private List<AudioSource> objects = new List<AudioSource>();
    private readonly Collider[] colliders = new Collider[50];
    private float ScanInterval;
    private float ScanTimer;

    // Timer for cleaning up destroyed AudioSource references
    private float cleanupTimer;
    private float cleanupInterval = 2.0f; // Clean up every 2 seconds

    void Start()
    {
        ScanInterval = 1.0f / scanFrequency;
        cleanupTimer = cleanupInterval;
    }

    void Update()
    {
        ScanTimer -= Time.deltaTime;
        cleanupTimer -= Time.deltaTime;


        if (ScanTimer < 0)
        {
            ScanTimer += ScanInterval;
            Scan();
        }

        // Clean up destroyed AudioSource references periodically
        if (cleanupTimer < 0)
        {
            cleanupTimer = cleanupInterval;
            CleanupDestroyedAudioSources();
        }


        ListenForAudioPlay();
    }

    private void Scan()
    {
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, HearingDistance, colliders, ~excludeLayerMask);

        for (int i = 0; i < colliderCount; i++)
        {
            GameObject objectInRange = colliders[i].gameObject;

            if (objectInRange.TryGetComponent<AudioSource>(out AudioSource audioFound))
            {
                if (!objects.Contains(audioFound))
                {
                    objects.Add(audioFound);
                }
            }
        }
    }

    private void ListenForAudioPlay()
    {
        SoundDetected = false;  // Reset the detection status each frame

        for (int i = 0; i < objects.Count; i++)
        {
            // Skip if the AudioSource is null (destroyed)
            if (objects[i] == null)
            {
                continue;
            }

            if (objects[i].isPlaying)
            {
                SoundDetected = true;

                AudioSourcePosition = objects[i].gameObject.transform;
                break;
            }
        }
    }

    private void CleanupDestroyedAudioSources()
    {
        // Remove null (destroyed) AudioSource references from the list
        objects.RemoveAll(audioSource => audioSource == null);
    }

    public Transform ObjectLocation()
    {
        return SoundDetected ? AudioSourcePosition : null;

    }

    private void OnValidate()
    {
        ScanInterval = 1.0f / scanFrequency;
    }

}
