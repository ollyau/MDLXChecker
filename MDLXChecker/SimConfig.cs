using iniLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDLXChecker {
    class SimConfig {
        private string cfgPath;
        private Simulator sim;

        public SimConfig(Simulator sim) {
            if (!File.Exists(sim.CfgPath)) {
                throw new FileNotFoundException("Unable to locate specified CFG file: " + cfgPath);
            }
            if (!Directory.Exists(sim.Directory)) {
                throw new DirectoryNotFoundException("Unable to locate simulator directory: " + sim.GetType());
            }
            this.cfgPath = sim.CfgPath;
            this.sim = sim;
        }

        public List<string> SimObjectDirectories() {
            Ini simCfg = new Ini(cfgPath);
            List<string> result = new List<string>();
            foreach (var s in simCfg.GetKeyNames("Main").Where(x => x.StartsWith("SimObjectPaths."))) {
                var path = simCfg.GetKeyValue("Main", s);
                if (!Path.IsPathRooted(path)) {
                    path = Path.Combine(sim.Directory, path);
                }
                if (Directory.Exists(path)) {
                    result.Add(path);
                }
            }
            return result;
        }
    }
}
