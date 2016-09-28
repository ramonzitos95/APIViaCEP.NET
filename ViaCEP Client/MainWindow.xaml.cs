using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
namespace ViaCEP_Client_3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PostgreSQL banco;

        private List<Endereco> listaEnderecos;

        private GridViewColumnHeader lastheaderClicked;
        
        private ListSortDirection lastdirection = ListSortDirection.Descending;

        public MainWindow()
        {
            InitializeComponent();

            listaEnderecos = new List<Endereco>();
            listViewEnderecos.ItemsSource = listaEnderecos;

            maskedTextBoxCep.Focus();

        }

        public async void GetCep(string cep, string formatoRetorno)
        {

            if (listaEnderecos.Exists(l => l.Cep == cep))
            {
                SelecionarLinha(cep);

                return;
            }

            //verifica a conectividade de rede
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Verifique a conexão de rede...");

                return;
            }


            string uri = "http://viacep.com.br/ws/" + cep.Trim() + "/" + formatoRetorno.Trim() + "/";

            HttpClient cliente = new HttpClient();

            HttpResponseMessage resposta = await cliente.GetAsync(uri);

            HttpContent conteudo = resposta.Content;

            string resultado = await conteudo.ReadAsStringAsync();

            //Manipula o retorno do Web Service
            Endereco endereco = new Endereco();

            switch (formatoRetorno)
            {
                case "json":

                    var json = JObject.Parse(resultado);

                    endereco.Cep = (string)json["cep"];
                    endereco.Logradouro = (string)json["logradouro"];
                    endereco.Bairro = (string)json["bairro"];
                    endereco.Localidade = (string)json["localidade"];
                    endereco.Uf = (string)json["uf"];


                    break;

                case "xml":

                    var xml = XDocument.Parse(resultado);

                    endereco.Cep = (string)xml.Root.Element("cep");
                    endereco.Logradouro = (string)xml.Root.Element("logradouro");
                    endereco.Bairro = (string)xml.Root.Element("bairro");
                    endereco.Localidade = (string)xml.Root.Element("localidade");
                    endereco.Uf = (string)xml.Root.Element("uf");


                    break;
            }


            listaEnderecos.Add(endereco);

            listViewEnderecos.Items.Refresh();

            OrdenarListas(listViewEnderecos,
                          lastheaderClicked,
                          lastdirection);

            SelecionarLinha(cep);
        }




            public void OrdenarListas(ListView lv,
                        GridViewColumnHeader gvch,
                        ListSortDirection lsd)
            {
                if (gvch == null)
                    return;
                var sd = new SortDescription((string)gvch.Column.Header, lsd);

                var defaultView = CollectionViewSource.GetDefaultView(lv.ItemsSource);

                defaultView.SortDescriptions.Clear();
                defaultView.SortDescriptions.Add(sd);
                defaultView.Refresh();
            }

            

        
        

        

        private void SelecionarLinha(string cep)
        {
            foreach (var item in listViewEnderecos.Items.SourceCollection)
            {
                var i = (Endereco)item;

                if (i.Cep.Equals(cep))
                {
                    listViewEnderecos.SelectedIndex = listViewEnderecos.Items.IndexOf(i);

                    return;
                }
            }
        }

        private void buttonPesquisar_Click(object sender, RoutedEventArgs e)
        {
            if(!maskedTextBoxCep.IsMaskCompleted)
            {
                MessageBox.Show("Verifique o CEP...");
                maskedTextBoxCep.Focus();
                return;
            }

            if(comboBoxFormatoRetorno.Text.Trim().Length == 0)
            {
                MessageBox.Show("Verifique o Formato de Retorno...");
                comboBoxFormatoRetorno.Focus();
                return;
            }

            GetCep(maskedTextBoxCep.Text, comboBoxFormatoRetorno.Text);

            maskedTextBoxCep.Focus();
        }

        private void listViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            lastheaderClicked = (GridViewColumnHeader)e.OriginalSource;

            lastdirection = (lastdirection != ListSortDirection.Ascending) 
                            ? ListSortDirection.Ascending 
                            : ListSortDirection.Descending;

            OrdenarListas(listViewEnderecos,
                          lastheaderClicked,  
                          lastdirection);

            
        }

    }

}
