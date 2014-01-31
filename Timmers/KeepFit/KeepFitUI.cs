﻿using UnityEngine;

namespace KeepFit
{
    public abstract class SaveableWindow : MonoBehaviourWindow
    {
        private class RectStorage
        {
            [Persistent]
            float x, y, width, height;

            public Rect Restore()
            {
                Rect ret = new Rect();
                ret.x = x;
                ret.y = y;
                ret.width = width;
                ret.height = height;

                return ret;
            }

            public RectStorage Store(Rect source)
            {
                this.x = source.x;
                this.y = source.y;
                this.width = source.width;
                this.height = source.height;

                return this;
            }
        }

        private class Config : ConfigNodeStorage
        {

            //Custom Class Storage
            [Persistent]
            private RectStorage WindowRectStore = new RectStorage();
            internal Rect WindowRect = new Rect();



            internal Config(string configNodeName) : base(configNodeName)
            {
            }

            // Events to convert ManNode to Storable object
            public override void OnDecodeFromConfigNode()
            {
                WindowRect = WindowRectStore.Restore();
            }
            public override void OnEncodeToConfigNode()
            {
                WindowRectStore.Store(WindowRect);
            }
        }

        public bool Resizable { get; set; }
            
        private GUIStyle closeButtonStyle;
        private GUIStyle resizeStyle;
        private GUIContent resizeContent;
        private bool mouseDown;

        internal override void DrawWindow(int id)
        {
            ConfigureStyles();

            if (GUI.Button(new Rect(WindowRect.width - 24, 4, 20, 20), "X"))
            {
                Visible = false;
            }

            if (Resizable)
            {
                var resizeRect = new Rect(WindowRect.width - 16, WindowRect.height - 16, 16, 16);
                GUI.Label(resizeRect, resizeContent, resizeStyle);

                HandleWindowEvents(resizeRect);
            }
        }

        protected string GetConfigNodeName()
        {
            return WindowCaption.Replace(" ", "") + "Window";
        }

        public void Load(ConfigNode configNode)
        {
            this.Log("Load", "Loading config for window[" + WindowCaption + "]");
            Config config = new Config(GetConfigNodeName());
            if (config.Load(configNode, true) && 
                config.WindowRect != null && 
                config.WindowRect.height != 0 &&
                config.WindowRect.width != 0)
            {
                WindowRect = config.WindowRect;
            }
            this.Log("Load", "Loaded config for window[" + WindowCaption + "] WindowRect[" + WindowRect + "]");
        }

        public virtual void Save(ConfigNode configNode)
        {
            this.Log("Save", "Saving config for window[" + WindowCaption + "]");
            Config config = new Config(GetConfigNodeName());
            config.WindowRect = WindowRect;
            config.Save(configNode);
            this.Log("Load", "Saved config for window[" + WindowCaption + "] WindowRect[" + WindowRect + "] to configNode[" + configNode + "]");
        }

        private void HandleWindowEvents(Rect resizeRect)
        {
            var theEvent = Event.current;
            if (theEvent != null)
            {
                if (!mouseDown)
                {
                    if (theEvent.type == EventType.MouseDown && theEvent.button == 0 && resizeRect.Contains(theEvent.mousePosition))
                    {
                        mouseDown = true;
                        theEvent.Use();
                    }
                }
                else if (theEvent.type != EventType.Layout)
                {
                    if (Input.GetMouseButton(0))
                    {
                        // Flip the mouse Y so that 0 is at the top
                        float mouseY = Screen.height - Input.mousePosition.y;

                        WindowRect.width = Mathf.Clamp(Input.mousePosition.x - WindowRect.x + (resizeRect.width / 2), 50, Screen.width - WindowRect.x);
                        WindowRect.height = Mathf.Clamp(mouseY - WindowRect.y + (resizeRect.height / 2), 50, Screen.height - WindowRect.y);
                    }
                    else
                    {
                        mouseDown = false;
                    }
                }
            }
        }

