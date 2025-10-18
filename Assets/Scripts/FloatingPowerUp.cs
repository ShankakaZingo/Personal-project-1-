using UnityEngine;

public class FloatingPowerUp : MonoBehaviour
{
    [Header("Animation Settings")]
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;
    public float rotationSpeed = 50f;
    
    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        AnimateFloating();
        AnimateRotation();
    }

    void AnimateFloating()
    {
        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void AnimateRotation()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}