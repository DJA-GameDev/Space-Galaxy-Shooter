using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private bool _isShaking = false;
    [SerializeField]
    private float duration = 0.5f;
    [SerializeField]
    private float magnitude = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        _isShaking = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCameraShake()
    {
        StartCoroutine(CameraShakeRoutine());
    }

    IEnumerator CameraShakeRoutine()
    {
        _isShaking = true;
        Vector3 defaultPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float xPosition = Random.Range(-1.0f, 1.0f) * magnitude;
            float yPosition = Random.Range(-1.0f, 1.0f) * magnitude;
            transform.position = new Vector3(yPosition, xPosition, -10.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isShaking = false;
        transform.position = defaultPosition;
    }
}
