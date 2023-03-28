在 URP 3D 里实现 URP 2D 的光照模式.

(1) 在 URP Renderer Asset 中添加一个 Light Render Feature.
(2) 给这个 Light Render Feature 添加一个材质, 该材质使用的 shader 是 LightToScreenShader.
(3) 在 URP Render Asset 中添加一个 Full Screen Pass Render Feature.
(4) 将刚才的材质拖进新建的 Render Feature.
(5) InjectionPoint 选 Before Rendering Post Processing.
(6) 在场景里放一个 空的 GameObject.
(7) 给它挂载 SpriteRenderer 脚本.
(8) 材质选 2DLightAdd 或 2DLightMult.
(8) 此时可以正常看到光照效果了.
