using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;

namespace SWSAnalyzer
{
    [UserRepositoryItem("RegisterYmDateEdit")]
    public class RepositoryItemYmDateEdit : RepositoryItemDateEdit
    {
        static RepositoryItemYmDateEdit()
        {
            RegisterYmDateEdit();
        }
        public const string CustomEditName = "YmDateEdit";

        public RepositoryItemYmDateEdit()
        {
        }

        public override string EditorTypeName
        {
            get
            {
                return CustomEditName;
            }
        }

        public static void RegisterYmDateEdit()
        {
            Image img = null;
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomEditName, typeof(YmDateEdit), typeof(RepositoryItemYmDateEdit), typeof(YmDateEditViewInfo), new YmDateEditPainter(), true, img));
        }

        public override void Assign(RepositoryItem item)
        {
            BeginUpdate();
            try
            {
                base.Assign(item);
                RepositoryItemYmDateEdit source = item as RepositoryItemYmDateEdit;
                if (source == null) return;
                //
            }
            finally
            {
                EndUpdate();
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CalendarTimeProperties)).BeginInit();
            // 
            // RepositoryItemYmDateEdit
            // 
            this.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.Mask.EditMask = "y";
            this.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.Mask.UseMaskAsDisplayFormat = true;
            this.VistaCalendarViewStyle = ((DevExpress.XtraEditors.VistaCalendarViewStyle)((DevExpress.XtraEditors.VistaCalendarViewStyle.YearView | DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView)));
            ((System.ComponentModel.ISupportInitialize)(this.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
    }

    [ToolboxItem(true)]
    public class YmDateEdit : DateEdit
    {
        static YmDateEdit()
        {
            RepositoryItemYmDateEdit.RegisterYmDateEdit();
        }

        public YmDateEdit()
        {
            this.Properties.Mask.EditMask = "y";
            this.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.Properties.VistaCalendarViewStyle = ((DevExpress.XtraEditors.VistaCalendarViewStyle)((DevExpress.XtraEditors.VistaCalendarViewStyle.YearView | DevExpress.XtraEditors.VistaCalendarViewStyle.YearsGroupView)));
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemYmDateEdit Properties
        {
            get
            {
                return base.Properties as RepositoryItemYmDateEdit;
            }
        }

        public override string EditorTypeName
        {
            get
            {
                return RepositoryItemYmDateEdit.CustomEditName;
            }
        }

        protected override PopupBaseForm CreatePopupForm()
        {
            return new YmDateEditPopupForm(this);
        }
    }

    public class YmDateEditViewInfo : DateEditViewInfo
    {
        public YmDateEditViewInfo(RepositoryItem item) : base(item)
        {
        }
    }

    public class YmDateEditPainter : ButtonEditPainter
    {
        public YmDateEditPainter()
        {
        }
    }

    public class YmDateEditPopupForm : PopupDateEditForm
    {
        public YmDateEditPopupForm(YmDateEdit ownerEdit) : base(ownerEdit)
        {
        }
    }
}
