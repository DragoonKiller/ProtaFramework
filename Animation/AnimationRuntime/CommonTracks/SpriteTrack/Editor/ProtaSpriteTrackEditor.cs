using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Animation;
using UnityEngine;
using System.Collections.Generic;
using Prota.Unity;

namespace Prota.Editor
{
    public partial class ProtaAnimationTrackEditor
    {
        [TrackEditor(typeof(ProtaAnimationTrack.Sprite))]
        public class ProtaSpriteTrackEditor : ProtaAnimationTrackEditor
        {
            bool setup = false;
            ObjectField spriteCollection;
            ProtaAnimationTrack.Sprite spriteTrack;
            Toggle useCull;
            Toggle useOffset;
            Toggle useGravity;
            Foldout showSpriteView;
            ScrollView spriteView;
            List<VisualElement> spriteViewList = new List<VisualElement>();
            
            VisualElement spriteTimeline;
            List<VisualElement> spriteTag = new List<VisualElement>();
            
            
            
            public override void UpdateTrackContent()
            {
                if(!setup)
                {
                    Setup();
                }
                UpdateAll();
            }
            
            
            void Setup()
            {
                setup = true;
                spriteTrack = track as ProtaAnimationTrack.Sprite;
                
                spriteCollection = new ObjectField();
                spriteCollection.objectType = typeof(ProtaSpriteCollection);
                spriteCollection.allowSceneObjects = true;
                spriteCollection.RegisterValueChangedCallback(e => {
                    spriteTrack.spriteAsset = e.newValue as ProtaSpriteCollection;
                    UpdateAll();
                });
                content.dataPanel.Add(spriteCollection);
                spriteCollection.value = spriteTrack.spriteAsset;
                
                var options = new VisualElement();
                options.style.flexDirection = FlexDirection.Row;
                options.SetGrow().SetHeight(18);
                content.dataPanel.Add(options);
                
                useCull = new Toggle();
                useCull.text = "区域裁剪";
                useCull.RegisterValueChangedCallback(e => UpdateAll());
                options.Add(useCull);
                
                
                useOffset = new Toggle();
                useOffset.text = "动画位移";
                useCull.RegisterValueChangedCallback(e => UpdateAll());
                options.Add(useOffset);
                
                var button = new Button() { name = "DeleteButton", text = "删除" };
                button.RegisterCallback<ClickEvent>(e => {
                    spriteTrack.RemoveAssign(time);
                    UpdateAll();
                });
                options.Add(button);
                
                
                showSpriteView = new Foldout();
                showSpriteView.text = "Sprite列表";
                showSpriteView.value = false;
                showSpriteView.SetHeight(18);
                showSpriteView.RegisterValueChangedCallback(e => {
                    UpdateAll();
                });
                content.dataPanel.Add(showSpriteView);
                
                spriteView = new ScrollView();
                spriteView.SetFixedSize();
                content.dataPanel.Add(spriteView);
                spriteView.SetVisible(false);
                
                
                spriteTimeline = new VisualElement();
                spriteTimeline.SetHeight(40).AutoWidth().SetGrow();
                content.trackContent.Add(spriteTimeline);
                spriteTimeline.MarkDirtyRepaint();
                
                var sep = new VisualElement().AsHorizontalSeperator(2, new Color(.2f, .2f, .2f, 1));
                content.trackContent.Add(sep);
                sep.MarkDirtyRepaint();
                
            }
            
            
            
