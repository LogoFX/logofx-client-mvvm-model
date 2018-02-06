using Solid.Patterns.Memento;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        interface ISnapshot
        {
            void Restore(EditableModel<T> model);
        }

        sealed class SnapshotMementoAdapter : IMemento<EditableModel<T>>
        {
            private readonly ISnapshot _snapshot;

            internal SnapshotMementoAdapter(EditableModel<T> model)
            {
#if NETSTANDARD2_0
                _snapshot = new ComplexSnapshot(model);
#else
                _snapshot = new HierarchicalSnapshot(model);
#endif
            }

            public IMemento<EditableModel<T>> Restore(EditableModel<T> target)
            {
                IMemento<EditableModel<T>> inverse = new SnapshotMementoAdapter(target);
                _snapshot.Restore(target);
                return inverse;
            }
        }
    }
}
