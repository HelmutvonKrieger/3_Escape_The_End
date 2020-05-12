
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
            Thrust();
            Rotate();
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
                state = State.Transcending;
                Invoke("LoadLevel", 1f);
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    #region Private methods

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float thrustThisFrame = mainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);

            // To avoid layering of audio
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            //if (!particleSystem.IsAlive())
            //{
            //    particleSystem.Play();
            //}
        }
        else
        {
            audioSource.Stop();
            //particleSystem.Stop();
        }
    }

    private void Rotate()
    {
        // Take manual control of rotation
        rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        // Return control to physics
        rigidBody.freezeRotation = false;
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
