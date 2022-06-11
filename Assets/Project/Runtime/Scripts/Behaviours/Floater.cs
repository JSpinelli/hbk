using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaters = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;
    public float offset = 0f;
    private void FixedUpdate()
    {
        rigidBody.AddForceAtPosition(Physics.gravity/ floaters, transform.position, ForceMode.Acceleration);
        float waveHeight = WaveManager.Instance.GetWaveHeight(transform.position.x, transform.position.z);
        //float waveHeight = WavesGenerator.instance.GetWaterHeight(transform.position);
        if (transform.position.y + offset < waveHeight)
        {
            float displacementMultiplier =
                Mathf.Clamp01((waveHeight-transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position,ForceMode.Acceleration);
            rigidBody.AddForce(-rigidBody.velocity * (displacementMultiplier * waterDrag * Time.fixedDeltaTime),ForceMode.VelocityChange);
            rigidBody.AddTorque(-rigidBody.angularVelocity * (displacementMultiplier * waterAngularDrag * Time.fixedDeltaTime),ForceMode.VelocityChange);
        }
    }
}
