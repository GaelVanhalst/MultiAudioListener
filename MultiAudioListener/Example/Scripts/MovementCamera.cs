using UnityEngine;
using System.Collections;

public class MovementCamera : MonoBehaviour
{

    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Forward;
    public KeyCode Backward;
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKey(Left))transform.Rotate(0,-30.0f*Time.deltaTime,0);
        if (Input.GetKey(Right)) transform.Rotate(0, 30.0f * Time.deltaTime, 0);

        if (Input.GetKey(Forward)) transform.Translate(0,0,10*Time.deltaTime);
        if (Input.GetKey(Backward)) transform.Translate(0,0,-10*Time.deltaTime);
    }
}
