﻿using System;
using System.Windows;
using System.Diagnostics;

namespace SrkToolkit.Mvvm.Tools {

    /// <summary>
    /// Abstraction of the MessageBox component. 
    /// </summary>
    public class MessageBoxService {

        private readonly Func<string, string, MessageBoxButton, MessageBoxResult> action;

        public MessageBoxService() { }

        public MessageBoxService(Func<string, string, MessageBoxButton, MessageBoxResult> action) {
            this.action = action;
        }

        [Conditional("DEBUG"), DebuggerStepThrough]
        public MessageBoxResult ShowDebug(string messageBoxText) {
            if (action != null) {
                return action(messageBoxText, null, MessageBoxButton.OK);
            } else {
                return MessageBox.Show(messageBoxText);
            }
        }

        [DebuggerStepThrough]
        public MessageBoxResult Show(string messageBoxText) {
            if (action != null) {
                return action(messageBoxText, null, MessageBoxButton.OK);
            } else {
                return MessageBox.Show(messageBoxText);
            }
        }

        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button) {
            if (action != null) {
                return action(messageBoxText, caption, button);
            } else {
                return MessageBox.Show(messageBoxText, caption, button);
            }
        }

    }
}
