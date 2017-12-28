using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticArm : MonoBehaviour {

    Chromosome chromosome;
    public GameObject[] Joints;
    private Vector3 target;
    public GameObject endEffector;

    public void InitArm(Chromosome _chromosome, Vector3 _target)
    {
        chromosome = _chromosome;
        target = _target;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void Evolve(RoboticArm roboticArm)
    {
        chromosome = chromosome.Evolve(roboticArm.chromosome);
    }

    public double fitness
    {
        get
        {
            double distance = Vector3.Distance(endEffector.transform.position, target);
            if (distance == 0)
            {
                distance = 0.0001;
            }
            double fitness = 1 / distance;

            return fitness;
        }
    }

}