            // 更新界面元素.
            void UpdateContent()
            {
                // Sprite 列表.
                if(spriteTrack.spriteAsset == null)
                {
                    showSpriteView.SetEnabled(false);
                    showSpriteView.value = false;
                }
                else
                {
                    showSpriteView.SetEnabled(true);
                    
                    spriteView.SetVisible(showSpriteView.value);
                    for(int _i = 0; _i < spriteTrack.spriteAsset.sprites.Count; _i++)
                    {
                        var i = _i;
                        if(!spriteViewList.TryGetValue(i, out var x))
                        {
                            x = new VisualElement();
                            x.style.flexDirection = FlexDirection.Row;
                            
                            var nameField = new TextField() { name = "Name" };
                            nameField.SetWidth(40);
                            nameField.pickingMode = PickingMode.Ignore;
                            x.Add(nameField);
                            
                            var insertButton = new Button() { name = "InsertButton", text = "插入" };
                            insertButton.SetWidth(50);
                            insertButton.RegisterCallback<ClickEvent>(e => {
                                spriteTrack.AddAssign(time, spriteTrack.spriteAsset.sprites[i].id);
                                UpdateAll();
                            });
                            x.Add(insertButton);
                            
                            var spriteField = new ObjectField() { name = "Sprite" };
                            spriteField.SetGrow();
                            spriteField.objectType = typeof(Sprite);
                            x.Add(spriteField);
                            
                            spriteViewList.Add(x);
                            spriteView.Add(x);
                        }
                        
                        x.Q<TextField>("Name").value = spriteTrack.spriteAsset.sprites[i].id;
                        x.Q<ObjectField>("Sprite").value = spriteTrack.spriteAsset.sprites[i].sprite;
                    }
                    
                    for(int i = spriteViewList.Count - 1; i >= spriteTrack.spriteAsset.sprites.Count; i--)
                    {
                        spriteView.Remove(spriteViewList[i]);
                        spriteViewList.RemoveAt(i);
                    }
                }
            }
            
            
            // 更新时间轴元素.
            void UpdateTrack()
            {
                // Sprite 列表.
                for(int _i = 0; _i < spriteTrack.records.Count; _i++)
                {
                    var i = _i;
                    Debug.Assert(i == 0 || spriteTrack.records[i - 1].time <= spriteTrack.records[i].time);
                    if(!spriteTag.TryGetValue(i, out var x))
                    {
                        x = new VisualElement() { name = "Sprite " + i };
                        x.SetHeight(spriteTimeline.style.height).SetAbsolute().SetGrow();
                        
                        var spriteImage = new VisualElement() { name = "SpriteImage" };
                        spriteImage.SetGrow().SetFixedSize().SetHeight(spriteTimeline.style.height).SetAbsolute();
                        spriteImage.style.backgroundColor = new Color(.9f, .9f, .9f, 1f);
                        x.Add(spriteImage);
                        
                        var stamp = new VisualElement() { name = "Stamp" };
                        stamp.style.backgroundColor = new Color(.6f, .2f, .2f, 1f);
                        stamp.SetFixedSize().SetAbsolute().SetWidth(2).SetHeight(spriteTimeline.style.height);
                        x.Add(stamp);
                        
                        spriteTimeline.Add(x);
                        spriteTag.Add(x);
                    }
                    
                    var assign = spriteTrack.records[i];
                    var sprite = spriteTrack.spriteAsset[assign.assetId];
                    x.SetWidth(spriteTimeline.style.height.value.value * sprite.rect.width / sprite.rect.height);
                    x.style.left = assign.time.XMap(timeFrom, timeTo, 0, displayWidth);
                    
                    var xSpriteImage = x.Q("SpriteImage");
                    x.Q("SpriteImage").style.backgroundImage = new StyleBackground(sprite);
                    xSpriteImage.SetWidth(x.style.width);
                    
                    // Sprite 按照顺序放到最后.
                    x.BringToFront();
                }
                
                for(int i = spriteTag.Count - 1; i >= spriteTrack.records.Count; i--)
                {
                    spriteTimeline.Remove(spriteTag[i]);
                    spriteTag.RemoveAt(i);
                }
                
                content.timeStamp.BringToFront();
                
                
                
            }
            
            // 更新场景内元素.
            void UpdateScene()
            {
                
            }
            
            void UpdateAll()
            {
                UpdateContent();
                UpdateTrack();
                UpdateScene();
            }
        }
    }
}