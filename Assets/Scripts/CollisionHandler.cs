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
    private Label scoreLabel;
    private Label winLabel;
    private Button restartButton;
    private Label gameOverLabel;

    AudioSource audioSource;

    bool isControllable = true;
    bool isCollidable = true;

    //the total score of the overall game
    static int totalScore = 0;
    ////the temporary score of each individual level
    int currentScore = 0;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();

        healthLabel = uiDocument.rootVisualElement.Q<Label>("HealthLabel");
        scoreLabel = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        winLabel = uiDocument.rootVisualElement.Q<Label>("WinLabel");
        gameOverLabel = uiDocument.rootVisualElement.Q<Label>("GameOverLabel");

        winLabel.style.display = DisplayStyle.None;
        gameOverLabel.style.display = DisplayStyle.None;

        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");

        restartButton.clicked += RestartGame;

    }
    
    private void Update() 
    {
        RespondToDebugKeys();
        healthLabel.text = " ❤" + healthAmount;
        scoreLabel.text = " ★" + (totalScore + currentScore);
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
        else if (objTag == "Finish")
        {
            StartSuccessSequence();
        }
        else if (objTag == "Collectible")
        {
            AddScore(other);
        }

        //if collision occurs with obstacle or part of environment
        else
        {
            TakeDamage();
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
        winLabel.style.display = DisplayStyle.Flex;

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

    void TakeDamage()
    {
        healthAmount -= 1;
        healthLabel.text = " ❤" + healthAmount;
        healthLabel.style.display = DisplayStyle.Flex;
    }

    void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        winLabel.style.display = DisplayStyle.None;

        //if current level is last level, display Game Over screen
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            gameOverLabel.style.display = DisplayStyle.Flex;
            restartButton.text = "Play Again";
            restartButton.style.display = DisplayStyle.Flex;
        }

        //if current level is NOT last, load next level
        else
        {
            SceneManager.LoadScene(nextScene);
            totalScore += currentScore;
            currentScore = 0;
        }
    }

    //reloads current level - resets health and current level score
    void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
        currentScore = 0;
    }

    //restarts entire game, sending player back to first level - resets health, current level score, and total score
    void RestartGame()
    {
        SceneManager.LoadScene(0);
        currentScore = 0;
        totalScore = 0;
    }

    void AddScore(Collision other)
    {
        currentScore += 1;
        scoreLabel.text = " ★" + (totalScore + currentScore);
        scoreLabel.style.display = DisplayStyle.Flex;
        Destroy(other.gameObject);
    }

}
