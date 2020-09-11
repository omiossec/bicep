// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        private UnionType(string name, ImmutableArray<TypeReference> members)
            : base(name)
        {
            this.Members = members;
        }

        public override TypeKind TypeKind => this.Members.Any() ? TypeKind.Union : TypeKind.Never;

        public ImmutableArray<TypeReference> Members { get; }

        public static TypeSymbol Create(IEnumerable<TypeReference> unionMembers)
        {
            // flatten and then de-duplicate members
            var finalMembers = FlattenMembers(unionMembers)
                .Distinct()
                .OrderBy(m => m.Type.Name, StringComparer.Ordinal)
                .ToImmutableArray();

            return finalMembers.Length switch
            {
                0 => new UnionType("never", ImmutableArray<TypeReference>.Empty),
                1 => finalMembers[0].Type,
                _ => new UnionType(FormatName(finalMembers), finalMembers)
            };
        }

        public static TypeSymbol Create(params TypeReference[] members) => Create((IEnumerable<TypeReference>) members);

        public static TypeSymbol Create(params TypeSymbol[] members) => Create(members.Select(x => x.AsReference()));

        private static IEnumerable<TypeReference> FlattenMembers(IEnumerable<TypeReference> members) => 
            members.SelectMany(member => member.Type is UnionType union 
                ? FlattenMembers(union.Members)
                : member.AsEnumerable());

        private static string FormatName(IEnumerable<TypeReference> unionMembers) => unionMembers.Select(m => m.Type.Name).ConcatString(" | ");
    }
}

