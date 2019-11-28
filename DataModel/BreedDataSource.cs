using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using Windows.Data.Json;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Storage;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FriendsWithPaws.Data
{
    
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class BreedDataCommon : FriendsWithPaws.Common.BindableBase
    {
        internal  static Uri _baseUri = new Uri("ms-appx:///");

        public BreedDataCommon(String uniqueId, String title, String shortTitle, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._shortTitle = shortTitle;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _shortTitle = string.Empty;
        public string ShortTitle
        {
            get { return this._shortTitle; }
            set { this.SetProperty(ref this._shortTitle, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;

        public Uri ImagePath
        {
            get
            {
                return new Uri(BreedDataCommon._baseUri, this._imagePath); 
            }
        } 

        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(BreedDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public string GetImageUri()
        {
            return _imagePath;
        }
    }

    /// <summary>
    /// Recipe item data model.
    /// </summary>
    public class BreedDataItem : BreedDataCommon
    {
        public BreedDataItem()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }
        
        public BreedDataItem(String uniqueId, String title, String shortTitle, String imagePath, String description, ObservableCollection<string> breed_details, BreedDataGroup group)
            : base(uniqueId, title, shortTitle, imagePath)
        {
            this._description = description;
            this._breed_details = breed_details;
            this._group = group;
        }

        
        
        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ObservableCollection<string> _breed_details;
        public ObservableCollection<string> Breed_details
        {
            get { return this._breed_details; }
            set { this.SetProperty(ref this._breed_details, value); }
        }
    
        private BreedDataGroup _group;
        public BreedDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        private ImageSource _tileImage;
        private string _tileImagePath;

        public Uri TileImagePath
        {
            get
            {
                return new Uri(BreedDataCommon._baseUri, this._tileImagePath);
            }
        }
        
        public ImageSource TileImage
        {
            get
            {
                if (this._tileImage == null && this._tileImagePath != null)
                {
                    this._tileImage = new BitmapImage(new Uri(BreedDataCommon._baseUri, this._tileImagePath));
                }
                return this._tileImage;
            }
            set
            {
                this._tileImagePath = null;
                this.SetProperty(ref this._tileImage, value);
            }
        }

        public void SetTileImage(String path)
        {
            this._tileImage = null;
            this._tileImagePath = path;
            this.OnPropertyChanged("TileImage");
        }
    }

    
    public class BreedDataGroup : BreedDataCommon
    {
        public BreedDataGroup()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }
        
        public BreedDataGroup(String uniqueId, String title, String shortTitle, String imagePath, String description)
            : base(uniqueId, title, shortTitle, imagePath)
        {
        }

        private ObservableCollection<BreedDataItem> _items = new ObservableCollection<BreedDataItem>();
        public ObservableCollection<BreedDataItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<BreedDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _groupImage;
        private string _groupImagePath;  

        public ImageSource GroupImage
        {
            get
            {
                if (this._groupImage == null && this._groupImagePath != null)
                {
                    this._groupImage = new BitmapImage(new Uri(BreedDataCommon._baseUri, this._groupImagePath));
                }
                return this._groupImage;
            }
            set
            {
                this._groupImagePath = null;
                this.SetProperty(ref this._groupImage, value);
            }
        }

        public int BooksCount
        {
            get
            {
                return this.Items.Count; 
            } 
        } 

        public void SetGroupImage(String path)
        {
            this._groupImage = null;
            this._groupImagePath = path;
            this.OnPropertyChanged("GroupImage");
        }
    }

    
    public sealed class BreedDataSource
    {
        

        private static BreedDataSource _breedDataSource = new BreedDataSource();
        
        private ObservableCollection<BreedDataGroup> _allGroups = new ObservableCollection<BreedDataGroup>();
        public ObservableCollection<BreedDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<BreedDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _breedDataSource.AllGroups;
        }

        public static BreedDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _breedDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static BreedDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _breedDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        
        public static async Task LoadDataAsync()
        {
            
            var file = await Package.Current.InstalledLocation.GetFileAsync("Data\\Dogs.txt");
            var result = await FileIO.ReadTextAsync(file);
         
            
            var breeds = JsonArray.Parse(result);

            
            CreateBreedsAndBreedGroups(breeds);
        }

        private static void CreateBreedsAndBreedGroups(JsonArray array)
        {
            foreach (var item in array)
            {
                var obj = item.GetObject();
                BreedDataItem breed = new BreedDataItem();
                BreedDataGroup group = null;

                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;

                    switch (key)
                    {
                        case "key":
                            breed.UniqueId = val.GetNumber().ToString();
                            break;
                        case "title":
                            breed.Title = val.GetString();
                            break;
                        case "shortTitle":
                            breed.ShortTitle = val.GetString();
                            break;
                        case "description":
                            breed.Description = val.GetString();
                            break;
                        case "breed_details":
                            var breed_details = val.GetArray();
                            var list = (from i in breed_details select i.GetString()).ToList();
                            breed.Breed_details = new ObservableCollection<string>(list);
                            break;
                        case "backgroundImage":
                            breed.SetImage(val.GetString());
                            break;
                        case "tileImage":
                            breed.SetTileImage(val.GetString());
                            break;
                        case "group":
                            var breedGroup = val.GetObject();

                            IJsonValue groupKey;
                            if (!breedGroup.TryGetValue("key", out groupKey))
                                continue;

                            group = _breedDataSource.AllGroups.FirstOrDefault(c => c.UniqueId.Equals(groupKey.GetString()));

                            if (group == null)
                                group = CreateBreedGroup(breedGroup);

                            breed.Group = group;
                            break;
                    }
                }

                if (group != null)
                    group.Items.Add(breed);
            }
        }
        
        private static BreedDataGroup CreateBreedGroup(JsonObject obj)
        {
            BreedDataGroup group = new BreedDataGroup();

            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;

                switch (key)
                {
                    case "key":
                        group.UniqueId = val.GetString();
                        break;
                    case "title":
                        group.Title = val.GetString();
                        break;
                    case "shortTitle":
                        group.ShortTitle = val.GetString();
                        break;
                    case "description":
                        group.Description = val.GetString();
                        break;
                    case "backgroundImage":
                        group.SetImage(val.GetString());
                        break;
                    case "groupImage" :
                        group.SetGroupImage(val.GetString());
                        break; 
                }
            }

            _breedDataSource.AllGroups.Add(group);
            return group;
        }
    }
}
