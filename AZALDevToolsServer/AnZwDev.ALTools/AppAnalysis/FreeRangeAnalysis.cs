using AnZwDev.ALTools.ALSymbols;
using AnZwDev.ALTools.ALSymbols.SymbolReaders;
using Microsoft.Dynamics.Nav.CodeAnalysis.CommandLine;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace AnZwDev.ALTools.AppAnalysis
{
  public class FreeRangeAnalysis
  {
    private string ProjectPath { get; set; }

    public FreeRangeAnalysis(string projectPath)
    {
      ProjectPath = projectPath;
    }

    public List<KeyValuePair<string, List<int>>> AnalyzeProjectFreeRange()
    {

      ALProjectFile projectFile = ALProjectFile.Load(Path.Combine(this.ProjectPath, "app.json"));

      ALSymbolInfoSyntaxTreeReader syntaxTreeReader = new ALSymbolInfoSyntaxTreeReader(false);
      string[] files = System.IO.Directory.GetFiles(ProjectPath, "*.al", SearchOption.AllDirectories);

      List<SymbolsIDs> report = new List<SymbolsIDs>();

      for (int i = 0; i < files.Length; i++)
      {
        ALSymbolInformation documentSymbols = syntaxTreeReader.ProcessSourceFile(files[i]);


        if ((documentSymbols.childSymbols != null))
        {
          documentSymbols.childSymbols.ForEach(doc =>
          {
            var objID = doc.id;
            if (!report.Contains(new SymbolsIDs() { Type = doc.kind, id = objID }))
              report.Add(new SymbolsIDs() { Type = doc.kind, id = objID });

            /*if (doc.kind == ALSymbolKind.TableExtensionObject)
              doc.childSymbols.Where(t => t.kind == ALSymbolKind.FieldList).ToList().ForEach(FieldList =>
              {
                FieldList.childSymbols.ForEach(symbol =>
                {
                  var fieldID = symbol.id;

                  if (!report.Contains(new SymbolsIDs() { Type = symbol.kind, id = fieldID }))
                    report.Add(new SymbolsIDs() { Type = symbol.kind, id = fieldID });
                });
              });*/
          });

        }
      }
      List<int> availableIds = new List<int>();
      availableIds = availableIds.Distinct().ToList();

      projectFile.idRanges.ForEach(range =>
      {
        availableIds.AddRange(Enumerable.Range(range.from, (range.to - (range.from - 1))));
      });
      availableIds = availableIds.Distinct().ToList();

      var FreePageId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.PageObject).Select(o => o.id).ToList(), availableIds);
      var FreeTableId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.TableObject).Select(o => o.id).ToList(), availableIds);
      var FreeCodeunitId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.CodeunitObject).Select(o => o.id).ToList(), availableIds);
      var FreePageExtId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.PageExtensionObject).Select(o => o.id).ToList(), availableIds);
      var FreeTableExtId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.TableExtensionObject).Select(o => o.id).ToList(), availableIds);
      var FreeEnumExtId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.EnumExtensionType).Select(o => o.id).ToList(), availableIds);
      var FreeEnumId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.EnumType).Select(o => o.id).ToList(), availableIds);
      var FreeReportId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.ReportObject).Select(o => o.id).ToList(), availableIds);
      var FreeXmlPortId = GetAvailableIds(report.Where(obj => obj.Type == ALSymbolKind.XmlPortObject).Select(o => o.id).ToList(), availableIds);

      List<KeyValuePair<string, List<int>>> results = new List<KeyValuePair<string, List<int>>>();
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.PageObject.ToName(), FreePageId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.TableObject.ToName(), FreeTableId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.CodeunitObject.ToName(), FreeCodeunitId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.PageExtensionObject.ToName(), FreePageExtId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.TableExtensionObject.ToName(), FreeTableExtId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.EnumExtensionType.ToName(), FreeEnumExtId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.EnumType.ToName(), FreeEnumId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.ReportObject.ToName(), FreeReportId));
      results.Add(new KeyValuePair<string, List<int>>(ALSymbolKind.XmlPortObject.ToName(), FreeXmlPortId));


      return results;
    }

    private List<int> GetAvailableIds(List<int> UsedIds, List<int> availableIds)
    {
      return availableIds.Except(UsedIds).ToList();
    }

    struct SymbolsIDs
    {
      public ALSymbolKind Type;
      public int id;
    }
  }
}