        protected virtual void ConfigureStyles()
        {
            if (closeButtonStyle == null)
            {
                closeButtonStyle = new GUIStyle(GUI.skin.button);
                closeButtonStyle.padding = new RectOffset(5, 5, 3, 0);
                closeButtonStyle.margin = new RectOffset(1, 1, 1, 1);
                closeButtonStyle.stretchWidth = false;
                closeButtonStyle.stretchHeight = false;
                closeButtonStyle.alignment = TextAnchor.MiddleCenter;
            }

            if (resizeStyle == null)
            {
                resizeStyle = new GUIStyle(GUI.skin.button);
                resizeStyle.alignment = TextAnchor.MiddleCenter;
                resizeStyle.padding = new RectOffset(1, 1, 1, 1);
            }

            if (resizeContent == null)
            {
                resizeContent = new GUIContent("R", "Drag to resize the window.");
            }
        }
    }

    /// <summary>
    /// Window for displaying and editing per-game settings for KeepFit
    /// </summary>
    public class KeepFitGameConfigWindow : SaveableWindow
    {
        internal GameConfig config { get; set; }

        public KeepFitGameConfigWindow()
        {
            this.WindowCaption = "KeepFit Game Config";
            this.Visible = false;
            this.DragEnabled = true;
            
            this.WindowRect = new Rect(0, 0, 400, 300);
        }

        internal override void DrawWindow(int id)
        {
            base.DrawWindow(id);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button((config.enabled ? "Disable" : "Enable"), GUILayout.Width(80)))
            {
                config.enabled = !config.enabled;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Initial fitness level:" + config.initialFitnessLevel.ToString());
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+5", GUILayout.Width(80)))
            {
                config.initialFitnessLevel += 5;
                config.validate();
            }
            if (GUILayout.Button("-5", GUILayout.Width(80)))
            {
                config.initialFitnessLevel -= 5;
                config.validate();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Min fitness level:" + config.minFitnessLevel.ToString());
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+5", GUILayout.Width(80)))
            {
                config.minFitnessLevel += 5;
                config.validate();
            }
            if (GUILayout.Button("-5", GUILayout.Width(80)))
            {
                config.minFitnessLevel -= 5;
                config.validate();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max fitness level:" + config.maxFitnessLevel.ToString());
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+5", GUILayout.Width(80)))
            {
                config.maxFitnessLevel += 5;
                config.validate();
            }
            if (GUILayout.Button("-5", GUILayout.Width(80)))
            {
                config.maxFitnessLevel -= 5;
                config.validate();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

    /// <summary>
    /// Window for displaying and editing per-game settings for KeepFit
    /// </summary>
    public class KeepFitRosterWindow : SaveableWindow
    {
        internal GameConfig config { get; set; }
        internal KeepFitGameConfigWindow configWindow;
        
        private Vector2 scrollPosition;

        public KeepFitRosterWindow()
        {
            this.WindowCaption = "KeepFit Roster";
            this.Visible = false;
            this.DragEnabled = true;
            this.Resizable = true;

            this.WindowRect = new Rect(0, 0, 300, 300);
        }

        internal override void DrawWindow(int id)
        {
            base.DrawWindow(id);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();
            GUILayout.Space(4);
            if (GUILayout.Button("Configure", GUILayout.Width(80)))
            {
                configWindow.Visible = true;
            }

            GUILayout.Space(10);
            foreach (KeepFitCrewMember crewInfo in config.knownCrew.Values)
            {
                GUILayout.Label("Name: " + crewInfo.Name ); 
                GUILayout.Label("Fitness Level: " + crewInfo.fitnessLevel);
                GUILayout.Label("Activity Level: " + crewInfo.activityLevel);

                if (crewInfo.vesselName == null)
                {
                    GUILayout.Label("Location: Currently at KSC"); 
                }
                else
                {
                    GUILayout.Label("Location: " + crewInfo.vesselName);
                    GUILayout.Label("G(inst): " + crewInfo.instantaniousGeeLoadingAccumulator.ToString());
                    GUILayout.Label("G(short): " + crewInfo.shortTermGeeLoadingAccumulator.ToString());
                    GUILayout.Label("G(mid): " + crewInfo.mediumTermGeeLoadingAccumulator.ToString());
                    GUILayout.Label("G(long): " + crewInfo.longTermGeeLoadingAccumulator.ToString());
                }
                GUILayout.Space(4);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

    }
}