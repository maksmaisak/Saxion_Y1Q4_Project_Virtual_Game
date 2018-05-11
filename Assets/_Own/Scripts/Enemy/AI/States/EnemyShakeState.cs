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
    void FixedUpdate()
    {
        if (shakingBehaviour)
        {
            rb.AddForce(new Vector3(Random.Range(-2, 2) * maxShakingForce, Random.Range(-2, 2) * maxShakingForce, Random.Range(-2, 2) * maxShakingForce), ForceMode.Impulse);
            counter += Time.fixedDeltaTime;
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
