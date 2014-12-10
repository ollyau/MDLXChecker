using Microsoft.Win32;
using System;
using System.IO;

namespace MDLXChecker {
    abstract class Simulator {
        public const string NOT_FOUND = "NOT_FOUND";

        protected string _registryKey;
        protected string _cfgRelativePath;
        protected string _name;

        private string _registryValue;
        private string _directory;
        private string _cfgPath;

        public Simulator() {
            _registryValue = "SetupPath";
        }

        private string GetDirectory() {
            _directory = (string)Registry.GetValue(_registryKey, _registryValue, null);

            if (string.IsNullOrEmpty(_directory)) {
                _directory = (string)Registry.GetValue(_registryKey.Insert("HKEY_LOCAL_MACHINE\\SOFTWARE\\".Length, "Wow6432Node\\"), _registryValue, null);
            }

            if (!string.IsNullOrEmpty(_directory) && !_directory.EndsWith("\\")) {
                _directory = _directory.Insert(_directory.Length, "\\");
            }

            if (string.IsNullOrEmpty(_directory)) {
                _directory = NOT_FOUND;
            }

            return _directory;
        }

        private string GetCfgPath() {
            _cfgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _cfgRelativePath);
            return _cfgPath;
        }

        public string Directory {
            get { return string.IsNullOrEmpty(_directory) ? GetDirectory() : _directory; }
        }

        public string CfgPath {
            get { return string.IsNullOrEmpty(_cfgPath) ? GetCfgPath() : _cfgPath; }
        }

        public string Name {
            get { return _name; }
        }
    }

    class FlightSimulatorX : Simulator {
        public FlightSimulatorX() {
            _registryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\microsoft games\flight simulator\10.0";
            _cfgRelativePath = @"Microsoft\FSX\fsx.CFG";
            _name = "Microsoft Flight Simulator X";
        }
    }

    class EnterpriseSimulationPlatform : Simulator {
        public EnterpriseSimulationPlatform() {
            _registryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft ESP\1.0";
            _cfgRelativePath = @"Microsoft\ESP\ESP.cfg";
            _name = "Microsoft ESP";
        }
    }

    class Prepar3D : Simulator {
        public Prepar3D() {
            _registryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\LockheedMartin\Prepar3D";
            _cfgRelativePath = @"Lockheed Martin\Prepar3D\Prepar3D.CFG";
            _name = "Lockheed Martin Prepar3D";
        }
    }

    class Prepar3D2 : Simulator {
        public Prepar3D2() {
            _registryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Lockheed Martin\Prepar3D v2";
            _cfgRelativePath = @"Lockheed Martin\Prepar3D v2\Prepar3D.CFG";
            _name = "Lockheed Martin Prepar3D v2";
        }
    }
}
