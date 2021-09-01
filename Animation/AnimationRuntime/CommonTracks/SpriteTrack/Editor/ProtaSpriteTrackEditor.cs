using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Animation;
using UnityEngine;
using System.Collections.Generic;
using Prota.Unity;
using System.Linq;

namespace Prota.Editor
{
    public partial class ProtaAnimationTrackEditor
    {
        [TrackEditor(typeof(ProtaAnimationTrack.Sprite))]
        public class ProtaSpriteTrackEditor : ProtaAnimationTrackEditor
        {
            const int spriteSize = 60;
            
            
            bool setup = false;
            ObjectField spriteCollection;
            ProtaAnimationTrack.Sprite spriteTrack;
            Toggle useCull;
            Toggle useOffset;
            Toggle useGravity;
            
            Tab mode;
            
            
            bool showSpriteView;
            VisualElement spriteView;
            ScrollView spriteScroll;
            Toggle fixedFramerate;
            IntegerField framerate;
            
            
            bool showCullView;
            VisualElement cullView;
            
            
            
            bool showOffsetView;
            VisualElement offsetView;
            
            
            
            
            DragState dragSprite = new DragState();
            int currentDragging = -1;
            
            List<VisualElement> spriteScrollList = new List<VisualElement>();
            
            VisualElement spriteTimeline;
            List<VisualElement> spriteTag = new List<VisualElement>();
            
            
            VisualElement timeTagRoot;
            List<VisualElement> tiemTag = new List<VisualElement>();
            
            
            
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
                
                mode = new Tab() { name = "Tab" }.SetParent(content.dataPanel);
                mode.AddTab("Sprites", "图片");
                mode.AddTab("Cull", "裁剪");
                mode.AddTab("Offset", "偏移");
                mode.RegisterCallback<SelectTabEvent>(e => {
                    showSpriteView = e.name == "Sprites";
                    showCullView = e.name == "Cull";
                    showOffsetView = e.name == "Offset";
                    UpdateAll();
                });
                mode.currentSelect = "Sprites";
                
                // SpriteView.
                {
                    spriteView = new VisualElement().SetGrow().SetParent(content.dataPanel);
                    content.dataPanel.Add(spriteView);
                    
                    //options bar
                    {
                        var options = new VisualElement().SetParent(spriteView);
                        options.style.flexDirection = FlexDirection.Row;
                        options.SetGrow().SetHeight(18);
                        
                        var button = new Button() { name = "DeleteButton", text = "删除" }.SetParent(options);
                        button.RegisterCallback<ClickEvent>(e => {
                            spriteTrack.RemoveAssign(time);
                            UpdateAll();
                        });
                        
                        new VisualElement().AsVerticalSeperator(2, new Color(.1f, .1f, .1f, 1)).SetParent(options);
                        
                        var label = new Label() { text = "定帧" }.SetParent(options).SetPadding(5, 2, 0, 0);
                        fixedFramerate = new Toggle() { name = "FixedFramerate", text = "" }.SetParent(options);
                        fixedFramerate.value = false;
                        fixedFramerate.RegisterValueChangedCallback(e => {
                            if(e.previousValue == false) framerate.value = 30;
                            else framerate.value = Mathf.Max(1, framerate.value);
                            UpdateAll();
                        });
                        
                        framerate = new IntegerField(){ name = "Framerate" }.SetParent(options).SetWidth(spriteSize);
                        framerate.value = 1;
                        framerate.RegisterValueChangedCallback(e => {
                            framerate.value = Mathf.Max(1, framerate.value);
                            UpdateAll();
                        });
                    }
                    
                    // sprite list
                    {
                        spriteScroll = new ScrollView().SetParent(spriteView);
                        spriteScroll.VerticalScroll().SetGrow();
                        
                        spriteCollection = new ObjectField().SetParent(spriteScroll);
                        spriteCollection.objectType = typeof(ProtaSpriteCollection);
                        spriteCollection.allowSceneObjects = true;
                        spriteCollection.RegisterValueChangedCallback(e => {
                            spriteTrack.spriteAsset = e.newValue as ProtaSpriteCollection;
                            UpdateAll();
                        });
                        spriteCollection.value = spriteTrack.spriteAsset;
                    }
                    
                    spriteView.SetVisible(true);
                }
                
                // CullView
                {
                    useCull = new Toggle(){ text = "区域裁剪" }.SetParent(content.dataPanel);
                    useCull.RegisterValueChangedCallback(e => UpdateAll());
                    useCull.SetVisible(false);
                }
                
                
                
                // Sprite Timeline
                {
                    spriteTimeline = new VisualElement().SetParent(content.trackContent);
                    spriteTimeline.SetHeight(spriteSize).AutoWidth().SetGrow();
                    dragSprite.Register(spriteTimeline);
                    void GetIndex()
                    {
                        currentDragging = spriteTag.FindIndex(x => {
                            var (from, to) = (x.style.left.value.value, x.style.width.value.value);
                            to += from;
                            return dragSprite.now.x.Within(from, to);
                        });
                    }
                    dragSprite.canDrag = () => {
                        GetIndex();
                        return currentDragging != -1;
                    };
                    dragSprite.onDragBegin += () => {
                        GetIndex();
                        UpdateAll();
                    };
                    dragSprite.onDrag += UpdateAll;
                    dragSprite.onDragEnd += () => {
                        var originAssign = spriteTrack.records[currentDragging];
                        spriteTrack.AddAssign(GetSpriteDisplayTime(currentDragging), originAssign.assetId);
                        spriteTrack.RemoveAssign(originAssign.time);
                        currentDragging = -1;
                        UpdateAll();
                    };
                    
                }
                
