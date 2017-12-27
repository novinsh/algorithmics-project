using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offspring : MonoBehaviour {

    public GameObject[] Joints;
    Quaternion[] rotational_vectors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        foreach (GameObject joint in Joints)
        {
            Quaternion temp = Quaternion.Euler(Random.Range(0,360), 0, Random.Range(0, 360));
            joint.transform.rotation = temp;
        }


    }
}
