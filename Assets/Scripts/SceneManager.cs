using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;

    [SerializeField] private Text _wavesInfoText;
    [SerializeField] private Image _superAttackButton;
    public bool CanSuperAttack { get; private set; } = true;
    private float _timeSinceLastSuperAttack;

    private int currWave = 0;
    [SerializeField] private LevelConfig Config;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
    }

    private void Update()
    {
        if (CanSuperAttack)
        {
            return;
        }
        _timeSinceLastSuperAttack += Time.deltaTime;
        _superAttackButton.fillAmount = _timeSinceLastSuperAttack / Player.SuperAttackCooldown;
        if (_timeSinceLastSuperAttack >= Player.SuperAttackCooldown)
        {
            CanSuperAttack = true;
        }
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if(Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    public void InitiateSuperAttackCooldown()
    {
        _timeSinceLastSuperAttack = 0;
        CanSuperAttack = false;
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    public void AddHealthToPlayer(float healthAmount)
    {
        Player.Hp += healthAmount;
        Debug.Log(Player.Hp);
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length)
        {
            Win.SetActive(true);
            return;
        }
        _wavesInfoText.text = string.Format("Wave : {0} / {1}", currWave + 1, Config.Waves.Length);

        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;

    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    

}
