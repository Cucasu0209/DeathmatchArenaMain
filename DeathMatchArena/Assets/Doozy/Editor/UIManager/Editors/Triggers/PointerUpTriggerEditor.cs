﻿// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Mody;
using Doozy.Runtime.UIManager.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Triggers
{
    [CustomEditor(typeof(PointerUpTrigger))]
    public class PointerUpTriggerEditor : ModyTriggerEditor<PointerUpTrigger>
    {
        public override List<Texture2D> secondaryIconTextures => EditorSpriteSheets.EditorUI.Icons.PointerUp;

        public override VisualElement CreateInspectorGUI()
        {
            CreateEditor();
            return root;
        }

        protected override void CreateEditor()
        {
            base.CreateEditor();
            fluidHeader.SetSecondaryIcon(secondaryIconTextures); //TRIGGER HEADER
            Compose(); //COMPOSE
        }
    }
}