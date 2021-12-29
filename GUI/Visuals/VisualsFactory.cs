// -----------------------------------------------------------------------
// <copyright file="VisualsFactory.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SystemX.GUI.Visuals {
    /// <summary>
    ///     Used for creating instances of I_Visual objects
    /// </summary>
    public class VisualsFactory {
        #region Constructor
        private VisualsFactory() {
            // Perform variable initializations
            _visCache = new Dictionary<string, Type>();

            // Initialize the factory
            Initialize();
        }
        #endregion

        #region Methods
        public I_Visual CreateVisual(string key) {
            if (!_visCache.ContainsKey(key))
                throw new NullReferenceException("Invalid key supplied, must be non-null.");

            Type type = _visCache[key];
            if (type == null)
                return null;

            object inst = _asmCache[_typeAsmLookup[key]].CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

            if (inst == null)
                throw new NullReferenceException("Null Visual instance. Unable to create necessary visual class.");

            return (I_Visual)inst;
        }
        #endregion

        #region Singleton Pattern
        private static volatile VisualsFactory _mInstance;
        private static readonly object MSyncRoot = new object();

        public static VisualsFactory Instance {
            get {
                if (_mInstance == null) {
                    lock (MSyncRoot) {
                        if (_mInstance == null) _mInstance = new VisualsFactory();
                    }
                }

                return _mInstance;
            }
        }
        #endregion

        #region Variables

        // Key to type mappings for visual creation
        private Dictionary<string, Type> _visCache;

        // Type -> Assembly lookup
        private Dictionary<string, string> _typeAsmLookup;

        // Cached reference to the containing assembly
        private Dictionary<string, Assembly> _asmCache;
        #endregion

        #region Helpers

        // Find and map available visual classes
        private void Initialize() {
            _asmCache = new Dictionary<string, Assembly>();
            _visCache = new Dictionary<string, Type>();
            _typeAsmLookup = new Dictionary<string, string>();

            // check Current Domain
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type t in a.GetTypes())
                    RegisterObject(a, t);
            }
        }

        private void RegisterObject(Assembly a, Type t) {
            // Only add classes that aren't abstract & implement I_Visual
            if ((t.IsClass && !t.IsAbstract) &&
                (t.GetInterface("I_Visual") != null)) {
                if (!_asmCache.ContainsKey(a.FullName))
                    _asmCache.Add(a.FullName, a);

                // Create a temporary instance of that class...
                object inst = a.CreateInstance(t.FullName, true, BindingFlags.CreateInstance, null, null, null, null);

                I_Visual keyDesc = (I_Visual)inst;

                if (keyDesc != null &&
                    !_visCache.ContainsKey(keyDesc.KeyRef)) {
                    _visCache.Add(keyDesc.KeyRef, t);
                    _typeAsmLookup.Add(keyDesc.KeyRef, a.FullName);
                }
            }
        }
        #endregion
    }
}