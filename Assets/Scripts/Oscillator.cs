using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 5f;

    /*
    Removido da inspeto da unity.
    [Range(0,1)][SerializeField] float movementFactor; //0for not moved, 1 for fully moved.
    Mantido cópia por causa da função "range" que permite criar sliders no inspetor.
    */

    float movementFactor; //0for not moved, 1 for fully moved.


    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position; // stores the starting position
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; } // protect against "period" = 0

        float cycles = Time.time / period; // grows continuasly from zero.

        const float tau = Mathf.PI * 2f;
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPosition + offset;
    }
}