                new VisualElement().AsHorizontalSeperator(2, new Color(.2f, .2f, .2f, 1)).SetParent(content.trackContent);
                
                // Time Tag
                {
                    var timeTagRoot = new VisualElement().SetParent(content.trackContent);
                    timeTagRoot.SetHeight(spriteSize).SetColor(Color.black.A(0)).SetGrow();
                }
                
                new VisualElement().AsHorizontalSeperator(2, new Color(.2f, .2f, .2f, 1)).SetParent(content.trackContent);
            }
            
            
            
            // 更新界面元素.
            void UpdateContent()
            {
                // SpriteView.
                framerate.SetVisible(fixedFramerate.value);
                spriteView.SetVisible(showSpriteView);
                for(int _i = 0; _i < spriteTrack.spriteAsset.sprites.Count; _i++)
                {
                    var i = _i;
                    if(!spriteScrollList.TryGetValue(i, out var x))
                    {
                        x = new VisualElement();
                        x.style.flexDirection = FlexDirection.Row;
                        
                        var nameField = new TextField() { name = "Name" };
                        nameField.SetWidth(spriteSize).SetNoInteraction();
                        x.Add(nameField);
                        
                        var insertButton = new Button() { name = "InsertButton", text = "插入" };
                        insertButton.SetWidth(50);
                        insertButton.RegisterCallback<ClickEvent>(e => {
                            spriteTrack.AddAssign(time, spriteTrack.spriteAsset.sprites[i].id);
                            UpdateAll();
                        });
                        x.Add(insertButton);
                        
                        var spriteField = new ObjectField() { name = "Sprite" };
                        spriteField.SetTargetType<Sprite>().SetGrow();
                        x.Add(spriteField);
                        
                        spriteScrollList.Add(x);
                        spriteScroll.Add(x);
                    }
                    
                    x.Q<TextField>("Name").value = spriteTrack.spriteAsset.sprites[i].id;
                    x.Q<ObjectField>("Sprite").value = spriteTrack.spriteAsset.sprites[i].sprite;
                }
                
                for(int i = spriteScrollList.Count - 1; i >= spriteTrack.spriteAsset.sprites.Count; i--)
                {
                    spriteScroll.Remove(spriteScrollList[i]);
                    spriteScrollList.RemoveAt(i);
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
                        
                        var spriteImage = new VisualElement() { name = "SpriteImage" }.SetParent(x);
                        spriteImage.SetFixedSize().SetHeight(spriteTimeline.style.height).SetAbsolute();
                        spriteImage.style.backgroundColor = new Color(.8f, .8f, .8f, 1f);
                        
                        var stamp = new VisualElement() { name = "Stamp" }.SetParent(x);
                        stamp.style.backgroundColor = new Color(.8f, .4f, .4f, 1f);
                        stamp.SetFixedSize().SetAbsolute().SetWidth(2).SetHeight(spriteTimeline.style.height);
                        
                        var timeLabel = new Label() { name = "TimeLabel", text = "undefined" }.SetParent(x);
                        timeLabel.SetAbsolute().SetHeight(18).SetFixedSize();
                        timeLabel.style.unityTextAlign = TextAnchor.UpperLeft;
                        timeLabel.style.top = spriteTimeline.style.height.value.value + 2;
                        
                        spriteTimeline.Add(x);
                        spriteTag.Add(x);
                    }
                    
                    var assign = spriteTrack.records[i];
                    var sprite = spriteTrack.spriteAsset[assign.assetId];
                    x.SetWidth(spriteTimeline.style.height.value.value * sprite.rect.width / sprite.rect.height);
                    var time = GetSpriteDisplayTime(i);
                    x.style.left = time.XMap(timeFrom, timeTo, 0, displayWidth);
                    
                    var xSpriteImage = x.Q("SpriteImage");
                    x.Q("SpriteImage").style.backgroundImage = new StyleBackground(sprite);
                    xSpriteImage.SetWidth(x.style.width);
                    var xTimeLabel = x.Q<Label>("TimeLabel");
                    if(fixedFramerate.value)
                    {
                        xTimeLabel.text = Mathf.Floor(time * framerate.value).ToString();
                    }
                    else
                    {
                        xTimeLabel.text = time.ToString("0.00");
                    }
                    
                    
                    // Sprite 按照顺序放到最后.
                    x.BringToFront();
                }
                
                for(int i = spriteTag.Count - 1; i >= spriteTrack.records.Count; i--)
                {
                    spriteTimeline.Remove(spriteTag[i]);
                    spriteTag.RemoveAt(i);
                }
                
                content.timeStamp.BringToFront();
                
                
                
                // 时间戳.
                
                
                
                
                
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
            
            
            
            float GetSpriteDisplayTime(int i)
            {
                var time = spriteTrack.records[i].time;
                if(currentDragging == i) time += dragSprite.delta.x / displayWidth * (timeTo - timeFrom);
                if(fixedFramerate.value) time = Mathf.Floor(time * framerate.value) / framerate.value;
                return time;
            }
        }
    }
}