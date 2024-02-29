﻿using ColorMC.Gui.Objs;
using System;

namespace ColorMC.Gui.Utils.Hook;

public interface INative
{
    void AddHook(IntPtr id);
    void Stop();
    void SendMouse(double cursorX, double cursorY, bool message);
    void SendKey(InputKeyObj key, bool down, bool message);
    void SendScoll(bool up);
    IntPtr GetHandel();
}