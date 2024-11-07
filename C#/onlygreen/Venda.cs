﻿using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace onlygreen
{
    public partial class Venda : Form
    {
        public Venda()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Venda_Load(object sender, EventArgs e)
        {
            // TODO: esta linha de código carrega dados na tabela 'bdonlygreenDataSet16.tb_Venda'. Você pode movê-la ou removê-la conforme necessário.
            this.tb_VendaTableAdapter1.Fill(this.bdonlygreenDataSet16.tb_Venda);
           
        }
        ////////////////////////////////IGNORAR
   

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtRegistro_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        ////////////////////////////////IGNORAR


        //PESQUISAR
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisar.Text))
            {
                MessageBox.Show("Por favor: insira um termo de pesquisa.");
                txtPesquisar.Focus();
                return;
            }

            string pesquisar = txtPesquisar.Text;
            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
            using (SqlConnection conectar = new SqlConnection(bdonlygreen))
            {

                conectar.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM tb_Venda WHERE nome LIKE @pesquisar OR " +
                    "nome LIKE @pesquisar OR " +
                    "id LIKE @pesquisar", conectar))
                {
                    cmd.Parameters.AddWithValue("@pesquisar", "%" + pesquisar + "%");
                    DataTable dt = new DataTable();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        tbVenda.DataSource = dt;
                    }
                }
            }
        }

        //VOLTAR HOME
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("Você tem certeza que deseja sair da gestão de venda?", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (resultado == DialogResult.OK)
            {
                var menu = new Home();
                menu.Show(this);

                this.Visible = false;
            }
        }


  
        // ADICIONAR NOVA VENDA
        private void btnSelecionarestoque_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdestoque.Text) || !int.TryParse(txtIdestoque.Text, out int estoqueId))
            {
                MessageBox.Show("Por favor, insira um ID de estoque válido. \nConferir na tabela de estoque os produtos disponíveis.");
                txtIdestoque.Focus();
                return;
            }

            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
            using (SqlConnection conectar = new SqlConnection(bdonlygreen))
            {
                conectar.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tb_Estoque WHERE id = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@id", estoqueId);

                    DataTable dt = new DataTable();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtIdestoque.Text = dt.Rows[0]["id"].ToString();
                            txtNome.Text = dt.Rows[0]["nome"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Nenhum registro encontrado com esse ID. \nPor favor, verificar a tabela de estoque.");
                  
                        }
                    }
                }
            }
        }


        private bool ValidarCampo()
        {
            if (string.IsNullOrWhiteSpace(txtIdestoque.Text) || !int.TryParse(txtIdestoque.Text, out int estoqueId))
            {
                MessageBox.Show("Por favor, insira um ID de estoque válido.");
                txtIdestoque.Focus();
                return true;
            }

            if (string.IsNullOrWhiteSpace(txtIdcliente.Text) || !int.TryParse(txtIdcliente.Text, out int clienteId))
            {
                MessageBox.Show("Por favor, insira um ID de cliente válido.");
                txtIdcliente.Focus();
                return true;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidadeVenda) || quantidadeVenda <= 0)
            {
                MessageBox.Show("Por favor, insira uma quantidade válida.");
                txtQuantidade.Focus();
                return true;
            }

            if (string.IsNullOrWhiteSpace(txtValor.Text))
            {
                MessageBox.Show("O campo valor é obrigatório.");
                txtValor.Focus();
                return true;
            }

            return false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidarCampo())
            {
                string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";

                using (var conectar = new SqlConnection(bdonlygreen))
                {
                    conectar.Open();

                    
                    int clienteId = int.Parse(txtIdcliente.Text);
                    int estoqueId = int.Parse(txtIdestoque.Text);
                    int quantidadeVenda = int.Parse(txtQuantidade.Text);

                    
                    using (var cmdCliente = new SqlCommand("SELECT COUNT(*) FROM tb_Cliente WHERE id = @id", conectar))
                    {
                        cmdCliente.Parameters.AddWithValue("@id", clienteId);
                        int clienteCount = (int)cmdCliente.ExecuteScalar();
                        

                        if (clienteCount == 0)
                        {
                            MessageBox.Show("Cliente não encontrado. Por favor, verifique o ID do cliente.");
                            txtIdcliente.Focus();
                            return;
                        }
                    }

                    // Verificar se o cliente existe e se está ativo
                    using (var cmdCliente = new SqlCommand("SELECT situacao FROM tb_Cliente WHERE id = @id", conectar))
                    {
                        cmdCliente.Parameters.AddWithValue("@id", clienteId);

                        var situacaoCliente = cmdCliente.ExecuteScalar() as string;

                        if (situacaoCliente == null)
                        {
                            MessageBox.Show("Cliente não encontrado. Por favor, verifique o ID do cliente.");
                            txtIdcliente.Focus();
                            return;
                        }
                        else if (situacaoCliente == "Inativo")
                        {
                            MessageBox.Show("Cliente está inativo e não pode realizar novas compras.");
                            txtIdcliente.Focus();
                            return;
                        }
                    }


                    // Verificar a quantidade disponível no estoque
                    using (var cmdEstoque = new SqlCommand("SELECT quantidade FROM tb_Estoque WHERE id = @id", conectar))
                    {
                        cmdEstoque.Parameters.AddWithValue("@id", estoqueId);
                        int estoqueQuantidade = (int)cmdEstoque.ExecuteScalar();

                        if (estoqueQuantidade < quantidadeVenda)
                        {
                            MessageBox.Show("Quantidade insuficiente em estoque.");
                            txtQuantidade.Focus();  
                            return;
                        }

                        // Registrar a venda
                        using (var troca = conectar.BeginTransaction())
                        {
                            try
                            {
                                // Insere na tabela de vendas
                                using (var addvenda = new SqlCommand("INSERT INTO tb_Venda (nome, quantidade, dregistro, pk_idCliente, pk_idEstoque, valor) " +
                                    "VALUES (@nome, @quantidade, GETDATE(), @idcliente, @idestoque, @valor)", conectar, troca))
                                {
                                    addvenda.Parameters.AddWithValue("@nome", txtNome.Text);
                                    addvenda.Parameters.AddWithValue("@idcliente", clienteId);
                                    addvenda.Parameters.AddWithValue("@idestoque", estoqueId);
                                    addvenda.Parameters.AddWithValue("@quantidade", quantidadeVenda);
                                    addvenda.Parameters.AddWithValue("@valor", Convert.ToDouble(txtValor.Text));

                                    addvenda.ExecuteNonQuery();
                                }

                                // Atualiza a quantidade no estoque
                                using (var attquantidade = new SqlCommand("UPDATE tb_Estoque SET quantidade = quantidade - @quantidade WHERE id = @id", conectar, troca))
                                {
                                    attquantidade.Parameters.AddWithValue("@quantidade", quantidadeVenda);
                                    attquantidade.Parameters.AddWithValue("@id", estoqueId);
                                    attquantidade.ExecuteNonQuery();
                                }

                                using (var verificaQuantidade = new SqlCommand("SELECT quantidade FROM tb_Estoque WHERE id = @id", conectar, troca))
                                {
                                    verificaQuantidade.Parameters.AddWithValue("@id", estoqueId);
                                    var novaQuantidade = (int)verificaQuantidade.ExecuteScalar();

                                    if (novaQuantidade <= 0)
                                    {
                                        using (var attSituacao = new SqlCommand("UPDATE tb_Estoque SET situacao = 'Esgotado' WHERE id = @id", conectar, troca))
                                        {
                                            attSituacao.Parameters.AddWithValue("@id", estoqueId);
                                            attSituacao.ExecuteNonQuery();
                                        }
                                    }
                                }

                                // Commit da transação
                                troca.Commit();
                                MessageBox.Show("Venda registrada com sucesso!");

                                var menu = new Venda();
                                menu.Show(this);
                                this.Visible = false;
                            }
                            catch (Exception ex)
                            {
                                troca.Rollback(); // Reverte a transação em caso de erro
                                MessageBox.Show("Erro ao registrar nova quantidade na tabela estoque: " + ex.Message);
                                txtIdestoque.Focus();
                            }
                        }
                    }
                }
            }
        }




        private void Limpar()
        {
            txtId.Clear();
            txtIdestoque.Clear();
            txtIdcliente.Clear();
            txtNome.Clear();
            txtQuantidade.Clear();
            txtRegistro.Clear();
            txtValor.Clear();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("Você tem certeza que deseja limpar todos os campos de texto?", "Aviso", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (resultado == DialogResult.OK)
            {
                Limpar();
            }
        }



//VISUALIZAR/ALTERAR/DELETAR

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int estoqueId))
            {
                MessageBox.Show("Por favor, insira um ID de venda válido.");
                txtId.Focus();
                return;
            }

            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
            using (SqlConnection conectar = new SqlConnection(bdonlygreen))
            {
                conectar.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM tb_Venda WHERE id = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@id", estoqueId);

                    DataTable dt = new DataTable();

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            txtIdestoque.Text = dt.Rows[0]["pk_idEstoque"].ToString();
                            txtIdcliente.Text = dt.Rows[0]["pk_idCliente"].ToString();
                            txtNome.Text = dt.Rows[0]["nome"].ToString();
                            txtQuantidade.Text = dt.Rows[0].Field<int>("quantidade").ToString();
                            txtValor.Text = dt.Rows[0].Field<decimal>("valor").ToString();

                            DateTime dregistro = dt.Rows[0].Field<DateTime>("dregistro");
                            txtRegistro.Text = dregistro.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            MessageBox.Show("Nenhum registro encontrado com esse ID. \nPor favor, verificar a tabela de venda.");
                        }
                    }
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
             
            if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int vendaId))
            {
                MessageBox.Show("Por favor, insira um ID de venda válido.");
                txtId.Focus();
                return;
            }

            if (!int.TryParse(txtQuantidade.Text, out int novaQuantidade) || !int.TryParse(txtIdestoque.Text, out int estoqueId))
            {
                MessageBox.Show("Por favor, insira uma quantidade válida.");
                return;
            }

            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
            using (SqlConnection conectar = new SqlConnection(bdonlygreen))
            {
                conectar.Open();
                
                int clienteId = int.Parse(txtIdcliente.Text);

                // Verificar se o cliente existe
                using (var cmdCliente = new SqlCommand("SELECT COUNT(*) FROM tb_Cliente WHERE id = @id", conectar))
                {
                    cmdCliente.Parameters.AddWithValue("@id", clienteId);
                    int clienteid = (int)cmdCliente.ExecuteScalar();

                    if (clienteid == 0)
                    {
                        MessageBox.Show("Cliente não encontrado. Por favor, verifique o ID do cliente.");
                        return;
                    }
                }

                // Verificar se o cliente existe e se está ativo
                using (var cmdCliente = new SqlCommand("SELECT situacao FROM tb_Cliente WHERE id = @id", conectar))
                {
                    cmdCliente.Parameters.AddWithValue("@id", clienteId);

                    var situacaoCliente = cmdCliente.ExecuteScalar() as string;

                    if (situacaoCliente == null)
                    {
                        MessageBox.Show("Cliente não encontrado. Por favor, verifique o ID do cliente.");
                        txtIdcliente.Focus();
                        return;
                    }
                    else if (situacaoCliente == "Inativo")
                    {
                        MessageBox.Show("Cliente está inativo e não pode realizar novas compras.");
                        txtIdcliente.Focus();
                        return;
                    }
                }

                // Seleciona a venda para obter a quantidade antiga
                int quantidadeAntiga = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT quantidade FROM tb_Venda WHERE id = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@id", vendaId);
                    quantidadeAntiga = (int)cmd.ExecuteScalar();
                }

                using (var troca = conectar.BeginTransaction())
                {
                    try
                    {
                        // Atualiza a venda
                        using (var cmd = new SqlCommand("UPDATE tb_Venda SET quantidade = @quantidade, valor = @valor WHERE id = @id", conectar, troca))
                        {
                            cmd.Parameters.AddWithValue("@quantidade", novaQuantidade);
                            cmd.Parameters.AddWithValue("@valor", Convert.ToDouble(txtValor.Text));
                            cmd.Parameters.AddWithValue("@id", vendaId);
                            cmd.ExecuteNonQuery();
                        }

                        // Atualiza a quantidade no estoque
                        using (var cmd = new SqlCommand("UPDATE tb_Estoque SET quantidade = quantidade + @quantidadeAntiga - @novaQuantidade WHERE id = @id", conectar, troca))
                        {
                            cmd.Parameters.AddWithValue("@quantidadeAntiga", quantidadeAntiga);
                            cmd.Parameters.AddWithValue("@novaQuantidade", novaQuantidade);
                            cmd.Parameters.AddWithValue("@id", estoqueId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var verificaQuantidade = new SqlCommand("SELECT quantidade FROM tb_Estoque WHERE id = @id", conectar, troca))
                        {
                            verificaQuantidade.Parameters.AddWithValue("@id", estoqueId);
                            var qntd = (int)verificaQuantidade.ExecuteScalar();

                            if (qntd <= 0)
                            {
                                using (var attSituacao = new SqlCommand("UPDATE tb_Estoque SET situacao = 'Esgotado' WHERE id = @id", conectar, troca))
                                {
                                    attSituacao.Parameters.AddWithValue("@id", estoqueId);
                                    attSituacao.ExecuteNonQuery();
                                }
                            }
                        }

                        troca.Commit();
                        MessageBox.Show("Venda alterada com sucesso!");

                        var menu = new Venda();
                        menu.Show(this);
                        this.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        troca.Rollback();
                        MessageBox.Show("Erro ao atualizar a venda: " + ex.Message);
                    }
                }
            }
        }


        private void btnDel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || !int.TryParse(txtId.Text, out int vendaId))
            {
                MessageBox.Show("Por favor, insira um ID de venda válido.");
                txtId.Focus();
                return;
            }

            string bdonlygreen = "Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;";
            using (SqlConnection conectar = new SqlConnection(bdonlygreen))
            {
                conectar.Open();

                int quantidadeVenda = 0;
                int estoqueId = 0;

                // Obtenha a quantidade da venda e o ID do estoque
                using (SqlCommand cmd = new SqlCommand("SELECT quantidade, pk_idEstoque FROM tb_Venda WHERE id = @id", conectar))
                {
                    cmd.Parameters.AddWithValue("@id", vendaId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            quantidadeVenda = reader.GetInt32(0); // quantidade
                            estoqueId = reader.GetInt32(1); // pk_idEstoque
                        }
                        else
                        {
                            MessageBox.Show("Venda não encontrada.");
                            return;
                        }
                    }
                }

                using (var troca = conectar.BeginTransaction())
                {
                    try
                    {
                        // Deletar a venda
                        using (var cmd = new SqlCommand("DELETE FROM tb_Venda WHERE id = @id", conectar, troca))
                        {
                            cmd.Parameters.AddWithValue("@id", vendaId);
                            cmd.ExecuteNonQuery();
                        }

                        // Atualiza a quantidade no estoque
                        using (var cmd = new SqlCommand("UPDATE tb_Estoque SET quantidade = quantidade + @quantidade WHERE id = @id", conectar, troca))
                        {
                            cmd.Parameters.AddWithValue("@quantidade", quantidadeVenda);
                            cmd.Parameters.AddWithValue("@id", estoqueId);
                            cmd.ExecuteNonQuery();
                        }

                        troca.Commit();
                        MessageBox.Show("Venda deletada com sucesso!");
                       
                        var menu = new Venda();
                        menu.Show(this);
                        this.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        troca.Rollback();
                        MessageBox.Show("Erro ao deletar a venda: " + ex.Message);
                    }
                }
            }
        }

        private void txtPesquisar_Enter(object sender, EventArgs e)
        {
            txtPesquisar.BackColor = Color.LightBlue;
        }

        private void txtPesquisar_Leave(object sender, EventArgs e)
        {
            txtPesquisar.BackColor = Color.White;
        }

        private void btnBuscar_Enter(object sender, EventArgs e)
        {
            btnBuscar.BackColor = Color.LightGreen;
        }

        private void btnBuscar_Leave(object sender, EventArgs e)
        {
            btnBuscar.BackColor = Color.Silver;
        }

        private void txtIdestoque_Enter(object sender, EventArgs e)
        {
            txtIdestoque.BackColor = Color.LightBlue;
        }

        private void txtIdestoque_Leave(object sender, EventArgs e)
        {
            txtIdestoque.BackColor = Color.White;
        }

        private void btnSelecionarestoque_Enter(object sender, EventArgs e)
        {
            btnSelecionarestoque.BackColor = Color.LightGreen;
        }

        private void btnSelecionarestoque_Leave(object sender, EventArgs e)
        {
            btnSelecionarestoque.BackColor = Color.Silver;
        }

        private void btnAdd_Enter(object sender, EventArgs e)
        {
            btnAdd.BackColor = Color.LightGreen;
        }

        private void btnAdd_Leave(object sender, EventArgs e)
        {
            btnAdd.BackColor = Color.Silver;
        }

        private void btnLimpar_Enter(object sender, EventArgs e)
        {
            btnLimpar.BackColor = Color.LightGreen;
        }

        private void btnLimpar_Leave(object sender, EventArgs e)
        {
            btnLimpar.BackColor = Color.Silver;
        }

        private void txtId_Enter(object sender, EventArgs e)
        {
            txtId.BackColor = Color.LightBlue;
        }

        private void txtId_Leave(object sender, EventArgs e)
        {
            txtId.BackColor = Color.White;
        }

        private void btnSelecionar_Enter(object sender, EventArgs e)
        {
            btnSelecionar.BackColor = Color.LightGreen;
        }

        private void btnSelecionar_Leave(object sender, EventArgs e)
        {
            btnSelecionar.BackColor = Color.Silver;
        }

        private void btnSalvar_Enter(object sender, EventArgs e)
        {
            btnSalvar.BackColor = Color.Orange;
        }

        private void btnSalvar_Leave(object sender, EventArgs e)
        {
            btnSalvar.BackColor = Color.Silver;
        }

        private void btnDel_Enter(object sender, EventArgs e)
        {
            btnDel.BackColor = Color.Red;
        }

        private void btnDel_Leave(object sender, EventArgs e)
        {
            btnDel.BackColor = Color.Silver;
        }

        private void txtIdcliente_Enter(object sender, EventArgs e)
        {
            txtIdcliente.BackColor = Color.LightBlue;
        }

        private void txtIdcliente_Leave(object sender, EventArgs e)
        {
            txtIdcliente.BackColor = Color.White;
        }

        private void txtQuantidade_Enter(object sender, EventArgs e)
        {
            txtQuantidade.BackColor = Color.LightBlue;
        }

        private void txtQuantidade_Leave(object sender, EventArgs e)
        {
            txtQuantidade.BackColor = Color.White;
        }

        private void txtValor_Enter(object sender, EventArgs e)
        {
            txtValor.BackColor = Color.LightBlue;
        }

        private void txtValor_Leave(object sender, EventArgs e)
        {
            txtValor.BackColor = Color.White;
        }

        private void btnVoltar_Enter(object sender, EventArgs e)
        {
            btnVoltar.BackColor = Color.Red;
        }

        private void btnVoltar_Leave(object sender, EventArgs e)
        {
            btnVoltar.BackColor = Color.Silver;
        }
    }
}
