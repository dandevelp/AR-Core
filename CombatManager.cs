using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CombatManager : MonoBehaviour
{

    public delegate void BattleStatus();
    public static event BattleStatus OnEnd;

    private float _player_hp;

    private bool _isPlayerTurn;
    private bool _battleMode;

    private Enemy _currentEnemy;

    private Label _text;
    private Label _text2;

    private VisualElement _visualElementBattle;
    private VisualElement _visualElementEnemy;
    private VisualElement _visualElementWorld;

    private VisualElement _enemySpriteElement;

    private Button _buttonAttack;

    public UIDocument uiDocument;

    public GameObject EnemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _player_hp = 100;
        
        _isPlayerTurn = true;
        _battleMode = false;

        _text = uiDocument.rootVisualElement.Q("ui_lbl_01") as Label;
        _text2 = uiDocument.rootVisualElement.Q("ui_lbl_02") as Label;

        _visualElementBattle = uiDocument.rootVisualElement.Q("ve_battle") as VisualElement;
        _visualElementEnemy = uiDocument.rootVisualElement.Q("ve_enemy") as VisualElement;
        _visualElementWorld = uiDocument.rootVisualElement.Q("ve_bottom") as VisualElement;

        _enemySpriteElement = uiDocument.rootVisualElement.Q("enemy_sprite") as VisualElement;

        _buttonAttack = uiDocument.rootVisualElement.Q("ui_btn_attack") as Button;
        _buttonAttack.clicked += _buttonAttack_onClick;

        updateUI();
    }

    public void InnitiateBattle(Enemy enemy)
    {
        _battleMode = true;
        this._currentEnemy = enemy;
        updateUI();
        setEnemySprite();
    }

    private void setEnemySprite()
    {
        //EnemyPrefab.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Bat");

        Instantiate(EnemyPrefab);
        //UnityEngine.UI.Image g = 
        GameObject.Find("EnemySprite").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>(string.Format("Sprites/{0}", _currentEnemy.Img));
        //g.sprite = Resources.Load<Sprite>("Sprites/Bat");

        //.sprite = Resources.Load<Sprite>("Sprites/Bat");


        /*.GetComponent<Camera>();*/

        //_enemySpriteElement.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("Sprites/Bat"));

        //var sprite = Resources.Load<Sprite>("Sprites/Bat");
    }

    private void destroyEnemyComponent()
    {
        GameObject[] destroyList = GameObject.FindGameObjectsWithTag("SpawnedEnemySprite");
        foreach (GameObject go in destroyList)
        {
            Destroy(go);
        }
        //Destroy(EnemyPrefab);
    }

    private void _buttonAttack_onClick()
    {
        playerAttack();
    }

    private void playerAttack()
    {
        _currentEnemy.HP -= 5;
        updateTurn();
    }

    private void updateTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        updateUI();
    }

    private void enemyAttack()
    {
        _player_hp -= _currentEnemy.AP;
        updateTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_battleMode) return;

        if (_player_hp <= 0 || _currentEnemy.HP <= 0) endBattle();

        new WaitForSecondsRealtime(2);

        if (!_isPlayerTurn) enemyAttack();
    }

    private void endBattle()
    {
        destroyEnemyComponent();
        _isPlayerTurn = true;
        _battleMode = false;
        updateUI();
        OnEnd();
    }

    private void updateUI()
    {
        _buttonAttack.focusable = _isPlayerTurn;
        _visualElementBattle.visible = _battleMode ? true : false;
        _visualElementEnemy.visible = _battleMode ? true : false;
        _visualElementWorld.visible = _battleMode ? false : true;

        if (_battleMode)
        {
            _text.text = string.Format("Player HP: {0}", _player_hp);
            _text2.text = string.Format("Enemy HP: {0}", _currentEnemy.HP);
        }
    }
}
