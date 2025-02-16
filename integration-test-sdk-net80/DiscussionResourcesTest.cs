﻿using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class DiscussionResourcesTest
    {
        [TestMethod]
        public void TestDiscussionResources()
        {
            SmartsheetClient smartsheet = new SmartsheetBuilder().SetMaxRetryTimeout(30000).Build(); 
            
            long sheetId = CreateSheet(smartsheet);

            Discussion discussionToCreate = new Discussion.CreateDiscussionBuilder("A discussion", new Comment.AddCommentBuilder("a comment").Build()).Build();
            Discussion createdDiscussion = smartsheet.SheetResources.DiscussionResources.CreateDiscussion(sheetId, discussionToCreate);
            Assert.IsNotNull(createdDiscussion.Id);
            long createdDiscussionId = createdDiscussion.Id.Value;
            string path = "../../../../integration-test-sdk-net80/TestFile.txt";
            Discussion createdDiscussionWithFile = smartsheet.SheetResources.DiscussionResources.CreateDiscussionWithAttachment(sheetId, discussionToCreate, path, null);
            Assert.IsTrue(createdDiscussionWithFile.Comments[0].Attachments[0].Name == "TestFile.txt");


            PaginatedResult<Discussion> discussions = smartsheet.SheetResources.DiscussionResources.ListDiscussions(sheetId, new DiscussionInclusion[] { DiscussionInclusion.COMMENTS, DiscussionInclusion.ATTACHMENTS });
            Assert.IsTrue(discussions.TotalCount == 2);
            Assert.IsTrue(discussions.Data.Count == 2);
            var discussionIds = discussions.Data.Select(d => d.Id.Value).ToList();
            var discussionDataFirst = discussions.Data[0].Id;
            var discussionDataSecond = discussions.Data[1].Id;
            Assert.IsNotNull(discussionDataFirst);
            Assert.IsNotNull(discussionDataSecond);
            Assert.IsNotNull(createdDiscussionWithFile.Id);
            Assert.IsTrue(discussionDataFirst.Value == createdDiscussion.Id.Value || discussionDataFirst.Value == createdDiscussionWithFile.Id.Value);
            Assert.IsTrue(discussionDataSecond.Value == createdDiscussion.Id.Value || discussionDataSecond.Value == createdDiscussionWithFile.Id.Value);


            Discussion getDiscussionWithFile = smartsheet.SheetResources.DiscussionResources.GetDiscussion(sheetId, createdDiscussionWithFile.Id.Value);
            Assert.IsTrue(getDiscussionWithFile.Title == "A discussion");
            Assert.IsTrue(getDiscussionWithFile.Comments.Count == 1);
            Assert.IsTrue(getDiscussionWithFile.Comments[0].Attachments.Count == 1);
            Assert.IsTrue(getDiscussionWithFile.Comments[0].Attachments[0].Name == "TestFile.txt");

            Row row = new Row.AddRowBuilder(true, null, null, null, null).Build();
            IList<Row> rows = smartsheet.SheetResources.RowResources.AddRows(sheetId, new Row[] { row });
            Assert.IsTrue(rows.Count == 1);
            var rowsId = rows[0].Id;
            Assert.IsNotNull(rowsId);
            long rowId = rowsId.Value;
            Comment comment = new Comment.AddCommentBuilder("a comment!").Build();
            Discussion discussionToCreateOnRow = new Discussion.CreateDiscussionBuilder("discussion on row", comment).Build();
            Discussion discussionCreatedOnRow = smartsheet.SheetResources.RowResources.DiscussionResources.CreateDiscussionWithAttachment(sheetId, rowId, discussionToCreateOnRow, path, null);
            PaginatedResult<Discussion> discussionsOnRow = smartsheet.SheetResources.RowResources.DiscussionResources
            .ListDiscussions(sheetId, rowId, new DiscussionInclusion[] { DiscussionInclusion.COMMENTS });
            Assert.IsTrue(discussionsOnRow.Data.Count == 1);
            Assert.IsTrue(discussionsOnRow.Data[0].Title == "discussion on row");
            Assert.IsTrue(discussionsOnRow.Data[0].Comments[0].Text == "discussion on row\n\na comment!");

            smartsheet.SheetResources.DeleteSheet(sheetId);
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