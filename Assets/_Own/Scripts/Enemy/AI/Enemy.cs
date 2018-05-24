using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio))]
[RequireComponent(typeof(SteeringManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShootingController))]
[RequireComponent(typeof(Grappleable))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Saveable))]
public class Enemy : MonoBehaviour, IAgent
{
    private static List<SteeringManager> steeringManagers = new List<SteeringManager>();
    public static IEnumerable<SteeringManager> allAsSteerables { get { return steeringManagers.AsReadOnly(); } }

    [SerializeField] private GrappleReactionBehaviour grappleReactionBehaviour;

    public FSM<Enemy> fsm { get; private set; }

    new public EnemyAudio audio { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public SteeringManager steering { get; private set; }
    public ShootingController shootingController { get; private set; }
    public Health health { get; private set; }
    public Grappleable grappleable { get; private set; }
    new public Rigidbody rigidbody { get; private set; }

    private float initialHeight;

    private Saveable saveable;
    struct SaveData 
    {
        public bool isDead;
    }

    private enum GrappleReactionBehaviour
    {
        None,
        ThrustUp,
        Shake,
        PullPlayer
    }

    void Awake()
    {
        saveable = GetComponent<Saveable>();
        LoadSaveData();
    }

    // Use this for initialization
    void Start()
    {
        initialHeight = transform.position.y;

        fsm = new FSM<Enemy>(this);

        audio = GetComponent<EnemyAudio>();
        particleManager = GetComponentInChildren<ParticleManager>();
        health = GetComponent<Health>();

        rigidbody = GetComponent<Rigidbody>();

        steering = GetComponent<SteeringManager>();
        steeringManagers.Add(steering);

        shootingController = GetComponent<ShootingController>();

        grappleable = GetComponent<Grappleable>();
        grappleable.OnGrappled   += OnGrapple;
        grappleable.OnUngrappled += OnRelease;

        fsm.ChangeState<EnemyMoveRandomlyAroundPointState>();

        StartCoroutine(WhileGrappledScreamCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    void OnDestroy()
    {
        steeringManagers.Remove(steering);

        bool isDead =
            !gameObject.activeSelf ||
            (health != null && health.isDead) ||
            IsFallingToDeath();
        
        saveable.SaveData(new SaveData() {isDead = isDead});
    }

    IEnumerator WhileGrappledScreamCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
            // Seriously meh.
            yield return new WaitUntil(() => grappleable.isGrappled && fsm.GetCurrentState().GetType() != typeof(EnemyFallingToDeathState));
            audio.PlayScreamWhileGrappled();
        }
    }

    public void Print(string message)
    {
        Debug.Log(message);
    }

    public float GetInitialHeight()
    {
        return initialHeight;
    }

    private void OnGrapple(Grappleable sender)
    {
        if (IsFallingToDeath())
        {
            return;
        }

        audio.PlayOnGrappled();
        particleManager.SwitchGrappled();

        switch (grappleReactionBehaviour)
        {
            case GrappleReactionBehaviour.ThrustUp:
                fsm.ChangeState<EnemyThrustUpState>();
                break;
            case GrappleReactionBehaviour.Shake:
                fsm.ChangeState<EnemyShakeState>();
                break;
            case GrappleReactionBehaviour.PullPlayer:
                fsm.ChangeState<EnemyPullPlayerState>();
                break;
        }

        GetComponent<Shooting>().enabled = false;
    }

    private void OnRelease(Grappleable sender)
    {
        fsm.ChangeState<EnemyFallingToDeathState>();
    }

    private void LoadSaveData()
    {
        SaveData saveData;
        if (!saveable.GetSavedData(out saveData)) return;

        if (saveData.isDead) 
        {
            gameObject.SetActive(false);
        }
    }

    private bool IsFallingToDeath()
    {
        if (fsm == null) return false;
        // Meh, but works.
        return fsm.GetCurrentState().GetType() == typeof(EnemyFallingToDeathState);
    }
}
