using System;
using System.Collections.Generic;
using Controllers;
using DefaultNamespace;
using External;
using ScriptableObj;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int SpawnCount;
    public float SpawnRadius;
    
    public int SeverityCountIncrease = 0;
    public AnimationCurve SeverityCurve = AnimationCurve.Linear(0,1,1,1);


    public bool OnlyRunIfEnd;
    public bool OnlyRunIfMatchEnd;
    public bool OnlyRunIfNotEnd;
    public bool OnlyRunIfNotMatchEnd;

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
            int count = (int)(SpawnCount +
                              (SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel) *
                               SeverityCountIncrease));
            var deg = (360.0f / count) * Mathf.Deg2Rad;
            for (int i = 0; i < count; i++)
            {
                var pos = new Vector3(Mathf.Sin(deg * i), 0, Mathf.Cos(deg * i)) * SpawnRadius;

                var enemy = Instantiate(
                    MakeMega ? Resources.Load<GameObject>("Prefabs/LargeEnemy") : Resources.Load<GameObject>("Prefabs/SmallEnemy"), transform.position + pos + new Vector3(0, 0.1f, 0),
                    Quaternion.Euler(0,180,0));

                var list = MakeMega
                    ? DataController.saveData.NextMap.BigEnemies
                    : DataController.saveData.NextMap.SmallEnemies;

                enemy.GetComponent<EnemyController>().enemyObject = list.GetRandom();
                
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

        int count = (int)(SpawnCount +
                          (SeverityCurve.Evaluate((float)MatchController._instance.SeverityLevel/MatchController.MaxSeverityLevel) * SeverityCountIncrease));


        var deg = (360.0f / count) * Mathf.Deg2Rad;
        for (int i = 0; i < count; i++)
        {
            var pos = new Vector3(Mathf.Sin(deg*i),0, Mathf.Cos(deg*i)) * SpawnRadius;
            Gizmos.DrawWireSphere(pos + transform.position, 0.5f);
        }
    }
}
