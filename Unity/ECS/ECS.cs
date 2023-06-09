using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Threading.Tasks;
using UnityEditor;

namespace Prota.Unity
{
    [RequireComponent(typeof(ESystem))]
    public sealed class ECS : Singleton<ECS>
    {
        
        readonly static List<Type> systemTypes = new List<Type>();
        
        static ECS()
        {
            systemTypes.AddRange(Prota.ProtaReflection.GetTypesDerivedFrom<ESystem>().Where(x => !x.IsAbstract));
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
                if(isInFixedUpdate) return Time.fixedTime;
                if(isInUpdate) return Time.time;
                throw new InvalidOperationException("ECS.time can only be accessed in update or fixed update of ESystem.");
            }
        }
        
        public static float realtime
        {
            get => Time.realtimeSinceStartup;
        }
        
        // dt 的值会根据当前逻辑在 Update 还是 FixedUpdate 中而变化.
        public static float dt
        {
            get
            {
                if(isInFixedUpdate) return Time.fixedDeltaTime;
                if(isInUpdate) return Time.deltaTime;
                if(isInLateUpdate) return Time.deltaTime;
                throw new InvalidOperationException("ECS.dt can only be accessed in update process of ESystem.");
            }
        }
        
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
        [SerializeField, Readonly] float maxFixedDeltaTime = 0f;
        
        [SerializeReference] List<ESystem> systems = new List<ESystem>();
        
        void OnValidate()
        {
            maxFixedDeltaTime = Time.fixedDeltaTime;
            
            InitSystems();
        }
        
        void Awake()
        {
            OnValidate();
        }
        
        void FixedUpdate()
        {
            if(Physics2D.simulationMode == SimulationMode2D.Script) return;
            if(stopPhysicsUpdate) throw new InvalidOperationException("Physics update cannot be stopped when controlled by Unity.");
            fixedUpdateFrame++;
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
                Time.fixedDeltaTime = dt;
                
                while(Time.time - physicsTimer >= Time.fixedDeltaTime)
                {
                    physicsTimer += Time.fixedDeltaTime;
                    Physics2D.Simulate(Time.fixedDeltaTime);
                    isInFixedUpdate = true;
                    foreach(var i in systems) i.InvokeFixedUpdate();
                    isInFixedUpdate = false;
                }
            }
            
            isInUpdate = true;
            updateFrame++;
            asyncControl.Step();
            foreach(var i in systems) i.InvokeUpdate();
            isInUpdate = false;
        }
        
        public void CheckSystemOrder()
        {
            using var _ = TempList<ESystem>.Get(out var stack);
            stack.AddRange(systems);
            for(int i = 0; i < stack.Count; i++)
            {
                var cur = stack[i].ProtaReflection().type;
                var before = cur.GetTypeAttribute<ESystemBefore>();
                var after = cur.GetTypeAttribute<ESystemAfter>();
                
                if(before != null)
                {
                    var rest = before.types.ToList();
                    
                    rest.RemoveAll(x => stack.Skip(i).Any(y => y.GetType() == x));
                    
                    foreach(var s in rest)
                        Debug.LogError($"ECS: System { cur.name } 需要在 { s.Name } 之前.");
                }
                
                if(after != null)
                {
                    var rest = after.types.ToList();
                    
                    rest.RemoveAll(x => stack.Take(i).Any(y => y.GetType() == x));
                    
                    foreach(var s in rest)
                        Debug.LogError($"ECS: System { cur.name } 需要在 { s.Name } 之后.");
                }
            }
            
            var duplicated = stack.GroupBy(x => x.GetType()).Where(x => x.Count() > 1).Select(x => x.Key);
            foreach(var s in duplicated)
                if(s.ProtaReflection().GetTypeAttribute<ESystemAllowDuplicate>() == null)
                    Debug.LogError($"ECS: System { s.Name } 重复.");
        }
        
        public void InitSystems()
        {
            systems.RemoveAll(x => x == null);
            using var _h = TempHashSet<Type>.Get(out var types);
            types.AddRange(systemTypes);
            foreach(var i in systems) types.Remove(i.GetType());
            foreach(var ss in types)
            {
                var s = Activator.CreateInstance(ss) as ESystem;
                s.ProtaReflection().SetAs<ESystem>("name", s.GetType().Name);
                systems.Add(s);
            }
            
            CheckSystemOrder();
        }
    }
    
    
    public static partial class ECSMethodExtensions
    {
        public static ERoot EntityRoot(this GameObject go)
            => go.GetComponentInParent<ERoot>();
            
        public static ERoot EntityRoot(this Component g)
            => g.GetComponentInParent<ERoot>();
        
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
    }
    
}



