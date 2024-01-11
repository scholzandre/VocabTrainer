public enum IconType {
    Edit,
    Save,
    Delete,
    Cancel,
    Approve,
    Undo,
    Redo
}

public static class IconTypeStrings {
    public const string Edit = "🖉";
    public const string Save = "💾";
    public const string Delete = "🗑";
    public const string Cancel = "🗙";
    public const string Approve = "✓";
    public const string Undo = "↶";
    public const string Redo = "↷";
}

namespace VocabTrainer.Models {
    internal class ButtonIcons {
        public static string GetIconString(IconType icon) {
            switch (icon) {
                case IconType.Edit:
                    return IconTypeStrings.Edit;
                case IconType.Save:
                    return IconTypeStrings.Save;
                case IconType.Delete:
                    return IconTypeStrings.Delete;
                case IconType.Cancel:
                    return IconTypeStrings.Cancel; 
                case IconType.Approve:
                    return IconTypeStrings.Approve;
                case IconType.Undo:
                    return IconTypeStrings.Approve;
                case IconType.Redo:
                    return IconTypeStrings.Approve;
                default:
                    return string.Empty;
            }
        }
    }
}
