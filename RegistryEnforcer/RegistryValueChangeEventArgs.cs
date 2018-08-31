﻿/****************************** Module Header ******************************\
* Module Name:  RegistryKeyChangeEventArgs.cs
* Project:	    CSMonitorRegistryChange
* Copyright (c) Microsoft Corporation.
*
* This class derived from EventArgs. It is used to wrap the ManagementBaseObject of
* EventArrivedEventArgs.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace RegistryEnforcer
{
    public class RegistryValueChangeEventArgs : EventArgs
    {
        public object Value { get; }

        public RegistryValueChangeEventArgs(object value)
        {
            Value = value;
        }
    }
}