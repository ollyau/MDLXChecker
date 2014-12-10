using iniLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MDLXChecker {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(string.Format("MDLX Checker by Orion Lyau\r\nVersion: {0}\r\n", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version));
            try {
                Init();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            Console.WriteLine("\r\nPress any key to close.");
            Console.ReadKey();
        }

        static void Init() {
            var parser = new ArgumentParser();
            List<Simulator> sims = new List<Simulator>();

            if (!parser.Check("sim", (arg) => {
                if (arg.Equals("fsx", StringComparison.InvariantCultureIgnoreCase)) {
                    sims.Add(new FlightSimulatorX());
                }
                if (arg.Equals("esp", StringComparison.InvariantCultureIgnoreCase)) {
                    sims.Add(new EnterpriseSimulationPlatform());
                }
                else if (arg.Equals("p3d", StringComparison.InvariantCultureIgnoreCase)) {
                    sims.Add(new Prepar3D());
                }
                else if (arg.Equals("p3d2", StringComparison.InvariantCultureIgnoreCase)) {
                    sims.Add(new Prepar3D2());
                }
            })) {
                List<Simulator> allSims = new List<Simulator>();
                allSims.Add(new FlightSimulatorX());
                allSims.Add(new EnterpriseSimulationPlatform());
                allSims.Add(new Prepar3D());
                allSims.Add(new Prepar3D2());
                sims = new List<Simulator>(allSims.Where(x => x.Directory != Simulator.NOT_FOUND));
            }

            if (sims.Count == 0) {
                Console.WriteLine("No simulators found.");
                return;
            }

            foreach (var sim in sims) {
                Console.WriteLine("Simulator: {0}\r\n", sim.Name);
                var modelTypes = ModelTypes(new SimConfig(sim).SimObjectDirectories());
                var notNative = modelTypes.Where(x => !x.Value.Equals("MDLX"));
                foreach (var mdl in notNative) {
                    Console.WriteLine("{0}, {1}", mdl.Key, mdl.Value);
                }
            }
        }

        private static Dictionary<string, string> ModelTypes(List<string> directories) {
            string[] filenames = { "aircraft.cfg", "sim.cfg" };
            string[] models = { "normal", "interior" };
            Dictionary<string, string> modelTypes = new Dictionary<string, string>();
            foreach (var objectsDir in directories) {
                if (!Directory.Exists(objectsDir)) {
                    continue;
                }
                foreach (var objectDir in Directory.GetDirectories(objectsDir)) {
                    foreach (var name in filenames) {
                        var cfgPath = Path.Combine(objectDir, name);
                        if (!File.Exists(cfgPath)) {
                            continue;
                        }
                        Ini simCfg = new Ini(cfgPath);
                        foreach (string s in simCfg.GetCategoryNames().Where(x => x.StartsWith("fltsim."))) {
                            if (simCfg.KeyValueExists(s, "visual_model_guid")) {
                                Console.WriteLine("{0} {1} not checked (model referenced using visual_model_guid)", cfgPath, s);
                                continue;
                            }
                            var modelFolderName = simCfg.GetKeyValue(s, "model");
                            var modelFolderPath = string.IsNullOrWhiteSpace(modelFolderName) ? Path.Combine(objectDir, "model") : Path.Combine(objectDir, "model." + modelFolderName);
                            var modelCfgPath = Path.Combine(modelFolderPath, "model.cfg");
                            if (!File.Exists(modelCfgPath)) {
                                Console.WriteLine("{0} {1} not checked (missing model.cfg)", cfgPath, s);
                                continue;
                            }
                            Ini modelCfg = new Ini(modelCfgPath);
                            foreach (string m in models) {
                                if (modelCfg.KeyValueExists("models", m)) {
                                    var modelName = modelCfg.GetKeyValue("models", m);
                                    if (modelName.EndsWith(".mdl", StringComparison.InvariantCultureIgnoreCase)) {
                                        modelName = modelName.Substring(0, modelName.Length - 4);
                                    }
                                    var modelPath = Path.Combine(modelFolderPath, modelName + ".mdl");
                                    if (modelTypes.Keys.Contains(modelPath)) {
                                        continue;
                                    }
                                    if (File.Exists(modelPath)) {
                                        byte[] data = new byte[4];
                                        using (var fstream = new FileStream(modelPath, FileMode.Open, FileAccess.Read)) {
                                            fstream.Seek(8, SeekOrigin.Begin);
                                            fstream.Read(data, 0, 4);
                                        }
                                        modelTypes.Add(modelPath, Encoding.UTF8.GetString(data));
                                    }
                                    else {
                                        Console.WriteLine("Missing model: {0}", modelPath);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return modelTypes;
        }
    }
}
