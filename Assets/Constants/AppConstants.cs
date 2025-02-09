using System;

public enum Datas
{
    ConfigurationData,
    TierRawContainer,
    NpcRawContainer,
    InGameBGMRawContainer,
    InGameSFXRawContainer,
    MineHeroBGMRawContainer,
    MineHeroSFXRawContainer,
    MatchMiningBGMRawContainer,
    MatchMiningSFXRawContainer,
    CrystalPropertyRawContainer,
    ShopRawContainer,
    BetRawContainer,
    SendRewardRawContainer,
    LevelRawContainer,
    RankingRewardRawContainer,
    ItemRawContainer,
    BuffRawContainer,
    MapObjectRawContainer,
    JumpingPetBgmRawContainer,
    JumpingPetSFXRawContainer,
    EmotionRawContainer
}

public enum UIViews
{
    InGameUIMainView,
    SettingPopupView,
    MineHeroView,
    PlayRoomUIView,
    ShopUIView,
    MiningOfficeUIView,
    PortalPopup,
    DialogPopup,
    AlertPopup,
    InGameUITopView,
    MapPopup,
    JumpingPetView,
    AcquireItemPopup,
    ADRewardPopup
}

public enum ComponentDatas
{

}

public enum AtlasId
{

}

public enum UIComponents
{
    MyNickname,
    MyToolTip,
    TutorialVideo,
    RankingPage,
    MailPage,
    SettingPage
}

public enum UIObjectPoolId
{
    TextChat,
    ToolTip,
    PointHistory,
    DropTokenDisplay,
    QuickChating,
    LastRankingItem,
    RankingItem,
    HowToRankingItem,
    Nickname,
    MailItem,
    MailRewardItem,
    RewardAnimIcon,
    PlayTierItem,
    ShopCashProduct,
    ShopSpecialProduct,
    EmotionItem
}

public enum WorldObjectPoolId
{
    None,
    CharacterMale,
    CharacterFemale,
    DropTokenSiver,
    DropTokenGold,
    PetDragon,
    MyPlayer,
    OtherPlayer,


    //FireSlime,
    //WaterSlime,
    //AirSlime,
    //EarthSlime
}

public enum WorldObjectId
{
    None,
    CharacterMale,
    CharacterFemale,
}

public enum TextureId
{

}

public enum AudioGroupType
{
    MasterVolume,
    BGMVolume,
    SFXVolume
}

public enum InGameBgm
{
    Title_BGM,
    Aleoplaza_BGM,
    Miningzone_BGM,
    Swapzone_BGM,
    Playzone_BGM,
    Miningoffice_BGM,
}

public enum InGameSFX
{
    Click_Normal,
    Click_Tention,
    Char_Jump,
    Char_Landing,
    Get_Token,
    Popup_Open,
    Popup_Close,
    Slime_Start,
    Slime_Damage,
    Slime_Boom,
    Tier_Gauge,
    Tier_Up,
    Christmastreenpc,
    eastguardianpc
}

public enum MineHeroBgm
{
    Minehero_Normalbg,
    Minehero_Bonusbg,
    Minehero_Shieldbg,
    Minehero_x2bg,
    Minehero_Feverbg,
    Minehero_Tention,
}

public enum MineHeroSFX
{
    Minehero_Start,
    Minehero_Swipe,
    Minehero_Objectdrop,
    Minehero_Objectboom,
    Minehero_emp,
    Minehero_Result1,
    Minehero_Result2,
    Minehero_Result3,
}

public enum JumpingPetBgm
{
    JumpingPet_Bgm
}

public enum JumpingPetSFX
{
    JumpingPet_ButtonClick,
    JumpingPet_Death,
    JumpingPet_GetCoin,
    JumpingPet_GetItem,
    JumpingPet_Jump,
    JumpingPet_LevelPass,
    JumpingPet_NewRecord,
    JumpingPet_Finish,
    JumpingPet_Wing
}
public enum MatchMiningBgm
{
    MatchMining_Ingame,
    MatchMining_Loading
}

public enum MatchMiningSFX
{
    MatchMining_Time_Under_5,
    //MatchMining_Failed_Game,
    MatchMining_Draw,
    MatchMining_Loading_Start,
    MatchMining_Click_Arrow,
    MatchMining_Click_Lock_Stage,
    MatchMining_Successes,
    MatchMining_Time_Item,
    MatchMining_Click_Successes,
    MatchMining_ClickAll,
    MatchMining_Bomb,
    MatchMining_GameStart,
    MatchMining_Hint,
    MatchMining_GO_ON,
    MatchMining_FeverTime,
    MatchMining_OneClick,
    MatchMining_Hammer,
    MatchMining_ComboStep1,
    MatchMining_ComboStep2,
    MatchMining_ComboStep3,
    MatchMining_ComboStep4,
    MatchMining_ComboStep5,
    MatchMining_Finish,
    MatchMining_Click_Failed

}

