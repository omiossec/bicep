// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class DiscriminatedObjectType : TypeSymbol
    {
        public DiscriminatedObjectType(string name, string discriminatorKey, IEnumerable<TypeReference> unionMembers)
            : base(name)
        {
            var unionMembersByKey = new Dictionary<string, TypeReference>();
            foreach (var member in unionMembers)
            {
                if (!(member.Type is NamedObjectType namedObject))
                {
                    throw new ArgumentException($"Invalid member of type {member.Type.GetType()}");
                }

                if (!namedObject.Properties.TryGetValue(discriminatorKey, out var discriminatorProp))
                {
                    throw new ArgumentException("Missing discriminator field on member");
                }

                if (!(discriminatorProp.TypeReference.Type is StringLiteralType stringLiteral))
                {
                    throw new ArgumentException($"Invalid discriminator field type {discriminatorProp.TypeReference.Type.Name} on member");
                }

                if (unionMembersByKey.ContainsKey(stringLiteral.Name))
                {
                    throw new ArgumentException($"Duplicate discriminator field {stringLiteral.Name} on member");
                }

                unionMembersByKey[stringLiteral.Name] = member;
            }

            this.DiscriminatorKey = discriminatorKey;
            this.UnionMembersByKey = unionMembersByKey.ToImmutableDictionary();
        }

        public override TypeKind TypeKind => TypeKind.DiscriminatedObject;

        public string DiscriminatorKey { get; }

        public ImmutableDictionary<string, TypeReference> UnionMembersByKey { get; }
    }
}
