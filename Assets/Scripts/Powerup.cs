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
    private float _magnetSpeed = 1.0f;
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private AudioClip _clip;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetKey(KeyCode.C))
        {
            Magnet();
        }

    }

    private void Magnet()
    {
        transform.position = Vector3.Lerp(this.transform.position, Player.transform.position, _magnetSpeed * Time.deltaTime);
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
                        player.SlowSpeedActive();
                        break;
                    case 5:
                        player.SuperLaserActive();
                        break;
                    case 6:
                        player.RestoreLives();
                        break;
                    case 7:
                        player.MissileReady();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
        }

    }

}
