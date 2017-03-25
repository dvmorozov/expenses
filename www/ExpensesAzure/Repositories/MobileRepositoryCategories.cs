using System;
using System.Linq;
using Newtonsoft.Json;
using SocialApps.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

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

        //  Returns cached categories.
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
                //  Removes all possibly existing old files.
                DeleteCachedCategories(userId);

                //  https://action.mindjet.com/task/144796941
                var categoryList =
                    ((shortList != null && (bool)shortList) ?
                    _db.EstimatedCategoriesByUser3(date.Year, date.Month, date.Day, userId, true)
                    :
                    _db.EstimatedCategoriesByUser3(date.Year, date.Month, date.Day, userId, false)).ToArray();

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
    }
}
