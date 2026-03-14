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
                // OPTIMIZATION: TryParse is much faster than IsDefined and ignores case automatically if we pass 'true'
                if (Enum.TryParse(itemName, true, out PoweredupItem parsedItem))
                {
                    player.OnPoweredUpItemCollected(itemName);
                    Collect(); 
                }
                else 
                {   
                    GameManager.instance.AddCoin();
                    GameManager.instance.AddScore(200);
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