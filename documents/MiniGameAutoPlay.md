# Mini Game Auto Play
MetaCore에는 3개의 Mini Game이 포함되어 있으며, 개발 순서에 따라 각각 Mine Hero, Match Mining, Jumping Pet입니다.
각 미니게임은 개발 및 테스트의 편의를 위해 Auto Play 기능을 포함하고 있으며, Unity Editor 환경 + ENABLE_DEV 옵션 또는 ENABLE_DEV 옵션을 포함한 DEV 빌드에서 기능을 활성화하여 사용할 수 있습니다.
## SRDebugger 활성화
Auto Play 기능을 활성화하기 위해서는 초기 실행시 보여지는 개발자 설정 (Developer View)에서 DEBUGGER 버튼을 눌러 활성화 시키고 DEV 환경으로 접속합니다.
DEBUGGER 버튼을 통해 SRDebugger가 활성화 되면 screen의 좌측 상단 구석을 세번 연속 터치하여 SRDebugger를 띄울 수 있습니다.
SRDebugger의 좌측 패널에서 Options를 선택하면 각 Mini Game의 Auto Play 기능을 활성화할 수 있습니다.
## AutoPlay 활성화
각 게임별 Auto Play 값이 0보다 크면 Auto Play가 활성화됩니다. 값을 0으로 설정하면 Auto Play 기능이 off됩니다.
### JumpingPet Bot
Jumping Pet의 Auto Play 유효 범위는 0~1 사이의 실수입니다. 이는 Pet이 추락하지 않는 확률입니다. 1 = 100%, 0.5 = 50%인 방식이며, Auto Play가 활성화되면 Combo 및 Perfect는 추락하지 않은 확률과 상관 없이 100% 획득하게 됩니다.
### MatchMining Bot
Match Mining의 Auto Play 유효 범위는 0~N의 정수입니다. 이는 한 쌍의 패를 제거한 후 다음 한 쌍의 패를 제거할 때까지 기다릴 시간의 기초값으로 단위는 초입니다. 기다리는 시간의 계산 식은 아래와 같습니다.
```
기다리는 시간 = 기초값 * (0.5 ~ 1.2 사이의 random 값)
```
따라서, 기초값으로 0.8을 지정한 경우 각 턴마다 대기하는 시간은 0.8 * 0.5 ~ 0.8 * 1.2 = 0.4 ~ 0.96 사이의 값이 됩니다.
### MineHero Bot
Mine Hero의 값은 Test Blade의 충돌 영역 반지름입니다. Auto Play가 활성화 되면 폭탄과는 충돌하지 않으며, Test Blade가 화면을 돌아다니면서 object를 파괴합니다.
## Save
체크하면 값이 저장되며, 다음 실행시 SRDebugger를 활성화하지 않아도 저장된 값으로 Auto Play가 자동 적용, 활성화됩니다.
체크하지 않으면 저장된 값은 삭제되지만, 게임을 종료할 때까지는 설정한 TestBlade 값이 적용됩니다.
