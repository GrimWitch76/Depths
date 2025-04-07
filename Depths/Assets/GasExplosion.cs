using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GasExplosion : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    [SerializeField] AudioSource _source;
    [SerializeField] CinemachineImpulseSource _impulse;


    public void Explode()
    {
        _particle.Play();
        _source.Play();
        _impulse.GenerateImpulse();

        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(_source.clip.length + 0.5f);
        Destroy(gameObject);
    }
}
