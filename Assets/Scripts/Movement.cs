using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
    Button startButton;

    Rigidbody rb;
    AudioSource audioSource;

    //for tracking game state
    bool isPaused = false;
    bool isStarted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        startButton = uiDocument.rootVisualElement.Q<Button>("StartButton");

        resumeButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        startButton.style.display = DisplayStyle.None;

        resumeButton.clicked += Unpause;
        restartButton.clicked += RestartGame;
        startButton.clicked += StartGame;

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

        //if game has been paused by user
        if (isPaused == true)
        {
            Pause();
        }
        //if game has not started yet 
        else if (isStarted == false)
        {
            startButton.style.display = DisplayStyle.Flex;
            Time.timeScale = 0;
        }
        else if (isStarted == true)
        {
            StartGame();
        }
        else
        {
            Unpause();
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
        if (rotationInput < 0)
        {
            RotateRight();
        }
        else if (rotationInput > 0)
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

    //restarts entire game, sending player back to first level - resets health, current level score, and total score
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
        Unpause();
    }

    //stop game in entirety, bring up pause menu
    private void Pause()
    {
        Time.timeScale = 0;
        resumeButton.style.display = DisplayStyle.Flex;
        restartButton.style.display = DisplayStyle.Flex;
    }

    //restart game from pause screen
    private void Unpause()
    {
        Time.timeScale = 1;
        resumeButton.style.display = DisplayStyle.None;
        restartButton.style.display = DisplayStyle.None;
        startButton.style.display = DisplayStyle.None;
        isPaused = false;
    }

    //start game from start screen
    private void StartGame()
    {
        Time.timeScale = 1;
        startButton.style.display = DisplayStyle.None;
        isStarted = true;
    }
}
