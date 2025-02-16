﻿using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class SheetSummaryResourcesTest : TestResources
    {
        private Sheet? sheet;
        private IList<SummaryField>? asf;

        [TestInitialize]
        public void TestInitialize()
        {
            smartsheet = CreateClient();
            sheet = CreateSheet();
        }

        [TestCleanup]
        public void TestTeardown()
        {
            Assert.IsNotNull(sheet?.Id);
            DeleteSheet(sheet.Id.Value);
        }

        [Ignore]
        [TestMethod]
        public void TestSheetSummaryResources()
        {
            TestAddSheetSummaryFields();
            // FAIL: "no healthy upstream"
            TestGetSheetSummary();
            TestAddSheetSummaryFieldsWithPartialSuccess();
            TestGetSheetSummaryFields();
            TestUpdateSheetSummaryFields();
            TestUpdateSheetSummaryFieldsWithPartialSuccess();
            //TestAddSheetSummaryFieldImage();
            TestDeleteSheetSummaryFields();
        }

        private void TestAddSheetSummaryFields()
        {
            SummaryField sf = new SummaryField();
            sf.Title = "Hello World";
            sf.Type = ColumnType.CHECKBOX;
            sf.ObjectValue = new BooleanObjectValue(true);

            SummaryField sf1 = new SummaryField();
            sf1.Title = "Checkbox";
            sf1.Type = ColumnType.CHECKBOX;
            sf1.ObjectValue = new BooleanObjectValue(false);

            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            asf = smartsheet.SheetResources.SummaryResources.AddSheetSummaryFields(sheet.Id.Value,
                new List<SummaryField> { sf, sf1 }, true);

            Assert.AreEqual(asf.Count, 2);
        }

        private void TestGetSheetSummary()
        {
            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            SheetSummary sheetSummary = smartsheet.SheetResources.SummaryResources.GetSheetSummary(sheet.Id.Value,
                new List<SummaryFieldInclusion> { SummaryFieldInclusion.FORMAT, SummaryFieldInclusion.WRITER_INFO },
                new List<SummaryFieldExclusion> { SummaryFieldExclusion.DISPLAY_VALUE });
            Assert.AreEqual(sheetSummary.Fields.Count, 2);

            //Test without includes/excludes that are optional.
            sheetSummary = smartsheet.SheetResources.SummaryResources.GetSheetSummary(sheet.Id.Value);
            Assert.AreEqual(sheetSummary.Fields.Count, 2);
        }

        private void TestAddSheetSummaryFieldsWithPartialSuccess()
        {
            SummaryField sf = new SummaryField();
            sf.Title = "Hello World";
            sf.Type = ColumnType.TEXT_NUMBER;
            sf.ObjectValue = new StringObjectValue("Sally Smart");

            SummaryField sf1 = new SummaryField();
            sf1.Title = "Eeck!";
            sf1.Type = ColumnType.TEXT_NUMBER;
            sf1.ObjectValue = new StringObjectValue("Sammy Smart");

            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            BulkItemResult<SummaryField> asf = smartsheet.SheetResources.SummaryResources.AddSheetSummaryFieldsAllowPartialSuccess(
                sheet.Id.Value,  new List<SummaryField> { sf, sf1 });

            Assert.AreEqual(asf.FailedItems.Count, 1);
        }

        private void TestGetSheetSummaryFields()
        {
            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            PaginatedResult<SummaryField> fields = smartsheet.SheetResources.SummaryResources.GetSheetSummaryFields(sheet.Id.Value,
                new List<SummaryFieldInclusion> { SummaryFieldInclusion.WRITER_INFO },
                new List<SummaryFieldExclusion> { SummaryFieldExclusion.DISPLAY_VALUE });
            Assert.AreEqual(fields.Data.Count, 3);

            fields = smartsheet.SheetResources.SummaryResources.GetSheetSummaryFields(sheet.Id.Value);
            Assert.AreEqual(fields.Data.Count, 3);
        }

        private void TestUpdateSheetSummaryFields()
        {
            SummaryField sf = new SummaryField();
            Assert.IsNotNull(asf);
            sf.Id = asf[0].Id;
            sf.Title = "Hellooo World!";

            SummaryField sf1 = new SummaryField();
            sf1.Id = asf[1].Id;
            sf1.Title = "Eeek!";

            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            IList<SummaryField> usf = smartsheet.SheetResources.SummaryResources.UpdateSheetSummaryFields(sheet.Id.Value,
                new List<SummaryField> { sf, sf1 });
        }

        private void TestUpdateSheetSummaryFieldsWithPartialSuccess()
        {
            SummaryField sf = new SummaryField();
            Assert.IsNotNull(asf);
            sf.Id = asf[0].Id;
            sf.Title = "Hello World!";

            SummaryField sf1 = new SummaryField();
            sf1.Id = 123;
            sf1.Title = "Eeek!";

            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(smartsheet);
            BulkItemResult<SummaryField> usf = smartsheet.SheetResources.SummaryResources.UpdateSheetSummaryFieldsAllowPartialSuccess(sheet.Id.Value,
                new List<SummaryField> { sf, sf1 }, true);

            Assert.AreEqual(usf.FailedItems.Count, 1);
        }

        private void TestAddSheetSummaryFieldImage()
        {
            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(asf);
            var addSheetSummaryField = asf[0].Id;
            Assert.IsNotNull(addSheetSummaryField);
            Assert.IsNotNull(smartsheet);
            SummaryField summaryField = smartsheet.SheetResources.SummaryResources.AddSheetSummaryFieldImage(sheet.Id.Value,
                addSheetSummaryField.Value, "D:\\Smartsheet\\vHepGiJaeL6GPOX3wAx8yaxD75ym5eAbk0GB-MSz0gc.png", null, "alt text");
            Assert.IsInstanceOfType(summaryField.Image, typeof(Image));
        }

        private void TestDeleteSheetSummaryFields()
        {
            Assert.IsNotNull(sheet?.Id);
            Assert.IsNotNull(asf);
            var addSheetSummaryField = asf[0].Id;
            Assert.IsNotNull(addSheetSummaryField);
            Assert.IsNotNull(smartsheet);
            IList<long> delIds = smartsheet.SheetResources.SummaryResources.DeleteSheetSummaryFields(sheet.Id.Value,
                new List<long> { addSheetSummaryField.Value, 123L }, true);
            Assert.AreEqual(delIds.Count, 1);
        }
    }
}
