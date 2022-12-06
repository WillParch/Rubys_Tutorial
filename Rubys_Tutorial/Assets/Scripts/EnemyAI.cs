using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public Transform enemyGFX;
    float timer;
    Path path;
    int Direction = 1;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    Animator animator;
    private RubyController rubyController;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        rubyController = rubyControllerObject.GetComponent<RubyController>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    
    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (path == null)
            return;
        
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
             timer -= Time.deltaTime;
            return;
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

         if(force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
            
        } 
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
            
        }
    
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            gameObject.GetComponent<Animator>().Play("Idle");
            player.ChangeHealth(-1);
            
        }
    }

    
            
}


