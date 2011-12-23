using UnityEngine;
using System.Collections;

public class PointNClick : MonoBehaviour {

    Vector3 touchPos;
    Vector3 target;

    public GameObject player;
    public GameObject targetSelected;
    public GameObject persistentObject;
    public GUISkin customGUISkin;
    public AudioClip audioDepressed;
    public AudioClip audioDefault;

    
    float speed = 1f;
    float startSpotAngle;
    public bool move = false;
    public string movingTo = "point";
    public float targetColliding;
    public float stateOfDepression = 0; //the higher, the more depressed;
    public int currentDay = 0; //monday through sunday, 0-3
    public int currentAction = 0; // What should you do now? Talk to the main patient, a patient in general or just go home? Actions are define in numbers
    public bool showMessage = false;
    public bool playerWillCollide = false; // If true when the player collides with wall, she will stop.

    //Message box attributes
    int msgBoxLeft = 10;
    int msgBoxTop = 10;
    int msgBoxWidth = 128;
    int msgBoxHeight = 64;
    Rect rectMsgBox;

    //message box's button attributes
    //many of the attributes are dependant on the message box's attributes. I will thus not write them.
    int msgButtonHeight = 32;
    Rect rectMsgButton;

    //Message
    public string msgText = "text";


    void Awake()
    {
        if (stateOfDepression <= -20)
        {
            GetComponent<AudioSource>().clip = audioDepressed;
            GetComponent<AudioSource>().Play();
        }
        if (stateOfDepression >= 0)
        {
            GetComponent<AudioSource>().clip = audioDefault;
            GetComponent<AudioSource>().Play();
        }

        startSpotAngle = player.transform.FindChild("Spotlight").GetComponent<Light>().spotAngle;

        player.GetComponent<Animation>().wrapMode = WrapMode.Loop;
        player.GetComponent<Animation>()["Take 001"].speed = 0;

        rectMsgBox = new Rect(msgBoxLeft, msgBoxTop, msgBoxWidth, msgBoxHeight);
        rectMsgButton = new Rect(msgBoxLeft, msgBoxHeight, msgBoxWidth, msgButtonHeight);
    }
	void Update () {

        // Moves the character if allowed
        if (move == true) {
            player.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        else {
            player.transform.Translate(Vector3.zero);
        }

        // detects whether you're close to the target or not. If you are, then you stop and perform the actions
        // based on what target you're moving to.
        if (Vector3.Distance(player.transform.position, target) < targetColliding )
        {
            player.GetComponent<Animation>()["Take 001"].speed = 0;

            switch (movingTo)
            {
                case "Colleague":
                    targetSelected = GameObject.Find("Colleague");
                    move = false;
                    if (targetSelected.GetComponent<Interaction>().beginDialogue != true)
                    {
                        movingTo = "Colleague";
                        
                        targetSelected.GetComponent<Interaction>().TalkingTo = 1;
                        targetSelected.GetComponent<Interaction>().ChangeCase();
                        targetSelected.GetComponent<Interaction>().beginDialogue = true;
                        showMessage = false;
                    }
                    movingTo = "point";
                    break;

                case "Patient":
                    targetSelected = GameObject.Find("Patient");
                    move = false;
                    if (currentAction == 0)
                    {
                        if (targetSelected.GetComponent<Interaction>().beginDialogue != true)
                        {
                            targetSelected.GetComponent<Interaction>().TalkingTo = 0;
                            targetSelected.GetComponent<Interaction>().beginDialogue = true;
                            showMessage = false;
                            movingTo = "point";
                        }
                    }
                    else
                    {
                        ShowMessage("Patienten er ikke snaksaglig lige nu.");
                    }
                    break;

                case "MainPatient":
                    targetSelected = GameObject.Find("MainPatient");
                    move = false;
                    if (currentAction == 1)
                    {
                        if (targetSelected.GetComponent<MainStory>().beginDialogue != true)
                        {
                            targetSelected.GetComponent<MainStory>().ChangeCase();
                            targetSelected.GetComponent<MainStory>().beginDialogue = true;
                            showMessage = false;
                            
                        }
                        movingTo = "point";
                    }
                    else
                    {
                        if (currentAction == 0)
                        {
                            ShowMessage("Der er andre patienter, som du lige skal se til.");
                        }
                        else { ShowMessage("Dagen er snart ovre. Du må hellere gå hjem."); }
                    }
                    break;

                case "ExitDoor":
                    targetSelected = GameObject.Find("ExitDoor");
                    if (currentDay < 2)
                    {
                        if (currentAction == 2)
                        {

                            ShowMessage("En ny dag begynder.");
                            GameObject patient = GameObject.Find("Patient");
                            GameObject colleague = GameObject.Find("Colleague");
                            movingTo = "point";
                            currentDay++;
                            patient.GetComponent<Interaction>().currentCase++;
                            colleague.GetComponent<Interaction>().currentCase++;
                            NewDay();
                        }
                        else
                        {
                            ShowMessage("Du kan ikke gå hjem nu.");
                        }

                    }
                    else if (currentDay == 2)
                    {
                        if (currentAction == 2)
                        {
                            Application.LoadLevel(2);
                        }
                    }

                    move = false;
                    break;

                default:
                    move = false;
                    break;
            }

        }
        
        // when clicking on the screen with the cursor
        if (Input.GetMouseButtonDown(0))
        {

            // init. these variables to get where you click and to setup for a raycast
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // raycast is used to check if the clicked area is walkable (ie. the ground)
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                switch (hit.collider.name)
                {
                    case "Ground":
                        movingTo = "Ground";
                        move = true;
                        target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                        targetColliding = 0.2f;
                        player.transform.LookAt(target);
                        player.GetComponent<Animation>()["Take 001"].speed = 5;
                        
                        break;

                    case "Colleague":
                        movingTo = "Colleague";
                        move = true;
                        target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                        player.transform.LookAt(target);
                        targetColliding = hit.collider.bounds.size.x / 2 + player.collider.bounds.size.x / 2;
                        player.GetComponent<Animation>()["Take 001"].speed = 5;
                        break;

                    case "Patient":
                        movingTo = "Patient";
                        move = true;
                        target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                        player.transform.LookAt(target);
                        targetColliding = hit.collider.bounds.size.x/2 + player.collider.bounds.size.x/2;
                        player.GetComponent<Animation>()["Take 001"].speed = 5;
                        break;

                    case "MainPatient":
                        movingTo = "MainPatient";
                        move = true;
                        target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                        player.transform.LookAt(target);
                        targetColliding = hit.collider.bounds.size.x / 2 + player.collider.bounds.size.x / 2;
                        player.GetComponent<Animation>()["Take 001"].speed = 5;
                        break;

                    case "ExitDoor":
                        movingTo = "ExitDoor";
                        move = true;
                        target = new Vector3(hit.point.x, player.transform.position.y, hit.point.z);
                        player.transform.LookAt(target);
                        targetColliding = hit.collider.bounds.size.x / 2 + player.collider.bounds.size.x / 2;
                        player.GetComponent<Animation>()["Take 001"].speed = 5;
                        break;
                }
            }
        }
	}
    void OnGUI()
    {
        if (showMessage)
        {
            GUI.Box(rectMsgBox,msgText,customGUISkin.box);
            if (GUI.Button(rectMsgButton, "Okay.",customGUISkin.button))
            {
                showMessage = false;
                movingTo = "point";
            }
        }
    }
    public void refreshDepression()
    {
        player.transform.FindChild("Spotlight").GetComponent<Light>().spotAngle = startSpotAngle + stateOfDepression;
        if (stateOfDepression <= -20)
        {
            persistentObject.GetComponent<PersistentObject>().depressed = 0;
            if (GetComponent<AudioSource>().clip != audioDepressed)
            {
                GetComponent<AudioSource>().clip = audioDepressed;
                GetComponent<AudioSource>().Play();
            }
        }
        if (stateOfDepression >= 0)
        {
            persistentObject.GetComponent<PersistentObject>().depressed = 1;

            if (GetComponent<AudioSource>().clip != audioDefault)
            {
                GetComponent<AudioSource>().clip = audioDefault;
                GetComponent<AudioSource>().Play();
            }
        }
    }
    void ShowMessage(string newText)
    {
        msgText = newText;
        showMessage = true;
    }
    void NewDay()
    {
        currentAction = 0;
        // Starts a new day. Inits Interactions NewDay Method which resets it's cases. Also places doctor's position.
        GameObject.Find("Patient").GetComponent<Interaction>().NewDay();
        GameObject.Find("Colleague").GetComponent<Interaction>().NewDay();
        player.transform.position = new Vector3(
            GameObject.Find("StartPos").transform.position.x,
            0,
            GameObject.Find("StartPos").transform.position.z);
    }
}
