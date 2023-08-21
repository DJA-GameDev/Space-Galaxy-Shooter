using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int powerupID;

    [SerializeField]
    private AudioClip _clip;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Destroy(this.gameObject);

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            Player player = other.transform.GetComponent<Player>();

            if (player != null) 
            {

                switch(powerupID) 
                { 
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoCount(15);
                        break;
                    case 4:
                        player.RestoreLives();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
       
        }
    }

}
