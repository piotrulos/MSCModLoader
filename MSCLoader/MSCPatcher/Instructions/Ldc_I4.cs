using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher.Instructions
{
    public class Ldc_I4
    {
        public static Instruction ParseInstruction(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            int value = int.Parse(instrXML.Attribute("Value").Value);
            return processor.Create(OpCodes.Ldc_I4, value);
        }
    }
}