using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    
    public GameObject personPrefab;
    [Range(1, 100)]
    public int populationSize = 10;

    public bool controlColor;
    public bool controlSize;
    
    public float delay = 0.5f;
    public float generationDelay = 1f;
    
    [Header("Mutation amount in n/10,000")]
    public int mutationRate = 1;

    public bool automated;
    
    public Color idealColor;
    [Range(.1f, 0.3f)]
    public float idealSize;
    
    
    public static float elapsed = 0;
    
    List<GameObject> population = new List<GameObject>();
    private float trialTime = 10f;
    private int generation = 1;
    private float automatedElapsed = 0;
    
    GUIStyle guiStyle = new GUIStyle();
    
    // Start is called before the first frame update
    void Start()
    {
        CreatePopulation();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        
        if (automated)
        {
            elapsed += Time.deltaTime;
            
            automatedElapsed += Time.deltaTime;
            if (automatedElapsed > delay)
            {
                if (!IsPopulationDead())
                {
                    RemovePerson();
                }
                else
                {
                    CreatePopulation();
                }

                automatedElapsed = 0;
            }
                
        }
        else
        {
            
            if (IsPopulationDead())
            {
                CreatePopulation();
                elapsed = 0;
            }
        }
        
        
        
    }
    
    private void CreatePopulation()
    {
        if (generation == 1)
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-9,9), Random.Range(-4.5f, 4.5f), 0);
                GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);
                go.GetComponent<DNA>().r = Random.Range(0.0f, 1.0f);
                go.GetComponent<DNA>().g = Random.Range(0.0f, 1.0f);
                go.GetComponent<DNA>().b = Random.Range(0.0f, 1.0f);
                go.GetComponent<DNA>().scale = Random.Range(0.1f, 0.3f);
                population.Add(go);
            }
            
            generation++;
        }
        else
        {
            List<GameObject> newPopulation = new List<GameObject>();
            List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA>().timeToDie).ToList();
            
            CleanUpPersonObjects();
            population.Clear();
            
            for (int i = (int) (sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
            {
                newPopulation.Add(Breed(sortedList[i], sortedList[i + 1]));
                newPopulation.Add(Breed(sortedList[i + 1], sortedList[i]));
            }
        
            population = newPopulation;
            generation++;
        }
        
    }

    private void CleanUpPersonObjects()
    {
        for (int i = 0; i < population.Count; i++)
        {
            Destroy(population[i]);
        }
    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(Random.Range(-9,9), Random.Range(-4.5f, 4.5f), 0);
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);

        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        
        //TODO add different % rates for color and size
        
        if (Random.Range(0, 10000) > mutationRate)
        {
            // swap parent DNA
            offspring.GetComponent<DNA>().r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r;
            offspring.GetComponent<DNA>().g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g;
            offspring.GetComponent<DNA>().b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b;
            
            offspring.GetComponent<DNA>().scale = Random.Range(0, 10) < 5 ? dna1.scale : dna2.scale;
            
        }
        else
        {
            offspring.GetComponent<DNA>().r = Random.Range(0.0f, 1.0f);
            offspring.GetComponent<DNA>().g = Random.Range(0.0f, 1.0f);
            offspring.GetComponent<DNA>().b = Random.Range(0.0f, 1.0f);
            
            offspring.GetComponent<DNA>().scale = Random.Range(0.1f, 0.3f);
        }
        
        
        return offspring;
    }

    private bool IsPopulationDead()
    {
        for (int i = 0; i < population.Count; i++)
        {
            if (!population[i].GetComponent<DNA>().isDead)
                return false;
        }

        return true;
    }
    
    

    private void RemovePerson()
    {
        var r = -1f;
        var g = -1f;
        var b = -1f;
        

        int mostDifferent = -1;
        
        for (int i = 0; i < population.Count; i++)
        {
            if (population[i].GetComponent<DNA>().isDead)
                continue;

            var personR = population[i].GetComponent<DNA>().r;
            var personG = population[i].GetComponent<DNA>().g;
            var personB = population[i].GetComponent<DNA>().b;

            var differenceR = Mathf.Abs(idealColor.r - personR);
            var differenceG = Mathf.Abs(idealColor.g - personG);
            var differenceB = Mathf.Abs(idealColor.b - personB);
            
            /*if (differenceR > r && differenceG > g && differenceB > b)
            {
                r = differenceR;
                g = differenceG;
                b = differenceB;
                mostDifferent = i;
            }i*/
            if ((differenceR + differenceG + differenceB) > (r +  g + b))
            {
                r = differenceR;
                g = differenceG;
                b = differenceB;
                mostDifferent = i;
            }
            else if (differenceR == r && differenceG == g && differenceB == b)
            {
                mostDifferent = i;
            }
            

        }
        

        population[mostDifferent].GetComponent<DNA>().Kill();
            
        
    }



    #region Public Methods

    public  float GetElapsedTime()
    {
        return elapsed;
    }

    public int GetPopulationSize()
    {
        return populationSize;
    }

    public void StartNewGeneration()
    {
        print("start new gen");
        CreatePopulation();
        elapsed = 0;
    }

    #endregion
    

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10, 35, 100, 20), "Trial Time: " + (int)elapsed, guiStyle);
    }
}
