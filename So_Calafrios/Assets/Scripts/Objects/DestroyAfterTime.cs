using UnityEngine;

/// <summary>
/// Class which destroy the object after an inserted amount of time.
/// </summary>
public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float refreshTime = default;
    [SerializeField] private float maxTime = default;
    private float period, timeDestroy;

    /// <summary>
    /// Private method called every frame.
    /// </summary>
    private void Update()
    {
        // This is for the battery of the flashlight.
        if (period > refreshTime)
        {
            timeDestroy++;
            if(timeDestroy == maxTime)
            {
                Destroy(gameObject);
            }
            period = 0;
        }

        period += Time.deltaTime;
    }
}