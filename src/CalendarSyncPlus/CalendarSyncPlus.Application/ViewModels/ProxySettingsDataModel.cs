﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Waf.Foundation;
using CalendarSyncPlus.Common.Attributes;
using CalendarSyncPlus.Domain;
using CalendarSyncPlus.Domain.Models;

namespace CalendarSyncPlus.Application.ViewModels
{
    public class ProxySettingsDataModel : ValidatableModel
    {
        #region Fields

        private bool _bypassOnLocal;
        private string _domain;
        private string _password;
        private int _port;
        private string _proxyAddress;
        private ProxyType _proxyType;
        private bool _useDefaultCredentials;
        private string _userName;

        #endregion

        #region Properties
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "Invalid URL")]
        public string ProxyAddress
        {
            get { return _proxyAddress; }
            set { SetPropertyAndValidate(ref _proxyAddress, value); }
        }
        [Range(0, 65535,ErrorMessage="Port should ne between {0}")]
        public int Port
        {
            get { return _port; }
            set { SetPropertyAndValidate(ref _port, value); }
        }


        public ProxyType ProxyType
        {
            get { return _proxyType; }
            set { SetProperty(ref _proxyType, value); }
        }

        public bool BypassOnLocal
        {
            get { return _bypassOnLocal; }
            set { SetProperty(ref _bypassOnLocal, value); }
        }

        public bool UseDefaultCredentials
        {
            get { return _useDefaultCredentials; }
            set { SetProperty(ref _useDefaultCredentials, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public string Domain
        {
            get { return _domain; }
            set { SetProperty(ref _domain, value); }
        }

        #endregion
    }
}