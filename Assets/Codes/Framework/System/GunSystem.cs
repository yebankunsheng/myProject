using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlatformShoot
{
    public interface IGunSystem : ISystem
    {
        void SwitchGun(bool isPositive, LoopList<GunInfo> infos);
        bool GetGun(string name, out GunInfo gun);
        GunInfo[] GetAll();
    }

    [System.Serializable]
    public class GunInfo
    {
        /// <summary>
        /// name: 枪的名字
        /// remainCount: 弹夹中剩余子弹的数量
        /// capacity: 弹夹的容量
        /// consumptionCount: 每一次发射子弹的数量，可设置一发两发三发等
        /// frequency: 射击频率
        /// reloadTime: 装弹时间
        /// </summary>
        public string name;
        public int remainCount;
        public int capacity;
        public int consumptionCount;
        public float frequency;
        public float reloadTime;

        public GunInfo(GunInfo info) : this(info.name, info.capacity, info.frequency, info.reloadTime, info.consumptionCount) { }

        public GunInfo(string name, int capacity, float frequency, float reloadTime, int consumptionCount)
        {
            this.name = name;
            this.capacity = capacity;
            this.frequency = frequency;
            this.reloadTime = reloadTime;
            this.consumptionCount = consumptionCount;
            Reload();
        }
        public void Reload() => remainCount = capacity;
        public bool Shoot()
        {
            if (remainCount >= consumptionCount)
            {
                remainCount -= consumptionCount;
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class LoopList<T> : IEnumerable<T>
    {
        [SerializeField] private List<T> mItems;
        [SerializeField] private int vernier;
        public T Current => mItems[vernier];
        
        public LoopList(int capacity)
        {
            mItems = new(capacity);
            vernier = 0;
        }
        public LoopList(IEnumerable<T> items)
        {
            mItems = items.ToList();
        }
        public LoopList() : this(2) { }
        public void Add(T t) => mItems.Add(t);
        public void LoopPos() => vernier = (vernier + 1) % mItems.Count;
        public void LoopNeg() => vernier = (vernier + mItems.Count - 1) % mItems.Count;
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return default;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return default;
        }
    }

    public class GunSystem : AbstractSystem, IGunSystem
    {
        private Dictionary<string, GunInfo> mGuns;
        protected override void OnInit()
        {
            mGuns = new()
            {
                { "手枪", new GunInfo("手枪", 10, 0.5f, 1.5f, 1) },
                { "机枪", new GunInfo("机枪", 40, 0.2f, 2.5f, 1) } 
            };
        }

        GunInfo[] IGunSystem.GetAll()
        {
            var guns = new GunInfo[mGuns.Count];
            int index = 0;
            foreach (var gun in mGuns.Values)
            {
                guns[index++] = gun;
            }
            return guns;
        }

        bool IGunSystem.GetGun(string name, out GunInfo gun)
        {
            if (mGuns.TryGetValue(name, out gun))
            {
                gun = new GunInfo(gun);
                return true;
            }
            return false;
        }

        void IGunSystem.SwitchGun(bool isPositive, LoopList<GunInfo> infos)
        {
            if (isPositive) infos.LoopPos();
            else infos.LoopNeg();
        }
    }
}
