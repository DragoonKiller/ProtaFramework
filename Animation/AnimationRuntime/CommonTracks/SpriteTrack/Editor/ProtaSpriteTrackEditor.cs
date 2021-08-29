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
                options.style.flexGrow = 1;
                options.style.flexShrink = 0;
                options.style.maxHeight = options.style.minHeight = options.style.height = 18;
                content.dataPanel.Add(options);
                
                useCull = new Toggle();
                useCull.text = "区域裁剪";
                useCull.RegisterValueChangedCallback(e => UpdateAll());
                options.Add(useCull);
                
                
                useOffset = new Toggle();
                useOffset.text = "动画位移";
                useCull.RegisterValueChangedCallback(e => UpdateAll());
                options.Add(useOffset);
                
                showSpriteView = new Foldout();
                showSpriteView.text = "Sprite列表";
                showSpriteView.value = false;
                showSpriteView.style.maxHeight = showSpriteView.style.minHeight = showSpriteView.style.height = 18;
                showSpriteView.RegisterValueChangedCallback(e => {
                    UpdateAll();
                });
                content.dataPanel.Add(showSpriteView);
                
                spriteView = new ScrollView();
                spriteView.style.flexShrink = 0;
                spriteView.style.flexGrow = 0;
                content.dataPanel.Add(spriteView);
                spriteView.visible = false;
                
                
                spriteTimeline = new VisualElement();
                spriteTimeline.style.height = spriteTimeline.style.minHeight = spriteTimeline.style.maxHeight = 40;
                spriteTimeline.style.width = spriteTimeline.style.minWidth = spriteTimeline.style.maxWidth = new StyleLength() { keyword = StyleKeyword.Auto };
                spriteTimeline.style.flexGrow = 1;
                spriteTimeline.style.flexShrink = 0;
                content.trackContent.Add(spriteTimeline);
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
                    
                    spriteView.visible = showSpriteView.value;
                    for(int _i = 0; _i < spriteTrack.spriteAsset.sprites.Count; _i++)
                    {
                        var i = _i;
                        if(!spriteViewList.TryGetValue(i, out var x))
                        {
                            x = new VisualElement();
                            x.style.flexDirection = FlexDirection.Row;
                            
                            var nameField = new TextField() { name = "Name" };
                            nameField.style.width = nameField.style.minWidth = nameField.style.maxWidth = 40;
                            nameField.pickingMode = PickingMode.Ignore;
                            x.Add(nameField);
                            
                            var insertButton = new Button() { name = "InsertButton", text = "插入" };
                            insertButton.style.width = insertButton.style.minWidth = insertButton.style.maxWidth = 50;
                            insertButton.RegisterCallback<ClickEvent>(e => {
                                spriteTrack.AddAssign(time, spriteTrack.spriteAsset.sprites[i].id);
                                UpdateAll();
                            });
                            x.Add(insertButton);
                            
                            var spriteField = new ObjectField() { name = "Sprite" };
                            // spriteField.style.width = spriteField.style.minWidth = spriteField.style.maxWidth = 120;
                            spriteField.style.flexGrow = 1;
                            spriteField.style.flexShrink = 0;
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
                for(int _i = 0; _i < spriteTrack.assign.Count; _i++)
                {
                    var i = _i;
                    Debug.Assert(i == 0 || spriteTrack.assign[i - 1].time < spriteTrack.assign[i].time);
                    if(!spriteTag.TryGetValue(i, out var x))
                    {
                        x = new VisualElement() { name = "Sprite " + i };
                        x.style.height = x.style.minHeight = x.style.maxHeight = spriteTimeline.style.height;
                        x.style.position = Position.Absolute;
                        x.style.top = 0;
                        x.style.flexShrink = 0;
                        x.style.flexGrow = 1;
                        
                        var spriteBackground = new VisualElement() { name = "SpriteBackground" };
                        spriteBackground.style.flexShrink = 0;
                        spriteBackground.style.flexGrow = 1;
                        spriteBackground.style.height = spriteBackground.style.minHeight = spriteBackground.style.maxHeight = spriteTimeline.style.height;
                        spriteBackground.style.position = Position.Absolute;
                        spriteBackground.style.left = 0;
                        spriteBackground.style.top = 0;
                        spriteBackground.style.backgroundColor = new Color(.4f, .4f, .4f, 1);
                        x.Add(spriteBackground);
                        
                        var spriteImage = new VisualElement() { name = "SpriteImage" };
                        spriteImage.style.flexShrink = 0;
                        spriteImage.style.flexGrow = 1;
                        spriteImage.style.height = spriteImage.style.minHeight = spriteImage.style.maxHeight = spriteTimeline.style.height;
                        spriteImage.style.position = Position.Absolute;
                        spriteImage.style.left = 0;
                        spriteImage.style.top = 0;
                        x.Add(spriteImage);
                        
                        var stamp = new VisualElement() { name = "Stamp" };
                        stamp.style.backgroundColor = new Color(.6f, .2f, .2f, 1f);
                        stamp.style.flexShrink = 0;
                        stamp.style.flexGrow = 1;
                        stamp.style.width = stamp.style.minWidth = stamp.style.maxWidth = 2;
                        stamp.style.height = stamp.style.minHeight = stamp.style.maxHeight = spriteTimeline.style.height;
                        stamp.style.position = Position.Absolute;
                        stamp.style.left = 0;
                        stamp.style.top = 0;
                        x.Add(stamp);
                        
                        spriteTimeline.Add(x);
                        spriteTag.Add(x);
                    }
                    
                    var assign = spriteTrack.assign[i];
                    var sprite = spriteTrack.spriteAsset[assign.assetId];
                    x.style.width = x.style.minWidth = x.style.maxWidth = spriteTimeline.style.height.value.value * sprite.rect.width / sprite.rect.height;
                    x.style.left = assign.time.XMap(timeFrom, timeTo, 0, displayWidth);
                    
                    var xSpriteImage = x.Q("SpriteImage");
                    x.Q("SpriteImage").style.backgroundImage = new StyleBackground(sprite);
                    xSpriteImage.style.width = xSpriteImage.style.minWidth = xSpriteImage.style.maxWidth = x.style.width;
                    
                    content.timeStamp.BringToFront();
                }
                
                for(int i = spriteTag.Count - 1; i >= spriteTrack.assign.Count; i--)
                {
                    spriteTimeline.Remove(spriteTag[i]);
                    spriteTag.RemoveAt(i);
                }
                
                
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