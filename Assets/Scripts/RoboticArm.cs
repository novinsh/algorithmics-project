using UnityEngine;
using System;
using UnityEngine.UI;

public class RoboticArm : MonoBehaviour {

    public Chromosome chromosome;   // analogous the creature DNA defining its characteristics
    public GameObject[] Joints;     // joints of the creature that is being emulated in the simulation
    private Vector3 target;         // target to which arm should reach
    public GameObject endEffector;  // the endpoint of the arm
    public bool freeze = false;     // handled by the population_controller to control when arm positions should be updated on scene

    [Space(5)]

    [Header("References")]
    [Space(15)]
    public Population_Controller popController;
    public GameObject contentList;
    public GameObject UIEntry;
    private Text UIName;
    private Text UIFitness;

    public void InitArm(Chromosome _chromosome, Vector3 _target)
    {
        chromosome = _chromosome;
        target = _target;
    }

    // Use this for initialization
    void Start () {
        UIEntry = Instantiate(Resources.Load("_Entry") as GameObject);
        UIEntry.transform.parent = contentList.transform;
        this.gameObject.name = "Offspring-" + popController.InitialPopCounter;
        popController.InitialPopCounter++;
        UIName = UIEntry.transform.Find("ObjectName").GetComponent<Text>();
        UIFitness = UIEntry.transform.Find("Fitness").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!freeze)
        {
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].transform.rotation = chromosome.genes[i];
            }
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
            UIFitness.text = "| " + Math.Round(fitness,5);
            UIName.text = this.gameObject.name;

            return fitness;
        }
    }

}
