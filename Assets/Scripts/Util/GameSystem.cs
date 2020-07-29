using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSystem : StateMachine {

    public const float TIME_TO_HOOK = 1.4f;
    public const float TIME_TO_PULL = 0.4f;

    [SerializeField] public GameObject WindParticlesGo;
    [SerializeField] private GameObject playerPrefab;

    private Player player;

    public Player Player => player;

    public Vector3 InitialPlayerPos;

    private void Start()
    {
        player = playerPrefab.GetComponent<Player>();
        InitialPlayerPos = player.GetPosition();

        SetState(new IdleState(this));
    }
}

