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
    public class ExemplaireTests
    {
        private const int numero = 2005;
        private const string photo = "";
        private static readonly DateTime dateAchat = DateTime.Now.AddYears(-2).AddMonths(-5);
        private const string idEtat = "00000";
        private const string id = "00000";
        private static Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, id);

        [TestMethod()]
        public void ExemplaireTest()
        {
            Assert.AreEqual(numero, exemplaire.Numero, "OK");
            Assert.AreEqual(photo, exemplaire.Photo, "OK");
            Assert.AreEqual(dateAchat, exemplaire.DateAchat, "OK");
            Assert.AreEqual(idEtat, exemplaire.IdEtat, "OK");
            Assert.AreEqual(id, exemplaire.Id, "OK");
        }
    }
}
