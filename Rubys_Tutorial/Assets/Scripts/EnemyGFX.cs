using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyGFX : MonoBehaviour
{
    public AIPath aiPath;    
    Rigidbody2D rigidbody2D;
    Animator animator;
    private RubyController rubyController;
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        rubyController = rubyControllerObject.GetComponent<RubyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
}
