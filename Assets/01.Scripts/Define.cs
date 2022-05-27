using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const float GRID_WORLD_WIDTH = 1f;           //한 그리드의 월드좌표 기준 길이값
    public const int MIN_MATCH_COUNT = 3;               //매치의 최소 블록 갯수 조건
    public static readonly GridPosition BOARD_SIZE = new GridPosition(8, 7); //보드 사이즈

    
    public const int GEM_SCORE = 50;                        //젬 하나당 점수
    public const float TIME_LIMIT_SEC = 45f;                //게임 플레이 시간 (초)
    public const float START_TICKING_SEC = 10f;             //시계소리 시작하는 시간
    public const float FALL_DURATION = 0.05f;                //보드 오브젝트 1 Row당 떨어지는 시간 (초)
    
    public const float COMBO_INTERVAL = 1.55f;              //콤보 간격 (초)
    public const int FEVER_COMBO = 8;                       //피버모드에 필요한 콤보 수
    public const float FEVER_TIME = 8f;                     //피버모드 시간 (초)    
    public const int FEVER_BONUS_NEED_GEM = 10;             //피버 보너스에 필요한 젬 갯수
    public const float FEVER_BONUS_CHANCE = 0.3f;           //피버 보너스 확률

    public const float GEM_OBJECT_CHANCE = 0.35f;            //젬오브젝트 생성 확률
    public const float GEM_OBJECT_LIFETIME = 3f;            //젬오브젝트 생존 시간 (초)
    public const int GEM_OBJECT_WAIT_TURN = 3;            //젬오브젝트 생존 쿨타임 턴

    public const int MAX_POINTGAUGE = 100;                  //포인트 게이지 최대치
    public const int POINTGAUGE_LIMIT = 15;                 //한 번에 오르는 포인트 게이지의 최대치

    // 프리팹 경로
    public const string DATA_SETTING_DIR_PATH = "Data/Settings/";
}
