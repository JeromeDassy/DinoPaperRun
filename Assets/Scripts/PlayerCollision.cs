using UnityEngine;

public class PlayerCollision : MonoBehaviour 
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision involves the GameObjects you're interested in
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Handle the collision here
            Debug.Log("GameOver");
            GameManager.Instance.GameOver();
        }
    }
}
