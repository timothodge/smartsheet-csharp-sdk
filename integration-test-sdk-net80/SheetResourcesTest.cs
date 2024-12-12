using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class SheetResourcesTest : TestResources
    {
        private Sheet? sheetHome;
        private Sheet? newSheetHome;
        private Sheet? newSheetTemplate;
        private Folder? folder;
        private Workspace? workspace;

        [TestInitialize]
        public void TestInitialize()
        {
            smartsheet = CreateClient();
        }

        [TestMethod]
        public void TestSheetResources()
        {
            TestCreateSheetHome();
            TestCopySheet();
            TestMoveSheet();
            TestCreateSheetHomeFromTemplate();
            TestCreateSheetInFolder();
            TestCreateSheetInFolderFromTemplate();
            TestCreateSheetInWorkspace();
            TestCreateSheetInWorkspaceFromTemplate();
            TestGetSheet();
            TestGetSheetVersion();
            TestGetSheetAsExcel();
            TestGetSheetAsPDF();
            TestGetPublishStatus();
            TestPublishSheetDefaults();
            TestPublishSheet();
            TestUpdateSheet();
            TestListSheets();
            TestSendSheet();
            TestCreateUpdateRequest();
            TestSortSheet();
            TestDeleteSheet();
        }

        private void TestCreateSheetHome()
        {
            sheetHome = CreateSheetObject();
            Assert.IsNotNull(smartsheet);
            newSheetHome = smartsheet.SheetResources.CreateSheet(sheetHome);
            if(newSheetHome.Columns.Count != sheetHome.Columns.Count)
            {
                Assert.Fail("SheetResourcesTest.TestCreateSheetHome Failed to create sheet");
            }
        }

        private void TestCopySheet()
        {
            Folder folder = CreateFolderHome();

            ContainerDestination destination = new ContainerDestination();
            destination.DestinationType = DestinationType.FOLDER;
            destination.DestinationId = folder.Id;
            destination.NewName = "CSharp SDK Copied Sheet";
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            Sheet sheet = smartsheet.SheetResources.CopySheet(newSheetHome.Id.Value, destination,
                new List<SheetCopyInclusion> { SheetCopyInclusion.ALL });
            Assert.IsTrue(sheet.Name == "CSharp SDK Copied Sheet");

            destination.NewName = "CSharp SDK Copied Sheet Without Inclusions";
            sheet = smartsheet.SheetResources.CopySheet(newSheetHome.Id.Value, destination);
            Assert.IsTrue(sheet.Name == "CSharp SDK Copied Sheet Without Inclusions");
            Assert.IsNotNull(folder.Id);
            DeleteFolder(folder.Id.Value);
        }

        private void TestMoveSheet()
        {
            Folder folder = CreateFolderHome();
            Assert.IsNotNull(smartsheet);
            Sheet sheet = smartsheet.SheetResources.CreateSheet(CreateSheetObject());

            ContainerDestination destination = new ContainerDestination();
            destination.DestinationType = DestinationType.FOLDER;
            destination.DestinationId = folder.Id;

            Assert.IsNotNull(sheet.Id);
            Sheet movedSheet = smartsheet.SheetResources.MoveSheet(sheet.Id.Value, destination);
            Assert.IsNotNull(movedSheet);
            Assert.IsNotNull(movedSheet.Id);
            DeleteSheet(movedSheet.Id.Value);
            Assert.IsNotNull(folder.Id);
            DeleteFolder(folder.Id.Value);
        }

        private void TestCreateSheetHomeFromTemplate()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Sheet sheet = new Sheet.CreateSheetFromTemplateBuilder("CSharp SDK Sheet from Template", newSheetHome.Id.Value).Build();
            Assert.IsNotNull(smartsheet);
            newSheetTemplate = smartsheet.SheetResources.CreateSheetFromTemplate(sheet,
                new List<TemplateInclusion> { TemplateInclusion.ATTACHMENTS, TemplateInclusion.DATA, TemplateInclusion.DISCUSSIONS, TemplateInclusion.RULE_RECIPIENTS, TemplateInclusion.RULES });
            Assert.AreEqual(AccessLevel.OWNER, newSheetTemplate.AccessLevel);
        }

        private void TestCreateSheetInFolder()
        {
            folder = CreateFolderHome();
            Assert.IsNotNull(folder.Id);
            Assert.IsNotNull(smartsheet);
            Assert.IsNotNull(sheetHome);
            Sheet newSheetFolder = smartsheet.FolderResources.SheetResources.CreateSheet(folder.Id.Value, sheetHome);

            if(newSheetFolder.Columns.Count != sheetHome.Columns.Count)
            {
                Assert.Fail("SheetResourcesTest.TestCreateSheetInFolder Failed to create sheet");
            }
        }

        private void TestCreateSheetInFolderFromTemplate()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(folder?.Id);
            Sheet sheet = new Sheet.CreateSheetFromTemplateBuilder("CSharp SDK Sheet from Template", newSheetHome.Id.Value).Build();
            Assert.IsNotNull(smartsheet);
            Sheet newSheetFromTemplate = smartsheet.FolderResources.SheetResources.CreateSheetFromTemplate(folder.Id.Value, sheet);

            if(newSheetFromTemplate?.Id?.ToString().Length == 0 || newSheetFromTemplate?.AccessLevel != AccessLevel.OWNER ||
                newSheetFromTemplate.Permalink.Length == 0)
            {
                Assert.Fail("SheetResourcesTest.TestCreateSheetInFolderFromTemplate Failed to create sheet");
            }
        }

        private void TestCreateSheetInWorkspace()
        {
            workspace = CreateWorkspace("CSharp Test Workspace");
            Assert.IsNotNull(workspace.Id);
            Assert.IsNotNull(smartsheet);
            Assert.IsNotNull(sheetHome);
            Sheet newSheet = smartsheet.WorkspaceResources.SheetResources.CreateSheet(workspace.Id.Value, sheetHome);
            Assert.AreEqual(sheetHome.Columns.Count, newSheet.Columns.Count);
        }

        private void TestCreateSheetInWorkspaceFromTemplate()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(workspace?.Id);
            Sheet sheet = new Sheet.CreateSheetFromTemplateBuilder("CSharp SDK Sheet in Workspace from Template", newSheetHome.Id.Value).Build();
            Assert.IsNotNull(smartsheet);
            Sheet newSheetFromTemplate = smartsheet.WorkspaceResources.SheetResources.CreateSheetFromTemplate(workspace.Id.Value, sheet,
                new List<TemplateInclusion> { TemplateInclusion.ATTACHMENTS, TemplateInclusion.DATA, TemplateInclusion.DISCUSSIONS, TemplateInclusion.RULE_RECIPIENTS, TemplateInclusion.RULES });
            if (newSheetFromTemplate?.Id?.ToString().Length == 0 || newSheetFromTemplate?.AccessLevel != AccessLevel.OWNER ||
                newSheetFromTemplate.Permalink.Length == 0)
            {
                Assert.Fail("SheetResourcesTest.TestCreateSheetInWorkspaceFromTemplate Failed to create sheet");
            }
        }

        private void TestGetSheet()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            Sheet sheet = smartsheet.SheetResources.GetSheet(newSheetHome.Id.Value);
            Assert.AreEqual(sheet.Permalink, newSheetHome.Permalink);

            // Test rowsModifiedSince

            sheet = smartsheet.SheetResources.GetSheet(newSheetHome.Id.Value, rowsModifiedSince: DateTime.UnixEpoch);
            Assert.AreEqual(sheet.Permalink, newSheetHome.Permalink);
        }

        private void TestGetSheetVersion()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);            
            int? version = smartsheet.SheetResources.GetSheetVersion(newSheetHome.Id.Value);
            if (version == null || version.Value != 1)
                Assert.Fail("SheetResourcesTest.TestGetSheetVersion GetSheetVersion incorrect");
        }

        private void TestGetSheetAsExcel()
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            smartsheet.SheetResources.GetSheetAsExcel(newSheetHome.Id.Value, writer);
        }

        private void TestGetSheetAsPDF()
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            smartsheet.SheetResources.GetSheetAsPDF(newSheetHome.Id.Value, writer, PaperSize.A3);

            //Test with no size given
            smartsheet.SheetResources.GetSheetAsPDF(newSheetHome.Id.Value, writer);
        }

        private void TestGetPublishStatus()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            SheetPublish sheetPublish = smartsheet.SheetResources.GetPublishStatus(newSheetHome.Id.Value);
            Assert.IsNotNull(sheetPublish);
        }

        private void TestUpdateSheet()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Sheet sheet = new Sheet.UpdateSheetBuilder(newSheetHome.Id).SetName("CSharp SDK Updated Sheet").Build();
            Assert.IsNotNull(smartsheet);
            Sheet newSheet = smartsheet.SheetResources.UpdateSheet(sheet);
            Assert.AreEqual(sheet.Name, newSheet.Name);
        }

        private void TestPublishSheetDefaults()
        {
            SheetPublish sheetPublish = new SheetPublish.PublishStatusBuilder(true, true, true, false).Build();
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            SheetPublish newSheetPublish = smartsheet.SheetResources.UpdatePublishStatus(newSheetHome.Id.Value, sheetPublish);
            Assert.IsTrue(newSheetPublish.ReadWriteShowToolbar.GetValueOrDefault(false));
        }

        private void TestPublishSheet()
        {
            SheetPublish sheetPublish = new SheetPublish();
            sheetPublish.IcalEnabled = false;
            sheetPublish.ReadOnlyFullEnabled = true;
            sheetPublish.ReadWriteEnabled = true;
            sheetPublish.ReadOnlyLiteEnabled = true;
            sheetPublish.ReadWriteShowToolbar = false;
            sheetPublish.ReadOnlyFullShowToolbar = false;
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            SheetPublish newSheetPublish = smartsheet.SheetResources.UpdatePublishStatus(newSheetHome.Id.Value, sheetPublish);

            Assert.IsNotNull(newSheetPublish.ReadWriteShowToolbar);
            Assert.IsFalse(newSheetPublish.ReadWriteShowToolbar.Value);
            Assert.IsNotNull(newSheetPublish.ReadOnlyFullShowToolbar);
            Assert.IsFalse(newSheetPublish.ReadOnlyFullShowToolbar.Value);
        }

        private void TestListSheets()
        {
            PaginationParameters pagination = new PaginationParameters(false, 1, 1);
            Assert.IsNotNull(smartsheet);
            PaginatedResult<Sheet> sheets = smartsheet.SheetResources.ListSheets(new List<SheetInclusion> { SheetInclusion.SOURCE }, pagination);
            Assert.IsTrue(sheets.PageNumber == 1);
        }

        private void TestSendSheet()
        {
            List<Recipient> recipients = new List<Recipient>();
            Recipient recipient = new Recipient();
            recipient.Email = "test.user@smartsheet.com";
            recipients.Add(recipient);

            FormatDetails formatDetails = new FormatDetails();
            formatDetails.PaperSize = PaperSize.A4;

            SheetEmail email = new SheetEmail.CreateSheetEmail(recipients, SheetEmailFormat.PDF).SetFormatDetails(formatDetails).Build();
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(smartsheet);
            smartsheet.SheetResources.SendSheet(newSheetHome.Id.Value, email);
        }

        private void TestCreateUpdateRequest()
        {
            Assert.IsNotNull(smartsheet);
            Sheet sheet = smartsheet.SheetResources.CreateSheet(CreateSheetObject());

            PaginationParameters pagination = new PaginationParameters(true, null, null);
            Assert.IsNotNull(sheet.Id);
            PaginatedResult<Column> columns = smartsheet.SheetResources.ColumnResources.ListColumns(sheet.Id.Value, new List<ColumnInclusion> { ColumnInclusion.FILTERS }, pagination);

            Column addedColumn1 = columns.Data[0];
            Column addedColumn2 = columns.Data[1];
            Assert.IsNotNull(addedColumn1.Id);
            Assert.IsNotNull(addedColumn2.Id);

            List<Cell> cellsA = new List<Cell> { new Cell.AddCellBuilder(addedColumn1.Id.Value, true).Build(),
                new Cell.AddCellBuilder(addedColumn2.Id.Value, "new status").Build() };

            Row rowA = new Row.AddRowBuilder(null, true, null, null, null).SetCells(cellsA).Build();

            List<Cell> cellsB = new List<Cell> { new Cell.AddCellBuilder(addedColumn1.Id.Value, true).Build(),
                new Cell.AddCellBuilder(addedColumn2.Id.Value, "new status").Build() };

            Row rowB = new Row.AddRowBuilder(null, true, null, null, null).SetCells(cellsB).Build();

            IList<Row> newRows = smartsheet.SheetResources.RowResources.AddRows(sheet.Id.Value, new Row[] { rowA, rowB });

            List<Recipient> recipients = new List<Recipient>();
            Recipient recipient = new Recipient();
            recipient.Email = "test.user@smartsheet.com";
            recipients.Add(recipient);

            UpdateRequest updateRequest = new UpdateRequest();
            updateRequest.SendTo = recipients;
            updateRequest.Subject = "some subject";
            updateRequest.Message = "some message";
            updateRequest.CcMe = false;
            var rowId = newRows[0].Id.GetValueOrDefault();
            Assert.IsNotNull(rowId);
            updateRequest.RowIds = new List<long> { rowId };
            updateRequest.ColumnIds = new List<long> { addedColumn2.Id.Value };
            updateRequest.IncludeAttachments = false;
            updateRequest.IncludeDiscussions = false;

            smartsheet.SheetResources.UpdateRequestResources.CreateUpdateRequest(sheet.Id.Value, updateRequest);

            DeleteSheet(sheet.Id.Value);
        }

        private void TestSortSheet()
        {
            Sheet sheet = CreateSheet();
            Assert.IsNotNull(sheet?.Id);

            SortSpecifier specifier = new SortSpecifier();
            SortCriterion criterion = new SortCriterion();
            var columnId = sheet?.Columns[1]?.Id.GetValueOrDefault();
            Assert.IsNotNull(columnId);
            Assert.IsNotNull(sheet?.Id);
            var sheetId = sheet.Id.Value;
            criterion.ColumnId = columnId.Value;
            criterion.Direction = SortDirection.DESCENDING;
            specifier.SortCriteria = new SortCriterion[] { criterion };
            Assert.IsNotNull(smartsheet);
            sheet = smartsheet.SheetResources.SortSheet(sheetId, specifier);
            Assert.AreEqual(sheet.Rows[0].Cells[1].DisplayValue, "C");

            DeleteSheet(sheetId);
        }

        private void TestDeleteSheet()
        {
            Assert.IsNotNull(newSheetHome?.Id);
            Assert.IsNotNull(newSheetTemplate?.Id);
            Assert.IsNotNull(workspace?.Id);
            Assert.IsNotNull(folder?.Id);
            Assert.IsNotNull(smartsheet);
            smartsheet.SheetResources.DeleteSheet(newSheetHome.Id.Value);
            smartsheet.SheetResources.DeleteSheet(newSheetTemplate.Id.Value);
            DeleteWorkspace(workspace.Id.Value);
            DeleteFolder(folder.Id.Value);
        }
    }
}