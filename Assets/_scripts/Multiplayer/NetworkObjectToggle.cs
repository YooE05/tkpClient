using UnityEngine;
using Mirror;

public class NetworkObjectToggle : NetworkBehaviour
{
    // Метод для вызова отключения объекта
    [Client]
    public void ToggleObject(bool isActive)
    {
        if (!isLocalPlayer) return;

        // Отправляем команду на сервер
        CmdToggleObject(gameObject, isActive);
    }

    // Команда: вызывается на сервере
    [Command]
    private void CmdToggleObject(GameObject target, bool isActive)
    {
        RpcSetObjectActive(target, isActive); // Рассылаем всем клиентам
    }

    // RPC: вызывается на клиентах
    [ClientRpc]
    private void RpcSetObjectActive(GameObject target, bool isActive)
    {
        target.SetActive(isActive);
    }
}