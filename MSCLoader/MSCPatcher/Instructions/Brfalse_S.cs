using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher.Instructions
{
    public class Brfalse_S
    {
        public static Instruction ParseInstruction(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            int value = int.Parse(instrXML.Attribute("Value").Value);
            return processor.Create(OpCodes.Brfalse_S, processor.Body.Instructions[value]);
        }
    }
}