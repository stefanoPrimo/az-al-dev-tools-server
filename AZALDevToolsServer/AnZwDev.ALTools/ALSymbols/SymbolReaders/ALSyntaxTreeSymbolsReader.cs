﻿using AnZwDev.ALTools.Extensions;
using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnZwDev.ALTools.ALSymbols.SymbolReaders
{
    public class ALSyntaxTreeSymbolsReader
    {

        public ALSyntaxTreeSymbolsReader()
        {
        }

        public ALSyntaxTreeSymbol ProcessSourceFile(string path)
        {
            string sourceCode;
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                sourceCode = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                return ProcessSourceCode(sourceCode);
            }
            catch (Exception e)
            {
                return new ALSyntaxTreeSymbol(ALSymbolKind.Undefined, "LangServer Error: " + e.Message);
            }
        }


        protected SyntaxTree ParseObjectText(string source)
        {
            return SyntaxTree.ParseObjectText(source);
        }

        protected SyntaxTree ParseObjectTextNav2018(string source)
        {
            return typeof(SyntaxTree).CallStaticMethod<SyntaxTree>("ParseObjectText", source,
                Type.Missing, Type.Missing, Type.Missing);
        }

        public ALSyntaxTreeSymbol ProcessSourceCode(string source)
        {
            SyntaxTree syntaxTree = null;

            try
            {
                syntaxTree = this.ParseObjectText(source);
            }
            catch (MissingMethodException e)
            {
                syntaxTree = this.ParseObjectTextNav2018(source);
            }

            if (syntaxTree != null)
            {
                SyntaxNode node = syntaxTree.GetRoot();
                if (node != null)
                    return ProcessSyntaxTreeNode(syntaxTree, node);
            }
            return null;
        }

        protected ALSyntaxTreeSymbol ProcessSyntaxTreeNode(SyntaxTree syntaxTree, SyntaxNode node)
        {
            ALSyntaxTreeSymbol symbolInfo = new ALSyntaxTreeSymbol();
            symbolInfo.kind = ALSymbolKind.SyntaxTreeNode;
            symbolInfo.name = node.Kind.ToString();
            symbolInfo.fullName = symbolInfo.name + " " + node.FullSpan.ToString();
            symbolInfo.syntaxTreeNode = node;
            symbolInfo.type = node.GetType().Name;

            if (node.ContainsDiagnostics)
                symbolInfo.containsDiagnostics = true;

            var lineSpan = syntaxTree.GetLineSpan(node.FullSpan);
            symbolInfo.range = new Range(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character,
                lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character);

            lineSpan = syntaxTree.GetLineSpan(node.Span);
            symbolInfo.selectionRange = new Range(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character,
                lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character);
            
            IEnumerable<SyntaxNode> list = node.ChildNodes();
            if (list != null)
            {
                foreach (SyntaxNode childNode in list)
                {
                    symbolInfo.AddChildSymbol(ProcessSyntaxTreeNode(syntaxTree, childNode));
                }
            }

            return symbolInfo;
        }


    }
}
