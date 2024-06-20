using QFramework;
using UnityEngine;

namespace PlatformShoot
{
    public class Player : PlatformShootGameController, ICamTarget
    {
        private Rigidbody2D mRig;
        private BoxCollider2D mBoxColl;
        private LayerMask mGroundLayer;
        private PlayerWeapon mWeapon;

        private float mAccDelta = 0.6f;
        private float mDecDelta = 0.9f;
        private float mGroundMoveSpeed = 5f;
        private float mJumpForce = 12f;

        private int mFaceDir = 1;
        private bool isJumping;
        [SerializeField] private int mJumpCount;
        [SerializeField] private int mMaxJumpCount = 2;
        private IAudioMgrSystem mAudioMgr;
        private bool mGround;

        private PlayerInputHandle mInputHandle;
        Vector2 ICamTarget.Pos => transform.position;

        private void Awake()
        {
            this.SendCommand<InitGameCommand>();
            mWeapon = GetComponentInChildren<PlayerWeapon>();
            mInputHandle = GetComponent<PlayerInputHandle>();
        }
        private void Start()
        {
            mRig = GetComponent<Rigidbody2D>();
            mGroundLayer = LayerMask.GetMask("Ground");
            mBoxColl = GetComponent<BoxCollider2D>();
            this.GetSystem<ICameraSystem>().SetTarget(this);
            mAudioMgr = this.GetSystem<IAudioMgrSystem>();
            mAudioMgr.PlayBgm("打雷");
        }
        private void FixedUpdate()
        {
            if (mJumpCount == 0)
            {
                mInputHandle.JumpInput = false;
            }
            else if (mInputHandle.JumpInput)
            {
                mAudioMgr.PlaySound("跳跃");
                isJumping = true;
                mJumpCount--;
                mInputHandle.JumpInput = false;
                mRig.velocity = new Vector2(mRig.velocity.x, mJumpForce);
            }
            if (mInputHandle.InputX != 0)
            {
                if (mInputHandle.InputX != mFaceDir) Flip();
                mRig.velocity = new Vector2(Mathf.Clamp(mRig.velocity.x + mInputHandle.InputX * mAccDelta, -mGroundMoveSpeed, mGroundMoveSpeed), mRig.velocity.y);
            }
            else
            {
                mRig.velocity = new Vector2(Mathf.MoveTowards(mRig.velocity.x, 0, mDecDelta), mRig.velocity.y);
            }
        }
        private void Flip()
        {
            mFaceDir = ~mFaceDir + 1;
            transform.Rotate(0, 180, 0);
        }

        private void Update()
        {
            if (mInputHandle.AttackInput)
            {
                mWeapon.Shoot(mFaceDir);
            }
            mGround = Physics2D.OverlapBox(transform.position + mBoxColl.size.y * Vector3.down * 0.5f, Vector2.one * 0.1f, 0, mGroundLayer);// 检测地面
            if (mGround && isJumping)
            {
                mAudioMgr.PlaySound("落地");
                isJumping = false;
            } 
            if (mGround)
            {
                mJumpCount = mMaxJumpCount;
            }
        }
         
        private void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.CompareTag("Interactive"))
            {
                coll.GetComponent<IInteractiveItem>()?.Trigger();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (mBoxColl == null) return;
            Gizmos.DrawWireCube(transform.position + mBoxColl.size.y * Vector3.down * 0.5f, new Vector2(mBoxColl.size.x * 0.8f, 0.1f));
        }
    }
}

