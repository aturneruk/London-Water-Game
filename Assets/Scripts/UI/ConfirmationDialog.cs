using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {

    public enum DialogResult {
        None, Yes, No
    }

    public enum DialogType {
        EmptyCell, PopulatedCell, Upgrade
    }

    public class ConfirmationDialog : MonoBehaviour {

        public DialogResult result;

        [SerializeField]
        CanvasGroup confirmationDialog;
        [SerializeField]
        Text textField;

        public void ShowConfirmationDialog(DialogType dialogType, double capitalCost) {
            result = DialogResult.None;

            switch (dialogType) {
                case DialogType.EmptyCell:
                    textField.text = "Construct a reservoir in an empty field.\nThis will cost " + Money.FormatMoney(capitalCost) + ".\nDo you wish to continue?";
                    break;
                case DialogType.PopulatedCell:
                    throw new System.ArgumentException("A population must be supplied where the dialog type is PopulatedCell");
                case DialogType.Upgrade:
                    textField.text = "Upgrade the existing reservoir to increase storage and supply capacity.\nThis will cost " + Money.FormatMoney(capitalCost) + ".\nDo you wish to continue?";
                    break;
                default:
                    throw new System.ArgumentException("Confirmation dialog type is null");
            }

            confirmationDialog.alpha = 1f;
            confirmationDialog.interactable = true;
            confirmationDialog.blocksRaycasts = true;
        }

        public void ShowConfirmationDialog(DialogType dialogType, double capitalCost, double populationLoss, double populationCost) {
            result = DialogResult.None;

            switch (dialogType) {
                case DialogType.EmptyCell:
                    throw new System.ArgumentException("A population loss must not be supplied where the dialog type is EmptyCell");
                case DialogType.PopulatedCell:
                    textField.text = "Construct a reservoir in populated area, costing " + Money.FormatMoney(capitalCost) + ".\n" + populationLoss.ToString("F0") + " residents will be relocated, costing " + Money.FormatMoney(populationCost) +  "\nThe total cost will be " + Money.FormatMoney(capitalCost + populationCost) + ".\nDo you wish to continue?";
                    break;
                case DialogType.Upgrade:
                    throw new System.ArgumentException("A population loss must not be supplied where the dialog type is Upgrade");
                default:
                    throw new System.ArgumentNullException("Confirmation dialog type is null");
            }

            confirmationDialog.alpha = 1f;
            confirmationDialog.interactable = true;
            confirmationDialog.blocksRaycasts = true;
        }

        private void HideConfirmationDialog() {
            confirmationDialog.alpha = 0f;
            confirmationDialog.interactable = false;
            confirmationDialog.blocksRaycasts = false;
        }

        public void YesButton() {
            result = DialogResult.Yes;
            HideConfirmationDialog();
        }

        public void NoButton() {
            result = DialogResult.No;
            HideConfirmationDialog();
        }
    }
}


