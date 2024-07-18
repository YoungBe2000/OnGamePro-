using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject GO;

    // Start is called before the first frame update
    void Start()
    {
        GO.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OverSceneOn()
    {
        GO.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


}
