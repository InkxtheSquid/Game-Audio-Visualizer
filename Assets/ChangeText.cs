using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeText : MonoBehaviour
{
    public TextMeshProUGUI changingText;
    public int id;
    public string[] songs = new string[]{
                                          "A Corner Of Memories - Persona 4",
                                          "Like a Dream Come True - Persona 4",
                                          "Joy - Persona 3",
                                          "Break It Down - Persona 5",
                                          "Normal Battle - SMTIII",
                                          "Normal Battle ~Town~ - SMTIII"
                                        };
    // Start is called before the first frame update
    void Start()
    {
        setText();
    }

    void setText(){
        changingText.SetText(songs[id]);
    }
}
