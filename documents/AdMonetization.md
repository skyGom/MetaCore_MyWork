# Advertisement Monetization
특정 조건의 사용자들에게 특정 시점에 전면 광고 (Interstitial)나 보상형 광고 (Rewarded)를 노출하여 광고를 보게하고 이에 따른 수익을 창출합니다.
## [AppLovin](https://www.applovin.com)
광고 Mediation 도구로 [AppLovin](https://www.applovin.com)을 사용하며, [AppLovin](https://www.applovin.com)에서 제공하는 [Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)를 사용합니다.
### [AdMob](https://admob.google.com)
[AdMob](https://admob.google.com) 광고를 노출할 수 있으며, [Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)에 의해 관리됩니다.
### [Pangle](https://www.pangleglobal.com)
[Pangle](https://www.pangleglobal.com) 광고를 노출할 수 있으며, [Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)에 의해 관리됩니다.
### iOS App Tracking Transparency
iOS에서는 사용자와 직접 연계되지 않는 정보 수집을 위한 동의를 받기 위한 팝업 노출 및 권한 획득을 요구합니다. [Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)에서는 이러한 기능을 제공하고 있지 않으며 이를 위해 [Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration)의 일부 코드를 커스텀하여 사용하고 있습니다.

광고 SDK의 Update가 진행될 경우 커스텀 작업 내용이 되돌아갈 수 있으므로 주의하시기 바랍니다.

수정 내용 참고 : https://github.com/Dasverse/Aleo_Client/commit/d0d4d52c3dfc2d3baf0b01e467c48f174bda9d2c
#### MAUnityAdManager.h
App Tracking Transparency 기능 추가를 위해 header file 추가
#### MAUnityAdManager.m
App Tracking Transparency 관련 code 추가
#### MAUnityAdManager.m.meta
Framework Dependencies "AppTrackingTransparency" 추가
## 광고 노출
[Max SDK](https://dash.applovin.com/documentation/mediation/unity/getting-started/integration) 사용의 편의성을 위해 wrapping한 ADMonetizationManager를 통해 광고 노출을 진행합니다.
기본 instance는 GameCore에 생성됩니다.
GameCore.ADMonetizationManager로 접근하며, 광고 형태에 따라 호출하는 함수가 다릅니다.
호출 결과로 Subject을 반환하며, 이를 구독하여 광고 노출 결과에 따른 처리를 진행합니다. 구독 결과로 반환되는 IDisposable object를 관리하여 불필요한 구독은 Dispose하여 주세요.
```C#
IDisposable interstitialAdSubscription = GameCore.ADMonetizationManager.ShowInterstitialAd().Subscribe(step =>
    {
        // 진행 상태에 따라 step이 반환
    }, exception =>
    {
        // 예외는 Excpetion을 사용하며, Message로 error가 발생한 원인을 알 수 있습니다.
        Debug.Log("GameCore.OnVerified() ShowInterstitialAd() exception : " + exception.Message);

        // TODO: 예외 발생시 별도의 조치 없이 다음 단계를 진행합니다.
    }, () =>
    {
        // 광고를 시청하고 창을 닫았을 때 complete가 호출됩니다.
        // TODO: 전면 광고는 별도의 처리 없이 다음 단계를 진행합니다.
        // TODO: 보상형 광고의 경우 적절한 보상을 지급하고 다음 단계를 진행합니다.
    });
```
보상형 광고는 ShowRewardedAd() 함수를 사용하며, 호출 결과나 처리 방식은 전면 광고와 다르지 않습니다.
단, 보상형 광고의 경우 세로형이 있으며, mini game JumpingPet에서 사용합니다. 보상형 광고의 세로형은 postfix로 "Vertical"을 가집니다.
