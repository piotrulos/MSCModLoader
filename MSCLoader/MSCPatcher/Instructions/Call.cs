using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MSCPatcher.Instructions
{
    public class Call 
    {
        public static Instruction ParseInstruction(ILProcessor processor, TypeDefinition type, XElement instrXML)
        {
            string assemblyName = instrXML.Attribute("Assembly").Value;
            string typeName = instrXML.Attribute("Type").Value;
            string methodName = instrXML.Attribute("Method").Value;

            // Get Assembly
            AssemblyDefinition assembly = Program.GetAssembly(assemblyName);

            if (assembly == null)
                return null;

            // Get Type
            TypeDefinition typeDefinition = assembly.MainModule.GetType(typeName);

            // Get Method
            MethodDefinition methodDefinition = typeDefinition.Methods.Single(m => m.Name == methodName);
            MethodReference methodReference =
                Program.GetAssembly("Assembly-CSharp").MainModule.Import(methodDefinition);

            // Generic Parameter Check
            if (instrXML.HasElements)
            {
                List<TypeReference> genericParameters = new List<TypeReference>();

                foreach (XElement genericParameter in instrXML.Elements("GenericParameter"))
                {
                    var gPAssemblyName = genericParameter.Attribute("Assembly").Value;
                    var gPType = genericParameter.Attribute("Type").Value;

                    AssemblyDefinition gPAssembly = Program.GetAssembly(gPAssemblyName);
                    TypeDefinition gPTypeDefinition = gPAssembly.MainModule.GetType(gPType);
                    TypeReference gPTypeReference =
                        Program.GetAssembly("Assembly-CSharp").MainModule.Import(gPTypeDefinition);

                    genericParameters.Add(gPTypeReference);
                }

                methodReference = methodReference.MakeGeneric(genericParameters.ToArray());
            }

            // Return Instruction
            return processor.Create(OpCodes.Call, methodReference);
        }
    }
}
