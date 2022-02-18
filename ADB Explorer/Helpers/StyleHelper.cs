﻿using System.Windows;
using System.Windows.Controls;

namespace ADB_Explorer.Helpers
{
    public static class StyleHelper
    {
        public enum ContentAnimation
        {
            None,
            Bounce,
            Rotate,
        }

        public static ContentAnimation GetContentAnimation(Control control) =>
            (ContentAnimation)control.GetValue(ContentAnimationProperty);

        public static void SetContentAnimation(Control control, ContentAnimation value) =>
            control.SetValue(ContentAnimationProperty, value);

        public static readonly DependencyProperty ContentAnimationProperty =
            DependencyProperty.RegisterAttached(
                "ContentAnimation",
                typeof(ContentAnimation),
                typeof(StyleHelper),
                null);

        public static bool GetActivateAnimation(Control control) =>
            (bool)control.GetValue(ActivateAnimationProperty);

        public static void SetActivateAnimation(Control control, bool value) =>
            control.SetValue(ActivateAnimationProperty, value);

        public static readonly DependencyProperty ActivateAnimationProperty =
            DependencyProperty.RegisterAttached(
                "ActivateAnimation",
                typeof(bool),
                typeof(StyleHelper),
                null);

    }
}
