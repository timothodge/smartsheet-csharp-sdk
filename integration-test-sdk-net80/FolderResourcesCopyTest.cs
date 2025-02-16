﻿using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace integration_test_sdk_net80
{
    [TestClass]
    public class FolderResourcesCopyTest
    {
        [TestMethod]
        public void TestFolderCopyResources()
        {
            SmartsheetClient smartsheet = new SmartsheetBuilder().SetMaxRetryTimeout(30000).Build();
            // Before
            // Folder1-----SubFolder1
            // Folder2

            // After
            // Folder1-----SubFolder1
            // Folder2-----SubFolder1Copy
            long createdFolderInHomeId1 = CreateFolderInHome(smartsheet, "Folder1");
            long createdFolderInHomeId2 = CreateFolderInHome(smartsheet, "Folder2");

            long createdFolderInFolderId = CreateFolderInFolder(smartsheet, createdFolderInHomeId1, "SubFolder1");

            ContainerDestination destination = new ContainerDestination
            {
                DestinationId = createdFolderInHomeId2,
                DestinationType = DestinationType.FOLDER,
                NewName = "SubFolder1Copy"
            };
            Folder newCopiedFolder = smartsheet.FolderResources.CopyFolder(createdFolderInFolderId, destination, new FolderCopyInclusion[] { FolderCopyInclusion.ALL }, new FolderRemapExclusion[] { FolderRemapExclusion.CELL_LINKS });

            Assert.IsTrue(newCopiedFolder.Name == "SubFolder1Copy");
            Assert.IsNotNull(newCopiedFolder.Id);
            long copiedFolderId = newCopiedFolder.Id.Value;

            Folder copiedFolder = smartsheet.FolderResources.GetFolder(copiedFolderId);
            Assert.IsTrue(copiedFolder.Name == "SubFolder1Copy");

            DeleteFolders(smartsheet, createdFolderInHomeId1, createdFolderInHomeId2);
        }


        private static void DeleteFolders(SmartsheetClient smartsheet, long folder1, long folder2)
        {
            smartsheet.FolderResources.DeleteFolder(folder2);
            try
            {
                smartsheet.FolderResources.GetFolder(folder2);
                Assert.Fail("Exception should have been thrown. Cannot get a deleted folder.");
            }
            catch
            {
                // Should be "Not Found".
            }
            smartsheet.FolderResources.DeleteFolder(folder1);
            try
            {
                smartsheet.FolderResources.GetFolder(folder1);
                Assert.Fail("Exception should have been thrown. Cannot get a deleted folder.");
            }
            catch
            {
                // Should be "Not Found".
            }
        }

        private static long CreateFolderInFolder(SmartsheetClient smartsheet, long createdFolderInHomeId, string folderName)
        {
            Folder createdFolderInFolder = smartsheet.FolderResources.CreateFolder(createdFolderInHomeId, new Folder.CreateFolderBuilder(folderName).Build());
            Assert.IsNotNull(createdFolderInFolder.Id);
            return createdFolderInFolder.Id.Value;
        }

        private static long CreateFolderInHome(SmartsheetClient smartsheet, string folderName)
        {
            Folder createdFolderInHome = smartsheet.HomeResources.FolderResources.CreateFolder(new Folder.CreateFolderBuilder(folderName).Build());
            Assert.IsNotNull(createdFolderInHome.Id);
            return createdFolderInHome.Id.Value;
        }
    }
}