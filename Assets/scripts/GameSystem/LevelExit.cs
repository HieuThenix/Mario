using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Header("Level Transition Settings")]
    [Tooltip("The exact name of the next scene to load.")]
    public string nextSceneName; 
    
    [Tooltip("Delay before loading starts (useful for victory animations/sounds).")]
    public float loadDelay = 1.5f; 

    private bool isLoading = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player hit the trigger and we aren't already loading
        if (other.CompareTag("Player") && !isLoading)
        {
            isLoading = true;

            // disable player movement here
            other.GetComponent<PlayerController>().enabled = false;
            
            // Start the asynchronous loading coroutine
            StartCoroutine(LoadNextSceneAsync());
        }
    }

    private IEnumerator LoadNextSceneAsync()
    {
        // 1. Wait for any victory fanfare or animations to play out
        if (loadDelay > 0)
        {
            yield return new WaitForSeconds(loadDelay);
        }

        // 2. Begin loading the scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // Optional: Prevent the scene from activating immediately if you want a "Press any key to continue" screen
        // asyncLoad.allowSceneActivation = false; 

        // 3. Wait until the background loading is completely finished
        while (!asyncLoad.isDone)
        {
            // Note: asyncLoad.progress goes from 0 to 0.9. 
            // 0.9 means it's fully loaded and ready to activate.
            // You could link a UI Loading Bar to this progress value later!
            
            yield return null; // Wait for the next frame before checking again
        }
    }
}