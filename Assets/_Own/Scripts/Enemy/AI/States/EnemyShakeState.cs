using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShakeState : FSMState<Enemy>
{
    [SerializeField] private float shakingLenght = 3;
    [SerializeField] private float maxShakingForce = 5;

    private bool shakingBehaviour;
    private float counter = 0;
    private Rigidbody rb;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        shakingBehaviour = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakingBehaviour)
        {
            rb.AddForce(new Vector3(Random.Range(0, 2) * maxShakingForce, Random.Range(0, 2) * maxShakingForce, Random.Range(0, 2) * maxShakingForce), ForceMode.Impulse);
            counter += Time.deltaTime;
            if (counter >= shakingLenght)
            {
                shakingBehaviour = false;
                counter = 0;
            }
        }

    }

    public override void Exit()
    {
        base.Exit();
    }
}
