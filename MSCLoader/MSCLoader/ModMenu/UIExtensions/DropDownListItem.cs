///Heavily modified Unity UI extensions (old ass version) (BSD3 license)
using System;

namespace MSCLoader
{
    [Serializable]
    internal class DropDownListItem
    {
        [SerializeField]
        private string _caption;
        /// <summary>
        /// Caption of the Item
        /// </summary>
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                if (OnUpdate != null)
                    OnUpdate();
            }
        }

        [SerializeField]
        private Sprite _image;
        /// <summary>
        /// Image component of the Item
        /// </summary>
        public Sprite Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                if (OnUpdate != null)
                    OnUpdate();
            }
        }

        [SerializeField]
        private bool _isDisabled;
        /// <summary>
        /// Is the Item currently enabled?
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return _isDisabled;
            }
            set
            {
                _isDisabled = value;
                if (OnUpdate != null)
                    OnUpdate();
            }
        }

        [SerializeField]
        private string _id;
        ///<summary>
        ///ID exists so that an item can have a caption and a value like in traditional windows forms. Ie. an item may be a student's name, and the ID can be the student's ID number
        ///</summary>
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public Action OnSelect; //action to be called when this item is selected

        internal Action OnUpdate; //action to be called when something changes.  

        /// <summary>
        /// Constructor for Drop Down List panelItems
        /// </summary>
        /// <param name="caption">Caption for the item </param>
        /// <param name="inId">ID of the item </param>
        /// <param name="image"></param>
        /// <param name="disabled">Should the item start disabled</param>
        /// <param name="onSelect">Action to be called when this item is selected</param>
        public DropDownListItem(string caption = "", string inId = "", Sprite image = null, bool disabled = false, Action onSelect = null)
        {
            _caption = caption;
            _image = image;
            _id = inId;
            _isDisabled = disabled;
            OnSelect = onSelect;
        }
    }
}