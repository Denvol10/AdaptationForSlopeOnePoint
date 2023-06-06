using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AdaptationForSlopeOnePoint.Infrastructure;

namespace AdaptationForSlopeOnePoint.ViewModels
{
    internal class MainWindowViewModel : Base.ViewModel
    {
        private RevitModelForfard _revitModel;

        internal RevitModelForfard RevitModel
        {
            get => _revitModel;
            set => _revitModel = value;
        }

        #region Заголовок

        private string _title = "Адаптация под уклон 1 точка";

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        #endregion

        #region Линия на поверхности

        private string _roadLineElemIds1;

        public string RoadLineElemIds1
        {
            get => _roadLineElemIds1;
            set => Set(ref _roadLineElemIds1, value);
        }

        #endregion

        #region Команды

        #region Получение линии на поверхности дороги

        public ICommand GetRoadLine { get; }

        private void OnGetRoadLineCommandExecuted(object parameter)
        {
            RevitCommand.mainView.Hide();
            RevitModel.GetRoadLine1();
            RoadLineElemIds1 = RevitModel.RoadLineElemIds1;
            RevitCommand.mainView.ShowDialog();
        }

        private bool CanGetRoadLineCommandExecute(object parameter)
        {
            return true;
        }

        #endregion

        #endregion


        #region Конструктор класса MainWindowViewModel
        public MainWindowViewModel(RevitModelForfard revitModel)
        {
            RevitModel = revitModel;

            #region
            GetRoadLine = new LambdaCommand(OnGetRoadLineCommandExecuted, CanGetRoadLineCommandExecute);

            #endregion
        }

        public MainWindowViewModel()
        { }
        #endregion
    }
}
