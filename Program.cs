using dnlib.DotNet;

var dllFiles = Directory.EnumerateFiles("./", 
                                        "*", SearchOption.TopDirectoryOnly)
               .Where(s => s.EndsWith(".dll"))
               .ToArray();

for (int i = 0; i < dllFiles.Length; i++) {
    var mod = ModuleDefMD.Load(dllFiles[i]);
    var x = mod.GetTypes();
    foreach (TypeDef type in x.ToArray()) {
        if (type.FullName.Contains("$UnityType") || type.FullName.Contains("UnityEngine.Internal")) {
            mod.Types.Remove(type);
        }
        
        foreach (MethodDef m in type.Methods.ToArray()) {
            if (m.FullName.Contains("UnityEngine.Internal") || m.FullName == "System.Void <Module>::.cctor()" || 
            m.FullName.Contains("Unity_") || m.FullName.Contains("$Invoke") || m.FullName.Contains("$Get") || m.FullName.Contains("$Set")
            || m.HasParams() && m.GetParamCount() < 3 && m.GetParam(0).FullName.Contains("UIntPtr")) {
                type.Methods.Remove(m);
            }
        }

        foreach (FieldDef f in type.Fields.ToArray()) {
            if (f.FullName.Contains("UnityEngine.Internal")) {
                type.Fields.Remove(f);
            }
        }
    }
    mod.Write(mod.Name += ".cleaned");
}