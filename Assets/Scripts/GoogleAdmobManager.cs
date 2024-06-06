using UnityEngine;
using GoogleMobileAds.Api;

public class GoogleAdmobManager : MonoBehaviour 
{
    // Remember to uncheck test when making a release build
    public bool test = true;

    public bool disable = false;

    public AdPosition bannerADPosition = AdPosition.Top;

    // test ad
    string m_testBannerID = "";

    // test
    string m_testFullScrennAdID = "";

    // release ad
    string m_bannerID = "";
   
    // release
    string m_fullScrennAdID = "";

    InterstitialAd m_fullScreenAd;
    
    BannerView m_bannerView;

    private void Start()
    {
        if (!Application.isPlaying||disable)
            return;
        
        if (test)
        {
            m_bannerID = m_testBannerID;
            m_fullScrennAdID = m_testFullScrennAdID;
        }

        RequestFullScreenAd();        
    }

    internal void RequestBannerAd()
    {
        if (disable)
            return;

        m_bannerView = new BannerView(m_bannerID, AdSize.Banner, bannerADPosition);

        AdRequest request = new AdRequest.Builder().Build();

        m_bannerView.LoadAd(request);

        m_bannerView.Show();
    }

    internal void HideBanner()
    {
        if (disable)
            return;

        m_bannerView.Hide();
    }

    public void RequestFullScreenAd()
    {
        if (disable)
            return;

        m_fullScreenAd = new InterstitialAd(m_fullScrennAdID);

        AdRequest _request = new AdRequest.Builder().Build();

        m_fullScreenAd.LoadAd(_request);
    }

    public void ShowFullScreenAd()
    {
        if (disable)
            return;

        if (m_fullScreenAd.IsLoaded())
            m_fullScreenAd.Show();
    }
}

