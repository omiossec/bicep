// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Syntax
{
    public class ProgramSyntax : SyntaxBase
    {
        public ProgramSyntax(SeparatedSyntaxList statements, Token endOfFile, IEnumerable<Diagnostic> lexerDiagnostics)
        {
            this.Statements = statements;
            this.EndOfFile = endOfFile;
            this.LexerDiagnostics = lexerDiagnostics.ToImmutableArray();
        }

        public SeparatedSyntaxList Statements { get; }

        public Token EndOfFile { get; }

        public ImmutableArray<Diagnostic> LexerDiagnostics { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span => TextSpan.Between(Statements, EndOfFile);
    }
}
