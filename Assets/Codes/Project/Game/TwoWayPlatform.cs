using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformShoot
{
    public class TwoWayPlatform : MonoBehaviour
    {
        private Collider2D mPlatform;
        private HashSet<Collider2D> mTargets;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (mTargets.Add(other.collider))
            {
                Debug.Log($"添加{other.collider}对象");
            }
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (mTargets.Remove(other.collider))
            {
                Debug.Log($"移除{other.collider}对象");
            }
        }
        private void Start()
        {
            mPlatform = GetComponent<Collider2D>();
            mTargets = new();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (mTargets == null) return;
                Fall(GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>());
            }
        }
        private void Fall(Collider2D coll)
        {
            if (mTargets.Contains(coll))
            {
                StartCoroutine(_Fall(coll));
            }
        }
        private IEnumerator _Fall(Collider2D coll)
        {
            Physics2D.IgnoreCollision(coll, mPlatform, true);
            yield return new WaitForSeconds(0.3f);
            Physics2D.IgnoreCollision(coll, mPlatform, false);
        }
    }
}
