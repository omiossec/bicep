// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public static class TypeSymbolExtensions
    {
        public static TypeReference AsReference(this TypeSymbol symbol)
            => new TypeReference(symbol);
    }
}
