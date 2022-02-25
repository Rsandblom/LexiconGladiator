using Gladiator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Gladiator.ViewModels.AdminFighters
{
	public class AddModifiersViewModel
    {
        public int Id { get; set; }
        public List<Modifiers> Modifiers { get; set; }

        public int ModifiersId { get; set; }

        private List<SelectListItem> _modifiers;
        public List<SelectListItem> ModifiersSelectList { get => _modifiers; }

        public void CreateModifiersSelectList(List<Modifiers> modifiersList)
        {
            List<SelectListItem> modifiersSelectList = new List<SelectListItem>();
            foreach (var mod in modifiersList)
            {
                modifiersSelectList.Add(new SelectListItem { Value = mod.Id.ToString(), Text = mod.Name });
            }
            _modifiers = modifiersSelectList;
        }

    }
}
