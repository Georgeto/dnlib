using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace dnlib.Examples {
	// This example will open mscorlib.dll and then print out all types
	// in the assembly, including the number of methods, fields, properties
	// and events each type has.
	public class Gothic {
		public static void Run() {
			// Load GenomeLE.exe
            ModuleDefMD mod = ModuleDefMD.Load(@"c:\Program Files (x86)\Gothic 3 Modkit\GenomeLE.exe");
            mod.LoadPdb();

		    TypeDef guiFormList = mod.FindNormal("Genome.GUIFormList");
            guiFormList.ModifyAttributes(true, TypeAttributes.Public);
            TypeDef guiWindowList = mod.FindNormal("Genome.GUIWindowList");
            guiWindowList.ModifyAttributes(true, TypeAttributes.Public);

            TypeDef resources = mod.FindNormal("Genome.aCFormGUIToolboxResources");
		    MethodDef met = resources.FindMethod("m_pMenuItemAddNewPage_Click");
		    CilBody body = met.Body;
		   
            LocalList locals = body.Variables;
            locals.Clear();
            //List<Local> locals = new List<Local>();
            locals.Add(new Local(new ClassSig(mod.FindNormal("Genome.GUIContext"))));
            locals.Add(new Local(new ClassSig(mod.FindNormal("Genome.GUIFormList"))));
            locals.Add(new Local(mod.CorLibTypes.Int32));
            locals.Add(new Local(new ClassSig(mod.FindNormal("Genome.GUIForm"))));


            IList<Instruction> instructions = body.Instructions;
            instructions.Clear();
            //List<Instruction> instructions = new List<Instruction>();
            instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            instructions.Add(OpCodes.Call.ToInstruction(mod.FindNormal("Genome.aCFormGUIToolboxResources").FindMethod("get_GUIEditor")));
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.aCFormGUIEditor").FindMethod("get_CodeGen")));
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.GUICodeGen").FindMethod("SaveResourceFile")));
            instructions.Add(OpCodes.Pop.ToInstruction());

            instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            instructions.Add(OpCodes.Call.ToInstruction(resources.FindMethod("get_GUIContext")));
            instructions.Add(OpCodes.Stloc_0.ToInstruction());
            instructions.Add(OpCodes.Ldloc_0.ToInstruction());
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.GUIContext").FindMethod("get_Pages")));
            instructions.Add(OpCodes.Stloc_1.ToInstruction());
            instructions.Add(OpCodes.Ldc_I4_0.ToInstruction());
            instructions.Add(OpCodes.Stloc_2.ToInstruction());
		    Instruction firstJumpTarget = OpCodes.Ldloc_2.ToInstruction();
            instructions.Add(OpCodes.Br.ToInstruction(firstJumpTarget));
            Instruction secondJumpTarget = OpCodes.Ldloc_1.ToInstruction();
            instructions.Add(secondJumpTarget);
            instructions.Add(OpCodes.Ldloc_2.ToInstruction());
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.GUIFormList").FindMethod("get_Item")));
            instructions.Add(OpCodes.Castclass.ToInstruction(mod.FindNormal("Genome.GUIForm")));
            instructions.Add(OpCodes.Stloc_3.ToInstruction());
            instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            instructions.Add(OpCodes.Call.ToInstruction(mod.FindNormal("Genome.aCFormGUIToolboxResources").FindMethod("get_GUIEditor")));
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.aCFormGUIEditor").FindMethod("get_CodeGen")));
            instructions.Add(OpCodes.Ldloc_3.ToInstruction());
            instructions.Add(OpCodes.Callvirt.ToInstruction(mod.FindNormal("Genome.GUICodeGen").FindMethod("SaveFormFiles")));
            instructions.Add(OpCodes.Pop.ToInstruction());
            instructions.Add(OpCodes.Ldloc_2.ToInstruction());
            instructions.Add(OpCodes.Ldc_I4_1.ToInstruction());
            instructions.Add(OpCodes.Add.ToInstruction());
            instructions.Add(OpCodes.Stloc_2.ToInstruction());
            instructions.Add(firstJumpTarget);
            instructions.Add(OpCodes.Ldloc_1.ToInstruction());

            MemberRefUser getCount = new MemberRefUser(mod, "get_Count",
                MethodSig.CreateInstance(mod.CorLibTypes.Int32),
                mod.CorLibTypes.GetTypeRef("System.Collections", "CollectionBase"));
            instructions.Add(OpCodes.Callvirt.ToInstruction(getCount));
            instructions.Add(OpCodes.Blt_S.ToInstruction(secondJumpTarget));
            instructions.Add(OpCodes.Ret.ToInstruction());

           

            if (mod.IsILOnly)
            {
                var wopts = new dnlib.DotNet.Writer.ModuleWriterOptions(mod);
                wopts.WritePdb = true;
                // This assembly has only IL code, and no native code (eg. it's a C# or VB assembly)
                mod.Write(@"c:\Program Files (x86)\Gothic 3 Modkit\GenomeLE.PP.exe", wopts);
            }
            else
            {
                var wopts = new dnlib.DotNet.Writer.NativeModuleWriterOptions(mod);
                wopts.WritePdb = true;
                // This assembly has native code (eg. C++/CLI)
                mod.NativeWrite(@"c:\Program Files (x86)\Gothic 3 Modkit\GenomeLE.PP.exe", wopts);

            }

            /*int totalNumTypes = 0;
			// mod.Types only returns non-nested types.
			// mod.GetTypes() returns all types, including nested types.
			foreach (TypeDef type in mod.GetTypes()) {
				totalNumTypes++;
				Console.WriteLine();
				Console.WriteLine("Type: {0}", type.FullName);
				if (type.BaseType != null)
					Console.WriteLine("  Base type: {0}", type.BaseType.FullName);

				Console.WriteLine("  Methods: {0}", type.Methods.Count);
				Console.WriteLine("  Fields: {0}", type.Fields.Count);
				Console.WriteLine("  Properties: {0}", type.Properties.Count);
				Console.WriteLine("  Events: {0}", type.Events.Count);
				Console.WriteLine("  Nested types: {0}", type.NestedTypes.Count);

				if (type.Interfaces.Count > 0) {
					Console.WriteLine("  Interfaces:");
					foreach (InterfaceImpl iface in type.Interfaces)
						Console.WriteLine("    {0}", iface.Interface.FullName);
				}
			}
			Console.WriteLine();
			Console.WriteLine("Total number of types: {0}", totalNumTypes);*/
		}
	}
}
