
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserLeaderboardViewWithModes : MonoBehaviour
{
    //private Text positionTxt; //old code
    private Image profilePhotoImg;

    private Text nameTxt;
    private Text pointsTxt;
    private Text winCountTxt;

    int position;
    UserData userData;
    string gameMode;

    private void Awake()
    {
        //positionTxt = transform.Find("Position").GetComponentInChildren<Text>(); //old code
        nameTxt = transform.FindDeepChild("Name").GetComponent<Text>();
        pointsTxt = transform.FindDeepChild("Points").GetComponent<Text>();
        winCountTxt = transform.FindDeepChild("WinCount").GetComponent<Text>();

        profilePhotoImg = transform.FindDeepChild("ProfilePhoto").GetComponent<Image>();
    }

    private void Start()
    {
        if (!gameObject.activeInHierarchy)
            return;

        UpdateView();

        //Debug.Log("start end()");
    }

    private void OnEnable()
    {
        
    }

    public void loadViewData(int position, UserData userData, string gameMode = null)
    {
        this.position = position;
        this.userData = userData;
        this.gameMode = gameMode;

        //Debug.Log("laod view data end");
    }

    private void UpdateView()
    {

        Debug.Log("[debug] parsing userdata to UserLeaderboardView. Data - " + userData);
        //bad code. refactor. Костыль так как данный метод вызывается 6 раз, если три объекта с userData null
        if(userData == null)
        {
            Debug.Log("Can not update UserLeaderboardView. User data is null");
            return;
        }

        //positionTxt.text = position.ToString(); //old

        nameTxt.text = userData.ProgressData.Name;

        if (userData.ProgressData.PointsDict.TryGetValue(gameMode, out int points))
            pointsTxt.text = points.ToString();

        /*not adapted for games with game modes */
        //winCountTxt.text = userData.Statistics?.WinCount.ToString();

        //temp uncomment
        //StartCoroutine(DownloadProfilePhoto(userData.ProfilePhotoUrl, (downloadedSprite) => profilePhotoImg.sprite = downloadedSprite));
        
        //Texture2D tex = await ImageLoader.LoadImage(userData.ProfilePhotoUrl);
        //profilePhotoImg.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);


        StartCoroutine(DownloadAndSetPhoto(userData.ProfilePhotoUrl));        
    }

    private IEnumerator DownloadAndSetPhoto(string url)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;

            profilePhotoImg.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    private IEnumerator DownloadProfilePhoto(string url, Action<Sprite> onLoaded)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError($"Error downloading texture with url {url}. Error - {www.error}");
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite downloadedSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            onLoaded.Invoke(downloadedSprite);
        }

    }
}
