using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace FillItUp
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class FIU : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Fill It Up"; } }
        public override string DisplaySection { get { return "Fill It Up"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        [GameParameters.CustomParameterUI("Display tooltips")]
        public bool displayTooltip = true;

        [GameParameters.CustomParameterUI("Save Locked Global Resource List",toolTip = "This option applies to the 'All Stages' window only")]
        public bool saveGlobalLocked = true;

#if false
        [GameParameters.CustomParameterUI("Save Locked Stage Resource List")]
        public bool saveStageLocked = true;
#endif




        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}