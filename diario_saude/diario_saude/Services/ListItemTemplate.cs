using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using System;

namespace diario_saude.Services
{
    public class ListItemTemplate : ReactiveObject
    {
        public string Label { get; }
        public Type ModelType { get; }
        public StreamGeometry? Icon { get; }

        public ListItemTemplate(Type type, string iconKey)
        {
            ModelType = type;
            Label = type.Name.Replace("PageViewModel", "");
            Application.Current!.TryFindResource(iconKey, out var icon);
            Icon = (StreamGeometry)icon!;
        }

        public ListItemTemplate(Type type)
        {
            ModelType = type;
            Label = type.Name.Replace("PageViewModel", "");
        }
    }
}
