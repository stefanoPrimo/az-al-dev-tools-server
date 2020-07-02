using AnZwDev.ALTools;
using AnZwDev.ALTools.ALSymbols;
using AnZwDev.ALTools.ALSymbols.SymbolReaders;
using AnZwDev.ALTools.CodeTransformations;
using AnZwDev.ALTools.AppAnalysis;
using System;
using System.IO;
using Newtonsoft.Json;

namespace AZALDevToolsTestConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      ALDevToolsServer alDevToolsServer = new ALDevToolsServer("C:\\Users\\sprimo\\.vscode\\extensions\\ms-dynamics-smb.al-5.0.288712");
      //
      string filePath = "C:\\Dati\\EOS\\_Extension\\Eos.EX009.AdvancedDocumentReporting\\BC16\\Source\\PageExt\\pageextension 41 Sales Quote.al";
      //string filePath = "C:\\Dati\\EOS\\_Extension\\Eos.EX009.AdvancedDocumentReporting\\BC16";

      ALSymbolInfoSyntaxTreeReader reader = new ALSymbolInfoSyntaxTreeReader(true);
      var test = reader.ProcessSourceFile(filePath);
      /*FreeRangeAnalysis test = new FreeRangeAnalysis(filePath);
      var res = test.AnalyzeProjectFreeRange();
      var ResJson = JsonConvert.SerializeObject(res);
      File.WriteAllText("c:\\_MyTmp\\test.json", ResJson);*/
      Console.WriteLine("");

    }
  }
}
