﻿using System.Collections.ObjectModel;
using Project.Model.Reflection.Model;

namespace Project.ViewModel
{
    public abstract class MetadataViewModel : BaseViewModel
    {
        #region Constructor

        internal MetadataViewModel()
        {
            Child = new ObservableCollection<MetadataViewModel>
            {
                null
            };

            _isExpanded = false;
            WasBuilt = false;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public ObservableCollection<MetadataViewModel> Child { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded)
                    return;

                _isExpanded = value;
                if (WasBuilt)
                {
                    return;
                }

                BuildMyself();
            }
        }

        #endregion

        internal readonly TypeMetadata TypeMetadata;
        private bool _isExpanded;
        protected bool WasBuilt;

        protected abstract void BuildMyself();

        internal string GetModifier(AccessLevel? accessLevel)
        {
            string modifier = "";
            switch (accessLevel)
            {
                case AccessLevel.IsPublic:
                    modifier = "public ";
                    break;
                case AccessLevel.IsProtected:
                    modifier = "protected ";
                    break;
                case AccessLevel.IsProtectedInternal:
                    modifier = "internal ";
                    break;
                case AccessLevel.IsPrivate:
                    modifier = "private ";
                    break;
            }

            return modifier;
        }


        
    }
}