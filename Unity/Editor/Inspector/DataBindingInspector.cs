// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UIElements;
// using System.Collections.Generic;
// 
// using Prota.Unity;
// using Prota.Editor;
// using UnityEditor.UIElements;
// using System.Linq;
// using NUnit.Framework;
// 
// namespace Prota.Editor
// {
//     [CustomEditor(typeof(DataBinding), false)]
//     public class DataBindingInspector : UpdateInspector
//     {
//         VisualElement list;
//         
//         public override VisualElement CreateInspectorGUI()
//         {
//             var root = new VisualElement() { name = "root" };
//             root.AddChild(new PropertyField(serializedObject.FindProperty("featureCharacter")))
//                 .AddChild(new ScrollView() { name = "scroll" }
//                     .VerticalScroll()
//                     .SetMaxHeight(400)
//                     .AddChild(list = new VisualElement() { name = "vis" })
//                 );
//             
//             return root;
//         }
//         
//         protected override void Update()
//         {
//             if(list == null) return;
//             var g = target as DataBinding;
//             var data = target.ProtaReflection().Get<List<DataBinding.Entry>>("data");
//             list.SyncData(data.Count,
//                 (i) => {
//                     var e = new VisualElement();
//                     e.SetHorizontalLayout();
//                     e.SetGrow();
//                     e.AddChild(new Label() { name = "i" }
//                         .SetWidth(200)
//                     );
//                     e.AddChild(new ObjectField() { name = "o" }
//                         .SetGrow()
//                     );
//                     return e;
//                 },
//                 (i, e) => {
//                     e.Q<Label>("i").text = data[i].name;
//                     e.Q<ObjectField>("o").value = data[i].target;
//                     e.SetVisible(true);
//                 },
//                 (i, e) => {
//                     e.SetVisible(false);
//                 }
//             );
//                 
//         }
//     }
// }
