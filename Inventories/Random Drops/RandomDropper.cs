using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.AI;
using RPG.Inventories;
using RPG.Stats;
public class RandomDropper : ItemDropper
{
   // CONFIG DATA
   [Tooltip("How far can the pickups be  scatteed from the dorpper.")]
   [SerializeField] float scatterDistance = 1;
   [SerializeField] DropLibrary dropLibrary;

    // CONSTANTS
    const int ATTEMPTS = 30;

    public void RandomDrop()
    {
        var baseStats = GetComponent<BaseStats>();
      
        var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
        foreach  (var drop in drops)
        {
            DropItem(drop.item, drop.number);
        }
    }

    protected override Vector3 GetDropLocation()
    {
        for (int i = 0; i < ATTEMPTS; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
            NavMeshHit hit;

                if(NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
        }
            return transform.position;
    }
}
