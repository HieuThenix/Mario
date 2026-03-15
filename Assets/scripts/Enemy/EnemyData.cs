using UnityEngine;

// This attribute adds a new item to Unity's Right-Click -> Create menu
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Basic Information")]
    public string enemyName = "Goomba";
    public int scoreValue = 100;
    
    [Header("Movement Stats")]
    public float moveSpeed = 2f;
    
}