## 混合模式速查文档

### 混合方程

```
finalValue = srcFactor * src op dstFactor * dstValue
```

假设我想把给定颜色
```
src = (srcR, srcG, srcB, srcA)
```
画到
```
dst = (dstR, dstG, dstB, dstA)
```
上.
则目标颜色如下:

```
xR = srcFactorR * srcR op dstFactorR * dstR
xG = srcFactorG * srcG op dstFactorG * dstG
xB = srcFactorB * srcB op dstFactorB * dstB
xA = srcFactorA * srcA op dstFactorA * dstA
x = (xR, xG, xB, xA)
```


其中 `op` 为如下之一:
```
Add: 相加
Sub: src 减去 dst
RevSub: dst 减去 src
Min: 最小值
Max: 最大值
```


srcFactor 和 dstFactor 为如下之一:
```
One: 1
Zero: 0
SrcColor: src 对应的 R/G/B/A 值, 每个颜色通道都一一对应.
SrcAlpha: src 的 Alpha 值, 每个颜色通道取同一个 Alpha 值.
SrcAlphaSaturate: 取 min((1 - srcAlpha), (1 - dstAlpha))
DstColor: dst 对应的 R/G/B/A 值, 每个颜色通道都一一对应.
DstAlpha: dst 的 Alpha 值, 每个颜色通道取同一个 Alpha 值.
OneMinusSrcColor: 1 - dst 对应的 R/G/B/A 值, 每个颜色通道都一一对应.
OneMinusSrcAlpha: 1 - dst 的 Alpha 值, 每个颜色通道取同一个 Alpha 值.
OneMinusDstColor: 1 - dst 对应的 R/G/B/A 值, 每个颜色通道都一一对应.
OneMinusDstAlpha: 1 - dst 的 Alpha 值, 每个颜色通道取同一个 Alpha 值.
```
srcFactor 和 dstFactor 都有对应的四个通道. 如果指定 Alpha 类型的值, 那么四个通道值都相同; 如果指定颜色类型的值, 那么四个通道分别问颜色RGBA值.

如果分开指定 srcColorFactor 和 srcAlphaFactor, Factor 会由各自指定的值进行组合.

## 混合模式

[参考链接1](
https://stackoverflow.com/questions/11164000/blending-mode-formula-for-hard-light-soft-light-color-dodge-color-burn)

[参考链接2](
https://docs.krita.org/en/reference_manual/blending_modes/arithmetic.html#bm-addition)



下面是混合模式的公式列表, 默认规则如下:
* op 默认是 Add.
* 比大小都是逐通道来比较大小.
* * src 和 dst 都是指颜色, alpha 通常要特殊处理.
```python
# 正常 Normal:
srcFactor = srcA
dstFactor = 1 - srcA

# 正片叠底(相乘) Multiply:
srcFactor = dstColor
dstFactor = 0

# 相加 Addition:
srcFactor = 1
dstFactor = 1

# 线性减淡(变亮) Linear Dodge:
srcFactor = 1
dstFactor = 1

# 滤色 Screen:
x = 1 - (1 - src) (1 - dst) = src + dst - src * dst = src * (1 - dst) + dst
srcFactor = 1 - dst
dstFactor = 1

# 溶解 Dissolve:
根据 srcA 和 dstA 的比值, 随机选取 src 和 dst 值.
无法通过混合模式实现.

# 变亮 Lighten:
srcFactor = 1
dstFactor = 1
op = Max

# 变暗 Darken:
srcFactor = 1
dstFactor = 1
op = Min

# 颜色减淡 Color Dodge:
x = dst / (1 - src) = dst - dst / src
混合参数不支持除法, 无法通过混合模式实现.

可以在 Fragment Shader 中将输出改变为 src = 1 / src,
此时 x = dst - src * dst = dst (1 - src), 并使用如下参数实现颜色减淡:
srcFactor = 0
dstFactor = 1 - src

# 颜色加深 Color Burn:
x = 1 - (1 - ((1 - dst) / src)) = 1 / src - dst / src
混合参数不支持除法, 无法通过混合模式实现.

可以在 Fragment Shader 中将输出改变为 src = 1 / src,
此时 x = src * (1 - dst), 并使用如下参数实现颜色加深:
srcFactor = 1 - dst
dstFactor = 0

# 硬光 Hard Light:
if src > 0.5:
    srcFactor = dst
    dstFactor = 0
else:
    srcFactor = 1 - dst
    dstFactor = 1
由于混合不支持对RGB通道分别做处理, 因而无法通过混合模式实现.

# 叠加 Overlay:
if src > 0.5:
    srcFactor = 1 - dst
    dstFactor = 1
else:
    srcFactor = dst
    dstFactor = 0
由于混合不支持对RGB通道分别做处理, 无法通过混合模式实现.

```

混合模式的强度, 即带有混合模式图层的"透明度",
是图层混合完毕后的颜色 x 带着透明度通道, 再一次与原色 dst 做**正常(Normal)混合**.
```
xFactor = xA
dstFactor = 1 - xA
```
如果用 Shader 的 Blend 功能做混合, 那么一些混合模式的强度控制就不正确了.


