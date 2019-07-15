
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueroTransporte.Modelo
{
    public class SolicitacaoVeiculo
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdViagem { get; set; }
        public string DataSolicitacao { get; set; }
        public bool IsAtendida { get; set; }
    }
}