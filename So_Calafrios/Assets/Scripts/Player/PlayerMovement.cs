﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Class which manages the movement of the player and his effects.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput = default;
    [SerializeField] private CharacterController controller = default;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private AudioSource walkSound = default;
    [SerializeField] private AudioSource runSound = default;
    [SerializeField] private AudioSource breathingSound = default;
    [SerializeField] private Volume globalVolume = default;
    [SerializeField] private Animator flashlight = default;
    [SerializeField] private float runStamina = default;
    [SerializeField] private float refreshTime = default;
    [SerializeField] private float maxTiredTime = default;
    [SerializeField] private float tiredGain = default;
    [SerializeField] private float runMultiplier = default;
    [SerializeField] private float staminaMultiplier = default;
    private float x, z, walkingSpeed, stamina, period, tired;
    private bool firstTimeStoping, firstTimeWalking;
    private Vignette globalVignette;

    /// <summary>
    /// Private method called before the first frame.
    /// </summary>
    private void Start()
    {
        stamina = maxStamina;
        firstTimeStoping = false;
        firstTimeWalking = true;
        walkingSpeed = speed;
        globalVolume.profile.TryGet<Vignette>(out globalVignette);
    }
    
    /// <summary>
    /// Private method called every frame.
    /// </summary>
    private void Update()
    {
        // Get the input for the movement.
        x = playerInput.movementX;
        z = playerInput.movementZ;
        Stamina();
    }

    /// <summary>
    /// Private method called 60 times per second.
    /// </summary>
    private void FixedUpdate()
    {
        // Update player position with the movement input.
        Vector3 move = (transform.right * x * speed) + 
            (transform.forward * z * speed) + (transform.up * -15);
        controller.Move(move * Time.deltaTime);
    }

    /// <summary>
    /// Private method called every frame.
    /// </summary>
    private void Stamina()
    {
        // This is for the stamina of the character.
        if (period > refreshTime)
        {
            if(speed != walkingSpeed && (Input.GetButton("Horizontal") ||
                Input.GetButton("Vertical")))
            {
                stamina -= runStamina;
                tired = 0f;
            }
            else
            {
                if(tired < maxTiredTime)
                {
                    tired += tiredGain;
                }
                else
                {
                    tired = maxTiredTime;
                    stamina += runStamina*staminaMultiplier;
                }
            }

            // Minimum stamina.
            if(stamina <= 0f)
            {
                stamina = 0f;
            }
            // Maximum stamina.
            else if(stamina > maxStamina)
            {
                stamina = maxStamina;
            }

            if(stamina < 5)
            {
                if(!breathingSound.isPlaying)
                {
                    breathingSound.Play();
                }
                breathingSound.volume = 1 - (stamina / 0.5f * 0.1f);
                globalVignette.intensity.value = 0.5f - (stamina * 0.1f);
            }
            else
            {
                if(breathingSound.isPlaying)
                {
                    breathingSound.Stop();
                    breathingSound.volume = 0;
                    globalVignette.intensity.value = 0;
                }
            }
            period = 0;
        }

        period += Time.deltaTime;
    }

    /// <summary>
    /// Public method called when the movement inputs are not pressed.
    /// </summary>
    public void Stop()
    {
        // Stops walk/run sound, flashlight don't move and resets the bools.
        if(firstTimeStoping)
        {
            if(walkSound.isPlaying) {walkSound.Stop();}
            if(runSound.isPlaying) {runSound.Stop();}
            speed = walkingSpeed;
            flashlight.SetBool("Walking", false);

            firstTimeStoping = false;
            firstTimeWalking = true;
        }
    }

    /// <summary>
    /// Public method called when the movement inputs are pressed.
    /// </summary>
    /// <param name="startRun">Bool that is responsible to know when the
    /// player starts to run.</param>
    /// <param name="stopRun">Bool that is responsible to know when the
    /// player stops to run.</param>
    public void Walk(bool startRun, bool stopRun)
    {

        // Starts to walk. 
        // Activates sounds and flashlight movement.
        if(firstTimeWalking)
        {
            if(runSound.isPlaying) {runSound.Stop();}
            walkSound.Play();
            flashlight.SetBool("Walking", true);

            firstTimeStoping = true;
            firstTimeWalking = false;
        }
        // Stop running, no run sound, and is ready to activate the walk. 
        else if((stopRun || stamina <= 0f) && speed != walkingSpeed)
        {
            if(runSound.isPlaying) {runSound.Stop();}
            flashlight.SetBool("Walking", false);
            firstTimeWalking = true;
            speed = walkingSpeed;
        }
        // Starts running, stopping walk sound if activated and starts run
        // sound, and flashlight movement if not activated.
        else if(startRun && stamina > 0f)
        {
            if(walkSound.isPlaying) {walkSound.Stop();}
            speed = walkingSpeed * runMultiplier;
            if(!runSound.isPlaying) {runSound.Play();}
            flashlight.SetBool("Walking", true);

            firstTimeStoping = true;
            firstTimeWalking = false;
        }
    }
}
