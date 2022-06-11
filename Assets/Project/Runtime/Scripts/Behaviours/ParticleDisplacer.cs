using UnityEngine;

public class ParticleDisplacer : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    //private NativeArray<Vector3> _vertices;
    // private ParticleWave _job;
    //private JobHandle _handle;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        var particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        var currentAmount = _particleSystem.GetParticles(particles);

        // Change only the particles that are alive
        for (int i = 0; i < currentAmount; i++)
        {
            particles[i].position = new Vector3(
                particles[i].position.x,
                WaveManager.Instance.GetWaveHeight(particles[i].position.x, particles[i].position.z),
                particles[i].position.z);
        }

        // Apply the particle changes to the Particle System
        _particleSystem.SetParticles(particles, currentAmount);
    }

    // private void MeshUpdate()
    // {
    //     _job = new ParticleWave
    //     {
    //         Vertices = _vertices,
    //         Position = transform.position,
    //     };
    //     _handle = _job.Schedule(_meshFilter.mesh.vertices.Length, 128);
    //     _handle.Complete();
    //     _mesh.vertices = _vertices.ToArray();
    //     _mesh.RecalculateBounds();
    //     _mesh.RecalculateNormals();
    // }

    // private void OnDestroy()
    // {
    //     _vertices.Dispose();
    // }
}

// public struct ParticleWave : IJobParallelFor
// {
//     public NativeArray<Vector3> Vertices;
//
//     public Vector3 Position;
//
//     public void Execute(int i)
//     {
//         Vertices[i] = new Vector3(
//             Vertices[i].x,
//             WaveManager.instance.GetWaveHeight(Position.x + Vertices[i].x, Position.z + Vertices[i].z),
//             Vertices[i].z);
//     }
// }