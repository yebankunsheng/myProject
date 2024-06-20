using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class ShootCommand : AbstractCommand
    {
        private readonly Vector2 mShootDir;
        private readonly Vector2 mShootPos;

        public ShootCommand(Vector2 dir, Vector2 pos)
        {
            mShootDir = dir;
            mShootPos = pos;
        }

        protected override void OnExecute()
        {
            //AttackInput = false;
            this.GetSystem<IAudioMgrSystem>().PlaySound("�û�");

            // �첽�����ӵ�Ԥ���壬�����ӵ�������λ�ã������ӵ��ķ���
            this.GetSystem<IObjectPoolSystem>().Get("Item/Bullet", o =>
            {
                o.transform.localPosition = mShootPos;
                o.GetComponent<Bullet>().InitDir(mShootDir);
            });
        }
    }

    public class PlayerWeapon : Weapon
    {
        protected override void ShootFunc(float shootDir)
        {
            this.SendCommand(new ShootCommand(Vector2.right * shootDir, transform.position));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var item = other.GetComponent<BagItem>();
                if (item != null)
                {
                    this.GetSystem<IGunSystem>().GetGun(item.nameId, out GunInfo gun);
                    mGuns.Add(gun);
                    Destroy(other.gameObject);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                this.GetSystem<IAudioMgrSystem>().PlaySound("ǹ����");
                this.GetSystem<IGunSystem>().SwitchGun(true, mGuns);
            }
        }
    }
}

