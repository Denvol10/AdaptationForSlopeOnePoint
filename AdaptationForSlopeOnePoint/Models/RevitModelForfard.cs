﻿using System;
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
using AdaptationForSlopeOnePoint.Models;
using System.IO;

namespace AdaptationForSlopeOnePoint
{
    public class RevitModelForfard
    {
        private UIApplication Uiapp { get; set; } = null;
        private Application App { get; set; } = null;
        private UIDocument Uidoc { get; set; } = null;
        private Document Doc { get; set; } = null;

        public RevitModelForfard(UIApplication uiapp)
        {
            Uiapp = uiapp;
            App = uiapp.Application;
            Uidoc = uiapp.ActiveUIDocument;
            Doc = uiapp.ActiveUIDocument.Document;
        }

        #region Семейства адаптивных профилей
        public List<FamilyInstance> AdaptiveProfiles { get; set; }

        private string _adaptiveProfileElemIds;
        public string AdaptiveProfileElemIds
        {
            get => _adaptiveProfileElemIds;
            set => _adaptiveProfileElemIds = value;
        }

        public void GetAdaptiveProfiles()
        {
            AdaptiveProfiles = RevitGeometryUtils.GetFamilyInstances(Uiapp, out _adaptiveProfileElemIds);
        }

        #endregion

        #region Проверка на то существуют профили в модели
        public bool IsFamilyInstancesExistInModel(string elemIdsInSettings)
        {
            var elemIds = RevitGeometryUtils.GetIdsByString(elemIdsInSettings);

            return RevitGeometryUtils.IsElemsExistInModel(Doc, elemIds, typeof(FamilyInstance));
        }
        #endregion

        #region Проверка на то существуют линии поверхности дороги в модели
        public bool IsLinesExistInModel(string elemIdsInSettings)
        {
            var elemIds = RevitGeometryUtils.GetIdsByString(elemIdsInSettings);

            return RevitGeometryUtils.IsElemsExistInModel(Doc, elemIds, typeof(DirectShape));
        }
        #endregion

        #region Получение профилей из settings
        public void GetFamilyInstancesBySettings(string elemIdsInSettings)
        {
            var elemIds = RevitGeometryUtils.GetIdsByString(elemIdsInSettings);
            AdaptiveProfiles = RevitGeometryUtils.GetFamilyInstancesById(Doc, elemIds);
        }
        #endregion

        #region Линия на поверхности
        public List<Line> RoadLines1 { get; set; }

        private string _roadLineElemIds1;
        public string RoadLineElemIds1
        {
            get => _roadLineElemIds1;
            set => _roadLineElemIds1 = value;
        }

        public void GetRoadLine1()
        {
            RoadLines1 = RevitGeometryUtils.GetRoadLines(Uiapp, out _roadLineElemIds1);
        }
        #endregion

        #region Получение линии на поверхности из settings
        public void GetRoadLinesBySettings(string elemIdsInSettings)
        {
            var elemIds = RevitGeometryUtils.GetIdsByString(elemIdsInSettings);
            RoadLines1 = RevitGeometryUtils.GetCurvesById(Doc, elemIds).OfType<Line>().ToList();
        }
        #endregion

        #region Перенос точки ручки формы на линию
        public void MoveShapeHandlePoint()
        {
            using (Transaction trans = new Transaction(Doc, "Адаптация Профиля Под Уклон"))
            {
                trans.Start();
                foreach (var profile in AdaptiveProfiles)
                {
                    XYZ intersectionPoint = RevitGeometryUtils.GetIntersectPoint(Doc, profile, RoadLines1);
                    ReferencePoint shapeHandlePoint = RevitGeometryUtils.GetShapeHandlePoints(Doc, profile).First();
                    shapeHandlePoint.Position = intersectionPoint;
                }
                trans.Commit();
            }
        }
        #endregion

    }
}
