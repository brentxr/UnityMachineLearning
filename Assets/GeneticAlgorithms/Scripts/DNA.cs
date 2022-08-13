using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    
    //gene for color
    public float r;
    public float g;
    public float b;
    public float scale;
    public float timeToDie = 0f;
    public bool isDead = false;

    private PopulationManager populationManager;
    
    

    SpriteRenderer spriteRenderer;
    Collider2D _collider2D;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        spriteRenderer.color = new Color(r, g, b, 1);

        this.transform.localScale = new Vector3(scale, scale, scale);

        populationManager = FindObjectOfType<PopulationManager>();

    }

    private void OnMouseDown()
    {
        isDead = true;
        timeToDie = populationManager.GetElapsedTime();
        spriteRenderer.enabled = false;
        _collider2D.enabled = false;
    }

    public void Kill()
    {
        isDead = true;
        timeToDie = populationManager.GetElapsedTime();
        spriteRenderer.enabled = false;
        _collider2D.enabled = false;
    }
}
