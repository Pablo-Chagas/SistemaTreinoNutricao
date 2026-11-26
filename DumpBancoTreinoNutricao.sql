CREATE DATABASE IF NOT EXISTS `db_treino_nutricao`
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_general_ci;

USE `db_treino_nutricao`;

CREATE TABLE IF NOT EXISTS `categorias` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `tipo` VARCHAR(30) NOT NULL,
  `nome` VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `registros` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `modulo` VARCHAR(30) NOT NULL,
  `titulo` VARCHAR(120) NOT NULL,
  `categoria` VARCHAR(100) NOT NULL,
  `detalhes` VARCHAR(150) NULL,
  `valor_referencia` DECIMAL(10,2) DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO `categorias` (`tipo`, `nome`) VALUES
('Treino', 'Peito e Tríceps'),
('Treino', 'Costas e Bíceps'),
('Treino', 'Pernas e Ombros'),
('Nutricao', 'Refeição Principal'),
('Nutricao', 'Pré-Treino / Pós-Treino');

INSERT INTO `registros` (`modulo`, `titulo`, `categoria`, `detalhes`, `valor_referencia`) VALUES
('Treino', 'Supino Reto com Barra', 'Peito e Tríceps', '4x10 repetições', 60.00),
('Treino', 'Rosca Scott na Polia', 'Costas e Bíceps', '3x12 repetições', 25.00),
('Treino', 'Agachamento Livre', 'Pernas e Ombros', '4x8 repetições', 80.00),
('Nutricao', 'Arroz, Feijão e Frango Grelhado', 'Refeição Principal', '150g arroz, 100g feijão, 150g frango', 540.00),
('Nutricao', 'Whey Protein com Banana', 'Pré-Treino / Pós-Treino', '30g whey + 1 banana prata', 210.00);
