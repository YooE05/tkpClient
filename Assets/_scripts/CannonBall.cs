using Mirror;
using UnityEngine;

public class CannonBall : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Untagged")
        {           
            gameObject.SetActive(false);
        }
    }

}
