﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KeepFit
{
    /// <summary>
    /// Part added to modules (command modules?) to allow them to interact with KeepFit directly
    /// </summary>
    public class KeepFitPartModule  : PartModule
    {
        private static Dictionary<ActivityLevel, string> infoColorMap;

        // Values from the .cfg file
        [KSPField(guiActiveEditor = true, isPersistant = true, guiActive = true, guiName="Activity Level")]
        public string strActivityLevel = ActivityLevel.UNKNOWN.ToString();

        internal ActivityLevel activityLevel { get; private set; }

        public override void OnAwake()
        {
            this.Log_DebugOnly("Awake", "Part[{0}] strActivityLevel[{1}]", this.name, this.strActivityLevel);

            if (infoColorMap == null)
            {
                infoColorMap = new Dictionary<ActivityLevel, string>(5);
                infoColorMap[ActivityLevel.UNKNOWN] = "maroon";
                infoColorMap[ActivityLevel.CRAMPED] = "red";
                infoColorMap[ActivityLevel.COMFY] = "orange";
                infoColorMap[ActivityLevel.NEUTRAL] = "aqua";
                infoColorMap[ActivityLevel.EXERCISING] = "lime";
            }

            initActivityLevel();
        }

        private void initActivityLevel()
        {
            if (activityLevel == ActivityLevel.UNKNOWN)
            {
                activityLevel = ActivityLevel.COMFY;

                if (strActivityLevel != null)
                {
                    try
                    {
                        activityLevel = (ActivityLevel)Enum.Parse(typeof(ActivityLevel), strActivityLevel);
                    }
                    catch (ArgumentException)
                    {
                        this.Log_DebugOnly("initActivityLevel", "Part[{0}] strActivityLevel[{1}] is not a valid ActivityLevel", this.name, this.strActivityLevel);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the part is started by Unity.
        /// </summary>
        public override void OnStart(StartState state)
        {
            print(":OnStart");

            base.OnStart(state);
         
            // perform any initialisation steps
            //  - ? list the kerbals in this vessel
            //  - ? setup vessel kerbals KeepFit status UI?
            // XX - actually all these things are currently done by the controllers - XX
        }

        public void Update()
        {
            //print("KeepFitPartModule::Update");
        }

        public void Destroy()
        {
            print("KeepFitPartModule::Destroy");
        }

        // Method to provide extra infomation about the part on response to the RMB
        public override string GetInfo()
        {
            initActivityLevel();

            if (this.part.CrewCapacity == 0)
            {
                return "";
            }


            return "Activity Level: <color=" + infoColorMap[activityLevel] + ">" + this.activityLevel.ToString() + "</color>";
        }

    }
}
