using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(name);
    }
    public void GameScene()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene");
    }
    public void GameScene1()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene");
    }
    public void GameScene2()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene2");
    }
    public void GameScene3()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene3");
    }
    public void GameScene4()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene4");
    }
    public void GameScene5()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("GameScene5");
    }
    public void MainScene()
    {
        GameObject audiovisualiser=GameObject.Find("audiovisualiser");
        audiovisualiser.GetComponent<FMODAudioVisualizer>().StopFMODEvent();
        SceneManager.LoadScene("SampleScene");
    }
    public void Exit()
    {
        Application.Quit();
    }
}