using UnityEngine;

public class ChoiceInstruction : MonoBehaviour
{
    InstructionController controllerInstruction;
    void Start()
    {
        controllerInstruction = FindObjectOfType<InstructionController>();
    }

    public void choiceInstructuin()
    {
        Debug.Log(this.name);
        string[] subs = this.name.Split('_');
        try
        {
            int index = System.Convert.ToInt32(subs[1]);
            controllerInstruction.choiceInstruction(index);
        }
        catch
        {
            Debug.Log(subs[1]);
            Debug.LogError("Некорректное имя точки индикации на панеле onboarding!");
        }
    }
}
