using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MenuTouchInput : MonoBehaviour
{
    GameObject particle;

    void OnMouseDown() {
        SceneManager.LoadScene("Scenes/GameScene");
    }
}