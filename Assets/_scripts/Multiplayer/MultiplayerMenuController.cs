using Mirror;
using TMPro;
using UnityEngine;

public class MultiplayerMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerName;

    [SerializeField] private GameObject _loadingView;

    private void Awake()
    {
        _loadingView.SetActive(false);
        if (PlayerPrefs.HasKey("Nickname"))
        {
            _playerName.text = PlayerPrefs.GetString("Nickname");
        }
    }

    public void StartHost()
    {
        _loadingView.SetActive(true);
        NetworkManager.singleton.StartHost();

        PlayerPrefs.SetString("Nickname", _playerName.text);
        PlayerPrefs.Save();
    }

    public void StartClient()
    {
        _loadingView.SetActive(true);
        NetworkManager.singleton.StartClient();
        PlayerPrefs.SetString("Nickname", _playerName.text);
        PlayerPrefs.Save();
    }
}