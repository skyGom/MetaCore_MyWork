# Plugins
MetaCore에는 여러가지 SDK가 사용됩니다. 특별한 문제가 없다면, 적용된 SDK를 계속 사용합니다. SDK의 Update는 native library 충돌이나 커스텀 코드의 롤백, deprecated api로 인한 legacy code 오류 발생 등 여러가지 문제를 일으킬 수 있습니다.

굳이 SDK를 업데이트해야 하는 상황이 발생한다면, 하기 내용들을 반드시 숙지후 진행하시기 바랍니다.

Update를 진행하고 Android 의존성이 해결되면 iOS 환경에서도 의존성 해결을 진행해야 합니다. Android의 의존성 해결 과정에서 iOS의 의존성 해결 내용이 롤백될 수 있습니다.
## Firebase
[Firebase Unity SDK](https://firebase.google.com/docs/unity/setup?hl=ko) 11.7을 사용하고 있습니다.
### 포함 기능
#### External Dependencies Manager 1.2.179
다른 Plugin들과 함께 의존성 문제 해결을 위한 EDM은 Firebase Unity SDK에 포함된 것을 기준으로 합니다. AppLovin 광고 SDK 업데이트 과정에서 EDM이 업데이트되거나 이전 버전으로 되돌아가지 않도록 주의가 필요합니다.
#### Analytics
Firebase Console에서 분석을 위해 포함된 기능입니다.
#### Auth
Google Login, Apple Login 등을 위해 포함된 기능입니다.
#### Dynamic Links
MetaBucket Login 지원을 위해 포함된 기능입니다.
#### Messaging
Firebase Cloud Messaging (FCM)을 위해 포함된 기능입니다.
#### In App Messaging
Firebase Console에서 MetaCore App 실행시 in app 공지 팝업을 노출하기 위해 포함된 기능입니다.
### Update
[Firebase Unity SDK](https://firebase.google.com/docs/unity/setup?hl=ko)에서 최신 SDK를 다운로드 한 후 상기 package들을 나열된 순서대로 Unity에 끌어다 놓습니다. 제거된 기존 버전은 삭제합니다. EDM을 사용하여 의존성 문제를 해결합니다.
## AppLovin MAX SDK
광고 SDK입니다. Unity에서 AppLovin -> Intergration Manager를 클릭하여 새창을 열어 줍니다.
### 포함 기능
#### App Lovin MAX SDK
기본 SDK입니다. 설치된 버전은 Unity 6.3.1, Android 12.3.1, iOS 12.3.1입니다.
#### Mediation Networks - Google AdMob
구글 Admob 광고를 노출하기 위한 adapter입니다. 설치된 버전은 android_23.0.0.0_ios_11.2.0.0입니다. Upgrade 후 App ID가 초기화 될 수 있습니다. Upgrade 전에 App ID가 유실되지 않도록 복사해 둡니다.
#### Mediation Networks - Pangle
Pangle 광고를 노출하기 위한 adapter입니다. 설치된 버전은 android_5.8.1.0.1_ios_5.8.0.8.1입니다.
#### AppLovin SDK Key
Upgrade 과정에서 초기화 될 수 있습니다. Upgrade 전에 설정 값을 복사해 둡니다.
### 주의 사항
업데이트시 기존 Custom Code가 롤백됩니다. iOS에서 AppTrackingTransparency 기능이 빠지게 되므로, [AdMonetization.md](./AdMonetization.md)를 확인하여 커스텀 코드가 유실되지 않도록 합니다.
## Sign In With Apple
iOS에서 Apple Login 기능을 As Service type으로 제공하기 위해 개발된 plugin입니다.

Apple에서 해당 기능 관련 문제나 업데이트를 요구하는 경우 직접 code를 수정해야 합니다.

Unity Level에서 C#으로 작성되었으며, Assets/SignInWithApple/Plugins/iOS/에 Obj-C code로 작성되어 있습니다.
## Google Sign In
Android, iOS에서 Google Login 기능 지원을 위해 제공되는 plugin입니다. 공식 update가 중단된지 매우 오래되었기 때문에 최신 iOS 환경에서 동작하지 않는 legacy code가 포함되어 있습니다. MetaCore에는 이를 수정한 코드가 적용용어 있으며, Google이나 Apple에서 해당 기능 관련 문제나 업데이트를 요구하는 경우 직접 code를 수정해야 합니다. Assets/Plugins/iOS/GoogleSignIn/에 Obj-C로 작성된 code를 확인하세요.
## AVProVideo
영상 재생을 위한 plugin입니다. iOS 버전과 Android 버전이 별도로 구매되었기 때문에, Update 하려면 서로 덮어 쓰지 않도록 주의합니다. Android의 경우 이 plugin에 포함된 guava library가 build 과정에서 충돌할 수 있으므로 update 후에 build 과정에서 제외될 수 있도록 .meta의 수정이 필요합니다.

## 기타
이외 여러 가지 plugin들이 있으나, native 기능과 관련하여 충돌하거나 문제가 되는 것들은 없습니다.
