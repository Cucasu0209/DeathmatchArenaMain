﻿// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.Reactor
{
    [AddComponentMenu("Doozy/Reactor/Progressor")]
    public class Progressor : MonoBehaviour
    {
        [SerializeField] private List<ProgressTarget> ProgressTargets;
        /// <summary> Progress targets that get updated by this Progressor when activated </summary>
        public List<ProgressTarget> progressTargets
        {
            get
            {
                Initialize();
                return ProgressTargets;
            }
        }

        [SerializeField] private List<Progressor> ProgressorTargets;
        /// <summary> Other Progressors that get updated by this Progressor when activated </summary>
        public List<Progressor> progressorTargets
        {
            get
            {
                Initialize();
                return progressorTargets;
            }
        }
        
        [SerializeField] protected float FromValue = 0f;
        /// <summary> Start Value </summary>
        public float fromValue
        {
            get => FromValue;
            set
            {
                FromValue = value;
                if (reaction.isActive) reaction.SetFrom(fromValue);
            }
        }

        [SerializeField] protected float ToValue = 1f;
        /// <summary> End Value </summary>
        public float toValue
        {
            get => ToValue;
            set
            {
                ToValue = value;
                if (reaction.isActive) reaction.SetTo(toValue);
            }
        }

        [SerializeField] protected float CurrentValue;
        /// <summary> Current Value </summary>
        public float currentValue => CurrentValue;

        [SerializeField] protected float Progress;
        /// <summary> Current Progress </summary>
        public float progress => Progress;

        [SerializeField] protected float CustomResetValue;
        /// <summary> Custom Reset Value </summary>
        public float customResetValue
        {
            get => CustomResetValue;
            set => CustomResetValue = Mathf.Clamp(value, FromValue, ToValue);
        }

        [SerializeField] protected FloatReaction Reaction;
        /// <summary> Reaction that runs this progressor </summary>
        public FloatReaction reaction
        {
            get
            {
                Initialize();
                return Reaction;
            }
        }

        public ResetValue ResetValueOnEnable = ResetValue.Disabled;

        public FloatEvent OnValueChanged;
        
        public FloatEvent OnProgressChanged;

        private bool initialized { get; set; }

        protected virtual void Initialize()
        {
            if (initialized) return;
            ProgressTargets ??= new List<ProgressTarget>();
            ProgressorTargets ??= new List<Progressor>();
            Reaction = Reaction ?? ReactionPool.Get<FloatReaction>();
            Reaction.SetFrom(fromValue);
            Reaction.SetTo(toValue);
            Reaction.SetValue(fromValue);
            Reaction.OnUpdateCallback = UpdateProgressor;
            initialized = true;
        }

        protected virtual void Awake()
        {
            if(!Application.isPlaying) return;
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if(!Application.isPlaying) return;
            ValidateTargets();
            ResetCurrentValue(ResetValueOnEnable);
        }

        protected virtual void OnDisable()
        {
            if(!Application.isPlaying) return;
            ValidateTargets();
            reaction.Stop();
        }

        protected void OnDestroy()
        {
            if(!Application.isPlaying) return;
            Reaction?.Recycle();
        }

        /// <summary> Remove null and duplicate targets </summary>
        private void ValidateTargets() =>
            ProgressTargets = progressTargets.Where(t => t != null).Distinct().ToList();

        /// <summary> Reset the Progressor to the given reset value </summary>
        /// <param name="resetValue"> Value to reset to </param>
        protected void ResetCurrentValue(ResetValue resetValue)
        {
            if (resetValue == ResetValue.Disabled) return;
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            switch (resetValue)
            {
                case ResetValue.FromValue:
                    SetProgressAtZero();
                    break;
                case ResetValue.EndValue:
                    SetProgressAtOne();
                    break;
                case ResetValue.CustomValue:
                    SetProgressAt(reaction.GetProgressAtValue(CustomResetValue));
                    break;
                case ResetValue.Disabled:
                default:
                    throw new ArgumentOutOfRangeException(nameof(resetValue), resetValue, null);
            }
        }

        /// <summary> Update current value and trigger callbacks </summary>
        public virtual void UpdateProgressor()
        {
            CurrentValue = reaction.currentValue;
            Progress = Mathf.InverseLerp(fromValue, toValue, currentValue);

            OnValueChanged?.Invoke(CurrentValue);
            OnProgressChanged?.Invoke(Progress);

            ProgressTargets.RemoveNulls();
            ProgressTargets.ForEach(t => t.UpdateTarget(this));

            ProgressorTargets.RemoveNulls();
            ProgressorTargets.ForEach(p => p.SetProgressAt(Progress));
        }

        /// <summary> Set the Progressor's current value to the given target value </summary>
        /// <param name="value"> Target value </param>
        public void SetValueAt(float value)
        {
            SetProgressAt(reaction.GetProgressAtValue(Mathf.Clamp(value, fromValue, toValue)));
        }

        /// <summary> Set the Progressor's current progress to the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        public void SetProgressAt(float targetProgress)
        {
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            reaction.SetProgressAt(targetProgress);
            UpdateProgressor();
        }

        /// <summary> Set the Progressor's current progress to 1 (100%) </summary>
        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        /// <summary> Set the Progressor's current progress to 0 (0%) </summary>
        public void SetProgressAtZero() =>
            SetProgressAt(0f);

        /// <summary> Play from start (from value) to end (to value), depending on the given PlayDirection </summary>
        /// <param name="direction"> Play direction </param>
        public void Play(PlayDirection direction) =>
            Play(direction == PlayDirection.Reverse);

        /// <summary> Play from the given start (from value) to end (to value), or in reverse </summary>
        /// <param name="inReverse"> Play in reverse? </param>
        public void Play(bool inReverse = false)
        {
            reaction.SetValue(inReverse ? ToValue : FromValue);
            reaction.Play(FromValue, ToValue, inReverse);
        }

        /// <summary> Play from the current value to the given target value </summary>
        /// <param name="value"> Target value </param>
        public void PlayToValue(float value)
        {
            value = Mathf.Clamp(value, fromValue, toValue); //clamp the value
            
            if (Math.Abs(value - fromValue) < 0.001f)
            {
                PlayToProgress(0f);
                return;
            }

            if (Math.Abs(value - toValue) < 0.001f)
            {
                PlayToProgress(1f);
                return;
            }
            
            PlayToProgress(Mathf.InverseLerp(fromValue, toValue, value));
        }

        /// <summary> Play from the current progress to the given target progress </summary>
        /// <param name="toProgress"> Target progress </param>
        public void PlayToProgress(float toProgress)
        {
            float p = Mathf.Clamp01(toProgress); //clamp the progress

            switch (p)
            {
                case 0:
                    reaction.Play(currentValue, fromValue);
                    break;
                case 1:
                    reaction.Play(currentValue, toValue);
                    break;
                default:
                    reaction.Play(currentValue, Mathf.Lerp(fromValue, toValue, p));
                    break;
            }
        }

        /// <summary> Stop the Progressor from playing </summary>
        public void Stop() =>
            reaction.Stop();

        /// <summary> Reverse the Progressor's playing direction (works only if the Progressor is playing) </summary>
        public void Reverse() =>
            reaction.Reverse();

        /// <summary> Rewind the Progressor to 0% if playing forward or to 100% if playing in reverse </summary>
        public void Rewind() =>
            reaction.Rewind();
        
        public float GetStartDelay() =>
            reaction.isActive ? reaction.startDelay : reaction.settings.GetStartDelay();

        public float GetDuration() =>
            reaction.isActive ? reaction.duration : reaction.settings.GetDuration();

        public float GetTotalDuration() =>
            GetStartDelay() + GetDuration();
    }
}
