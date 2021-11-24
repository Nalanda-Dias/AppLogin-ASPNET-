using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace AppLoginAutenticacao.ViewModels
{
    public class AlterarSenhaViewModels
    {
        [Display(Name = "Senha atual")]
        [Required(ErrorMessage = "Informe a senha atual")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string SenhaAtual { set; get; }

        [Display(Name = "Nova senha")]
        [Required(ErrorMessage = "Informe a nova senha ")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        [DataType(DataType.Password)]
        public string NovaSenha { set; get; }

        [Display(Name = "Confirmar Senha")]
        [Required(ErrorMessage = "Confirme a senha")]
        [MinLength(6, ErrorMessage = "As senhas são diferentes")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { set; get; }
    }
}