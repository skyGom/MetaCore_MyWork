# Custom Alert Popup
기본 Alert Popup에서 제목 문자열, 제목 문자열 색상, 확인 버튼 문자열, 취소 버튼 문자열을 custom 할 수 있는 기능이 추가되었습니다.
단, 제목 문자열 색상은 Popup이 구성된 후에 적용되기 때문에 ShowAlerUI 함수의 parameter로 전달되지 않고, ShowAlertUI의 반환 값인 AlertPopup instance에서 TitleColor setter를 호출하여 설정합니다.
```C#
/// <summary>
/// custom alert popup에서 title color를 설정
/// </summary>
public Color TitleColor { set { titleText.color = value; } }

/// <summary>
/// 주어진 옵션으로 custom alert을 구성하여 출력한다
/// </summary>
/// <param name="title">제목 text</param>
/// <param name="confirm">확인 button text</param>
/// <param name="cancel">취소 button text</param>
/// <returns>TitleColor를 설정하기 위해 노출된 AlertPopup instance를 반환한다</returns>
/// <inheritdoc cref="ShowAlertUI(string, Action, Action, AlertPopupButtonState)"/>
public AlertPopup ShowAlertUI(string title, string message, string confirm = null, Action confirmAction = null, string cancel = null, Action cancelAction = null, AlertPopupButtonState buttonState = AlertPopupButtonState.NoHide)
```
아래와 같이 사용할 수 있습니다.
```C#
string title = InGameUIMainView.Instance.ReturnAlertPopupTitle(ScriptConstants.Title.RequestDelete1);
string message = InGameUIMainView.Instance.ReturnAlertPopupMessage(ScriptConstants.Message.RequestDelete2);
string confirm = InGameUIMainView.Instance.ReturnAlertPopupMessage(ScriptConstants.Message.RestoreAnswer);
AlertPopup.Instance.ShowAlertUI(title, message, confirm, requestCancelDeleteAccount, null, cancelServerLogin).TitleColor = new Color(1, 71 / 255f, 33 / 255f, 1);
```
* nullable 값인 confirm, cancel에 null 값을 집어 넣으면 원래 AlertPopup이 설정하는 기본 확인, 취소 버튼 문자열로 초기화됩니다.
* TitleColor 역시 ShowAlertUI 호출 후에 별도로 호출하지 않으면 원래 AlertPopup이 설정하는 제목의 기본 색상 값이 적용됩니다.
