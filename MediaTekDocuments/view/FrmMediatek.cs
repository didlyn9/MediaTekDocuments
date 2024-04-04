using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly Utilisateur utilisateur;
        private bool ajouter = false;


        /// <summary>
        /// Vérifie si les droits utilisateur
        /// </summary>
        /// <param name="lutilisateur"></param>
        private void VerifDroitAccueil(Utilisateur lutilisateur)
        {
            if (!controller.VerifDroitAccueil(lutilisateur))
            {
                MessageBox.Show("Vous n'avez pas les droits");
                Application.Exit();
            }
        }

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmMediatek(Utilisateur lutilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.utilisateur = lutilisateur;
            VerifDroitAccueil(lutilisateur);
        }

        /// <summary>
        /// Fermeture de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }


        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();



        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;


        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }


        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Parutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("Numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Le numéro de parution doit être un numéro", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("Merci d'entrer un numéro de parution", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Commandes de livres
        private readonly BindingSource bdgLivresComListe = new BindingSource();
        private readonly BindingSource bdgLivresComListeCommande = new BindingSource();
        private readonly BindingSource bdgLivresComEtat = new BindingSource();
        private List<Livre> lesLivresCom = new List<Livre>();
        private List<CommandeDocument> lesCommandes = new List<CommandeDocument>();


        /// <summary>
        /// Pour remplir un combo d'etat
        /// </summary>
        /// <param name="lesEtats"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboEtat(List<Etat> lesEtats, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesEtats;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Ouverture de l'onglet Commande de livres : 
        /// Appelle les méthodes pour remplir le dgv des livres et les combos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivresCom_Enter(object sender, EventArgs e)
        {
            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Vous n'avez pas les droits.");
                tabOngletsApplication.SelectedIndex = 0;
            }
            else
            {
                tabCommandesLivres.AutoScroll = false;
                tabCommandesLivres.HorizontalScroll.Enabled = false;
                tabCommandesLivres.HorizontalScroll.Visible = false;
                tabCommandesLivres.HorizontalScroll.Maximum = 0;
                tabCommandesLivres.AutoScroll = true;
                lesLivresCom = controller.GetAllLivres();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresComGenres);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresComPublics);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresComRayons);
                RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
                EnCoursModifLivresCom(false);
                RemplirLivresComListeComplete();

            }
                

        }

        /// <summary>
        /// Pour remplir un cbx de suivi
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }


        /// <summary>
        /// Pour remplir le dgv avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresComListe(List<Livre> livres)
        {
            bdgLivresComListe.DataSource = livres;
            dgvLivresComListe.DataSource = bdgLivresComListe;
            dgvLivresComListe.Columns["isbn"].Visible = false;
            dgvLivresComListe.Columns["idRayon"].Visible = false;
            dgvLivresComListe.Columns["idGenre"].Visible = false;
            dgvLivresComListe.Columns["idPublic"].Visible = false;
            dgvLivresComListe.Columns["image"].Visible = false;
            dgvLivresComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresComListe.Columns["id"].DisplayIndex = 0;
            dgvLivresComListe.Columns["titre"].DisplayIndex = 1;
            dgvLivresComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresComListeComplete()
        {
            RemplirLivresComListe(lesLivresCom);
        }

        /// <summary>
        /// Pour annuler l'action en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnuler_Click(object sender, EventArgs e)
        {
            ajouter = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
            EnCoursModifLivresCom(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
                AfficheLivresComInfo(commandeDocument);
            }
            catch
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// Contrôle des saisies
        /// </summary>
        private void VerifValueLivreCom()
        {
            float montant = -1;
            int nbExemplaire = -1;
            try
            {
                montant = float.Parse(txbLivresComMontant.Text);
                nbExemplaire = int.Parse(txbLivresComNbExemplaires.Text);
            }
            catch
            {
                if (montant == -1)
                    MessageBox.Show("Le montant doit etre un float");
                if (nbExemplaire == -1)
                    MessageBox.Show("Le nombre d'exemplaire doit etre un integer");
            }
            Suivi suivi = (Suivi)cbxLivresComEtat.SelectedItem;
            if (suivi == null)
                MessageBox.Show("Veuillez selectionner un état");
        }

        /// <summary>
        /// Mets à jour l'affichage des livres après modif
        /// </summary>
        private void UpdateLivreCom()
        {
            if (!ajouter)
                RemplirComboSuivi(controller.GetAllSuivis(), bdgLivresComEtat, cbxLivresComEtat);
            EnCoursModifLivresCom(false);
            try
            {
                Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                AfficheLivresCommandeInfos(livre);
                txbLivresComNumLivre.Text = livre.Id;
            }
            catch
            {
                VideLivresComZones();
            }
        }

        /// <summary>
        /// Demande de modification ou d'ajout au controlleur
        /// </summary>
        /// <param name="commandeLivre"></param>
        private void PushLivresComValider(CommandeDocument commandeLivre)
        {
            bool checkValid;
            if (!ajouter)
                checkValid = controller.UpdateLivreDvdCom(commandeLivre);
            else
                checkValid = controller.CreerLivreDvdCom(commandeLivre);
            if (checkValid)
            {
                UpdateLivreCom();
            }
            else
                MessageBox.Show("Erreur");
        }


        /// <summary>
        /// Valide l'action en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sûr ?", "Oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbLivresComNbCommande.Text;
                DateTime dateCommande = dtpLivresComDateCommande.Value;
                float montant = float.TryParse(txbLivresComMontant.Text, out _) ? float.Parse(txbLivresComMontant.Text) : -1;
                int nbExemplaire = int.TryParse(txbLivresComNbExemplaires.Text, out _) ? int.Parse(txbLivresComNbExemplaires.Text) : -1;
                string idLivreDvd = txbLivresComNumLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxLivresComEtat.SelectedItem;
                VerifValueLivreCom();
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                if (montant != -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    PushLivresComValider(commandeLivre);
                }
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre correspond avec la saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbLivresComTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresComTitreRecherche.Text.Equals(""))
            {
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                txbLivresComNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivresCom.FindAll(x => x.Titre.ToLower().Contains(txbLivresComTitreRecherche.Text.ToLower()));
                RemplirLivresComListe(lesLivresParTitre);
            }
            else
            {
                if (cbxLivresComGenres.SelectedIndex < 0 && cbxLivresComPublics.SelectedIndex < 0 && cbxLivresComRayons.SelectedIndex < 0
                    && txbLivresComNumRecherche.Text.Equals(""))
                {
                    RemplirLivresComListeComplete();
                }
            }

        }

        /// <summary>
        /// Suppression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
            if (dgvLivresComListeCom.CurrentCell != null && txbLivresComNbCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sûr de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesLivresCom.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Veuillez valider", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {
                        try
                        {
                            Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbLivresComNumLivre.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une commande");
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le dgv
        /// Affichage des commandes du livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresComListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresComListe.List[bdgLivresComListe.Position];
                    AfficheLivresCommandeInfos(livre);
                    txbLivresComNumLivre.Text = livre.Id;
                }
                catch
                {
                    VideLivresComZones();
                }
            }
            else
            {
                txbLivresComNumLivre.Text = "";
                VideLivresComInfos();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Sinon affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresComNumRecherche.Text.Equals(""))
            {
                txbLivresComTitreRecherche.Text = "";
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                Livre livre = lesLivresCom.Find(x => x.Id.Equals(txbLivresComNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresComListe(livres);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirLivresComListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresComGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComGenres.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                dgvLivresComListe.ClearSelection();
                Genre genre = (Genre)cbxLivresComGenres.SelectedItem;
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresComListe(livres);

            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresComPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComPublics.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                cbxLivresComRayons.SelectedIndex = -1;
                cbxLivresComGenres.SelectedIndex = -1;
                dgvLivresComListe.ClearSelection();
                Public lePublic = (Public)cbxLivresComPublics.SelectedItem;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresComListe(livres);
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxLivresComRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresComRayons.SelectedIndex >= 0)
            {
                txbLivresComTitreRecherche.Text = "";
                txbLivresComNumRecherche.Text = "";
                cbxLivresComGenres.SelectedIndex = -1;
                cbxLivresComPublics.SelectedIndex = -1;
                dgvLivresComListe.ClearSelection();
                Rayon rayon = (Rayon)cbxLivresComRayons.SelectedItem;
                List<Livre> livres = lesLivresCom.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresComListe(livres);
            }
        }


        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }

        /// <summary>
        /// +1 sur un index numerique de type string (pour l'ajout)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string PlusUnIdString(string id)
        {
            int taille = id.Length;
            int idnum = int.Parse(id) + 1;
            id = idnum.ToString();
            if (id.Length > taille)
                MessageBox.Show("Taille du registre arrivé à saturation");
            while (id.Length != taille)
            {
                id = "0" + id;
            }
            return id;
        }

        /// <summary>
        /// Ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAjouter_Click(object sender, EventArgs e)
        {
            EnCoursModifLivresCom(true);
            txbLivresComNumLivre.ReadOnly = true;
            ajouter = true;
            string id = PlusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideLivresComInfos();
            cbxLivresComEtat.SelectedIndex = 0;
            txbLivresComNbCommande.Text = id;
            cbxLivresComEtat.Enabled = false;
        }

        private void btnlivresComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }
        
        /// <summary>
        /// Annulation, on affiche la liste des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresComListeComplete();
        }
        
        /// <summary>
        /// Sélection d'une ligne ou cellule dans le dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresComListeCom.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgLivresComListeCommande[bdgLivresComListeCommande.Position];
                    AfficheLivresComInfo(commandeDocument);
                }
                catch
                {
                    VideLivresComInfos();
                }
            }
            else
            {
                VideLivresComInfos();
            }
        }

        /// <summary>
        /// Affiche les détails d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheLivresComInfo(CommandeDocument laCommande)
        {
            txbLivresComNbCommande.Text = laCommande.Id;
            txbLivresComNumLivre.Text = laCommande.IdLivreDvd;
            dtpLivresComDateCommande.Value = laCommande.DateCommande;
            txbLivresComMontant.Text = laCommande.Montant.ToString();
            txbLivresComNbExemplaires.Text = laCommande.NbExemplaire.ToString();
            txbLivresComNumLivre.Text = laCommande.IdLivreDvd;
            cbxLivresComEtat.SelectedIndex = cbxLivresComEtat.FindString(laCommande.Etat);
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresCommandeInfos(Livre livre)
        {
            string idLivre = livre.Id;
            VideLivresComInfos();
            lesCommandes = controller.GetCommandesLivres(idLivre);
            grpLivresCommandes.Text = livre.Titre + " de " + livre.Auteur;
            if (lesCommandes.Count == 0)
                VideLivresComInfos();
            RemplirLivresComListeCommandes(lesCommandes);
        }

        /// <summary>
        /// Vide les zones d'affichage d'une commande
        /// </summary>
        private void VideLivresComInfos()
        {
            txbLivresComNbCommande.Text = "";
            dtpLivresComDateCommande.Value = DateTime.Now.Date;
            txbLivresComMontant.Text = "";
            txbLivresComNbExemplaires.Text = "";
            cbxLivresComEtat.SelectedIndex = -1;
        }

        // <summary>
        /// Vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresComZones()
        {
            cbxLivresComGenres.SelectedIndex = -1;
            cbxLivresComRayons.SelectedIndex = -1;
            cbxLivresComPublics.SelectedIndex = -1;
            txbLivresComNumRecherche.Text = "";
            txbLivresComTitreRecherche.Text = "";
            grpLivresCommandes.Text = "";
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresComZones();
            string titreColonne = dgvLivresComListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivresCom.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivresCom.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivresCom.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivresCom.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivresCom.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivresCom.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivresCom.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresComListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou d'une cellule dans le dgv
        /// Affichage des informations de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresComListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandes.Count > 0 && dgvLivresComListeCom != null)
            {
                VideLivresComInfos();
                string titreColonne = dgvLivresComListeCom.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesCommandes.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesCommandes.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandes.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandes.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirLivresComListeCommandes(sortedList);
            }
        }

        /// <summary>
        /// Pour remplir le dgv avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresComListeCommandes(List<CommandeDocument> LesCommandes)
        {
            if (LesCommandes.Count > 0)
            {
                bdgLivresComListeCommande.DataSource = LesCommandes;
                dgvLivresComListeCom.DataSource = bdgLivresComListeCommande;
                dgvLivresComListeCom.Columns["idLivreDvd"].Visible = false;
                dgvLivresComListeCom.Columns["idSuivi"].Visible = false;
                dgvLivresComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresComListeCom.Columns["id"].DisplayIndex = 0;
                dgvLivresComListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvLivresComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvLivresComListeCom.Columns.Clear();
            }

        }

        /// <summary>
        /// Ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresComListeCom.CurrentCell != null && txbLivresComNbCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxLivresComEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                EnCoursModifLivresCom(true);
                RemplirComboSuivi(lesSuivi, bdgLivresComEtat, cbxLivresComEtat);
                cbxLivresComEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionnée");
            }
        }


        /// <summary>
        /// Contrôle sur les possibilités d'entrées en fonction des actions
        /// </summary>
        /// <param name="modif"></param>
        private void EnCoursModifLivresCom(bool modif)
        {
            btnLivresComAjouter.Enabled = !modif;
            btnLivresComSupprimer.Enabled = !modif;
            btnLivresComModifier.Enabled = !modif;
            txbLivresComNbCommande.ReadOnly = true;
            dtpLivresComDateCommande.Enabled = modif;
            txbLivresComNbExemplaires.ReadOnly = !modif;
            txbLivresComNumLivre.ReadOnly = true;
            txbLivresComMontant.ReadOnly = !modif;
            cbxLivresComEtat.Enabled = modif;
            dgvLivresComListe.Enabled = !modif;
            dgvLivresComListeCom.Enabled = !modif;
            cbxLivresComGenres.Enabled = !modif;
            cbxLivresComPublics.Enabled = !modif;
            cbxLivresComRayons.Enabled = !modif;
            btnLivresComNbRecherche.Enabled = !modif;
            txbLivresComTitreRecherche.Enabled = !modif;
            btnLivresComAnnulRayons.Enabled = !modif;
            btnLivresComAnnulGenres.Enabled = !modif;
            btnlivresComAnnulPublics.Enabled = !modif;
            ajouter = false;
        }




        #endregion

        #region Commande de DVDs
        private readonly BindingSource bdgDvdComListe = new BindingSource();
        private readonly BindingSource bdgDvdComListeCommande = new BindingSource();
        private readonly BindingSource bdgDvdComEtat = new BindingSource();
        private List<Dvd> lesDvdCom = new List<Dvd>();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirDvdComListe(List<Dvd> livres)
        {
            bdgDvdComListe.DataSource = livres;
            dgvDvdComListe.DataSource = bdgDvdComListe;
            dgvDvdComListe.Columns["synopsis"].Visible = false;
            dgvDvdComListe.Columns["idRayon"].Visible = false;
            dgvDvdComListe.Columns["idGenre"].Visible = false;
            dgvDvdComListe.Columns["idPublic"].Visible = false;
            dgvDvdComListe.Columns["image"].Visible = false;
            dgvDvdComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdComListe.Columns["id"].DisplayIndex = 0;
            dgvDvdComListe.Columns["titre"].DisplayIndex = 1;
            dgvDvdComListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdComZones()
        {
            cbxDvdComGenres.SelectedIndex = -1;
            cbxDvdComRayons.SelectedIndex = -1;
            cbxDvdComPublics.SelectedIndex = -1;
            txbDvdComNumRecherche.Text = "";
            txbDvdComTitreRecherche.Text = "";
            grpDvdCommandes.Text = "";
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdComListeComplete()
        {
            RemplirDvdComListe(lesDvdCom);
            VideDvdComZones();
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void EnCoursModifDvdCom(bool modif)
        {
            btnDvdComAjouter.Enabled = !modif;
            btnDvdComSupprimer.Enabled = !modif;
            btnDvdComModifier.Enabled = !modif;
            btnDvdComAnnuler.Enabled = modif;
            btnDvdComValider.Enabled = modif;
            txbDvdComNbCommande.ReadOnly = true;
            dtpDvdComDateCommande.Enabled = modif;
            txbDvdComNbExemplaires.ReadOnly = !modif;
            txbDvdComNumLivre.ReadOnly = true;
            txbDvdComMontant.ReadOnly = !modif;
            cbxDvdComEtat.Enabled = modif;
            dgvDvdComListe.Enabled = !modif;
            dgvDvdComListeCom.Enabled = !modif;
            cbxDvdComGenres.Enabled = !modif;
            cbxDvdComPublics.Enabled = !modif;
            cbxDvdComRayons.Enabled = !modif;
            btnDvdComNbRecherche.Enabled = !modif;
            txbDvdComTitreRecherche.Enabled = !modif;
            btnDvdComAnnulRayons.Enabled = !modif;
            btnDvdComAnnulGenres.Enabled = !modif;
            btnlivresComAnnulPublics.Enabled = !modif;
            ajouter = false;

        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideDvdComInfos()
        {
            txbDvdComNbCommande.Text = "";
            dtpDvdComDateCommande.Value = DateTime.Now.Date;
            txbDvdComMontant.Text = "";
            txbDvdComNbExemplaires.Text = "";
            cbxDvdComEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheDvdCommandeInfos(Dvd dvd)
        {
            string idDvd = dvd.Id;
            VideDvdComInfos();
            lesCommandesDvd = controller.GetCommandesLivres(idDvd);
            grpDvdCommandes.Text = dvd.Titre + " de " + dvd.Realisateur;
            if (lesCommandes.Count == 0)
                VideDvdComInfos();
            RemplirDvdComListeCommandes(lesCommandesDvd);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirDvdComListeCommandes(List<CommandeDocument> LesCommandes)
        {
            if (LesCommandes.Count > 0)
            {
                bdgDvdComListeCommande.DataSource = LesCommandes;
                dgvDvdComListeCom.DataSource = bdgDvdComListeCommande;
                dgvDvdComListeCom.Columns["idLivreDvd"].Visible = false;
                dgvDvdComListeCom.Columns["idSuivi"].Visible = false;
                dgvDvdComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvDvdComListeCom.Columns["id"].DisplayIndex = 0;
                dgvDvdComListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvDvdComListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvDvdComListeCom.Columns.Clear();
            }

        }

        private void dgvDvdComListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdComListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                    AfficheDvdCommandeInfos(dvd);
                    txbDvdComNumLivre.Text = dvd.Id;
                }
                catch
                {
                    VideDvdComZones();
                }
            }
            else
            {
                txbDvdComNumLivre.Text = "";
                VideDvdComInfos();
            }
        }

        private void dgvDvdComListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdComZones();
            string titreColonne = dgvDvdComListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvdCom.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvdCom.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvdCom.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvdCom.OrderBy(o => o.Duree).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvdCom.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvdCom.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvdCom.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdComListe(sortedList);
        }

        /// <summary>
        /// Ouverture de l'onglet Dvd : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {

            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Vous n'avez pas les droits.");
                tabOngletsApplication.SelectedIndex = 0;
            }
            else
            {
                lesDvdCom = controller.GetAllDvd();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdComGenres);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdComPublics);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdComRayons);
                RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
                tabCommandesDvd.AutoScroll = false;
                tabCommandesDvd.HorizontalScroll.Enabled = false;
                tabCommandesDvd.HorizontalScroll.Visible = false;
                tabCommandesDvd.HorizontalScroll.Maximum = 0;
                tabCommandesDvd.AutoScroll = true;
                EnCoursModifDvdCom(false);
                RemplirDvdComListeComplete();
            }
            

        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdComTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdComTitreRecherche.Text.Equals(""))
            {
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                txbDvdComNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvdCom.FindAll(x => x.Titre.ToLower().Contains(txbDvdComTitreRecherche.Text.ToLower()));
                RemplirDvdComListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdComGenres.SelectedIndex < 0 && cbxDvdComPublics.SelectedIndex < 0 && cbxDvdComRayons.SelectedIndex < 0
                    && txbDvdComNumRecherche.Text.Equals(""))
                {
                    RemplirDvdComListeComplete();
                }
            }
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdComNumRecherche.Text.Equals(""))
            {
                txbDvdComTitreRecherche.Text = "";
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                Dvd dvd = lesDvdCom.Find(x => x.Id.Equals(txbDvdComNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> lesDvds = new List<Dvd>() { dvd };
                    RemplirDvdComListe(lesDvds);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirDvdComListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdComGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComGenres.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                dgvDvdComListe.ClearSelection();
                Genre genre = (Genre)cbxDvdComGenres.SelectedItem;
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                List<Dvd> lesDvds = lesDvdCom.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdComListe(lesDvds);

            }
        }

        /// <summary>
        /// Filtre sur le public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdComPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComPublics.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                cbxDvdComRayons.SelectedIndex = -1;
                cbxDvdComGenres.SelectedIndex = -1;
                dgvDvdComListe.ClearSelection();
                Public lePublic = (Public)cbxDvdComPublics.SelectedItem;
                List<Dvd> lesDvds = lesDvdCom.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdComListe(lesDvds);
            }
        }

        /// <summary>
        /// Filtre sur les rayons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdComRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdComRayons.SelectedIndex >= 0)
            {
                txbDvdComTitreRecherche.Text = "";
                txbDvdComNumRecherche.Text = "";
                cbxDvdComGenres.SelectedIndex = -1;
                cbxDvdComPublics.SelectedIndex = -1;
                dgvDvdComListe.ClearSelection();
                Rayon rayon = (Rayon)cbxDvdComRayons.SelectedItem;
                List<Dvd> LesDvd = lesDvdCom.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdComListe(LesDvd);
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Clic bouton annuler public, affichage de la liste complète des dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Clic bouton annuler rayons, affichage de la liste complète des dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdComListeComplete();
        }

        /// <summary>
        /// Ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAjouter_Click(object sender, EventArgs e)
        {
            EnCoursModifDvdCom(true);
            txbDvdComNumLivre.ReadOnly = true;
            ajouter = true;
            string id = PlusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideDvdComInfos();
            cbxDvdComEtat.SelectedIndex = 0;
            txbDvdComNbCommande.Text = id;
            cbxDvdComEtat.Enabled = false;
        }

        /// <summary>
        /// Modification d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComModifier_Click(object sender, EventArgs e)
        {
            if (dgvDvdComListeCom.CurrentCell != null && txbDvdComNbCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxDvdComEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                EnCoursModifDvdCom(true);
                RemplirComboSuivi(lesSuivi, bdgDvdComEtat, cbxDvdComEtat);
                cbxDvdComEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionnée");
            }
        }

        /// <summary>
        /// Supprimer une commande Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
            if (dgvDvdComListeCom.CurrentCell != null && txbDvdComNbCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sûr de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesDvdCom.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation de la suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {
                        try
                        {
                            Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                            AfficheDvdCommandeInfos(dvd);
                            txbDvdComNumLivre.Text = dvd.Id;
                        }
                        catch
                        {
                            VideDvdComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }
        }

        /// <summary>
        /// Affiche les détails d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheDvdComInfo(CommandeDocument laCommande)
        {
            txbDvdComNbCommande.Text = laCommande.Id;
            txbDvdComNumLivre.Text = laCommande.IdLivreDvd;
            dtpDvdComDateCommande.Value = laCommande.DateCommande;
            txbDvdComMontant.Text = laCommande.Montant.ToString();
            txbDvdComNbExemplaires.Text = laCommande.NbExemplaire.ToString();
            txbDvdComNumLivre.Text = laCommande.IdLivreDvd;
            cbxDvdComEtat.SelectedIndex = cbxDvdComEtat.FindString(laCommande.Etat);
        }

        /// <summary>
        /// Annuler l'action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnuler_Click(object sender, EventArgs e)
        {
            ajouter = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
            EnCoursModifDvdCom(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
                AfficheDvdComInfo(commandeDocument);
            }
            catch
            {
                VideDvdComInfos();
            }
        }

        /// <summary>
        /// Vérification des données
        /// </summary>
        private void VerifValueDvdComPourx()
        {
            float montant = -1;
            int nbExemplaire = -1;
            try
            {
                montant = float.Parse(txbDvdComMontant.Text);
                nbExemplaire = int.Parse(txbDvdComNbExemplaires.Text);
            }
            catch
            {
                if (montant == -1)
                    MessageBox.Show("Le montant doit etre un float");
                if (nbExemplaire == -1)
                    MessageBox.Show("Le nombre d'exemplaire doit etre un integer");
            }
            Suivi suivi = (Suivi)cbxDvdComEtat.SelectedItem;
            if (suivi == null)
                MessageBox.Show("Veuillez selectionner un état");
        }

        /// <summary>
        /// Actualise les infos
        /// </summary>
        private void UpdateDvd()
        {
            EnCoursModifDvdCom(false);
            try
            {
                Dvd dvd = (Dvd)bdgDvdComListe.List[bdgDvdComListe.Position];
                AfficheDvdCommandeInfos(dvd);
                txbDvdComNumLivre.Text = dvd.Id;
            }
            catch
            {
                VideDvdComZones();
            }
        }

        /// <summary>
        /// Demande au contrôleur de valider l'action (dvd/livre)
        /// </summary>
        /// <param name="commandeLivre"></param>
        public void DvdComValide(CommandeDocument commandeLivre)
        {
            bool checkValid;
            if (!ajouter)
                checkValid = controller.UpdateLivreDvdCom(commandeLivre);
            else
                checkValid = controller.CreerLivreDvdCom(commandeLivre);
            if (checkValid)
            {
                if (!ajouter)
                    RemplirComboSuivi(controller.GetAllSuivis(), bdgDvdComEtat, cbxDvdComEtat);
                UpdateDvd();
            }
            else
                MessageBox.Show("Erreur");
        }

        /// <summary>
        /// Valide l'action en cours 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sûr ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbDvdComNbCommande.Text;
                DateTime dateCommande = dtpDvdComDateCommande.Value;
                float montant = float.TryParse(txbDvdComMontant.Text, out _) ? float.Parse(txbDvdComMontant.Text) : -1;
                int nbExemplaire = int.TryParse(txbDvdComNbExemplaires.Text, out _) ? int.Parse(txbDvdComNbExemplaires.Text) : -1;
                string idLivreDvd = txbDvdComNumLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxDvdComEtat.SelectedItem;
                VerifValueDvdComPourx();
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                if (montant != -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    DvdComValide(commandeLivre);
                }
            }
        }

        /// <summary>
        /// Sélection d'une ligne ou cellule dans le dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandesDvd.Count > 0 && dgvDvdComListeCom != null)
            {
                VideDvdComInfos();
                string titreColonne = dgvDvdComListeCom.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandesDvd.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirDvdComListeCommandes(sortedList);
            }
        }

        /// <summary>
        /// Sélection dans le dgv des commandes dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdComListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdComListeCom.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgDvdComListeCommande[bdgDvdComListeCommande.Position];
                    AfficheDvdComInfo(commandeDocument);
                }
                catch
                {
                    VideDvdComInfos();
                }
            }
            else
            {
                VideDvdComInfos();
            }
        }

        #endregion

        #region Abonnements
        private readonly BindingSource bdgAboListe = new BindingSource();
        private readonly BindingSource bdgAboListeCommande = new BindingSource();
        private List<Revue> lesRevuesAbo = new List<Revue>();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        bool filtre;

        /// <summary>
        /// Contrôle des entrées en fonction de l'action
        /// </summary>
        /// <param name="modif"></param>
        private void EnCoursModifAbo(bool modif)
        {
            btnAboAjouter.Enabled = !modif;
            btnAboSupprimer.Enabled = !modif;
            btnAboModifier.Enabled = !modif;
            btnAboAnnuler.Enabled = modif;
            btnAboValider.Enabled = modif;
            txbAboNbCommande.ReadOnly = true;
            dtpAboDateCommande.Enabled = modif;
            txbAboNumRevue.ReadOnly = true;
            txbAboMontant.ReadOnly = !modif;
            dtpAboDateCommande.Enabled = modif;
            dtpAboDateFin.Enabled = modif;
            dgvAboListe.Enabled = !modif;
            dgvAboListeCom.Enabled = !modif;
            cbxAboGenres.Enabled = !modif;
            cbxAboPublics.Enabled = !modif;
            cbxAboRayons.Enabled = !modif;
            btnAboNbRecherche.Enabled = !modif;
            txbAboTitreRecherche.Enabled = !modif;
            btnAboAnnulRayons.Enabled = !modif;
            btnAboAnnulGenres.Enabled = !modif;
            btnAboAnnulPublics.Enabled = !modif;
            ajouter = false;
        }

        /// <summary>
        /// Pour remplir le dgv Abo
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirAboListe(List<Revue> revues)
        {
            bdgAboListe.DataSource = revues;
            dgvAboListe.DataSource = bdgAboListe;
            dgvAboListe.Columns["idRayon"].Visible = false;
            dgvAboListe.Columns["idGenre"].Visible = false;
            dgvAboListe.Columns["idPublic"].Visible = false;
            dgvAboListe.Columns["image"].Visible = false;
            dgvAboListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAboListe.Columns["id"].DisplayIndex = 0;
            dgvAboListe.Columns["titre"].DisplayIndex = 1;
            dgvAboListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // <summary>
        /// Vide les entrées
        /// </summary>
        private void VideAboZones()
        {
            cbxAboGenres.SelectedIndex = -1;
            cbxAboRayons.SelectedIndex = -1;
            cbxAboPublics.SelectedIndex = -1;
            txbAboNumRecherche.Text = "";
            txbAboTitreRecherche.Text = "";
            grpAboCommandes.Text = "";
        }

        /// <summary>
        /// Affichage de la liste complète des abo
        /// </summary>
        private void RemplirAboListeComplete()
        {
            RemplirAboListe(lesRevuesAbo);
            VideAboZones();
        }

        /// <summary>
        /// Recherche et affichage des Abo dont le titre correspond la saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbAboTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbAboTitreRecherche.Text.Equals(""))
            {
                cbxAboGenres.SelectedIndex = -1;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                txbAboNumRecherche.Text = "";
                List<Revue> lesRevueParTitre;
                lesRevueParTitre = lesRevuesAbo.FindAll(x => x.Titre.ToLower().Contains(txbAboTitreRecherche.Text.ToLower()));
                RemplirAboListe(lesRevueParTitre);
            }
            else
            {
                if (cbxAboGenres.SelectedIndex < 0 && cbxAboPublics.SelectedIndex < 0 && cbxAboRayons.SelectedIndex < 0
                    && txbAboNumRecherche.Text.Equals(""))
                {
                    RemplirAboListeComplete();
                }
            }
        }

        /// <summary>
        /// Ouverture de l'onglet Abonnements : 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnements_Enter(object sender, EventArgs e)
        {
            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Vous n'avez pas les droits.");
                tabOngletsApplication.SelectedIndex = 0;
            }
            else
            {
                lesRevuesAbo = controller.GetAllRevues();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxAboGenres);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxAboPublics);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxAboRayons);
                tabAbonnements.AutoScroll = false;
                tabAbonnements.HorizontalScroll.Enabled = false;
                tabAbonnements.HorizontalScroll.Visible = false;
                tabAbonnements.HorizontalScroll.Maximum = 0;
                tabAbonnements.AutoScroll = true;
                EnCoursModifAbo(false);
                RemplirAboListeComplete();
                filtre = false;

            }

        }

        /// <summary>
        /// Recherche et affichage abonnement
        /// Retourne un message si non trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboNbRecherche_Click(object sender, EventArgs e)
        {
            if (!txbAboNumRecherche.Text.Equals(""))
            {
                txbAboTitreRecherche.Text = "";
                cbxAboGenres.SelectedIndex = -1;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                Revue revue = lesRevuesAbo.Find(x => x.Id.Equals(txbAboNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirAboListe(revues);
                }
                else
                {
                    MessageBox.Show("Introuvable");
                    RemplirAboListeComplete();
                }
            }
            else
            {
                RemplirAboListeComplete();
            }
        }

        /// <summary>
        /// Filtre les abonnement dont la fin est dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboFiltreRevue_Click(object sender, EventArgs e)
        {
            if (filtre)
            {
                lesRevuesAbo = controller.GetAllRevues();
                btnAboFiltreRevue.Text = "Filtrer";
                filtre = false;
            }
            else
            {
                List<Revue> listeTrie = new List<Revue>();
                foreach (Revue revue in lesRevuesAbo)
                {
                    List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                    if (abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddDays(30))
                            && (o.DateFinAbonnement >= DateTime.Now)).Count > 0)
                        listeTrie.Add(revue);
                }
                lesRevuesAbo = listeTrie;
                btnAboFiltreRevue.Text = "X";
                filtre = true;
            }
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAboGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboGenres.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                dgvAboListe.ClearSelection();
                Genre genre = (Genre)cbxAboGenres.SelectedItem;
                cbxAboRayons.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                List<Revue> lesRevuesGenres = lesRevuesAbo.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirAboListe(lesRevuesGenres);

            }
        }

        /// <summary>
        /// Filtre sur le public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAboPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboPublics.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                cbxAboRayons.SelectedIndex = -1;
                cbxAboGenres.SelectedIndex = -1;
                dgvAboListe.ClearSelection();
                Public lePublic = (Public)cbxAboPublics.SelectedItem;
                List<Revue> lesRevuesPublics = lesRevuesAbo.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirAboListe(lesRevuesPublics);
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAboRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboRayons.SelectedIndex >= 0)
            {
                txbAboTitreRecherche.Text = "";
                txbAboNumRecherche.Text = "";
                cbxAboGenres.SelectedIndex = -1;
                cbxAboPublics.SelectedIndex = -1;
                dgvAboListe.ClearSelection();
                Rayon rayon = (Rayon)cbxAboRayons.SelectedItem;
                List<Revue> lesRevuesRayons = lesRevuesAbo.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirAboListe(lesRevuesRayons);
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des abonnements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des abonnements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des abonnements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirAboListeComplete();
        }

        /// <summary>
        /// Pour remplir le dgv Abo
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirAboListeCommandes(List<Abonnement> LesAbonnements)
        {
            if (LesAbonnements.Count > 0)
            {
                bdgAboListeCommande.DataSource = LesAbonnements;
                dgvAboListeCom.DataSource = bdgAboListeCommande;
                dgvAboListeCom.Columns["idRevue"].Visible = false;
                dgvAboListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAboListeCom.Columns["id"].DisplayIndex = 0;
                dgvAboListeCom.Columns["dateCommande"].DisplayIndex = 1;
                dgvAboListeCom.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvAboListeCom.Columns.Clear();
            }

        }

        /// <summary>
        /// Filtre les abonnement dont la fin est dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboFiltreAbo_Click(object sender, EventArgs e)
        {
            lesAbonnements = lesAbonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddDays(30))
                && (o.DateFinAbonnement >= DateTime.Now));
            if (lesAbonnements.Count == 0)
                VideAboInfos();
            RemplirAboListeCommandes(lesAbonnements);

        }

        /// <summary>
        /// Vide les entrées d'un abonnement
        /// </summary>
        private void VideAboInfos()
        {
            txbAboNbCommande.Text = "";
            dtpAboDateCommande.Value = DateTime.Now.Date;
            txbAboMontant.Text = "";
            dtpAboDateFin.Value = DateTime.Now.Date.AddMonths(6);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListeCom_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesAbonnements.Count > 0 && dgvAboListeCom != null)
            {
                VideAboInfos();
                string titreColonne = dgvAboListeCom.Columns[e.ColumnIndex].HeaderText;
                List<Abonnement> sortedList = new List<Abonnement>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesAbonnements.OrderBy(o => o.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = lesAbonnements.OrderBy(o => o.DateCommande).ToList();
                        break;
                    case "Montant":
                        sortedList = lesAbonnements.OrderBy(o => o.Montant).ToList();
                        break;
                    case "DateFinAbonnement":
                        sortedList = lesAbonnements.OrderBy(o => o.DateFinAbonnement).ToList();
                        break;
                }
                RemplirAboListeCommandes(sortedList);
            }
        }

        /// <summary>
        /// Afficher les détails d'un abonnement
        /// </summary>
        /// <param name="labonnement"></param>
        private void AfficheAboInfo(Abonnement labonnement)
        {
            txbAboNbCommande.Text = labonnement.Id;
            txbAboNumRevue.Text = labonnement.IdRevue;
            dtpAboDateCommande.Value = labonnement.DateCommande;
            txbAboMontant.Text = labonnement.Montant.ToString();
            dtpAboDateFin.Value = labonnement.DateFinAbonnement;
        }

        /// <summary>
        /// Affichages des détails d'un abonnement après sélection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListeCom_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAboListeCom.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
                    AfficheAboInfo(abonnement);
                }
                catch
                {
                    VideAboInfos();
                }
            }
            else
            {
                VideAboInfos();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideAboZones();
            string titreColonne = dgvAboListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevuesAbo.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevuesAbo.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirAboListe(sortedList);
        }

        /// <summary>
        /// Affichag de l'abonnement
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheAboInfos(Revue revue)
        {
            string idRevue = revue.Id;
            VideAboInfos();
            lesAbonnements = controller.GetAbonnements(idRevue);
            grpAboCommandes.Text = revue.Titre;
            if (lesAbonnements.Count == 0)
                VideAboInfos();
            RemplirAboListeCommandes(lesAbonnements);
        }

        /// <summary>
        /// Affichage des abonnements pour la revue après sélection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAboListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAboListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgAboListe.List[bdgAboListe.Position];
                    AfficheAboInfos(revue);
                    txbAboNumRevue.Text = revue.Id;
                }
                catch
                {
                    VideAboZones();
                }
            }
            else
            {
                txbAboNumRevue.Text = "";
                VideAboInfos();
            }
        }

        /// <summary>
        /// Ajout Abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAjouter_Click(object sender, EventArgs e)
        {
            EnCoursModifAbo(true);
            ajouter = true;
            string id = PlusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideAboInfos();
            dtpAboDateCommande.Value = DateTime.Now;
            dtpAboDateFin.Value = DateTime.Now.AddMonths(2);
            txbAboNbCommande.Text = id;
            Console.WriteLine("btnAboAjouter_Click  " + id);
        }

        /// <summary>
        /// Modif Abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboModifier_Click(object sender, EventArgs e)
        {
            if (dgvAboListeCom.CurrentCell != null && txbAboNbCommande.Text != "")
            {
                EnCoursModifAbo(true);
            }
            else
            {
                MessageBox.Show("Aucune revue sélectionnée");
            }
        }

        /// <summary>
        /// Vérifie si au moins un exemplaire a été acquis pendant la période de validité de l'abonnement.
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        private bool VerifExemplaireAbo(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplairesAbo = controller.GetExemplairesRevue(abonnement.IdRevue);
            return lesExemplairesAbo.FindAll(o => (o.DateAchat >= abonnement.DateCommande) && (o.DateAchat <= abonnement.DateCommande)).Count > 0;
        }

        /// <summary>
        /// Suppression Abo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboSupprimer_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
            if (dgvAboListeCom.CurrentCell != null && txbAboNbCommande.Text != "")
            {
                if (VerifExemplaireAbo(abonnement))
                    MessageBox.Show("Une revue a été livrée le temps de cet abonnement et ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sûr de vouloir supprimer la commande n°" + abonnement.Id +
                    " concernant " + lesRevuesAbo.Find(o => o.Id == abonnement.IdRevue).Titre + " ?",
                    "Validation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerAbonnement(abonnement))
                    {
                        try
                        {
                            Revue Revue = (Revue)bdgAboListe.List[bdgAboListe.Position];
                            AfficheAboInfos(Revue);
                            txbAboNumRevue.Text = Revue.Id;
                        }
                        catch
                        {
                            VideAboZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un abonnement");
            }
        }

        /// <summary>
        /// Annule l'action Abo en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboAnnuler_Click(object sender, EventArgs e)
        {
            ajouter = false;
            EnCoursModifAbo(false);
            try
            {
                Abonnement abonnement = (Abonnement)bdgAboListeCommande[bdgAboListeCommande.Position];
                AfficheAboInfo(abonnement);
            }
            catch
            {
                VideAboInfos();
            }
        }

        /// <summary>
        /// Valide l'action Abo en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboValider_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous bien sûr ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbAboNbCommande.Text;
                double montant = 0;
                double? b = null;
                try
                {
                    montant = double.Parse(txbAboMontant.Text);
                    b = montant;
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un float");
                }
                DateTime dateDeCommande = dtpAboDateCommande.Value;
                DateTime dateFin = dtpAboDateFin.Value;
                string idRevue = txbAboNumRevue.Text;
                if (b != null && dateDeCommande <= dateFin && idRevue != "")
                {
                    Abonnement abonnement = new Abonnement(id, dateDeCommande, montant, dateFin, idRevue);
                    if (!ajouter)
                        checkValid = controller.UpdateAbonnement(abonnement);
                    else
                        checkValid = controller.CreerAbonnement(abonnement);
                    if (checkValid)
                    {
                        EnCoursModifAbo(false);
                        lesRevuesAbo = controller.GetAllRevues();
                        RemplirAboListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        #endregion

    }
}
