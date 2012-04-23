/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace VersionOne.ServiceHost {
    internal static class ServiceUtil {
        #region DLLImport

        private const int ScManagerCreateService = 0x0002;
        private const int ServiceWin32OwnProcess = 0x00000010;
        private const int ServiceErrorNormal = 0x00000001;
        private const int StandardRightsRequired = 0xF0000;
        private const int ServiceQueryConfig = 0x0001;
        private const int ServiceChangeConfig = 0x0002;
        private const int ServiceQueryStatus = 0x0004;
        private const int ServiceEnumerateDependents = 0x0008;
        private const int ServiceStart = 0x0010;
        private const int ServiceStop = 0x0020;
        private const int ServicePauseContinue = 0x0040;
        private const int ServiceInterrogate = 0x0080;
        private const int ServiceUserDefinedControl = 0x0100;

        private const int ServiceAllAccess = (StandardRightsRequired |
                                              ServiceQueryConfig |
                                              ServiceChangeConfig |
                                              ServiceQueryStatus |
                                              ServiceEnumerateDependents |
                                              ServiceStart |
                                              ServiceStop |
                                              ServicePauseContinue |
                                              ServiceInterrogate |
                                              ServiceUserDefinedControl);

        private const int ServiceAutoStart = 0x00000002;

        private const int GenericWrite = 0x40000000;
        private const int Delete = 0x10000;

        [DllImport("advapi32.dll")]
        private static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("Advapi32.dll")]
        private static extern IntPtr CreateService(IntPtr scHandle, string lpSvcName, string lpDisplayName,
                                                   int dwDesiredAccess, int dwServiceType, int dwStartType,
                                                   int dwErrorControl, string lpPathName, string lpLoadOrderGroup,
                                                   int lpdwTagId, string lpDependencies, string lpServiceStartName,
                                                   string lpPassword);

        [DllImport("advapi32.dll")]
        private static extern void CloseServiceHandle(IntPtr scHandle);

        [DllImport("advapi32.dll")]
        private static extern int StartService(IntPtr svHandle, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern IntPtr OpenService(IntPtr scHandle, string lpSvcName, int dwDesiredAccess);

        [DllImport("advapi32.dll")]
        private static extern int DeleteService(IntPtr svHandle);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        #endregion DLLImport

        public const string LocalService = "NT AUTHORITY\\LocalService";
        public const string NetworkService = "NT AUTHORITY\\NetworkService";
        public const string LocalSystem = null;

        public static bool InstallService(string svcPath, string svcName, string svcDispName, string svcUsername, string svcPassword) {
            var scm = OpenSCManager(null, null, ScManagerCreateService);

            if (scm.ToInt32() != 0) {
                var svc = CreateService(scm, svcName, svcDispName, ServiceAllAccess, ServiceWin32OwnProcess,
                                        ServiceAutoStart, ServiceErrorNormal, svcPath, null, 0, null,
                                        svcUsername, svcPassword);
                var installed = svc.ToInt32() != 0;
                CloseServiceHandle(scm);
                return installed;
            }
                
            return false;
        }

        public static void StartService(string svcName) {
            var ctrl = new ServiceController(svcName);
            ctrl.Start();
            ctrl.WaitForStatus(ServiceControllerStatus.StartPending);
        }

        public static void StopService(string svcName) {
            var ctrl = new ServiceController(svcName);
            ctrl.Stop();
            ctrl.WaitForStatus(ServiceControllerStatus.StopPending);
        }

        public static bool UnInstallService(string svcName) {
            var scHandle = OpenSCManager(null, null, GenericWrite);

            if(scHandle.ToInt32() != 0) {
                var svcHandle = OpenService(scHandle, svcName, Delete);
                
                if (svcHandle.ToInt32() != 0) {
                    var i = DeleteService(svcHandle);
                    CloseServiceHandle(scHandle);
                    return i != 0;
                }
                
                return false;
            }
            
            return false;
        }
    }
}