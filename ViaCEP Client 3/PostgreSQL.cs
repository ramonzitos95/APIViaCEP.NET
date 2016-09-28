using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ViaCEP_Client_3
{
    class PostgreSQL
    {
        private NpgsqlConnection connection;

        public bool OpenConnection(string host, string port, string database, string username, string password)
        {
            try
            {
                if (CloseConnection()) //Garante apenas uma conexão, sem multiplicadas na base de dados
                {
                    connection = new NpgsqlConnection();

                    
                    connection.ConnectionString = String.Format(@"Host={0}; Port={1}; Database={2}; Username={3}; Password={4}",
                                                                host, port, database, username, password);

                    connection.Open();
                }

                return true;
            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(ne.Message);

                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
            //finally
            //{

            //}
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection != null)
                    connection.Close();
                return true;
            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(ne.Message);

                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
        }

        public List<Endereco> GetData(string tabela)
        {
            try
            {
                string query = string.Format("SELECT * FROM  {0}", tabela); //Consulta com o banco

                NpgsqlCommand command = new NpgsqlCommand(query, connection); //Comando para a query

                List<Endereco> enderecos = new List<Endereco>(); //Classe Endereco

                NpgsqlDataReader dataReader = command.ExecuteReader(); //Ler dados da tabela, para adicionar ao listView

                while (dataReader.Read())
                {
                    //Criando um objeto do tipo endereco
                    var endereco = new Endereco();
                    //Adicionar o conteudo  da data reader ao objeto
                    endereco.Cep = dataReader.GetString(0);
                    endereco.Logradouro = dataReader.GetString(1);
                    endereco.Bairro = dataReader.GetString(2);
                    endereco.Localidade = dataReader.GetString(3);
                    endereco.Uf = dataReader.GetString(4);

                    //inserir o objeto na lista 
                    enderecos.Add(endereco);
                }

                return enderecos;
            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(ne.Message);

                return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return null;
            }
        }
        
        public bool SetData(string tabela, Endereco endereco)
        {
            try
            {
                string query = string.Format(@"INSERT INTO {0} VALUES('{1}', '{2}', '{3}', '{4}', '{5}')", tabela, endereco.Cep, endereco.Logradouro, endereco.Bairro, endereco.Localidade, endereco.Uf);

                var command = new NpgsqlCommand(query, connection);

                if(command.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(ne.Message);

                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
        }

    }
}

