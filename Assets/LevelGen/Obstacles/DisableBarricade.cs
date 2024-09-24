using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableBarricade : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Collider _barricadeCollider;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _barricadeCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (player)
            _barricadeCollider.enabled = !player.GetComponent<PlayerController>().isSliding;
    }
}
