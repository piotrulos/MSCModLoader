using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher.Instructions
{
    public class Ldloc_0
    {
        public static Instruction ParseInstruction(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            return processor.Create(OpCodes.Ldloc_0);
        }
    }
}