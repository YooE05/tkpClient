using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Не нужен думаю
public class AppController : MonoBehaviour
{
    static DataController dataController;

    public static void SetDataController(DataController controller)
    {
        dataController = controller;
    }

    private static void OnApplicationQuit()
    {
        if (dataController == null)
            return;

        
    }
}
