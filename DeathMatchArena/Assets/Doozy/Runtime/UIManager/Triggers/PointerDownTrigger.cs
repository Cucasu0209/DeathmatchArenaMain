﻿// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("Doozy/UI/Triggers/Pointer/Pointer Down")]
    public class PointerDownTrigger : SignalProvider, IPointerDownHandler
    {
        public PointerDownTrigger() : base(ProviderType.Local, "Pointer", "Down", typeof(PointerDownTrigger)) {}

        public void OnPointerDown(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
        }
    }
}
