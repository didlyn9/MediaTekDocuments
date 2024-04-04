using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.Tests

{   [TestClass()]
    public class CommandeDocumentTests
    {
        private const string id = "0000";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const float montant = 3.141592F;
        private const int nbExemplaire = 2;
        private const string idLivreDvd = "0000";
        private const int idSuivi = 4;
        private const string etat = "Neuf";
        private static readonly CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire
            , idLivreDvd, idSuivi, etat);

        [TestMethod()]
        public void CommandeDocumentTest()
        {
            Assert.AreEqual(id, commandeDocument.Id, "OK");
            Assert.AreEqual(dateCommande, commandeDocument.DateCommande, "OK");
            Assert.AreEqual(montant, commandeDocument.Montant, "OK");
            Assert.AreEqual(nbExemplaire, commandeDocument.NbExemplaire, "OK");
            Assert.AreEqual(idLivreDvd, commandeDocument.IdLivreDvd, "OK");
            Assert.AreEqual(idSuivi, commandeDocument.IdSuivi, "OK");
            Assert.AreEqual(etat, commandeDocument.Etat, "OK");
        }

    }
}
