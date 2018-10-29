﻿using UnityEngine;

namespace RTSLockstep
{
    public class EditorLocalVisualize : EditorVisualize
    {
        public override System.Type TargetType
        {
            get
            {
                return typeof(LocalVisualizeAttribute);
            }
        }

        public override void OnSceneGUI(CerealBehaviour source, UnityEditor.SerializedProperty property, GUIContent label)
        {
            base.OnSceneGUI(source, property, label);
        }
    }
}