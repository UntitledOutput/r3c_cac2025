using Controllers;
using External;
using Unity.AI.Navigation;
using UnityEngine;

public class MapController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AttachSection(MatchController.MapSection section, Vector3 offset)
    {
        var obj = Instantiate(section.Ref.Prefab);
        obj.transform.SetParent(transform.Find("Sections"),false);
        obj.transform.position = offset;

        
        var connections = obj.transform.Find("ConnectionPoints");

        for (var i = 0; i < section.Nexts.Count; i++)
        {
            var sectionNext = section.Nexts[i];
            AttachSection(sectionNext,connections.GetChild(i).position + new Vector3(0,0,40));
        }

        if (section.Nexts.Count <= 0)
        {
            for (int i = 0; i < connections.childCount; i++)
            {
                var gate = Instantiate(Resources.Load<GameObject>("Prefabs/GateController"), transform);
                gate.transform.position = connections.GetChild(i).position - new Vector3(0, 0, 5);
            }

            var spawners = obj.GetComponentsInChildren<EnemySpawner>();
            foreach (var enemySpawner in spawners)
            {
                enemySpawner.IsEnd = true;
            }
        }
    }

    public void SetupMap(MatchController.MatchRound round)
    {
        transform.Find("Sections").RemoveAllChildren();
        if (GetComponentInChildren<GateController>()) Destroy(GetComponentInChildren<GateController>().gameObject);
        
        AttachSection(round.root, new Vector3(0,0,-80));

        var surfaces = GetComponents<NavMeshSurface>();
        foreach (var navMeshSurface in surfaces)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}
