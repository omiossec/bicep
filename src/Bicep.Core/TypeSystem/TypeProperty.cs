// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class TypeProperty
    {
        public TypeProperty(string name, TypeReference typeReference, TypePropertyFlags flags = TypePropertyFlags.None)
        {
            this.Name = name;
            this.TypeReference = typeReference;
            this.Flags = flags;
        }

        public string Name { get; }

        public TypeReference TypeReference { get; }

        public TypePropertyFlags Flags { get; }
    }
}
