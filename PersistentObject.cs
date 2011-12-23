using UnityEngine;
using System.Collections;

public class PersistentObject : MonoBehaviour {

    // This script's only function is to carry over information when going to the end-screen.

    public int depressed = 0;
    public bool showHelp;

    void Awake()
    {
        Object.DontDestroyOnLoad(this);
    }

    void Update()
    {
        switch(Application.loadedLevel)
        {
            case 0:
                GameObject.Find("Main Camera").GetComponent<TitleScreen>().showHelp = showHelp;
                break;
            case 2:
                GameObject.Find("Main Camera").GetComponent<Ending>().depressionState = depressed;
                break;
        }
    }
}
