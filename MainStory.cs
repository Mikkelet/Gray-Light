using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStory : MonoBehaviour
{
    public GUIContent playerFace;
    public GUIContent targetFace;
    public GUISkin customGUISkin;
    public GameObject camera;

    public bool beginDialogue = false;
    public int Progress = 0; // current progress with the patient
    int dialogueProgress = 0;

    #region GUI positions
    //
    int mainTextTop = Screen.height / 8;
    int mainTextLeft = Screen.width / 5;
    int mainTextWidth = Screen.width / 5 * 2;
    int mainTextHeight = Screen.height / 8 * 4;
    Rect rectMainText;

    int buttonHeight = 32;
    Rect rectButton;

    int imageWidth = 143;
    int imageHeight = 384;
    #endregion

    List<string> replyOptions = new List<string>();

    string[,] textBoxTexts = new string[4, 2]{
            {"Patienten er en højgravid kvinde, hun har haft nogle smerter og hendes læge har overført hende til hospitalet for at kunne blive holdt øje med i de sidste perioder af graviditeten.\n\n>Patienten\n>klarer mit barn sig?<\n>Jeg skal gøre alt hvad jeg kan for at dit barn klarer sig, men som det ser ud lige nu er der ikke rigtig noget galt, vi er nød til at lave nogle tests for at sikre os, men der er ingen grund til bekymring som det ser ud nu.<\n>Jeg har det egentlig også fint lige nu..<", "Godt. Men jeg træt. Vi ses i morgen."},
            {"Hej doktor, hva' nyt?", ">jeg har det okay, men jeg bliver mere og mere nervøs for at mit barn tager skade.<\n\n>Det kan jeg godt forstå, et hospital er ikke altid det mest beroligende sted at ligge. Jeg har fået nogle test resultater tilbage og der ser ikke ud til at være noget galt.<\n\n>det var da altid rart at høre.<"},
            {">Hvad er der galt siden du kaldte?<\n>Jeg fik nogle smerter.<", ">de mindede om dem jeg har haft før, bare lidt kraftigere.<\n>hmm, jeg har dog fri om ikke så lang tid, men vi må nok hellere lige få kørt nogle tests.<"},
            {">hvad er der sket siden du kaldte?<\n>jeg har haft lidt smerter her på det seneste, jeg ved ikke helt hvad det er.<",">de minder meget om de tidligere smerter jeg har haft, de opstår også de samme steder.<\n>Okay. Ved du hvad, jeg skriver dig lige over til en vagtlæge for i aften. Det har været en sort og dårlig uge - og jeg har brug for søvn.<\n>okay, tak.<"}
    };

    int numberOfResponses = 2;
    string[,] cases = new string[4, 2] {
            // Reply options when talk to the patient
            { "Det lyder godt.", "" }, 
            { "Hvordan har du det idag?", "" }, 
            { "hvor er smertene henne? og hvordan føles de?", "" },
            {"hvor er smertene henne? og hvordan føles de?", ""}
    };

    string[] endResponses = new string[4]
    {
        "Ja, det gør vi.",
        "Ses i morgen.",
        "Jeg går lige ud og gør mig klar.",
        "Vi snakkes ved i morgen."
    };

    //texts
    string mainText = "Hey, how do\n you feel?";

    void Awake()
    {
        rectMainText = new Rect(mainTextLeft, mainTextTop, mainTextWidth, mainTextHeight);
    }

    void Update()
    {

    }

    void OnMouseEnter()
    {
        renderer.material.color = new Color(0.5f, 0f, 0f, 1f);
    }

    void OnMouseExit()
    {
        renderer.material.color = Color.white;
    }

    void OnGUI()
    {
        if (beginDialogue)
        {
            //Left - Talking to (colleague, patient, etc.)
                GUI.Box(new Rect(
                        mainTextLeft - imageWidth,
                        mainTextTop,
                        imageWidth,
                        imageHeight),
                        targetFace,
                        customGUISkin.box);
                //Right - player's face
                GUI.Box(new Rect(
                        mainTextLeft + mainTextWidth,
                        mainTextTop,
                        imageWidth,
                        imageHeight),
                        playerFace,
                        customGUISkin.box);

            GUI.Box(rectMainText, mainText, customGUISkin.box);

            for (int i = 0; i < replyOptions.Count; i++)
            {
                rectButton = new Rect(
                            mainTextLeft,
                            (mainTextTop + mainTextHeight) + (buttonHeight * i),
                            mainTextWidth,
                            buttonHeight);

                if (GUI.Button(rectButton, replyOptions[i],customGUISkin.button))
                {
                    if (dialogueProgress == 0)
                    {
                        mainText = textBoxTexts[Progress, 1];
                        replyOptions.Clear();
                        replyOptions.Add(endResponses[Progress]);
                        dialogueProgress = 1;
                    }
                    else if (dialogueProgress == 1)
                    {
                        beginDialogue = false;
                        camera.GetComponent<PointNClick>().currentAction++;
                        camera.GetComponent<PointNClick>().targetSelected = null;
                        Progress++;
                        if (Progress == 2 && camera.GetComponent<PointNClick>().stateOfDepression <= -20)
                        {
                            Progress++;
                        }
                        dialogueProgress = 0;
                    }
                }
            }
        }
    }

    public void ChangeCase()
    {
        replyOptions.Clear();

            mainText = textBoxTexts[Progress, 0];

        for (int i = 0; i < numberOfResponses; i++)
        {

            if (cases[Progress, i] != "")
            {
                replyOptions.Add(cases[Progress, i]);
            }
        }
    }
}
