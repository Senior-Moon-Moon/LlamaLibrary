﻿using System;
using ff14bot;
using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace LlamaLibrary.RemoteWindows
{
    public class RemoteWindow<T> : RemoteWindow where T : RemoteWindow<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance ?? (_instance = new T());

        protected RemoteWindow(string name) : base(name)
        {
            // _instance = new T();
        }
    }

    public abstract class RemoteWindow
    {
        internal string _name;
        private const int Offset0 = 0x1CA;
        private const int Offset2 = 0x160;
        /*      Can Also do this: Will pull the same offsets Mastahg stores in RB
                var off = typeof(Core).GetProperty("Offsets", BindingFlags.NonPublic | BindingFlags.Static);
                var struct158 = off.PropertyType.GetFields()[72];
                var offset0 = (int)struct158.FieldType.GetFields()[0].GetValue(struct158.GetValue(off.GetValue(null)));
                var offset2 = (int)struct158.FieldType.GetFields()[2].GetValue(struct158.GetValue(off.GetValue(null)));
        */

        public virtual bool IsOpen => WindowByName != null;

        public virtual string Name => _name;

        public virtual AtkAddonControl WindowByName => RaptureAtkUnitManager.GetWindowByName(Name);

        protected bool HasAgentInterfaceId => GetAgentInterfaceId() != 0;

        protected RemoteWindow(string name)
        {
            _name = name;
        }

        public virtual void Close()
        {
            if (IsOpen)
                SendAction(1, 3uL, 4294967295uL);
        }

        public int GetAgentInterfaceId()
        {
            if (WindowByName == null)
                return 0;

            var test = WindowByName.TryFindAgentInterface();

            return test == null ? 0 : test.Id;
        }

        protected TwoInt[] ___Elements()
        {
            if (WindowByName == null) return null;
            var elementCount = ElementCount();
            var addr = Core.Memory.Read<IntPtr>(WindowByName.Pointer + Offset2);
            return Core.Memory.ReadArray<TwoInt>(addr, elementCount);
        }

        protected ushort ElementCount()
        {
            return WindowByName != null ? Core.Memory.Read<ushort>(WindowByName.Pointer + Offset0) : (ushort)0;
        }

        protected void SendAction(int pairCount, params ulong[] param)
        {
            if (IsOpen)
                WindowByName.SendAction(pairCount, param);
        }
    }
}