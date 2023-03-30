using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    PlayerMotor motor;
    public AudioClip walkingSFX;
    AudioSource asource;

    //Interval in seconds for how often the walking sfx plays
    public float walkSFXInterval;

    float walkSFXtimer = 0;
    bool isWalking = false;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        asource = GetComponentInChildren<AudioSource>();
        asource.clip = walkingSFX;
        walkSFXtimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Player is on ground = " + motor.IsOnGround);
        //Debug.Log("Player speed = " + motor.CurrentSpeed);
        if (motor.CurrentSpeed > 0.5f && motor.IsOnGround)
        {
            if (!isWalking) asource.Play();
            isWalking = true;
        }else
        {
            asource.Stop();
            isWalking = false;
            walkSFXtimer = 0;
        }

        if (isWalking)
        {
            if (walkSFXtimer < walkSFXInterval)
            {
                walkSFXtimer += Time.deltaTime;
            }else if (walkSFXtimer >= walkSFXInterval)
            {
                asource.Stop();
                asource.Play();
                walkSFXtimer = 0;
            }
        }
    }
}
