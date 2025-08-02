using DCFApixels.DragonECS.Core;
using DCFApixels.DragonECS.EntLongExtensions.Internal;
using System;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace DCFApixels.DragonECS
{
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public static class EntLongBaseExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static entlong Delete(in this entlong self)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            var id = self.GetIDUnchecked();
            var worldid = self.GetWorldIDUnchecked();
            EcsWorld.GetWorld(worldid).DelEntity(id);
            return default;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static entlong Clone(in this entlong self)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            var world = self.GetWorldUnchecked();
            entlong result = world.NewEntityLong();
            world.CopyEntity(self.GetIDUnchecked(), result.GetIDUnchecked());
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static entlong Clone(in this entlong self, EcsWorld toWorld)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            entlong result = toWorld.NewEntityLong();
            self.GetWorldUnchecked().CopyEntity(self.GetIDUnchecked(), toWorld, result.GetIDUnchecked());
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatchesMask(in this entlong self, IComponentMask mask)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return self.GetWorldUnchecked().IsMatchesMask(mask, self.GetIDUnchecked());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMatchesMask(in this entlong self, EcsMask mask)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return self.GetWorldUnchecked().IsMatchesMask(mask, self.GetIDUnchecked());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetComponentsCount(in this entlong self)
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return self.GetWorldUnchecked().GetComponentsCount(self.GetIDUnchecked());
        }
    }

    #region EntLongPoolExtensions
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    public static class EntLongPoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static entlong Add<T>(in this entlong self, T component) where T : struct, IEcsComponent
        {
            self.Get<T>() = component;
            return self;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(in this entlong self) where T : struct, IEcsComponent
        {
            return ref self.Get<T>();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(in this entlong self) where T : struct, IEcsComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return EcsWorld.GetPoolInstance<EcsPool<T>>(self.GetWorldIDUnchecked()).Has(self.GetIDUnchecked());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Get<T>(in this entlong self) where T : struct, IEcsComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return ref EcsWorld.GetPoolInstance<EcsPool<T>>(self.GetWorldIDUnchecked()).TryAddOrGet(self.GetIDUnchecked());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Del<T>(in this entlong self) where T : struct, IEcsComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            EcsWorld.GetPoolInstance<EcsPool<T>>(self.GetWorldIDUnchecked()).TryDel(self.GetIDUnchecked());
        }
    }
    #endregion

    #region EntLongTagPoolExtensions
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
    internal static class EntLongTagPoolExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static entlong Add<T>(in this entlong self) where T : struct, IEcsTagComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            EcsWorld.GetPoolInstance<EcsTagPool<T>>(self.GetWorldIDUnchecked()).TryAdd(self.GetIDUnchecked());
            return self;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(in this entlong self, bool value) where T : struct, IEcsTagComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            EcsWorld.GetPoolInstance<EcsTagPool<T>>(self.GetWorldIDUnchecked()).Set(self.GetIDUnchecked(), value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle<T>(in this entlong self) where T : struct, IEcsTagComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            EcsWorld.GetPoolInstance<EcsTagPool<T>>(self.GetWorldIDUnchecked()).Toggle(self.GetIDUnchecked());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has<T>(in this entlong self) where T : struct, IEcsTagComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            return EcsWorld.GetPoolInstance<EcsTagPool<T>>(self.GetWorldIDUnchecked()).Has(self.GetIDUnchecked());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Del<T>(in this entlong self) where T : struct, IEcsTagComponent
        {
#if (DEBUG && !DISABLE_DEBUG) || DRAGONECS_STABILITY_MODE
            if (self.IsAlive == false) { Throw.Ent_ThrowIsNotAlive(self); }
#endif
            EcsWorld.GetPoolInstance<EcsTagPool<T>>(self.GetWorldIDUnchecked()).TryDel(self.GetIDUnchecked());
        }
    }
    #endregion
}

namespace DCFApixels.DragonECS.EntLongExtensions.Internal
{
    internal static class Throw
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void Ent_ThrowIsNotAlive(entlong entity)
        {
            if (entity.IsNull)
            {
                throw new InvalidOperationException($"The {entity} is null.");
            }
            else
            {
                throw new InvalidOperationException($"The {entity} is not alive.");
            }
        }
    }
}