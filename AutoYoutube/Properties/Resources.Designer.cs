// Decompiled with JetBrains decompiler
// Type: AutoYoutube.Properties.Resources
// Assembly: AutoYoutube, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00E87B93-1BD7-497A-BDD2-3A581C9826C5
// Assembly location: C:\Users\skyho\Downloads\Release-Copy\Release - Copy\AutoYoutube.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AutoYoutube.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AutoYoutube.Properties.Resources.resourceMan == null)
          AutoYoutube.Properties.Resources.resourceMan = new ResourceManager("AutoYoutube.Properties.Resources", typeof (AutoYoutube.Properties.Resources).Assembly);
        return AutoYoutube.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AutoYoutube.Properties.Resources.resourceCulture;
      set => AutoYoutube.Properties.Resources.resourceCulture = value;
    }

    internal static string jquery => AutoYoutube.Properties.Resources.ResourceManager.GetString(nameof (jquery), AutoYoutube.Properties.Resources.resourceCulture);
  }
}
