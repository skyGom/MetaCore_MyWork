using Dasverse.Framework;
using UnityEngine;

namespace Dasverse.Aleo
{
    // 자르는 방향을 알기 위한 부분 현재 주석 처리

    public class Blade : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem trailParticlePrefab;

        //private Vector3 currentPosition;
        //private Vector3 previousPosition;
        private Vector3 touchPosition;
        private Rigidbody2D rb;
        private CircleCollider2D circleCollider;
        private Camera cam;

        //[HideInInspector]
        //public Vector3 Dir;
        [HideInInspector]
        public bool IsCutting = false;

        void Start()
        {
            cam = Camera.main;
            rb = GetComponent<Rigidbody2D>();

            circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.enabled = false;

            trailParticlePrefab.gameObject.SetActive(false);
            trailParticlePrefab.GetComponent<Renderer>().sortingOrder = 10;
        }

        private void FixedUpdate()
        {
            if (Input.touchCount > 0)
            {
                Touch firstTouch = Input.GetTouch(0);

                switch (firstTouch.phase)
                {
                    case TouchPhase.Began:
                        startCutting();
                        touchPosition = cam.ScreenToWorldPoint(firstTouch.position);
                        break;
                    case TouchPhase.Moved:
                        IsCutting = true;
                        circleCollider.enabled = true;
                        touchPosition = cam.ScreenToWorldPoint(firstTouch.position);

                        //previousPosition = firstTouch.position - firstTouch.deltaPosition;
                        //currentPosition = firstTouch.position;

                        //Dir = currentPosition - previousPosition;
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        stopCutting();
                        break;
                    case TouchPhase.Canceled:
                        stopCutting();
                        break;
                    default:
                        break;
                }
            }
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch firstTouch = Input.GetTouch(0);

                switch (firstTouch.phase)
                {
                    case TouchPhase.Began:
#if UNITY_IOS
                        transform.position = touchPosition;
#else
                        rb.position = touchPosition;
#endif
                        break;
                    case TouchPhase.Moved:
#if UNITY_IOS
                        transform.position = touchPosition;
#else
                        rb.position = touchPosition;
#endif

                        if (!trailParticlePrefab.isPlaying)
                        {
                            trailParticlePrefab.gameObject.SetActive(true);
                            trailParticlePrefab.Play();
                        }
                        break;
                    case TouchPhase.Ended:
                        stopCutting();
                        break;
                    case TouchPhase.Canceled:
                        stopCutting();
                        break;
                    default:
                        break;
                }
            }
        }

        private void startCutting()
        {
            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_Swipe);
        }

        private void stopCutting()
        {
            IsCutting = false;
            circleCollider.enabled = false;
            touchPosition = Vector3.zero;
            trailParticlePrefab.Stop();
        }
    }
}