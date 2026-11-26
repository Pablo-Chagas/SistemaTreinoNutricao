using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SistemaCadastro
{
	public partial class Sistema : Form
	{
		private readonly string conexaoString =
			"Server=localhost;Port=3306;Database=db_treino_nutricao;Uid=root;Pwd=;";

		private int idSelecionado = 0;

		public Sistema()
		{
			InitializeComponent();
		}

		private MySqlConnection ObterConexao()
		{
			return new MySqlConnection(conexaoString);
		}

		private void Sistema_Load(object sender, EventArgs e)
		{
			CarregarCategorias();
			ListarRegistros("");
		}

		private void CarregarCategorias()
		{
			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = "SELECT nome FROM categorias ORDER BY nome ASC;";
					using (var cmd = new MySqlCommand(sql, conn))
					using (var reader = cmd.ExecuteReader())
					{
						cbGenero.Items.Clear();
						cbAlteraGenero.Items.Clear();

						while (reader.Read())
						{
							string nomeCategoria = reader.GetString("nome");
							cbGenero.Items.Add(nomeCategoria);
							cbAlteraGenero.Items.Add(nomeCategoria);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao carregar categorias: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ListarRegistros(string filtro)
		{
			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = @"
                        SELECT 
                            id AS ID,
                            modulo AS Modulo,
                            titulo AS 'Exercicio / Refeicao',
                            categoria AS Categoria,
                            detalhes AS 'Series / Macros',
                            valor_referencia AS 'Carga (kg) / Kcal'
                        FROM registros
                        WHERE titulo LIKE @filtro 
                           OR categoria LIKE @filtro 
                           OR modulo LIKE @filtro
                           OR detalhes LIKE @filtro
                        ORDER BY id DESC;";

					using (var cmd = new MySqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
						using (var adapter = new MySqlDataAdapter(cmd))
						{
							DataTable dt = new DataTable();
							adapter.Fill(dt);
							dgBandas.DataSource = dt;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao listar registros: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private string DefinirModulo(string categoria)
		{
			string c = (categoria ?? "").ToLower();
			if (c.Contains("refei") || c.Contains("nutri") || c.Contains("pré") || c.Contains("pós") || c.Contains("macro"))
			{
				return "Nutricao";
			}
			return "Treino";
		}

		private decimal ConverterDecimal(string texto)
		{
			if (string.IsNullOrWhiteSpace(texto)) return 0m;
			string normalizado = texto.Trim().Replace(",", ".");
			if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
			{
				return valor;
			}
			return 0m;
		}

		private void bntAddGenero_Click(object sender, EventArgs e)
		{
			string novaCategoria = cbGenero.Text.Trim();
			if (string.IsNullOrWhiteSpace(novaCategoria))
			{
				MessageBox.Show("Digite ou selecione uma categoria/grupo antes de adicionar.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = "INSERT IGNORE INTO categorias (tipo, nome) VALUES (@tipo, @nome);";
					using (var cmd = new MySqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@tipo", DefinirModulo(novaCategoria));
						cmd.Parameters.AddWithValue("@nome", novaCategoria);
						cmd.ExecuteNonQuery();
					}
				}

				CarregarCategorias();
				cbGenero.Text = novaCategoria;
				MessageBox.Show("Categoria adicionada com sucesso!",
					"Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao adicionar categoria: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void BtnConfirmaCadastro_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtnome.Text) || string.IsNullOrWhiteSpace(cbGenero.Text))
			{
				MessageBox.Show("Preencha pelo menos o nome do exercício/refeição e a categoria.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string categoria = cbGenero.Text.Trim();
			string modulo = DefinirModulo(categoria);
			decimal valorRef = ConverterDecimal(txtranking.Text);

			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = @"
                        INSERT INTO registros (modulo, titulo, categoria, detalhes, valor_referencia)
                        VALUES (@modulo, @titulo, @categoria, @detalhes, @valor);";

					using (var cmd = new MySqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@modulo", modulo);
						cmd.Parameters.AddWithValue("@titulo", txtnome.Text.Trim());
						cmd.Parameters.AddWithValue("@categoria", categoria);
						cmd.Parameters.AddWithValue("@detalhes", txtintegrantes.Text.Trim());
						cmd.Parameters.AddWithValue("@valor", valorRef);
						cmd.ExecuteNonQuery();
					}
				}

				MessageBox.Show("Registro salvo com sucesso!",
					"Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LimparCamposCadastro();
				ListarRegistros(txtBusca.Text.Trim());
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao cadastrar: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtBusca_TextChanged(object sender, EventArgs e)
		{
			ListarRegistros(txtBusca.Text.Trim());
		}

		private void btnAlterar_Click(object sender, EventArgs e)
		{
			if (dgBandas.CurrentRow == null)
			{
				MessageBox.Show("Selecione um registro na tabela para alterar.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var linha = dgBandas.CurrentRow;
			idSelecionado = Convert.ToInt32(linha.Cells["ID"].Value);
			txtAlteraNome.Text = linha.Cells["Exercicio / Refeicao"].Value?.ToString();
			cbAlteraGenero.Text = linha.Cells["Categoria"].Value?.ToString();
			txtAlteraIntegrantes.Text = linha.Cells["Series / Macros"].Value?.ToString();
			txtAlteraRanking.Text = linha.Cells["Carga (kg) / Kcal"].Value?.ToString();

			tabControl1.SelectedTab = tabAlterar;
		}

		private void btnConfirmaAlteracao_Click(object sender, EventArgs e)
		{
			if (idSelecionado == 0)
			{
				MessageBox.Show("Nenhum registro selecionado para alteração. Vá em Buscar e clique em Alterar.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(txtAlteraNome.Text) || string.IsNullOrWhiteSpace(cbAlteraGenero.Text))
			{
				MessageBox.Show("Preencha o nome e a categoria antes de confirmar.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string categoria = cbAlteraGenero.Text.Trim();
			string modulo = DefinirModulo(categoria);
			decimal valorRef = ConverterDecimal(txtAlteraRanking.Text);

			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = @"
                        UPDATE registros
                        SET modulo = @modulo,
                            titulo = @titulo,
                            categoria = @categoria,
                            detalhes = @detalhes,
                            valor_referencia = @valor
                        WHERE id = @id;";

					using (var cmd = new MySqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@modulo", modulo);
						cmd.Parameters.AddWithValue("@titulo", txtAlteraNome.Text.Trim());
						cmd.Parameters.AddWithValue("@categoria", categoria);
						cmd.Parameters.AddWithValue("@detalhes", txtAlteraIntegrantes.Text.Trim());
						cmd.Parameters.AddWithValue("@valor", valorRef);
						cmd.Parameters.AddWithValue("@id", idSelecionado);
						cmd.ExecuteNonQuery();
					}
				}

				MessageBox.Show("Registro alterado com sucesso!",
					"Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LimparCamposAlterar();
				ListarRegistros(txtBusca.Text.Trim());
				tabControl1.SelectedTab = tabBuscar;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao alterar registro: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnRemoveBanda_Click(object sender, EventArgs e)
		{
			if (dgBandas.CurrentRow == null)
			{
				MessageBox.Show("Selecione um registro na tabela para remover.",
					"Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int id = Convert.ToInt32(dgBandas.CurrentRow.Cells["ID"].Value);
			var confirmacao = MessageBox.Show("Tem certeza de que deseja remover este registro?",
				"Confirmar remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (confirmacao != DialogResult.Yes) return;

			try
			{
				using (var conn = ObterConexao())
				{
					conn.Open();
					string sql = "DELETE FROM registros WHERE id = @id;";
					using (var cmd = new MySqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@id", id);
						cmd.ExecuteNonQuery();
					}
				}

				MessageBox.Show("Registro removido com sucesso!",
					"Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
				ListarRegistros(txtBusca.Text.Trim());
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao remover registro: " + ex.Message,
					"Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void LimparCamposCadastro()
		{
			txtnome.Clear();
			cbGenero.SelectedIndex = -1;
			cbGenero.Text = string.Empty;
			txtintegrantes.Clear();
			txtranking.Clear();
		}

		private void LimparCamposAlterar()
		{
			idSelecionado = 0;
			txtAlteraNome.Clear();
			cbAlteraGenero.SelectedIndex = -1;
			cbAlteraGenero.Text = string.Empty;
			txtAlteraIntegrantes.Clear();
			txtAlteraRanking.Clear();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void btnCadastra_Click(object sender, EventArgs e)
		{
			marcador.Height = btnCadastra.Height;
			marcador.Top = btnCadastra.Top;
			tabControl1.SelectedTab = tabCadastrar;
		}

		private void btnBusca_Click(object sender, EventArgs e)
		{
			marcador.Height = btnBusca.Height;
			marcador.Top = btnBusca.Top;
			tabControl1.SelectedTab = tabBuscar;
			ListarRegistros(txtBusca.Text.Trim());
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void panel2_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}