// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public static class SyntaxBaseExtensions
    {
        public static SyntaxBase? TryFindMostSpecificNodeInclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) => 
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: true);

        public static SyntaxBase? TryFindMostSpecificNodeExclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) => 
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: false);

        public static IList<Token> GetTokens(this SyntaxBase root)
        {
            var tokens = new List<Token>();
            var visitor = new TokenCollectorVisitor(tokens);
            visitor.Visit(root);

            return tokens;
        }

        private static SyntaxBase? TryFindMostSpecificNodeInternal(SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
        {
            var visitor = new NavigationSearchVisitor(offset, predicate, inclusive);
            visitor.Visit(root);

            return visitor.Result;
        }

        private sealed class NavigationSearchVisitor : SyntaxVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxBase, bool> predicate;
            private readonly bool inclusive;

            public NavigationSearchVisitor(int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
            {
                this.offset = offset;
                this.predicate = predicate;
                this.inclusive = inclusive;
            }

            public SyntaxBase? Result { get; private set; }

            protected override void VisitInternal(SyntaxBase node)
            {
                // check if offset is inside the node's span
                if (CheckNodeContainsOffset(node))
                {
                    // the node span contains the offset
                    // check the predicate
                    if (this.predicate(node))
                    {
                        // store the potential result
                        this.Result = node;
                    }

                    // visiting the children may find a more specific node
                    base.VisitInternal(node);
                }

                // the offset is outside of the node span
                // there's no point to visit the children
            }

            private bool CheckNodeContainsOffset(SyntaxBase node) => this.inclusive
                    ? node.Span.ContainsInclusive(offset)
                    : node.Span.Contains(offset);
        }

        private sealed class TokenCollectorVisitor : SyntaxVisitor
        {
            private readonly IList<Token> tokens;

            public TokenCollectorVisitor(IList<Token> tokens)
            {
                this.tokens = tokens;
            }

            protected override void VisitTokenInternal(Token token)
            {
                this.tokens.Add(token);
                base.VisitTokenInternal(token);
            }
        }
    }
}

