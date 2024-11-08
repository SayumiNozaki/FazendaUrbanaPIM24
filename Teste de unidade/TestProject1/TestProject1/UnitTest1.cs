namespace TestProject1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Data.SqlClient;
using System;

[TestClass]
public class ClassSupervisorTests
{
    [TestMethod]
    public void Test_InsertUsuario_Success()
    {
        // Arrange: Criando um mock do SqlConnection e SqlCommand
        var mockConnection = new Mock<SqlConnection>("Server=FEUERWOLF;Database=bdonlygreen;Integrated Security=True;");
        var mockCommand = new Mock<SqlCommand>();

        // Configurando o comportamento do mock para o ExecuteNonQuery
        mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1); // Simula uma execução bem-sucedida

        // Criando o objeto de usuário para o teste
        var usuario = new classUsuario
        {
            nome = "João",
            cpf = "12345678901",
            telefone = "9876543210",
            email = "joao@exemplo.com",
            rua = "Rua Teste",
            nrua = "100",
            bairro = "Centro",
            cidade = "Cidade Teste",
            estado = "Estado Teste",
            cep = "12345000",
            ulogar = "joao123",
            senha = "senha123",
            tipousuario = "Admin",
            situacao = "Ativo"
        };

        // Instanciando o objeto da classe supervisor
        var supervisor = new classSupervisor();

        // Act: Executando o método de inserção
        supervisor.INSERTusuario(usuario);

        // Assert: Verificando se o comando ExecuteNonQuery foi chamado
        mockCommand.Verify(x => x.ExecuteNonQuery(), Times.Once());
    }

    [TestMethod]
    public void Test_InsertUsuario_DuplicateCpf_ThrowsError()
    {
        // Arrange
        var usuario = new classUsuario { nome = "João", cpf = "12345678901" };

        // Criando o mock do SqlException para simular um erro de duplicidade (erro 2627)
        var mockException = new Mock<SqlException>();
        mockException.Setup(x => x.Number).Returns(2627); // Simula erro de duplicidade de CPF

        var supervisor = new classSupervisor();

        // Act & Assert: Verificando se a exceção é tratada corretamente
        try
        {
            supervisor.INSERTusuario(usuario);
            Assert.Fail("Exceção não foi lançada.");
        }
        catch (SqlException ex)
        {
            Assert.AreEqual(2627, ex.Number); // Verifica se o erro é de duplicidade
        }
    }
}
