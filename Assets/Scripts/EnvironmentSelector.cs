using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSelector : MonoBehaviour
{
    public GameObject[] environment;
    private int selectedEnvironment = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject en in environment)
        {
            en.SetActive(false);
        }

        environment[selectedEnvironment].SetActive(true);
    }

    public void ChangeCharacter(int newEnvironment)
    {
        environment[selectedEnvironment].SetActive(false);
        environment[newEnvironment].SetActive(true);
        selectedEnvironment = newEnvironment;
    }
}
