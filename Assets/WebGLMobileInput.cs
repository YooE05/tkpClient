using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebGLMobileInput : MonoBehaviour
{
    private InputField input;
    private string promptTitle;
    private void Start()
    {
        input = GetComponent<InputField>();
        promptTitle = input.placeholder.GetComponent<Text>().text;
    }

    public void OnSelected()
    {
        if(Application.isMobilePlatform)
        {
            Debug.Log("Temp log. Is mobile input");
            PromptInput();
        }
    }

    public void PromptInput()
    {
        string text = WebNativeDialog.OpenNativeStringDialog(promptTitle, input.text);
        input.text = text;
    }
}
