using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    #region Public variables
    
    [Header("Population Settings")]
    [SerializeField] GameObject personPrefab;
    [Range(1, 100)]
    [SerializeField] int populationSize = 10;

    [Header("Delay Between Eliminations")]
    [SerializeField] float delay = 0.5f;

    [Header("Mutation amount in n/10,000")]
    [SerializeField] int mutationRate = 1;

    [Header("Should the test be automated")]
    [SerializeField] bool automated;
    
    [Header("Preferred Color")]
    [SerializeField] Color idealColor;
    
    [Header("Preferred Size")]
    [Range(.1f, 0.3f)]
    [SerializeField] float idealSize;
    
    #endregion

    #region Private variables
    
    private static float _elapsed = 0;
    private List<GameObject> _population = new List<GameObject>();
    private int _generation = 1;
    private float _automatedElapsed = 0;
    readonly GUIStyle _guiStyle = new GUIStyle();
    
    #endregion
    
    #region Unity
    
    void Start()
    {
        CreatePopulation();
    }
    
    void Update()
    {
        _elapsed += Time.deltaTime;
        
        if (automated)
        {
            _elapsed += Time.deltaTime;
            
            _automatedElapsed += Time.deltaTime;
            if (_automatedElapsed > delay)
            {
                if (!IsPopulationDead())
                {
                    RemovePerson();
                }
                else
                {
                    CreatePopulation();
                }

                _automatedElapsed = 0;
            }
                
        }
        else
        {
            
            if (IsPopulationDead())
            {
                CreatePopulation();
                _elapsed = 0;
            }
        }

    }
    
    #endregion
    
    #region Private methods
    private void CreatePopulation()
    {
        if (_generation == 1)
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-9,9), Random.Range(-4.5f, 4.5f), 0);
                GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);
                DNA tempDNA = go.GetComponent<DNA>();
                tempDNA.r = Random.Range(0.0f, 1.0f);
                tempDNA.g = Random.Range(0.0f, 1.0f);
                tempDNA.b = Random.Range(0.0f, 1.0f);
                tempDNA.scale = Random.Range(0.1f, 0.5f);
                _population.Add(go);
            }
            
            _generation++;
        }
        else
        {
            List<GameObject> newPopulation = new List<GameObject>();
            List<GameObject> sortedList = _population.OrderBy(o => o.GetComponent<DNA>().timeToDie).ToList();
            
            CleanUpPersonObjects();
            _population.Clear();
            
            for (int i = (int) (sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
            {
                newPopulation.Add(Breed(sortedList[i], sortedList[i + 1]));
                newPopulation.Add(Breed(sortedList[i + 1], sortedList[i]));
            }
        
            _population = newPopulation;
            _generation++;
        }
        
    }

    private void CleanUpPersonObjects()
    {
        foreach (var p in _population)
        {
            Destroy(p);
        }
    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(Random.Range(-9,9), Random.Range(-4.5f, 4.5f), 0);
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
        DNA offspringDNA = offspring.GetComponent<DNA>();

        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        
        //TODO add different % rates for color and size
        
        if (Random.Range(0, 10000) > mutationRate)
        {
            
            
            // swap parent DNA
            offspringDNA.r = Random.Range(0, 10) < 5 ? dna1.r + Random.Range(-.05f, .05f) : dna2.r + Random.Range(-.05f, .05f);
            offspringDNA.g = Random.Range(0, 10) < 5 ? dna1.g + Random.Range(-.05f, .05f)  : dna2.g + Random.Range(-.05f, .05f);
            offspringDNA.b = Random.Range(0, 10) < 5 ? dna1.b + Random.Range(-.05f, .05f)  : dna2.b + Random.Range(-.05f, .05f);
            
            offspringDNA.scale = Random.Range(0, 10) < 5 ? dna1.scale + Random.Range(-.05f, .05f)  : dna2.scale + Random.Range(-.05f, .05f);
            
        }
        else
        {
            offspringDNA.r = Random.Range(0.0f, 1.0f);
            offspringDNA.g = Random.Range(0.0f, 1.0f);
            offspringDNA.b = Random.Range(0.0f, 1.0f);
            
            offspringDNA.scale = Random.Range(0.1f, 0.5f);
        }
        
        
        return offspring;
    }

    private bool IsPopulationDead()
    {
        foreach (var p in _population)
        {
            if (!p.GetComponent<DNA>().isDead)
                return false;
        }

        return true;
    }
    
    

    private void RemovePerson()
    {
        var r = -1f;
        var g = -1f;
        var b = -1f;
        var s = -1f;
        

        int mostDifferentColor = -1;
        int mostDifferentSize = -1;
        int mostDifferent = -1;
        
        for (int i = 0; i < _population.Count; i++)
        {
            if (_population[i].GetComponent<DNA>().isDead)
                continue;
            
            var personDNA = _population[i].GetComponent<DNA>();

            var personR = personDNA.r;
            var personG = personDNA.g;
            var personB = personDNA.b;

            var differenceR = Mathf.Abs(idealColor.r - personR);
            var differenceG = Mathf.Abs(idealColor.g - personG);
            var differenceB = Mathf.Abs(idealColor.b - personB);
            
            if (differenceR > r || differenceB > b || differenceG > g)
            {
                if ((differenceR + differenceG + differenceB) > (r +  g + b))
                {
                    r = differenceR;
                    g = differenceG;
                    b = differenceB;
                    mostDifferentColor = i;
                }
            }
            else if (differenceR == r && differenceG == g && differenceB == b)
            {
                mostDifferentColor = i;
            }
            
            // get biggest size dif
            var personScale = _population[i].GetComponent<DNA>().scale;
            var differenceScale = Mathf.Abs(idealSize - personScale);
            
            if (differenceScale > s)
            {
                s = differenceScale;
                mostDifferentSize = i;
            }

            mostDifferent = mostDifferentSize > mostDifferentColor ? mostDifferentSize : mostDifferentColor;

        }
        

        _population[mostDifferent].GetComponent<DNA>().Kill();
            
        
    }

    #endregion


    #region Public Methods

    public  float GetElapsedTime()
    {
        return _elapsed;
    }

    public int GetPopulationSize()
    {
        return populationSize;
    }

    public void StartNewGeneration()
    {
        print("start new gen");
        CreatePopulation();
        _elapsed = 0;
    }

    #endregion
    

    #region GUI
    private void OnGUI()
    {
        _guiStyle.fontSize = 25;
        _guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + _generation, _guiStyle);
        GUI.Label(new Rect(10, 35, 100, 20), "Trial Time: " + (int)_elapsed, _guiStyle);
    }
    
    #endregion
}
