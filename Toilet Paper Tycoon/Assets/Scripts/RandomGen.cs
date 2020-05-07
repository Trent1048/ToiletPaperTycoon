using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGen : MonoBehaviour
{
    public GameObject[] objects;

    

    void Start()
    {
        int roll = Random.Range(0, 50);
        if (roll <= 25)
        {
            int rand = Random.Range(0, objects.Length);
            Instantiate(objects[rand], transform);
        }
        else
        {
            return;
        }
    }


}
  