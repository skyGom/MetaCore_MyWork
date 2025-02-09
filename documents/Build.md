# Build
## Build machine을 통한 build
Windows PC에 Jenkins를 사용하여 build machine이 준비되어 있습니다. 그러나, build machine에 Unity3D license가 포함되어 있지 않으므로 별 의미는 없습니다.
## 개인 PC를 통한 build
Human error를 줄이기 위해 Unity Editor에서 Inspector를 통해 몇가지 설정을 자동화하는 script가 준비되어 있습니다. 이 script를 수정하려면 Environment branch에서 BuildPlayer.cs를 수정한 후 main에 병합하여 주시기 바랍니다.

개인 PC에서 Build하기 전에, 아래 사항을 확인하여 주십시오.
* Unity3D에 PRO license가 활성화 되어 있는가? license가 활성화되어 있지 않으면 실행시 Unity3D logo가 보이게 됩니다. live service에 적합하지 않습니다.
* Unity Project ID와 local project가 잘 연결되어 있는가? Project Settings -> Services에서 Unity Project ID가 정확히 연결되어 있지 않으면 In-App 결제 plugin을 제대로 사용할 수 없습니다. Services가 활성화되지 않는 경우 Unity3D 계정 관리자의 계정 권한 부여가 필요할 수 있습니다.
### MetaCore -> Settings and Build
Unity Editor에서 상단 MetaCore / Settings and Build 메뉴를 선택하면 Inspector에 준비해둔 설정 손쉽게 변경할 수 있는 UI가 나타납니다.
#### version 정보
Android의 경우 version: x.x.x , version code : xx 로, iOS의 경우 version: x.x.x , build no : xx (AppConstants.cs) 로 표시됩니다. 해당 정보는 반드시 AppConstants.cs를 통해 수정해 주시기 바랍니다. Settings and Build Inspector를 통해 build를 진행하는 경우 AppConstants.cs의 정보를 토대로 Player Settings를 덮어 쓰게 됩니다.
Android와 iOS의 버전은 다르게 관리될 수 있습니다.
#### UDID
Unity Editor를 통해 게임에 접속하게 되는 경우 사용되는 로그인 key가 됩니다. 임의 수정 가능하여, 원하는 만큼 계정을 생성할 수 있습니다.
시스템 기본 UDID를 사용하려면 Use System UDID 버튼을 클릭하세요. SystemInfo.deviceUniqueIdentifier가 반영됩니다.
새로운 UDID를 임의 생성하려면 ReCreate 버튼을 누르세요.
테스트를 위해 계정 보존이 필요한 경우, 생성한 UDID를 메모해 두었다가 Text Field에 붙어 넣어 기존 계정으로 접속할 수 있습니다.
#### Target
현재 활성화된 Build target을 확인 할 수 있습니다.
#### Define
기능 및 code에서 compile 분기를 태울 수 있는 Macro입니다.
##### TEST_FLIGHT
iOS only. Test Flight에 설정한 External Demo 배포를 위한 설정입니다. Test Flight에서는 in app 결제가 sand box로 이루어지기 때문에 Live 환경에서 결제 abusing이 발생될 수 있으며, TEST_FLIGHT를 설정하여 in app 결제를 차단하게 됩니다.
##### ENABLE_DEV
DEV 버전을 build합니다. apk 출력 파일 이름에 _dev_가 포함됩니다. Intro.scene/GameCore/Network/WebHttp.Dev_ip 를 기반으로 서버에 접속합니다. Unity Editor에도 적용됩니다.
```C#
// ex)
#if ENABLE_DEV
    Init(_dev_ip, _port);
#else // !ENABLE_DEV
    Init(_live_ip, _port);
#endif
```
DEV 버전 초기 실행시 DeveloperView가 노출되며, 접속할 환경 (DEV or LIVE)을 선택할 수 있습니다. runtime에 이를 식별하기 위해 GameCore.ApiAddress를 조회합니다.
```C#
#if ENABLE_DEV
// DEV 버전
if (GameCore.ApiAddress == WebHttp.Instance.LiveAddress)
{
    // DeveloperView에서 선택한 환경의 API 주소를 Live API 주소인 경우
    Console.WriteLine("LIVE 환경에 접속");
}
else
{
    // DeveloperView에서 선택한 환경의 API 주소를 Live API 주소가 아닌 경우
    Console.WriteLine("DEV 환경에 접속");
}
#else
    // LIVE 버전 : 접속 환경 선택의 여지가 없음
    Console.WriteLine("LIVE 환경에 접속");
#endif
```
##### ENABLE_LOG
Debug.* code를 활성화합니다. ENABLE_DEV가 설정된 경우 off 할 수 없습니다. 이 macro가 켜져 있으면 aab 파일을 빌드할 수 없습니다.

