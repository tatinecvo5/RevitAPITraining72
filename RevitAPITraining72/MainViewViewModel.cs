using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining72
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        public List<FamilySymbol> TitleBlocks { get; }
        public List<View> Views { get; }
        public FamilySymbol SelectedTitleBlock { get; set; }
        public View SelectedView { get; set; }
        public DelegateCommand CreateLists { get; }
        public int ListCount { get; set; }
        public string TextDesignedBy { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;

            TitleBlocks = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .Cast<FamilySymbol>()
                .ToList();

            Views = new FilteredElementCollector(_doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_Views)
                //.OfClass(typeof(ViewPlan))
                .Cast<View>()
                .ToList();

            SelectedTitleBlock = TitleBlocks[0];
            SelectedView = Views[0];
            ListCount = 1;
            TextDesignedBy = "Ввведите текст";
            CreateLists = new DelegateCommand(OnCreateLists);

        }

        private void OnCreateLists()
        {
            using (var ts = new Transaction(_doc, "Add lists"))
            {
                ts.Start();
                for (int i = 0; i < ListCount; i++)
                {
                    ElementId viewPlanId = (SelectedView as View).Duplicate(ViewDuplicateOption.WithDetailing);
                    ViewSheet lst = ViewSheet.Create(_doc, SelectedTitleBlock.Id);
                    Parameter param = lst.LookupParameter("Designed By");
                    param.Set(TextDesignedBy);
                    lst.Name = "Приложение 7";
                    XYZ xYZ = new XYZ(-1, 1, 0);
                    Viewport.Create(_doc, lst.Id, viewPlanId, xYZ);
                }
                ts.Commit();
            }
            RaiseCloseRequest();
            return;
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
