using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    [SerializeField] private float _moveSpeed = 4;
    [SerializeField] private float _rotationSpeed = 7.5f;

    private float lastAttackTime = 0;
    private bool isDead = false;
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

        /*var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }

        }

        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (distance <= AttackRange)
            {
                if (Time.time - lastAttackTime > AtackSpeed)
                {
                    //transform.LookAt(closestEnemie.transform);
                    transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);

                    lastAttackTime = Time.time;
                    closestEnemie.Hp -= Damage;
                    AnimatorController.SetTrigger("Attack");
                }
            }
        }*/
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

}
