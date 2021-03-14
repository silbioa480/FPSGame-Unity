using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState 
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    EnemyState m_State;
    public float findDistance = 8f;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    float currentTime = 3f;
    float attackDelay = 2f;
    public int attackPower = 3;
    public float moveDistance = 20f;
    public int maxHp = 15;
    int hp = 15;
    public Slider hpSlider;
    Vector3 originPos;
    CharacterController cc;
    Transform player;
    Animator anim;
    Quaternion originRot;
    NavMeshAgent smith;
    // Start is called before the first frame update
    void Start()
    {
        m_State = EnemyState.Idle;

        player = GameObject.Find("Player").transform;

        cc = GetComponent<CharacterController>();

        originPos = transform.position;
        originRot = transform.rotation;

        anim = transform.GetComponentInChildren<Animator>();

        smith = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gm.gState != GameManager.GameState.Run) return;
        switch(m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }

        hpSlider.value = (float)hp / (float)maxHp;
    }

    void Idle()
    {
        hp = maxHp;
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            //print("상태 전환 : Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }
    void Move()
    {
        if(Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            //print("상태 전환 : Move -> Return");
        }
        else if(Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            // Vector3 dir = (player.position - transform.position).normalized;
            // cc.Move(dir * moveSpeed * Time.deltaTime);
            // transform.forward = dir;

            smith.destination = player.position;
            smith.stoppingDistance = attackDistance;
        }
        else
        {
            m_State = EnemyState.Attack;
            //print("상태 전환 : Move -> Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }
    void Attack()
    {
        if(Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                //print("공격");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else
        {
            m_State = EnemyState.Move;
            //print("상태 전환 : Attack -> Move");
            currentTime = attackDelay;
            anim.SetTrigger("AttackToMove");
        }
    }
    public void AttackAction()
    {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }
    void Return()
    {
        if(Vector3.Distance(transform.position, originPos) > 1f)
        {
            // Vector3 dir = (originPos - transform.position).normalized;
            // cc.Move(dir * moveSpeed * Time.deltaTime);
            // transform.forward = dir;

            smith.destination = originPos;
            smith.stoppingDistance = 0;
        }
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;
            m_State = EnemyState.Idle;
            //print("상태 전환 : Return -> Idle");
            anim.SetTrigger("MoveToIdle");
        }
    }
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }
    void Die()
    {
        StopAllCoroutines();

        StartCoroutine(DieProcess());
    }

    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(1f);

        m_State = EnemyState.Move;
        //print("상태 전환 : Damaged -> Move");
    }

    IEnumerator DieProcess()
    {
        cc.enabled = false;

        yield return new WaitForSeconds(2f);
        //print("소멸!");
        Destroy(gameObject);
    }

    public void HitEnemy(int hitPower)
    {
        if(m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        hp -= hitPower;

        smith.isStopped = true;
        smith.ResetPath();

        if(hp > 0)
        {
            m_State = EnemyState.Damaged;
            //print("상태 전환 : Any state -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            m_State = EnemyState.Die;
            //print("상태 전환 : Any state -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
