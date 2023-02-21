﻿using System;
using System.Collections.Generic;

namespace GameFrame
{
    public interface IECSComponent : IReference
    {
    }

    /// <summary>
    /// ECSEntity挂载的一定是Context
    /// </summary>
    public abstract class ECSEntity : IEntity
    {
        public IEntity ComponentParent { get; set; }
        public int ID { get; set; }
        public IEntity.EntityStatus m_EntityStatus { get; set; }

        private Dictionary<Type, IECSComponent> m_ECSComponents;
        public Dictionary<Type, IECSComponent> ECSComponents => m_ECSComponents;

        private List<int> TypeHashCode;

        protected ECSEntity()
        {
            m_ECSComponents = new();
            TypeHashCode = new(8);
        }

        // public abstract void InitComponent();

        public T AddComponent<T>() where T : class, IECSComponent
        {
            Type type = typeof(T);

            if (this.m_ECSComponents != null && this.m_ECSComponents.ContainsKey(type))
            {
                throw new Exception($"entity already has component: {type.FullName}");
            }

            IECSComponent entity = ReferencePool.Acquire(type) as IECSComponent;
            TypeHashCode.Add(type.GetHashCode());
            m_ECSComponents.Add(type, entity);
            (ComponentParent as Context).ChangeAddRomoveChildOrCompone(this);
            return entity as T;
        }

        public void RemoveComponent<T>() where T : class, IECSComponent
        {
            Type type = typeof(T);
            if (!m_ECSComponents.TryGetValue(type, out IECSComponent entity))
            {
                throw new Exception($"entity not already  component: {type.FullName}");
            }

            m_ECSComponents.Remove(type);
            TypeHashCode.Remove(type.GetHashCode());
            ReferencePool.Release(entity);
            (ComponentParent as Context).ChangeAddRomoveChildOrCompone(this);
        }

        public T GetComponent<T>() where T : class, IECSComponent
        {
            Type type = typeof(T);
            IECSComponent value = null;
            if (this.m_ECSComponents != null && !this.m_ECSComponents.TryGetValue(type, out value))
            {
                return null;
            }

            return value as T;
        }

        /// <summary>
        /// 全部包含
        /// </summary>
        /// <param name="hascode"></param>
        /// <returns></returns>
        public bool HasComponents(int[] hascode)
        {
            for (int index = 0; index < hascode.Length; ++index)
            {
                if (!TypeHashCode.Contains(hascode[index]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 包含任意一个
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public bool HasAnyComponent(int[] indices)
        {
            for (int index = 0; index < indices.Length; ++index)
            {
                if (TypeHashCode.Contains(indices[index]))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Initialize()
        {
        }

        public void ClearAllComponent()
        {
            foreach (var item in m_ECSComponents)
            {
                ReferencePool.Release(item.Value);
            }
            m_ECSComponents.Clear();
            TypeHashCode.Clear();
            (ComponentParent as Context).ChangeAddRomoveChildOrCompone(this);
        }

        public void Clear()
        {
            ClearAllComponent();
        }
    }
}