public enum Texts
{

}

public enum FontId
{

}

public enum PlayerVfxId
{
    GoldAura
}

public enum MaterialId
{
    Mat_PortalPlayZone,
    Mat_PortalMiningZone,
    Mat_PortalSwapZone,
    Mat_PortalAleoPlaza
}

public enum DropTokenType
{
    Silver,
    Gold
}

public enum SlimeType
{
    FireSlime,
    WaterSlime,
    AirSlime,
    EarthSlime
}

public enum DummyTables
{
    TokenDropDataContainer
}

/// <summary>
/// SceneId. 새로 scene을 추가하면 여기에 등록해야 합니다
/// RestartApp Scene 항상 마지막을 유지하세요
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
    /// <summary>Match Mining (Mini Game, 사천성)</summary>
    MatchMining,
    /// <summary>Jumping Pet (Mini Game)</summary>
    JumpingPet,
    /// <summary>App Reset을 위한 환경 Scene. 항상 제일 마지막에 둘 것.</summary>
    RestartApp
}

public enum PortalId
{
    None,
    PlayZone,
    MiningZone,
    SwapZone,
    AleoPlaza
}

public enum ADType
{
    Image,
    Video
}

public enum ADID
{
    None,

}

//public enum TextType
//{
//    UI,
//    Event
//}

public enum LanguageScripts
{
    Title,
    AvatarChoice,
    Loading,
    MainUI,
    PlayZone,
    MiniGame,
    MiningZone,
    SwapZone,
    Ranking,
    Setting,
    Common,
    Mob,
    Chat,
    NPC,
    Dialog,
    Message,
    Tutorial,
    Letter,
    RewardMessage,
    Item
}

public enum PlayerAnimatorParam
{
    IsWalking,
    IsRunning,
    IsJumping,
    IsLand,
    StateOn,
    StateValue
}

public enum MineHeroObjectId
{
    Seq_1,
    Seq_2,
    Seq_3,
    Seq_4,
    Seq_5,
    Seq_6,
    Seq_7,
    Seq_8,
    Seq_9,
    Seq_10,
    Seq_11,
    Seq_12,
    Seq_13,
    Seq_14,
    Seq_15,
    Seq_16,
    Seq_17,
    Seq_18,
    Seq_19,
    EarnedPoint,
    PointTrail,
    Effect_Gem,
    Effect_Trap
}

public enum JumpingPetObjectPoolId
{
    ScorePopupText,
    ComboPopupText
}

public enum NpcId
{
    None,
    Drerwin,
    TicketClerk,
    MiningGuard,
    MarketSeller,
    MiningOfficeController,
    ChristmasTree,
    EastGuardian
}

public enum Tags
{
    Player,
    OtherPlayer,
    NpcCommunication,
    NpcDetecting
}

public enum Layers
{
    InputEventObject
}

public enum LoaderId
{
    Debugger,
    Data,
    Asset,
    Audio,
    LoadManager
}

public enum TokenType
{
    Aleo,
    USDT,
    File
}

public enum ShopTab
{
    Gem = 1,
    Fuel,
    Special
}

/// <summary>
/// Mini Game 고유 번호
/// </summary>
public enum GameType
{
    /// <summary>AutoPlay 설정 자동화를 위해 추가된 값</summary>
    MIN_VALUE,
    MineHero = 1,
    MatchMining,
    JumpingPet,
    /// <summary>AutoPlay 설정 자동화를 위해 추가된 값. 항상 제일 마지막에 둘 것</summary>
    MAX_VALUE,
}

public enum SignInPlatform
{
    None,
    MetaBucket,
    Google,
    Apple,
    Win32
}

public enum ServerState
{
    Keep,
    Updated,
    Maintenance
}

public enum LanguageId
{
    EN,
    KR,
    JP,
    CN,
    PH,
    VN,
    TH,
    ID,
}

public enum MarketId : byte
{
    None,
    Meta,
    Google,
    Apple,
    Win32
}

/// <summary>
/// alert popup의 button들을 보여주거나 감추기 위한 option
/// </summary>
public enum AlertPopupButtonState
{
    /// <summary>감추지 않고 모두 보여 준다</summary>
    NoHide,
    /// <summary>취소 button만 숨긴다</summary>
    CancelHide,
    /// <summary>확인 button만 숨긴다</summary>
    OKHide,
    /// <summary>모든 button을 숨긴다 (Toast)</summary>
    AllHide
}

public enum TierID
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond
}

public enum MiningOfficeVideoID
{
    MiningzoneEntrance,
    MachineOut,
    CameraIn,
    CameraOut,
    MetaCoreTitleLoading
}

