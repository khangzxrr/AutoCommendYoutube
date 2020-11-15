// Decompiled with JetBrains decompiler
// Type: AutoYoutube.Program
// Assembly: AutoYoutube, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00E87B93-1BD7-497A-BDD2-3A581C9826C5
// Assembly location: C:\Users\skyho\Downloads\Release-Copy\Release - Copy\AutoYoutube.exe

using System;
using System.Windows.Forms;

namespace AutoYoutube
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new Form1());
    }
  }
}
