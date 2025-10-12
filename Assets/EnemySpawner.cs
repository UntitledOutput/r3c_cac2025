using System;
using System.Collections.Generic;
using External;
using ScriptableObj;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int SpawnCount;
    public float SpawnRadius;
    
    public bool OnlyRunIfEnd;
    public bool OnlyRunIfMatchEnd;
    public bool OnlyRunIfNotEnd;
    public bool OnlyRunIfNotMatchEnd;
    
    public BaseUtils.WeightedList<EnemyObject> enemies;

    [DoNotSerialize] public bool IsEnd;
    [DoNotSerialize] public bool IsMatchEnd;
    
    public bool MakeMega;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if ((OnlyRunIfEnd && IsEnd) ||
            (OnlyRunIfNotEnd && !IsEnd) ||
            (OnlyRunIfMatchEnd && IsMatchEnd) ||
            (OnlyRunIfNotMatchEnd && !IsMatchEnd) ||
            (!OnlyRunIfNotEnd && !OnlyRunIfNotEnd && !OnlyRunIfMatchEnd && !OnlyRunIfNotMatchEnd))
        {
            var deg = (360.0f / SpawnCount) * Mathf.Deg2Rad;
            for (int i = 0; i < SpawnCount; i++)
            {
                var pos = new Vector3(Mathf.Sin(deg * i), 0, Mathf.Cos(deg * i)) * SpawnRadius;

                var enemy = Instantiate(
                    MakeMega ? Resources.Load<GameObject>("Prefabs/LargeEnemy") : Resources.Load<GameObject>("Prefabs/SmallEnemy"), transform.position + pos + new Vector3(0, 0.1f, 0),
                    Quaternion.Euler(0,180,0));

                enemy.GetComponent<EnemyController>().enemyObject = enemies.GetRandom();
                
                if (MakeMega)
                {
                    enemy.GetComponent<EnemyController>().IsMegaEnemy = true;
                }
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
