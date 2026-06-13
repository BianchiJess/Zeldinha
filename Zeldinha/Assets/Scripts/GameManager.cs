using UnityEngine;

//MAQUINA DE ESTADOS
public enum enemyState
{
   IDLE,
   EXPLORE,
   ALERT,
   PATROL,
   FURY
}

public class GameManager : MonoBehaviour
{

    [Header("Slime IA")]
    public Transform[] slimeWayPoints;//Posições de Patrulha do slime
    public Transform[] turtleWayPoints;//Posições de Patrulha da tartaruga
    //=========Attack
    public Transform player;//Referencia do Player
    public float slimeDistanceAttack = 2f;//Distancia para o Slime atacar
    //=========Alerta
    public float slimeAlertTime = 3f;
    //=========Fury
    public float slimeFuryTime = 5f;
    public float slimeLosePlayerTime = 3f;
    //=========Explore
    public float slimePatrolWaitTime;//Tempo de espera no Patrol
    public float slimeIdelWaitTime;//Tempo de espera no Idle


    public bool isGameOver = false;

}