public enum TutorialStepID
{
    NoTutorialData,
    TutorialVideoEnd,
    TutorialDialogEnd,
    SpecialZoneClickEnd
}

public enum RankingItemType
{
    CurrentRankingItem,
    MyRankingItem,
    LastRankingItem,
    HowToRankingItem
}

public enum BuffID
{
    SpeedUpItem,
    DragonEvent
}

public enum MapKind
{
    World,
    AleoPlaza,
    MiningZone,
    SwapZone,
    PlayZone,
    MiningOffice,
}

/// <summary>
/// Item Type
/// </summary>
public enum ItemType : int
{
    /// <summary>클라이언트에서 정의되지 않은 아이템</summary>
    Unknown,

    /// <summary>광장에 drop 되거나 event를 통해 지급되는 token. 지갑이 연동된 사용자는 모아서 swap 할 수 있다.</summary>
    Token,
    /// <summary>in game 재화 보석. 미니게임 Multi Mode 입장이나 다른 item 구매에 사용.</summary>
    Gem,
    /// <summary>in game 재화 연료. 미니게임 Single Mode 입장에 사용.</summary>
    Fuel,
    /// <summary>알레오 코인</summary>
    AleoCoin,
    /// <summary>USDT 코인</summary>
    USDTCoin,
    /// <summary>File 코인</summary>
    FileCoin,
    /// <summary>? 코인</summary>
    QuestionCoin,

    /// <summary>Portal Card. 다른 zone (scene)으로 순간 이동할 때 사용.</summary>
    PortalCard,
    /// <summary>Speed Up. Zone (scene)에서 달리기 할 때 속도가 올라감.</summary>
    SpeedUp,
    /// <summary>Bomb. Mini Game Match Mining에서 사용하는 아이템</summary>
    Bomb,
    /// <summary>Shuffle. Mini Game Match Mining에서 사용하는 아이템</summary>
    Shuffle,
    /// <summary>Wing. Mini Game Jumping Pet에서 사용하는 아이템</summary>
    Wing,
    /// <summary>Revive. Mini Game Jumping Pet에서 사용하는 아이템</summary>
    Revive,
    /// <summary>Emp. Mini Game MineHero에서 사용하는 아이템</summary>
    Emp,
    /// <summary>NoBomb. Mini Game MineHero에서 사용하는 아이템</summary>
    Ignore,

    /// <summary>RandomBox. AD Reward Popup에서 보상받은 랜덤박스1</summary>
    RandomBox1,
    /// <summary>RandomBox. AD Reward Popup에서 보상받은 랜덤박스2</summary>
    RandomBox2,

    /// <summary>ItemType 최대 값. 클라에서 제공하는 ItemType 범위 확인용. 항상 마지막에 있어야 합니다.</summary>
    Max,
}

public static class Item
{
    public static ItemType Type(int no)
    {
        if (no <= (int)ItemType.Unknown || no >= (int)ItemType.Max)
            return ItemType.Unknown;
        return (ItemType)no;
    }

    public static ItemType Type(byte no)
    {
        if (no <= (byte)ItemType.Unknown || no >= (byte)ItemType.Max)
            return ItemType.Unknown;
        return (ItemType)no;
    }

    public static bool IsInGameMoney(ItemType type)
    {
        return type >= ItemType.Token && type <= ItemType.Fuel;
    }

    public static bool IsCoin(ItemType type)
    {
        return type >= ItemType.AleoCoin && type <= ItemType.QuestionCoin;
    }
}

/// 캐릭터의 코스튬 가능한 부위별로 분류
/// </summary>
public enum CostumeType
{
    Body,
    Head,
    Hair,
    Top,
    Bottom,
    Shoes
}

/// <summary>
/// 캐릭터 바디 파츠의 부위별로 분류
/// </summary>
public enum BodyPartType
{
    Torso_Hips = 0,
    Torso_Spine01,
    Torso_Spine02,
    Torso_Shoulders,
    Torso_Head,

    Arms_Lower = 20,
    Arms_Upper,
    Arms_Hand,

    Legs_Upper = 30,
    Legs_Knees,
    Legs_Lower,
    Legs_Feet,
}

/// <summary>
/// 캐릭터의 코스튬 바디 프리팹
/// </summary>
public enum BodyPart
{
    M_Body_001_white = 100,
    M_Body_002_yellow,
    M_Body_003_black,
    F_Body_001_white = 200,
    F_Body_002_yellow,
    F_Body_003_black,
}

