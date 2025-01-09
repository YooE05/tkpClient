using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    private ViewController _viewController;

    private readonly List<LaserInteraction> _lasers = new List<LaserInteraction>();

    private void Awake()
    {
        _viewController = FindObjectOfType<ViewController>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryDisableLaser();

            if (isServer)
            {
                RpcDisableLaser();
            }
            else
            {
                CmdTryDisableLaser();
            }
        }
    }

    [Command]
    private void CmdTryDisableLaser()
    {
        if (_lasers.Count == 0) return;

        foreach (var laser in _lasers.Where(laser => laser.CanBeTurnedOff()))
        {
            laser.Disable();
        }
    }

    [ClientRpc]
    private void RpcDisableLaser()
    {
        if (_lasers.Count == 0) return;

        foreach (var laser in _lasers.Where(laser => laser.CanBeTurnedOff()))
        {
            laser.Disable();
        }
    }

    private void TryDisableLaser()
    {
        if (_lasers.Count == 0) return;

        foreach (var laser in _lasers.Where(laser => laser.CanBeTurnedOff()))
        {
            laser.Disable();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "laserZone")
        {
            var laserComponent = collision.gameObject.GetComponent<LaserInteraction>();
            laserComponent.canDisableLaser = true;
            _lasers.Add(laserComponent);

            if (laserComponent.CanBeTurnedOff())
                _viewController.ChangeLaserDisableText(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "laserZone")
        {
            var laserComponent = collision.gameObject.GetComponent<LaserInteraction>();
            laserComponent.canDisableLaser = false;
            _lasers.Remove(laserComponent);

            _viewController.ChangeLaserDisableText(false);
        }
    }
}