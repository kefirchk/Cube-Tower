using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class Ads : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameId = "5535767";
    [SerializeField] private string iOSGameId = "5535766";
    [SerializeField] private bool testMode = true;
    private string gameId;

    private Coroutine showAd;
    private bool needToStop;
    private static int countLoses;

    private void Start()
    {
#if UNITY_IOS
            gameId = iOSGameId;
#elif UNITY_ANDROID
        gameId = androidGameId;
#elif UNITY_EDITOR
            gameId = androidGameId; //Only for testing the functionality in the Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }

        ++countLoses;
        
    }

    private void Update()
    {
        if (needToStop) 
        {
            needToStop = false;
            StopCoroutine(showAd);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        if(countLoses % 3 == 0)
            showAd = StartCoroutine(ShowAd());
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    private IEnumerator ShowAd()
    {
        while(true)
        {
            if (Advertisement.Banner.isLoaded)
            {
                Advertisement.Banner.Show(gameId);
                needToStop = true;
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
