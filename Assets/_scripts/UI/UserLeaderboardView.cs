
using UnityEngine;
using UnityEngine.UI;

public class UserLeaderboardView : MonoBehaviour
{
    LeaderBoardUser userData;

    private Text nameTxt;
    private Text pointsTxt;
    private Text positionTxt;

    private void Awake()
    {
        positionTxt = transform.Find("Position").GetComponentInChildren<Text>(); 
        nameTxt = transform.Find("Name").GetComponent<Text>();
        pointsTxt = transform.Find("Points").GetComponent<Text>();
    }

    public void loadViewData(LeaderBoardUser ud)
    {
        this.userData = ud;

        UpdateView();
    }

    private void UpdateView()
    {
        if(userData == null)
        {
            Debug.Log("Can not update UserLeaderboardView. User data is null");
            return;
        }

        nameTxt.text = userData.name+" "+ userData.surname;
        pointsTxt.text = userData.points.ToString();
        positionTxt.text = (userData.place+1).ToString();


 }

}
