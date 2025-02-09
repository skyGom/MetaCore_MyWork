# Certifications
## Android Key Store
Android 버전 build 및 배포를 위해서는 Key Store 파일과 암호가 필요합니다.
아래 파일을 확인하세요.
```
Certifications/das.keystore
```
alias는 metacore를 사용하며, alias password와 keystore password는 dasverse2022! 로 동일합니다. 이는 [Build.md](./Build.md)에 설명된 Build 자동화 script에 포함되어 있습니다.
## iOS Distribution Certifications
iOS 버전 build 및 배포를 위해서는 macOS의 키체인 접근 (Keychain Access.app)에 배포용 인증서를 설치해야 합니다.

Service 주체가 TrueSuccess이기 때문에, 해당 법인에서 iOS 배포용으로 사용하는 인증서를 설치합니다.

아래 파일들을 확인하세요.
```
Certifications/DistCert_J98HLZVY2T.p12
Certifications/DistCert_J98HLZVY2T.md
Certifications/DistCert_J98HLZVY2T.txt
```
p12 파일을 Keychain Access.app에 설치하고, 설치에 필요한 암호는 md 파일에 적혀 있습니다.
## iOS APNS Certification Key
iOS 버전 Remote Push Notification 발송을 위해 Push Server에 APNS 인증키를 등록해야 합니다.

Firebase Cloud Messaging을 사용하고 있기 때문에, 해당 키는 Firebase Console (https://console.firebase.google.com/project/metacore-395305/settings/cloudmessaging) 에 이미 등록되어 있습니다.

파일 위치를 확인해 주세요.
```
Certifications/AuthKey_KW7S7W255G.p8
```
## Sign In with Apple
Apple Login 기능을 사용하기 위한 인증키가 있습니다. Apple Login은 Android 버전에서는 Firebase를 통해 진행하게 되며, iOS 버전에서는 native code에서 Sign in with Apple as Service로 로그인한 다음 로그인 정보를 다시 Firebase로 넘겨 진행하도록 되어 있습니다.

해당 인증키는 이미 Firebase Console(https://console.firebase.google.com/project/metacore-395305/authentication/providers)에 등록되어 있습니다.

파일 위치를 확인해 주세요.
```
Certifications/AuthKey_947J5TQ273.p8
```
