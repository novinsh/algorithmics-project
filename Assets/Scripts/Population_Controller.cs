using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Population_Controller : MonoBehaviour {

    List<RoboticArm> population = new List<RoboticArm>();
    int populationSize=100;
    int chromosomeLength=4;
    double eliteRate=0.3f;
    double mutationRate = 0.01f;
    public GameObject target;
    public GameObject prefab;

    bool generateNew = false;
    int generationCounter = 0;


	// Use this for initialization
	void Start () {
        
        InitPopulation();

    }
	
	// Update is called once per frame
	void Update () {
        if (generateNew)
        {
                if (generationCounter < 100)
            {
                
                generateNew = false;
                NewGeneration();
                generationCounter++;
            }
            else
            {
                foreach(RoboticArm p in population)
                {
                    p.updateItMan = true;
                }
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
        generateNew = true;
    }


    void NewGeneration()
    {
        population = population.OrderBy(o => o.fitness).ToList();
        population.Reverse();
        List<Chromosome> evolvedChromosomes = new List<Chromosome>();
        int eliteSize = (int)(eliteRate * populationSize);
        float mutationChance = Random.Range(0.0f, 1.0f);
        int mutationSize = 0;
        if (mutationChance <= mutationRate)
        {
            mutationSize = (int)(0.1 * (populationSize - populationSize));  // mutation fraction
        }

        // Pick elite
        for (int i = 0; i < eliteSize; i++)
        {
            evolvedChromosomes.Add(population[i].chromosome);
        }


        // Mutate
        for (int i=0; i< mutationSize; i++)
        {
            System.Random rand = new System.Random();
            int j = rand.Next(eliteSize, populationSize);
            evolvedChromosomes.Add(Mutate(population[j].chromosome));
        }

        // Crossover
        // TODO: do not considern only the elit for crossover (that's discrimination!)
        for (int i = 0; i < (populationSize - eliteSize - mutationSize) / 2; i++)
        {
            System.Random rand = new System.Random();
            int j = rand.Next(0, eliteSize - 1); // to evolve (target individual)
            int k = rand.Next(0, eliteSize - 1); // to use as partner
            List<Chromosome> children = 
                Crossover(evolvedChromosomes[j], evolvedChromosomes[k]);

            evolvedChromosomes.Add(children[0]);
            evolvedChromosomes.Add(children[1]);
        }

        for(int i=0;i< evolvedChromosomes.Count;i++)
        {

            population[i].chromosome = evolvedChromosomes[i];
            Debug.Log(population[i].chromosome.genes[0]);
        }

        // TODO: fill up the population before we extinct!
        if (evolvedChromosomes.Count < populationSize)
        {
            Debug.Log("What have you done to his world!");
        }

        // Reset Joints to initial positions
        ResetToInitial();

        StartCoroutine(DelayEvolution());
    }

    private Chromosome Mutate(Chromosome c)
    {
        // TODO: mutute the c (input)
        return new Chromosome(4);
    }

    private List<Chromosome> Crossover(Chromosome father, Chromosome mother)
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
        List<Chromosome> children = new List<Chromosome>();
        children.Add(child1);
        children.Add(child2);
        //Chromosome[] children = new Chromosome[2] { child1, child2}; // happy children
        return children;
    }


    void ResetToInitial()
    {
        foreach(RoboticArm arm in population)
        {
            arm.freeze = true;
            foreach (GameObject joint in arm.Joints)
            {
                joint.transform.rotation = Quaternion.identity;
            }
        }
    }


    IEnumerator DelayEvolution()
    {
        yield return new WaitForSeconds(0.01f);
        foreach (RoboticArm arm in population)
        {
            arm.freeze = false;
        }

        yield return new WaitForSeconds(0.01f);
        generateNew = true;
    }
}
