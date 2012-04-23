using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using IBM.ClearQuest.Interop;
using VersionOne.ServiceHost.ClearQuestServices;
using System.Runtime.InteropServices;
using VersionOne.ServiceHost.ClearQuestServices.Exceptions;
using VersionOne.ServiceHost.Core.Configuration;
using VersionOne.ServiceHost.WorkitemServices;

namespace VersionOne.ServiceHost.Tests.WorkitemServices.ClearQuest {
    [TestFixture]
    public class ClearQuestQueryTester {
        private readonly MockRepository mockRepository = new MockRepository();

        private IOAdSession sessionMock;
        private IOAdEntity defectMock;
        private IOAdResultSet resultSetMock;
        private IOAdQueryDef queryMock;
        private IOAdQueryFilterNode filterNodeMock;
        private IClearQuestSessionBuilder sessionBuilderMock;

        private ClearQuestQuery clearQuestQuery;

        private const int FieldCount = 3;
        private const string FieldPrefix = "field";

        private const string DefectId = "SAMPL000001";
        private const int DefectCount = 2;
        private const string DefectSource = "ClearQuest";

        private const string ClearQuestPriorityName = "CQ Priority 2";
        private const string VersionOnePriorityId = "WorkitemPriority:2";
        private const string ClearQuestProjectName = "CQ Project 2";
        private const string VersionOneProjectId = "Scope:2";

        [SetUp]
        public void SetUp() 
        {
            sessionMock = mockRepository.StrictMock<IOAdSession>();
            queryMock = mockRepository.DynamicMock<IOAdQueryDef>();
            resultSetMock = mockRepository.StrictMock<IOAdResultSet>();
            defectMock = mockRepository.StrictMock<IOAdEntity>();
            filterNodeMock = mockRepository.StrictMock<IOAdQueryFilterNode>();
            sessionBuilderMock = mockRepository.StrictMock<IClearQuestSessionBuilder>();

            SetupResult.For(sessionBuilderMock.Create(null)).IgnoreArguments().Return(sessionMock);

            clearQuestQuery = new ClearQuestQuery(CreateConfig(), sessionBuilderMock);
        }

        [Test]
        public void ModifyEntityTest() 
        {
            Dictionary<string, object> fields = CreateFieldSetForModification();

            using (mockRepository.Ordered()) 
            {
                Expect.Call(sessionMock.GetEntity(clearQuestQuery.ClearQuestEntityType, DefectId)).Return(defectMock);
                sessionMock.EditEntity(defectMock, clearQuestQuery.ClearQuestModifyAction);
                
                foreach (KeyValuePair<string, object> field in fields) 
                {
                    Expect.Call(defectMock.SetFieldValue(field.Key, field.Value)).Return(null);
                }
                
                Expect.Call(defectMock.Validate()).Return(null);
                Expect.Call(defectMock.Commit()).Return(null);
            }

            mockRepository.ReplayAll();

			clearQuestQuery.ModifyEntity(DefectId, fields, clearQuestQuery.ClearQuestModifyAction);

            mockRepository.VerifyAll();
        }

		[Test]
		public void ModifyEntityTestWithExeption()
		{
			Dictionary<string, object> fields = new Dictionary<string, object>();
			int codeError = 14045;
			string key = "test key";
			object value = "test value";
			fields.Add(key, value);

			using (mockRepository.Ordered())
			{
				Expect.Call(sessionMock.GetEntity(clearQuestQuery.ClearQuestEntityType, DefectId)).Return(defectMock);

				sessionMock.EditEntity(defectMock, clearQuestQuery.ClearQuestModifyAction);

				Expect.Call(defectMock.SetFieldValue(key, value)).Throw(new COMException(null, codeError));
				Expect.Call(defectMock.IsEditable()).Return(true);

				defectMock.Revert();
			}

			mockRepository.ReplayAll();

			try
			{
				clearQuestQuery.ModifyEntity(DefectId, fields, clearQuestQuery.ClearQuestModifyAction);
				Assert.Fail("The method has to show ClearQuestUpdateException exception");
			}
			catch (ClearQuestUpdateException ex)
			{
				Assert.AreEqual(codeError, ex.CodeError);
			}

			mockRepository.VerifyAll();
		}

