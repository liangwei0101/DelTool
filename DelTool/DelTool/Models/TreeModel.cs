using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelTool.Models
{
    public class TreeModel : INotifyPropertyChanged
    {
        private string _nodeName;
        public string NodeName
        {
            get { return _nodeName; }
            set
            {
                _nodeName = value;
                OnPropertyChanged("NodeName");
            }
        }

        private ObservableCollection<TreeModel> _nodes;
        public ObservableCollection<TreeModel> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                OnPropertyChanged("Nodes");
            }
        }

        private ResourcesType _type;
        public ResourcesType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
