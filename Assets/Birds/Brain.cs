using UnityEngine;

namespace Birds
{
    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maze
{
    public class Brain : MonoBehaviour
    {
        private int DNALangth = 6;
        public DNA dna;
        public GameObject eyes;
        private bool seeDownWall;
        bool seeUpWall;
        private bool seeBottom;
        private bool seeTop;
        private bool seeWall = true;
        private Vector3 startPositiopn;
        public float distanceTravelled = 0;
        public int crash = 0;
        bool alive = true;

        private Rigidbody rb;

        public void Init()
        {
            dna = new DNA(DNALangth, 200);
            //this.transform.Translate(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            startPositiopn = transform.position;
            rb = this.GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("top") ||
                collision.gameObject.CompareTag("bottom") ||
                collision.gameObject.CompareTag("wall") ||
                collision.gameObject.CompareTag("downwall"))
            {
                crash++;
            }
            else if (collision.gameObject.CompareTag("dead"))
            {
                alive = false;
            }
        }

        private void Update()
        {
            if (!alive)
                return;
            
            seeDownWall = false;
            seeUpWall = false;
            seeBottom = false;
            seeTop = false;
            
            RaycastHit hit;
            
            if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out hit, 1.0f))
            {
                if (hit.collider.gameObject.CompareTag("upwall"))
                {
                    seeUpWall = true;
                }
                else if (hit.collider.gameObject.CompareTag("downwall"))
                {
                    seeDownWall = true;
                }
            }
            
            if (Physics.Raycast(eyes.transform.position, eyes.transform.up, out hit, 1.0f))
            {
                if (hit.collider.gameObject.CompareTag("upwall"))
                {
                    seeTop = true;
                }
            }
            
            if (Physics.Raycast(eyes.transform.position, -eyes.transform.up, out hit, 1.0f))
            {
                if (hit.collider.gameObject.CompareTag("bottom"))
                {
                    seeBottom = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!alive)
                return;
            
            float upforce = 0;
            float forwardforce = 1.0f;

            if (seeUpWall)
                upforce = dna.GetGene(0);
            else if (seeDownWall)
                upforce = dna.GetGene(1);
            else if (seeTop)
                upforce = dna.GetGene(2);
            else if (seeBottom)
                upforce = dna.GetGene(3);
            else
                upforce = dna.GetGene(4);
            
            rb.AddForce(this.transform.right * forwardforce);
            rb.AddForce(this.transform.up * upforce *0.1f);
            distanceTravelled = Vector3.Distance(startPositiopn, transform.position);
        }
    }

}

}