/// <summary>
/// 코스튬 헤어 프리팹 ID
/// </summary>
public enum HairPart
{
    Hairstyle_001 = 100,
    Hairstyle_002,
    Hairstyle_003,
    Hairstyle_004,
    Hairstyle_005,
    Hairstyle_006,
    Hairstyle_007,
    Hairstyle_008,
    Hairstyle_009,
    Hairstyle_010,
    Hairstyle_011,
    Hairstyle_012,
    Hairstyle_013,
    Hairstyle_014,
    Hairstyle_015,
    Hairstyle_016,
    Hairstyle_017,
    Hairstyle_018,
    Hairstyle_019,
    Hairstyle_020,
    Hairstyle_021,
    Hairstyle_022,
    Hairstyle_023,
    Hairstyle_024,
    Hairstyle_025,
    Hairstyle_026,
    Hairstyle_027,
    Hairstyle_028
}

/// <summary>
/// 코스튬 머리 프리팹 ID
/// </summary>
public enum HeadPart
{
    M_head_001 = 100,
    M_head_002,
    M_head_003,
    M_head_004,
    F_head_001 = 200,
    F_head_002,
    F_head_003,
    F_head_004
}

/// <summary>
/// 코스튬 상의 프리팹 ID
/// </summary>
public enum TopPart
{
    M_top_001_jacket = 100,
    M_top_002_openshirtT,
    M_top_003_shirt,
    M_top_004_shirt,
    M_top_005_shirtvest,
    M_top_006_smalljacket,
    M_top_007_smalljacket,
    M_top_008_sweater,
    M_top_003_shirt_1,
    M_top_003_shirt_2,
    M_top_003_shirt_3,
    M_top_003_shirt_4,
    F_top_001_jacket = 200,
    F_top_002_openshirtT,
    F_top_003_shirt,
    F_top_004_shirt,
    F_top_005_shirtvest,
    F_top_006_smalljacket,
    F_top_007_smalljacket,
    F_top_008_sweater,
    F_top_003_shirt_1,
    F_top_003_shirt_2,
    F_top_003_shirt_3,
    F_top_003_shirt_4
}

/// <summary>
/// 코스튬 하의 프리팹 ID
/// </summary>
public enum BottomPart
{
    M_bot_001_bigshorts = 100,
    M_bot_002_loosepants,
    M_bot_003_pants,
    M_bot_004_shorts,
    M_bot_005_shortskirt,
    M_bot_006_skinnyjeans,
    M_bot_001_bigshorts_1,
    M_bot_001_bigshorts_2,
    M_bot_001_bigshorts_3,
    M_bot_001_bigshorts_4,
    F_bot_001_bigshorts = 200,
    F_bot_002_loosepants,
    F_bot_003_pants,
    F_bot_004_shorts,
    F_bot_005_shortskirt,
    F_bot_006_skinnyjeans,
    F_bot_005_shortskirt_1,
    F_bot_005_shortskirt_2,
    F_bot_005_shortskirt_3,
    F_bot_005_shortskirt_4,
}

/// <summary>
/// 코스튬 신발 프리팹 ID
/// </summary>
public enum ShoesPart
{
    Shoes_001_boots = 100,
    Shoes_002_chelsea,
    Shoes_003_flipflop,
    Shoes_004_highboots,
    Shoes_005_lowsneakers,
    Shoes_006_sandals,
    Shoes_007_sneakers,
}

/// <summary>
/// 미니게임 tutorial 노출 설정 자동화. 미니게임 추가시 1개씩 추가. 값은 2의 N승을 사용한다.
/// </summary>
public struct ContentTutorialID
{
    public const int MineHero = 1;
    public const int MatchMining = 2;
}

public enum Emotion : byte
{
    UNKNOWN,
    HI,
    TRUST,
    WHISPER,
    YES,
    NO,
    COMEON,
    KISS,
    EXCITED,
    READY,
    SAD,
    DESPAIR,
    ANGRY,
    DANCE1,
    DANCE2
}

public class AppConstants
{
#if UNITY_ANDROID || UNITY_EDITOR_WIN
    private static readonly byte versionMajor = 2;
    private static readonly byte versionMinor = 0;
    private static readonly byte versionMicro = 5;

    // android version code
    public static readonly int VersionCode = 49;
#elif UNITY_IOS || UNITY_EDITOR_OSX
    private static readonly byte versionMajor = 2;
    private static readonly byte versionMinor = 0;
    private static readonly byte versionMicro = 5;

    // ios build no
#   if TEST_FLIGHT
    // Test Flight External Demo 배포용. In App 결제가 차단 된다.
    private static readonly int buildNo = 46; // 다음을 47로 변경해야 함
#   else
    // Apple App Store 배포용. In App 결제가 허용된다.
    private static readonly int buildNo = 45; // ㄷ다음을 48로 변경해야 함
#   endif

    public static string BuildNo
    {
        get
        {
            return buildNo.ToString();
        }
    }
#endif

    public static readonly int Version = versionMajor * 10000 + versionMinor * 100 + versionMicro;

    public static string VersionString
    {
        get
        {
            return string.Format("{0}.{1}.{2}", versionMajor, versionMinor, versionMicro);
        }
    }
}
