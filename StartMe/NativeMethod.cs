﻿//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Xamarin.Forms;


    /****************************** Module Header ******************************\ 
     Module Name:  NativeMethod.cs 
     Project:      CSUACSelfElevation 
     Copyright (c) Microsoft Corporation. 

     The P/Invoke signature some native Windows APIs used by the code sample. 

     This source is subject to the Microsoft Public License. 
     See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
     All other rights reserved. 

     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
     EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
     WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
    \***************************************************************************/

    #region Using directives 
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    #endregion


    namespace StartMe
    {
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);
    }

    /// <summary> 
    /// The TOKEN_INFORMATION_CLASS enumeration type contains values that  
    /// specify the type of information being assigned to or retrieved from  
    /// an access token. 
    /// </summary> 
    internal enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        /// <summary> 
        /// The WELL_KNOWN_SID_TYPE enumeration type is a list of commonly used  
        /// security identifiers (SIDs). Programs can pass these values to the  
        /// CreateWellKnownSid function to create a SID from this list. 
        /// </summary> 
        internal enum WELL_KNOWN_SID_TYPE
        {
            WinNullSid = 0,
            WinWorldSid = 1,
            WinLocalSid = 2,
            WinCreatorOwnerSid = 3,
            WinCreatorGroupSid = 4,
            WinCreatorOwnerServerSid = 5,
            WinCreatorGroupServerSid = 6,
            WinNtAuthoritySid = 7,
            WinDialupSid = 8,
            WinNetworkSid = 9,
            WinBatchSid = 10,
            WinInteractiveSid = 11,
            WinServiceSid = 12,
            WinAnonymousSid = 13,
            WinProxySid = 14,
            WinEnterpriseControllersSid = 15,
            WinSelfSid = 16,
            WinAuthenticatedUserSid = 17,
            WinRestrictedCodeSid = 18,
            WinTerminalServerSid = 19,
            WinRemoteLogonIdSid = 20,
            WinLogonIdsSid = 21,
            WinLocalSystemSid = 22,
            WinLocalServiceSid = 23,
            WinNetworkServiceSid = 24,
            WinBuiltinDomainSid = 25,
            WinBuiltinAdministratorsSid = 26,
            WinBuiltinUsersSid = 27,
            WinBuiltinGuestsSid = 28,
            WinBuiltinPowerUsersSid = 29,
            WinBuiltinAccountOperatorsSid = 30,
            WinBuiltinSystemOperatorsSid = 31,
            WinBuiltinPrintOperatorsSid = 32,
            WinBuiltinBackupOperatorsSid = 33,
            WinBuiltinReplicatorSid = 34,
            WinBuiltinPreWindows2000CompatibleAccessSid = 35,
            WinBuiltinRemoteDesktopUsersSid = 36,
            WinBuiltinNetworkConfigurationOperatorsSid = 37,
            WinAccountAdministratorSid = 38,
            WinAccountGuestSid = 39,
            WinAccountKrbtgtSid = 40,
            WinAccountDomainAdminsSid = 41,
            WinAccountDomainUsersSid = 42,
            WinAccountDomainGuestsSid = 43,
            WinAccountComputersSid = 44,
            WinAccountControllersSid = 45,
            WinAccountCertAdminsSid = 46,
            WinAccountSchemaAdminsSid = 47,
            WinAccountEnterpriseAdminsSid = 48,
            WinAccountPolicyAdminsSid = 49,
            WinAccountRasAndIasServersSid = 50,
            WinNTLMAuthenticationSid = 51,
            WinDigestAuthenticationSid = 52,
            WinSChannelAuthenticationSid = 53,
            WinThisOrganizationSid = 54,
            WinOtherOrganizationSid = 55,
            WinBuiltinIncomingForestTrustBuildersSid = 56,
            WinBuiltinPerfMonitoringUsersSid = 57,
            WinBuiltinPerfLoggingUsersSid = 58,
            WinBuiltinAuthorizationAccessSid = 59,
            WinBuiltinTerminalServerLicenseServersSid = 60,
            WinBuiltinDCOMUsersSid = 61,
            WinBuiltinIUsersSid = 62,
            WinIUserSid = 63,
            WinBuiltinCryptoOperatorsSid = 64,
            WinUntrustedLabelSid = 65,
            WinLowLabelSid = 66,
            WinMediumLabelSid = 67,
            WinHighLabelSid = 68,
            WinSystemLabelSid = 69,
            WinWriteRestrictedCodeSid = 70,
            WinCreatorOwnerRightsSid = 71,
            WinCacheablePrincipalsGroupSid = 72,
            WinNonCacheablePrincipalsGroupSid = 73,
            WinEnterpriseReadonlyControllersSid = 74,
            WinAccountReadonlyControllersSid = 75,
            WinBuiltinEventLogReadersGroup = 76,
            WinNewEnterpriseReadonlyControllersSid = 77,
            WinBuiltinCertSvcDComAccessGroup = 78
        }

        /// <summary> 
        /// The SECURITY_IMPERSONATION_LEVEL enumeration type contains values  
        /// that specify security impersonation levels. Security impersonation  
        /// levels govern the degree to which a server process can act on behalf  
        /// of a client process. 
        /// </summary> 
        internal enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        /// <summary> 
        /// The TOKEN_ELEVATION_TYPE enumeration indicates the elevation type of  
        /// token being queried by the GetTokenInformation function or set by  
        /// the SetTokenInformation function. 
        /// </summary> 
        internal enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        /// <summary> 
        /// The structure represents a security identifier (SID) and its  
        /// attributes. SIDs are used to uniquely identify users or groups. 
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        internal struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public UInt32 Attributes;
        }

        /// <summary> 
        /// The structure indicates whether a token has elevated privileges. 
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        internal struct TOKEN_ELEVATION
        {
            public Int32 TokenIsElevated;
        }

        /// <summary> 
        /// The structure specifies the mandatory integrity level for a token. 
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        internal struct TOKEN_MANDATORY_LABEL
        {
            public SID_AND_ATTRIBUTES Label;
        }

        /// <summary> 
        /// Represents a wrapper class for a token handle. 
        /// </summary> 
        internal class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle() : base(true)
            {
            }

            internal SafeTokenHandle(IntPtr handle) : base(true)
            {
                base.SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                return SafeNativeMethods.CloseHandle(base.handle);
            }
        }

        internal class NativeMethods
        {
            // Token Specific Access Rights 

            public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
            public const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
            public const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;
            public const UInt32 TOKEN_DUPLICATE = 0x0002;
            public const UInt32 TOKEN_IMPERSONATE = 0x0004;
            public const UInt32 TOKEN_QUERY = 0x0008;
            public const UInt32 TOKEN_QUERY_SOURCE = 0x0010;
            public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
            public const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
            public const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
            public const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
            public const UInt32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
            public const UInt32 TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE |
                TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES |
                TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID);


            public const Int32 ERROR_INSUFFICIENT_BUFFER = 122;


            // Integrity Levels 

            public const Int32 SECURITY_MANDATORY_UNTRUSTED_RID = 0x00000000;
            public const Int32 SECURITY_MANDATORY_LOW_RID = 0x00001000;
            public const Int32 SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
            public const Int32 SECURITY_MANDATORY_HIGH_RID = 0x00003000;
            public const Int32 SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;


            /// <summary> 
            /// The function opens the access token associated with a process. 
            /// </summary> 
            /// <param name="hProcess"> 
            /// A handle to the process whose access token is opened. 
            /// </param> 
            /// <param name="desiredAccess"> 
            /// Specifies an access mask that specifies the requested types of  
            /// access to the access token.  
            /// </param> 
            /// <param name="hToken"> 
            /// Outputs a handle that identifies the newly opened access token  
            /// when the function returns. 
            /// </param> 
            /// <returns></returns> 
            [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenProcessToken(IntPtr hProcess,
                UInt32 desiredAccess, out SafeTokenHandle hToken);


            /// <summary> 
            /// The function creates a new access token that duplicates one  
            /// already in existence. 
            /// </summary> 
            /// <param name="ExistingTokenHandle"> 
            /// A handle to an access token opened with TOKEN_DUPLICATE access. 
            /// </param> 
            /// <param name="ImpersonationLevel"> 
            /// Specifies a SECURITY_IMPERSONATION_LEVEL enumerated type that  
            /// supplies the impersonation level of the new token. 
            /// </param> 
            /// <param name="DuplicateTokenHandle"> 
            /// Outputs a handle to the duplicate token.  
            /// </param> 
            /// <returns></returns> 
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public extern static bool DuplicateToken(
                SafeTokenHandle ExistingTokenHandle,
                SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
                out SafeTokenHandle DuplicateTokenHandle);


            /// <summary> 
            /// The function retrieves a specified type of information about an  
            /// access token. The calling process must have appropriate access  
            /// rights to obtain the information. 
            /// </summary> 
            /// <param name="hToken"> 
            /// A handle to an access token from which information is retrieved. 
            /// </param> 
            /// <param name="tokenInfoClass"> 
            /// Specifies a value from the TOKEN_INFORMATION_CLASS enumerated  
            /// type to identify the type of information the function retrieves. 
            /// </param> 
            /// <param name="pTokenInfo"> 
            /// A pointer to a buffer the function fills with the requested  
            /// information. 
            /// </param> 
            /// <param name="tokenInfoLength"> 
            /// Specifies the size, in bytes, of the buffer pointed to by the  
            /// TokenInformation parameter.  
            /// </param> 
            /// <param name="returnLength"> 
            /// A pointer to a variable that receives the number of bytes needed  
            /// for the buffer pointed to by the TokenInformation parameter.  
            /// </param> 
            /// <returns></returns> 
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetTokenInformation(
                SafeTokenHandle hToken,
                TOKEN_INFORMATION_CLASS tokenInfoClass,
                IntPtr pTokenInfo,
                Int32 tokenInfoLength,
                out Int32 returnLength);


            /// <summary> 
            /// Sets the elevation required state for a specified button or  
            /// command link to display an elevated icon.  
            /// </summary> 
            public const UInt32 BCM_SETSHIELD = 0x160C;


            /// <summary> 
            /// Sends the specified message to a window or windows. The function  
            /// calls the window procedure for the specified window and does not  
            /// return until the window procedure has processed the message.  
            /// </summary> 
            /// <param name="hWnd"> 
            /// Handle to the window whose window procedure will receive the  
            /// message. 
            /// </param> 
            /// <param name="Msg">Specifies the message to be sent.</param> 
            /// <param name="wParam"> 
            /// Specifies additional message-specific information. 
            /// </param> 
            /// <param name="lParam"> 
            /// Specifies additional message-specific information. 
            /// </param> 
            /// <returns></returns> 
            //[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
            //public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);


            /// <summary> 
            /// The function returns a pointer to a specified subauthority in a  
            /// security identifier (SID). The subauthority value is a relative  
            /// identifier (RID). 
            /// </summary> 
            /// <param name="pSid"> 
            /// A pointer to the SID structure from which a pointer to a  
            /// subauthority is to be returned. 
            /// </param> 
            /// <param name="nSubAuthority"> 
            /// Specifies an index value identifying the subauthority array  
            /// element whose address the function will return. 
            /// </param> 
            /// <returns> 
            /// If the function succeeds, the return value is a pointer to the  
            /// specified SID subauthority. To get extended error information,  
            /// call GetLastError. If the function fails, the return value is  
            /// undefined. The function fails if the specified SID structure is  
            /// not valid or if the index value specified by the nSubAuthority  
            /// parameter is out of bounds. 
            /// </returns> 
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetSidSubAuthority(IntPtr pSid, UInt32 nSubAuthority);
        }
    }
