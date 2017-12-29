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

        // pick elite and add noise
        for (int i = 0; i < eliteSize; i++)
        {
            if (i % 2 == 0)
            {
                AddNoise(ref population[i].chromosome, Random.Range(1, chromosomeLength / 2));
            }
            evolvedChromosomes.Add(population[i].chromosome);
        }

        // ----------------------------------------------------------------

        // cross over elite-elite 
        System.Random rand1 = new System.Random();
        List<Chromosome> elites = new List<Chromosome>(evolvedChromosomes);
        for (int i = 0; i < 8; i++)
        {
            int parent_1_idx = rand1.Next(0, eliteSize - 1);
            int parent_2_idx = rand1.Next(0, eliteSize - 1);
            elites.Remove(evolvedChromosomes[parent_1_idx]);
            elites.Remove(evolvedChromosomes[parent_2_idx]);
            Chromosome[] children =
                Crossover(evolvedChromosomes[parent_1_idx], evolvedChromosomes[parent_2_idx]);

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        // ----------------------------------------------------------------

        // cross over elite-non-elite 
        //System.Random rand2 = new System.Random();
        //for (int i = 0; i < elites.Count(); i++)
        //{
        //    int parent_1_idx = i;
        //    int parent_2_idx = rand2.Next(eliteSize, population.Count());
        //    Chromosome[] children =
        //        Crossover(elites[parent_1_idx], population[parent_2_idx].chromosome);

        //    evolvedChromosomes.Add(children[0]);
        //    evolvedChromosomes.Add(children[1]);
        //}

        // ----------------------------------------------------------------

        // cross over none-elite-none-elite
        //System.Random rand3 = new System.Random();
        //for (int i = 0; i < (populationSize - eliteSize)/2; i++)
        //{
        //    int parent_1_idx = rand3.Next(eliteSize, populationSize);
        //    int parent_2_idx = rand3.Next(eliteSize, populationSize);
        //    Chromosome[] children =
        //        Crossover(population[parent_1_idx].chromosome, population[parent_2_idx].chromosome);

        //    evolvedChromosomes.Add(children[0]);
        //    evolvedChromosomes.Add(children[1]);
        //}

        // ----------------------------------------------------------------

        // Mutate
        System.Random rand4 = new System.Random();
        //int mutationSize = populationSize - evolvedChromosomes.Count();
        int mutationSize = 10;
        for (int i = 0; i < mutationSize; i++)
        {
            int j = rand4.Next(0, eliteSize);
            evolvedChromosomes.Add(Mutate(population[j].chromosome, 2));
        }

        if (evolvedChromosomes.Count != populationSize)
        {
            Debug.LogError(populationSize - evolvedChromosomes.Count +
                " individuals passed to next generation without any alteration!");
        }

        // pass the evolved chromosomes to the next generation population
        for (int i = 0; i < evolvedChromosomes.Count; i++)
        {
            population[i].chromosome = evolvedChromosomes[i];
        }

        // Reset Joints to initial positions
        ResetToInitial();
        // Put a delay for visualization purpose
        StartCoroutine(DelayEvolution());
    }

    private void AddNoise(ref Chromosome chrome, int effectedGenes)
    {
        for(int i = chrome.genes.Count()-1; i>=chrome.genes.Count()-effectedGenes;i--)
        {
            float randNoise = Random.Range(-1, 1);
            chrome.genes[i] = Quaternion.Euler(chrome.genes[i].x + randNoise, 0, chrome.genes[i].z + randNoise);

        }
    }

    private Chromosome Mutate(Chromosome chrome, int effectedLastGenes = 1)
    {
        int i = Random.Range(chromosomeLength-effectedLastGenes, chromosomeLength - 1); 
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
            arm.freeze = false;
        }

        yield return new WaitForSeconds(0.1f);
        updateScene = true;
    }
}
