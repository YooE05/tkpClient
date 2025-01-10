using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public int StartHealth;
    public int CurrentHealth { get; private set; }

    private ViewController _viewController;
    private WorldInitializer _worldInitializer;

    private NetworkObjectToggle _networkObjectToggle;
    // public bool IsDead => CurrentHealth <= 0;

    //  [SyncVar] public bool _isEnabled;


    private void Awake()
    {
        _viewController = FindObjectOfType<ViewController>();
        _worldInitializer = FindObjectOfType<WorldInitializer>();
        _networkObjectToggle = GetComponent<NetworkObjectToggle>();

        // _isEnabled = true;
    }

    public void ResetHealth()
    {
        CurrentHealth = StartHealth;
        _viewController.SetHealth(CurrentHealth);
        _networkObjectToggle.ToggleObject(true);
    }

    private void Start()
    {
        if (!isLocalPlayer) return;
        ResetHealth();
        GameEvents.current.OnDamagedPlayer += TakeDamage;
    }

    /*private void Update()
    {
        if (!isLocalPlayer) return;
        gameObject.SetActive(_isEnabled);
    }*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "trap")
        {
            collision.gameObject.SetActive(false);

            if (!isLocalPlayer) return;
            GameEvents.current.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (!isLocalPlayer) return;

        CurrentHealth--;
        _viewController.SetHealth(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            // gameObject.SetActive(false);
            transform.position = new Vector3(1, 1, 0);
            _networkObjectToggle.ToggleObject(false);
            CmdCheckPlayersDeath();
            //GameEvents.current.Death();
        }
    }

    [Command]
    private void CmdCheckPlayersDeath()
    {
        RpcPlayerDeath();
    }

    [ClientRpc]
    private void RpcPlayerDeath()
    {
        _worldInitializer.DeadCount++;
        //_isEnabled=false;
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer) return;

        GameEvents.current.OnDamagedPlayer -= TakeDamage;
    }
}