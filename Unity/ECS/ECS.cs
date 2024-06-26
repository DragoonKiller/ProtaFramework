using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using System.Reflection;

namespace Prota.Unity
{
    public sealed class ECS : Singleton<ECS>
    {
        
        readonly static List<Type> systemTypes = new List<Type>();
        readonly static List<Type> subSystemTypes = new List<Type>();
        
        static ECS()
        {
            systemTypes.AddRange(Prota.ProtaReflection.GetTypesDerivedFrom<ESystem>().Where(x => !x.IsAbstract));
            subSystemTypes.AddRange(Prota.ProtaReflection.GetTypesDerivedFrom<ESubSystem>().Where(x => !x.IsAbstract));
        }
        
        
        // ====================================================================================================
        // 公共接口.
        // ====================================================================================================
        
        static bool _isInFixedUpdate = false;
        public static bool isInFixedUpdate
        {
            get
            {
                if(Physics2D.simulationMode != SimulationMode2D.Script)
                    throw new NotSupportedException("Physics update is controlled by Unity.");
                return _isInFixedUpdate;
            }
            private set => _isInFixedUpdate = value;
        }
        
        
        public static int fixedUpdateFrame { get; private set; }
        
        public static int updateFrame { get; private set; }
        
        public static int lateUpdateFrame { get; private set; }
        
        public static bool isInUpdate { get; private set; }
        
        public static bool isInLateUpdate { get; private set; }
        
        
        public static float time
        {
            get
            {
                if(isInFixedUpdate) return ECS.instance.physicsTimerScaled;
                if(isInUpdate) return Time.time;
                if(isInLateUpdate) return Time.time;
                throw new InvalidOperationException("ECS.time can only be accessed in update or fixed update of ESystem.");
            }
        }
        
        public static float timeUnscaled
        {
            get
            {
                if(isInFixedUpdate) return ECS.instance.physicsTimer;
                if(isInUpdate) return Time.unscaledTime;
                if(isInLateUpdate) return Time.unscaledTime;
                throw new InvalidOperationException("ECS.timeUnscaled can only be accessed in update or fixed update of ESystem.");
            }
        }
        
        public static float realtime => Time.realtimeSinceStartup;
        
        // dt 的值会根据当前逻辑在 Update 还是 FixedUpdate 中而变化.
        public static float dt
        {
            get
            {
                if(isInFixedUpdate) return ECS.instance.physiscDeltaTimeScaled;
                if(isInUpdate) return Time.deltaTime;
                if(isInLateUpdate) return Time.deltaTime;
                throw new InvalidOperationException("ECS.dt can only be accessed in update process of ESystem.");
            }
        }
        
        public static float dtUnsacled
        {
            get
            {
                if(isInFixedUpdate) return ECS.instance.physicsDeltaTime;
                if(isInUpdate) return Time.unscaledDeltaTime;
                if(isInLateUpdate) return Time.unscaledDeltaTime;
                throw new InvalidOperationException("ECS.dtUnsacle can only be accessed in update process of ESystem.");
            }
        }
        
        // 物理模拟时长缩放. 一次 fixedUpdate 若耗时 dt, 告知业务系统的时间将为 dt * fixedTimeScale.
        // ** 只在物理模拟模式为 manually 的情况下生效.
        public static float fixedTimeScale = 1;
        
        public static readonly AsyncControl asyncControl = new AsyncControl();
        public static readonly AsyncControl physicsAsyncControl = new AsyncControl();
        public static readonly AsyncControl lateAsyncControl = new AsyncControl();
        
        public static bool stopPhysicsUpdate = false;
        
        public static async Task Timer(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.asyncControl.Wait(seconds, token);
        }
        
        public static async Task TimerPhysics(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.physicsAsyncControl.Wait(seconds, token);
        }
        
        public static async Task TimerLate(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.lateAsyncControl.Wait(seconds, token);
        }
        
        public static async Task TimerRealtime(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.asyncControl.WaitRealtime(seconds, token);
        }
        
        public static async Task TimerPhysicsRealtime(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.physicsAsyncControl.WaitRealtime(seconds, token);
        }
        
