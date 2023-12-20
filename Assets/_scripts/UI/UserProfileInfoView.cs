using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserProfileInfoView : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text winCountText;

    public void LoadInfo(string name, string surname, int winCount)
    {
        this.nameText.text = name + "\r\n" + surname;

        if (winCountText != null)
            this.winCountText.text = winCount.ToString();
    }
    
}
