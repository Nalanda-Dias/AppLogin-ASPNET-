using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppLoginAutenticacao.Models;
using AppLoginAutenticacao.ViewModels;
using AppLoginAutenticacao.Utils;
using System.Security.Claims;

namespace AppLoginAutenticacao.Controllers
{
    public class AutenticacaoController : Controller
    {
        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(CadastroUsuarioViewModel viewmodel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewmodel);
            }

            Usuario novousuario = new Usuario
            {
                UsuNome = viewmodel.UsuNome,
                Login = viewmodel.Login,
                Senha = Hash.GerarHash(viewmodel.Senha)
            };
            novousuario.Insert(novousuario);

            TempData["MensagemLogin"] = "Cadastro realizado com sucesso! Faça o Login.";
            return RedirectToAction("Login", "Autenticacao");
        }

        public ActionResult LoginBusca(string Login)
        {
            bool LoginExists;

            var usuario = new Usuario();
            string login = usuario.SelectLogin(Login);

            if (login.Length == 0)
                LoginExists = false;
            else
                LoginExists = true;

            return Json(!LoginExists, JsonRequestBehavior.AllowGet);
        }

  
        public  ActionResult Login(string ReturnUrl)
        {
            var viewmodel = new LoginViewModel
            {
                UrlRetorno = ReturnUrl
            };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewmodel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewmodel);
            }

            Usuario usuario = new Usuario();
            usuario = usuario.SelectUsuario(viewmodel.Login);

            if (usuario == null | usuario.Login != viewmodel.Login)
            {
                ModelState.AddModelError("Login", "Login incorreto");
                return View(viewmodel);
            }
            if(usuario.Senha != Hash.GerarHash(viewmodel.Senha))
            {
                ModelState.AddModelError("Senha", "Senha incorreto");
                return View(viewmodel);
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name,usuario.Login),
                new Claim("Login",usuario.Login)
            }, "AppAplicationCookie");

            Request.GetOwinContext().Authentication.SignIn(identity);

            if (!String.IsNullOrWhiteSpace(viewmodel.UrlRetorno) || Url.IsLocalUrl(viewmodel.UrlRetorno))
                return Redirect(viewmodel.UrlRetorno);
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut("AppAplicationCookie");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AlterarSenha()
        {
            
                return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AlterarSenha(AlterarSenhaViewModels viewmodel)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var identity = User.Identity as ClaimsIdentity;
            var login = identity.Claims.FirstOrDefault(c => c.Type == "Login").Value;

            Usuario usuario = new Usuario();
            usuario = usuario.SelectUsuario(login);

            if (Hash.GerarHash(viewmodel.NovaSenha) == usuario.Senha)
            {
                ModelState.AddModelError("SenhaAtual", "Senha incorreta");
                return View();
            }

            usuario.Senha = Hash.GerarHash(viewmodel.NovaSenha);

            usuario.UpdateSenha(usuario);

            TempData["MensagemLogin"] = "Senha alterada com sucesso!";

            return RedirectToAction("Index", "Home");
        }
    }
}