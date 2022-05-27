using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

using BackEnd;
using UnityEngine;

public class NetworkManager : SimpleSingleton<NetworkManager>
{
    NetworkManager() { }

    public void Init()
    {
        LoadingUI.Instance.SetLoading(true);

        Backend.Initialize(() =>
        {
            if (Backend.IsInitialized)
            {
                //서버 접속 성공                
                LoadingUI.Instance.SetLoading(false);
            }
            else
            {
                //서버 접속 실패
            }
        });

        //Activate GPGS
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        //GPGS Login
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("GPGS 로그인 성공");
                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetGPGSToken(), FederationType.Google, "gpgs");
                
                if(BRO.IsSuccess())
                {
                    Debug.Log("뒤끝 로그인 성공");
                }
                else
                {
                    Debug.Log("뒤끝 로그인 실패");
                }
                
            }
            else
            {
                Debug.Log("GPGS 로그인 실패");
            }
        });
    }

    private string GetGPGSToken()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }
}
