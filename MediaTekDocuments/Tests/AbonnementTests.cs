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
    public class AbonnementTests
    {
        private const string id = "0000";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const double montant = 25.32;
        private static readonly DateTime dateFinAbonnement = DateTime.Now.AddMonths(4);
        private const string idRevue = "0000";
        private static readonly Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);


        [TestMethod()]
        public void AbonnementTest()
        {
            Assert.AreEqual(id, abonnement.Id, "OK");
            Assert.AreEqual(dateCommande, abonnement.DateCommande, "OK");
            Assert.AreEqual(montant, abonnement.Montant, "OK");
            Assert.AreEqual(dateFinAbonnement, abonnement.DateFinAbonnement, "OK");
            Assert.AreEqual(idRevue, abonnement.IdRevue, "OK");
        }

    }
}
