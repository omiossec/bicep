// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class DiscriminatedObjectTypeTests
    {
        [TestMethod]
        public void DiscriminatedObjectType_should_be_correctly_instantiated()
        {
            var namedObjectA = new NamedObjectType("objA", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA").AsReference()),
                new TypeProperty("keyAProp", LanguageConstants.String.AsReference()),
            }, null);

            var namedObjectB = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyB").AsReference()),
                new TypeProperty("keyBProp", LanguageConstants.String.AsReference()),
            }, null);

            var discObj = new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA.AsReference(), namedObjectB.AsReference() });

            discObj.UnionMembersByKey.Keys.Should().BeEquivalentTo("'keyA'", "'keyB'");
            discObj.TypeKind.Should().Be(TypeKind.DiscriminatedObject);

            discObj.UnionMembersByKey[new StringLiteralType("keyA").Name].Type.Should().Be(namedObjectA);
            discObj.UnionMembersByKey[new StringLiteralType("keyB").Name].Type.Should().Be(namedObjectB);
        }

        [TestMethod]
        public void DiscriminatedObject_should_throw_for_various_badly_formatted_object_arguments()
        {
            var namedObjectA = new NamedObjectType("objA", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA").AsReference()),
                new TypeProperty("keyAProp", LanguageConstants.String.AsReference()),
            }, null);

            var missingKeyObject = new NamedObjectType("objB", new []
            {
                new TypeProperty("keyBProp", LanguageConstants.String.AsReference()),
            }, null);
            Action missingKeyConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA.AsReference(), missingKeyObject.AsReference() });
            missingKeyConstructorAction.Should().Throw<ArgumentException>();

            var invalidKeyTypeObject = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", LanguageConstants.String.AsReference()),
                new TypeProperty("keyBProp", LanguageConstants.String.AsReference()),
            }, null);
            Action invalidKeyTypeConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA.AsReference(), invalidKeyTypeObject.AsReference() });
            invalidKeyTypeConstructorAction.Should().Throw<ArgumentException>();

            var duplicateKeyObject = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA").AsReference()),
                new TypeProperty("keyBProp", LanguageConstants.String.AsReference()),
            }, null);
            Action duplicateKeyConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA.AsReference(), duplicateKeyObject.AsReference() });
            duplicateKeyConstructorAction.Should().Throw<ArgumentException>();
        }
    }
}