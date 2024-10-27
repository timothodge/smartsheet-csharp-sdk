using RestSharp;
using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class CrossSheetReferencesTest : TestResources
    {
        private Sheet? sheetA;
        private Sheet? sheetB;
        private CrossSheetReference? xref;

        [TestInitialize]
        public void Setup()
        {
            smartsheet = CreateClient();

            sheetA = smartsheet.SheetResources.CreateSheet(CreateSheetObject());
            sheetB = CreateSheet();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Assert.IsNotNull(sheetA?.Id);
            Assert.IsNotNull(sheetB?.Id);
            DeleteSheet(sheetA.Id.Value);
            DeleteSheet(sheetB.Id.Value);
        }

        [TestMethod]
        public void TestCrossSheetReferences()
        {
            TestCreateCrossSheetReference();
            TestListCrossSheetReferences();
            TestGetCrossSheetReference();
        }

        private void TestCreateCrossSheetReference()
        {
            Assert.IsNotNull(sheetA?.Id);
            Assert.IsNotNull(sheetB?.Id);
            var sheetBColumns = sheetB.Columns[0].Id;
            Assert.IsNotNull(sheetBColumns);
            var sheetBRows = sheetB.Rows[0].Id;
            Assert.IsNotNull(sheetBRows);

            xref = new CrossSheetReference();
            xref.SourceSheetId = sheetB.Id.Value;
            xref.StartColumnId = sheetBColumns.Value;
            xref.EndColumnId = sheetBColumns.Value;
            xref.StartRowId = sheetBRows.Value;
            xref.EndRowId = sheetBRows.Value;
            Assert.IsNotNull(smartsheet);
            xref = smartsheet.SheetResources.CrossSheetReferenceResources.CreateCrossSheetReference(sheetA.Id.Value, xref);
        }

        private void TestListCrossSheetReferences()
        {
            PaginationParameters pagination = new PaginationParameters(true, null, null);
            Assert.IsNotNull(sheetA?.Id);
            Assert.IsNotNull(smartsheet);
            PaginatedResult<CrossSheetReference> xrefs = smartsheet.SheetResources.CrossSheetReferenceResources.ListCrossSheetReferences(sheetA.Id.Value, pagination);
            var xrefsDataId = xrefs.Data[0].Id;
            Assert.IsNotNull(xrefsDataId);
            Assert.IsNotNull(xref?.Id);
            Assert.AreEqual(xrefsDataId.Value, xref.Id.Value);

            xrefs = smartsheet.SheetResources.CrossSheetReferenceResources.ListCrossSheetReferences(sheetA.Id.Value);
            Assert.AreEqual(xrefsDataId.Value, xref.Id.Value);
        }

        private void TestGetCrossSheetReference()
        {
            Assert.IsNotNull(sheetA?.Id);
            Assert.IsNotNull(smartsheet);
            Sheet sheet = smartsheet.SheetResources.GetSheet(sheetA.Id.Value, new List<SheetLevelInclusion> { SheetLevelInclusion.CROSS_SHEET_REFERENCES });
            Assert.IsTrue(sheet.CrossSheetReferences.Count == 1);

            var sheetCrossReferenceId = sheet.CrossSheetReferences[0].Id;
            Assert.IsNotNull(sheetCrossReferenceId);
            CrossSheetReference _xref = smartsheet.SheetResources.CrossSheetReferenceResources.GetCrossSheetReference(sheetA.Id.Value, sheetCrossReferenceId.Value);
        }
    }
}