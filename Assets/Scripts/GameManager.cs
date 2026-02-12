using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject[] objectsToSpawn;
    public Eraser eraser;
    public Slider cleanProgressSlider;

    [Header("Values")]
    public float percentCleanThreshold;
    public float roundTransitionDuration;

    private TintErasable activeObject;
    private int currentObjectIndex;
    public static bool IsCleaning { get; private set; }

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
        IsCleaning = false;

        float elapsed = 0f;
        Vector3 startPoint = this.transform.position;
        Vector2 endPoint = new(this.transform.position.x, -10);

        // Slide downwards out of view.
        while (elapsed < this.roundTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / this.roundTransitionDuration;
            this.transform.position = Vector2.Lerp(startPoint, endPoint, t);
            yield return null;
        }
        this.transform.position = endPoint;

        // End game if last object.
        if (this.currentObjectIndex == this.objectsToSpawn.Length - 1)
        {
            this.EndGame();
            yield break;
        }

        // Change object.
        Destroy(this.activeObject);
        this.activeObject = SpawnObject(this.currentObjectIndex);

        // Slide upwards into view.
        elapsed = 0f;
        startPoint = this.transform.position;
        endPoint = new(this.transform.position.x, 0);

        while (elapsed < this.roundTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / this.roundTransitionDuration;
            this.transform.position = Vector2.Lerp(startPoint, endPoint, t);
            yield return null;
        }
        this.transform.position = endPoint;

        IsCleaning = true;
    }

    void EndGame()
    {
        Debug.Log("Game ended");
    }
}
