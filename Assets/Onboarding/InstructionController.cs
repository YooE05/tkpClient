using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InstructionController : MonoBehaviour
{
    [SerializeField] Transform PanelInstruction;
    public int onboardingDelay;
    [SerializeField] Transform PanelInstructionImage;
    [SerializeField] Transform PanelUI;
    [SerializeField] Transform PanelIndication;
    [SerializeField] Transform PanelLoading;
    [SerializeField] GameObject ButtonSkip;
    [SerializeField] GameObject ButtonNext;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject prefabIndication;
    [SerializeField] Text textPanel;

    string gameName;
    string userId;
    int index = 0;
    bool endInstruction = false;
    List<GameObject> listIndicator = new List<GameObject>();

    public InstructionData instructionData;
    public bool InstructionPassed = false;

    Color colorActive = new Color(0, 1, 0, 1);
    Color colorDeActive = new Color(0, 0.5f, 1, 1);

    public void ShowInstructionPanel()
    {
        PanelInstruction.gameObject.SetActive(true);
    }

    private IEnumerator SomeCoroutine()
    {
        if (instructionData.Game[gameName].wasTrained)
        {
            InstructionPassed = true;
            PanelLoading.transform.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitUntil(() => instructionData.Game[gameName].InstructionDownloaded);
            PanelLoading.transform.gameObject.SetActive(false);
            Initialize();
        }
    }

    public void InitData(InstructionData data, string userId, string gameName = "Main")
    {
        instructionData = data;
        this.gameName = gameName;
        this.userId = userId;
        PanelLoading.transform.gameObject.SetActive(true);
        StartCoroutine(SomeCoroutine());
    }

    void Initialize()
    {
        PanelInstruction.gameObject.SetActive(true);
        
        PanelUI.GetComponent<RectTransform>().sizeDelta = new Vector2((instructionData.Game[gameName].instructions.Count * 80f + 400f), 140f);
        PanelIndication.GetComponent<RectTransform>().sizeDelta = new Vector2((instructionData.Game[gameName].instructions.Count * 80f), 80f);
        for (int i = 0; i < instructionData.Game[gameName].instructions.Count; i++)
        {
            GameObject indicator = Instantiate(prefabIndication, PanelIndication.transform.position, Quaternion.identity);
            indicator.name = "Indicator_" + i.ToString();
            indicator.transform.parent = PanelIndication.transform;
            indicator.transform.localScale = new Vector3(1f, 1f, 1f);
            listIndicator.Add(indicator);
        }

        loadSprite();
    }

    void loadSprite()
    {
        if (instructionData.Game[gameName].instructions[index].type == InstructionData.InstructionType.image)
        {
            videoPlayer.Stop();
            PanelInstructionImage.GetComponent<Image>().enabled = true;
            //PanelInstuctionVideo.gameObject.SetActive(false);

            PanelInstructionImage.GetComponent<Image>().sprite = instructionData.Game[gameName].instructions[index].image;
        }
        else
        {
            //PanelInstuctionVideo.gameObject.SetActive(true);
            PanelInstructionImage.GetComponent<Image>().enabled = false;

            videoPlayer.GetComponent<VideoPlayer>().url = instructionData.Game[gameName].instructions[index].url;
            videoPlayer.Play();
        }

        textPanel.text = instructionData.Game[gameName].instructions[index].text;
        listIndicator[index].GetComponent<Image>().color = colorActive;
    }

    public void nextInstruction()
    {
        listIndicator[index].GetComponent<Image>().color = colorDeActive;
        index++;

        if (endInstruction)
        {
            //Выход из режима интсукции!
            skipInstruction();
            return;
        }

        if (index >= instructionData.Game[gameName].instructions.Count - 1)
        {
            ButtonSkip.GetComponent<Image>().color = colorDeActive;
            ButtonSkip.GetComponent<Button>().interactable = false;
            ButtonNext.GetComponentInChildren<Text>().text = "Готово";
            endInstruction = true;
            index = instructionData.Game[gameName].instructions.Count - 1;
        }

        loadSprite();
    }

    public void skipInstruction()
    {
        instructionData.Game[gameName].wasTrained = true;
        PanelInstruction.gameObject.SetActive(false);
        if (gameName == "Main")
        {
            instructionData.SendCompleteInstructionGame(userId);
        }
        else
        {
            instructionData.SendCompleteInstructionGame(gameName, userId);
        }
        InstructionPassed = true;
        //Application.LoadLevel(gameName);
    }

    public void choiceInstruction(int _index)
    {
        listIndicator[index].GetComponent<Image>().color = colorDeActive;

        index = _index;
        loadSprite();
    }
}