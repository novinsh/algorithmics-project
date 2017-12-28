using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population_Controller : MonoBehaviour {

    List<RoboticArm> population = new List<RoboticArm>();
    int populationSize;
    int chromosomeLength;
    double eliteRate;
    public GameObject target;
    public GameObject prefab;

	// Use this for initialization
	void Start () {
        InitPopulation();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitPopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject go = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
            go.GetComponent<RoboticArm>().InitArm(new Chromosome(chromosomeLength), target.transform.position);
            population.Add(go.GetComponent<RoboticArm>());
        }
    }

    void NextGeneration()
    {
        //TODO
    }
}
