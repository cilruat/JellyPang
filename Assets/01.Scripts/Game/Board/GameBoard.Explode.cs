using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public partial class GameBoard
{
    IEnumerator TouchGridProc(BoardGrid touchGrid)
    {
        if (touchGrid.IsEmpty)
            yield break;

        if (!touchGrid.BoardObject.IsExplodable)
            yield break;

        bool isFeverBonus = false;

        if (touchGrid.BoardObject.IsSpecialObject)
        {
            _inputCtrl.IsLock = true;

            //특수 오브젝트의 경우
            touchGrid.BoardObject.Explode(this);
        }
        else
        {
            //일반 오브젝트의 경우
            List<GridPosition> matchList = new List<GridPosition>();
            FindConnectedGems(touchGrid, matchList);

            if (matchList.Count < Define.MIN_MATCH_COUNT)
            {
                ResetCombo();
                yield break;
            }

            //pointgauge
            _curPointGauge += Mathf.Min(matchList.Count, Define.POINTGAUGE_LIMIT);
            while (true)
            {
                if (_curPointGauge >= Define.MAX_POINTGAUGE)
                {
                    _curPointGauge -= Define.MAX_POINTGAUGE;

                    _curMakePointGaugeBonus++;
                }
                else
                {
                    GameUIManager.Instance.SetPointGauge(_curPointGauge, Define.MAX_POINTGAUGE);
                    break;
                }
            }

            //remove
            _inputCtrl.IsLock = true;

            for (int idx = 0; idx < matchList.Count; ++idx)
            {
                BoardGrid grid = GetGrid(matchList[idx]);

                grid.BoardObject.Explode(this);
            }

            AddCombo();

            //fever
            if (_isFever)
            {
                //진동
                MMVibrationManager.Haptic(HapticTypes.LightImpact, false, true, this);
                transform.DOShakePosition(0.2f, 0.25f, 30);

                //효과음
                SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.FEVER);
            }

            isFeverBonus = _isFever
                            && (matchList.Count >= Define.FEVER_BONUS_NEED_GEM)
                            && (Random.Range(0, 1f) <= Define.FEVER_BONUS_CHANCE); 
        }

        yield return new WaitUntil(()=>CurrentExplodingCnt == 0);

        yield return StartCoroutine(MakeGemsFall(isFeverBonus));

        yield return StartCoroutine(MakePointGaugeBonus());

        if (AfterExplodeFunc != null)
        {
            yield return StartCoroutine(AfterExplodeFunc);
            AfterExplodeFunc = null;
        }

        //젬오브젝트 계산 및 생성
        MakeGemObject();

        _inputCtrl.IsLock = false;
    }

    private void FindConnectedGems(BoardGrid touchGrid, List<GridPosition> list)
    {
        FindFiils(touchGrid.Position, touchGrid.BoardObject.Color, list);

        if (list.Count < Define.MIN_MATCH_COUNT)
            return;

        if(_isFever)
        {
            list.AddRange(FindConnectedFever(list));
        }
    }

    IEnumerator MakeGemsFall(bool isFeverBonus)
    {
        //내리기
        for (int row = Define.BOARD_SIZE.row - 2; row >= 0; --row)
        {
            for(int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                BoardGrid grid = GetGrid(row, col);
                if (grid == null || grid.IsEmpty)
                    continue;

                int emptyGridCnt = GetEmptyGridBelow(grid.Position);
                if (emptyGridCnt == 0)
                    continue;

                BoardObjectBase boardObj = grid.BoardObject;

                BoardGrid toGrid = GetGrid(grid.Position.row + emptyGridCnt, grid.Position.col);
                FallBoardObject(grid, toGrid);

                float fallDuration = Define.FALL_DURATION;
                boardObj.transform.DOLocalMove(GridPosToWorldPos(toGrid.Position), fallDuration).SetEase(Ease.Linear);
            }
        }

        //채우기
        BoardObjectBase.COLOR bonusColor = Util.RandomEnumValue<BoardObjectBase.COLOR>(true, true);
        for (int col = 0; col < Define.BOARD_SIZE.col; ++col)
        {
            BoardGrid grid = GetGrid(0, col);
            if (!grid.IsEmpty)
                continue;

            int emptyGridCnt = GetEmptyGridBelow(grid.Position) + 1;            
            for (int cnt = 0; cnt < emptyGridCnt; ++cnt)
            {
                Vector3 spawnPos = GridPosToWorldPos(new GridPosition(cnt - (Define.BOARD_SIZE.row - 2), col));

                BoardGrid toGrid = GetGrid(cnt, col);

                BoardObject_Gem gem = _boardObjPool.Get<BoardObject_Gem>();                
                gem.transform.position = spawnPos;
                
                BoardObjectBase.COLOR gemColor = BoardObjectBase.COLOR.RED;
                if (isFeverBonus)
                    gemColor = bonusColor;
                else
                    gemColor = Util.RandomEnumValue<BoardObjectBase.COLOR>(true, true);
                
                gem.Set(gemColor);

                toGrid.SetBoardObj(gem);

                float fallDuration = Define.FALL_DURATION;
                gem.transform.DOLocalMove(GridPosToWorldPos(toGrid.Position), fallDuration).SetEase(Ease.Linear);
            }
        }

        //조합 유무 찾기
        bool isExplodable = false;
        for(int row = 0; row < Define.BOARD_SIZE.row; ++row)
        {
            for(int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                BoardGrid grid = GetGrid(row, col);

                List<GridPosition> fillList = new List<GridPosition>();
                FindFiils(grid.Position, grid.BoardObject.Color, fillList);

                if (fillList.Count >= Define.MIN_MATCH_COUNT)
                {
                    isExplodable = true;
                    break;
                }                    
            }

            if (isExplodable)
                break;
        }

        if(!isExplodable)
        {
            List<GridPosition> visitedPos = new List<GridPosition>();

            while(true)
            {
                BoardGrid grid = GetGrid(Random.Range(0, Define.BOARD_SIZE.row), Random.Range(0, Define.BOARD_SIZE.col));
                if (visitedPos.Contains(grid.Position))
                    continue;

                visitedPos.Add(grid.Position);

                List<GridPosition> fillList = new List<GridPosition>();
                FindFiils(grid.Position, grid.BoardObject.Color, fillList);

                if (fillList.Count > 1)
                {
                    List<BoardGrid> aroundList = GetAroundDiffrentColorGrid(grid);

                    if (aroundList.Count == 0)
                        continue;

                    BoardGrid targetGrid = Util.RandomListValue(aroundList);
                    if (targetGrid.BoardObject.Type != BoardObjectBase.TYPE.GEM)
                        continue;

                    BoardObject_Gem gem = targetGrid.BoardObject as BoardObject_Gem;
                    gem.ChangeColor(grid.BoardObject.Color);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(Define.FALL_DURATION);

        //효과음
        SoundCtrl.Instance.PlaySound(SoundCtrl.SFX.DROP_GEM);
    }

    IEnumerator MakePointGaugeBonus()
    {
        List<GridPosition> visitedPos = new List<GridPosition>();
        for (int idx = 0; idx < _curMakePointGaugeBonus; ++idx)
        {
            BoardGrid grid = null;
            while (true)
            {
                grid = GetGrid(Random.Range(0, Define.BOARD_SIZE.row), Random.Range(0, Define.BOARD_SIZE.col));
                if (visitedPos.Contains(grid.Position))
                    continue;
                else
                {
                    visitedPos.Add(grid.Position);
                    break;
                }                    
            }

            //생성
            _boardObjPool.Return(grid.BoardObject);
            grid.ReleaseBoardObj();

            BoardObject_Diamond diamond = _boardObjPool.Get<BoardObject_Diamond>();
            diamond.transform.position = grid.transform.position;
            diamond.Set(BoardObjectBase.COLOR.NONE);
            grid.SetBoardObj(diamond);
        }

        yield return null;

        _curMakePointGaugeBonus = 0;
    }
    
    public void AddScore()
    {
        _curScore += Define.GEM_SCORE;
        GameUIManager.Instance.SetScore(_curScore);
    }

    private void AddCombo()
    {
        if (_isFever)
            return;

        ++_curComboCnt;        
        _comboTimeLeft = Define.COMBO_INTERVAL;

        //sound
        SoundCtrl.Instance.PlayComboSFX(_curComboCnt);

        if (_curComboCnt >= Define.FEVER_COMBO)
        {
            StartCoroutine(FeverModeProc());
        }
    }

    IEnumerator FeverModeProc()
    {
        ResetCombo();
        _isFever = true;

        //상단 이펙트
        BoardFx topFx = GetBoardFx(BoardFx.TYPE.FEVER_FIRE_TOP, false);
        topFx.transform.position = new Vector3(0f, _camera.orthographicSize);
        topFx.Play(Define.FEVER_TIME);
        
        //하단 이펙트
        BoardFx botFx = GetBoardFx(BoardFx.TYPE.FEVER_FIRE_BOT, false);
        botFx.transform.position = new Vector3(0f, -_camera.orthographicSize);
        botFx.Play(Define.FEVER_TIME);

        _backGroundImg.DOColor(new Color(0.4f, 0.4f, 0.4f), 1f);

        yield return new WaitForSeconds(Define.FEVER_TIME);

        _backGroundImg.DOColor(Color.white, 1f);

        _isFever = false;
    }

    private void ResetCombo()
    {
        _curComboCnt = 0;
        _comboTimeLeft = 0f;
    }

    public void ReturnBoardObject(BoardObjectBase boardObj)
    {
        _boardObjPool.Return(boardObj);        
    }

    public void MakeGemObject()
    {
        if (_curGemObject != null)
            return;

        if(_gemObjectWaitTurn > 0)
        {
            --_gemObjectWaitTurn;
            return;
        }

        if (Random.Range(0f, 1f) > Define.GEM_OBJECT_CHANCE)
            return;

        //조합 찾기
        List<List<GridPosition>> matchList = new List<List<GridPosition>>();
        List<GridPosition> visitedPos = new List<GridPosition>();
        for (int row = 0; row < Define.BOARD_SIZE.row; ++row)
        {
            for (int col = 0; col < Define.BOARD_SIZE.col; ++col)
            {
                BoardGrid grid = GetGrid(row, col);
                
                if (visitedPos.Contains(grid.Position))
                    continue;

                List<GridPosition> fillList = new List<GridPosition>();
                FindFiils(grid.Position, grid.BoardObject.Color, fillList);
                                
                if (fillList.Count >= Define.MIN_MATCH_COUNT)
                {
                    matchList.Add(fillList);

                    visitedPos.AddRange(fillList);
                }
            }
        }

        if(matchList.Count < 1)
        {
            Debug.LogError("GameBoard - MakeGemObject - Not Found Match");
            return;
        }

        List<GridPosition> match = Util.RandomListValue(matchList);
        BoardGrid targetGrid = GetGrid(Util.RandomListValue(match));
        BoardObject_Gem gem = targetGrid.BoardObject as BoardObject_Gem;

        //생성
        GemObjectBase gemObject = _gemObjPool.Get(Util.RandomListValue(_useGemObjList));
        
        _curGemObject = gemObject;
        _gemObjectTimeLeft = Define.GEM_OBJECT_LIFETIME;

        gem.SetGemObj(_curGemObject);

        //대기턴 생성
        _gemObjectWaitTurn = Define.GEM_OBJECT_WAIT_TURN;
    }

    public void ReturnGemObject()
    {
        _curGemObject.transform.SetParent(transform);
        
        _gemObjPool.Return(_curGemObject);
        
        _curGemObject = null;
        _gemObjectTimeLeft = 0f;
    }
}
