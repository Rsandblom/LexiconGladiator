using Gladiator.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Gladiator.ViewModels.PlayerGladiators
{
	public class EditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int HeadModifierId { get; set; }


        private List<SelectListItem> _headModifiers;
        public List<SelectListItem> Heads { get => _headModifiers; }

        public void CreateHeadSelectList(List<Modifiers> headModifiersList)
        {
            List<SelectListItem> headModifiersSelectList = new List<SelectListItem>();
            foreach (var head in headModifiersList)
            {
                headModifiersSelectList.Add(new SelectListItem { Value = head.Id.ToString(), Text = head.Name });
            }
            _headModifiers = headModifiersSelectList;
        }


        public int BodyModifierId { get; set; }

        private List<SelectListItem> _bodyModifiers;
        public List<SelectListItem> Bodies { get => _bodyModifiers; }

        public void CreateBodySelectList(List<Modifiers> bodyModifiersList)
        {
            List<SelectListItem> bodyModifiersSelectList = new List<SelectListItem>();
            foreach (var body in bodyModifiersList)
            {
                bodyModifiersSelectList.Add(new SelectListItem { Value = body.Id.ToString(), Text = body.Name });
            }
            _bodyModifiers = bodyModifiersSelectList;
        }

        public int RightHandModifierId { get; set; }

        private List<SelectListItem> _rightHandModifiers;
        public List<SelectListItem> RightHands { get => _rightHandModifiers; }

        public void CreateRightHandSelectList(List<Modifiers> rightHandModifiersList)
        {
            List<SelectListItem> rightHandModifiersSelectList = new List<SelectListItem>();
            foreach (var rightHand in rightHandModifiersList)
            {
                rightHandModifiersSelectList.Add(new SelectListItem { Value = rightHand.Id.ToString(), Text = rightHand.Name });
            }
            _rightHandModifiers = rightHandModifiersSelectList;
        }

        public int LeftHandModifierId { get; set; }

        private List<SelectListItem> _leftHandModifiers;
        public List<SelectListItem> LeftHands { get => _leftHandModifiers; }

        public void CreateLeftHandSelectList(List<Modifiers> leftHandModifiersList)
        {
            List<SelectListItem> leftHandModifiersSelectList = new List<SelectListItem>();
            foreach (var leftHand in leftHandModifiersList)
            {
                leftHandModifiersSelectList.Add(new SelectListItem { Value = leftHand.Id.ToString(), Text = leftHand.Name });
            }
            _leftHandModifiers = leftHandModifiersSelectList;
        }
    }
}
