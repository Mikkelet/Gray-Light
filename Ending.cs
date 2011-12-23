using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {

    public GUISkin customGUISkin;
    public int depressionState = 0;
    int currentText = 0;

    string[,] texts = new string[2, 5]{
        {
        "Hen over natten blev smerterne v�rre.",
        "Vagtl�gen skyndte sig at f� hende p� akutafdelingen.",
        "Men det var desv�rre for sent. Hverken barnet eller kvinden stod til at redde.",
        "Spillet er lavet af 6 elever p� GameIT College:\n\nProjektstyring: Danny Greve\n2D/Tegninger: Rasmus Bjerborg\nProgramm�r: Mikkel Ekenberg Thygesen\n3D: Alexander Wed�\nTekst/Dialog/Historie: Allan Hvid Bisgaard\nMusik: Michael Hejlskov",
        "~ fin ~"
        },
        {
        "Kvinden ryger ind p� operationsstuen.",
        "Du har heldet med dig. Lidt til intet er g�et galt denne uge.",
        "Kvinden og barnet overlever. Begger er i sundeste tilstand.",
        "Spillet er lavet af 6 elever p� GameIT College:\n\nProjektstyring: Danny Greve\n2D/Tegninger: Rasmus Bjerborg\nProgramm�r: Mikkel Ekenberg Thygesen\n3D: Alexander Wed�\nTekst/Dialog/Historie: Allan Hvid Bisgaard\nMusik: Michael Hejlskov",
        "~ fin ~"
        }
    };
    string[] btnTexts = new string[5]
    {
        "N�ste",
        "N�ste",
        "Hvem har lavet dette spil?",
        "Er spillet slut?",
        "Tilbage til menuen"
    };

    // GUI positions
    int textLeft = Screen.width/2;
    int textTop = Screen.height/2;
    int textWidth = 200;
    int textHeight = 200;
    Rect rectText;

    int btnWidth = 100;
    int btnHeight = 48;
    Rect rectButton;

	// Use this for initialization
	void Start () {
        rectText = new Rect(textLeft - textWidth / 2, textTop - textHeight / 2, textWidth, textHeight);
        rectButton = new Rect(textLeft - btnWidth/2, rectText.yMax, btnWidth, btnHeight);
	}

    void OnGUI()
    {
        GUI.Box(rectText, texts[depressionState, currentText], customGUISkin.box);

        if (GUI.Button(rectButton, btnTexts[currentText], customGUISkin.button))
        {
            if (currentText < 4)
            {
                currentText++;
            }
            else { 
                GameObject.Find("PersistentObject").GetComponent<PersistentObject>().showHelp = true;
                Application.LoadLevel(0); 
            }
        }
    }
}
