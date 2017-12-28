using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticArm : MonoBehaviour {

    public Chromosome chromosome;
    public GameObject[] Joints;
    private Vector3 target;
    public GameObject endEffector;
    public bool freeze=false;
    public bool updateItMan = false;

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
        if (!freeze)
        {
            //    if (updateItMan)
            //{ 
                for (int i = 0; i < Joints.Length; i++)
                {
                    Joints[i].transform.rotation = chromosome.genes[i];
                }
            //}
        }
    }

    //public Chromosome Evolve(RoboticArm roboticArm)
    //{
    //    return chromosome.Evolve(roboticArm.chromosome);
    //}

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
