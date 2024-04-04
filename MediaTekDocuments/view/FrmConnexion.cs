using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.controller;

namespace MediaTekDocuments.view
{
    public partial class FrmConnexion : Form
    {

        private FrmConnexionController controller;

        public FrmConnexion()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Initialisations :
        /// Création du controleur
        /// </summary>
        private void Init()
        {
            txbLogin.Text = "";
            txbPwd.Text = "";
            controller = new FrmConnexionController();
            this.AcceptButton = btnConnexion;
        }

        /// <summary>
        /// Connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnexion_Click(object sender, EventArgs e)
        {
            if (controller.GetLogin(txbLogin.Text, txbPwd.Text))
                this.Visible = false;
            else
            {
                MessageBox.Show("Identifiant ou mot de passe erroné");
                txbLogin.Text = "";
                txbPwd.Text = "";
            }
        }
    }
}
