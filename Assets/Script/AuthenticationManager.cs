using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using System.Threading.Tasks;

public class AuthenticationManager : MonoBehaviour
{
    public TMP_Text statusText;

    private async void Start()
    {
        await InitializeAuthentication();
    }

    private async Task InitializeAuthentication()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            statusText.text = $"Signed in! Player ID: {AuthenticationService.Instance.PlayerId}";
        };

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await SignInAnonymously();
        }
    }

    public async Task SignInAnonymously()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            statusText.text = $"Signed in anonymously! Player ID: {AuthenticationService.Instance.PlayerId}";
        }
        catch (AuthenticationException e)
        {
            statusText.text = $"Error: {e.Message}";
        }
    }
}
