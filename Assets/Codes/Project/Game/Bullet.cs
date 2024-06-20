using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class Bullet : PlatformShootGameController
    {
        private LayerMask mLayerMask;

        private Vector2 bulletDir;

        private float mBulletSpeed = 20f;

        private Timer mTimer;

        public void Awake()
        {
            mLayerMask = LayerMask.GetMask("Ground", "Trigger", "Enemy");
        }
        private void OnEnable()
        {
            mTimer = this.GetSystem<ITimerSystem>().AddTimer(3f, () =>
            {
                this.GetSystem<IObjectPoolSystem>().Recovery(gameObject);
            });
        }
        private void OnDisable()
        {
            mTimer.Stop();
        }
        public void InitDir(Vector2 dir)
        {
            bulletDir = dir.normalized;
        }
        private void Update()
        {
            transform.Translate(mBulletSpeed * Time.deltaTime * bulletDir);
        }
        private void FixedUpdate()
        {
            var coll = Physics2D.OverlapBox(transform.position, transform.localScale, 0, mLayerMask);
            if (coll)
            {
                if (coll.CompareTag("Enemy"))
                {
                    Destroy(coll.gameObject);
                    this.GetSystem<IAudioMgrSystem>().PlaySound("¹Ö2");
                }
                else if (coll.CompareTag("Trigger"))
                {
                    Destroy(coll.gameObject);
                    this.SendCommand<ShowPassDoorCommand>();
                    this.GetSystem<IAudioMgrSystem>().PlaySound("¹Ö2");
                }
                else
                {
                    this.GetSystem<IAudioMgrSystem>().PlaySound("½Å²½");
                }
                this.GetSystem<IObjectPoolSystem>().Recovery(gameObject);
            }
        }
    }
}

