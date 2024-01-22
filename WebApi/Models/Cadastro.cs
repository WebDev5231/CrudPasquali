using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    [Dapper.Contrib.Extensions.Table("Cadastro")]
    public class Cadastro
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string NomeCompleto { get; set; }
        public DateTime DataNascimento { get; set; }
        public decimal ValorRenda { get; set; }
        public string CPF { get; set; }
    }
}

//SCRIPT PARA GERAR A TABELA
//NECESSARIO CRIAR UMA BASE CHAMADA CRUD

/*
USE [CRUD]
GO

Object:  Table [dbo].[Cadastro]    Script Date: 21/01/2024
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE[dbo].[Cadastro]
(
[ID][bigint] IDENTITY(1,1) NOT NULL,
[NomeCompleto] [nvarchar] (max) NOT NULL,
[DataNascimento]
[datetime]
NOT NULL,
[ValorRenda] [decimal] (18, 2) NOT NULL,
[CPF] [nvarchar] (15) NOT NULL,
PRIMARY KEY CLUSTERED
(
    [ID] ASC
) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
GO
}

*/
