using UnityEngine;
using System.Collections;
using Assets.MultiAudioListener;

public class Explosion : MonoBehaviour
{

    public MultiAudioSource AudioSource = null;
    public ParticleSystem Particles = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        AudioSource.Play();
            Particles.Play();
	    }
	}
}
