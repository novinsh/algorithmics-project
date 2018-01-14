using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Population_Controller : MonoBehaviour {

    List<RoboticArm> population = new List<RoboticArm>();
    [Header("Tweakers")]
    [Space(15)]
    public int populationSize = 100;    // number of robotic arms (virtual creatures)
    public int numOfGenerations;  // number of generations
    [HideInInspector]
    public int chromosomeLength;        // number of genes (properties of the creature) = chromosomeLength
    [Range(0, 1f)]
    public float eliteRate;             // number of top best individuals in each generation
    [Range(0, 1f)]
    public float mutationRate = 0.01f;  // chance of getting mutated for each individual 
    [Range(0, 1f)]
    public float eliteXOverRate = 0.4f; // number of elite-elite cross over

    //[Range(-1,1)]
    public float noiseRate;     // TODO: employ this factor!

    public bool Pause = false;  // Pause called by user to stop iteration and see current best


    [Header("References")]
    [Space(15)]
    public GameObject target;   // target for the creature
    public GameObject prefab;   // prefabrication of the robotic arm
    public Text UIcurrentGeneration;
    public Text UIpopulation;
    public Text UICurrentFittest;


    public int InitialPopCounter = 0;


    bool updateScene = false;   // we update in delayed intervals for visualization purposes
    int generationCounter = 0;  // to keep track of generation 
    bool playAnimation = true;


	// Use this for initialization
	void Start () {
        chromosomeLength = prefab.GetComponent<RoboticArm>().Joints.Count();
        InitPopulation();
        UIpopulation.text = "Population Size: " + populationSize;
    }
	
	// Update is called once per frame
	void Update () {

        // if user paused the iteration
        if (!Pause)
        {
            // Unfreeze arms
            if(!playAnimation)
            {
                population[0].StopTween();
                foreach (RoboticArm arm in population)
                {
                    arm.freeze = false;
                    arm.gameObject.SetActive(true);
                }
                playAnimation = true;
            }
            
            // update the scene with the defined intervals
            if (!updateScene)
                return;

            updateScene = false;

            // TODO: stop optimization when reached at the target!
            if (generationCounter < numOfGenerations)
            {

                //Debug.Log("Generation: " + generationCounter);
                NewGeneration();
                generationCounter++;
            }
            else // This is to update only if we want to show the final result of the optimization!
            {
                //foreach (RoboticArm p in population)
                //{
                //    p.freeze = false;
                //}
            }
            //Current Gen and Fittest Text
            UIcurrentGeneration.text = "Current Generation: " + generationCounter;
        }
        else
        {
            if(playAnimation)
            {
                playAnimation = false;
                foreach(RoboticArm arm in population)
                {
                    arm.freeze = true;
                    arm.gameObject.SetActive(false);
                }
                population = population.OrderBy(o => o.fitness).ToList();
                population.Reverse();
                RoboticArm currentFittest = population[0];
                Vector3[] jointAngles = new Vector3[currentFittest.Joints.Length];
                for(int i =0;i<jointAngles.Length;i++)
                {
                    jointAngles[i] = currentFittest.Joints[i].transform.rotation.eulerAngles;
                }
                foreach(GameObject joint in currentFittest.Joints)
                {
                    joint.transform.rotation = Quaternion.identity;
                }

                currentFittest.gameObject.SetActive(true);
                //jointAngles.Reverse();
                currentFittest.JointsRotationalVector = jointAngles;
                currentFittest.PlayTween();
            }

        }
    }

    void InitPopulation()
    {
        for (int i = 0; i < populationSize-1; i++)
        {
            GameObject go = Instantiate(prefab, prefab.transform.position, Quaternion.identity);
            go.GetComponent<RoboticArm>().InitArm(new Chromosome(chromosomeLength), target.transform.position);
            population.Add(go.GetComponent<RoboticArm>());
        }
        prefab.GetComponent<RoboticArm>().InitArm(new Chromosome(chromosomeLength), target.transform.position);
        population.Add(prefab.GetComponent<RoboticArm>());
        updateScene = true;
    }


    void NewGeneration()
    {
       

        // find fitness of the population
        population = population.OrderBy(o => o.fitness).ToList();
        population.Reverse();   // sort descending


        // Show current fittest
        UICurrentFittest.text = population[0].gameObject.name + "   : " + System.Math.Round(population[0].fitness,5);

        List<Chromosome> evolvedChromosomes = new List<Chromosome>();
        int eliteSize = (int)(eliteRate * populationSize);

        // pick elite and add noise
        for (int i = 0; i < eliteSize; i++)
        {
            if (i % 2 == 0)
            {
                // FIXME: the noise is biased - turns the endeffector upward (perhaps a gaussian noise is better!)
                //AddNoise(ref population[i].chromosome, Random.Range(1, chromosomeLength-1));
            }
            evolvedChromosomes.Add(population[i].chromosome);
        }

        // ----------------------------------------------------------------

        // cross over elite-elite 
        System.Random rand1 = new System.Random();
        List<Chromosome> elitesAsParent = new List<Chromosome>(evolvedChromosomes); // because so far, evolvedChromosomes contain only elites
        int elitEliteSize = (int)((eliteSize/2) * eliteXOverRate);
        for (int i = 0; i < elitEliteSize; i++)
        {
            int parent_1_idx = rand1.Next(0, eliteSize - 1);
            int parent_2_idx = rand1.Next(0, eliteSize - 1);
            elitesAsParent.Remove(evolvedChromosomes[parent_1_idx]);
            elitesAsParent.Remove(evolvedChromosomes[parent_2_idx]);
            Chromosome[] children =
                Crossover(evolvedChromosomes[parent_1_idx], evolvedChromosomes[parent_2_idx]);

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        // ----------------------------------------------------------------

        // cross over elite-non-elite 
        System.Random rand2 = new System.Random();
        for (int i = 0; i < elitesAsParent.Count()/2; i++)
        {
            int parent_1_idx = i;
            int parent_2_idx = rand2.Next(eliteSize, population.Count());
            Chromosome[] children =
                Crossover(elitesAsParent[parent_1_idx], population[parent_2_idx].chromosome);

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        // ----------------------------------------------------------------

        // cross over none-elite-none-elite
        System.Random rand3 = new System.Random();
        int nonEliteCrossOverSize = (populationSize - evolvedChromosomes.Count) / 2;
        for (int i = 0; i < nonEliteCrossOverSize; i++)
        {
            int parent_1_idx = rand3.Next(eliteSize, populationSize);
            int parent_2_idx = rand3.Next(eliteSize, populationSize);
            Chromosome[] children =
                Crossover(population[parent_1_idx].chromosome, population[parent_2_idx].chromosome);

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        // ----------------------------------------------------------------

        // Mutate - this mutation overwrite some of the crossovered individuals as if they were not exiting at all
        for (int i = 0; i < evolvedChromosomes.Count; i++)
        {
            float mutationChance = Random.Range(0.0f, 1.0f);
            if (mutationChance <= mutationRate)
            {
                // for elite mutate only one of the last two joints
                if( i < eliteSize)
                {
                    Mutate(evolvedChromosomes[i], 2);

                }
                else // mutate all joints for non-elite
                {
                    //Mutate(evolvedChromosomes[i]);
                    Mutate();
                }
            }
        }

        // Make sure the population size remain same
        if (evolvedChromosomes.Count != populationSize)
        {
            Debug.LogError(populationSize - evolvedChromosomes.Count +
                " individuals passed to next generation without any alteration!");
        }

        // pass the evolved chromosomes to the next generation
        for (int i = 0; i < evolvedChromosomes.Count(); i++)
            population[i].chromosome = evolvedChromosomes[i];

        // Reset Joints to initial positions
        ResetToInitial();
        // Put a delay for visualization purpose
        StartCoroutine(DelayEvolution());
    }

    private void AddNoise(ref Chromosome chrome, int effectedGenes)
    {
        for(int i = chrome.genes.Count()-1; i>=effectedGenes;i--)
        {
            float randNoise = Random.Range(-0.5f, 0.5f);
            //testing...
            //Vector3 temp = new Vector3(chrome.genes[i].eulerAngles.x + randNoise, 0, chrome.genes[i].eulerAngles.x + randNoise);
            //Quaternion temp2 = Quaternion.EulerAngles(temp);
            //chrome.genes[i] = temp2;
            chrome.genes[i] = Quaternion.Euler(chrome.genes[i].x + randNoise, 0, chrome.genes[i].z + randNoise);

        }
    }

    private Chromosome Mutate(Chromosome chrome, int effectedLastGenes = 1)
    {
        System.Random rand = new System.Random();
        //int i = Random.Range(chromosomeLength-effectedLastGenes, chromosomeLength - 1); 
        int i = rand.Next(chromosomeLength - effectedLastGenes, chromosomeLength - 1);
        chrome.genes[i] = Quaternion.Euler(Random.Range(-90, 90), 0, Random.Range(-90, 90));
        return chrome;
    }

    private Chromosome Mutate()
    {
        return new Chromosome(4); // generate totaly a new chromosome randomly
    }

    private Chromosome[] Crossover(Chromosome father, Chromosome mother)
    {
        int cutOff = Random.Range(1, chromosomeLength - 2);
        List<Quaternion> child1_genes = new List<Quaternion>();
        List<Quaternion> child2_genes = new List<Quaternion>();

        for (int i = 0; i < chromosomeLength; i++)
        {
            if (i < cutOff)
            {
                child1_genes.Add(father.genes[i]);
                child2_genes.Add(mother.genes[i]);
            }
            else
            {
                child1_genes.Add(mother.genes[i]);
                child2_genes.Add(father.genes[i]);
            }
        }

        Chromosome child1 = new Chromosome(child1_genes);
        Chromosome child2 = new Chromosome(child2_genes);
        Chromosome[] children = new Chromosome[2] { child1, child2 }; // happy children
        return children;
    }


    void ResetToInitial()
    {
        foreach(RoboticArm arm in population)
        {
            arm.freeze = true;
            // TODO: maybe better not to reset!
            //foreach (GameObject joint in arm.Joints)
            //{
            //    joint.transform.rotation = Quaternion.identity;
            //}
        }
    }


    IEnumerator DelayEvolution()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (RoboticArm arm in population)
        {
            if(!Pause)
                arm.freeze = false;
        }

        yield return new WaitForSeconds(0.1f);
        updateScene = true;
    }
}
