## ProtaTweening

类似 DOTween, 但是有如下区别:
1. tweening 尽可能避免 GC, 使用数组而不是 class 对象池.
2. 显式指定 Tween 操作的对象和功能, 给对象新建一个 Tween 会把前面相同功能的 Tween 顶掉.
3. 提供 GameObject/LifeSpan 生命周期绑定功能. 不使用 try-catch 来判定 Tween 是否合法 (这样会掩盖逻辑问题).
4. 区分正常结束的 Tween 和非正常结束的 Tween, 提供不一样的回调接口.
5. 提供关于 tween 各种属性的查询能力, 并且大多数属性支持在 Tween 执行时修改.






## 自定义用法示例:

```C#
using UnityEngine;
using Prota;
using Prota.Unity;
using Prota.Tween;

public class DynamicSprite : MonoBehaviour
{
    public Vector3 minScale = Vector3.one;
    public Vector3 maxScale = Vector3.one * 1.2f;
    public Vector3 pivot = Vector3.zero;
    public float tweenTime = 1f;
    
    TweenHandle tween;
    void OnValidate()
    {
        this.transform.localScale = minScale;
        if(UnityEngine.Application.isPlaying && !tween.isNone) tween.Start(tweenTime.Max(0.001f));
    }
    
    void Start()
    {
        this.transform.localScale = minScale;
        
        tween = this.NewTween(TweenId.None, (h, t) => {
            var self = h.target as DynamicSprite;
            self.transform.localScale = (self.minScale, self.maxScale).Lerp(h.Evaluate(t));
        }).SetFromTo(0, 1).SetEase(TweenEase.quadInOut).SetLoop(true).Start(tweenTime.Max(0.001f));
        
        tween.onFinish = h => h.SetReverse();
    }
}

```