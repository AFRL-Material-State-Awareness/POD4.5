using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    [Serializable]
    public class PODObject : SkillTypeHolder
    {
        PODObjectTypeEnum _objectType;

        public PODObjectTypeEnum ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }
        //global variable that is initialize in the POD object class (used to store the data analysis type such as hit/miss, ahat, custom etc
        AnalysisDataTypeEnum _analysisDataType;

        public AnalysisDataTypeEnum AnalysisDataType
        {
            get { return _analysisDataType; }
            set { _analysisDataType = value; }
        }

        public WizardEnum WizardType
        {
            get
            {
                WizardEnum type = WizardEnum.None;

                switch (ObjectType)
                {
                    case PODObjectTypeEnum.Analysis:
                        switch(AnalysisDataType)
                        {
                            case AnalysisDataTypeEnum.AHat:
                                switch(AnalysisType)
                                {
                                    case AnalysisTypeEnum.Full:
                                        type = AHatFull();
                                        break;
                                    case AnalysisTypeEnum.Quick:
                                        type = AHatQuick();
                                        break;
                                    case AnalysisTypeEnum.Undefined:
                                    case AnalysisTypeEnum.None:
                                        type = WizardEnum.None;
                                        break;
                                }
                                break;
                            case AnalysisDataTypeEnum.HitMiss:
                                switch (AnalysisType)
                                {
                                    case AnalysisTypeEnum.Full:
                                        type = HitMissFull();
                                        break;
                                    case AnalysisTypeEnum.Quick:
                                        type = HitMissQuick();
                                        break;
                                    case AnalysisTypeEnum.Undefined:
                                    case AnalysisTypeEnum.None:
                                        type = WizardEnum.None;
                                        break;
                                }
                                break;
                            case AnalysisDataTypeEnum.Undefined:
                            case AnalysisDataTypeEnum.None:
                            default:
                                type = WizardEnum.None;
                                break;
                        }
                        break;
                    case PODObjectTypeEnum.Project:
                        type = Project();
                        break;
                    case PODObjectTypeEnum.RunAllAnalysis:
                        type = RunHitMissAll();
                        break;
                    case PODObjectTypeEnum.None:
                    default: 
                        type = WizardEnum.None;
                        break;
                }

                return type;
            }
        }

        private WizardEnum Project()
        {
            WizardEnum type = WizardEnum.None;

            switch (SkillLevel)
            {
                case SkillLevelEnum.Quick:
                    type = WizardEnum.ProjectQuick;
                    break;
                case SkillLevelEnum.Tutorial:
                    type = WizardEnum.ProjectTutorial;
                    break;
                case SkillLevelEnum.Training:
                    type = WizardEnum.ProjectTraining;
                    break;
                case SkillLevelEnum.Normal:
                case SkillLevelEnum.Advanced:
                    type = WizardEnum.ProjectNormal;
                    break;
                case SkillLevelEnum.None:
                default:
                    type = WizardEnum.None;
                    break;
            }

            return type;
        }
        private WizardEnum RunHitMissAll()
        {
            return WizardEnum.HitMissSolveAllAnalyses;
        }

        private WizardEnum HitMissQuick()
        {
            WizardEnum type = WizardEnum.None;
            
            switch (SkillLevel)
            {
                case SkillLevelEnum.Tutorial:
                    type = WizardEnum.QuickHitMissTutorial;
                    break;
                case SkillLevelEnum.Training:
                case SkillLevelEnum.Normal:
                case SkillLevelEnum.Advanced:
                    type = WizardEnum.QuickHitMissBeginner;
                    break;
                case SkillLevelEnum.None:
                default:
                    type = WizardEnum.None;
                    break;
            }

            return type;
        }

        private WizardEnum HitMissFull()
        {
            WizardEnum type = WizardEnum.None;

            switch (SkillLevel)
            {
                case SkillLevelEnum.Tutorial:
                    type = WizardEnum.HitMissTutorial;
                    break;
                case SkillLevelEnum.Training:
                    type = WizardEnum.HitMissBeginner;
                    break;
                case SkillLevelEnum.Normal:
                    type = WizardEnum.HitMissIntermediate;
                    break;
                case SkillLevelEnum.Advanced:
                    type = WizardEnum.HitMissExpert;
                    break;
                case SkillLevelEnum.None:
                default:
                    type = WizardEnum.None;
                    break;
            }

            return type;
        }

        private WizardEnum AHatQuick()
        {
            WizardEnum type = WizardEnum.None;

            switch (SkillLevel)
            {
                case SkillLevelEnum.Tutorial:
                    type = WizardEnum.QuickAHatTutorial;
                    break;
                case SkillLevelEnum.Training:
                case SkillLevelEnum.Normal:
                case SkillLevelEnum.Advanced:
                    type = WizardEnum.QuickAHatBeginner;
                    break;
                case SkillLevelEnum.None:
                default:
                    type = WizardEnum.None;
                    break;
            }

            return type;
        }

        private WizardEnum AHatFull()
        {
            WizardEnum type = WizardEnum.None;

            switch (SkillLevel)
            {
                case SkillLevelEnum.Tutorial:
                    type = WizardEnum.AHatTutorial;
                    break;
                case SkillLevelEnum.Training:
                    type = WizardEnum.AHatBeginner;
                    break;
                case SkillLevelEnum.Normal:
                    type = WizardEnum.AHatIntermediate;
                    break;
                case SkillLevelEnum.Advanced:
                    type = WizardEnum.AHatExpert;
                    break;
                case SkillLevelEnum.None:
                default:
                    type = WizardEnum.None;
                    break;
            }

            return type;
        }
    }
}
