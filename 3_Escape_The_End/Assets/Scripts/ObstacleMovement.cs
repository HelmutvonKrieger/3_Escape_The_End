using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField]
    Vector3 movementVector = new Vector3(0, 10f, 0);

    [SerializeField]
    float period = 2f;

    float movementFactor;
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Grows constantly from 0
        if (period <= Mathf.Epsilon)
        {
            period++;
        }

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2; // ~6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // from -1 to 1

        movementFactor = (rawSinWave / 2f) + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
