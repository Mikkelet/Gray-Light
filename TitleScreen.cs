using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{


    public GUISkin customGUISkin;
    public bool showHelp = false;
    string helpText = "Tak fordi du ville spille Gråt Lys!\n-- Vi er kede af, at man kan gå gennem vægge. Det var simpelthen en uløselig fejl :( --\nDu styrer din karakter ved at klikke med musen på banen.\n Ting du kan snakke med bliver røde når du holder musen over dem.\nVælg de passende valg, og prøv ikke at blive deprimeret over udfaldet... ;)";
    string creditsText = "Spillet er lavet af 6 elever på GameIT College:\n\nProjektstyring: Danny Greve\n2D/Tegninger: Rasmus Bjerborg\nProgrammør: Mikkel Ekenberg Thygesen\n3D: Alexander Wedø\nTekst/Dialog/Historie: Allan Hvid Bisgaard\nMusik: Michael Hejlskov";

    #region GUI
    //Background
    public Texture texBackground;
    int bgPosX = 0;
    int bgPosY = 0;
    int bgWidth = 1024;
    int bgHeight = 768;
    Rect rectBackground;

    // play button
    public Texture texPlay;
    int playPosX = 30;
    int playPosY = 550;
    Rect rectPlay;

    // help
    public Texture texHelp;
    int helpPosX = 30;
    Rect rectHelp;

    //Exit
    public Texture texExit;
    int exitPosX = 30;

    //Help box
    int hbwidth = Screen.width/5 * 3;
    Rect rectHelpBox;
    Rect rectCredits;

    //spacing int
    int spacing = 20;

    Rect rectExit;
    #endregion

    // Use this for initialization
	void Start () {
        rectPlay = new Rect(playPosX, playPosY, texPlay.width, texPlay.height);
        rectHelp = new Rect(helpPosX, playPosY + texPlay.height + spacing, texHelp.width, texHelp.height);
        rectExit = new Rect(exitPosX, rectHelp.yMin + texHelp.height + spacing, texExit.width, texExit.height);

        rectHelpBox = new Rect(playPosX + rectPlay.width + spacing*2, playPosY, hbwidth/2, rectExit.yMax - playPosY);
        rectCredits = new Rect(rectHelpBox.xMax, playPosY, hbwidth / 2, rectExit.yMax - playPosY);

        rectBackground = new Rect(bgPosX, bgPosY, bgWidth, bgHeight);
        
	}

    void OnGUI()
    {
        GUI.Box(rectBackground, texBackground, GUIStyle.none);

        if (GUI.Button(rectPlay, texPlay, GUIStyle.none))
        {
            Application.LoadLevel(1);
        }
        if (GUI.Button(rectHelp, texHelp, GUIStyle.none))
        {
            if (showHelp == false)
            {
                showHelp = true;
            }
            else { showHelp = false; }
        }
        if (GUI.Button(rectExit, texExit, GUIStyle.none))
        {
            Application.Quit();
        }
        if (showHelp)
        {
            GUI.Box(rectHelpBox, helpText,customGUISkin.box);
            GUI.Box(rectCredits, creditsText, customGUISkin.box);
        }
    }
}
