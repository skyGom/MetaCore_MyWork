# Coding Convention
C#, json 등 MetaCore 개발에 사용되는 source code 작성시 지켜야 할 규칙입니다.

## Encoding
UTF-8을 사용합니다. 이미 EUC-KR이 적용되어 저장소에 저장된 경우, file encoding을 UTF-8 with BOM으로 변경하여 기존 Editor 환경에서도 인식이 수월하도록 합니다.

## C# (.Net Code style base)
기본적으로 .Net code style을 적용하며, 약간의 예외가 적용됩니다.
문서의 마지막에는 최소 1줄 이상 줄바꿈을 추가합니다.

### .editorconfig
project root에 .editorconfig 파일이 포함되어 있으므로, 가능하면 각 editor에서 이 rule을 기본 적용할 수 있도록 해 주시기 바랍니다.

file 저장시 자동으로 적용 되도록 설정해 주세요. (Visual Studio Code의 경우, 설정에서 Editor:Format on Save를 활성화합니다.)

### 예외 사항
static field prefix "\_s"는 사용하지 않습니다.
instance field prefix "\_"는 사용하지 않습니다.
private 또는 protected method는 carmelCase를 사용합니다. (단, Unity 기본 method들은 제외합니다)
public method는 PascalCase를 사용합니다.

### Documentation comment
"///"로 시작하는 XML 형식으로 주석을 작성합니다.

### TimeUtil.Now 및 TimeUtil.UtcNow 사용
DateTime.Now, DateTime.UtcNow 대신 TimeUtil.Now, TimeUtil.UtcNow를 사용합니다. TimeUtil에서는 Rest API Server에서 Timestamp 값을 가져와 Client의 Local System Time을 보정하여 사용합니다.
Client는 시스템 설정 시간 값을 자동으로 설정하지 않거나, 배터리 문제, GPS나 Cellular 사용 불가, 사용자의 의도 등 여러가지 원인에 따라 시간 값이 정확하지 않을 수 있기 때문에 이런 방식을 사용하게 됩니다.

### using Debug = Dasverse.Debug 사용
UnityEngine.Debug의 경우 Release build에서도 log가 발생합니다. 지나친 log 발생은 성능 저하 문제를 발생시킬 수 있기 때문에 Release build에서는 이를 제외시킬 수 있도록 UnityEngine.Debug를 wrapping한 Dasverse.Debug를 사용하세요.

## JSON
ident size는 whitespace 2칸입니다.

## 주의 사항
code, comment, commit description, naming 등에서 오탈자가 발생하지 않도록 주의하시기 바랍니다. 추후 검색에 문제가 발생할 수 있습니다.