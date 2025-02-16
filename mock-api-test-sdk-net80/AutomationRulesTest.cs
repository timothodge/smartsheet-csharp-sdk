﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smartsheet.Api;
using Smartsheet.Api.Models;

namespace mock_api_test_sdk_net80
{
    [TestClass]
    public class AutomationRuleTests
    {
        [TestMethod]
        public void ListAutomationRules()
        {
            SmartsheetClient ss = HelperFunctions.SetupClient("List Automation Rules");
            PaginatedResult<AutomationRule> automationRules = ss.SheetResources.AutomationRuleResources.ListAutomationRules(324);

            Assert.IsNotNull(automationRules.TotalCount);
            Assert.AreEqual(2, (long)automationRules.TotalCount);
        }

        [TestMethod]
        public void GetAutomationRule()
        {
            SmartsheetClient ss = HelperFunctions.SetupClient("Get Automation Rule");
            AutomationRule automationRule = ss.SheetResources.AutomationRuleResources.GetAutomationRule(324, 284);
            Assert.IsNotNull(automationRule.Id);
            Assert.AreEqual(284, (long)automationRule.Id);
        }

        [TestMethod]
        public void UpdateAutomationRule()
        {
            SmartsheetClient ss = HelperFunctions.SetupClient("Update Automation Rule");
            AutomationAction autoRuleAction = new AutomationAction();
            Recipient recipient = new Recipient();
            recipient.Email = "jane@example.com";
            autoRuleAction.Recipients = new Recipient[] { recipient };
            autoRuleAction.Type = AutomationActionType.NOTIFICATION_ACTION;
            autoRuleAction.Frequency = AutomationActionFrequency.WEEKLY;
            AutomationRule autoRule = new AutomationRule();
            autoRule.Id = 284;
            autoRule.Action = autoRuleAction;
            AutomationRule automationRule = ss.SheetResources.AutomationRuleResources.UpdateAutomationRule(324, autoRule);
        }

        [TestMethod]
        public void DeleteAutomationRule()
        {
            SmartsheetClient ss = HelperFunctions.SetupClient("Delete Automation Rule");
            ss.SheetResources.AutomationRuleResources.DeleteAutomationRule(324, 284);
        }
    }
}
