# Swap Web
## Swap Web on/off 설정
운영팀에서 config file의 shopVisible 값을 bit 연산을 사용하여 설정하면 Service Target별로 on/off가 가능합니다.
* Google Play Store Service : 1
* Android APK Service : 2
* Apple App Store Serivce : 4

on 시키고 싶은 Service Target의 지정 값들을 모두 더하여 shopVisible 값을 설정하면 됩니다.
모두 off하려면 0을 입력합니다.
```
// 예) Google Play Store와 Android APK Service만 켜고 싶을 때, 각 service 활성화 값 1과 2를 더하여 3으로 설정합니다.
shopVisible = 3
// 예) Android APK Service와 Apple App Store Service 만 켜고 싶을 때, 각 service 활성화 값 2와 4를 더하여 6으로 설정합니다.
shopVisible = 6
```

## Swap Web page 노출
api server에 요청 (RequestBucketShopUrl) 하여 받아오는 url과 parameter를 연결하여 webview로 노출합니다.
