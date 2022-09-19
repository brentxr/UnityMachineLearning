using UnityEngine;

namespace Birds
{
    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maze
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject botPrefab;
        public GameObject startPosition;
        public int populationSize = 50;
        List<GameObject> population = new List<GameObject>();
        public static float elapsed = 0;
        public float trialTime = 5;
        int generation = 1;
        
        GUIStyle guiStyle = new GUIStyle();

        private void OnGUI()
        {
            guiStyle.fontSize = 25;
            guiStyle.normal.textColor = Color.white;
            GUI.BeginGroup(new Rect(10, 10, 250, 150));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
            GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
            GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
            GUI.EndGroup();
        }

        public void fizz()
        {
            int test = 16;
            
            (test % 3 == 0 && test % 5 == 0) ? Console.WriteLine("FizzBuzz") : Console.WriteLine("Fizz");
        }

        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                GameObject b = Instantiate(botPrefab, startPosition.transform.position, this.transform.rotation);
                b.GetComponent<Brain>().Init();
                population.Add(b);
            }
        }
        
        GameObject Breed(GameObject parent1, GameObject parent2)
        {
            GameObject offspring = Instantiate(botPrefab, startPosition.transform.position, this.transform.rotation);
            Brain b = offspring.GetComponent<Brain>();
            if (Random.Range(0, 100) == 1)
            {
                b.Init();
                b.dna.Mutate();
            }
            else
            {
                b.Init();
                b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }
            return offspring;
        }
        
        void BreedNewPopulation()
        {
            List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().distanceTravelled - o.GetComponent<Brain>().crash).ToList();
            population.Clear();
            

            for (int i = (int) (3 * sortedList.Count / 4.0f) - 1; i < sortedList.Count - 1; i++)
            {
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                Destroy(sortedList[i]);
            }

            generation++;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= trialTime)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }
    }
}
}