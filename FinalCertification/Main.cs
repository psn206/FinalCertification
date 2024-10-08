using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace FinalCertification
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            FilteredElementCollector rooms = new FilteredElementCollector(doc);
            rooms.OfCategory(BuiltInCategory.OST_Rooms);

            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .OfType<ViewPlan>()
                .ToList();

            if (rooms.Count() == 0)
            {
                TaskDialog.Show("Ошибка", "Нет помещений!");
                return Result.Succeeded;
            }

            Transaction ts = new Transaction(doc, "Добавление номера комнаты!");
            ts.Start();

            foreach (Room room in rooms)
            {
                foreach (var view in views)
                {
                    BoundingBoxXYZ bounding = room.get_BoundingBox(null);
                    XYZ center = (bounding.Max + bounding.Min) * 0.5;
                    UV centerRoom = new UV(center.X, center.Y);
                    RoomTag roomTag = doc.Create.NewRoomTag(new LinkElementId(room.Id), centerRoom, view.Id);
                }
            }

            ts.Commit();
            return Result.Succeeded;
        }
    }
}
