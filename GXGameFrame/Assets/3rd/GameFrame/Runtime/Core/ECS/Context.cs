﻿using System.Collections.Generic;

namespace GameFrame
{
    public class Context : Entity,IStartSystem
    {
        private Dictionary<int, HashSet<ECSEntity>> m_ECSEnitiyGroup =new();
        private Dictionary<Matcher, Group> m_Groups = new ();
        private List<Group>[] m_GroupsList;
        
        public virtual void Start()
        {
            m_GroupsList = new List<Group>[GXComponents.ComponentTypes.Length];
        }
        public new T AddChild<T>() where T : ECSEntity
        {
            T ecsEntity = base.AddChild<T>();
            ecsEntity.SetContext(this);
            ChangeAddRomoveChildOrCompone(ecsEntity);
            return ecsEntity;
        }

        public  void RemoveChild(ECSEntity ecsEntity)
        {
            base.RemoveChild(ecsEntity);
            ChangeAddRomoveChildOrCompone(ecsEntity);
        }

        public void ChangeAddRomoveChildOrCompone(ECSEntity ecsEntity)
        {
            foreach (var group in m_Groups.Values)
            {
                Group gourp = group;
                gourp.HandleEntitySilently(ecsEntity);
            }
        }

        public Group GetGroup(Matcher matcher)
        {
            if (!m_Groups.TryGetValue(matcher, out Group grop))
            {
                grop = Group.CreateGroup(matcher);
                foreach (var item in Children)
                {
                    grop.HandleEntitySilently(item as ECSEntity);
                }
                m_Groups.Add(matcher, grop);
                foreach (var index in matcher.Indices)
                {
                    m_GroupsList[index] ??= new List<Group>();
                    m_GroupsList[index].Add(grop);
                }
            }
            return grop;
        }

        public void Reactive(int comid, ECSEntity entity)
        {
            var groupList = m_GroupsList[comid];
            if (groupList != null)
            {
                foreach (var t in groupList)
                {
                    t.HandleEntitySilently(entity);
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            foreach (var group in m_Groups)
            {
                //Matcher 匹配器其他的Context可能也会用到不需要删除.
                // Matcher.RemoveMatcher(group.Key);
                Group.RemoveGroup(group.Value);
            }
            m_Groups.Clear();
        }
        
    }
}