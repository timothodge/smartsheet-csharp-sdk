﻿using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class SheetResourcesUpdateRequestTest
    {
        [TestMethod]
        public void TestSheetUpdateRequestResources()
        {
            SmartsheetClient smartsheet = new SmartsheetBuilder().SetMaxRetryTimeout(30000).Build();

            long sheetId = CreateSheet(smartsheet);

            PaginatedResult<Column> columnsResult = smartsheet.SheetResources.ColumnResources.ListColumns(sheetId);
            var columnsResultDataId = columnsResult.Data[0].Id;
            Assert.IsNotNull(columnsResultDataId);
            long columnId = columnsResultDataId.Value;

            Cell[] cellsToAdd = new Cell[] { new Cell.AddCellBuilder(columnId, true).SetValue("hello").SetStrict(false).Build() };

            IList<long> rowIds = AddRows(smartsheet, sheetId, columnId, cellsToAdd);

            UpdateRequest updateReq = new UpdateRequest
            {
                RowIds = rowIds,
                IncludeAttachments = true,
                IncludeDiscussions = true,
                CcMe = true,
                SendTo = new Recipient[]
                {
                    new Recipient
                    {
                        Email = "ericyan99@outlook.com"
                    }
                },
                Subject = "hello",
                Message = "tada"
            };
            UpdateRequest updateRequest = smartsheet.SheetResources.UpdateRequestResources.CreateUpdateRequest(sheetId, updateReq);

            Assert.IsNotNull(updateRequest.Id);

            Sheet a = smartsheet.SheetResources.GetSheet(sheetId, new SheetLevelInclusion[] { SheetLevelInclusion.ROW_PERMALINK });

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

        private static long CreateSheet(SmartsheetClient smartsheet)
        {
            Column[] columnsToCreate = new Column[] {
            new Column.CreateSheetColumnBuilder("col 1", true, ColumnType.TEXT_NUMBER).Build(),
            new Column.CreateSheetColumnBuilder("col 2", false, ColumnType.DATE).Build(),
            new Column.CreateSheetColumnBuilder("col 3", false, ColumnType.TEXT_NUMBER).Build(),
            };
            Sheet createdSheet = smartsheet.SheetResources.CreateSheet(new Sheet.CreateSheetBuilder("new sheet", columnsToCreate).Build());
            Assert.IsTrue(createdSheet.Columns.Count == 3);
            Assert.IsTrue(createdSheet.Columns[1].Title == "col 2");
            Assert.IsNotNull(createdSheet.Id);
            return createdSheet.Id.Value;
        }
    }
}