        public static async Task TimerLateRealtime(float seconds, TaskCanceller.Token? token = null)
        {
            await ECS.lateAsyncControl.WaitRealtime(seconds, token);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [SerializeField, Readonly] float physicsTimer = 0f;
        [SerializeField, Readonly] float physicsDeltaTime = 0f;
        [SerializeField, Readonly] float physicsTimerScaled = 0f;
        [SerializeField, Readonly] float physiscDeltaTimeScaled = 0f;
        [SerializeField, Readonly] float maxFixedDeltaTime = 0f;
        
        [SerializeReference, ReferenceEditor(flat = true)] List<ESystem> systems = new List<ESystem>();
        
        void OnValidate()
        {
            maxFixedDeltaTime = Time.fixedDeltaTime;
            
            if(Application.isPlaying) return;
            InitSystems();
        }
        
        protected override void Awake()
        {
            base.Awake();
            InitSystems();
            fixedTimeScale = 1;
        }
        
        void FixedUpdate()
        {
            if(Physics2D.simulationMode == SimulationMode2D.Script) return;
            if(stopPhysicsUpdate) throw new InvalidOperationException("Physics update cannot be stopped when controlled by Unity.");
            fixedUpdateFrame++;
            foreach(var i in systems) i.InvokeFixedUpdate();
            physicsAsyncControl.Step();
        }
        
        void LateUpdate()
        {
            isInLateUpdate = true;
            lateUpdateFrame++;
            lateAsyncControl.Step();
            foreach(var i in systems) i.InvokeLateUpdate();
            isInLateUpdate = false;
        }
        
        void Update()
        {
            if(Physics2D.simulationMode == SimulationMode2D.Script && !stopPhysicsUpdate)
            {
                // 保证每次 Update 至少会执行一次 FixedUpdate.
                var dt = maxFixedDeltaTime;
                while(dt > Time.deltaTime) dt /= 2;
                physicsDeltaTime = dt;
                physiscDeltaTimeScaled = physicsDeltaTime * fixedTimeScale;
                
                (dt != 0).Assert();
                
                Time.fixedDeltaTime = physiscDeltaTimeScaled;
                
                // 每帧最多 1000 次 fixed update.
                for(int i = 0; i < 1000 && Time.time - physicsTimer >= physicsDeltaTime; i++)
                {
                    physicsTimer += physicsDeltaTime;
                    physicsTimerScaled += physiscDeltaTimeScaled;
                    isInFixedUpdate = true;
                    Physics2D.Simulate(physiscDeltaTimeScaled);
                    fixedUpdateFrame += 1;
                    physicsAsyncControl.Step();
                    foreach(var s in systems) s.InvokeFixedUpdate();
                    isInFixedUpdate = false;
                }
            }
            
            isInUpdate = true;
            updateFrame++;
            asyncControl.Step();
            foreach(var i in systems) i.InvokeUpdate();
            isInUpdate = false;
        }
        
        public void InitSystems()
        {
            systems.RemoveAll(x => x == null);
            using var _h = TempHashSet.Get<Type>(out var types);
            types.AddRange(systemTypes);
            foreach(var i in systems) types.Remove(i.GetType());
            foreach(var ss in types)
            {
                var s = Activator.CreateInstance(ss) as ESystem;
                s.ProtaReflection().SetAs<ESystem>("name", s.GetType().Name);
                systems.Add(s);
            }
            
            if(Application.isPlaying)
            {
                using var _ = systems.ToTempDict(x => x.GetType(), x => x, out var systemTable);
                using var __ = subSystemTypes.Select(x => Activator.CreateInstance(x) as ESubSystem).ToTempList(out var subSystems);
                foreach(var subsystem in subSystems)
                {
                    var t = subsystem.systemType;
                    if(!systemTable.ContainsKey(t)) throw new InvalidOperationException($"Cannot find system {t} for subsystem {subsystem.GetType()}.");
                    subsystem.system = systemTable[t];
                    subsystem.system.subSystems.Add(subsystem);
                }
            }
            
            if(Application.isPlaying) foreach(var i in systems) i.OnSystemCreate();
            
            ReorderSystems();
        }
        
        public void ReorderSystems()
        {
            // 根据 ESystemDataProvider, ESystemDataAccessor, ESystemDataClearer 获取各个 System 的依赖关系.
            // 顺序是: DataProvider -> DataAccessor -> DataClearer.
            
            // 分 Component 处理.
            var _ = TempDict.Get<Type, List<(ESystem sys, int order)>>(out var d);
            foreach(var sys in systems)
            {
                var sysType = sys.GetType();
                var provider = sysType.GetCustomAttribute<ESystemDataProvider>();
                var accessor = sysType.GetCustomAttribute<ESystemDataAccessor>();
                var clearer = sysType.GetCustomAttribute<ESystemDataClearer>();
                var providerTypes = provider?.types ?? Enumerable.Empty<Type>();
                var accessorTypes = accessor?.types ?? Enumerable.Empty<Type>();
                var clearerTypes = clearer?.types ?? Enumerable.Empty<Type>();
                if(providerTypes.Where(x => !typeof(EComponent).IsAssignableFrom(x)).Any())
                    throw new InvalidOperationException($"Invalid ESystemDataProvider on {sysType}.");
                if(accessorTypes.Where(x => !typeof(EComponent).IsAssignableFrom(x)).Any())
                    throw new InvalidOperationException($"Invalid ESystemDataAccessor on {sysType}.");
                if(clearerTypes.Where(x => !typeof(EComponent).IsAssignableFrom(x)).Any())
                    throw new InvalidOperationException($"Invalid ESystemDataClearer on {sysType}.");
                foreach(var i in providerTypes) d.GetOrCreate(i).Add((sys, 0));
                foreach(var i in accessorTypes) d.GetOrCreate(i).Add((sys, 1));
                foreach(var i in clearerTypes) d.GetOrCreate(i).Add((sys, 2));
            }
            
            // 记录每个 System 必须在谁之前, 谁之后.
            var graph = new Graph<ESystem, Nothing.Struct>();
            foreach(var s in systems) graph.AddNode(s);
            foreach(var (type, list) in d)
            {
                list.Sort((a, b) => a.order.CompareTo(b.order));
                var c0 = list.Where(x => x.order == 0).Select(x => x.sys).ToArray();
                var c1 = list.Where(x => x.order == 1).Select(x => x.sys).ToArray();
                var c2 = list.Where(x => x.order == 2).Select(x => x.sys).ToArray();
                foreach(var i in c0) foreach(var j in c1) graph.AddEdge(i, j);
                foreach(var i in c1) foreach(var j in c2) graph.AddEdge(i, j);
                foreach(var i in c0) foreach(var j in c2) graph.AddEdge(i, j);
            }
            
            // 拓扑排序.
            var sort = graph.Toposort();
            sort.Execute();
            
            // 获取排序结果.
            if(sort.queue.Count != systems.Count)
            {
                systems.ToStringJoined().LogError();
                sort.queue.ToStringJoined().LogError();
                throw new Exception(sort.queue.Count.ToString() + " " + systems.Count.ToString());
            }
            
            for(int i = 0; i < systems.Count; i++) systems[i] = sort.queue[i];
        }
    }
    
