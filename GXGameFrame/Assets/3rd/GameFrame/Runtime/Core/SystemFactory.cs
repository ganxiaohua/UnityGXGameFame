﻿namespace GameFrame
{
    public static class SystemFactory
    {
        public static void AddSystem<T>(this World entity) where T : class, ISystem
        {
            EntityHouse.Instance.AddSystem<T>(entity);
        }


        public static void RemoveSystem<T>(this World entity) where T : class, ISystem
        {
            EntityHouse.Instance.RemoveSystem<T>(entity);
        }
    }
}