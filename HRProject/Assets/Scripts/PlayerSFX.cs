using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    PlayerMotor motor;

    [SerializeField]
    private AudioClip walkingSFX;

    [SerializeField]
    private AudioSource asource;

    string playstate;

    //Interval in seconds for how often the walking sfx plays
    public float walkSFXInterval;

    //Lower threshold for if the walking sfx will play after player lets go of WASD, will be obsolete if raw WASD input can be read at some point
    public float speedThreshold;

    float walkSFXtimer = 0;
    bool isWalking = false, gotToHighSpeed = false;
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        asource.clip = walkingSFX;
        walkSFXtimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        playstate = motor.CurrentState.ToString();
        Debug.Log(playstate);
        Debug.Log("Player speed = " + motor.CurrentSpeed);
        if (motor.CurrentSpeed > 0.7f && playstate == "Grounded")
        {
            if (!gotToHighSpeed)
            {
                if (!isWalking) asource.Play();
                isWalking = true;
            }else
            {
                if (motor.CurrentSpeed >= speedThreshold && playstate == "Grounded")
                {
                    if (!isWalking) asource.Play();
                    isWalking = true;
                }else if (motor.CurrentSpeed < speedThreshold)
                {
                    gotToHighSpeed = false;
                }
            }
        }else
        {
            gotToHighSpeed = false;
            isWalking = false;
            walkSFXtimer = 0;
        }

        if (isWalking)
        {
            if (motor.CurrentSpeed >= speedThreshold) gotToHighSpeed = true;
            if (walkSFXtimer < walkSFXInterval)
            {
                walkSFXtimer += Time.deltaTime;
            }else if (walkSFXtimer >= walkSFXInterval)
            {
                //asource.Stop();
                asource.PlayOneShot(asource.clip, asource.volume);
                walkSFXtimer = 0;
            }
        }
    }
}
