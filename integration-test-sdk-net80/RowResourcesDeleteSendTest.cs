﻿using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class RowResourcesDeleteSendTest
    {
        [TestMethod]
        public void TestRowDeleteSendResources()
        {
            SmartsheetClient smartsheet = new SmartsheetBuilder().SetMaxRetryTimeout(30000).Build();
            long templateId = smartsheet.TemplateResources.ListPublicTemplates().Data[0].Id.GetValueOrDefault();
            Assert.IsNotNull(templateId);

            long sheetId = CreateSheetFromTemplate(smartsheet, templateId);

            PaginatedResult<Column> columnsResult = smartsheet.SheetResources.ColumnResources.ListColumns(sheetId);
            long columnId = columnsResult.Data[0].Id.GetValueOrDefault();
            Assert.IsNotNull(columnId);

            Cell[] cellsToAdd = new Cell[] { new Cell.AddCellBuilder(columnId, true).SetValue("hello").SetStrict(false).Build() };

            IList<long> rowIds = AddRows(smartsheet, sheetId, columnId, cellsToAdd);

            MultiRowEmail multiEmail = new MultiRowEmail 
            {
                RowIds = rowIds,
                IncludeAttachments = true,
                IncludeDiscussions = true,
                SendTo = new Recipient[]
                {
                    new Recipient
                    {
                        Email = "eric.yan@smartsheet.com"
                    }
                },
                Subject = "hello",
                Message = "tada"
            };
            smartsheet.SheetResources.RowResources.SendRows(sheetId, multiEmail);

            DeleteRows(smartsheet, sheetId, rowIds);

            smartsheet.SheetResources.DeleteSheet(sheetId);

        }

        private static void DeleteRows(SmartsheetClient smartsheet, long sheetId, IList<long> rowIds)
        {

            IList<long> rowsDeleted = smartsheet.SheetResources.RowResources.DeleteRows(sheetId, rowIds, false);
            Assert.IsTrue(rowsDeleted.Contains(rowIds[0]));
            Assert.IsTrue(rowsDeleted.Contains(rowIds[1]));
            Assert.IsTrue(rowsDeleted.Contains(rowIds[2]));

        }

        private static void CopyRowToCreatedSheet(SmartsheetClient smartsheet, long sheetId, long rowId)
        {
            var tempSheet = smartsheet.SheetResources.CreateSheet(new Sheet.CreateSheetBuilder("tempSheet", new Column[] { new Column.CreateSheetColumnBuilder("col1", true, ColumnType.TEXT_NUMBER).Build() }).Build()).Id;
            Assert.IsNotNull(tempSheet);
            long tempSheetId = tempSheet.Value;
            CopyOrMoveRowDestination destination = new CopyOrMoveRowDestination { SheetId = tempSheetId };
            CopyOrMoveRowDirective directive = new CopyOrMoveRowDirective { RowIds = new long[] { rowId }, To = destination };
            CopyOrMoveRowResult result = smartsheet.SheetResources.RowResources.CopyRowsToAnotherSheet(sheetId, directive, new CopyRowInclusion[] { CopyRowInclusion.CHILDREN }, false);
            smartsheet.SheetResources.DeleteSheet(tempSheetId);
        }

        private static IList<long> AddRows(SmartsheetClient smartsheet, long sheetId, long columnId, Cell[] cellsToAdd)
        {
            Row row1 = new Row.AddRowBuilder(true, null, null, null, null).SetCells(cellsToAdd).Build();
            Row row2 = new Row.AddRowBuilder(true, null, null, null, null).SetCells(cellsToAdd).Build();
            Row row3 = new Row.AddRowBuilder(true, null, null, null, null).SetCells(cellsToAdd).Build();

            IList<Row> rows = smartsheet.SheetResources.RowResources.AddRows(sheetId, new Row[] { row1, row2, row3 });
            IList<long> rowIds = new List<long>();
            foreach (Row row in rows)
            {
                Assert.IsNotNull(row.Id);
                rowIds.Add(row.Id.Value);
            }
            return rowIds;
        }


        private static long CreateSheetFromTemplate(SmartsheetClient smartsheet, long templateId)
        {
            // Create a new sheet off of that template.
            Sheet newSheet = smartsheet.SheetResources.CreateSheetFromTemplate(new Sheet.CreateSheetFromTemplateBuilder("New Sheet", templateId).Build(), new TemplateInclusion[] { TemplateInclusion.DATA, TemplateInclusion.RULE_RECIPIENTS, TemplateInclusion.RULES });
            Assert.IsNotNull(newSheet.Id);
            return newSheet.Id.Value;
        }
    }
}