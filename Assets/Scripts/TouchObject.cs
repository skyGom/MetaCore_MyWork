using Dasverse.Aleo.InGame;
using UnityEngine;
using UnityEngine.EventSystems;

#if !UNITY_EDITOR
using System.Collections.Generic;
#endif

namespace Dasverse.Aleo.UI
{
    public class TouchObject : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(IsInputted() )
            {
                //OnCheckRay();
            }

            OnClearInput();
        }

        private bool isTouchEnded = true;

        private Vector3 vecInputPos;
        private Camera rayCamera; // Ray용 카메라(World Camera)
        private GameObject touchedObject;


        private float inputRayDistance = 500f;
        private float avatarRayDistance = 30f;
        private float signageRayDistance = 500f;

        public void Init()
        {
            //rayCamera = camera;
            rayCamera = GameObject.Find("Camera").GetComponent<Camera>(); // World Camera
        }

        public void OnInput()
        {
            isTouchEnded = true;
        }

        public void OffInput()
        {
            isTouchEnded = false;
            touchedObject = null;
        }

        private GameObject IsRayHit(float maxDistance)
        {
            vecInputPos = GetInputPosition();
            Ray ray = rayCamera.ScreenPointToRay(vecInputPos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
            {
                return hitInfo.collider.gameObject;
            }
            return null;
        }

        private bool IsInputted()
        {
            if (isTouchEnded == false)
                return false;

#if UNITY_EDITOR
            // 마우스 클릭 시
            if (Input.GetMouseButtonDown(0))
            {
                touchedObject = IsRayHit(inputRayDistance);
                if (touchedObject != null)
                {
                    return true;
                }
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //touchedObject = null;
                // vecInputPos = GetInputPosition();
                // Ray ray = rayCamera.ScreenPointToRay(vecInputPos);
                // if (Physics.Raycast(ray, out RaycastHit hitInfo, signageRayDistance))
                // {
                //     touchedObject = hitInfo.collider.gameObject;
                //     return true;
                // }
                // return false;


                touchedObject = IsRayHit(inputRayDistance);
                if (touchedObject != null)
                {
                    return true;
                }
            }
                
#endif
            return false;
        }

        private void OnClearInput()
        {
            if (touchedObject == null)
                return;

#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                OnCheckRay();
                touchedObject = null;
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                OnCheckRay();
                touchedObject = null;
            }
            else if(Input.touchCount == 0)
            {
                touchedObject = null;
            }
#endif 
            return;
        }

        private Vector3 GetInputPosition()
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#else
            return Input.GetTouch(0).position;
#endif
        }

        private void OnCheckRay()
        {
            if (IsPointerOverUIObject())
            {
                Debug.Log("Chat IsPointerOverUIObject true");
                return;
            }

            vecInputPos = GetInputPosition();
            Ray ray = rayCamera.ScreenPointToRay(vecInputPos);
            if (Physics.Raycast(ray, out RaycastHit hitPlayer, avatarRayDistance, LayerMask.GetMask("Avatar")))
            {
                if(OnTouchedOtherPlayer(hitPlayer))
                    return;

            }
            else if (Physics.Raycast(ray, out RaycastHit hitSignage, signageRayDistance))//, LayerMask.GetMask("Signage")))
            {
                if(OnTouchedSignage(hitSignage))
                    return;
            }
        }

        private bool OnTouchedOtherPlayer(RaycastHit hitInfo)
        {
            OtherPlayer otherPlayer = hitInfo.collider.gameObject.GetComponent<OtherPlayer>();
            if (otherPlayer != null && touchedObject == otherPlayer.gameObject)
            {
                Debug.Log($"On Touched OnTouchedOtherPlayer");
                otherPlayer.OnTouchedExecute();
                return true;
            }
            return false;
        }

        private bool OnTouchedSignage(RaycastHit hitInfo)
        {
            SignageObject signageObject = hitInfo.collider.gameObject.GetComponent<SignageObject>();
            if (signageObject != null && touchedObject == signageObject.gameObject)
            {
                Debug.Log($"On Touched OnTouchedSignage");
                signageObject.OnTouchedExecute();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 포인터 위치가 UI 위인지 체크는 함수로 
        /// 유니티 New Input System 사용시 기존에 제공하는
        /// IsPointerOverUIObject() 함수가 제대로 작동하지 않아 따로 만든 함수
        /// </summary>
        /// <returns>true면 포인터가 UI 위에 있다.</returns>
        private bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null)
                return true;

#if UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#else
            var touchPosition = UnityEngine.InputSystem.Touchscreen.current.position.ReadValue();
            var eventData = new PointerEventData(EventSystem.current) {position = touchPosition};
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
#endif
        }
    }

}
