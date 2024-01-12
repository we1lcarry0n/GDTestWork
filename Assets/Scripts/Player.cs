using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _rotationSpeed = 7.5f;
    [SerializeField] private float _attackCooldown = 1f;
    
    [SerializeField] private float _superAttackMultiplier = 2f;
    [SerializeField] public float SuperAttackCooldown = 2f;

    private float lastAttackTime = 0;
    private bool _canAttack = true;
    private bool isDead = false;
    private Enemie closestEnemie = null;
    public Animator AnimatorController;
    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        Move(CalculateMovement());
        AdjustMoveAnimator(CalculateMovement());

        closestEnemie = CalculateClosestEnemy();

        if (!_canAttack)
        {
            TickAttackCooldown();
        }
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        movement = movement.normalized;
        return movement;
    }

    private void Move(Vector3 movement)
    {
        if (movement == Vector3.zero) return;
        _characterController.Move(movement * _moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(transform.forward), Quaternion.LookRotation(movement), _rotationSpeed * Time.deltaTime);
    }

    private void AdjustMoveAnimator(Vector3 direction)
    {
        AnimatorController.SetFloat("Speed", direction.magnitude);
    }

    private Enemie CalculateClosestEnemy()
    {
        var enemies = SceneManager.Instance.Enemies;
        Enemie closest = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closest == null)
            {
                closest = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closest.transform.position);

            if (distance < closestDistance)
            {
                closest = enemie;
            }

        }
        return closest;
    }

    public void TryAttack()
    {
        if (!_canAttack)
        {
            return;
        }
        if (closestEnemie != null)
        {
            TryDealDamage(Damage);
        }
        AnimatorController.SetTrigger("Attack");
        _canAttack = false;
        lastAttackTime = 0f;
    }

    public void TrySuperAttack()
    {
        if (!SceneManager.Instance.CanSuperAttack || !_canAttack)
        {
            return;
        }
        if (Vector3.Distance(transform.position, closestEnemie.transform.position) > AttackRange)
        {
            return;
        }
        if (closestEnemie != null)
        {
            TryDealDamage(Damage * _superAttackMultiplier);
        }
        AnimatorController.SetTrigger("SuperAttack");
        SceneManager.Instance.InitiateSuperAttackCooldown();
        _canAttack = false;
        lastAttackTime = 0f;
    }

    private void TryDealDamage(float damage)
    {
        if (closestEnemie == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, closestEnemie.transform.position) > AttackRange)
        {
            return;
        }
        transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
        closestEnemie.Hp -= damage;
    }

    private void TickAttackCooldown()
    {
        lastAttackTime += Time.deltaTime;
        if (lastAttackTime >= _attackCooldown)
        {
            _canAttack = true;
        }
    }
}
