using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POD.Data;
using POD.Controls;

namespace POD.Wizards
{
    public partial class NewHitMissRangeForm : Form
    {
        double _maxFlawValue = 1.0;

        

        public double MaxFlawValue
        {
            get { return _maxFlawValue; }
            set { _maxFlawValue = value; }
        }

        double _minFlawValue = 0.0;

        private PODChartNumericUpDown _activeBefore;
        private List<string> operators;
        private List<string> operatorsWithEmpties;
        private List<string> lastSpecimen;
        private List<string> lastInstrument;
        private List<string> specimens;
        private List<string> specimensWithEmpties;
        private List<string> flawMins;
        private List<string> flawMaxs;
        //private List<string> instruments;
        //private List<string> instrumentsWithEmpties;
        //private List<string> responseMins;
        //private List<string> responseMaxs;
        private List<string> specimenUnits;
        //private List<string> instrumentUnits;
        private List<string> specimenUnitsWithEmpties;
        //private List<string> instrumentUnitsWithEmpties;
        private List<string> flawMinsWithEmpties;
        private List<string> flawMaxsWithEmpties;
        //private List<string> responseMinsWithEmpties;
        //private List<string> responseMaxsWithEmpties;
        
        public double MinFlawValue
        {
            get { return _minFlawValue; }
            set { _minFlawValue = value; }
        }

        public NewHitMissRangeForm()
        {
            InitializeComponent();

            UpdateDropLists();

            MaxFlaw.PartType = ChartPartType.CrackMax;
            MinFlaw.PartType = ChartPartType.CrackMin;

            SetInitialValues();

            MaxFlaw.TooltipForNumeric = "Maximum flaw size of the data set." + Environment.NewLine + "Can be changed later.";
            MinFlaw.TooltipForNumeric = "Minimum flaw size of the data set." + Environment.NewLine + "Can be changed later.";
      
            UpdateSpecimenFromComboBoxes();
            UpdateValuesFromControls();
        }

        

        private void SetInitialValues()
        {
            if(operators.Count > 0)
                operatorComboBox.Text = operatorsWithEmpties[0];

            if (specimens.Count > 0)
                specimenComboBox.Text = specimensWithEmpties[0];

            if (specimenUnitsWithEmpties.Count > 0)
                SpecimenUnitComboBox.Text = specimenUnitsWithEmpties[0];
        }

        private void UpdateSpecimenFromComboBoxes()
        {
            var spec = specimenComboBox.Text;
            var specIndex = specimens.IndexOf(spec);
            var emptyIndex = specimensWithEmpties.IndexOf(spec);
            
            if (specIndex > -1)
            {
                double min = 0.0;
                double max = 1.0;

                if (!double.TryParse(flawMaxs[specIndex], out _maxFlawValue))
                    MaxFlawValue = max;

                if (!double.TryParse(flawMins[specIndex], out _minFlawValue))
                    MinFlawValue = min;

                MaxFlaw.Value = Convert.ToDecimal(MaxFlawValue);
                MinFlaw.Value = Convert.ToDecimal(MinFlawValue);

                SpecimenUnitComboBox.Text = specimenUnits[specIndex];
            }
            else if (emptyIndex > -1)
            {
                double min = 0.0;
                double max = 1.0;

                if (!double.TryParse(flawMaxsWithEmpties[emptyIndex], out _maxFlawValue))
                    MaxFlawValue = max;

                if (!double.TryParse(flawMinsWithEmpties[emptyIndex], out _minFlawValue))
                    MinFlawValue = min;

                MaxFlaw.Value = Convert.ToDecimal(MaxFlawValue);
                MinFlaw.Value = Convert.ToDecimal(MinFlawValue);

                SpecimenUnitComboBox.Text = specimenUnitsWithEmpties[emptyIndex];
            }
            
        }

        private bool UpdateValuesFromControls()
        {
            MaxFlawValue = Convert.ToDouble(MaxFlaw.Value);
            MinFlawValue = Convert.ToDouble(MinFlaw.Value);

            if (MinFlawValue > MaxFlawValue)
            {
                var temp = MaxFlawValue;
                MaxFlawValue = MinFlawValue;
                MinFlawValue = temp;

                MaxFlaw.Value = Convert.ToDecimal(MaxFlawValue);
                MinFlaw.Value = Convert.ToDecimal(MinFlawValue);
            }

            return true;
        }
        
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            

            Close();
        }

        private void FillFromMRUFiles()
        {
            
            operators = new List<string>();
            lastSpecimen = new List<string>();
            lastInstrument = new List<string>();

            specimens = new List<string>();
            flawMins = new List<string>();
            flawMaxs = new List<string>();
            specimenUnits = new List<string>();

         
            operatorsWithEmpties = new List<string>();
            specimensWithEmpties = new List<string>();
            specimenUnitsWithEmpties = new List<string>();
            flawMinsWithEmpties = new List<string>();
            flawMaxsWithEmpties = new List<string>();
        
            GetListFromSplittableMRUFileWithoutEmpties(Globals.PODv4OperatorFile, operators, lastSpecimen, lastInstrument, null);
            GetListFromSplittableMRUFileWithoutEmpties(Globals.PODv4SpecimenFile, specimens, flawMins, flawMaxs, specimenUnits);
         
            GetListFromSplittableMRUFile(Globals.PODv4OperatorFile, operatorsWithEmpties, null, null, null);
            GetListFromSplittableMRUFile(Globals.PODv4SpecimenFile, specimensWithEmpties, flawMinsWithEmpties, flawMaxsWithEmpties, specimenUnitsWithEmpties);              
        }

