using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] int healthAmount = 3;
    [SerializeField] AudioClip successSFX;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    [SerializeField] UIDocument uiDocument;
    private Label healthLabel;
    
    AudioSource audioSource;

    bool isControllable = true;
    bool isCollidable = true;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        healthLabel = uiDocument.rootVisualElement.Q<Label>("HealthLabel");
        healthAmount += 1;
    }
    
    private void Update() 
    {
        RespondToDebugKeys();
        healthLabel.text = " ❤" + healthAmount;
    }

    void RespondToDebugKeys()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            LoadNextLevel();
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            isCollidable = !isCollidable;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (!isControllable || !isCollidable) { return; }

        string objTag = other.gameObject.tag;
        
        if (objTag == "Friendly")
        {
            Debug.Log("Everything is looking good!");
        }
        if (objTag == "Finish")
        {
            StartSuccessSequence();
        }
        else
        {
            healthAmount -= 1;
            healthLabel.text = " ❤" + healthAmount;
            healthLabel.style.display = DisplayStyle.Flex;
            if (healthAmount == 0)
            {
                StartCrashSequence();
            }
        }
    }

    void StartSuccessSequence()
    {
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(successSFX);
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence()
    {
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(crashSFX);
        crashParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", levelLoadDelay);
    }

    void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
        
        SceneManager.LoadScene(nextScene);
    }

    void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

}
