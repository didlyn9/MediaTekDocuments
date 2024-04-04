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
    public class CommandeTests
    {
        private const string id = "0005";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const double montant = 10.10;
        private static readonly Commande commande = new Commande(id, dateCommande, montant);


        [TestMethod()]
        public void CommandeTest()
        {
            Assert.AreEqual(id, commande.Id, "OK");
            Assert.AreEqual(dateCommande, commande.DateCommande, "OK");
            Assert.AreEqual(montant, commande.Montant, "OK");
        }
    }
}
