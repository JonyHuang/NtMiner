﻿using NTMiner.Views;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NTMiner {
    public static class AppHelper {
        public static Action<RemoteDesktopInput> RemoteDesktop;

        public static void RunAsAdministrator() {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ClientId.AppFileFullName;
            startInfo.Arguments = string.Join(" ", CommandLineArgs.Args);
            startInfo.Verb = "runas";
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        #region Init
        public static void Init(Application app) {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
                var exception = e.ExceptionObject as Exception;
                if (exception != null) {
                    Handle(exception);
                }
            };

            app.DispatcherUnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => {
                Handle(e.Exception);
                e.Handled = true;
            };

            UIThread.InitializeWithDispatcher();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
        }
        #endregion

        #region ShowMainWindow
        public static void ShowMainWindow(Application app, int clientPort) {
            try {
                Client.MinerClientService.ShowMainWindowAsync("localhost", clientPort, isSuccess=> {
                    if (!isSuccess) {
                        RestartNTMiner();
                    }
                    UIThread.Execute(() => {
                        app.Shutdown();
                    });
                });
            }
            catch (Exception ex) {
                RestartNTMiner();
                Logger.ErrorDebugLine(ex.Message, ex);
            }
        }

        private static void RestartNTMiner() {
            Process thisProcess = Process.GetCurrentProcess();
            foreach (var process in Process.GetProcessesByName(thisProcess.ProcessName)) {
                if (process.Id != thisProcess.Id) {
                    try {
                        Windows.TaskKill.Kill(process.Id);
                    }
                    catch {
                    }
                }
            }
            Windows.Cmd.RunClose(ClientId.AppFileFullName, string.Join(" ", CommandLineArgs.Args));
        }
        #endregion

        #region private methods
        private static void Handle(Exception e) {
            if (e == null) {
                return;
            }
            if (e is ValidationException) {
                DialogWindow.ShowDialog(title: "验证失败", message: e.Message, icon: "Icon_Error");
            }
            else {
                Logger.ErrorDebugLine(e);
            }
        }
        #endregion

        public static void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                ScrollViewer scrollViewer = (ScrollViewer)sender;
                if (scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible) {
                    Point p = e.GetPosition(scrollViewer);
                    if (p.X > scrollViewer.ActualWidth - SystemParameters.ScrollWidth) {
                        return;
                    }
                }
                Window.GetWindow(scrollViewer).DragMove();
                e.Handled = true;
            }
        }
    }
}
