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
    private bool isCleaning = false;

    void Start()
    {
        this.currentObjectIndex = 0;
        this.activeObject = SpawnObject(this.currentObjectIndex);

        this.cleanProgressSlider.maxValue = this.percentCleanThreshold - 0.01f;

        this.isCleaning = true;
    }

    void Update()
    {
        if (!this.isCleaning)
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
        this.isCleaning = false;

        float elapsed = 0f;
        Vector2 endPoint = new(this.transform.position.x, -100);

        // Slide downwards out of view.
        while (elapsed < this.roundTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / this.roundTransitionDuration;
            Vector2.Lerp(this.transform.position, endPoint, t);
            yield return null;
        }

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
        endPoint = new(this.transform.position.x, 0);

        while (elapsed < this.roundTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / this.roundTransitionDuration;
            Vector2.Lerp(this.transform.position, endPoint, t);
            yield return null;
        }

        this.isCleaning = true;
    }

    void EndGame()
    {
        Debug.Log("Game ended");
    }
}
