using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager raycastManager;
    List<ARRaycastHit> aRRaycastHits = new List<ARRaycastHit>();

    Camera cam;
    
    public List<PlacedEnemy> PlacedEnemyList = new List<PlacedEnemy>();

    private Label _text;
    private Label _text2;

    private Button _buttonDelete;
    private Button _buttonClear;
    private Button _buttonCreation;
    private Button _buttonCombatStart;

    private VisualElement _visualElementWorld;
    private VisualElement _visualElementCreation;

    public UIDocument uiDocument;
    public GameObject combatManager;
    private CombatManager _combatManager;

    GameObject spawnableObj;
    GameObject lastSelectedSpawnableObj;
    [SerializeField]
    GameObject spawnablePrefab;

    double _lowerUIHeight;

    bool _isCreationMode;
    bool _isCombat;

    private Material _defaultMat;
    private Material _selectedMat;

    private Vector2 _centerVector2;
    RaycastHit hit;

    /// <summary> small checklist
    /// create new on clicking in world:            O
    /// able to move selected object:               O
    /// add clear button:                           O
    /// able to click on previous created object:   O
    /// add delete button on seleceted one:         O
    /// add item to list of objects:                O
    /// detect UI and not let it spawn objects:     O
    /// creation and play mode:                     O
    /// combat:                                     O
    /// characters:                                 O
    /// able to display which one is selected:      O
    /// </summary>
    /// 

    // Start is called before the first frame update
    void Start()
    {
        spawnableObj = null;
        cam = GameObject.Find("AR Camera").GetComponent<Camera>();
        _text = uiDocument.rootVisualElement.Q("ui_lbl_01") as Label;
        _text2 = uiDocument.rootVisualElement.Q("ui_lbl_02") as Label;
        _buttonDelete = uiDocument.rootVisualElement.Q("ui_btn_01") as Button;
        _buttonClear = uiDocument.rootVisualElement.Q("ui_btn_02") as Button;
        _buttonCreation = uiDocument.rootVisualElement.Q("ui_btn_03") as Button;
        _buttonCombatStart = uiDocument.rootVisualElement.Q("ui_btn_04") as Button;

        _buttonCombatStart.SetEnabled(false);

        _combatManager = combatManager.GetComponent<CombatManager>();

        _buttonCreation.text = "Play";

        _visualElementCreation = uiDocument.rootVisualElement.Q("ve_bottom") as VisualElement;
        _visualElementWorld = uiDocument.rootVisualElement.Q("ve_world") as VisualElement;

        _buttonDelete.clicked += _buttonDelete_clicked;
        _buttonClear.clicked += _buttonClear_clicked;
        _buttonCreation.clicked += _creationButton_clicked;
        _buttonCombatStart.clicked += _buttonCombatStart_clicked;

        updateCreationButtons();

        _lowerUIHeight = (Screen.height * 0.1);

        _isCreationMode = true;
        _isCombat = false;

        var rend = spawnablePrefab.GetComponent<Renderer>();
        _defaultMat = new Material(rend.material);
        _selectedMat = new Material(rend.material);
        _selectedMat.SetColor("_Color", Color.yellow);

        CombatManager.OnEnd += _combatManager_OnEnd;

        _centerVector2 = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCombat) return;
        if(!_isCreationMode) combatRayCaster();

        if (Input.touchCount == 0) return;

        if (Input.GetTouch(0).position.y < _lowerUIHeight) return;

        if (raycastManager.Raycast(Input.GetTouch(0).position, aRRaycastHits))
        {
            touchRaycaster();
        }

    }

    #region Raycastings

    private void touchRaycaster()
    {
        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
        _text2.text = string.Format("x: {0} | y: {1}", Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
        _text.text = "";
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Spawnable")
                {
                    _text.text = "selected object";
                    spawnableObj = hit.collider.gameObject;
                }
                else
                {
                    _text.text = "created object";
                    SpawnPrefab(aRRaycastHits[0].pose.position);
                    AddNewObject();
                }
            }
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnableObj != null)
        {
            updateSelectedColor(spawnableObj);
            _text.text = "moving object";
            spawnableObj.transform.position = aRRaycastHits[0].pose.position;
            UpdateObjectPosition();
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            updateSelectedColor(spawnableObj, false);
            lastSelectedSpawnableObj = spawnableObj;
            spawnableObj = null;
        }

        updateCreationButtons();
    }

    private void combatRayCaster()
    {
        if (raycastManager.Raycast(_centerVector2, aRRaycastHits))
        {
            _text.text = "battle mode";
            Ray ray = cam.ScreenPointToRay(_centerVector2);
            if (Physics.Raycast(ray, out hit))
            {
                _text.text += " casted";
                if (hit.collider.gameObject.tag == "Spawnable")
                {
                    _buttonCombatStart.SetEnabled(true);
                    _text2.text = "hit one";
                    lastSelectedSpawnableObj = hit.collider.gameObject;
                    updateSelectedColor(lastSelectedSpawnableObj);
                }
                else
                {
                    _buttonCombatStart.SetEnabled(false);
                    updateSelectedColor(lastSelectedSpawnableObj, false);
                    _text2.text = "no hit";
                }
            }
        }
    }

    #endregion Raycastings



    #region World Object Positioning

    private void AddNewObject()
    {
        PlacedEnemyList.Add(new PlacedEnemy(spawnablePrefab, aRRaycastHits[0].pose.position));
    }

    void UpdateObjectPosition()
    {
        PlacedEnemyList.FirstOrDefault(x => x.Obj == spawnableObj).Vector3 = aRRaycastHits[0].pose.position;
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnableObj = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
    }

    #endregion World Object Positioning



    #region Visual updates

    private void updateCreationButtons()
    {
        _buttonDelete.SetEnabled(lastSelectedSpawnableObj != null ? true : false);
        _buttonClear.SetEnabled(PlacedEnemyList.Any());
    }

    private void updateSelectedColor(GameObject selected, bool newColor = true)
    {
        var renderer = selected.GetComponent<Renderer>();
        renderer.material = newColor ? _selectedMat : _defaultMat;
    }

    #endregion Visual updates



    #region Button Events

    private void _combatManager_OnEnd()
    {
        deleteSelected();
        _isCombat = false;
    }

    private void _buttonCombatStart_clicked()
    {
        combatStart();
    }

    private void _creationButton_clicked()
    {
        creationMode();
    }

    private void _buttonDelete_clicked()
    {
        deleteSelected();
    }

    private void _buttonClear_clicked()
    {
        clearObjects();
    }

    private void combatStart()
    {
        Array names = Enum.GetValues(typeof(EnemyNames));
        var rnd = new System.Random();

        _combatManager.InnitiateBattle(new Enemy(50, 1, (EnemyNames)names.GetValue(rnd.Next(names.Length - 1))));
        _isCombat = true;
    }

    private void creationMode()
    {
        _isCreationMode = !_isCreationMode;

        _buttonClear.visible = _isCreationMode;
        _buttonDelete.visible = _isCreationMode;

        _buttonCombatStart.visible = !_isCreationMode;

        _buttonCreation.text = _isCreationMode ? "Play" : "Create";
    }

    private void deleteSelected()
    {
        PlacedEnemy placedObject = PlacedEnemyList.FirstOrDefault(x => x.Obj == lastSelectedSpawnableObj);

        PlacedEnemyList.Remove(placedObject);
        Destroy(lastSelectedSpawnableObj);
        _text.text = "deleted object";
        updateCreationButtons();
    }

    private void clearObjects()
    {
        GameObject[] destroyList = GameObject.FindGameObjectsWithTag("Spawnable");
        foreach (GameObject go in destroyList)
        {
            Destroy(go);
        }
        _text2.text = "";
        PlacedEnemyList.Clear();
        updateCreationButtons();
    }

    #endregion Button Events

}
