using DCFApixels.DragonECS.SimpleSyntax.Delegates;
using System;
using System.Collections.Generic;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace DCFApixels.DragonECS
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    internal static class EntLongIterationExtensions
    {
        #region ExcMaskCache
        private static class Exc<T0>
        {
            public readonly static EcsStaticMask m = EcsStaticMask.Exc<T0>().Build();
        }
        private static class Exc<T0, T1>
        {
            public readonly static EcsStaticMask m = EcsStaticMask.Exc<T0>().Exc<T1>().Build();
        }
        private static class Exc<T0, T1, T2>
        {
            public readonly static EcsStaticMask m = EcsStaticMask.Exc<T0>().Exc<T1>().Exc<T2>().Build();
        }
        private static class Exc<T0, T1, T2, T3>
        {
            public readonly static EcsStaticMask m = EcsStaticMask.Exc<T0>().Exc<T1>().Exc<T2>().Exc<T3>().Build();
        }
        #endregion

        #region ForEachBuilder
        public readonly struct ForEachBuilder : IDisposable
        {
            private readonly Stack<List<EcsStaticMask>> _maskListsPool;
            internal readonly EcsWorld _world;
            internal readonly List<EcsStaticMask> _masks;
            internal ForEachBuilder(EcsWorld world, EcsStaticMask mask)
            {
                _maskListsPool = new Stack<List<EcsStaticMask>>();
                _world = world;
                if (_maskListsPool.TryPeek(out _masks) == false)
                {
                    _masks = new List<EcsStaticMask>();
                }
                _masks.Add(mask);
            }
            internal ForEachBuilder(ForEachBuilder h, EcsStaticMask mask)
            {
                _maskListsPool = new Stack<List<EcsStaticMask>>();
                _world = h._world;
                _masks = h._masks;
                _masks.Add(mask);
            }
            public void Dispose()
            {
                _masks.Clear();
                _maskListsPool.Push(_masks);
            }
            public EcsMask BuildMaskAndDispose()
            {
                EcsMask result = _masks[0].ToMask(_world);
                for (int i = 1; i < _masks.Count; i++)
                {
                    result = result + _masks[i];
                }
                Dispose();
                return result;
            }
        }
        #endregion

        #region Without
        public static ForEachBuilder Without<T0>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Exc<T0>.m);
        }
        public static ForEachBuilder Without<T0, T1>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Exc<T0, T1>.m);
        }
        public static ForEachBuilder Without<T0, T1, T2>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Exc<T0, T1, T2>.m);
        }
        public static ForEachBuilder Without<T0, T1, T2, T3>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Exc<T0, T1, T2, T3>.m);
        }

        public static ForEachBuilder Without<T0>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Exc<T0>.m);
        }
        public static ForEachBuilder Without<T0, T1>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Exc<T0, T1>.m);
        }
        public static ForEachBuilder Without<T0, T1, T2>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Exc<T0, T1, T2>.m);
        }
        public static ForEachBuilder Without<T0, T1, T2, T3>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Exc<T0, T1, T2, T3>.m);
        }
        #endregion

        #region With
        public static ForEachBuilder With<T0>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Inc<T0>.m);
        }
        public static ForEachBuilder With<T0, T1>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Inc<T0, T1>.m);
        }
        public static ForEachBuilder With<T0, T1, T2>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Inc<T0, T1, T2>.m);
        }
        public static ForEachBuilder With<T0, T1, T2, T3>(this EcsWorld world)
        {
            return new ForEachBuilder(world, Inc<T0, T1, T2, T3>.m);
        }

        public static ForEachBuilder With<T0>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Inc<T0>.m);
        }
        public static ForEachBuilder With<T0, T1>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Inc<T0, T1>.m);
        }
        public static ForEachBuilder With<T0, T1, T2>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Inc<T0, T1, T2>.m);
        }
        public static ForEachBuilder With<T0, T1, T2, T3>(this ForEachBuilder h)
        {
            return new ForEachBuilder(h, Inc<T0, T1, T2, T3>.m);
        }
        #endregion

        #region ForEach
        public static void ForEach<T0>(this EcsWorld world, Lambda<T0> lambda)
            where T0 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0>(this ForEachBuilder h, Lambda<T0> lambda)
            where T0 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e));
            }
        }

        public static void ForEach<T0, T1>(this EcsWorld world, Lambda<T0, T1> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1>(this ForEachBuilder h, Lambda<T0, T1> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2>(this EcsWorld world, Lambda<T0, T1, T2> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2>(this ForEachBuilder h, Lambda<T0, T1, T2> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2, T3>(this EcsWorld world, Lambda<T0, T1, T2, T3> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2, T3>(this ForEachBuilder h, Lambda<T0, T1, T2, T3> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            var pool3 = h._world.GetPoolInstance<EcsPool<T3>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e),
                    ref pool3.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2, T3, T4>(this EcsWorld world, Lambda<T0, T1, T2, T3, T4> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2, T3, T4>(this ForEachBuilder h, Lambda<T0, T1, T2, T3, T4> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            var pool3 = h._world.GetPoolInstance<EcsPool<T3>>();
            var pool4 = h._world.GetPoolInstance<EcsPool<T4>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e),
                    ref pool3.Get(e),
                    ref pool4.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2, T3, T4, T5>(this EcsWorld world, Lambda<T0, T1, T2, T3, T4, T5> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2, T3, T4, T5>(this ForEachBuilder h, Lambda<T0, T1, T2, T3, T4, T5> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            var pool3 = h._world.GetPoolInstance<EcsPool<T3>>();
            var pool4 = h._world.GetPoolInstance<EcsPool<T4>>();
            var pool5 = h._world.GetPoolInstance<EcsPool<T5>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e),
                    ref pool3.Get(e),
                    ref pool4.Get(e),
                    ref pool5.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2, T3, T4, T5, T6>(this EcsWorld world, Lambda<T0, T1, T2, T3, T4, T5, T6> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
            where T6 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2, T3, T4, T5, T6>(this ForEachBuilder h, Lambda<T0, T1, T2, T3, T4, T5, T6> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
            where T6 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            var pool3 = h._world.GetPoolInstance<EcsPool<T3>>();
            var pool4 = h._world.GetPoolInstance<EcsPool<T4>>();
            var pool5 = h._world.GetPoolInstance<EcsPool<T5>>();
            var pool6 = h._world.GetPoolInstance<EcsPool<T6>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e),
                    ref pool3.Get(e),
                    ref pool4.Get(e),
                    ref pool5.Get(e),
                    ref pool6.Get(e));
            }
        }

        public static void ForEach<T0, T1, T2, T3, T4, T5, T6, T7>(this EcsWorld world, Lambda<T0, T1, T2, T3, T4, T5, T6, T7> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
            where T6 : struct, IEcsComponent
            where T7 : struct, IEcsComponent
        {
            new ForEachBuilder(world, EcsStaticMask.Empty).ForEach(lambda);
        }
        public static void ForEach<T0, T1, T2, T3, T4, T5, T6, T7>(this ForEachBuilder h, Lambda<T0, T1, T2, T3, T4, T5, T6, T7> lambda)
            where T0 : struct, IEcsComponent
            where T1 : struct, IEcsComponent
            where T2 : struct, IEcsComponent
            where T3 : struct, IEcsComponent
            where T4 : struct, IEcsComponent
            where T5 : struct, IEcsComponent
            where T6 : struct, IEcsComponent
            where T7 : struct, IEcsComponent
        {
            var pool0 = h._world.GetPoolInstance<EcsPool<T0>>();
            var pool1 = h._world.GetPoolInstance<EcsPool<T1>>();
            var pool2 = h._world.GetPoolInstance<EcsPool<T2>>();
            var pool3 = h._world.GetPoolInstance<EcsPool<T3>>();
            var pool4 = h._world.GetPoolInstance<EcsPool<T4>>();
            var pool5 = h._world.GetPoolInstance<EcsPool<T5>>();
            var pool6 = h._world.GetPoolInstance<EcsPool<T6>>();
            var pool7 = h._world.GetPoolInstance<EcsPool<T7>>();
            foreach (var e in h._world.Where(h.BuildMaskAndDispose()))
            {
                lambda((e, h._world),
                    ref pool0.Get(e),
                    ref pool1.Get(e),
                    ref pool2.Get(e),
                    ref pool3.Get(e),
                    ref pool4.Get(e),
                    ref pool5.Get(e),
                    ref pool6.Get(e),
                    ref pool7.Get(e));
            }
        }
    }
    #endregion
}

#region Delegates
namespace DCFApixels.DragonECS.SimpleSyntax.Delegates
{
    public delegate void Lambda<T0>(
        entlong e,
        ref T0 t0);
    public delegate void Lambda<T0, T1>(
        entlong e,
        ref T0 t0,
        ref T1 t1);
    public delegate void Lambda<T0, T1, T2>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2);
    public delegate void Lambda<T0, T1, T2, T3>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2,
        ref T3 t3);
    public delegate void Lambda<T0, T1, T2, T3, T4>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2,
        ref T3 t3,
        ref T4 t4);
    public delegate void Lambda<T0, T1, T2, T3, T4, T5>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2,
        ref T3 t3,
        ref T4 t4,
        ref T5 t5);
    public delegate void Lambda<T0, T1, T2, T3, T4, T5, T6>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2,
        ref T3 t3,
        ref T4 t4,
        ref T5 t5,
        ref T6 t6);
    public delegate void Lambda<T0, T1, T2, T3, T4, T5, T6, T7>(
        entlong e,
        ref T0 t0,
        ref T1 t1,
        ref T2 t2,
        ref T3 t3,
        ref T4 t4,
        ref T5 t5,
        ref T6 t6,
        ref T7 t7);
}
#endregion