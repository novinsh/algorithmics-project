using System.Collections.Generic;
using UnityEngine;


public class Chromosome 
{
    public List<Quaternion> genes;  // properties of the creature
    int chromosomeLength;           // length of the properties

    public Chromosome(int _chromosomeLength)
    {
        chromosomeLength = _chromosomeLength;
        genes = new List<Quaternion>();
        for (int i = 0; i < chromosomeLength; i++)
        {
            genes.Add(Quaternion.Euler(Random.Range(-90, 90), 0, Random.Range(-90, 90)));
            //Debug.Log(genes[i]);
        }
    }

    public Chromosome(List<Quaternion> _genes)
    {
        genes = _genes;
    }

    public string getInString()
    {
        string tmp = "";
        foreach (Quaternion g in genes)
        {
            tmp = tmp + g.ToString();
        }
        return tmp;
    }

    /* OBSOLETE IMPLEMNETATION OF GENETIC OPERATIONS
    public Chromosome Evolve(Chromosome partner, double mutationRate = 0.01)
    {
        float mutationChance = Random.Range(0.0f, 1.0f);
        if (mutationChance <= mutationRate)
        {
            Mutate();
            return null;
        }
        else
        {
            return Crossover(partner);
        }
    }

    private void Mutate()
    {
        genes.Clear();

        for (int i = 0; i < chromosomeLength; i++)
        {
            genes.Add(Quaternion.Euler(Random.Range(-90, 90), 0, Random.Range(-90, 90)));
        }
    }

    private Chromosome Crossover(Chromosome partner)
    {
        int cutOff = Random.Range(1, chromosomeLength - 2);
        List<Quaternion> child1_genes = new List<Quaternion>();
        List<Quaternion> child2_genes = new List<Quaternion>();

        for (int i = 0; i < chromosomeLength; i++)
        {
            if (i < cutOff)
            {
                child1_genes.Add(genes[i]);
                child2_genes.Add(partner.genes[i]);
            }
            else
            {
                child1_genes.Add(partner.genes[i]);
                child2_genes.Add(genes[i]);
            }
        }

        genes = child1_genes;
        return new Chromosome(child2_genes);
    }
    */
}

