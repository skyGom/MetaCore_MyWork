# Local Notification
특정 시간에 사용자에게 예약된 알림을 노출하여 게임에 진입하도록 유도하기 위해 Local Notification 기능을 사용합니다.
## Local Notification Manager
Unity에서 제공하는 Local Notification 기능이 iOS와 Android가 상이하기 때문에, 이를 동일한 interface로 맞춰 호출 할 수 있도록 wrapping한 Local Notification Manager를 사용합니다.
Local Notification Manager instance는 GameCore에 할당되어 있으며, GameCore.LocalNotificationManager로 접근할 수 있습니다.
### Local Notification ID
알림 예약 및 취소를 위해 enum으로 정의된 ID를 사용합니다.
```C#
public enum LocalNotificaationID : int
{
    idFuelCharged = 1001,           /// 연료가 모두 충전되는 시점에 노출할 알림 (최대 5시간 뒤)
    idRankingFishedSoon = 1002,     /// 랭킹 종료 임박 알림 (사용자 local 시간대 기준 일요일 오전 10시)
    idArrivedRankingResult = 1003,  /// 랭킹 결과 도착 알림 (사용자 local 시간대 기준 월요일 오후 8시)
}
```
추후 알림 예약 추가시 ID를 추가하여 사용하시기 바랍니다.
### 예약 취소
CancelNotification(LocalNotificationID) 함수를 사용합니다.
### 예약 진행
SendNotification(LocalNotificationID, string, string, int) 함수를 사용합니다.
```C#
/// <summary>
/// 알림을 예약합니다.
/// </summary>
/// <param name="id">알림 형태 ID</param>
/// <param name="title">알림 제목</param>
/// <param name="messaage">알림 내용</param>
/// <param name="startTime">발동 시간. 현재 시간 + startTime초  뒤에 알림이 발생합니다.</param>
public void SendNotification(LocalNotificaationID id, string title, string messaage, int startTime)
```
알림은 단발성만 지원하고 있으며, 반복성 알림의 경우 Local Notificaion Manager의 기능 추가 개발이 필요합니다.
