using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject[] objectsToSpawn;
    public Eraser eraser;
    public Slider cleanProgressSlider;
    public IdleTextAnimation roundTextAnimation;
    public new Light2D light;
    public AudioSource audioSource;
    public AudioClip roundEndSound;

    [Header("Values")]
    public float percentCleanThreshold;
    public float roundTransitionDuration;

    private TintErasable activeObject;
    private int currentObjectIndex;
    public static bool IsCleaning { get; private set; }
    public float RoundProgress => this.currentObjectIndex / (float)this.objectsToSpawn.Length;

    void Start()
    {
        this.currentObjectIndex = 0;
        this.activeObject = SpawnObject(this.currentObjectIndex);

        this.cleanProgressSlider.maxValue = this.percentCleanThreshold - 0.01f;

        IsCleaning = true;
    }

    void Update()
    {
        if (!IsCleaning)
            return;

        this.cleanProgressSlider.value = this.activeObject.percentClean;
        if (this.activeObject.percentClean >= percentCleanThreshold)
            StartCoroutine(this.ShowNextObject());
    }

    TintErasable SpawnObject(int index)
    {
        Debug.Log($"Spawning object at index {index}");
        return Instantiate(objectsToSpawn[this.currentObjectIndex], this.transform).GetComponentInChildren<TintErasable>();
    }

    IEnumerator ShowNextObject()
    {
        // End round.
        IsCleaning = false;

        // Play effects.
        this.audioSource.PlayOneShot(this.roundEndSound);

        // Slide downwards out of view.
        {
            float elapsed = 0f;
            Vector3 startPoint = this.transform.position;
            Vector2 endPoint = new(this.transform.position.x, -10);

            while (elapsed < this.roundTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / this.roundTransitionDuration;
                this.transform.position = Vector2.Lerp(startPoint, endPoint, t);
                yield return null;
            }
            this.transform.position = endPoint;
        }

        // Prepare next object. Play effects.
        this.currentObjectIndex++;
        {
            // Increase vignette strength.
            this.light.falloffIntensity = RoundProgress;

            // Decrease volume.
            this.audioSource.volume = 1 - RoundProgress;
        }

        // End game if last object.
        if (this.currentObjectIndex == this.objectsToSpawn.Length)
        {
            this.EndGame();
            yield break;
        }

        // Change object.
        Destroy(this.activeObject.transform.parent.gameObject);
        this.activeObject = SpawnObject(this.currentObjectIndex);

        // Slide upwards into view.
        {
            float elapsed = 0f;
            Vector3 startPoint = this.transform.position;
            Vector3 endPoint = new(this.transform.position.x, 0);

            while (elapsed < this.roundTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / this.roundTransitionDuration;
                this.transform.position = Vector2.Lerp(startPoint, endPoint, t);
                yield return null;
            }
            this.transform.position = endPoint;
        }

        // Start round.
        IsCleaning = true;

        // Play effects.
        {
            // Emphasize text.
            this.roundTextAnimation.StartAnimation();
        }
    }

    void EndGame()
    {
        Debug.Log("Game ended");
    }
}