        [Test]
        public void SelectDefectsTest() 
        {
            using (mockRepository.Ordered()) 
            {
                ExpectCreateQuery(clearQuestQuery.FieldList);
                ExpectAddQueryFilter(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

                Expect.Call(sessionMock.BuildResultSet(queryMock)).Return(resultSetMock);
                Expect.Call(resultSetMock.ExecuteAndCountRecords()).Return(DefectCount);

                for (int i = 0; i < DefectCount; i++) 
				{
                    Expect.Call(resultSetMock.MoveNext()).Return(1);
                    ExpectExtractRow();
                }

                Expect.Call(resultSetMock.MoveNext()).Return(0);
            }

            mockRepository.ReplayAll();

            IList<Defect> defects = clearQuestQuery.SelectDefects(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

            mockRepository.VerifyAll();
            
            Assert.AreEqual(defects.Count, DefectCount);
            Assert.AreEqual(defects[0].ExternalSystemName, DefectSource);
            Assert.IsNotNull(defects[0].ExternalUrl);
            Assert.IsNull(defects[0].Priority);
        }

        [Test]
        public void SelectDefectsTestWithMappedPriority() 
        {
            string[] values = { "string1", "string2", "string3", "string4", "string5", ClearQuestPriorityName };
            using (mockRepository.Ordered()) 
            {
                ExpectCreateQuery(clearQuestQuery.FieldList);
                ExpectAddQueryFilter(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

                Expect.Call(sessionMock.BuildResultSet(queryMock)).Return(resultSetMock);
                Expect.Call(resultSetMock.ExecuteAndCountRecords()).Return(DefectCount);

                for (int i = 0; i < DefectCount; i++) {
                    Expect.Call(resultSetMock.MoveNext()).Return(1);
                    ExpectExtractRow(values);
                }

                Expect.Call(resultSetMock.MoveNext()).Return(0);
            }

            mockRepository.ReplayAll();

            IList<Defect> defects = clearQuestQuery.SelectDefects(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

            mockRepository.VerifyAll();

            Assert.AreEqual(DefectCount, defects.Count);
            Assert.AreEqual(DefectSource, defects[0].ExternalSystemName);
            Assert.IsNotNull(defects[0].ExternalUrl);
            Assert.IsNotNull(defects[0].Priority);
            Assert.AreEqual(VersionOnePriorityId, defects[0].Priority);
        }

        [Test]
        public void SelectDefectsTestWithProjectPriority() {
            string[] values = { "string1", "string2", ClearQuestProjectName, "string4", "string5", "string6" };
            using (mockRepository.Ordered()) {
                ExpectCreateQuery(clearQuestQuery.FieldList);
                ExpectAddQueryFilter(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

                Expect.Call(sessionMock.BuildResultSet(queryMock)).Return(resultSetMock);
                Expect.Call(resultSetMock.ExecuteAndCountRecords()).Return(DefectCount);

                for (int i = 0; i < DefectCount; i++) {
                    Expect.Call(resultSetMock.MoveNext()).Return(1);
                    ExpectExtractRow(values);
                }

                Expect.Call(resultSetMock.MoveNext()).Return(0);
            }

            mockRepository.ReplayAll();

            IList<Defect> defects = clearQuestQuery.SelectDefects(clearQuestQuery.FieldList[0], ComparisonOperator.Equal, string.Empty);

            mockRepository.VerifyAll();

            Assert.AreEqual(DefectCount, defects.Count);
            Assert.AreEqual(DefectSource, defects[0].ExternalSystemName);
            Assert.IsNotNull(defects[0].ProjectId);
            Assert.IsNull(defects[0].Priority);
            Assert.AreEqual(VersionOneProjectId, defects[0].ProjectId);
        }

        #region Expectations for utility methods

        private void ExpectCreateQuery(params string[] fields) 
        {
            Expect.Call(sessionMock.BuildQuery(clearQuestQuery.ClearQuestEntityType)).Return(queryMock);
            foreach (string field in fields) 
            {
                queryMock.BuildField(field);
            }
        }

        private void ExpectAddQueryFilter(string fieldName, ComparisonOperator op, string filterValue) 
        {
            Expect.Call(queryMock.BuildFilterOperator((int) op)).Return(filterNodeMock);
            filterNodeMock.BuildFilter(fieldName, (int) op, filterValue);
        }

        private void ExpectExtractRow() 
        {
            Expect.Call(resultSetMock.GetNumberOfColumns()).Return(clearQuestQuery.FieldList.Length);
            
            for (int i = 1; i <= clearQuestQuery.FieldList.Length; i++) 
            {
                Expect.Call(resultSetMock.GetColumnValue(i)).Return(RandomString);
                Expect.Call(resultSetMock.GetColumnLabel(i)).Return(clearQuestQuery.FieldList[i - 1]);
            }
        }

        private void ExpectExtractRow(string[] values) 
        {
            Expect.Call(resultSetMock.GetNumberOfColumns()).Return(clearQuestQuery.FieldList.Length);

            for (int i = 1; i <= clearQuestQuery.FieldList.Length; i++) {
                Expect.Call(resultSetMock.GetColumnValue(i)).Return(values[i - 1]);
                Expect.Call(resultSetMock.GetColumnLabel(i)).Return(clearQuestQuery.FieldList[i - 1]);
            }
        }

        #endregion

        #region Helper methods

        private static Dictionary<string, object> CreateFieldSetForModification() 
        {
            Dictionary<string, object> fieldSet = new Dictionary<string, object>();
            
            for(int i = 1; i <= FieldCount; i++) 
            {
                fieldSet.Add(FieldPrefix + i, RandomString);
            }
            
            return fieldSet;
        }

        private static string RandomString 
        {
            get 
            {
                return Guid.NewGuid().ToString();
            }
        }

        private static ClearQuestServiceConfiguration CreateConfig() 
        {
            ClearQuestServiceConfiguration config = new ClearQuestServiceConfiguration();
            config.SourceFieldValue = DefectSource;
            config.WebUrlTemplate = "http://example.com/defect/#defectid#";
            config.WebUrlTitle = "Link to defect";

			config.ClearQuestEntityType   = "defect";
			config.IdField                = "id";
			config.DefectTitleField       = "Headline";
			config.DescriptionField       = "Description";
			config.ProjectNameField       = "Project.Name";
			config.OwnerLoginField        = "Owner.login_name";
			config.StateField             = "State";
            config.ClearQuestPriorityField= "Priority";
			config.ClearQuestModifyAction = "modify";

            //Priority mapping data
            MappingInfo cqPriority1 = new MappingInfo(null, "CQ Priority 1");
            MappingInfo cqPriority2 = new MappingInfo(null, ClearQuestPriorityName);
            MappingInfo v1Priority1 = new MappingInfo("WorkitemPriority:1", "V1 Priority 1");
            MappingInfo v1Priority2 = new MappingInfo(VersionOnePriorityId, "V1 Priority 2");
            config.PriorityMappings.Add(cqPriority1, v1Priority1);
            config.PriorityMappings.Add(cqPriority2, v1Priority2);

            //Project mapping data
            MappingInfo cqProject1 = new MappingInfo(null, "CQ Project 1");
            MappingInfo cqProject2 = new MappingInfo(null, ClearQuestProjectName);
            MappingInfo v1Project1 = new MappingInfo("Scope:1", "V1 Project 1");
            MappingInfo v1Project2 = new MappingInfo(VersionOneProjectId, "V1 Project 2");
            config.ProjectMappings.Add(cqProject1, v1Project1);
            config.ProjectMappings.Add(cqProject2, v1Project2);

            return config;
        }

        #endregion
    }
}