        private static void GetListFromSplittableMRUFileWithoutEmpties(string fileName, List<string> itemIndex0, List<string> itemIndex1, List<string> itemIndex2, List<string> itemIndex3)
        {           
            var operatorLines = Globals.GetMRUListWithoutEmpties(fileName, true, "|");
            ProcessListFromSplittableMRUFile(itemIndex0, itemIndex1, itemIndex2, itemIndex3, operatorLines);
        }

        private static void GetListFromSplittableMRUFile(string fileName, List<string> itemIndex0, List<string> itemIndex1, List<string> itemIndex2, List<string> itemIndex3)
        {
            var operatorLines = Globals.GetMRUList(fileName);
            ProcessListFromSplittableMRUFile(itemIndex0, itemIndex1, itemIndex2, itemIndex3, operatorLines);
        }

        private static void ProcessListFromSplittableMRUFile(List<string> itemsIndex0, List<string> itemsIndex1, List<string> itemsIndex2, List<string> itemsIndex3, List<string> lines)
        {
            var listOfValues = new List<List<string>>();
            foreach (var line in lines)
            {
                var values = line.Split(new char[] { '|', }, StringSplitOptions.None).ToList();

                listOfValues.Add(values);
            }

            foreach (var list in listOfValues)
            {
                AddItemToList(itemsIndex0, list, 0);
                AddItemToList(itemsIndex1, list, 1);
                AddItemToList(itemsIndex2, list, 2);
                AddItemToList(itemsIndex3, list, 3);
            }
        }

        private static void AddItemToList(List<string> listToAddTo, List<string> listToAddFrom, int index)
        {
            if (listToAddTo != null)
                if (listToAddFrom.Count > index)
                    listToAddTo.Add(listToAddFrom[index]);
                else
                    listToAddTo.Add("");
        }

        private void UpdateDropLists()
        {
            FillFromMRUFiles();

            operatorComboBox.Items.Clear();
            specimenComboBox.Items.Clear();
            SpecimenUnitComboBox.Items.Clear();

            operatorComboBox.Items.AddRange(operators.ToArray());
            specimenComboBox.Items.AddRange(specimens.ToArray());
            SpecimenUnitComboBox.Items.AddRange(specimenUnits.Distinct().ToArray());
            
        }

        private void UpdateMRUFiles()
        {
            var operatorName = operatorComboBox.Text.Trim();
            var specimenSetName = specimenComboBox.Text.Trim();
            var specimenUnit = SpecimenUnitComboBox.Text.Trim();

            var operatorLine = operatorName + "|" + specimenSetName + "|" + " ";
            var specimenSetLine = specimenSetName + "|" + MinFlawValue +"|" + MaxFlawValue + "|" + specimenUnit;


            Globals.UpdateMRUList(operatorLine, Globals.PODv4OperatorFile, true, "|");
            Globals.UpdateMRUList(specimenSetLine, Globals.PODv4SpecimenFile, true, "|");
        }

        private void MinimumNumeric_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Numeric_Entered(object sender, EventArgs e)
        {
            var numeric = sender as PODChartNumericUpDown;

            if (numeric != null && numeric != _activeBefore)
            {
                numeric.NumericUpDown.Select(0, numeric.NumericUpDown.Text.Length);
                _activeBefore = numeric;
            }
        }

        private void Specimen_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateSpecimenFromComboBoxes();
        }

        private void Numeric_Validating(object sender, CancelEventArgs e)
        {
            CheckValidation(sender);
        }

        private void CheckValidation(object sender)
        {
            var numeric = sender as PODChartNumericUpDown;

            if (numeric != null)
            {
                if (numeric.Value <= 0.0m)
                {
                    errorProvider1.SetError(numeric, "Value must be greater than zero.");
                }
                else
                {
                    errorProvider1.SetError(numeric, null);
                }
            }
        }

        private void Numeric_Validated(object sender, EventArgs e)
        {
            CheckValidation(sender);
        }

        protected override void OnClosing(CancelEventArgs e)
        {

            CheckValidation(MinFlaw);
            CheckValidation(MaxFlaw);

            if (MinFlaw.Value <= 0.0m || MaxFlaw.Value <= 0.0m)
            {
                e.Cancel = true;
                return;
            }

            UpdateValuesFromControls();

            UpdateMRUFiles();

            base.OnClosing(e);
        }


        public string Operator
        {
            get
            {
                return operatorComboBox.Text.Trim();
            }
        }

        public string SpecimenSet
        {
            get
            {
                return specimenComboBox.Text.Trim();
            }
        }

        public string SpecimenUnits
        {
            get
            {
                return SpecimenUnitComboBox.Text.Trim();
            }
        }
    }
}
