using UnityEngine;


public class PlayerCollisionHandler : MonoBehaviour
{
    public GameManager gameManager;


    void OnTriggerEnter(Collider other)
    {


        Transform root = other.transform.root;

        if (other.CompareTag("Jewel"))
        {
            gameManager.AddScore(100);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Spike"))
        {
            gameManager.AddScore(-100);
            Destroy(other.gameObject);
        }
    }
}