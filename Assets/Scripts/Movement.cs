using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{   
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] float thrustStrength = 100f;
    [SerializeField] float rotationStrength = 100f;
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    [SerializeField] ParticleSystem leftThrustParticles;

    [SerializeField] UIDocument uiDocument;
    Button resumeButton;
    Button restartButton;

    Rigidbody rb;
    AudioSource audioSource;

    bool isPaused = false;

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        resumeButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;  
    }

    private void OnEnable() 
    {
        thrust.Enable(); 
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
        CheckForPause();
        if (isPaused == true)
        {
            Time.timeScale = 0;
            resumeButton.style.display = DisplayStyle.Flex;
            restartButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1;
            resumeButton.style.display = DisplayStyle.None;
            restartButton.style.display = DisplayStyle.None;
        }
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if(rotationInput < 0)
        {
            RotateRight();
        }
        else if(rotationInput > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotating();
        }
    }

    private void RotateRight()
    {
        ApplyRotation(rotationStrength);
        if (!rightThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Play();
        }
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationStrength);
        if (!leftThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
            leftThrustParticles.Play();
        }
    }

    private void StopRotating()
    {
        rightThrustParticles.Stop();
        leftThrustParticles.Stop();
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }

    private void CheckForPause()
    {
        if (Keyboard.current.pKey.isPressed == true)
        {
            isPaused = true;
        }
    }
}
