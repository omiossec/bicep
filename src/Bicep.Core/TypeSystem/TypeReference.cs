// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    public class TypeReference
    {
        private readonly Func<TypeSymbol> typeGetterFunc;

        public TypeReference(TypeSymbol type)
            : this(() => type)
        {
        }

        public TypeReference(Func<TypeSymbol> typeGetterFunc)
        {
            this.typeGetterFunc = typeGetterFunc;
        }

        public TypeSymbol Type => typeGetterFunc();

        public override bool Equals(object obj)
            => obj is TypeReference other && other.Type == this.Type;

        public override int GetHashCode()
            => this.Type.GetHashCode();

        public override string ToString()
            => this.Type.ToString();
    }
}