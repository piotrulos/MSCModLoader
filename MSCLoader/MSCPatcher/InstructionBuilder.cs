using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher
{
    public class InstructionBuilder
    {
        public static Instruction Build(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            var opCode = instrXML.Attribute("OpCode").Value;

            switch (opCode)
            {
                case "Add":
                    return Instructions.Add.ParseInstruction(processor, type, instrXML);
                case "Brfalse.S":
                    return Instructions.Brfalse_S.ParseInstruction(processor, type, instrXML);
                case "Brtrue.S":
                    return Instructions.Brtrue_S.ParseInstruction(processor, type, instrXML);
                case "Call":
                    return Instructions.Call.ParseInstruction(processor, type, instrXML);
                case "Ceq":
                    return Instructions.Ceq.ParseInstruction(processor, type, instrXML);
                case "Div":
                    return Instructions.Div.ParseInstruction(processor, type, instrXML);
                case "Ldarg.0":
                    return Instructions.Ldarg_0.ParseInstruction(processor, type, instrXML);
                case "Ldc.I4":
                    return Instructions.Ldc_I4.ParseInstruction(processor, type, instrXML);
                case "Ldfld":
                    return Instructions.Ldfld.ParseInstruction(processor, type, instrXML);
                case "Ldloc.0":
                    return Instructions.Ldloc_0.ParseInstruction(processor, type, instrXML);
                case "Ldloc.S":
                    return Instructions.Ldloc_S.ParseInstruction(processor, type, instrXML);
                case "Ldnull":
                    return Instructions.Ldnull.ParseInstruction(processor, type, instrXML);
                case "Mul":
                    return Instructions.Mul.ParseInstruction(processor, type, instrXML);
                case "Ret":
                    return Instructions.Ret.ParseInstruction(processor, type, instrXML);
                case "Stloc.0":
                    return Instructions.Stloc_0.ParseInstruction(processor, type, instrXML);
                case "Stloc.S":
                    return Instructions.Stloc_S.ParseInstruction(processor, type, instrXML);
                case "Sub":
                    return Instructions.Sub.ParseInstruction(processor, type, instrXML);
                default:
                    Console.WriteLine("Unable to locate OpCode {0}!", opCode);
                    return null;
            }
        }
    }
}
