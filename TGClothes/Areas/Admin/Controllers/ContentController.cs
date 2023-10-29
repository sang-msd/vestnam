using Data.EF;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGClothes.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        private readonly ICategoryService _category;
        private readonly IContentService _content;

        public ContentController(ICategoryService category, IContentService content)
        {
            _category = category;
            _content = content;
        }

        // GET: Admin/Content
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            SetViewbag();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Content content)
        {
            if (ModelState.IsValid)
            {

            }
            SetViewbag();
            return View();
        }

        public ActionResult Edit(long id)
        {
            var content = _content.GetById(id);
            SetViewbag(content.CategoryId);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Content content)
        {
            if (ModelState.IsValid)
            {

            }
            SetViewbag(content.CategoryId);
            return View();
        }

        public void SetViewbag(long? selectedId = null)
        {
            ViewBag.CategoryId = new SelectList(_category.GetAll(), "Id", "Name", selectedId);
        }
    }
}