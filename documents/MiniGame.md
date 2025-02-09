# Mini Game 추가
Mini Game 1호인 Mine Hero를 주로 참고하시기 바랍니다.
## Mini Game 진입 과정에 필요한 UI 구성
In Game (World) -> InGameUIMainView.prefab -> 우상단 Room 버튼을 클릭하여 Mini Game List Popup을 통해 Mini Game에 접근합니다.
### Mini Game List Popup
PlayRoomUIView.prefab/Body/GameListUI 하위에 위치합니다. 각 Mini Game별 List의 icon을 PlayRoomUIView.prefab/Body/GameListUI/ScrollView/Viewport/Content/에 추가해야 합니다.
### Mini Game 진입 Popup
PlayRoomUIView.prefab/Body/GameMainUI 하위에 위치합니다. 각 Mini Game별 진입 popup을 PlayRoomUIView.prefab/Body/GameMainUI/GameContainer/에 추가해야 합니다.
PlayRoomUIView.prefab/Body/GameMainUI/Title 이미지는 게임에 따라 변경됩니다. 이 이미지를 참조하는 경로는 PlayRoomUIView.prefab/Body/GameMainUI/GameContainer/각 게임/Title.Image이므로, popup 생성 및 구성시 해당 경로를 반드시 유지해 주시기 바랍니다.
GAME RULES 버튼을 누르면 Rule Guide Popup이 노출됩니다.
### Mini Game Rule Guide Popup
PlayRoomUIView.prefab/Body/RulesUI 하위에 위치합니다. 각 Mini Game별 Rule Guide popup을 PlayRoomUIView.prefab/Body/RulesUI/RuleContainer/에 추가해야 합니다.
## Game Type 및 Scene Id 추가
[AppConstants.cs](../Assets/Constants/AppConstants.cs)에 Server 개발팀과 협의하여 결정한 Mini Game Type을 추가하고, Scene Id도 추가하도록 합니다.
```C#
...
/// <summary>
/// SceneId. 새로 scene을 추가하면 여기에 등록해야 합니다
/// </summary>
public enum SceneId
{
    /// <summary>없음</summary>
    None,
    /// <summary>최초 진입</summary>
    Intro,
    /// <summary>Lobby. 사용 안 함</summary>
    Lobby,
    /// <summary>MetaCore 광장</summary>
    AleoPlaza,
    /// <summary>Mining Zone</summary>
    MiningZone,
    /// <summary>Swap Zone</summary>
    SwapZone,
    /// <summary>Play Zone</summary>
    PlayZone,
    /// <summary>Mine Hero (Mini Game)</summary>
    MineHero,
    /// <summary>Mining Office (채굴 서비스 확인)</summary>
    MiningOffice,
    /// <summary>App Reset을 위한 환경 Scene</summary>
    RestartApp
}
...
public enum GameType
{
    // 추가하려는 GameType을 Server 개발팀과 협의하여 정의된 값을 추가합니다
    MineHero = 1
}
...
```
## [PlayRoomUIView.cs](../Assets/Scripts/UI/Views/PlayRoomUIView.cs) code 작성
Mini Game 진입을 위해 추가 code 작성이 필요합니다.
### StartMiniGameAsync
Mini Game 진입 Popup에서 GAME PLAY를 누를 때 호출되는 함수입니다.
```C#
...
int serverType = 0;                                 // 접속 Server Type. 3: Mini Game
SceneId sceneId = SceneId.None;                     // 진입할 SceneId
MiniGamePacketProcessor packetProcessor = null;     // 각 게임별 Packet 처리기
switch (gameType)
{
    // gameType에 따라 상기 3가지 변수를 설정하도록 합니다
    case GameType.MineHero: serverType = 3; sceneId = SceneId.MineHero; packetProcessor = new MineHeroPacketProcessor(); break;
}
...
```
### showMatchPopup
Mini Game Multi Mode 진입 전 matching 상태 popup 출력을 위한 함수입니다.
```C#
...
SceneId sceneId = SceneId.None;

switch (index)
{
    case 0:
        position = ScriptConstants.MiniGame.LookingForOpponent;
        alertMessage = ScriptDisplayUtil.GetText(LanguageScripts.MiniGame, position.ToString());
        alertPopup.ShowAlertUI(alertMessage, null, BattleServerManager.Instance.CancelMatch, AlertPopupButtonState.OKHide);
        return;
    // 주어진 Mini Game index를 사용하여 진입할 sceneId를 설정합니다.
    case (int)GameType.MineHero: sceneId = SceneId.MineHero; break;
}
position = ScriptConstants.MiniGame.GameWaitingGuide;
alertMessage = ScriptDisplayUtil.GetText(LanguageScripts.MiniGame, position.ToString());
alertPopup.ShowAlertUI(alertMessage, null, null, AlertPopupButtonState.AllHide);
alertPopup.GameCountdownAsync(() => GameCore.StartLoadingScene(sceneId)).Forget();
...
```
## Packet Processor 추가
Mini Game은 Server와의 통신을 위해 WebSocket을 사용합니다. Protocol은 json string을 사용하여, Mini Game 공통 packet을 사용하며 Game에 따라 custom packet이 추가됩니다. 혹은 공통 packet이라도 게임별 처리가 필요한 부분은 추가 관리가 필요합니다.
### [Mini Game Packet Processor](../Assets/Scripts/Networks/Webs/MiniGamePacketProcessor.cs)
각 Mini Game의 Packet Processor는 이 class를 상속 받아 사용합니다.
모든 Mini Game들이 공통으로 사용, 처리하는 packet은 이 class에 이미 처리 방식이 정의되어 있으나, 개별 Mini Game의 특성에 따라 처리 방식이 달라지거나 독자적인 규격의 packet을 추가로 처리해야 하는 경우라면 별도의 Packet Processor를 작성하여 사용해야 합니다.
작성된 Packet Processor는 StartMiniGameAsync 내부에 instance를 생성하는 code를 추가 작성하여 Mini Game 접속시 사용하도록 해야 합니다.
