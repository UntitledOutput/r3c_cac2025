using System;
using System.Collections.Generic;
using External;
using ScriptableObj;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int SpawnCount;
    public float SpawnRadius;
    public bool OnlyRunIfEnd;
    public bool OnlyRunIfNotEnd;
    
    public BaseUtils.WeightedList<EnemyObject> enemies;

    public bool IsEnd;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if ((OnlyRunIfEnd && IsEnd) || (OnlyRunIfNotEnd && !IsEnd) || (!OnlyRunIfNotEnd && !OnlyRunIfNotEnd))
        {
            var deg = (360.0f / SpawnCount) * Mathf.Deg2Rad;
            for (int i = 0; i < SpawnCount; i++)
            {
                var pos = new Vector3(Mathf.Sin(deg * i), 0, Mathf.Cos(deg * i)) * SpawnRadius;

                var enemy = Instantiate(
                    enemies.GetRandom().Prefab, transform.position + pos + new Vector3(0, 0.1f, 0),
                    Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1,0,1));
        Gizmos.DrawWireSphere(transform.position,SpawnRadius);

        var deg = (360.0f / SpawnCount) * Mathf.Deg2Rad;
        for (int i = 0; i < SpawnCount; i++)
        {
            var pos = new Vector3(Mathf.Sin(deg*i),0, Mathf.Cos(deg*i)) * SpawnRadius;
            Gizmos.DrawWireSphere(pos + transform.position, 0.5f);
        }
    }
}
