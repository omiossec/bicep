// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;

        public const string ListSeparator = ", ";

        public const string ParameterKeyword = "param";
        public const string OutputKeyword = "output";
        public const string VariableKeyword = "var";
        public const string ResourceKeyword = "resource";

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol ResourceRef = new ResourceRefType();
        public static readonly TypeSymbol String = new PrimitiveType("string");
        public static readonly TypeSymbol Object = new ObjectType("object");
        public static readonly TypeSymbol Int = new PrimitiveType("int");
        public static readonly TypeSymbol Bool = new PrimitiveType("bool");
        public static readonly TypeSymbol Null = new PrimitiveType("null");
        public static readonly TypeSymbol Array = new ArrayType("array");

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new NamedObjectType(nameof(ParameterModifierMetadata), CreateParameterModifierMetadataProperties(), Any.AsReference(), TypePropertyFlags.Constant);

        public static readonly TypeSymbol Tags = new NamedObjectType(nameof(Tags), Enumerable.Empty<TypeProperty>(), String.AsReference(), TypePropertyFlags.None);

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly string DeclarationTypesString = LanguageConstants.DeclarationTypes.Keys.ConcatString(ListSeparator);

        public static TypeSymbol CreateParameterModifierType(TypeSymbol parameterType)
        {
            if (parameterType.TypeKind != TypeKind.Primitive)
            {
                throw new ArgumentException($"Modifiers are not supported for type '{parameterType.Name}'.");
            }

            return new NamedObjectType($"ParameterModifier_{parameterType.Name}", CreateParameterModifierProperties(parameterType), additionalProperties: null);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierProperties(TypeSymbol parameterType)
        {
            if (ReferenceEquals(parameterType, String) || ReferenceEquals(parameterType, Object))
            {
                // only string and object types have secure equivalents
                yield return new TypeProperty("secure", Bool.AsReference(), TypePropertyFlags.Constant);
            }

            // default value is allowed to have expressions
            yield return new TypeProperty("default", parameterType.AsReference());

            yield return new TypeProperty("allowed", new TypedArrayType(parameterType.AsReference()).AsReference(), TypePropertyFlags.Constant);

            if (ReferenceEquals(parameterType, Int))
            {
                // value constraints are valid on integer parameters only
                yield return new TypeProperty("minValue", Int.AsReference(), TypePropertyFlags.Constant);
                yield return new TypeProperty("maxValue", Int.AsReference(), TypePropertyFlags.Constant);
            }

            if (ReferenceEquals(parameterType, String) || ReferenceEquals(parameterType, Array))
            {
                // strings and arrays can have length constraints
                yield return new TypeProperty("minLength", Int.AsReference(), TypePropertyFlags.Constant);
                yield return new TypeProperty("maxLength", Int.AsReference(), TypePropertyFlags.Constant);
            }

            yield return new TypeProperty("metadata", ParameterModifierMetadata.AsReference(), TypePropertyFlags.Constant);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierMetadataProperties()
        {
            yield return new TypeProperty("description", String.AsReference(), TypePropertyFlags.Constant);
        }

        public static IEnumerable<TypeProperty> CreateResourceProperties(ResourceTypeReference resourceTypeReference)
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            yield return new TypeProperty("id", String.AsReference(), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("name", String.AsReference(), TypePropertyFlags.Required | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("type", new StringLiteralType(resourceTypeReference.FullyQualifiedType).AsReference(), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("apiVersion", new StringLiteralType(resourceTypeReference.ApiVersion).AsReference(), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            // TODO: Model type fully
            yield return new TypeProperty("sku", Object.AsReference());

            yield return new TypeProperty("kind", String.AsReference());
            yield return new TypeProperty("managedBy", String.AsReference());

            var stringArray = new TypedArrayType(String.AsReference());
            yield return new TypeProperty("managedByExtended", stringArray.AsReference());

            yield return new TypeProperty("location", String.AsReference());

            // TODO: Model type fully
            yield return new TypeProperty("extendedLocation", Object.AsReference());

            yield return new TypeProperty("zones", stringArray.AsReference());

            yield return new TypeProperty("plan", Object.AsReference());

            yield return new TypeProperty("eTag", String.AsReference());

            yield return new TypeProperty("tags", Tags.AsReference());

            // TODO: Model type fully
            yield return new TypeProperty("scale", Object.AsReference());

            // TODO: Model type fully
            yield return new TypeProperty("identity", Object.AsReference());

            yield return new TypeProperty("properties", Object.AsReference());

            var resourceRefArray = new TypedArrayType(ResourceRef.AsReference());
            yield return new TypeProperty("dependsOn", resourceRefArray.AsReference(), TypePropertyFlags.WriteOnly);
        }
    }
}