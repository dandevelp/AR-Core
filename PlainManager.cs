using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlainManager : MonoBehaviour
{
    //public GameObject button;
    //private Button _btn;

    // Start is called before the first frame update
    void Start()
    {
        //_btn = button.GetComponent<Button>();

        //_btn = GameObject.Find("ui_btn_02").GetComponent<Button>();

        //_btn.onClick.AddListener(clearAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void clearAction()
    {
        GameObject[] destroyList = GameObject.FindGameObjectsWithTag("Spawnable");
        foreach (GameObject go in destroyList)
        {
            Destroy(go);
        }
    }

}
