using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmbeSimp.Core.IO
{
    /// <summary>
    /// A wrapper class for accessing Wikipedia pages stored in an SQL Server relational database
    /// </summary>
    public class WikiDataProvider
    {
        /// <summary>
        /// Retrieving the page by ID
        /// </summary>
        /// <param name="id">the id of the page to be retrieved</param>
        /// <returns>The fetched Wikipedia page</returns>
        public Page GetPageByID(int id)
        {
            using (var ctx = new WikipediaEntities())
            {
                return ctx.Pages.Where(p => p.PageID == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Retrieving all the wiki pages whose ID is within the given range
        /// </summary>
        /// <param name="fromID">starting ID of the range</param>
        /// <param name="toID">ending ID of the range</param>
        /// <returns>list of pages</returns>
        public List<Page> GetPageByIDInRange(int fromID, int toID)
        {
            using (var ctx = new WikipediaEntities())
            {
                return ctx.Pages.Where(p => p.PageID > fromID && p.PageID <= toID).ToList();
            }
        }
    }
}
