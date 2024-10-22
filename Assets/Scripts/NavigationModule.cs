using UnityEngine;

public class NavigationModule : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("The maximum speed at which the enemy is moving (in world units per second).")]
    public float moveSpeed = 0f;
    [Tooltip("The maximum speed at which the enemy is rotating (degrees per second).")]
    public float angularSpeed = 0f;
    [Tooltip("The acceleration to reach the maximum speed (in world units per second squared).")]
    public float acceleration = 0f;
}
