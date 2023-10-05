#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
#endregion

namespace RAA_QandA_231005
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // Your code goes here
            
            string schemeName = "Test1";

            ColorFillScheme colorfill = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ColorFillSchema)
                .Cast<ColorFillScheme>()
                .Where(x => x.Name.Equals(schemeName))
                .First();

            Level curLevel = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .Where(x => x.Name.Equals("Level 1"))
                .First();

            AreaScheme areaScheme = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_AreaSchemes)
                .Cast<AreaScheme>()
                .Where(x => x.Name.Equals("Rentable"))
                .First();

            using (Transaction t = new Transaction(doc))
            {
                Category areaCat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Areas);
                t.Start("create area plan");

                ViewPlan areaPlan = ViewPlan.CreateAreaPlan(doc, areaScheme.Id, curLevel.Id);

                areaPlan.SetColorFillSchemeId(areaCat.Id, colorfill.Id);

                List<ElementId> catList = areaPlan.SupportedColorFillCategoryIds().ToList();

                //ColorFillLegend curLegend = ColorFillLegend.Create(doc, areaPlan.Id, areaPlan.Category.Id, new XYZ(0, 0, 0));

                t.Commit();
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
