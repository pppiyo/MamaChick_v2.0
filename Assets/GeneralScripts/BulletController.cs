using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class BulletController : MonoBehaviour
{
    public float speed = 8f;  // Adjust this to control the bullet speed.
    public GameObject player; // Reference to the PlayerMovement script.
    private string prefabName;

    private void Start()
    {
        // Get the name of the prefab instance by accessing the name property of the GameObject.
        prefabName = gameObject.name;
        // Debug.Log("Prefab name: " + prefabName);
    }


    public void Initialize(Vector2 direction)
    {
        // Set the initial velocity to move the bullet.
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the bullet is off-screen and destroy it.
        void DestroyOutOfBounds()
        {
            if (transform.position.x < -20 || transform.position.x > 20) // destroy it when out of the scene
            {
                Destroy(gameObject);
            }
        }

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D obstacle)
    {
        // Debug.Log("!!!!!!! number and bullet collided!!!!!: " + player);
        if (obstacle.gameObject.CompareTag("Gun"))
        { return; }


        if (obstacle.gameObject.CompareTag("Number") || obstacle.gameObject.CompareTag("Monster2"))
        {
            GameObject numberText = null;
            // Debug.Log("Number collided by bullet");
            if (obstacle.gameObject.CompareTag("Number"))
            {
                numberText = obstacle.gameObject.transform.Find("Number_Text").gameObject;
            }
            if (obstacle.gameObject.CompareTag("Monster2"))
            {
                numberText = obstacle.gameObject.transform.Find("Monster_Text").gameObject;
            }

            TMP_Text textComponent = numberText.GetComponent<TMP_Text>();
            // Debug.Log("Number collided: " + textComponent.text);
            if (textComponent != null)
            {
                player = GameObject.Find("Player");
                // Debug.Log("player: " + player.transform.position);

                GameObject playerTextGO = player.transform.Find("Player_Number").gameObject;
                TMP_Text playerText = playerTextGO.GetComponent<TMP_Text>();
                // Debug.Log("player text: " + playerText.text);

                // textComponent.text = playerText.text; // Change the text to whatever you want.
                int number = int.Parse(textComponent.text);
                int playerNumber = int.Parse(playerText.text);

                // Debug.Log("Prefab name: " + prefabName);

                if (prefabName == "BulletAdd(Clone)")
                {
                    // Debug.Log("add");
                    textComponent.text = (playerNumber + number).ToString();
                }
                else if (prefabName == "BulletSub(Clone)")
                {
                    textComponent.text = (playerNumber - number).ToString();
                }
                else if (prefabName == "BulletMultiply(Clone)")
                {
                    textComponent.text = (playerNumber * number).ToString();
                }
                else if (prefabName == "BulletDivide(Clone)")
                {
                    if (number != 0)
                    {
                        textComponent.text = (playerNumber / number).ToString();
                    }
                    else
                    {
                        textComponent.text = "0";
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform_Solid") || other.gameObject.CompareTag("Platform_Mutate"))
        {
            Destroy(gameObject);
        }
    }
}