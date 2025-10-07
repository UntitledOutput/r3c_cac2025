using Controllers;
using MyBox;
using ScriptableObj;
using UnityEngine;

public class TestLauncher : MonoBehaviour
{
    public AbilityObject ability;
    public float YMax = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [ButtonMethod]
    public void Shoot()
    {
        var bullet = Instantiate(ability.Prefab);
        bullet.layer = LayerMask.NameToLayer("Player");
        bullet.transform.position = transform.position;
        if (ability.Type == AbilityObject.AbilityType.Bomb)
            bullet.transform.position += transform.up;
        else
            bullet.transform.position += transform.forward;
                
        bullet.transform.eulerAngles = transform.eulerAngles;
        if (ability.Type == AbilityObject.AbilityType.Shooter)
            bullet.GetComponent<BulletController>().Derive(ability, null);
        else
        {
            bullet.GetComponent<BombController>().Derive(ability, null);
            bullet.GetComponent<BombController>().LaunchPosition = new Vector3(0,0,0);
            bullet.GetComponent<BombController>().yMax = YMax + Random.Range(-1.0f,1.0f);
            bullet.GetComponent<Rigidbody>().AddForce(new Vector3(0,2,0),ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
