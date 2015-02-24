﻿#region File Header
// /******************************************************************************
//  * 
//  *      Copyright (C) Ankesh Dave 2015 All Rights Reserved. Confidential
//  * 
//  ******************************************************************************
//  * 
//  *      Project:        OutlookGoogleSyncRefresh
//  *      SubProject:     OutlookGoogleSyncRefresh.Presentation
//  *      Author:         Dave, Ankesh
//  *      Created On:     20-02-2015 3:18 PM
//  *      Modified On:    20-02-2015 3:18 PM
//  *      FileName:       UiInteractionService.cs
//  * 
//  *****************************************************************************/
#endregion

using System;
using System.ComponentModel.Composition;

using OutlookGoogleSyncRefresh.Application.Services;
using OutlookGoogleSyncRefresh.Helpers;
using OutlookGoogleSyncRefresh.Presentation.Views;

namespace OutlookGoogleSyncRefresh.Presentation.Services
{
    [Export(typeof(IGuiInteractionService))]
    public class GuiInteractionService : IGuiInteractionService
    {
        public ShellView ShellView { get; set; }

        [ImportingConstructor]
        public GuiInteractionService(ShellView shellView)
        {
            ShellView = shellView;
        }

        public void ShowApplication()
        {
            ShellView.Dispatcher.BeginInvoke(((Action)(() => Utilities.BringToForeground(ShellView))));
        }

        public void HideApplication()
        {
            ShellView.Dispatcher.BeginInvoke(((Action)(() => Utilities.HideForeground(ShellView))));
        }

    }
}