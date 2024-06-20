using UnityEngine;

namespace PlatformShoot
{
    public class Enemy : MonoBehaviour
    {
        private Rigidbody2D mRig;
        private int mFaceDir = 1;
        private float mSpeed = 5f;
        private void Start()
        {
            mRig = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var wallHit = Physics2D.Raycast(transform.position, new Vector2(mFaceDir, 0), 1, LayerMask.GetMask("Ground"));
            if (wallHit)
            {
                Flip();
            }
            
            var hit = Physics2D.Raycast(transform.position + new Vector3(mFaceDir, 0), Vector2.down, 1, LayerMask.GetMask("Ground"));

            Debug.DrawLine(transform.position + new Vector3(mFaceDir, 0), transform.position + new Vector3(mFaceDir, 0) + Vector3.down, hit ? Color.blue : Color.red);

            if (hit.collider)
            {
                Debug.Log("检测到地面");
                mRig.velocity = new Vector2(mSpeed * mFaceDir, mRig.velocity.y);
            }
            else
            {
                Debug.Log("未检测到地面");
                Flip();
            }
        }

        private void Flip()
        {
            int dir = mRig.velocity.x > 0 ? -1 : 1;
            if (dir != mFaceDir)
            {
                mFaceDir = dir;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}