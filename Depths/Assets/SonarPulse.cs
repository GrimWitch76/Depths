using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SonarPulse : MonoBehaviour
{
    [SerializeField] Transform _sonarPulse;
    [SerializeField] Vector3 _finalScale;
    [SerializeField] float _pulseTime;

    public void Reset()
    {
        _sonarPulse.localScale = Vector3.zero;
    }

    public void StartPulse()
    {
        StartCoroutine(SonarPulseTimer());
    }

    private IEnumerator SonarPulseTimer()
    {
        float timer = 0;

        while(timer <= _pulseTime)
        {
            timer += Time.deltaTime;
            var progress = timer / _pulseTime;
            var newScale = Vector3.Lerp(Vector3.zero, _finalScale, progress);
            _sonarPulse.localScale = newScale;
            yield return null;
        }

        Reset();
    }    
}
