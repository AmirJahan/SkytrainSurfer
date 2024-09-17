// Xav Laugo ©, 2024. All rights reserved.

using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

public class Authentication : MonoBehaviour
{
    public static Authentication Instance;
    
    //[SerializeField] private TextMeshProUGUI playerName;
    
    private async void Awake()
    {
        Instance = this;
        await InitUGS();
        SceneManager.LoadScene("Main Menu");
    }

    private async Task InitUGS()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Initialized");
            await SignInAnon();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public async Task SignInAnon()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign In successful");
            await GetAnonPlayerName();
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e.Message);
        }
        catch (RequestFailedException ex)
        {
             Debug.Log(ex.Message);
        }
    }

    public async Task GetAnonPlayerName()
    {
        try
        {
            await AuthenticationService.Instance.GetPlayerNameAsync();
            //playerName.text = AuthenticationService.Instance.PlayerName;
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e.Message);
        }
    }
}
