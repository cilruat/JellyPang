using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject _typoReady;
    [SerializeField]
    private GameObject _typoGo;
    [SerializeField]
    private GameObject _typoTimeOver;
    [SerializeField]
    private GameObject _typoJellyPang;

    private InputCtrl _inputCtrl;
    private BoardObjectPool _boardObjPool = null;
    private GemObjectPool _gemObjPool = null;
    private BoardFxPool _boardFxPool = null;

    private BoardGrid[,] _boardGrids;
    private SpriteRenderer _backGroundImg;
    private Camera _camera;

    private int _curScore = 0;
    private float _curTimeLeft = 0f;
    private int _curComboCnt = 0;
    private float _comboTimeLeft = 0f;
    private bool _isFever = false;
    private bool _isTicking = false;

    private List<GemObjectBase.TYPE> _useGemObjList = null;
    private GemObjectBase _curGemObject = null;
    private float _gemObjectTimeLeft = 0f;
    private int _gemObjectWaitTurn = 0;

    private int _curPointGauge = 0;
    private int _curMakePointGaugeBonus = 0;
    
    public bool IsFever => _isFever;
    public int CurrentExplodingCnt { get; set; }
    public IEnumerator AfterExplodeFunc { get; set; }

    new Transform transform;

    void Start()
    {
        transform = gameObject.transform;
        _camera = Camera.main;
        _backGroundImg = gameObject.GetComponentInChildren<SpriteRenderer>();

        //입력
        _inputCtrl = gameObject.AddComponent<InputCtrl>();
        _inputCtrl.Init(transform.position, Vector3.back, OnTouchBoard, null);
        _inputCtrl.IsLock = true;

        //보드오브젝트 풀
        _boardObjPool = gameObject.AddComponent<BoardObjectPool>();
        _boardObjPool.Init();

        //젬오브젝트 풀
        _gemObjPool = gameObject.AddComponent<GemObjectPool>();
        _gemObjPool.Init();

        //젬 오브젝트 테스트
        _useGemObjList = new List<GemObjectBase.TYPE>();
        _useGemObjList.Add(GemObjectBase.TYPE.ADD_TIME);
        _useGemObjList.Add(GemObjectBase.TYPE.COLOR_BOMB);
        _useGemObjList.Add(GemObjectBase.TYPE.ENERGY_BOMB);

        //보드이펙트 풀
        _boardFxPool = gameObject.AddComponent<BoardFxPool>();
        _boardFxPool.Init();

        //카메라 설정
        _camera.orthographicSize = (((Define.BOARD_SIZE.col * Define.GRID_WORLD_WIDTH) + 0.2f) * 0.5f) / _camera.aspect;

        StartCoroutine(StartGameProc());
    }

    public void Update()
    {
        if(!_inputCtrl.IsLock)
        {
            _curTimeLeft -= Time.deltaTime;
            GameUIManager.Instance.SetTimerValue(_curTimeLeft, Define.TIME_LIMIT_SEC);

            if(!_isTicking && _curTimeLeft <= Define.START_TICKING_SEC)
            {
                _isTicking = true;
                SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_TICKING);
            }

            if (_curTimeLeft <= 0f)
                StartCoroutine(GameOverProc());

            if (!_isFever && _comboTimeLeft > 0f)
            {
                _comboTimeLeft -= Time.deltaTime;
                if (_comboTimeLeft <= 0f)
                    ResetCombo();
            }

            if(_curGemObject != null && _gemObjectTimeLeft > 0f)
            {
                _gemObjectTimeLeft -= Time.deltaTime;
                if(_gemObjectTimeLeft <= 0f)
                {
                    _curGemObject.Owner.ReleaseGemObj();
                    ReturnGemObject();
                }
            }
        }
    }

    public IEnumerator StartGameProc()
    {
        //그리드 생성
        _boardGrids = new BoardGrid[Define.BOARD_SIZE.row, Define.BOARD_SIZE.col];

        for (int row = 0; row < Define.BOARD_SIZE.row; ++row)
        {
            for (int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                GridPosition pos;
                pos.row = row;
                pos.col = col;

                BoardGrid grid = Instantiate<BoardGrid>(GameDataSetting.DefaultSetting.GridPrefab);
                grid.Init(pos);
                grid.gameObject.name = Util.StringAppend(pos.row.ToString(), "/", pos.col.ToString());
                grid.transform.SetParent(transform);
                grid.transform.localPosition = GridPosToWorldPos(pos);

                _boardGrids[row, col] = grid;
            }
        }

        //젬으로 채우기    
        for (int row = Define.BOARD_SIZE.row - 1; row >= 0; --row)
        {
            for (int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                GridPosition pos = new GridPosition(row, col);
                GridPosition spawnPos = new GridPosition(-4, col);

                BoardObject_Gem gem = _boardObjPool.Get<BoardObject_Gem>();
                gem.Set(Util.RandomEnumValue<BoardObjectBase.COLOR>(true, true));
                gem.transform.localPosition = GridPosToWorldPos(spawnPos);

                _boardGrids[row, col].SetBoardObj(gem);

                gem.transform.DOLocalMove(GridPosToWorldPos(pos), Define.FALL_DURATION * row);

                yield return new WaitForSeconds(0.02f);
            }
        }

        _typoReady.SetActive(true);

        yield return new WaitForSeconds(0.75f);

        _typoReady.SetActive(false);
        _typoGo.SetActive(true);
        _typoGo.transform.DOScale(new Vector3(2f, 2f, 2f), 0.35f);

        yield return new WaitForSeconds(0.5f);

        _typoGo.transform.DOMoveY(-15f, 1f);

        _curTimeLeft = Define.TIME_LIMIT_SEC;
        _inputCtrl.IsLock = false;
    }

    public IEnumerator GameOverProc()
    {
        _inputCtrl.IsLock = true;

        //째각소리 종료
        SoundCtrl.Instance.StopSound(SoundCtrl.SFX.SFX_TICKING);

        //젬오브젝트 삭제
        if (_curGemObject != null)
        {
            _curGemObject.Owner.ReleaseGemObj();
            ReturnGemObject();
        }

        //타임오버 효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_TIMEOVER);

        _typoTimeOver.SetActive(true);
        yield return new WaitForSeconds(2f);

        _typoTimeOver.SetActive(false);

        List<BoardObjectBase> specialObjList = new List<BoardObjectBase>();
        for(int row = 0; row < Define.BOARD_SIZE.row; ++row)
        {
            for(int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                BoardGrid grid = GetGrid(row, col);

                if (!grid.BoardObject.IsSpecialObject || !grid.BoardObject.IsExplodable)
                    continue;

                specialObjList.Add(grid.BoardObject);
            }
        }

        if(specialObjList.Count > 0)
        {
            //젤리팡 효과음
            SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.SFX_JELLYPANG);

            _typoJellyPang.SetActive(true);

            yield return new WaitForSeconds(2f);

            _typoJellyPang.SetActive(false);
            _typoJellyPang.transform.DOMoveY(-15f, 1f);

            yield return new WaitForSeconds(0.5f);

            foreach (BoardObjectBase obj in specialObjList)
            {
                if (obj.IsExploding || !obj.isActiveAndEnabled)
                    continue;

                obj.Explode(this);
                yield return new WaitForSeconds(0.35f);
            }

            yield return new WaitUntil(() => CurrentExplodingCnt == 0);

            yield return new WaitForSeconds(0.5f);
        }

        //점수 체크 후 결과창 열기 테스트용
        int bestScore = PlayerPrefs.GetInt("RecordScore", 0);
        if(_curScore > bestScore)
        {
            bestScore = _curScore;
            PlayerPrefs.SetInt("RecordScore", bestScore);
        }
        GameUIManager.Instance.OpenGameResult(_curScore, bestScore);

        yield return new WaitForSeconds(0.1f);
    }

    public void AddTime()
    {
        _curTimeLeft += 1.5f;
        if (_curTimeLeft > Define.TIME_LIMIT_SEC)
            _curTimeLeft = Define.TIME_LIMIT_SEC;

        GameUIManager.Instance.SetTimerValue(_curTimeLeft, Define.TIME_LIMIT_SEC);
    }
}
