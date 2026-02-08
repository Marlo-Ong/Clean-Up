using UnityEngine;

/// <summary>
/// Plays a random scrubbing sound whenever
/// a transform changes direction.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ScrubAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private AudioSource source;
    private Vector3 lastPosition;
    private Vector3 lastDirection;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector2 movement = currentPosition - lastPosition;

        if (movement.sqrMagnitude > 0.0001f)
        {
            Vector2 direction = movement.normalized;

            if (ChangedDirection(direction, lastDirection))
            {
                int index = Random.Range(0, clips.Length);
                source.PlayOneShot(clips[index]);
            }
            lastDirection = direction;
        }
        lastPosition = currentPosition;
    }

    private bool ChangedDirection(Vector3 curr, Vector3 prev)
    {
        return
            Mathf.Sign(curr.y) != Mathf.Sign(prev.y) ||
            Mathf.Sign(curr.x) != Mathf.Sign(prev.x);
    }
}
