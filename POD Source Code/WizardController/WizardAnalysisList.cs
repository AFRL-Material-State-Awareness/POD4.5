using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using POD.Docks;
using POD.Wizards;

namespace POD
{
    /// <summary>
    /// A pair of Wizard/Dock pairs that can be indexed from the wizard, dock, wizard source or
    /// wizard source's name. Pairs with different source types should NOT be mixed and matched in a pair.
    /// </summary>
    [Serializable]
    public class WizardDockList : List<WizardDockPair>
    {
        public WizardDockPair Project;

        /// <summary>
        /// A pair of all the sources being used. Assumes each source has a unique name.
        /// </summary>
        internal List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                
                foreach (WizardDockPair pair in this)
                {
                    if(pair.Source != null)
                        names.Add(pair.Source.Name);
                }

                return names;
            }
        }

        /// <summary>
        /// Get dock pair by wizard source's name.
        /// </summary>
        /// <param name="myName">name of wizard's source</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[string myName]
        {
            get
            {
                int index = Names.IndexOf(myName);

                if (index >= 0)
                {
                    return this[index];
                }
                else if (Project != null && (Project.Source.Name == myName || myName == Globals.UndefinedProjectName))
                    return Project;

                return null;
            }
        }

        /*/// <summary>
        /// Get Wizard/Dock pair by analysis wizard's source
        /// </summary>
        /// <param name="myAnalysis">analysis that is the wizard's source</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[Analysis myAnalysis]
        {
            get
            {
                int index = -1;

                for (int i = 0; i < this.Count; i++)
                {
                    if (myAnalysis == this[i].Analysis)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    return this[index];

                return null;
            }
        }
        /// <summary>
        /// Get Wizard/Dock pair by project wizard's source
        /// </summary>
        /// <param name="myProject">project that is the wizard's source</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[Project myProject]
        {
            get
            {
                int index = -1;

                for (int i = 0; i < this.Count; i++)
                {
                    if (myProject == this[i].Project)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    return this[index];

                return null;
            }
        }*/

        /// <summary>
        /// Get Wizard/Dock pair by dock
        /// </summary>
        /// <param name="myDock">dock that the wizard is in</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[WizardDock myDock]
        {
            get
            {
                int index = -1;

                if (Project != null && myDock == Project.Dock)
                    return Project;

                for (int i = 0; i < this.Count; i++)
                {
                    if (myDock == this[i].Dock)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    return this[index];

                return null;
            }
        }

       

        /// <summary>
        /// Get Wizard/Dock pair by the wizard
        /// </summary>
        /// <param name="myWizard">wizard that the sitting in the dock</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[Wizard myWizard]
        {
            get
            {
                int index = -1;

                if (Project != null && myWizard == Project.Wizard)
                    return Project;

                for (int i = 0; i < this.Count; i++)
                {
                    if (myWizard == this[i].Wizard)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    return this[index];

                return null;
            }
        }

        /// <summary>
        /// Get Wizard/Dock pair by the wizard
        /// </summary>
        /// <param name="myWizard">wizard that the sitting in the dock</param>
        /// <returns>the corresponding Wizard/Dock pair</returns>
        public WizardDockPair this[WizardSource mySource]
        {
            get
            {
                int index = -1;

                if (Project != null && mySource == Project.Source)
                    return Project;

                for (int i = 0; i < this.Count; i++)
                {
                    if (mySource == this[i].Source)
                    {
                        index = i;
                        break;
                    }
                }

                if (index >= 0)
                    return this[index];

                return null;
            }
        }

    }
}
