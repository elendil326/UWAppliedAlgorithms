using Similarity;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimilarityTests
{
    [TestClass]
    public class GroupFactoryTest
    {
        string _data50Csv = @".\Data\data50.csv";
        string _labelCsv = @".\Data\label.csv";
        string _groupsCsv = @".\Data\groups.csv";

        [TestMethod]
        public void GetGroupArticles_RealFiles_NotEmpty()
        {
            List<Group> groups = GroupFactory.GetGroupArticles(_data50Csv, _labelCsv, _groupsCsv);

            Assert.AreEqual(20, groups.Count);
            Assert.IsTrue(groups.All(g => g.Articles.Count == 50));
        }
    }
}
