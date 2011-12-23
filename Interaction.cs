using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour {

    public GUIContent playerFace;
    public GUIContent[] targetFace  = new GUIContent[3];
    public GUISkin customGUISkin;

    public GameObject camera;
    public bool beginDialogue = false;
    public int TalkingTo = 0; // 0 = patient - 1 = Colleague. Used for finding dialogue in arrays
    public int dialogueProgress = 0;
    public int currentCase = 0;

    int response;
    float depressionStateValue = 20f;

    #region dialogue
    List<string> replyOptions = new List<string>();
    
    string[,,] textBoxTexts = new string[2, 3, 4]{
        {   // main text when talk to a patient
            // input 0 = init text. input 1 = replyOption 0. input 2 = replyOptions 1.
            {"En kvindelig patient i 20'erne ligger i koma og har gjordt det i over et halvt �r, hun har kun sm� chancer for at v�gne op igen, og selv hvis hun v�gner er der store chancer for hjerneskade, hendes mor har bes�gt hende n�sten hver dag siden ulykken og kommer for at snakke med dig. \n\n>min datter har altid frygtet s�dan en sk�bne.<\n>det er heller ikke en rar en.<\n>hun ville have �nsket at bare blive slukket for det ved du godt ikke?<\n>nej ikke rigtig, det er heller ikke noget jeg har lyst til at g�re.<\n>hun kan jo ikke ligge her resten af sit liv, hun har altid �nsket ikke at v�re i vejen for andre.<\n>det er hun da heller ikke.<\n>det er hun da; hun bruger plads her p� hospitalet som andre mennesker lige s� godt kunne bruge.<\n>men jeg kan kan ikke bare slukke den. Hvad hvis hun v�gne?<\n>det ved vi begge to godt at hun ikke g�r, kan du ikke bare g�re det der er bedst for hende... og mig.<",
                "du f�r det d�rligt over hvad du har gjort, men moderen hj�lper dig til at t�nke at det er det bedste for alle.",
                "du ser moderen hver dag. og det g�r dig ked af at se hendes smerte.",
                ""
            },
                {"En mand i midt 50�erne er kommet ind efter et cykeluheld, han har br�kket armen og f�et en hjernerystelse, manden har en del smerter og opf�re sig s�rdeles beruset.\n\n>jeg h�rer, at du har v�ret ude for et uheld.<\n>Overhoved ikke, og du skal holde op med at pine min arm.\n>jeg r�r slet ikke din arm.<\n>Du skal ikke lyve for mig, stop med at angribe mig eller det bliver v�rst for dig selv.<",
                "patienten bliver panisk og fl�r sig l�s. Han flygter ud af hospitalet, hvor han ender med at v�lte p� vejen og d�r i et trafik uheld.",
                "patienten bliver mere rolig efter han har f�et noget mod smerterne. Dette g�r det muligt for ham at sove den ud efter du har behandlet hans arm.",
                "patienten f�r det pludseligt d�rligere. Du finder ud af, at hans lever er fejlet. Efter en h�rd kamp d�r han af leverens udfald, det viste sig at han havde v�ret alkoholiker i l�ngere tid og at hans lever var aldeles sv�kket som et udfald af dette."
            },
            {"En patient er kommet ind efter et biluheld, patienten har f�et nogle s�r og h�jst sandsynlig ogs� br�kket benet, selve s�rene og skaderne er ikke d�delige i sig selv. Men patienten har mistet en stor del blod, og ville nok ikke overleve l�nge hvis ikke at patienten f�r en blodtransfusion. Der er bare et problem, patienten har en st�rk religi�s holdning, hans holdning g�r ogs� imod almen blodtransfusion. Du snakker med hans bror omkring hans status.\n\n>Som det ser ud nu er ingen af hans s�r direkte alvorlige, men han mangler store dele blod og p� grund af hans religion har vi ikke nogle muligheder for at give ham det.<\n>Kan du ikke bare give ham noget blod?<\n>Jeg ville godt kunne give ham blod, men det ville g� imod hans �nske, jeg kan ikke bare overse patientens �nske omkring om han vil behandles eller ej.<\n>Men jeg ved jo at han gerne vil leve, jeg kender ham jo, du kan ikke bare g�re ingenting.<\n>Der er ikke s� meget andet jeg kan g�re for hans religi�se holdninger.<\n>til helvede med den religion, han har n�sten v�ret d�d for den religion en gang, bare tag mit blod, jeg ved allerede at mit blod det passer.<\n>Det kan du ikke v�re sikker p� f�r vi har testet dit blod.\n>jo, for han fik noget af mit blod for 5 �r siden, 2 �r f�r han gik ind i den forbandede religi�se kult.<\n>hmm<.",
                "Da patienten v�gner er han selvf�lgelig forvirret og tr�t. Da han kommer til sig selv og f�r at vide, at han stik i mod sin religions tro har f�et en bloddonation bliver han tosset og forbander hele hospitalet.",
                "Han d�r af blodmangel. broderen bliver rasende og stormer ud af hospitalet.",
                ""
            }

        },
        {   // main text when talking to a colleague
            {"Hej kollega, er der noget galt?",
                "Jeg er her for dig.",
                "Ah, godt at h�re!",
                ""},
            {"Puha, h�rd uge, hva?",
                "Du er squ st�rk.",
                "Vi er alle med dig.",
                ""},
            {"Vil du med til antidepressionsgruppem�de i n�ste uge?",
                "Okay s�.",
                "Vi ses der s�!",
                ""}
        }
    };

    int numberOfResponses = 3; //for soft coding
    string[,,] cases = new string[2, 3, 3] {
        {   // Reply options when talk to a PATIENT
            { "giv patienten en overdosis smertestillende.",
                "lad patienten ligge som han er og foresl� psykolog hj�lp til moderen.",
                ""},
                { "pr�v at berolige ham med lidt dialog for ikke at v�kke panik med n�le.", 
                "giv ham en lille dosis smertestillende medicin.",
                "giv ham en normal dosis smertestillende for at berolige ham s� meget som muligt, s� han ikke g�r skade p� folk."},
            { "tag en bloddonation fra broderen for at sikre sig at han overlever.",
                "brug autolog ansfusion med h�j risiko for d�dsfald eller hjerneskade.",
                ""}
        },
        {   // Reply options when talking to a COLLEAGUE
            { "Ja, jeg er lidt nede i dag.", "N��, har det fint nok.", "" }, 
            { "Ja, men jeg klarer det nok.", "Meget, det river virkelig i ens sind.", "" }, 
            { "N�, jeg har andre ting at lave.", "Ja, jeg har brug for at snakke med nogen.", "" }
        }
    };

    string[] endResponses = new string[3]
    {
        "Okay.",
        "...",
        "Jeg bliver n�d til at g�."
    };

    // array stating whether the outcome of a specific response in a specific case will be good, bad or neutral.
    // "neutral" is needed because not all cases will have 3 response possibilities.
    // [talkingTo, currentcase, response outcome]
    string[,,] outcomes = new string[2, 3, 3]
    {
    {   // patient
        {
            "good","bad","neutral"
        },
        {
            "bad","good","bad"
        },
        {
            "bad","bad","neutral"
        }
    },
    {   // colleague
        {
            "good","bad","neutral"
        },
        {
            "bad","good","neutral"
        },
        {
            "bad","good","neutral"
        }
    }
};
    #endregion

    #region GUI positions
    //
    float mainTextTop = Screen.height / 8f;
    float mainTextLeft = Screen.width / 5 * 1.5f;
    int mainTextWidth = Screen.width / 5 * 2;
    int mainTextHeight = Screen.height / 8 * 5;
    Rect rectMainText;

    int buttonHeight = 48;
    Rect rectButton;

    int imageWidth = 265;
    int imageHeight = 512;

    #endregion

    //texts
    string mainText = "Hey, how do\n you feel?";

    void Awake()
    {
        NewDay();
        rectMainText = new Rect(mainTextLeft, mainTextTop, mainTextWidth, mainTextHeight);
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
                    targetFace[currentCase],
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

                if (GUI.Button(rectButton, replyOptions[i], customGUISkin.button))
                {
                    if (dialogueProgress == 0)
                    {
                        response = i;
                        ResponseOutcome();
                        mainText = textBoxTexts[TalkingTo, currentCase, i + 1];
                        replyOptions.Clear();
                        replyOptions.Add(endResponses[i]);
                        
                        dialogueProgress = 1;
                    }
                    else if (dialogueProgress == 1)
                    {
                        if (name == "Patient")
                        {
                            camera.GetComponent<PointNClick>().currentAction++;
                        }
                        camera.GetComponent<PointNClick>().targetSelected = null;
                        beginDialogue = false;
                    }
                }
            }
        }
    }

    // Methods
    public void ChangeCase()
    {
        if (dialogueProgress == 0)
        {
            replyOptions.Clear();
            mainText = textBoxTexts[TalkingTo, currentCase, 0];

            for (int i = 0; i < numberOfResponses; i++)
            {

                if (cases[TalkingTo, currentCase, i] != "")
                {
                    replyOptions.Add(cases[TalkingTo, currentCase, i]);
                }
            }
        }
    }
    public void NewDay()
    {
        dialogueProgress = 0;
        ChangeCase();
        
        if (name == "Patient")
        {
            // Generates a random number and combine it with the name RoomPos. 
            // RoomPos1-6 are reference objects, that I use to get their position.
            // Whenever a new day starts, the patient will be "teleported" to one of the ref. objects.
            int num = Random.Range(1, 6);
            GameObject refObject = GameObject.Find("RoomPos" + num.ToString());
            //GameObject refObject = GameObject.Find("RoomPos1");
            gameObject.transform.position = new Vector3(
                refObject.transform.position.x, 
                refObject.transform.position.y, 
                refObject.transform.position.z);
            gameObject.transform.eulerAngles = new Vector3(
                -90,
                270,
                refObject.transform.eulerAngles.z);
        }
    }
    void ResponseOutcome()
    {
        for (int i = 0; i < numberOfResponses; i++)
        {

            if (name == "Patient")
            {
                if (response == i)
                {
                    switch (currentCase)
                    {
                        default:
                            if (outcomes[TalkingTo, currentCase, i] == "good")
                            {
                                GoodOutcome();
                            }
                            else if (outcomes[TalkingTo, currentCase, i] == "bad")
                            {
                                BadOutCome();
                            }
                            else if (outcomes[TalkingTo, currentCase, i] == "neutral")
                            {
                            }
                            break;
                    }
                }
            }
            else if (name == "Colleague")
            {
                if (response == i)
                {
                    switch (currentCase)
                    {
                        default:
                            if (outcomes[TalkingTo, currentCase, i] == "good")
                            {
                                GoodOutcome();
                            }
                            else if (outcomes[TalkingTo, currentCase, i] == "bad")
                            {
                                if (camera.GetComponent<PointNClick>().stateOfDepression < 0)
                                {
                                    BadOutCome();
                                }
                            }
                            else if (outcomes[TalkingTo, currentCase, i] == "neutral")
                            {
                            }
                            break;
                    }
                }
            }
        }
    }
    void GoodOutcome()
    {
        camera.GetComponent<PointNClick>().stateOfDepression += depressionStateValue;
        camera.GetComponent<PointNClick>().refreshDepression();
    }
    void BadOutCome()
    {
        camera.GetComponent<PointNClick>().stateOfDepression -= depressionStateValue;
        camera.GetComponent<PointNClick>().refreshDepression();
    }
}

