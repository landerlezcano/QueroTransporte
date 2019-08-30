﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModel;
using QueroTransporte.Model;
using QueroTransporte.Negocio;

namespace QueroTransporteWeb.Controllers
{
    public class ComprarCreditosController : Controller
    {

        private readonly GerenciadorComprarCredito _gerenciadorComprarCredito;
        private readonly GerenciadorUsuario _gerenciadorUsuario;
        private readonly GerenciadorTransacao _gerenciadorTransacao;


        public ComprarCreditosController(GerenciadorUsuario gerenciadorUsuario, 
                                         GerenciadorComprarCredito gerenciadorComprarCredito,
                                         GerenciadorTransacao gerenciadorTransacao)
        {
            _gerenciadorComprarCredito = gerenciadorComprarCredito;
            _gerenciadorUsuario = gerenciadorUsuario;
            _gerenciadorTransacao = gerenciadorTransacao;
        }

        /// <summary>
        /// Inicia a tela de comprar creditos
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.Creditos = addListaCreditos();
            return View();
        }

        /// <summary>
        /// Inicia a tela de comprar creditos e pega o objeto com dados da compra
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CreditoViagemModel cv)
        {
            bool deferido;
            ViewBag.Creditos = addListaCreditos();
            if (ModelState.IsValid)
            {
                if (_gerenciadorUsuario.ObterPorId(cv.IdUsuario) != null)
                {
                    if (_gerenciadorComprarCredito.Inserir(cv))
                    {
                        TempData["mensagemSucesso"] = "Compra realizada com sucesso!.";
                        deferido = true;   
                    }
                    else
                    {
                        TempData["mensagemErro"] = "Compra não pode ser realizada!.";
                        deferido = false;    
                    }

                    if (_gerenciadorTransacao.Inserir(addTransacao(cv, deferido)))
                           TempData["mensagemSucessoTransacao"] = "";
                    else
                           TempData["mensagemErroTransacao"] = "Houve um problema ao gravar a transacao";
                }
                else
                    TempData["mensagemErro"] = "Compra não pode ser finalizada pois não existe nenhum usuário logado na sessão!.";

                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        public TransacaoModel addTransacao(CreditoViagemModel cv,bool deferido)
        {
            TransacaoModel tm = new TransacaoModel();
            tm.Data = DateTime.Now;
            tm.Deferido = deferido;
            tm.IdUsuario = cv.IdUsuario;
            tm.QtdCreditos = (double) cv.Saldo;
            if (deferido)
                tm.Status = "Aprovado";
            else
                tm.Status = "Cancelada";

            return tm;
        }

        /// <summary>
        /// Metodo que adiciona valores de creditos para comprar(esses dados devem vir do banco)
        /// </summary>
        public List<CreditoViagemViewModel> addListaCreditos()
        {
            List<CreditoViagemViewModel> creditoViagem = new List<CreditoViagemViewModel>();

            creditoViagem.Add(new CreditoViagemViewModel(1, "5 Creditos Para Viagem", 5.00));
            creditoViagem.Add(new CreditoViagemViewModel(2, "10 Creditos Para Viagem", 10.00));
            creditoViagem.Add(new CreditoViagemViewModel(3, "15 Creditos Para Viagem", 15.00));

            return creditoViagem;
        }
    }
}