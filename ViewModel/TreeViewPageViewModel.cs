using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NBT_Parser.Class;
using NBT_Studio.Model;

namespace NBT_Studio.ViewModel
{
    public class TreeViewPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private ObservableCollection<NbtNode> _nodes = [];
        public ObservableCollection<NbtNode> Nodes
        {
            get => _nodes;
            set => SetField(ref _nodes, value);
        }

        public TreeViewPageViewModel()
        {
            _nodes.Add(new NbtNode(BuildNbtTree()));
            return;

            static NbtTag BuildNbtTree()
            {
                var builder = new NbtTagBuilder(true);
                var pos = builder.IntArray("position", [-64, 128, 255]);
                var id = builder.Int("id", 42);
                var entity = builder.Dictionary(null, [pos, id]);
                var entities = builder.List("entities", [entity, entity, entity]);
                var dim = builder.String("dimension", "minecraft:overworld");
                return builder.Dictionary("root", [entities, dim]);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
