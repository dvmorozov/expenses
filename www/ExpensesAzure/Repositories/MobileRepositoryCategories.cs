using System;
using System.Linq;
using Newtonsoft.Json;
using SocialApps.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Globalization;

namespace SocialApps.Repositories
{
    public partial class MobileRepository
    {
        //  Cached category data.
        private class SelectedEstimatedCategories1
        {
            //  Fields are initialized to avoid warnings. Really they are filled during deserialization.
            public DateTime Date = DateTime.Now;
            public bool ShortList = false;
            public EstimatedCategoriesByUser3_Result[] List = null;
        }

        //  The second version of cached data.
        //  https://github.com/dvmorozov/expenses/issues/38
        private class SelectedEstimatedCategories2
        {
            public DateTime Date;
            public bool ShortList;
            public EstimatedCategoriesByUser4_Result[] List = null;
        }

        private const string SelectEstimatedCategoriesPrefix = "SelectEstimatedCategories";

        public bool CategoriesLoaded()
        {
            return (_session[SelectEstimatedCategoriesPrefix] != null);
        }

        //  Return cached categories.
        public EstimatedCategoriesByUser4_Result[] GetCategories()
        {
            return ((SelectedEstimatedCategories2)_session[SelectEstimatedCategoriesPrefix]).List;
        }

        //  https://action.mindjet.com/task/14509395
        private void DeleteCachedCategories(Guid userId)
        {
            try
            {
                _session[SelectEstimatedCategoriesPrefix] = null;
                DeleteBlobs(userId, SelectEstimatedCategoriesPrefix);
            }
            catch
            {
            }
        }

        //  https://github.com/dvmorozov/expenses/issues/47
        private void DeleteCategory(Guid userId, int categoryId)
        {
            //  Clean cache.
            DeleteCachedCategories(userId);

            (
                 from ec in _db.ExpensesCategories
                 join e in _db.Expenses on ec.ExpenseID equals e.ID
                 where ec.CategoryID == categoryId && e.DataOwner == userId
                 select ec
             ).ToList().ForEach(ec => ec.CategoryID = 0);

            _db.SaveChanges();
        }

        public EstimatedCategoriesByUser4_Result[] SelectEstimatedCategories(DateTime date, Guid userId, bool? shortList)
        {
            bool _shortList = (shortList != null && (bool)shortList);

            //  Check if data are already in session. Neglects possible date changing.
            if (_session[SelectEstimatedCategoriesPrefix] != null &&
                ((SelectedEstimatedCategories2)_session[SelectEstimatedCategoriesPrefix]).ShortList == _shortList)
                return ((SelectedEstimatedCategories2)_session[SelectEstimatedCategoriesPrefix]).List;

            var fileName = SelectEstimatedCategoriesPrefix + "_" + date.Year + "_" + date.Month + "_" + date.Day + "_" + _shortList;

            if (!DownloadText(userId, out string json, fileName))
            {
                //  Remove all possibly existing old files.
                DeleteCachedCategories(userId);

                //  https://action.mindjet.com/task/144796941
                var categoryList = _db.EstimatedCategoriesByUser4(date.Year, date.Month, date.Day, userId, shortList != null && (bool)shortList).ToArray();

                //  https://action.mindjet.com/task/14509395
                //  Converting to JSON.
                json = JsonConvert.SerializeObject(new SelectedEstimatedCategories2 { Date = date, ShortList = _shortList, List = categoryList });
                UploadText(userId, json, fileName);
            }

            //  Cache data in session.
            try
            {
                _session[SelectEstimatedCategoriesPrefix] = JsonConvert.DeserializeObject<SelectedEstimatedCategories2>(json);
            }
            catch {
                //  Conversion from structure of type 1 to structure of type 2.
                var v1Cache = JsonConvert.DeserializeObject<SelectedEstimatedCategories1>(json);
                var v2List = (
                    from v1Item in v1Cache.List
                    select new EstimatedCategoriesByUser4_Result
                    {
                        Currency = null,
                        EncryptedName = v1Item.EncryptedName,
                        Estimation = v1Item.Estimation,
                        ID = v1Item.ID,
                        Limit = v1Item.Limit,
                        NAME = v1Item.NAME,
                        Total = v1Item.Total
                    }).ToArray();

                _session[SelectEstimatedCategoriesPrefix] = new SelectedEstimatedCategories2
                {
                    Date = v1Cache.Date,
                    ShortList = v1Cache.ShortList,
                    List = v2List
                };
            }
            return ((SelectedEstimatedCategories2)_session[SelectEstimatedCategoriesPrefix]).List;
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
