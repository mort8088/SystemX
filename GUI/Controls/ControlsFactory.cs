// -----------------------------------------------------------------------
// <copyright file="ControlsFactory.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SystemX.GUI.Controls {
    /// <summary>
    ///     Used for creating instances of I_Control objects
    /// </summary>
    public class ControlsFactory {
        #region Constructor
        private ControlsFactory() {
            // Perform variable initializations
            _ctrlCache = new Dictionary<string, Type>();

            // Initialize the factory
            Initialize();
        }
        #endregion

        #region Methods
        public I_Control CreateControl(string key) {
            if (!_ctrlCache.ContainsKey(key))
                throw new NullReferenceException("Invalid key supplied, must be non-null.");

            Type type = _ctrlCache[key];
            if (type != null) {
                object inst = _asmCache[_typeAsmLookup[key]].CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                if (inst == null)
                    throw new NullReferenceException("Null Control instance. Unable to create necessary control class.");

                I_Control ctrl = (I_Control)inst;

                return ctrl;
            }
            return null;
        }
        #endregion

        #region Singleton Pattern
        private static volatile ControlsFactory _mInstance;
        private static readonly object MSyncRoot = new object();

        public static ControlsFactory Instance {
            get {
                if (_mInstance == null) {
                    lock (MSyncRoot) {
                        if (_mInstance == null) _mInstance = new ControlsFactory();
                    }
                }

                return _mInstance;
            }
        }
        #endregion

        #region Variables

        // Key to type mappings for visual creation
        private Dictionary<string, Type> _ctrlCache;

        // Type -> Assembly lookup
        private Dictionary<string, string> _typeAsmLookup;

        // Cached reference to the containing assembly
        private Dictionary<string, Assembly> _asmCache;
        #endregion

        #region Helpers

        // Find and map available visual classes
        private void Initialize() {
            _asmCache = new Dictionary<string, Assembly>();
            _ctrlCache = new Dictionary<string, Type>();
            _typeAsmLookup = new Dictionary<string, string>();

            // check Current Domain
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in a.GetTypes())
                    RegisterObject(a, t);
            }

            //// Check the current working directory for more DLLs that might have I_Command objects to add
            //foreach (string Filename in Directory.GetFiles(Application.StartupPath, "*.dll"))
            //{
            //    Assembly Asm = Assembly.LoadFile(Filename);
            //    foreach (Type AsmType in Asm.GetTypes())
            //        RegisterObject(Asm, AsmType);
            //}
        }

        private void RegisterObject(Assembly a, Type t) {
            // Only add classes that aren't abstract & implement I_Visual
            if ((!t.IsClass || t.IsAbstract) ||
                (t.GetInterface("I_Control") == null)) return;

            if (!_asmCache.ContainsKey(a.FullName))
                _asmCache.Add(a.FullName, a);

            // Create a temporary instance of that class...
            object inst = a.CreateInstance(t.FullName, true, BindingFlags.CreateInstance, null, null, null, null);
            if (inst == null) return;

            I_Control keyDesc = (I_Control)inst;

            if (_ctrlCache.ContainsKey(keyDesc.KeyRef)) return;

            _ctrlCache.Add(keyDesc.KeyRef, t);
            _typeAsmLookup.Add(keyDesc.KeyRef, a.FullName);
        }
        #endregion
    }
}