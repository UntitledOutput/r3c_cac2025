using UnityEngine;
using Utils;

public class TestCamera : MonoBehaviour
{
    public float Radius = 3;
    public float Y = 0;
    public Vector3 FocusPoint;
    
    public float Angle = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Angle += InputSystem.Move.x;
        
        var pos = new Vector3(Mathf.Sin(Mathf.Deg2Rad * Angle)*Radius, Y, Mathf.Cos(Mathf.Deg2Rad * Angle)*Radius);


        transform.position = pos;
        transform.LookAt(FocusPoint);
    }
}
