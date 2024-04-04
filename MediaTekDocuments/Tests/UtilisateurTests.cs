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
    public class UtilisateurTests
    {
        private const string id = "09605";
        private const string nom = "zaezae";
        private const string prenom = "zaeaze";
        private const string mail = "azeaze";
        private const string idService = "54";
        private const string service = "azer";
        private static readonly Utilisateur utilisateur = new Utilisateur(id, nom, prenom, mail, idService, service);

        [TestMethod()]
        public void UtilisateurTest()
        {
            Assert.AreEqual(id, utilisateur.Id, "OK");
            Assert.AreEqual(nom, utilisateur.Nom, "OK");
            Assert.AreEqual(prenom, utilisateur.Prenom, "OK");
            Assert.AreEqual(mail, utilisateur.Mail, "OK");
            Assert.AreEqual(idService, utilisateur.Service.Id, "OK");
            Assert.AreEqual(service, utilisateur.Service.Libelle, "OK");

        }
    }
}
