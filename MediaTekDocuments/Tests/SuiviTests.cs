using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass()]
    public class SuiviTests
    {
        private const int id = 2;
        private const string etat = "en cours";
        private static readonly Suivi suivi = new Suivi(id, etat);

        [TestMethod()]
        public void SuiviTest()
        {
            Assert.AreEqual(id, suivi.Id, "OK");
            Assert.AreEqual(etat, suivi.Etat, "OK");
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(etat, etat.ToString(), "OK");
        }
    }
}
