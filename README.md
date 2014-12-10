MDLXChecker
===========

A utility that parses all SimObjects in Flight Simulator to verify that model files are FSX native.

Usage
---

Simply launch MDLXChecker.exe and it will display a list of models that don't have MDLX in the RIFF header.  If you want to check a specific simulator, you can use the following command line syntax:

    MDLXChecker.exe -sim:value

Where value is one of FSX, ESP, P3D, or P3D2.  If you launch MDLX Checker without any arguments it will default to any installed simulator.
