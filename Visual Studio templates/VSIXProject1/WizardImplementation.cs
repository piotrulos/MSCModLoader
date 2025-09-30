using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;


namespace VSIXProject1
{
    class WizardImplementation : IWizard
    {
        private Form1 inputForm;
      /*  private string managedPath;
        private string authorName;
        private string modName;
        private string modVersion;*/

        // This method is called before opening any item that
        // has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        // This method is only called for item templates,
        // not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.
        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
                // Display a form to the user. The form collects
                // input for the custom message.
                inputForm = new Form1();
                //    inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.ShowDialog();
                /*    managedPath = UserInputForm.managedPath;
                    modName = UserInputForm.modName;
                    authorName = UserInputForm.modAuthor;
                    modVersion = UserInputForm.modVersion;*/



                //Set variables
                replacementsDictionary.Add("$managedPath$", Form1.managedPath);
                replacementsDictionary.Add("$modName$", Form1.modName);
                replacementsDictionary.Add("$modAuthor$", Form1.modAuthor);
                replacementsDictionary.Add("$modVersion$", Form1.modVersion);

                //Add references
                replacementsDictionary.Add("$assPM$", Form1.assPM);
                replacementsDictionary.Add("$assCS$", Form1.assCS);
                replacementsDictionary.Add("$asscInput$", Form1.asscInput);
                replacementsDictionary.Add("$assUI$", Form1.assUI);
                replacementsDictionary.Add("$assHarmony$", Form1.assHarmony);
                replacementsDictionary.Add("$assCSf$", Form1.assCSf);

                //Mod Functions
                replacementsDictionary.Add("$setOnMenuLoad$", Form1.setOnMenuLoad);
                replacementsDictionary.Add("$setOnNewGame$", Form1.setOnNewGame);
                replacementsDictionary.Add("$setPreLoad$", Form1.setPreLoad);
                replacementsDictionary.Add("$setOnLoad$", Form1.setOnLoad);
                replacementsDictionary.Add("$setPostLoad$", Form1.setPostLoad);
                replacementsDictionary.Add("$setOnSave$", Form1.setOnSave);
                replacementsDictionary.Add("$setOnGUI$", Form1.setOnGUI);
                replacementsDictionary.Add("$setUpdate$", Form1.setUpdate);
                replacementsDictionary.Add("$setFixedUpdate$", Form1.setFixedUpdate);

                //Post-build stuff
                replacementsDictionary.Add("$advScript$", Form1.advMiniDlls);
                replacementsDictionary.Add("$modsPath$", Form1.modsPath);
                replacementsDictionary.Add("$abPath$", "NONE");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // This method is only called for item templates,
        // not for project templates.
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }


}
