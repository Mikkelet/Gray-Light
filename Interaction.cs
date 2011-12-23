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
            {"En kvindelig patient i 20'erne ligger i koma og har gjordt det i over et halvt år, hun har kun små chancer for at vågne op igen, og selv hvis hun vågner er der store chancer for hjerneskade, hendes mor har besøgt hende næsten hver dag siden ulykken og kommer for at snakke med dig. \n\n>min datter har altid frygtet sådan en skæbne.<\n>det er heller ikke en rar en.<\n>hun ville have ønsket at bare blive slukket for det ved du godt ikke?<\n>nej ikke rigtig, det er heller ikke noget jeg har lyst til at gøre.<\n>hun kan jo ikke ligge her resten af sit liv, hun har altid ønsket ikke at være i vejen for andre.<\n>det er hun da heller ikke.<\n>det er hun da; hun bruger plads her på hospitalet som andre mennesker lige så godt kunne bruge.<\n>men jeg kan kan ikke bare slukke den. Hvad hvis hun vågne?<\n>det ved vi begge to godt at hun ikke gør, kan du ikke bare gøre det der er bedst for hende... og mig.<",
                "du får det dårligt over hvad du har gjort, men moderen hjælper dig til at tænke at det er det bedste for alle.",
                "du ser moderen hver dag. og det gør dig ked af at se hendes smerte.",
                ""
            },
                {"En mand i midt 50’erne er kommet ind efter et cykeluheld, han har brækket armen og fået en hjernerystelse, manden har en del smerter og opføre sig særdeles beruset.\n\n>jeg hører, at du har været ude for et uheld.<\n>Overhoved ikke, og du skal holde op med at pine min arm.\n>jeg rør slet ikke din arm.<\n>Du skal ikke lyve for mig, stop med at angribe mig eller det bliver værst for dig selv.<",
                "patienten bliver panisk og flår sig løs. Han flygter ud af hospitalet, hvor han ender med at vælte på vejen og dør i et trafik uheld.",
                "patienten bliver mere rolig efter han har fået noget mod smerterne. Dette gør det muligt for ham at sove den ud efter du har behandlet hans arm.",
                "patienten får det pludseligt dårligere. Du finder ud af, at hans lever er fejlet. Efter en hård kamp dør han af leverens udfald, det viste sig at han havde været alkoholiker i længere tid og at hans lever var aldeles svækket som et udfald af dette."
            },
            {"En patient er kommet ind efter et biluheld, patienten har fået nogle sår og højst sandsynlig også brækket benet, selve sårene og skaderne er ikke dødelige i sig selv. Men patienten har mistet en stor del blod, og ville nok ikke overleve længe hvis ikke at patienten får en blodtransfusion. Der er bare et problem, patienten har en stærk religiøs holdning, hans holdning går også imod almen blodtransfusion. Du snakker med hans bror omkring hans status.\n\n>Som det ser ud nu er ingen af hans sår direkte alvorlige, men han mangler store dele blod og på grund af hans religion har vi ikke nogle muligheder for at give ham det.<\n>Kan du ikke bare give ham noget blod?<\n>Jeg ville godt kunne give ham blod, men det ville gå imod hans ønske, jeg kan ikke bare overse patientens ønske omkring om han vil behandles eller ej.<\n>Men jeg ved jo at han gerne vil leve, jeg kender ham jo, du kan ikke bare gøre ingenting.<\n>Der er ikke så meget andet jeg kan gøre for hans religiøse holdninger.<\n>til helvede med den religion, han har næsten været død for den religion en gang, bare tag mit blod, jeg ved allerede at mit blod det passer.<\n>Det kan du ikke være sikker på før vi har testet dit blod.\n>jo, for han fik noget af mit blod for 5 år siden, 2 år før han gik ind i den forbandede religiøse kult.<\n>hmm<.",
                "Da patienten vågner er han selvfølgelig forvirret og træt. Da han kommer til sig selv og får at vide, at han stik i mod sin religions tro har fået en bloddonation bliver han tosset og forbander hele hospitalet.",
                "Han dør af blodmangel. broderen bliver rasende og stormer ud af hospitalet.",
                ""
            }

        },
        {   // main text when talking to a colleague
            {"Hej kollega, er der noget galt?",
                "Jeg er her for dig.",
                "Ah, godt at høre!",
                ""},
            {"Puha, hård uge, hva?",
                "Du er squ stærk.",
                "Vi er alle med dig.",
                ""},
            {"Vil du med til antidepressionsgruppemøde i næste uge?",
                "Okay så.",
                "Vi ses der så!",
                ""}
        }
    };

    int numberOfResponses = 3; //for soft coding
    string[,,] cases = new string[2, 3, 3] {
        {   // Reply options when talk to a PATIENT
            { "giv patienten en overdosis smertestillende.",
                "lad patienten ligge som han er og foreslå psykolog hjælp til moderen.",
                ""},
                { "prøv at berolige ham med lidt dialog for ikke at vække panik med nåle.", 
                "giv ham en lille dosis smertestillende medicin.",
                "giv ham en normal dosis smertestillende for at berolige ham så meget som muligt, så han ikke gør skade på folk."},
            { "tag en bloddonation fra broderen for at sikre sig at han overlever.",
                "brug autolog ansfusion med høj risiko for dødsfald eller hjerneskade.",
                ""}
        },
        {   // Reply options when talking to a COLLEAGUE
            { "Ja, jeg er lidt nede i dag.", "Nææ, har det fint nok.", "" }, 
            { "Ja, men jeg klarer det nok.", "Meget, det river virkelig i ens sind.", "" }, 
            { "Næ, jeg har andre ting at lave.", "Ja, jeg har brug for at snakke med nogen.", "" }
        }
    };

    string[] endResponses = new string[3]
    {
        "Okay.",
        "...",
        "Jeg bliver nød til at gå."
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

