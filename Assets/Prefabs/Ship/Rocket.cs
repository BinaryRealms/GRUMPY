using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 150f;
    [SerializeField] float rcsThrust = 100f; // [SerializeField] define que a variavel seja modificada no inspetor da Unity mas não por outros scripts.
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip rcsThrustSound;
    [SerializeField] AudioClip shipExplosionSound;
    [SerializeField] AudioClip shipReachedObjectiveSound;

    [SerializeField] ParticleSystem mainThrusterParticlesRight;
    [SerializeField] ParticleSystem mainThrusterParticlesLeft;
    [SerializeField] ParticleSystem rcsThrustParticlesRight;
    [SerializeField] ParticleSystem rcsThrustParticlesLeft;
    [SerializeField] ParticleSystem shipExplosionParticles;
    [SerializeField] ParticleSystem shipReachedObjectiveParticles;

    Rigidbody shipRigidBody;
    AudioSource shipSounds;
    // TODO - add shipManeuverThrustersSound

    enum ShipStatus { Intact, Damaged, Destroyed, Landed}
    ShipStatus shipStatus = ShipStatus.Intact;

    bool collisionsAreDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        shipSounds = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ShipMovimentInput();

        if (Debug.isDebugBuild) // Validates is the game in a debug build
        {
            RespondToDebugKeys();
        }
    }

    private void ShipMovimentInput()
    {
        if (shipStatus == ShipStatus.Intact)
        {
            ShipThrustInput();
            ShipRotationInput();
        }
        else
        {
        }

    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreDisabled = !collisionsAreDisabled;
        }
    }

    private void OnCollisionEnter(Collision shipCollision) // adds response to collision
    {
        if (shipStatus != ShipStatus.Intact || collisionsAreDisabled) { return; } // se o status da nave não for "Intact", o return faz com que o método seja finalizado antes de terminar a execução.

        switch (shipCollision.gameObject.tag)
        {
            case "LaunchPad":
                // Starting pad
                print("Why are you here again?");
                break;
            case "Fuel":
                // Increases fuel
                print("YUMY!");
                break;
            case "HardSurface":
                // Kill player
                print("Outch... THAT IS A LOT OF DAMAGE!!!");
                ShipLost();
                break;
            case "LandingPad":
                // Win
                ObjectiveReached();
                break;
        }
    }

    private void ShipLost()
    {
        shipStatus = ShipStatus.Destroyed;
        ThrustersParticlesStop();
        shipSounds.Stop(); //stops the thrusting sound
        shipSounds.PlayOneShot(shipExplosionSound);
        shipExplosionParticles.Play();
        Invoke("ReloadCurrentLevel", levelLoadDelay); // Invoke demands the use of a string (the method needs to be given to invoke as a string)
    }

    private void ObjectiveReached()
    {
        shipStatus = ShipStatus.Landed;
        ThrustersParticlesStop();
        shipSounds.Stop(); //stops the thrusting sound
        shipSounds.PlayOneShot(shipReachedObjectiveSound);
        shipReachedObjectiveParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // Invoke demands the use of a string (the method needs to be given to invoke as a string)
    }

    private void LoadFirstLevel() //Unused
    {
        SceneManager.LoadScene(0);
    }

    private void ReloadCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;    
        int nextSceneIndex = nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //loop back to start
        }

        SceneManager.LoadScene(nextSceneIndex); // TODO: Allow more than 2 levels.
    }

    private void ShipThrustInput()
    {
        float mainThrustPower = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // Boost the ship forward
        {
            ApplyThrust(mainThrustPower);
        }
        else if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            PlayShipThrustersSound();
            ThrustersParticlesStop();
        }
    }
    
    private void ApplyThrust(float mainThrustPower)
    {
        shipRigidBody.AddRelativeForce(Vector3.up * mainThrustPower); /* * Time.deltaTime ); TODO: try to make this work */ // Define a força aplicada no objjeto em uma direção específica.
        PlayShipThrustersSound();
        ThrustersParticlesPlay();
    }
    
    private void ShipRotationInput()
    {
        float rotationThrust = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) // Rotate ship right
        {
            ShipRotateRight(rotationThrust);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // Rotate ship left
        {
            ShipRotateLeft(rotationThrust);
        }
        else
        {
            PlayShipThrustersSound();
            ThrustersParticlesStop();
        }
    }       

    private void ShipRotateRight(float rotationThrust)
    {
        shipRigidBody.freezeRotation = true; // Take manual control of rotation
        transform.Rotate(Vector3.back * rotationThrust); // Rotaciona o objeto no eixo "Z"(Vector3 = Z) - Negativamente
        shipRigidBody.freezeRotation = false; // resume physics control of rotation
        PlayShipThrustersSound();
        ThrustersParticlesPlay();
    }

    private void ShipRotateLeft(float rotationThrust)
    {
        shipRigidBody.freezeRotation = true; // Take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThrust); // Rotaciona o objeto no eixo "Z"(Vector3 = Z) - Positivamente
        shipRigidBody.freezeRotation = false; // resume physics control of rotation
        PlayShipThrustersSound();
        ThrustersParticlesPlay();
    }

    private void ThrustersParticlesPlay()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            mainThrusterParticlesRight.Play();
            mainThrusterParticlesLeft.Play();
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rcsThrustParticlesLeft.Play();
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rcsThrustParticlesRight.Play();
        }
    }

    private void ThrustersParticlesStop()
    {
        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow))
        {
            mainThrusterParticlesRight.Stop();
            mainThrusterParticlesLeft.Stop();
        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
        {
            rcsThrustParticlesLeft.Stop();
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow))
        {
            rcsThrustParticlesRight.Stop();
        }

        if (shipStatus == ShipStatus.Destroyed) // Stops all thruster particles on death
        {
            mainThrusterParticlesRight.Stop();
            mainThrusterParticlesLeft.Stop();
            rcsThrustParticlesRight.Stop();
            rcsThrustParticlesLeft.Stop();
        }

    }

    private void PlayShipThrustersSound()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (!shipSounds.isPlaying)
            {
                shipSounds.PlayOneShot(mainEngineSound);
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!shipSounds.isPlaying)
            {
                shipSounds.PlayOneShot(rcsThrustSound);
            }
        }
        else
        {
            shipSounds.Stop();
        }
    }
}
