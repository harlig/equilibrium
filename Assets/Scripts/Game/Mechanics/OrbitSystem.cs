using System;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSystem : MonoBehaviour
{
    [SerializeField]
    private List<OrbiterData> supportedOrbiterPrefabs;
    private PlayerController player;
    private List<OrbiterData> orbiters;
    private float orbitDistance = 1.0f; // Distance from the player
    private float angularVelocity = 90.0f; // Degrees per second
    private float currentSystemAngle = 0.0f; // Current rotation angle of the system
    private float deflectProjectileChance = 0f;
    private readonly float maxDeflectProjectileChance = 0.8f;
    public Dictionary<OrbiterType, float> ChanceOfOrbiterTypeDoingElementalEffect = new();

    public enum OrbiterType
    {
        FIRE,
        ICE
    }

    void Awake()
    {
        orbiters = new();
        player = GetComponentInParent<PlayerController>();

        foreach (OrbiterType type in Enum.GetValues(typeof(OrbiterType)))
        {
            ChanceOfOrbiterTypeDoingElementalEffect.Add(type, 0.0f);
        }
    }

    void Update()
    {
        // Update the rotation of the system
        float angle = angularVelocity * Time.deltaTime;
        currentSystemAngle += angle;

        // Ensure the angle stays within 0-360 degrees
        currentSystemAngle %= 360.0f;

        // Update the position of each orbiter every frame
        for (int i = 0; i < orbiters.Count; i++)
        {
            if (orbiters[i] != null)
            {
                orbiters[i].transform.RotateAround(
                    player.transform.position,
                    Vector3.forward,
                    angle
                );
            }
        }
    }

    public void AddOrbiter(OrbiterType orbiter)
    {
        OrbiterData orbiterPrefabToInstantiate = null;
        foreach (OrbiterData supportedOrbiter in supportedOrbiterPrefabs)
        {
            if (supportedOrbiter.OrbiterType == orbiter)
            {
                orbiterPrefabToInstantiate = supportedOrbiter;
                break;
            }
        }
        if (orbiterPrefabToInstantiate == null)
        {
            throw new System.Exception(
                $"failed to find an orbiter to instantiate in this system of type {orbiter}"
            );
        }

        OrbiterData orbiterInstance = OrbiterData.Create(orbiterPrefabToInstantiate, this, player);
        orbiters.Add(orbiterInstance);

        // Rearrange all orbiters to be equidistant
        RearrangeOrbiters();
    }

    public void IncreaseDamageOfOrbiterType(OrbiterType orbiterType, float damageIncrase)
    {
        foreach (var orbiter in orbiters)
        {
            if (orbiter.OrbiterType == orbiterType)
            {
                orbiter.AddToDamage(damageIncrase);
            }
        }
    }

    public void IncreaseOrbitSpeed(float angularVelocityToIncrease)
    {
        angularVelocity += angularVelocityToIncrease;
    }

    public void IncreaseDistanceFromPlayer(float distanceToIncrease)
    {
        orbitDistance += distanceToIncrease;
        RearrangeOrbiters();
    }

    public void IncreaseDeflectProjectileChance(float chanceToAdd)
    {
        deflectProjectileChance += chanceToAdd;
    }

    public bool ShouldDeflectProjectile()
    {
        return UnityEngine.Random.Range(0f, 1.0f)
            < Mathf.Clamp(deflectProjectileChance, 0f, maxDeflectProjectileChance);
    }

    private void RearrangeOrbiters()
    {
        int numOrbiters = orbiters.Count;
        float angleStep = 360.0f / numOrbiters;

        for (int i = 0; i < numOrbiters; i++)
        {
            // Calculate the position for each orbiter, considering the current system rotation
            float angle = currentSystemAngle + angleStep * i;
            Vector3 position = CalculateOrbiterPosition(angle);
            orbiters[i].transform.position = position;
        }
    }

    private Vector3 CalculateOrbiterPosition(float angle)
    {
        // Convert angle to radians and calculate position
        float radianAngle = angle * Mathf.Deg2Rad;
        float x = player.transform.position.x + orbitDistance * Mathf.Cos(radianAngle);
        float y = player.transform.position.y + orbitDistance * Mathf.Sin(radianAngle);
        return new Vector3(x, y, player.transform.position.z);
    }
}
