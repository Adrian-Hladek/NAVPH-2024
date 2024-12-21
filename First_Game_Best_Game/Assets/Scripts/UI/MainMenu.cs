using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class MainMenu : MonoBehaviour
{

    public void PlayLevel1()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);



    }

    public void PlayLevel2()
    {

        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);



    }

    public void PlayLevel3()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);



    }

    public void QuitGame()
    {

        Application.Quit();


    }


}
