using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitSystem : MonoBehaviour
{
    private PlayerController player;
    private List<GameObject> orbiters; // List of orbiter GameObjects
    private readonly float orbitDistance = 1.0f; // Distance from the player

    void Awake()
    {
        orbiters = new List<GameObject>();
        player = GetComponentInParent<PlayerController>();
    }

    public void AddOrbiter(OrbiterData newOrbiter)
    {
        // Instantiate the new orbiter and add it to the list
        GameObject orbiterInstance = Instantiate(newOrbiter.gameObject, transform);
        orbiters.Add(orbiterInstance);

        // Rearrange all orbiters to be equidistant
        RearrangeOrbiters();
    }

    private void RearrangeOrbiters()
    {
        int numOrbiters = orbiters.Count;
        float angleStep = 360.0f / numOrbiters;

        for (int i = 0; i < numOrbiters; i++)
        {
            // Calculate the position for each orbiter
            float angle = angleStep * i;
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
