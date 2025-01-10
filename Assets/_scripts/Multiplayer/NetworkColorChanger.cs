using Mirror;
using UnityEngine;

public class NetworkColorChanger : NetworkBehaviour
{
    /*// Метод для вызова отключения объекта
    [Client]*/
    public void ChangeColor(Color color)
    {
        // if (!isLocalPlayer) return;

        // Отправляем команду на сервер
        if (isServer)
        {
            RpcChangeColor(gameObject, color);
        }
        else
        {
            CmdChangeColor(gameObject, color);
        }
    }

    // Команда: вызывается на сервере
    [Command]
    private void CmdChangeColor(GameObject target, Color color)
    {
        RpcChangeColor(target, color); // Рассылаем всем клиентам
    }

    // RPC: вызывается на клиентах
    [ClientRpc]
    private void RpcChangeColor(GameObject target, Color color)
    {
        target.GetComponent<SpriteRenderer>().color = color;
    }
}