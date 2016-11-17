using System;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher.Instructions
{
    public class Ldfld
    {
        public static Instruction ParseInstruction(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            string fieldName = instrXML.Attribute("Field").Value;
            FieldDefinition field = type.Fields.FirstOrDefault(f => f.Name == fieldName);

            if (field == null)
            {
                Console.WriteLine("Couldn't find field named " + fieldName);
                return null;
            }

            return processor.Create(OpCodes.Ldfld, field);
        }
    }
}