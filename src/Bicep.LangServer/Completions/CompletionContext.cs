// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions
{
    public class CompletionContext
    {
        public CompletionContext(CompletionContextKind kind)
        {
            this.Kind = kind;
        }

        public CompletionContextKind Kind { get; }

        public static CompletionContext Create(ProgramSyntax syntax, int offset)
        {
            var node = syntax.TryFindMostSpecificNodeExclusive(offset, current => true);
            if (node == null)
            {
                return new CompletionContext(CompletionContextKind.None);
            }

            var kind = IsDeclarationContext(syntax, offset, node) ? CompletionContextKind.Declaration : CompletionContextKind.None;

            return new CompletionContext(kind);
        }

        private static bool IsDeclarationContext(ProgramSyntax syntax, int offset, SyntaxBase mostSpecificNode)
        {
            var tokens = syntax.GetTokens();
            var tokenIndex = IndexOf(tokens, offset);

            if (tokenIndex < 0)
            {
                // somehow there's no token that that overlaps with the offset
                return false;
            }

            if (tokenIndex == 0 || mostSpecificNode is NoOpDeclarationSyntax)
            {
                // we are in the first token, which is the beginning of a declaration
                // or we are inside a noop declaration
                return true;
            }

            var current = tokens[tokenIndex];
            var previous = tokens[tokenIndex - 1];

            return previous.Type == TokenType.NewLine &&
                   current.Type == TokenType.Identifier &&
                   LanguageConstants.OutputKeyword.Contains(current.Text);
        }

        private static int IndexOf(IList<Token> tokens, int offset)
        {
            for(int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Span.Contains(offset))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
