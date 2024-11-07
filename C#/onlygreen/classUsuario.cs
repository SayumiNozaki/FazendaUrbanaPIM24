using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace onlygreen
{
    public class classUsuario
    {
        public Int32 id;
        public string nome;
        public string cpf;
        public string telefone;
        public string email;
        public string rua;
        public string nrua;
        public string bairro;
        public string cidade;
        public string estado;
        public string cep;
        public string tipousuario;
        public string ulogar;
        public string senha;
        public string situacao;

        public static void INSERTuser(classUsuario u)
        {
            try
            {
                string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
                using (var conectar = new SqlConnection(bdonlygreen))
                {
                    conectar.Open();
                    var command = new SqlCommand("INSERT INTO tb_Usuario (nome, cpf, telefone, email, rua, nrua, bairro, cidade, estado, cep, ulogar, senha, tipousuario, situacao) " +
                                                 "VALUES (@nome, @cpf, @telefone, @email, @rua, @nrua, @bairro, @cidade, @estado, @cep, @ulogar, @senha, @tipousuario, @situacao)", conectar);

                    command.Parameters.AddWithValue("@nome", u.nome);
                    command.Parameters.AddWithValue("@cpf", u.cpf);
                    command.Parameters.AddWithValue("@telefone", u.telefone);
                    command.Parameters.AddWithValue("@email", u.email);
                    command.Parameters.AddWithValue("@rua", u.rua);
                    command.Parameters.AddWithValue("@nrua", u.nrua);
                    command.Parameters.AddWithValue("@bairro", u.bairro);
                    command.Parameters.AddWithValue("@cidade", u.cidade);
                    command.Parameters.AddWithValue("@estado", u.estado);
                    command.Parameters.AddWithValue("@cep", u.cep);
                    command.Parameters.AddWithValue("@ulogar", u.ulogar);
                    command.Parameters.AddWithValue("@senha", u.senha);
                    command.Parameters.AddWithValue("@tipousuario", u.tipousuario);
                    command.Parameters.AddWithValue("@situacao", u.situacao);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Usuário adicionado com sucesso.");
                }
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("Cpf ou nome já existe no banco de dados. Altere e tente novamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }


        public static void UPDATEuser(classUsuario u)
        {
            try
            {
                string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
                using (SqlConnection conectar = new SqlConnection(bdonlygreen))
                {
                    conectar.Open();

                    string sql = "UPDATE tb_Usuario SET " +
                                 "nome = @nome, " +
                                 "telefone = @telefone, " +
                                 "email = @email, " +
                                 "rua = @rua, " +
                                 "nrua = @nrua, " +
                                 "bairro = @bairro, " +
                                 "cidade = @cidade, " +
                                 "estado = @estado, " +
                                 "cep = @cep, " +
                                 "tipousuario = @tipousuario, " +
                                 "situacao = @situacao, " +
                                 "senha = @senha " +
                                 "WHERE ulogar = @ulogar;";


                    using (SqlCommand cmd = new SqlCommand(sql, conectar))
                    {
                        cmd.Parameters.AddWithValue("@nome", u.nome);
                        cmd.Parameters.AddWithValue("@cpf", u.cpf);
                        cmd.Parameters.AddWithValue("@telefone", u.telefone);
                        cmd.Parameters.AddWithValue("@email", u.email);
                        cmd.Parameters.AddWithValue("@rua", u.rua);
                        cmd.Parameters.AddWithValue("@nrua", u.nrua);
                        cmd.Parameters.AddWithValue("@bairro", u.bairro);
                        cmd.Parameters.AddWithValue("@cidade", u.cidade);
                        cmd.Parameters.AddWithValue("@estado", u.estado);
                        cmd.Parameters.AddWithValue("@cep", u.cep);
                        cmd.Parameters.AddWithValue("@ulogar", u.ulogar);
                        cmd.Parameters.AddWithValue("@senha", u.senha);
                        cmd.Parameters.AddWithValue("@tipousuario", u.tipousuario);
                        cmd.Parameters.AddWithValue("@situacao", u.situacao);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Usuário atualizado com sucesso.");

                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erro ao atualizar usuário: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }


        public static DataTable SELECTuser(int userId)
        {
            DataTable retornarID = new DataTable();

            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";

            using (var conectar = new SqlConnection(bdonlygreen))
            {
                conectar.Open();
                using (var procurar = new SqlCommand("SELECT * FROM tb_Usuario WHERE id = @id", conectar))
                {
                    procurar.Parameters.AddWithValue("@id", userId);

                    using (var resp = new SqlDataAdapter(procurar))
                    {
                        resp.Fill(retornarID); // Preenche o DataTable
                    }
                }
            }
            return retornarID;
        }

    }


}
