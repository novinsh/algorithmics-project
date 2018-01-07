using UnityEngine;
using System;
using UnityEngine.UI;

public class RoboticArm : MonoBehaviour {

    public Chromosome chromosome;   // analogous the creature DNA defining its characteristics
    public GameObject[] Joints;     // joints of the creature that is being emulated in the simulation
    private Vector3 target;         // target to which arm should reach
    public GameObject endEffector;  // the endpoint of the arm
    public bool freeze = false;     // handled by the population_controller to control when arm positions should be updated on scene


    public TweenRotation[] AllRotationalTweens;

    [Space(5)]

    [Header("References")]
    [Space(15)]
    public Population_Controller popController;
    public GameObject contentList;
    public GameObject UIEntry;
    private Text UIName;
    private Text UIFitness;

    public Vector3[] JointsRotationalVector;
    int TweenCounter = 0;

    public void InitArm(Chromosome _chromosome, Vector3 _target)
    {
        chromosome = _chromosome;
        target = _target;
    }

    // Use this for initialization
    void Start () {

        AllRotationalTweens = transform.GetComponentsInChildren<TweenRotation>();


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
            if(chromosome!=null)
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


    // Play Animations
    public void PlayTween()
    {
        iTween.RotateTo(Joints[0], iTween.Hash("x",JointsRotationalVector[0].x,"y",0,"z", JointsRotationalVector[0].z,"time",1,"onComplete", "OnAnimationComplete","onCompleteTarget", gameObject));

    }

    public void OnAnimationComplete()
    {
        TweenCounter++;
        if (TweenCounter==AllRotationalTweens.Length-1)
        {
            
            //foreach(GameObject joint in Joints)
            //{
            //    joint.transform.rotation = Quaternion.identity;
            //}
            iTween.RotateTo(Joints[TweenCounter], iTween.Hash("x", JointsRotationalVector[TweenCounter].x, "y", 0, "z", JointsRotationalVector[TweenCounter].z, "time", 1));
            TweenCounter = 0;
        }
        else
        {
            iTween.RotateTo(Joints[TweenCounter], iTween.Hash("x", JointsRotationalVector[TweenCounter].x, "y", 0, "z", JointsRotationalVector[TweenCounter].z, "time", 1, "onComplete", "OnAnimationComplete", "onCompleteTarget", gameObject)); 
        }
    }

    // Stop Animations
    public void StopTween()
    {
        //foreach(GameObject joint in Joints)
        //{
        //    joint.GetComponent<TweenRotation>().enabled = false;
        //}
    }
}
