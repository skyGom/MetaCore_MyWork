using Dasverse.Framework;
using DG.Tweening;
using UnityEngine;

namespace Dasverse.Aleo
{
    public class Crystal : MonoBehaviour
    {   
        /// <summary>
        /// 폭탄용 무시 효과
        /// </summary>
        public GameObject IgnoreImage;

        private int crystalID;
        private string crystalName;
        private int type;
        private int size;
        private int hitPoint;
        private CircleCollider2D circleCollider2D;

        public int CrystalID => crystalID;
        public string CrystalName => crystalName;
        public int Type => type;
        public int Size => size;
        public int HitPoint => hitPoint;
        public bool IsCritical => isCritical;

        private float ScaleCal(int num)
        {
            if (num == 1)
            {
                return 1f;
            }
            else
            {
                return (float)(1 + (0.125 * (num - 1)));
            }
        }

        private float StartForce = 10f;
        private bool isCritical;

        private Rigidbody2D rb;
        private Ease[] randomEasingArr = { Ease.InSine, Ease.OutSine, Ease.InQuad, Ease.OutQuad };

        public void SetCrystalProperty(CrystalPropertyRawData crystalProperty)
        {
            crystalID = crystalProperty.id;
            crystalName = crystalProperty.name;
            type = crystalProperty.type;
            size = crystalProperty.size;
            hitPoint = crystalProperty.hitpoint;
            circleCollider2D = GetComponent<CircleCollider2D>();
        }

        public void LaunchCrystal(int power, bool isCritical = false)
        {
            Transform child = transform.GetChild(0).transform;

            if (crystalID < 113)
            {
                Transform outLine = child.GetChild(0).transform;
                outLine.gameObject.SetActive(false);

                if (isCritical)
                {
                    this.isCritical = isCritical;
                    outLine.gameObject.SetActive(true);
                }
            }
            else
            {
                this.isCritical = false;
            }

            float scale = ScaleCal(size);
            transform.localScale = new Vector3(scale, scale, scale);

            rb = GetComponent<Rigidbody2D>();
            rb.simulated = true;
            rb.gravityScale = 1;
            gameObject.SetActive(true);

            rb.AddForce(transform.up * (StartForce + power), ForceMode2D.Impulse);
            AudioManager<MineHeroSFX>.Play(MineHeroSFX.Minehero_Objectdrop);

            if (child != null)
            {
                Ease randomEasing = randomEasingArr[Random.Range(0, randomEasingArr.Length)];
                child.DOLocalRotate(new Vector3(Random.Range(-1080, 1080), 90, 90), 5f, RotateMode.FastBeyond360).SetEase(randomEasing).SetRelative(true);
            }
        }

        void Update()
        {
            if (crystalID == 116 && MineHeroGameMain.Instance.IsIgnoreBomb)
            {
                circleCollider2D.enabled = false;
                IgnoreImage?.SetActive(true);
                IgnoreImage.transform.rotation = Quaternion.identity;
            }
            else if (crystalID == 116 && !MineHeroGameMain.Instance.IsIgnoreBomb)
            {
                circleCollider2D.enabled = true;
                IgnoreImage?.SetActive(false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (MineHeroTimeManager.Instance.IsGameStart || MineHeroGameMain.Instance.IsFinishMode)
            {
                if (collision.gameObject.CompareTag("Blade"))
                {
                    switch (crystalID)
                    {
                        case 113:
                            MineHeroGameMain.Instance.BarrierActive();
                            break;
                        case 114:
                            MineHeroPointManager.Instance.DoubleChace();
                            break;
                        case 115:
                            CrystalSpawner.Instance.SpacialSpawnStart();
                            break;
                        case 116:
#if ENABLE_DEV
                            if (Spawner.Instance.AutoPlayData.Values[(int)GameType.MineHero] > 0)
                                return;
#endif                      
                            if(MineHeroGameMain.Instance.IsIgnoreBomb)
                                return;

                            rb.simulated = false;
                            rb.gravityScale = 0;
                            rb.velocity = Vector2.zero;
                            rb.angularVelocity = 0;
                            CrystalSpawner.Instance.ReturnAllCrystalAsync().Forget();
                            MineHeroPointManager.Instance.LostPoint(transform.position, crystalID);
                            return;
                    }

                    MineHeroPointManager.Instance.Hit(transform, crystalID, crystalID, isCritical);
                    CrystalSpawner.Instance.ReturnPool(this, true);
                }

                if (collision.gameObject.CompareTag("Barrier"))
                {
                    if (crystalID == 116)
                        return;

                    Vector2 incomingVector = transform.position - collision.transform.position;

                    var dir = Vector2.Reflect(incomingVector, collision.contacts[0].normal);

                    rb.gravityScale = 0;
                    rb.velocity = dir;
                }
            }
        }
    }
}
