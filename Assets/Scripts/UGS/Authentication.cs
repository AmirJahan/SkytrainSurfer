using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class Authentication : MonoBehaviour
{
    private async void Awake()
    {
        await InitUGS();
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
    private async Task SignInAnon()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign In successful");
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
}