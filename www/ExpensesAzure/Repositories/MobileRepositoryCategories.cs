using System;
using System.Linq;
using Newtonsoft.Json;
using SocialApps.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Globalization;
using System.Resources;

namespace SocialApps.Repositories
{
    public partial class MobileRepository
    {
        //  Cached category data.
        private class SelectedEstimatedCategories
        {
            public DateTime Date;
            public bool ShortList;
            public EstimatedCategoriesByUser3_Result[] List = null;
        }

        private const string SelectEstimatedCategoriesPrefix = "SelectEstimatedCategories";

        public bool CategoriesLoaded()
        {
            return (_session[SelectEstimatedCategoriesPrefix] != null);
        }

        //  Return cached categories.
        public EstimatedCategoriesByUser3_Result[] GetCategories()
        {
            return ((SelectedEstimatedCategories)_session[SelectEstimatedCategoriesPrefix]).List;
        }

        //  https://action.mindjet.com/task/14509395
        private void DeleteCachedCategories(Guid userId)
        {
            DeleteBlobs(userId, SelectEstimatedCategoriesPrefix);
            _session[SelectEstimatedCategoriesPrefix] = null;
        }

        public EstimatedCategoriesByUser3_Result[] SelectEstimatedCategories(DateTime date, Guid userId, bool? shortList)
        {
            bool _shortList = (shortList != null && (bool)shortList);

            //  Check if data are already in session. Neglects possible date changing.
            if (_session[SelectEstimatedCategoriesPrefix] != null &&
                ((SelectedEstimatedCategories)_session[SelectEstimatedCategoriesPrefix]).ShortList == _shortList)
                return ((SelectedEstimatedCategories)_session[SelectEstimatedCategoriesPrefix]).List;

            var fileName = SelectEstimatedCategoriesPrefix + "_" + date.Year + "_" + date.Month + "_" + date.Day + "_" + _shortList;

            string json;
            if (!DownloadText(userId, out json, fileName))
            {
                //  Remove all possibly existing old files.
                DeleteCachedCategories(userId);

                //  https://action.mindjet.com/task/144796941
                var categoryList = _db.EstimatedCategoriesByUser3(date.Year, date.Month, date.Day, userId, shortList != null && (bool)shortList).ToArray();

                //  https://action.mindjet.com/task/14509395
                //  Converting to JSON.
                json = JsonConvert.SerializeObject(new SelectedEstimatedCategories { Date = date, ShortList = _shortList, List = categoryList });
                UploadText(userId, json, fileName);
            }

            //  Cache data in session.
            _session[SelectEstimatedCategoriesPrefix] = JsonConvert.DeserializeObject<SelectedEstimatedCategories>(json);
            return ((SelectedEstimatedCategories)_session[SelectEstimatedCategoriesPrefix]).List;
        }

        public int? NewCategory(string name, double? limit, Guid userId, string encryptedName)
        {
            DeleteCachedCategories(userId);
            return _db.AddCategoryByUser(name, limit, userId, encryptedName).Single();
        }

        public void EditCategory(int id, string name, Guid userId, double? limit, string encryptedName)
        {
            DeleteCachedCategories(userId);

            var cat = _db.Categories.First(t => t.ID == id);
            cat.Name = name;
            cat.DataOwner = userId;
            cat.Limit = limit;
            cat.EncryptedName = encryptedName;

            var manager = ((IObjectContextAdapter)_db).ObjectContext.ObjectStateManager;
            manager.ChangeObjectState(cat, EntityState.Modified);
            _db.SaveChanges();
        }

        public CategoryModel GetCategory(int categoryId)
        {
            var category = _db.Categories.Single(t => t.ID == categoryId);
            var model = new CategoryModel
            {
                Name = category.Name.Trim(),
                Limit = category.Limit != null ? ((float)category.Limit).ToString(CultureInfo.InvariantCulture) : "",
                Id = categoryId,
                EncryptedName = category.EncryptedName
            };
            return model;
        }

        public void UpdateCategories(Guid userId, EncryptedList list)
        {
            foreach (var cat in list.List)
            {
                _db.UpdateCategoryByUser(cat.Id, cat.OpenText, cat.EncryptedText, userId);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/003838f7-d618-449e-836b-11b8a26669c2
        public void FillInitialCategories(Guid userId)
        {
            var catNames = Resources.Resources.AccountCategories.Split(',');

            foreach (var cat in catNames.OrderBy(t => t))
            {
                _db.Categories.Add(
                    new Categories
                    {
                        Name = cat,
                        DataOwner = userId
                    }
                );
                _db.SaveChanges();
            }
        }
    }
}
