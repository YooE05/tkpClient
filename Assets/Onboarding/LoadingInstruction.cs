using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingInstruction : MonoBehaviour
{
    public Image loading;
    public float waitTime = 0.5f;

    void Start()
    {
        StartCoroutine(animateLoading());
    }

    private IEnumerator animateLoading()
    {
        while (true)
        {
            loading.fillAmount = 0.0f;
            yield return new WaitForSeconds(waitTime / 2);
            loading.fillAmount = 0.33f;
            yield return new WaitForSeconds(waitTime);
            loading.fillAmount = 0.66f;
            yield return new WaitForSeconds(waitTime);
            loading.fillAmount = 1.0f;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
