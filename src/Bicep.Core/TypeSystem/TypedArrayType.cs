// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class TypedArrayType : ArrayType
    {
        public TypedArrayType(TypeReference itemReference)
            : base(itemReference.Type.Name + "[]")
        {
            this.Item = itemReference;
        }

        public override TypeReference Item { get; }
    }
}
