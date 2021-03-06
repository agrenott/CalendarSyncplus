﻿#region File Header

// /******************************************************************************
//  * 
//  *      Copyright (C) Ankesh Dave 2015 All Rights Reserved. Confidential
//  * 
//  ******************************************************************************
//  * 
//  *      Project:        CalendarSyncPlus
//  *      SubProject:     CalendarSyncPlus.Application
//  *      Author:         Dave, Ankesh
//  *      Created On:     02-02-2015 2:55 PM
//  *      Modified On:    02-02-2015 2:55 PM
//  *      FileName:       ApplicationController.cs
//  * 
//  *****************************************************************************/

#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using CalendarSyncPlus.Application.ViewModels;
using CalendarSyncPlus.Common;
using CalendarSyncPlus.Domain.Models;
using CalendarSyncPlus.GoogleServices.Google;
using CalendarSyncPlus.Services;
using CalendarSyncPlus.Services.Interfaces;

namespace CalendarSyncPlus.Application.Controllers
{
    [Export(typeof(IApplicationController))]
    public class ApplicationController : IApplicationController
    {
        public ILocalizationService LocalizationService { get; set; }
        private readonly AboutViewModel _aboutViewModel;
        private readonly DelegateCommand _exitCommand;
        private readonly IGuiInteractionService _guiInteractionService;
        private readonly HelpViewModel _helpViewModel;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly IShellController _shellController;
        private readonly ShellService _shellService;
        private readonly ShellViewModel _shellViewModel;
        private readonly SystemTrayNotifierViewModel _systemTrayNotifierViewModel;
        private bool _isApplicationExiting;

        [ImportingConstructor]
        public ApplicationController(Lazy<ShellViewModel> shellViewModelLazy,
            Lazy<SettingsViewModel> settingsViewModelLazy,
            Lazy<AboutViewModel> aboutViewModelLazy, Lazy<HelpViewModel> helpViewModelLazy,
            Lazy<ShellService> shellServiceLazy, CompositionContainer compositionContainer,
            Lazy<IAccountAuthenticationService> accountAuthenticationServiceLazy, IShellController shellController,
            Lazy<SystemTrayNotifierViewModel> lazySystemTrayNotifierViewModel,
            IGuiInteractionService guiInteractionService)
        {
            //ViewModels
            _shellViewModel = shellViewModelLazy.Value;
            _settingsViewModel = settingsViewModelLazy.Value;
            _aboutViewModel = aboutViewModelLazy.Value;
            _helpViewModel = helpViewModelLazy.Value;
            _systemTrayNotifierViewModel = lazySystemTrayNotifierViewModel.Value;
            //Commands
            _shellViewModel.Closing += ShellViewModelClosing;
            _exitCommand = new DelegateCommand(Close);

            //Services
            AccountAuthenticationService = accountAuthenticationServiceLazy.Value;

            _shellService = shellServiceLazy.Value;
            _shellService.ShellView = _shellViewModel.View;
            _shellService.SettingsView = _settingsViewModel.View;
            _shellService.AboutView = _aboutViewModel.View;
            _shellService.HelpView = _helpViewModel.View;
            _shellController = shellController;
            _guiInteractionService = guiInteractionService;
            if (_shellViewModel.IsSettingsVisible)
            {
                _settingsViewModel.Load();
            }
        }

        public IAccountAuthenticationService AccountAuthenticationService { get; set; }

        #region IApplicationController Members

        public void Initialize()
        {
            _shellViewModel.ExitCommand = _exitCommand;
            _systemTrayNotifierViewModel.ExitCommand = _exitCommand;
            //Initialize Other Controllers if Any
            _shellController.Initialize();
            PropertyChangedEventManager.AddHandler(_settingsViewModel, SettingsChangedHandler, "");
            PropertyChangedEventManager.AddHandler(_shellViewModel, ShellViewUpdatedHandler, "");
        }

        public void Run(bool startMinimized)
        {
            //Perform Other assignments if required
            _shellViewModel.Show(startMinimized);
            _settingsViewModel.ApplyProxySettings();
        }

        public void Shutdown()
        {
            //Close All controllers if required
            _shellController.Shutdown();
            _settingsViewModel.Shutdown();
            PropertyChangedEventManager.RemoveHandler(_settingsViewModel, SettingsChangedHandler, "");
            PropertyChangedEventManager.RemoveHandler(_shellViewModel, ShellViewUpdatedHandler, "");

            //Save Settings if any
        }

        #endregion

        private void ShellViewUpdatedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsAboutVisible":
                    if (_shellViewModel.IsAboutVisible)
                    {
                        if (_shellViewModel.Settings.AppSettings.CheckForUpdates)
                        {
                            _aboutViewModel.CheckForUpdatesCommand.Execute(null);
                        }
                    }
                    break;
                case "IsSettingsVisible":
                    if (_shellViewModel.IsSettingsVisible)
                    {
                        _settingsViewModel.Load();
                    }
                    break;
            }
        }

        private void SettingsChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SettingsSaved":
                    if (_settingsViewModel.SettingsSaved)
                    {
                        _shellViewModel.IsSettingsVisible = false;
                        _shellViewModel.Settings = _settingsViewModel.Settings;
                        foreach (CalendarSyncProfile syncProfile in _settingsViewModel.Settings.SyncProfiles)
                        {
                            if (syncProfile.IsSyncEnabled && syncProfile.SyncSettings.SyncFrequency != null)
                            {
                                syncProfile.NextSync =
                                    syncProfile.SyncSettings.SyncFrequency.GetNextSyncTime(DateTime.Now);
                            }
                        }
                    }
                    break;
                case "IsLoading":
                    _shellViewModel.IsSettingsLoading = _settingsViewModel.IsLoading;
                    break;
            }
        }

        private void Close()
        {
            _isApplicationExiting = true;
            _shellViewModel.Close();
        }

        private void ShellViewModelClosing(object sender, CancelEventArgs e)
        {
            // Try to  user has already saved settings or pending operation are left.
            if (_isApplicationExiting || !_shellViewModel.Settings.AppSettings.MinimizeToSystemTray)
            {
                return;
            }
            _guiInteractionService.HideApplication();
            e.Cancel = true;
        }
    }
}