using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Basic Information")]
    public string enemyName = "Goomba";
    public int scoreValue = 100;

    [Header("Movement Stats")]
    public float moveSpeed = 1f;

    [Header("Patrol Detection")]
    // The LayerMask the raycasts check against — assign "Ground" layer here
    public LayerMask groundLayer;

    // How far in front of the enemy we check for a wall (horizontal ray)
    public float wallDetectionDistance = 0.5f;

    // How far ahead of the feet we place the ledge-check ray origin (horizontal offset)
    public float ledgeCheckOffset = 0.6f;

    // How far down we cast the ledge ray; should clear one full tile height
    public float ledgeDetectionDistance = 0.5f;

    // Bounce force for Mario to stomp enemy
    public float bounceForce = 400f;
}