    // ====================================================================================================
    // ====================================================================================================
    // ====================================================================================================
    // ====================================================================================================
    
    
    public static partial class ECSMethodExtensions
    {
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static bool TryFindEntityComponent<T>(this GameObject g, out ERoot root, out T c) where T: EComponent
        {
            c = null;
            if (!g.EntityRoot().PassValue(out root)) return false;
            return root.TryGetEntityComponent(out c);
        }
        
        public static bool TryFindEntityComponent<T>(this GameObject g, out T c) where T: EComponent
        {
            c = null;
            if (!g.EntityRoot().PassValue(out var root)) return false;
            return root.TryGetEntityComponent(out c);
        }
        
        public static bool TryFindEntityComponent<T>(this Component x, out ERoot root, out T c) where T: EComponent
        {
            c = null;
            if (!x.EntityRoot().PassValue(out root)) return false;
            return root.TryGetEntityComponent(out c);
        }
        
        public static bool TryFindEntityComponent<T>(this Component x, out T c) where T: EComponent
        {
            Debug.Assert(x);
            c = null;
            if (!x.EntityRoot().PassValue(out var root)) return false;
            return root.TryGetEntityComponent(out c);
        }
        
        
        public static ERoot EntityRoot(this GameObject go)
        {
            var tr = go.transform;
            while(tr != null)
            {
                if(ERoot.entityMap.TryGetValue(tr.gameObject, out var res))
                    return res;
                tr = tr.parent;
            }
            
            return go.GetComponentInParent<ERoot>();
        }
            
        public static ERoot EntityRoot(this Component g)
            => g.gameObject.EntityRoot();
        
        public static bool EntityRoot(this Component g, out ERoot root)
        {
            root = g.gameObject.EntityRoot();
            return root != null;
        }
        
        public static ERoot EntityRoot(this EComponent g)
        {
            return g.entity;
        }
        
        public static bool EntityRoot(this EComponent g, out ERoot root)
        {
            root = g.entity;
            return root != null;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static Task Wait(this AsyncControl control, float seconds, TaskCanceller.Token? token = null)
        {
            if(ECS.instance == null) throw new InvalidOperationException("Cannot use this in non-ECS mode.");
            return control.Wait(seconds, () => ECS.dt, token);
        }
        
        public static Task WaitRealtime(this AsyncControl control, float seconds, TaskCanceller.Token? token = null)
        {
            if(ECS.instance == null) throw new InvalidOperationException("Cannot use this in non-ECS mode.");
            var currentTime = ECS.realtime;
            return control.Wait(seconds, () => {
                var res = ECS.realtime - currentTime;
                currentTime = ECS.realtime;
                return res;
            }, token);
        }
        
        public static async Task Wait(this AsyncControl control, float seconds, LifeSpan life)
        {
            while(life.alive && seconds > 0)
            {
                await control;
                seconds -= ECS.dt;
            }
        }
        
        public static async Task WaitRealtime(this AsyncControl control, float seconds, LifeSpan life)
        {
            while(life.alive && seconds > 0)
            {
                await control;
                seconds -= ECS.realtime;
            }
        }
    }
    
}



