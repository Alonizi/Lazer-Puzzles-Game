// copyrights Abdulaziz Alonizi 2025
using UnityEngine;

/// <summary>
/// Basic Player Control Script
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D RigidComponent; 
    /// <summary>
    /// Cache RigidBody2d Component
    /// </summary>
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RigidComponent = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// update player's linear velocity based on horizontal/vertical inputs
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        RigidComponent.linearVelocity = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
    }
}
