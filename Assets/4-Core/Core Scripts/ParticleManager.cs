using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{

    [Space(5)]
    [Header("References")]
    [Space(5)]

    public GameObject Appreciation;
    public GameObject LevelCompleted;

    /// <summary>
    /// Show your referenced particles
    /// </summary>
    /// <param name="current_particle"> Particle to show</param>
    /// <param name="lifetime"> Life time of the particle</param>
    /// <param name="position"> World Position at which the particle will be shown (Default Position = 0,0,0)</param>
    /// <param name="Instance"> Instaniate a copy of the given particle (Default = false) </param>

    public void ShowParticle(GameObject current_particle, float lifetime, Vector3 position = default(Vector3), bool Instance = false)
    {
        if (!Instance)
        {
            current_particle.transform.position = position;
            current_particle.SetActive(true);
            StartCoroutine(ParticleLifeTime(current_particle, lifetime));
        }
        else
        {
            GameObject newParticle = Instantiate(current_particle, position, Quaternion.identity);
            newParticle.SetActive(true);
            StartCoroutine(ParticleDestroyer(newParticle, lifetime));
        }
    }

    IEnumerator ParticleLifeTime(GameObject current, float timer)
    {
        yield return new WaitForSeconds(timer);
        current.SetActive(false);

    }

    IEnumerator ParticleDestroyer(GameObject newParticle, float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(newParticle);

    }



}
