using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
 
public class AEEnemyPatrol : MonoBehaviour
{
     //This controller uses the Rigidbody system for movement
    public Animator anim;
    public Transform[] points;      // the array of patrol points
 
    public Transform target;        // this is the player
    public float detectionDistance= 10f;
    public float attackDistance = 1.5f;

    public SpriteRenderer theSR;
 
    private int destPoint = 0;   
    private NavMeshAgent agent;
 
    private bool waiting = false;

    public bool isAttacking = false;
 
    void Start()
    {
        Debug.Log("Starting Start ()");
        agent = this.GetComponent<NavMeshAgent>();
 
        // keep it from stopping at each patrol point
        //agent.autoBraking = false;
 
        StartCoroutine(GoToNextPoint());
    }
 
    IEnumerator GoToNextPoint() 
    {
        Debug.Log("Starting GoToNextPoint()");
        // if no points exist, do nothing
        if(points.Length == 0)
        {
            Debug.Log("waiting for end of frame");
            yield return new WaitForEndOfFrame();       //exit this method()
        }
        
        //wait here for 2 seconds
        Debug.Log("Starting to wait.");
        waiting = true;
        yield return new WaitForSeconds(2);
        waiting = false;
        Debug.Log("Done Waiting, going to next point.");
    
        // Set the agent to go to the currently selected destination
        agent.destination = points[destPoint].position;
    
        //choose the next point in the array as the desination'
        //cycling to the start if necessary
        destPoint = (destPoint + 1) % points.Length;
    }

    void Update()
    {
        // if the NavMeshAgent doesn't exist, do nothing
        if(agent == null) 
        {
            return;
        }
 
        anim.SetFloat("Speed", agent.velocity.magnitude);
        

 
        //when the AI gets close to a destination,
        // go to the next point
        // ! is the NOT operator
 
        float distanceFromTarget = Vector3.Distance(target.position, this.transform.position);

        if(distanceFromTarget < attackDistance) {
            anim.SetBool("isAttacking",true);
        } else {
            anim.SetBool("isAttacking",false);
        }
        
 
        if(distanceFromTarget < detectionDistance && isAttacking == false) 
        {
            Debug.Log("<color=yellow>I see the target! Start Attacking!</color>");
            // stop patrolling, follow target
            isAttacking = true;
            // start following the player
        }
 
        //if the target is far away
        if(distanceFromTarget > detectionDistance * 1.5 && isAttacking == true)      // BF - Changed from false to true
        {
            Debug.Log("<color=cyan> Stop attacking! </color>");
            isAttacking = false;     //reset
    
        }
 
        // If attacking, go to the player.
        if(isAttacking) 
        {
            agent.destination = target.position;
            // BF - return here - don't Gotonextpoint.
            return;
        }
 
        //if the player is close (vector3.distance), agent.dest = player
        //else if, do the following stuff.
        else if(!agent.pathPending && agent.remainingDistance < 0.5f && !waiting) 
        {
            Debug.Log("In Update, calling gotonextpoint()");
            StartCoroutine(GoToNextPoint());
        }
    }
}
 
