
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
        Transcending
    }

    State state = State.Alive;

    [SerializeField]
    float rcsThrust = 100f;

    [SerializeField]
    float mainThrust = 50f;

    [SerializeField]
    AudioClip engineThrust;

    [SerializeField]
    AudioClip deathSound;

    [SerializeField]
    AudioClip endLevelChime;

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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
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
            audioSource.Stop();
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

    private void ApplyThrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

        // To avoid layering of audio
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(engineThrust);
        }
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
        audioSource.PlayOneShot(deathSound);
        Invoke("LoadFirstLevel", 1f);
    }

    private void EndLevelSequence()
    {
        state = State.Transcending;
        audioSource.PlayOneShot(endLevelChime);
        Invoke("LoadNextLevel", 1f);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    #endregion Private methods
}