단, Dasverse MetaCore 부분에만 적용되며, 다른 3rd party library에는 적용되지 않을 수 있습니다. 이는 새로 정의된 Debug class가 Dasverse namespace 내부에서 정의된 것이기 때문이며, Dasverse namespace 외부에서 해당 Debug class를 사용하려면 using을 사용합니다.
```C#
using Debug = Dasverse.Debug;
```
##### PLAY_STORE_SERVICE
Google Play Store 서비스용 build에서만 사용하는 code block을 활성화시키는 용도로 추가된 macro입니다. 이 macro를 활성화 시키면 APK_SERVICE macro가 비활성화 되며, 반대로 APK_SERVICE macro를 활성화하면 이 macro는 비활성화됩니다.
```C#
// ex)
#if UNITY_ANDROID
#   if PLAY_STORE_SERVICE
        Console.WriteLine("PLAY_STORE_SERVICE 활성화");
#   else // !PLAY_STORE_SERVICE
        Console.WriteLine("PLAY_STORE_SERVICE 비활성화");
#   endif
#endif
```
##### APK_SERVICE
Google Play Store를 통해 서비스할 수 없는 지역 (대한민국, 중국)에 별도의 apk를 build하여 배포할 때 사용하는 code block을 활성화시키는 용도로 추가된 macro입니다. 이 macro를 활성화 시키면 PLAY_STORE_SERVICE macro가 비활성화되며, 반대로 PLAY_STORE_SERVICE macro를 활성화하면 이 macro는 비활성화됩니다.
```C#
// ex)
#if UNITY_ANDROID
#   if APK_SERVICE
        Console.WriteLine("APK_SERVICE 활성화");
#   else // !APK_SERVICE
        Console.WriteLine("APK_SERVICE 비활성화");
#   endif
#endif
```
##### Note
apk or aab build file naming에 추가 할 수 있는 메모입니다.
##### Output
Android only. build 결과 출력되는 file 이름입니다. 날짜, 시간, 접속 환경 (dev or live), log 출력 여부, 서비스 대상 (playstore_service or apk_service), build version, android version code, Note 등의 내용이 포함됩니다.
##### Build APK
Android only. apk 파일을 build합니다. Output에 표기되는 apk 파일이 최종 출력되며, build 전, 후, Inspector에 표시 및 설정 되는 값들과 출력 file 이름을 확인해 주세요.
##### Build AAB
Android only. aab 파일을 build합니다. Output에 표기되는 apk 파일 이름에서 확장자만 aab로 변경되어 출력됩니다. ENABLE_DEV나 APK_SERVICE가 켜져 있으면 aab 파일을 build 할 수 없습니다. 현재 MetaCore는 asset bundle system을 사용하지 않기 때문에, 원활한 배포를 위해 App Bundle (Google Play)가 활성화 됩니다.
##### Append xcode*
iOS only. xcode* directory에 xcode project를 업데이트합니다. 출력된 xcode* directory가 없다면 오류가 발생합니다.
xcode*은 Define 값에 따라 다르게 정의 됩니다.
```
xcode_testflight : TEST_FLIGHT가 정의된 경우. ENABLE_DEV 설정은 무시됩니다.
xcode_dev : ENABLE_DEV가 정의된 경우. 내부 개발 테스트 및 배포 용도입니다.
xcode : 어떤 Define도 정의되지 않은 경우. App Store 배포용도입니다.
```
##### Replace xcode*
iOS only. xcode* directory에 새로운 xcode project를 생성합니다. 이미 출력된 xcode* directory가 있다면 덮어 씌웁니다.
xcode*은 Define 값에 따라 다르게 정의 됩니다.
```
xcode_testflight : TEST_FLIGHT가 정의된 경우. ENABLE_DEV 설정은 무시됩니다.
xcode_dev : ENABLE_DEV가 정의된 경우. 내부 개발 테스트 및 배포 용도입니다.
xcode : 어떤 Define도 정의되지 않은 경우. App Store 배포용도입니다.
```
## Android Build
### Build AAB
Google Play Store 배포용 포맷입니다. Define 값으로 PLAY_STORE_SERVICE만 활성화시킵니다. 현재 MetaCore는 asset bundle system을 사용하지 않기 때문에, 원활한 배포를 위해 App Bundle (Google Play)가 활성화 됩니다.
### Build APK
내부 테스트용 DEV 버전 혹은 Google Play Store 외부 (https://download.metacoreland.com)에 배포하기 위한 포맷입니다. Google Play Store에 서비스하지 않는 지역에 배포할 때에는 Define 값으로 APK_SERVICE만 활성화시킵니다.
## iOS Build
### 개발자 등록
Apple 개발자 프로그램에 등록하지 않은 경우, 클라이언트 개발팀장님께 초대를 요청합니다. Apple 개발자 프로그램에 초대를 받고 등록해야 내부 배포 버전을 빌드할 수 있습니다.
#### Xcode 개발자 로그인
Apple 개발자 프로그램에 등록한 후, Xcode 설정 -> 계정에 등록된 개발자 계정으로 로그인합니다.
개발자 계정에 로그인하지 않았거나 해당 계정에 적절한 개발자 권한이 부여되지 않았다면 Live를 위한 build가 불가능합니다.

개발자 계정이 [AppStoreConnect](https://appstoreconnect.apple.com/access/users)에서 MetaCore에 대한 제품 개발 역할이 있는지, 추가 리소스의 인증서, 식별자 및 프로파일에 액세스 권한이 있는지, 클라우드 관리형 배포 인증서에 액세스 권한이 있는지 확인하세요.
#### 배포용 인증서 등록
배포용 빌드를 위해서는 키체인 접근 (KeyChainAccess.app)에 code sign을 위한 key 및 인증서를 설치해야 합니다. [Certifications.md](./Certifications.md) 참조.
### Build & Run
Unity에서 Xcode용 project를 출력하면 Post Build Script에 따라 필요한 설정이 Xcode project에 적용됩니다.
mac에 테스트용 기기를 연결하고 Build, Run을 실행하여 확인할 수 있습니다.
### Archive
내부 배포용 ipa 제작이나 App Store Upload용 빌드를 출력합니다. Archive 한 경우, Organizer에서 Description에 버전 설명을 반드시 기록하여 주세요. DEV인지, LIVE인지, TEST_FLIGHT인지 알 수 없기 때문에 이에 대한 기록과 주요 변경 내용을 기록해 주시면 됩니다.
Archive가 완료되면 Organizer를 통해 Test Flight 및 App Store 서비스를 위한 build를 Apple에 제출할 수 있습니다. 자세한 내용은 애플 개발자 도움말에서 확인할 수 있습니다. (https://developer.apple.com/documentation/xcode/distributing-your-app-for-beta-testing-and-releases)
### iOS Simulator
AVPro Video Player Library에서 crash가 발생할 수 있습니다. iOS Simulator 환경에서의 테스트는 권장하지 않습니다.
