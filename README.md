# Sistema de Controle de Treinos e Nutrição

Aplicação desktop desenvolvida em **C# (Windows Forms)** integrada a um banco de dados **MySQL** via **XAMPP**, criada para gerenciar rotinas de treino (exercícios, grupos musculares, séries/repetições e carga em kg) e acompanhamento nutricional (refeições, categorias, macronutrientes e calorias em kcal) com operações completas de **CRUD** (Cadastrar, Buscar, Alterar e Remover).

---

## Tecnologias Utilizadas

* **Linguagem:** C# (.NET Framework / Windows Forms)
* **IDE:** Microsoft Visual Studio
* **Banco de Dados:** MySQL (via XAMPP / phpMyAdmin)
* **Conector MySQL:** `MySql.Data` (NuGet)
* **Versionamento:** Git e GitHub

---

## Funcionalidades

* **Cadastrar:** Registro de novos exercícios ou refeições com seleção ou adição dinâmica de categorias/grupos.
* **Buscar:** Pesquisa em tempo real por nome, categoria, módulo (`Treino` ou `Nutricao`) ou detalhes.
* **Alterar:** Edição completa de qualquer registro selecionado na grade.
* **Remover:** Exclusão segura com caixa de confirmação.

---

## Como Executar o Projeto

1. **Banco de Dados (XAMPP):**
   * Inicie os serviços **Apache** e **MySQL** no painel do XAMPP.
   * Acesse `http://localhost/phpmyadmin`.
   * Importe ou execute o script **`DumpBancoTreinoNutricao.sql`** disponível na raiz deste repositório para criar o banco `db_treino_nutricao` e suas tabelas.

2. **Aplicação C# (Visual Studio):**
   * Clone este repositório:
     ```bash
     git clone [https://github.com/Pablo-Chagas/SistemaTreinoNutricao.git](https://github.com/Pablo-Chagas/SistemaTreinoNutricao.git)
     ```
   * Abra o arquivo **`SistemaCadastro.sln`** no Visual Studio.
   * Restaure os pacotes NuGet (`MySql.Data`), se necessário.
   * Pressione **`F5`** (ou clique em **Iniciar**) para compilar e executar o sistema.

---

## Autor

* **Pablo Ruan Oliveira Chagas** — [GitHub](https://github.com/Pablo-Chagas)
