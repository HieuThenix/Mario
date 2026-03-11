using UnityEngine;
using System; // <--- ADD THIS to use Enum functions

public enum PoweredupItem
{
    mushroom,
    flower,
    star
}

public class Collectible : MonoBehaviour
{
    public string itemName; 

    void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Check if the item is a power-up before calling the function
                // Note: We use 'true' as the 3rd argument to ignore case (mushroom vs Mushroom)
                itemName = itemName.ToLower();
                if (Enum.IsDefined(typeof(PoweredupItem), itemName))
                {
                    player.OnPoweredUpItemCollected(itemName);
                    Collect(); // Only destroy after the player gets the item
                }
                else 
                {
                    // If it's just a coin or something else, just collect it
                    Collect();
                }
            }
        }
    }

    void Collect()
    {
        Destroy(gameObject);
    }
}