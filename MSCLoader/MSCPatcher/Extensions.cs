using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace MSCPatcher
{
    public static class Extensions
    {
        public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType)
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                DeclaringType = self.DeclaringType.MakeGenericInstanceType(arguments),
                CallingConvention = self.CallingConvention,
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));

            return reference;
        }
    }
}
