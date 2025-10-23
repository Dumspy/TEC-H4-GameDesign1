using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Collectable : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private string collectedAnimationState = "collected";
    public Action OnCollected;
    private bool collected = false;
    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            collected = true;
            OnCollected?.Invoke();
            animator.SetBool("isCollected", true);
            col.enabled = false;
        }
    }

    private void Update()
    {
        if (collected)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(collectedAnimationState) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
