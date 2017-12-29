using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Population_Controller : MonoBehaviour {

    List<RoboticArm> population = new List<RoboticArm>();
    [Header("Tweakers")]
    [Space(15)]
    public int populationSize = 100;   // number of robotic arms (virtual creatures)
    [HideInInspector]
    public int chromosomeLength;    // number of genes (properties of the creature)
    [Range(0,1)]
    public float eliteRate;    // number of top best individuals in each generation
    [Range(0, 1)]
    public float mutationRate = 0.1f; // chance of getting mutated for each individual 
    //[Range(0, 0.5f)]
    //public float crossOverRate;    // number of top best individuals in each generation

    //[Range(-1,1)]
    public float noiseRate;


    [Header("References")]
    [Space(15)]
    public GameObject target;   // target for the creature
    public GameObject prefab;   // prefabrication of the robotic arm



    bool updateScene = false;   // we update in delayed intervals for visualization purposes
    int generationCounter = 0;  // to keep track of generation 


	// Use this for initialization
	void Start () {
        chromosomeLength = prefab.GetComponent<RoboticArm>().Joints.Count();
        InitPopulation();
    }
	
	// Update is called once per frame
	void Update () {
        // update the scene with the defined intervals
        if (!updateScene)
            return;

        updateScene = false;
        if (generationCounter < 100)
        {
            //Debug.Log("Generation: " + generationCounter);
            NewGeneration();
            generationCounter++;
        }
        else
        {
            //foreach (RoboticArm p in population)
            //{
            //    p.updateItMan = true;
            //}
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
        population = population.OrderBy(o => o.fitness).ToList();
        population.Reverse();
        List<Chromosome> evolvedChromosomes = new List<Chromosome>();
        int eliteSize = (int)(eliteRate * populationSize);
        float mutationChance = Random.Range(0.0f, 1.0f);
        int mutationSize = 0;
        //Debug.Log("Elite Size" + eliteSize);

        // FIXME: This type of mutation is probably not effective or correct in essence!
        if (mutationChance <= mutationRate)
        {
            mutationSize = (int)(0.1 * (populationSize - eliteSize));  // mutation fraction
            Debug.Log("Mutation size" + mutationSize);
        }

        // Pick elite 
        for (int i = 0; i < eliteSize; i++)
        {
            evolvedChromosomes.Add(population[i].chromosome);
        }

        // Mutate
        System.Random rand = new System.Random();
        for (int i = 0; i < mutationSize; i++)
        {
            int j = rand.Next(eliteSize, populationSize);
            evolvedChromosomes.Add(Mutate(population[j].chromosome));
        }

        // Crossover (use elite-elite and elite-non-elite offsprings)
        int crossOverSize = (populationSize - eliteSize - mutationSize) / 2;
        int eliteCrossOverSize = (int)(0.1 * crossOverSize);
        for (int i = 0; i < (populationSize - eliteSize - mutationSize) / 2; i++)
        {
            int j = rand.Next(0, eliteSize - 1); // first parent (always from elite)
            //print(j);
            Chromosome[] children = null;
            if (i <= eliteCrossOverSize) // elite-elite crossover
            {
                int k = rand.Next(eliteSize, populationSize - 1); // second parent (elite)
                children = Crossover(evolvedChromosomes[j], population[k].chromosome);
            }
            else // elite-non-elite crossover
            {
                int k = rand.Next(0, eliteSize - 1); // second parent (non-elite)
                children = Crossover(evolvedChromosomes[j], evolvedChromosomes[k]);
            }

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        // Debug.Log("evolvedChromSize" + evolvedChromosomes.Count);
        Dictionary<string, int> sameChromosomesCount = new Dictionary<string, int>();
        for (int i = 0; i < evolvedChromosomes.Count; i++)
        {
            // pass the evolved chromosomes to the next generation population
            population[i].chromosome = evolvedChromosomes[i];
            if (sameChromosomesCount.ContainsKey(population[i].chromosome.getInString()))
                sameChromosomesCount[population[i].chromosome.getInString()] += 1;
            else
                sameChromosomesCount[population[i].chromosome.getInString()] = 1;
            //Debug.Log(i + ": " + population[i].chromosome.getInString());
            //Debug.Log("gene0: " + population[i].chromosome.genes[0]);
        }

        // A debug to see frequency of same chromosomes in population:
        //foreach(KeyValuePair<string, int> entry in sameChromosomesCount)
        //{
        //    Debug.Log(entry.Key + ": " + entry.Value);
        //}

        // FIXME: fix the following scenario
        if (evolvedChromosomes.Count < populationSize)
        {
            Debug.LogError(populationSize - evolvedChromosomes.Count +
                " individuals passed to next generation without any alteration!");
        }

        // Reset Joints to initial positions
        ResetToInitial();
        // Put a delay for visualization purpose
        StartCoroutine(DelayEvolution());
    }

    private void Noise(ref Chromosome chrome, int effectedGenes)
    {
        for(int i = chrome.genes.Count()-1; i>=chrome.genes.Count()-effectedGenes;i--)
        {
            float randNoise = Random.Range(-1, 1);
            chrome.genes[i] = Quaternion.Euler(chrome.genes[i].x + randNoise, 0, chrome.genes[i].z + randNoise);

        }
    }

    private Chromosome Mutate(Chromosome chrome)
    {
        // alter one of the genes randomly
        int i = Random.Range(0, chromosomeLength-1); 
        chrome.genes[i] = Quaternion.Euler(Random.Range(-90, 90), 0, Random.Range(-90, 90));
        return chrome;
        //return new Chromosome(4); // generate totaly a new chromosome randomly
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
            arm.freeze = false;
        }

        yield return new WaitForSeconds(0.1f);
        updateScene = true;
    }
}
