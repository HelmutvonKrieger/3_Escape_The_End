
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    //ParticleSystem particleSystem;

    enum State
    {
        Alive,
        Dying,
        Transcending,
        Paused
    }

    State state = State.Alive;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;

    [SerializeField] AudioClip engineThrust;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip endLevelChime;

    [SerializeField] ParticleSystem engineThrustParticles;
    [SerializeField] ParticleSystem deathExplosionParticles;
    [SerializeField] ParticleSystem successParticles;

    [SerializeField] float levelLoadDelay = 1f;

    bool collisionsAllowed = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        //particleSystem = GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        RespondToPauseInput();

        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsAllowed)
        {
            Debug.LogWarning(state);
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print("ok");
                break;
            case "Finish":
                EndLevelSequence();
                break;
            default:
                DeathSequence();
                break;
        }
    }

    #region Private methods

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            CancelThrust();
        }
    }

    private void RespondToRotateInput()
    {
        // Take manual control of rotation
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            ApplyLeftRotate(rotationThisFrame);
        }

        if (Input.GetKey(KeyCode.D))
        {
            ApplyRightRotate(rotationThisFrame);
        }

        // Return control to physics
        rigidBody.freezeRotation = false;
    }

    private void RespondToPauseInput()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            State stateBeforePause = state;

            if (state != State.Paused)
            {
                state = State.Paused;
                Time.timeScale = 0;
                ShowPauseMenu();
            }
            else
            {
                ClosePauseMenu();
                Time.timeScale = 1;
                state = stateBeforePause;
            }
        }
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAllowed = !collisionsAllowed;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Invoke("LoadNextLevel", levelLoadDelay);
        }
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        // To avoid layering of audio
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(engineThrust);
        }
        engineThrustParticles.Play();
    }

    private void CancelThrust()
    {
        audioSource.Stop();
        engineThrustParticles.Stop();
    }

    private void ApplyLeftRotate(float rotation)
    {
        transform.Rotate(Vector3.forward * rotation);
    }

    private void ApplyRightRotate(float rotation)
    {
        transform.Rotate(Vector3.back * rotation);
    }

    private void DeathSequence()
    {
        state = State.Dying;
        CancelThrust();
        deathExplosionParticles.Play();
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void EndLevelSequence()
    {
        state = State.Transcending;
        CancelThrust();
        successParticles.Play();
        audioSource.PlayOneShot(endLevelChime);
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void ShowPauseMenu()
    {
        PauseMenu pm = GetComponent<PauseMenu>();
        pm.ShowMenu();
    }

    private void ClosePauseMenu()
    {

    }

    private void LoadNextLevel()
    {
        int currSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currSceneIndex + 1 < SceneManager.sceneCountInBuildSettings ? currSceneIndex + 1 : 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(1);
    }

    #endregion Private methods
}
