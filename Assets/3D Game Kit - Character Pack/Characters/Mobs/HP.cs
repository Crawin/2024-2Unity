using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HP : MonoBehaviour
{
    public int MAXHP;
    private int currHP;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        currHP = MAXHP;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(int damage)
    { 
        currHP -= damage;
        if (currHP == 0)
        {
            animator.SetTrigger("Die");

        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }
    void Dead()
    {
        Destroy(gameObject);
    }
}
